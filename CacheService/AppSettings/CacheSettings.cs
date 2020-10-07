namespace CacheService.AppSettings
{
    public class CacheSettings
    {
        public string ConnectionString { get; set; }
        public string ServiceName { get; set; }
        public string ClientName { get; set; }
        public int ConnectTimeout { get; set; }
        public int DefaultExpireTime { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public int ConnectRetry { get; set; }
        public int AsyncTimeout { get; set; }
    }
}