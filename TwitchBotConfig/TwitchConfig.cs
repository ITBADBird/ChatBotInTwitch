using System;

namespace TwitchBotConfig
{
    public class TwitchConfig
    {
        static TwitchConfig() => config = new TwitchConfig();

        public static TwitchConfig config;
        public static TwitchConfig Instance => config;

        string twitcNicname;
        public string TwitcNicname => twitcNicname;

        string accessToken;
        public string AccessToken => accessToken;

        private TwitchConfig()
        {
            this.accessToken = "#######"; //токен вашего бота 
            this.twitcNicname = "#######"; //Название канала на который подключаемся 
            

        }

    }
}