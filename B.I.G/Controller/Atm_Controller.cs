using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using B.I.G.Model;
using Excel = Microsoft.Office.Interop.Excel;

namespace B.I.G.Controller
{
    internal class Atm_Controller
    { private SQLiteConnection connection;

        public Atm_Controller()
        {
            // Получение строки подключения из файла конфигурации
            var connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            // Создание объекта подключения
            connection = new SQLiteConnection(connString);
        }


        public void DeleteAfterSixMonthsLog()
        {
            var commandString = "DELETE FROM atms  WHERE date <= date('now', '-14 days')";
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
            connection.Open();
            deleteCommand.ExecuteNonQuery();
            connection.Close();
            DeleteAfterSixMonthsLog2();
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
                        var commandString = "DELETE FROM atms  WHERE date <= date('now', '-14 days')";
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


        public IEnumerable<atm> GetAllAtm(DateTime date)
        {
            var commandString = "SELECT * FROM atms WHERE date = @Date ORDER BY SUBSTR(Route, 1, INSTR(Route, '/') - 1) ASC, CAST(SUBSTR(Route, INSTR(Route, '/') + 1) AS INTEGER) ASC;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Route = reader.GetString(1);
                var Atmname = reader.GetString(2);
                var Name = reader.GetString(3);
                var Name2 = reader.GetString(4);
                var Date = reader.GetDateTime(5);
                var Circle = reader.GetString(6);
                var Atm = new atm
                {
                    id = Id,
                    route = Route,
                    atmname = Atmname,
                    name = Name,
                    name2 = Name2,
                    date = Date,
                    circle = Circle
                };
                yield return Atm;
            }
            connection.Close();
        }

        public void Insert(user_account User)
        {
            var commandString = "INSERT INTO user_accounts (username, password_hash,access, image) VALUES (@Username, @Password_hash,@Access, @Image)";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);            
            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
                new SQLiteParameter("Username", User.username),
                new SQLiteParameter("Password_hash", User.password_hash),
                new SQLiteParameter("Access", User.access),
                new SQLiteParameter("Image", User.image),
            });

            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void UpdateNull()
        {
            var commandString = "UPDATE atms SET route = CASE WHEN route IS NULL THEN '' ELSE route END, atmname = CASE WHEN atmname IS NULL THEN '' ELSE atmname END, name = CASE WHEN name IS NULL THEN '' ELSE name END, name2 = CASE WHEN name2 IS NULL THEN '' ELSE name2 END, circle = CASE WHEN circle IS NULL THEN '' ELSE circle END;";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            connection.Open();
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void Delete(int id)
        {
            var commandString = "DELETE FROM atms WHERE(id = @Id)";
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
                        var commandString = "DELETE FROM atms WHERE(id = @Id)";
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
                MessageBox.Show("Произошла ошибка связи с сервером: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void DeleteToDateLocal(DateTime date)
        {

            var commandString2 = "DELETE FROM atms WHERE (date = @Date)";
            SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);

            deleteCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            connection.Open();
            deleteCommand2.ExecuteNonQuery();
            connection.Close();
           
        }

        public void DeleteToDate(DateTime date)
        {

            var commandString2 = "DELETE FROM atms WHERE (date = @Date)";
            SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);

            deleteCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            connection.Open();
            deleteCommand2.ExecuteNonQuery();
            connection.Close();
            DeleteToDate2(date);
        }

        public void DeleteToDate2(DateTime date)
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
                        var commandString2 = "DELETE FROM atms WHERE (date = @Date)";
                        SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);

                        deleteCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                        deleteCommand2.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    connection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка связи с сервером: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public IEnumerable<atm> SearchAtmName(string name, string route, DateTime date)
        {
            connection.Open();
            var commandString = "SELECT id, route, atmname, name, name2, date, circle FROM atms WHERE atmname LIKE @Name And route LIKE @Route AND date = @Date ORDER BY SUBSTR(Route, 1, INSTR(Route, '/') - 1) ASC, CAST(SUBSTR(Route, INSTR(Route, '/') + 1) AS INTEGER) ASC;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
            getAllCommand.Parameters.AddWithValue("@Route", "%" + route + "%");
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Route = reader.GetString(1);
                var Atmname = reader.GetString(2);
                var Name = reader.GetString(3);
                var Name2 = reader.GetString(4);
                var Date = reader.GetDateTime(5);
                var Circle = reader.GetString(6);
                var Atm = new atm
                {
                    id = Id,
                    route = Route,
                    atmname = Atmname,
                    name = Name,
                    name2 = Name2,
                    date = Date,
                    circle = Circle
                };
                yield return Atm;
            }
            connection.Close();
        }


        public IEnumerable<user_account> Authorization(string login, string password)
        {
            connection.Close();

            var commandString = "SELECT * FROM user_accounts WHERE username = @login AND password_hash = @password ;";

            using (SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection))
            {
                getAllCommand.Parameters.AddWithValue("@login", login );
                getAllCommand.Parameters.AddWithValue("@password", password );

                connection.Open();

                using (var reader = getAllCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Id = reader.GetInt32(0);
                        var Username = reader.GetString(1);
                        var Password_hash = reader.GetString(2);
                        var User = new user_account
                        {
                            id = Id,
                            username = Username,
                            password_hash = Password_hash
                        };
                        connection.Close();
                        yield return User;
                    }
                   
                }
            }
           
        }

        public bool IsUsernameExists(string username, int? userId = null)
        {

            {
                connection.Open();

                string commandText;
                SQLiteCommand command;

                if (userId.HasValue)
                {
                    commandText = "SELECT COUNT(*) FROM user_accounts WHERE username = @Username AND id <> @UserId";
                    command = new SQLiteCommand(commandText, connection);
                    command.Parameters.AddWithValue("@UserId", userId.Value);
                }
                else
                {
                    commandText = "SELECT COUNT(*) FROM user_accounts WHERE username = @Username";
                    command = new SQLiteCommand(commandText, connection);
                }

                command.Parameters.AddWithValue("@Username", username);

                int count = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();

                return count > 0;
            }
        }

        public void SearchFoto(int id)
        {
            connection.Open();
            var commandString = "SELECT image FROM user_accounts WHERE id = @id;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@id", id);
            using (SQLiteDataReader reader = getAllCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    Add_User.image_bytes = (byte[])reader.GetValue(0); // Значение Id_Stocks (первый столбец)
                }
            }

            connection.Close();
        }

        public void MainPhoto(string name)
        {
            connection.Open();
            var commandString = "SELECT image, access FROM user_accounts WHERE username = @Username;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Username", name);
            using (SQLiteDataReader reader = getAllCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    MainWindow.image_Profil = (byte[])reader.GetValue(0); // Значение image (первый столбец)
                    MainWindow.acces = reader.GetString(1); // Значение access (второй столбец)
                }
            }

            connection.Close();
        }


        public void ImportExcelToDatabase(string filePath, DateTime date, BackgroundWorker worker, Action<int> reportProgress)
        {
            Excel.Application excel = null;
            Excel.Workbook workbook = null;
            try
            {
                // Строка подключения к базе данных SQLite
                string connectionString = @"Data Source=B.I.G.db;Version=3;";

                // Создание объекта подключения
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    // Открытие подключения
                    connection.Open();

                    // Создание объекта команды
                    SQLiteCommand command = new SQLiteCommand();

                    // Привязка команды к объекту подключения
                    command.Connection = connection;

                    // Создание объекта Excel
                    excel = new Excel.Application();

                    // Открытие книги Excel по пути к файлу
                    workbook = excel.Workbooks.Open(filePath);

                    // Выбор листа Excel для чтения данных (предполагаем, что данные находятся на первом листе)
                    Excel._Worksheet worksheet = workbook.Sheets[1];

                    // Получение диапазона ячеек для чтения данных
                    Excel.Range range = worksheet.UsedRange;

                    // Определение количества колонок в таблице Excel
                    int columnCount = range.Columns.Count;

                    // Создание SQL-запроса для вставки данных в таблицу atms
                    string query = "INSERT INTO atms (route, atmname, name, name2, date, circle) " +
                                   "VALUES (@Route, @AtmName, @Name, @Name2, @Date, @Circle)";

                    // Привязка SQL-запроса к объекту команды
                    command.CommandText = query;

                    // Создание параметров для SQL-запроса
                    command.Parameters.Add(new SQLiteParameter("@Route", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@AtmName", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Name", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Name2", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Date", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Circle", DbType.String));

                    // Определение общего количества строк для подсчета прогресса
                    int totalRows = range.Rows.Count;

                    // Проход по строкам диапазона
                    for (int row = 2; row <= totalRows; row++)
                    {
                        // Создание массива для хранения значений ячеек строки
                        object[] rowValues = new object[columnCount];

                        // Проход по ячейкам строки и заполнение массива rowValues
                        for (int col = 1; col <= columnCount; col++)
                        {
                            if (range.Cells[row, col].Value2 != null)
                            {
                                rowValues[col - 1] = (range.Cells[row, col] as Excel.Range).Value2.ToString();
                            }
                            else
                            {
                                rowValues[col - 1] = "";
                            }
                        }

                        // Проверка, что все необходимые ячейки в строке не пустые
                        if (rowValues.All(value => value != null))
                        {
                            command.Parameters["@Route"].Value = rowValues[0].ToString();
                            command.Parameters["@AtmName"].Value = rowValues[3]?.ToString();
                            command.Parameters["@Name"].Value = "";
                            command.Parameters["@Name2"].Value = "";
                            command.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");
                            command.Parameters["@Circle"].Value = rowValues[2]?.ToString();

                            // Выполнение SQL-запроса
                            command.ExecuteNonQuery();
                        }

                        // Расчет и передача прогресса в процентах
                        int progressPercentage = (int)((row - 1) / (double)(totalRows - 1) * 100);
                        reportProgress(progressPercentage);
                    }

                    // Закрытие книги Excel
                    workbook.Close(false);

                    // Закрытие приложения Excel
                    excel.Quit();
                    MessageBox.Show("Данные добавлены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Документ имеет неверный формат");
            }
            finally
            {
                // Закрытие приложения Excel
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }
            }
        }




        public int EmptyRouteCount(DateTime date)
        {
            int emptyRoute = 0;
            try
            {
                // Установка пути к базе данных
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "B.I.G.db");

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    var commandString = "SELECT COUNT(*) FROM atms WHERE date = @Date AND route = ''";
                    SQLiteCommand selectCommand = new SQLiteCommand(commandString, connection);
                    selectCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

                    emptyRoute = Convert.ToInt32(selectCommand.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при проверке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return emptyRoute;
        }


        public void UpdateJournalBase2(DateTime date)
        {
            // Путь к исходной базе данных (корень программы)
            string sourceDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "B.I.G.db");

            // Путь к целевой базе данных (из переменной MainWindow.puth)
            string destinationDbPath = Path.Combine(MainWindow.puth, "B.I.G.db");

            var journalEntries = new List<atm>();

            try
            {
                // Чтение данных из исходной базы данных
                using (SQLiteConnection sourceConnection = new SQLiteConnection($"Data Source={sourceDbPath};Version=3;"))
                {
                    sourceConnection.Open();
                    var commandString = "SELECT * FROM atms WHERE date = @Date";
                    SQLiteCommand selectCommand = new SQLiteCommand(commandString, sourceConnection);
                    selectCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entry = new atm
                            {
                                id = reader.GetInt32(0),
                                route = reader.GetString(1),
                                atmname = reader.GetString(2),
                                name = reader.GetString(3),
                                name2 = reader.GetString(4),
                                date = reader.GetDateTime(5),
                                circle = reader.GetString(6)
                            };
                            journalEntries.Add(entry);
                        }
                    }
                }

                // Перезапись данных в целевой базе данных
                using (SQLiteConnection destinationConnection = new SQLiteConnection($"Data Source={destinationDbPath};Version=3;"))
                {
                    destinationConnection.Open();

                    using (var transaction = destinationConnection.BeginTransaction()) // Начало транзакции
                    {
                        try
                        {
                            // Удаление существующих данных с той же датой
                            var deleteCommandString = "DELETE FROM atms WHERE date = @Date";
                            using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteCommandString, destinationConnection))
                            {
                                deleteCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Вставка новых данных
                            var insertCommandString = @"INSERT INTO atms (id, route, atmname, name, name2, date, circle) VALUES (@Id, @Route, @AtmName, @Name, @Name2, @Date, @Circle)";

                            foreach (var entry in journalEntries)
                            {
                                using (SQLiteCommand insertCommand = new SQLiteCommand(insertCommandString, destinationConnection))
                                {
                                    insertCommand.Parameters.AddWithValue("@Id", entry.id);
                                    insertCommand.Parameters.AddWithValue("@Route", entry.route);
                                    insertCommand.Parameters.AddWithValue("@AtmName", entry.atmname);
                                    insertCommand.Parameters.AddWithValue("@Name", entry.name);
                                    insertCommand.Parameters.AddWithValue("@Name2", entry.name2);
                                    insertCommand.Parameters.AddWithValue("@Date", entry.date.ToString("yyyy-MM-dd"));
                                    insertCommand.Parameters.AddWithValue("@Circle", entry.circle);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit(); // Фиксация транзакции
                            MessageBox.Show("Данные успешно опубликованы.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Откат транзакции в случае ошибки
                            throw; // Переброс исключения для обработки ниже
                        }
                    }

                    destinationConnection.Close(); // Закрытие соединения
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при перезаписи данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
