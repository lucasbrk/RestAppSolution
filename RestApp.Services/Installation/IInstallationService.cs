
namespace RestApp.Services.Installation
{
    public partial interface IInstallationService
    {
        void InstallData(string adminLoginName, string password, bool installTestData);
    }
}
