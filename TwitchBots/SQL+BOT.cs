using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace TwitchBots
{
    class SQLConnect6
    {
        private SqlConnection sqlConnection = null;
        
        public void ConnectS()
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseTwitchBot"].ConnectionString); //Присваеваем подключение

            sqlConnection.Open(); //Открываем БД
            if (sqlConnection.State == ConnectionState.Open)//проверка подключения
            {
                Console.WriteLine("SQL_DB.Connect ");
            }
        }

        public string[] SQLINVENTORY(string SerchNickName) //Функция которая принемает переменную и возращает строковый массив
        {                       
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM TwitchInv WHERE NickName = '{SerchNickName}'",sqlConnection);
            SqlDataReader dataReader;
            dataReader = sqlCommand.ExecuteReader();
                        
            dataReader.Read();
            string[] SQLinventory = new string[]{ 
                $"{Convert.ToString(dataReader["inForest"])}",     //0
                $"{Convert.ToString(dataReader["DataTimeSet"])}",  //1
                $"{Convert.ToString(dataReader["inForest"])}",     //2
                $"{Convert.ToString(dataReader["Leaves"])}",       //3
                $"{Convert.ToString(dataReader["Sticks"])}",       //4
                $"{Convert.ToString(dataReader["Clay"])}",         //5
                $"{Convert.ToString(dataReader["Stones"])}",       //6
                $"{Convert.ToString(dataReader["flint"])}",        //7
                $"{Convert.ToString(dataReader["gunpowder"])}",    //8
                $"{Convert.ToString(dataReader["TimeInForest"])}", //9
            };                     
            dataReader.Close();
            return SQLinventory;
        }

        public string[] SQLAllUsersDBinForestTrue() //Собираем пользователейц в массив у которых TRUE в позиции inForest
        {
            SqlDataAdapter sqlCommand = new SqlDataAdapter($"SELECT NickName FROM TwitchInv WHERE inForest = 'True'", sqlConnection);
            DataTable dataTable = new DataTable(); 
            sqlCommand.Fill(dataTable);
            string[] result = new string[dataTable.Rows.Count];
            int i = 0;
            foreach (DataRow dr in dataTable.Rows)
            {
                result[i++] = dr[0].ToString();
            }
            return result;           
        }
        public void AddSQL(string NickNameSQL, string DateTimeH) //Добавляем нового пользователя в БД
        {
            SqlCommand command = new SqlCommand(
                $"INSERT INTO TwitchInv (NickName, DataTimeSet) VALUES ('{NickNameSQL}', '{DateTimeH}')", 
                sqlConnection);
            command.ExecuteNonQuery();
        }
        public int SerchSQL(string SerchNickName) //Поиск по нику и ответ в INT 1\0
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(
                $"SELECT NickName FROM TwitchInv WHERE NickName = '{SerchNickName}'",
                sqlConnection);
            DataSet dataSet = new DataSet();
            
            return dataAdapter.Fill(dataSet);
        }
        public void SQLChangeTime(string SerchNickName, string DateTimeH) //замена времени в БД у определённого пользователя
        {
            SqlCommand command = new SqlCommand(
                $"UPDATE TwitchInv SET DataTimeSet = '{DateTimeH}' WHERE NickName = '{SerchNickName}'",
                sqlConnection);
            command.ExecuteNonQuery();
        }
        public void SetWaitTime(string SerchNickName, string Time) // замена минут в БД у определённого пользователя
        {
            SqlCommand command = new SqlCommand(
                $"UPDATE TwitchInv SET TimeInForest = '{Time}' WHERE NickName = '{SerchNickName}'",
                sqlConnection);
            command.ExecuteNonQuery();
        }
        public void SetResources(string SerchNickName, string Leaves, string Sticks, string Clay, string Stones, string flint, string gunpowder)
        {
            SqlCommand command = new SqlCommand(
                $"UPDATE TwitchInv SET Leaves = '{Leaves}', Sticks = '{Sticks}', Clay = '{Clay}', Stones = '{Stones}', flint = '{flint}', gunpowder = '{gunpowder}', TimeInForest = 0 WHERE NickName = '{SerchNickName}'", 
                sqlConnection);
            command.ExecuteNonQuery();
        }
        public void SQLChangeInForest(string SerchNickName, int InForest) //замена времени в БД у определённого пользователя
        {
            SqlCommand command = new SqlCommand(
                $"UPDATE TwitchInv SET inForest = '{InForest}' WHERE NickName = '{SerchNickName}'",
                sqlConnection);
            command.ExecuteNonQuery();
        }
        public void DeleteSqlString() // Удаление строки в БД где 30 колличество дней
        {
            SqlCommand command = new SqlCommand(
                $"DELETE FROM  TwitchInv WHERE (GETDATE()-30 > cast(dataTimeSet  as date)) ",
                sqlConnection);
            command.ExecuteNonQuery();
        }
    }
}
