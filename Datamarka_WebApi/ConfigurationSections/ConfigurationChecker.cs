namespace Datamarka_WebApi.ConfigurationSections
{
    public static class ConfigurationChecker
    {
        public static void ThrowIfConfigurationIsIncorrect(IConfiguration config)
        {
            var baseUrl = config.GetValue<string>("BaseUrl");
            if (baseUrl == null)
            {
                throw new ConfigurationException("BaseUrl is required, please add it as root-level field");
            }
        }
    }
}
