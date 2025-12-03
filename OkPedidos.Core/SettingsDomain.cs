using OkPedidos.Models.Models.Base;

namespace OkPedidos.Core
{
    public static class SettingsDomain
    {
        public static MySettingsModel Application { get; set; }
        public static JwtSettingsModel Token { get; set; }
        public static void Initialize(JwtSettingsModel Jwtsettings)
        {
            Application = new MySettingsModel
            {
                ApplicationName = "OkPedidos",
                TitleError = "Error",
                Directory = new()
            };

            Token = new()
            {
                Secret = Jwtsettings.Secret,
                ExpireSeconds = Jwtsettings.ExpireSeconds
            };

            string jsonPath = Path.Combine(AppContext.BaseDirectory, "SettingsDomain.json");
            string json = File.ReadAllText(jsonPath);
        }
    }
}
