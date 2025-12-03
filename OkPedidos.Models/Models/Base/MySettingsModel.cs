namespace OkPedidos.Models.Models.Base
{
    public class MySettingsModel
    {
        public string ApplicationName { get; set; }
        public string TitleError { get; set; }
        public DirectorySettingsModel Directory { get; set; }
        public JwtSettingsModel JwtSettings { get; set; }

    }

    public class DirectorySettingsModel
    {
        public static string DirTemplateHtml { get => "TemplateHtml"; }
        public static string DirScriptJson { get => "Data/Scripts"; }
    }

    public class JwtSettingsModel
    {
        public string Secret { get; set; }
        public int ExpireSeconds { get; set; }
    }
}
