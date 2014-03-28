using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using RestApp.Core;
using RestApp.Core.Configuration;
using RestApp.Core.Data;
using RestApp.Core.Domain;
using RestApp.Core.Domain.Users;
using RestApp.Core.Domain.Roles;
using RestApp.Core.Domain.Localization;
using RestApp.Core.Domain.Logging;
using RestApp.Core.Infrastructure;
using RestApp.Services.Users;
using RestApp.Services.Localization;
using RestApp.Common.Utility;
using RestApp.Services.Security;
using RestApp.Core.Domain.Tables;

namespace RestApp.Services.Installation
{
    public partial class InstallationService : IInstallationService
    {
        #region Fields
       
        private readonly IRepository<Language> gLanguageRepository;       
        private readonly IRepository<User> gUserRepository;
        private readonly IRepository<Role> gRoleRepository;        
        private readonly IRepository<ActivityLogType> gActivityLogTypeRepository;
        private readonly IPermissionService gPermissionService;
        private readonly IWebHelper gWebHelper;
        private readonly IRepository<Table> gTableRepository;
        #endregion

        #region Ctor

        public InstallationService(IRepository<Language> languageRepository,            
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,            
            IRepository<ActivityLogType> activityLogTypeRepository, 
            IPermissionService permissionService,
            IWebHelper webHelper, 
            IRepository<Table> tableRepository)
        {          
            this.gLanguageRepository = languageRepository;           
            this.gUserRepository = userRepository;
            this.gRoleRepository = roleRepository;           
            this.gActivityLogTypeRepository = activityLogTypeRepository;
            this.gPermissionService = permissionService;
            this.gWebHelper = webHelper;
            this.gTableRepository = tableRepository;
        }

        #endregion

        #region Classes

        private class LocaleStringResourceParent : LocaleStringResource
        {
            public LocaleStringResourceParent(XmlNode localStringResource, string nameSpace = "")
            {
                Namespace = nameSpace;
                var resNameAttribute = localStringResource.Attributes["Name"];
                var resValueNode = localStringResource.SelectSingleNode("Value");

                if (resNameAttribute == null)
                {
                    throw new ApException("All language resources must have an attribute Name=\"Value\".");
                }
                var resName = resNameAttribute.Value.Trim();
                if (string.IsNullOrEmpty(resName))
                {
                    throw new ApException("All languages resource attributes 'Name' must have a value.'");
                }
                Name = resName;

                if (resValueNode == null || string.IsNullOrEmpty(resValueNode.InnerText.Trim()))
                {
                    IsPersistable = false;
                }
                else
                {
                    IsPersistable = true;
                    Value = resValueNode.InnerText.Trim();
                }

                foreach (XmlNode childResource in localStringResource.SelectNodes("Children/LocaleResource"))
                {
                    ChildLocaleStringResources.Add(new LocaleStringResourceParent(childResource, NameWithNamespace));
                }
            }
            public string Namespace { get; set; }
            public IList<LocaleStringResourceParent> ChildLocaleStringResources = new List<LocaleStringResourceParent>();

            public bool IsPersistable { get; set; }

            public string NameWithNamespace
            {
                get
                {
                    var newNamespace = Namespace;
                    if (!string.IsNullOrEmpty(newNamespace))
                    {
                        newNamespace += ".";
                    }
                    return newNamespace + Name;
                }
            }
        }

        private class ComparisonComparer<T> : IComparer<T>, IComparer
        {
            private readonly Comparison<T> _comparison;

            public ComparisonComparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return _comparison(x, y);
            }

            public int Compare(object o1, object o2)
            {
                return _comparison((T)o1, (T)o2);
            }
        }

        #endregion

        #region Utilities

        private void RecursivelyWriteResource(LocaleStringResourceParent resource, XmlWriter writer)
        {
            //The value isn't actually used, but the name is used to create a namespace.
            if (resource.IsPersistable)
            {
                writer.WriteStartElement("LocaleResource", "");

                writer.WriteStartAttribute("Name", "");
                writer.WriteString(resource.NameWithNamespace);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Value", "");
                writer.WriteString(resource.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            foreach (var child in resource.ChildLocaleStringResources)
            {
                RecursivelyWriteResource(child, writer);
            }

        }

        private void RecursivelySortChildrenResource(LocaleStringResourceParent resource)
        {
            ArrayList.Adapter((IList)resource.ChildLocaleStringResources).Sort(new InstallationService.ComparisonComparer<LocaleStringResourceParent>((x1, x2) => x1.Name.CompareTo(x2.Name)));

            foreach (var child in resource.ChildLocaleStringResources)
            {
                RecursivelySortChildrenResource(child);
            }

        }

        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "Español",
                LanguageCulture = "es-ES",
                Enabled = true,
            };
            gLanguageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'Spanish' language
            var language = gLanguageRepository.Table.Where(l => l.Name == "Español").Single();

            //save resoureces
            foreach (var filePath in System.IO.Directory.EnumerateFiles(gWebHelper.MapPath("~/App_Data/Localization/"), "*.res.xml", SearchOption.TopDirectoryOnly))
            {
                #region Parse resource files (with <Children> elements)
                //read and parse original file with resources (with <Children> elements)

                var originalXmlDocument = new XmlDocument();
                originalXmlDocument.Load(filePath);

                var resources = new List<LocaleStringResourceParent>();

                foreach (XmlNode resNode in originalXmlDocument.SelectNodes(@"//Language/LocaleResource"))
                    resources.Add(new LocaleStringResourceParent(resNode));

                resources.Sort((x1, x2) => x1.Name.CompareTo(x2.Name));

                foreach (var resource in resources)
                    RecursivelySortChildrenResource(resource);

                var sb = new StringBuilder();
                var writer = XmlWriter.Create(sb);
                writer.WriteStartDocument();
                writer.WriteStartElement("Language", "");

                writer.WriteStartAttribute("Name", "");
                writer.WriteString(originalXmlDocument.SelectSingleNode(@"//Language").Attributes["Name"].InnerText.Trim());
                writer.WriteEndAttribute();

                foreach (var resource in resources)
                    RecursivelyWriteResource(resource, writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();

                var parsedXml = sb.ToString();
                #endregion

                //now we have a parsed XML file (the same structure as exported language packs)
                //let's save resources
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, parsedXml);
            }

        }

        protected virtual void InstallUsersAndRole(string adminLoginName, string password)
        {
            var defaultRole = new Role
            {
                Name = "DefaultAdmin",
                Enabled = true,               
                DateCreateOn = DateTime.UtcNow,
                DateEditOn = DateTime.UtcNow,
            };
            
            foreach (var permission in gPermissionService.GetAllPermissionRecords())
            {
                defaultRole.PermissionRecords.Add(permission);
            }
            

            gRoleRepository.Insert(defaultRole);

            //admin user
            var defaultAdminUser = new User()
            {
                UserGuid = Guid.NewGuid(),
                Email = "admin@admin.com.ar", // default email
                Name = "Default Admin", // default name
                Password = EncryptionUtility.Encrypt(password), // default pass                
                Enabled = true,
                DateCreateOn = DateTime.UtcNow,
                DateEditOn = DateTime.UtcNow,
                LoginName = adminLoginName, // default loginName,              
                DocumentNumber = 99999999,
                LastActivityDateUtc= DateTime.UtcNow
            };

            defaultAdminUser.Roles.Add(defaultRole);

            gUserRepository.Insert(defaultAdminUser);
        }

        protected virtual void InstallSettings()
        {
            EngineContext.Current.Resolve<IConfigurationProvider<UserSettings>>()
                .SaveSettings(new UserSettings()
                { 
                    PasswordMinLength = 6                                      
                });
        }

        //protected virtual void InstallTable()
        //{
        //    var table = new Table
        //    {
        //        Code = 123,
        //        Name = "Table123",
        //        Account = "xxxxxxxxx",
        //        Enabled = true,
        //    };
        //    gTableRepository.Insert(table);
        //}
        #endregion

        #region Methods

        public virtual void InstallData(string adminLoginName, string password, bool installTestData)
        {
            InstallLanguages();
            InstallUsersAndRole(adminLoginName, password);            
            InstallLocaleResources();
            InstallSettings();
            
            if (installTestData)
            {
                //InstallTable();
            }
        }

        #endregion
    }
}