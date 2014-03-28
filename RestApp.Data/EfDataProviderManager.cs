using System;
using RestApp.Core;
using RestApp.Core.Data;
using RestApp.Data;

namespace RestApp.Services
{
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        public EfDataProviderManager(DataSettings settings):base(settings)
        {
        }

        public override IDataProvider LoadDataProvider()
        {

            var providerName = Settings.DataProvider;
            if (String.IsNullOrWhiteSpace(providerName))
                throw new ApException("Data Settings doesn't contain a providerName");

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    //return new SqlServerDataProvider();
                    throw new ApException(string.Format("Not supported sql server: {0}", providerName));
                case "mysql":
                    return new MySqlDataProvider();
                default:
                    throw new ApException(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }

    }
}
