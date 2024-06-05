using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using B.I.G.Model;
using System.Windows;
using System.IO;


namespace B.I.G.Controller
{
    internal class Puth_Controller
    { private SQLiteConnection connection;

        public Puth_Controller()
        {
            // Получение строки подключения из файла конфигурации
            var connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            // Создание объекта подключения
            connection = new SQLiteConnection(connString);
        }

        public IEnumerable<string> GetAllPaths()
        {
            var commandString = "SELECT Adres FROM puths";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var adres = reader.GetString(0);
                yield return adres;
            }
            connection.Close();
        }

        public void Update(puth Puth)
        {
            var commandString = "UPDATE puths SET adres=@Adres";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
                 new SQLiteParameter("@Adres", Puth.adres),             
            });
            connection.Open();
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Update2(puth puth, string adres)
        {
            string dbPath = Path.Combine(MainWindow.puth, "B.I.G.db");

            if (!File.Exists(dbPath))
            {
               
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open(); // Открытие соединения с базой данных

                    using (SQLiteTransaction transaction = connection.BeginTransaction()) // Начало транзакции
                    {
                        var commandString = "UPDATE puths SET adres = @Adres"; // Предположим, что для обновления записи нужен идентификатор
                    SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
                    updateCommand.Parameters.AddRange(new SQLiteParameter[]
                    {
                new SQLiteParameter("@Adres", puth.adres),             
                    });
                  
                    updateCommand.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    connection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
               
            }
        }


    }
}
