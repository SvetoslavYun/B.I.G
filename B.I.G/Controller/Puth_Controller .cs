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
            Update2(Puth);
        }

        public void Update2(puth Puth)
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
                        var commandString = "UPDATE puths SET adres=@Adres";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
                 new SQLiteParameter("@Adres", Puth.adres),
            });

            updateCommand.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    connection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка связь с сервером : " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void CreateEmptyPuthIfNotExists()
        {
            string query = "SELECT COUNT(*) FROM puths";

                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count == 0)
                    {
                        // Если таблица пуста, вставляем новую строку со значением ''
                        InsertEmptyPuth(connection);
                    }
                }
            connection.Close();

        }

        public void CreateEmptyPuthIfNotExists2()
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
                        string query = "SELECT COUNT(*) FROM puths";

       
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    // Если таблица пуста, вставляем новую строку со значением ''
                    InsertEmptyPuth(connection);
                }
            }
           transaction.Commit();
                    }

                    connection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка связь с сервером : " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void InsertEmptyPuth(SQLiteConnection connection)
        {
            string query = "INSERT INTO puths (adres) VALUES ('')";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
            
        }

    }
}
