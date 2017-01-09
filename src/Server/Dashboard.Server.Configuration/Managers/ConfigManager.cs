using Dashboard.Server.Configuration.Helpers;

namespace Dashboard.Server.Configuration.Managers
{
    public class ConfigManager
    {
        private static string settingsPath;

        public ConfigManager(string settingsPath)
        {
            ConfigManager.settingsPath = settingsPath;
        }

        public void SaveSettings(Models.ConfigModel settings)
        {
            JsonHelper.SaveJson(settings, settingsPath);
        }

        public Models.ConfigModel GetSettings()
        {
            return JsonHelper.GetModel(settingsPath, new Models.ConfigModel());
        }
    }
}