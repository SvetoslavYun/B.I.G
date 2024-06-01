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
            // Добавление имени файла базы данных к указанному пути
            string dbPath = Path.Combine(adres, "B.I.G.db");

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    var commandString = "UPDATE puths SET adres = @Adres"; // Предположим, что для обновления записи нужен идентификатор
                    SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
                    updateCommand.Parameters.AddRange(new SQLiteParameter[]
                    {
                new SQLiteParameter("@Adres", puth.adres),             
                    });

                    connection.Open();
                    updateCommand.ExecuteNonQuery();
                }

              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка подключения к серверу: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
