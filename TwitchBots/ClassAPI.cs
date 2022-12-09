using System;
using TwitchLib.Api;
using TwitchBotConfig;
using System.Linq;
using System.Net;
using System.IO;

namespace TwitchBots
{
    public class ClassAPI
    {
        private static TwitchAPI api;
        TwitchConfig config = TwitchConfig.Instance;

        
        public void TwitchApIActived()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = config.TwitcNicname;
            api.Settings.AccessToken = config.AccessToken;
            Console.WriteLine("api.Connect");
        }

        public string[] CheckOnlineViewers() //возвращаем массив зрителей в онлайне
        {
            WebClient web = new WebClient();
            Stream stream = web.OpenRead($"http://tmi.twitch.tv/group/user/{config.TwitcNicname}/chatters"); //подключаемся к сылки где отображается данные канала по зрителям
            using (StreamReader reader = new StreamReader(stream))
            {
                
                string text = reader.ReadToEnd(); //читаем данные с ссылки
                //Console.WriteLine(text);
                //Тут мы убираем лишние элементы из ссылки
                string[] newArray = text.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("_links", "").Replace("chatter_count", "").Replace("chatters", "").Replace("broadcaster", "").Replace("vips", "").Replace("moderators", "").Replace("staff", "").Replace("admins", "").Replace("global_mods", "").Replace("viewers", "").Split(',', ':');
                newArray = newArray.Where(x => x != "").ToArray();//и убираем пустые элементы массива
                newArray = newArray.Where(x => x != "donationalerts_").
                    Where(x => x != "moobot").
                    Where(x => x != "soundalerts").
                    Where(x => x != "streamelements").ToArray();
                return newArray;
                
            }
        }
    }
}