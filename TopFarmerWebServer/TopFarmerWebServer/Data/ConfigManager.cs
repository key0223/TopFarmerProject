namespace TopFarmerWebServer.Data
{
    [Serializable]
    public class ServerConfig
    {
        public string dataPath;
        public string connectionString;
        public string redisConnectionString;
    }
    // 설정 파일
    public class ConfigManager
    {
        public static ServerConfig Config { get; private set; }
        public static void LoadConfig()
        {
            string text = File.ReadAllText("config.json");
            Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
        }
    }
}
