using System;
using TwitchBotConfig;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Threading;

namespace TwitchBots
{
    class ProgramBot
    {
        private static void Main()
        {
            TwitchConfig config = TwitchConfig.Instance;
            TwitchClient client = new TwitchClient();

            SQLConnect6 SQLConnecT = new SQLConnect6();
            SQLConnecT.ConnectS();      //подключили БД SQL          

            ClassAPI clasAPI = new ClassAPI();      // Создали клсса API
            //clasAPI.TwitchApIActived();
            
            client.Initialize(
                credentials: new ConnectionCredentials(
                    twitchUsername: config.TwitcNicname,

                    twitchOAuth: config.AccessToken),
                channel: config.TwitcNicname, // канал для получения сообщений чата
                autoReListenOnExceptions: !true);
            //client.OnLog += (s, arg) => Console.WriteLine(arg.Data); // информация подключения

            int ChatCount = 0; //Счётчик сообщений
            Random rand = new Random(); //Рандомизация чисел        

            //////////////////////////////////
            //////////////////////////////////
            async void UsersBackToForestAsync() // Проверка на наличие в бд и выдача ресурсов АСИНХРОННО
            {
                while (true)
                {
                    string[] SQLAllUsersDBinForestTrue = SQLConnecT.SQLAllUsersDBinForestTrue(); //Массив БД 
                    string[] CheckOnlineViewers = clasAPI.CheckOnlineViewers(); //Массив онлайн юзеров
                    SQLConnecT.DeleteSqlString();

                    for (int i = 0; i < SQLAllUsersDBinForestTrue.Length; i++)
                    {
                        Console.WriteLine(SQLAllUsersDBinForestTrue[i]);

                        for (int i2 = 0; i2 < CheckOnlineViewers.Length; i2++)
                        {
                            Console.WriteLine(CheckOnlineViewers[i2]);

                            if (SQLAllUsersDBinForestTrue[i].ToLower().Replace(" ", "") == CheckOnlineViewers[i2]) // Сравниваем пользователей
                            {
                                string[] arr = SQLConnecT.SQLINVENTORY(SQLAllUsersDBinForestTrue[i]); //полкчаем данные найденного пользователя
                                if (Convert.ToDateTime(arr[1]) <= DateTime.Now)// проверяем время пользователя
                                {
                                    int Leaves = rand.Next(1, Convert.ToInt32(arr[9]));
                                    int Sticks = rand.Next(1, Convert.ToInt32(arr[9]) / 2);
                                    int Clay = rand.Next(0, Convert.ToInt32(arr[9]) / 4);
                                    int Stones = rand.Next(0, Convert.ToInt32(arr[9]) / 3);
                                    int flint = rand.Next(0, Convert.ToInt32(arr[9]) / 6);
                                    int gunpowder = rand.Next(0, Convert.ToInt32(arr[9]) / 6);

                                    client.SendMessage(config.TwitcNicname, $"@{SQLAllUsersDBinForestTrue[i]} " +
                                        $"вы пришли из леса, в вашей сумке -> " +
                                        $"|| листьев {Leaves} " +
                                        $"|| палок {Sticks} " +
                                        $"|| глины {Clay} " +
                                        $"|| камня {Stones} " +
                                        $"|| кременя {flint} " +
                                        $"|| пороха {gunpowder} ");

                                    SQLConnecT.SQLChangeInForest(SQLAllUsersDBinForestTrue[i], 0);// Присваеваем значение False в inforest
                                                                                                  //Присваеваем новые ресурсы пользователю \/ \/ \/
                                    SQLConnecT.SetResources(SQLAllUsersDBinForestTrue[i],
                                        Convert.ToString(Convert.ToInt32(arr[3]) + Leaves),
                                        Convert.ToString(Convert.ToInt32(arr[4]) + Sticks),
                                        Convert.ToString(Convert.ToInt32(arr[5]) + Clay),
                                        Convert.ToString(Convert.ToInt32(arr[6]) + Stones),
                                        Convert.ToString(Convert.ToInt32(arr[7]) + flint),
                                        Convert.ToString(Convert.ToInt32(arr[8]) + gunpowder));

                                }
                            }
                        }
                    }                   
                    await Task.Delay(15*60*1000);
                }
            }           
            //////////////////////////////////
            //////////////////////////////////

            client.OnMessageReceived += (s, arg) =>
            {
                string channel = arg.ChatMessage.Channel; //канал к которому подключились
                string msg = arg.ChatMessage.Message; //Сообщение в чате 
                string displayName = arg.ChatMessage.DisplayName; // ник кто написал сообщение                               
                bool isSubscriber = arg.ChatMessage.IsSubscriber; // проверка на САБА TRUE FALSE
                bool IsVIP = arg.ChatMessage.IsVip; //Проверка на випа
                bool IsModer = arg.ChatMessage.IsModerator;
                
                Console.WriteLine($"channel: {channel}  displayName: {displayName}  msg: {msg}\n");

                if (ChatCount == 30)
                {
                    client.SendMessage(config.TwitcNicname, $"Подписывайся: https://www.twitch.tv/subs/itbadbird и я смогу сделать этого бота лучше");
                    ChatCount = 0; // да... да... грубо  сбрасываем счётчик
                }
                
                ChatCount++;        //считаем сообщение

                void NoVIPSUB()
                {
                    client.SendMessage(config.TwitcNicname, $" @{displayName} данная команда не доступнап SUBn't :D Подписывайся: https://www.twitch.tv/subs/{channel}");
                }  //Если пользователь не VIP или SUB
                void NotDB()
                {
                    client.SendMessage(config.TwitcNicname, $" @{displayName} Я не могу найти ваше имя в списке жителей, проверте ваш инвентарь что я смог вас внести (Команда: !Inv)");
                }     //Если пользователя нету в БД
                void inForestMessege(DateTime inForestDataDB)
                {
                    
                    TimeSpan time1 = inForestDataDB - DateTime.Now;
                    int time2 = (time1.Hours * 60) + time1.Minutes;
                    if (time2 <= 0)
                    {
                        client.SendMessage(config.TwitcNicname, $" @{displayName} До вашего возвращения остались считанные секунды");
                    }
                    else
                    {
                        client.SendMessage(config.TwitcNicname, $" @{displayName} Вы ещё в лесу и вернетесь через {time2} минут");
                    }
                }

                
                

                if ( (msg.ToUpper().Replace(" ", "") == "!INV") && (isSubscriber || IsVIP || IsModer)) //Посмотреть инвентарь зрителя 
                {
                    if (SQLConnecT.SerchSQL(displayName) == 0)
                    {
                        SQLConnecT.AddSQL(displayName, DateTime.Now.ToString($"yyyy-MM-dd HH:mm"));
                        string[] arr = SQLConnecT.SQLINVENTORY(displayName); // присваевает данные с массива 
                        client.SendMessage(config.TwitcNicname, $" Добро пожаловать @{displayName} В твоём инвентаре -> листьев:{arr[3]} || палок:{arr[4]} || глины:{arr[5]} || камня:{arr[6]} || кремния:{arr[7]} || пороха:{arr[8]}");
                    }
                    else
                    {
                        string[] arr = SQLConnecT.SQLINVENTORY(displayName); // присваевает данные с массива 
                        
                        if (Convert.ToBoolean(arr[0]))
                        {
                            inForestMessege(Convert.ToDateTime(arr[1])); // показывает что пользователь в лесу
                        }
                        else
                        {
                            client.SendMessage(config.TwitcNicname, $" @{displayName} В твоём инвентаре -> листьев:{arr[3]} || палок:{arr[4]} || глины:{arr[5]} || камня:{arr[6]} || кремния:{arr[7]} || пороха:{arr[8]}");
                        }                       
                    }
                }
                if ( (msg.ToUpper().Replace(" ", "") == "!INV") && !(isSubscriber || IsVIP || IsModer) )
                {
                    NoVIPSUB();
                }
                

                if ( (msg.ToUpper().Replace(" ", "") == "!FOREST") && (isSubscriber || IsVIP || IsModer) ) //Уйти в лес (СБОР РЕСУРСОВ)
                {
                    if (SQLConnecT.SerchSQL(displayName) == 1) //Проверка наличия в БД
                    {
                        string[] arr = SQLConnecT.SQLINVENTORY(displayName); // присваевает данные с массива                   
                        int RandomTime = rand.Next(10, 71); //Получаем рандомное время 
                        DateTime inForestData = DateTime.Now.AddMinutes(RandomTime);
                        //DateTime inForestDataDB = Convert.ToDateTime(arr[1]);

                        if (Convert.ToBoolean(arr[0]))
                        {
                                inForestMessege(Convert.ToDateTime(arr[1]));
                        }
                        else
                        {

                            SQLConnecT.SetWaitTime(displayName, RandomTime.ToString());
                            SQLConnecT.SQLChangeTime(displayName, inForestData.ToString($"yyyy-MM-dd HH:mm")); // Устанавливаем новое время активности
                            SQLConnecT.SQLChangeInForest(displayName, 1); //Переменная лес становится True
                            client.SendMessage(config.TwitcNicname, $" @{displayName} Вы ушли в лес и вернетесь через {RandomTime-1} минут");
                        }
                    }
                    else
                    {
                        NotDB();
                    }
                    
                }
                if ( (msg.ToUpper().Replace(" ", "") == "!FOREST") && !(isSubscriber || IsVIP || IsModer) )
                {
                    NoVIPSUB();
                }

                if ((msg.ToUpper().Replace(" ", "") == "!BOOOM") && (isSubscriber || IsVIP || IsModer))
                {
                    if (SQLConnecT.SerchSQL(displayName) == 1) //Проверка наличия в БД
                    {
                        string[] arr = SQLConnecT.SQLINVENTORY(displayName);
                        if (Convert.ToBoolean(arr[0]))
                        {
                            inForestMessege(Convert.ToDateTime(arr[1])); // показывает что пользователь в лесу
                        }
                        else
                        {
                            if (Convert.ToInt64(arr[3]) >= 10000 && Convert.ToInt64(arr[5]) >= 10000 && Convert.ToInt64(arr[6]) >= 10000 && Convert.ToInt64(arr[7]) >= 10000 && Convert.ToInt64(arr[8]) >= 10000)
                            {
                                SQLConnecT.SetResources(displayName, Convert.ToString(Convert.ToInt64(arr[3]) - 10000), arr[4], Convert.ToString(Convert.ToInt64(arr[5]) - 10000), Convert.ToString(Convert.ToInt64(arr[6]) - 10000), Convert.ToString(Convert.ToInt64(arr[7]) - 10000), Convert.ToString(Convert.ToInt64(arr[8]) - 10000));
                                client.SendMessage(config.TwitcNicname, $"{displayName} Взорвал чат!!! CurseLit CurseLit  Остались CurseLit CurseLit только CurseLit CurseLit эмоции  CurseLit CurseLit");
                                
                                client.SendMessage(config.TwitcNicname, $"/emoteonly");
                                Thread.Sleep(2*60*1000); //Спать 2 мин
                                client.SendMessage(config.TwitcNicname, $"/emoteonlyoff");
                            }
                            else 
                            {
                                client.SendMessage(config.TwitcNicname, $"{displayName} У Вас не хватает ресурсов, " +
                                   $"вам нужно: || 10k всех рессурсов ");
                            }
                        }
                    }
                    else
                    {
                        NotDB();
                    }
                }
                if ((msg.ToUpper().Replace(" ", "") == "!BOOOM") && !(isSubscriber || IsVIP || IsModer))
                {
                    NoVIPSUB();
                }    

                if (msg.ToUpper().Contains("!GREN") && (isSubscriber || IsVIP || IsModer)) //кинуть гранату
                {
                    if (SQLConnecT.SerchSQL(displayName) == 1)
                    {
                        string[] arr = SQLConnecT.SQLINVENTORY(displayName);
                        if (Convert.ToBoolean(arr[0]))
                        {
                            inForestMessege(Convert.ToDateTime(arr[1])); // показывает что пользователь в лесу
                        }
                        else 
                        {
                            
                            if (Convert.ToInt64(arr[3]) >= 200 && Convert.ToInt64(arr[5]) >= 70 && Convert.ToInt64(arr[6]) >= 50 && Convert.ToInt64(arr[7]) >= 25 && Convert.ToInt64(arr[8]) >= 15)
                            {
                                string[] arr2 = clasAPI.CheckOnlineViewers();
                                SQLConnecT.SetResources(displayName, Convert.ToString(Convert.ToInt64(arr[3]) - 200), arr[4], Convert.ToString(Convert.ToInt64(arr[5]) - 70), Convert.ToString(Convert.ToInt64(arr[6]) - 50), Convert.ToString(Convert.ToInt64(arr[7]) - 25), Convert.ToString(Convert.ToInt64(arr[8]) - 15));
                                int muteUser = rand.Next(30, 120);
                                if (msg.Contains("@"))
                                {
                                    string UserBOOM = msg.Remove(0, 5);
                                    //Console.WriteLine(UserBOOM);
                                    if (rand.Next(0, 100) > 35)
                                    {
                                        client.SendMessage(config.TwitcNicname, $" @{displayName} вы кинули гранату в {UserBOOM} || {UserBOOM} вам потребуется {muteUser} секунд на востановление");
                                        client.SendMessage(config.TwitcNicname, $"/timeout{UserBOOM} {muteUser}");
                                    }
                                    else
                                    {
                                        client.SendMessage(config.TwitcNicname, $" @{displayName} вы случайно роняете гранату CurseLit CurseLit CurseLit {UserBOOM} смеётся над вами! || @{displayName} вам потребуется {muteUser} секунд на востановление");
                                        client.SendMessage(config.TwitcNicname, $"/timeout {displayName} {muteUser}");
                                    }
                                }
                                else
                                {
                                    if (rand.Next(0, 100) > 30)
                                    {
                                        int randomUser = rand.Next(2, arr2.Length);
                                        client.SendMessage(config.TwitcNicname, $" @{displayName} вы кинули гранату в толпу и попали в @{arr2[randomUser]} || @{arr2[randomUser]} вам потребуется {muteUser} секунд на востановление");
                                        client.SendMessage(config.TwitcNicname, $"/timeout {arr2[randomUser]} {muteUser}");
                                    }
                                    else
                                    {
                                        client.SendMessage(config.TwitcNicname, $" @{displayName} вы случайно роняете гранату CurseLit CurseLit CurseLit || @{displayName} вам потребуется {muteUser} секунд на востановление");
                                        client.SendMessage(config.TwitcNicname, $"/timeout {displayName} {muteUser}");
                                    }
                                }
                            }
                            else
                            {
                                client.SendMessage(config.TwitcNicname, $"{displayName} У Вас не хватает ресурсов на создание гранаты, " +
                                    $"вам нужно: || 200 листьев || 70 глины || 50 камня || 25 кремня || 15 пороха");
                            }
                        }
                        
                    }
                    else
                    {
                        NotDB();
                    }
                }
                if (msg.ToUpper().Contains("!GREN") && !(isSubscriber || IsVIP || IsModer)) //кинуть гранату
                {
                    NoVIPSUB();
                }
                
                if (msg.ToUpper().Replace(" ", "") == "!DIS")
                {
                    client.SendMessage(config.TwitcNicname, $" Сылка на дискорд https://discord.gg/xR5r82XSvS");
                }
                

                
                
                if (msg[0] == '!' && msg[1] == '!' && (isSubscriber || IsVIP || IsModer)) // Команда сработает при !!
                {
                    if (isSubscriber || IsVIP)
                    {
                        using (SpeechSynthesizer synth = new SpeechSynthesizer()) //голосовое сообщение 
                        {
                            synth.SetOutputToDefaultAudioDevice();
                            synth.Volume = 100;
                            PromptBuilder builder = new PromptBuilder(new System.Globalization.CultureInfo("RUS"));
                            builder.AppendText(msg.Substring(2));
                            synth.Speak(builder);
                        }
                        //client.SendMessage(config.TwitcNicname, $" @{displayName} Кричит на весь чат :{msg.Substring(2)}");
                    }
                    else
                    {
                        client.SendMessage(config.TwitcNicname, $" @{displayName} так могут только Сабы и VIP пользователи");
                    }
                }
                //client.SendMessage(config.TwitcNicname, $" @{displayName} {msg}"); //Ответ на сообщение
            };
            

            client.Connect();
            UsersBackToForestAsync();
            Console.WriteLine("client.Connect");
            Console.ReadLine();
        }
        
    }
}
