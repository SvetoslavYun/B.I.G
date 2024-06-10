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
    internal class Log_Controller
    { private SQLiteConnection connection;

        public Log_Controller()
        {
            // Получение строки подключения из файла конфигурации
            var connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            // Создание объекта подключения
            connection = new SQLiteConnection(connString);
        }

        public IEnumerable<log> GetAllLogs()
        {
            var commandString = "SELECT * FROM logs ";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Process = reader.GetString(2);
                var Date = reader.GetDateTime(3);
                var Date2 = reader.GetDateTime(4);
                var Log = new log
                {
                    id = Id,
                    username = Username,
                    process = Process,
                    date = Date,
                    date2 = Date2
                };
                yield return Log;
            }
            connection.Close();
        }


        public void Insert(log Log)
        {
            var commandString = "INSERT INTO logs (username, process,date,date2) VALUES (@Username, @Process,@Date,@Date2)";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);
            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
                new SQLiteParameter("Username", Log.username),
                new SQLiteParameter("Process", Log.process),
                new SQLiteParameter("Date", Log.date),
                new SQLiteParameter("Date2", Log.date2),
            });

            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
            Insert2(Log);
        }


        public void Insert2(log Log)
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
                        // Определение SQL-команды для вставки данных
                        var commandString = "INSERT INTO logs (username, process, date, date2) VALUES (@Username, @Process, @Date, @Date2)";
                        using (SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection, transaction))
                        {
                            // Добавление параметров к команде
                            insertCommand.Parameters.AddRange(new SQLiteParameter[]
                            {
                        new SQLiteParameter("@Username", Log.username),
                        new SQLiteParameter("@Process", Log.process),
                        new SQLiteParameter("@Date", Log.date),
                        new SQLiteParameter("@Date2", Log.date2)
                            });

                            // Выполнение команды
                            insertCommand.ExecuteNonQuery();
                        }

                        // Фиксация транзакции
                        transaction.Commit();
                    }

                    connection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
              
            }
        }
   

        public void Delete(int id)
        {
            var commandString = "DELETE FROM logs WHERE(id = @Id)";
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
            deleteCommand.Parameters.AddWithValue("Id", id);
            connection.Open();
            deleteCommand.ExecuteNonQuery();
            connection.Close();
            Delete2(id);
        }


        public void Delete2(int id)
        {
            string dbPath = Path.Combine(MainWindow.puth, "B.I.G.db");

            if (!File.Exists(dbPath))
            {
                MessageBox.Show("Файл базы данных не найден: " + dbPath, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open(); // Открытие соединения с базой данных

                    using (SQLiteTransaction transaction = connection.BeginTransaction()) // Начало транзакции
                    {
                        var commandString = "DELETE FROM logs WHERE(id = @Id)";
                        SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
                        deleteCommand.Parameters.AddWithValue("Id", id);

                        deleteCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }

                    connection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
              
            }
        }



        public void DeleteAfterSixMonthsLog()
        {
            var commandString = "DELETE FROM logs WHERE date2 <= date('now', '-6 months')";
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
            connection.Open();
            deleteCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void DeleteAfterSixMonthsLog2()
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
                        var commandString = "DELETE FROM logs WHERE date2 <= date('now', '-6 months')";
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);          
            deleteCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }

                    connection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
              
            }
        }

        public IEnumerable<log> SearchUsername(string name)
        {
            connection.Open();
            if (name != "")
            {
                string selectQuery = "SELECT COUNT(*) FROM logs WHERE username = @Name";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@Name", name);
                    long existingRecords = (long)selectCommand.ExecuteScalar();
                    if (existingRecords == 0)
                    {
                        name = char.ToUpper(name[0]) + name.Substring(1);

                    }
                }
                name = char.ToUpper(name[0]) + name.Substring(1);
            }
            connection.Close();
            var commandString = "SELECT * FROM logs WHERE username LIKE @Name;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Process = reader.GetString(2);
                var Date = reader.GetDateTime(3);
                var Date2 = reader.GetDateTime(4);
                var Log = new log
                {
                    id = Id,
                    username = Username,
                    process = Process,
                    date = Date,
                    date2 = Date2
                };
                yield return Log;
            }
            connection.Close();
        }

        public IEnumerable<log> SearchDate(DateTime date)
        {
            var commandString = "SELECT * FROM logs WHERE date2 = @Date;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date);
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Process = reader.GetString(2);
                var Date = reader.GetDateTime(3);
                var Date2 = reader.GetDateTime(4);
                var Log = new log
                {
                    id = Id,
                    username = Username,
                    process = Process,
                    date = Date,
                    date2 = Date2
                };
                yield return Log;
            }
            connection.Close();
        }

        public IEnumerable<log> SearchNameDate(string name, DateTime date)
        {
            connection.Open();
            if (name != "")
            {
                string selectQuery = "SELECT COUNT(*) FROM logs WHERE username = @Name";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
                    long existingRecords = (long)selectCommand.ExecuteScalar();
                    if (existingRecords == 0)
                    {
                        name = char.ToUpper(name[0]) + name.Substring(1);

                    }
                }
                name = char.ToUpper(name[0]) + name.Substring(1);
            }
            connection.Close();
            var commandString = "SELECT * FROM logs WHERE username LIKE @Name and date2 = @Date;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date);
            getAllCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Process = reader.GetString(2);
                var Date = reader.GetDateTime(3);
                var Date2 = reader.GetDateTime(4);
                var Log = new log
                {
                    id = Id,
                    username = Username,
                    process = Process,
                    date = Date,
                    date2 = Date2
                };
                yield return Log;
            }
            connection.Close();
        }

        public IEnumerable<log> Search_Between_dates(DateTime startdate, DateTime enddate)
        {
            var commandString = "SELECT * FROM logs WHERE date2 BETWEEN @StartDate AND @EndDate ;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@StartDate", startdate);
            getAllCommand.Parameters.AddWithValue("@EndDate", enddate);
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Process = reader.GetString(2);
                var Date = reader.GetDateTime(3);
                var Date2 = reader.GetDateTime(4);
                var Log = new log
                {
                    id = Id,
                    username = Username,
                    process = Process,
                    date = Date,
                    date2 = Date2
                };
                yield return Log;
            }
            connection.Close();
        }

        public IEnumerable<log> Search_Name_Between_dates(string name, DateTime startdate, DateTime enddate)
        {
            connection.Open();
            if (name != "")
            {
                string selectQuery = "SELECT COUNT(*) FROM logs WHERE username = @Name";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
                    long existingRecords = (long)selectCommand.ExecuteScalar();
                    if (existingRecords == 0)
                    {
                        name = char.ToUpper(name[0]) + name.Substring(1);

                    }
                }
                name = char.ToUpper(name[0]) + name.Substring(1);
            } connection.Close();
            var commandString = "SELECT * FROM logs WHERE username LIKE @Name AND date2 BETWEEN @StartDate AND @EndDate ;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@StartDate", startdate);
            getAllCommand.Parameters.AddWithValue("@EndDate", enddate);
            getAllCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Process = reader.GetString(2);
                var Date = reader.GetDateTime(3);
                var Date2 = reader.GetDateTime(4);
                var Log = new log
                {
                    id = Id,
                    username = Username,
                    process = Process,
                    date = Date,
                    date2 = Date2
                };
                yield return Log;
            }
            connection.Close();
        }
    }
}
