﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using B.I.G.Model;
using Excel = Microsoft.Office.Interop.Excel;

namespace B.I.G.Controller
{
    internal class JournalCollectorController
    { private SQLiteConnection connection;

        public JournalCollectorController()
        {
            // Получение строки подключения из файла конфигурации
            var connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            // Создание объекта подключения
            connection = new SQLiteConnection(connString);
        }


        public IEnumerable<journalCollector> GetAllCashCollectors()
        {
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT jc.*, CASE WHEN cc.image IS NULL THEN @DefaultImage ELSE cc.image END AS image
                          FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));

            connection.Open();

            var reader = getAllCommand.ExecuteReader();

            while (reader.Read())
            {
                var Id = reader.GetInt32(reader.GetOrdinal("id"));
                var Name = reader.GetString(reader.GetOrdinal("name"));
                var Gun = reader.GetString(reader.GetOrdinal("gun"));
                var AutomatonSerial = reader.GetString(reader.GetOrdinal("automaton_serial"));
                var Automaton = reader.GetString(reader.GetOrdinal("automaton"));
                var Permission = reader.GetString(reader.GetOrdinal("permission"));
                var Meaning = reader.GetString(reader.GetOrdinal("meaning"));
                var Certificate = reader.GetString(reader.GetOrdinal("certificate"));
                var Token = reader.GetString(reader.GetOrdinal("token"));
                var Power = reader.GetString(reader.GetOrdinal("power"));
                var FullName = reader.GetString(reader.GetOrdinal("fullname"));
                var Profession = reader.GetString(reader.GetOrdinal("profession"));
                var Phone = reader.GetString(reader.GetOrdinal("phone"));
                var Id2 = reader.GetInt32(reader.GetOrdinal("id2"));
                var Route = reader.GetString(reader.GetOrdinal("route"));
                var Date = reader.GetDateTime(reader.GetOrdinal("date"));
                var DateWork = reader.GetString(reader.GetOrdinal("dateWork"));
                var Appropriation = reader.GetString(reader.GetOrdinal("appropriation"));
                var Route2 = reader.GetString(reader.GetOrdinal("route2"));
                var Image = (byte[])reader.GetValue(reader.GetOrdinal("image"));

                var JournalCollector = new journalCollector
                {
                    id = Id,
                    name = Name,
                    gun = Gun,
                    automaton_serial = AutomatonSerial,
                    automaton = Automaton,
                    permission = Permission,
                    meaning = Meaning,
                    certificate = Certificate,
                    token = Token,
                    power = Power,
                    fullname = FullName,
                    profession = Profession,
                    phone = Phone,
                    id2 = Id2,
                    route = Route,
                    date = Date,
                    dateWork = DateWork,
                    appropriation = Appropriation,
                    route2 = Route2, // Добавлено новое поле route2
                    image = Image
                };

                yield return JournalCollector;
            }

            connection.Close();
        }





        public void Insert(cashCollector CashCollector)
        {
            var commandString = "INSERT INTO cashCollectors (name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullName, profession, phone, image) VALUES (@Name, @Gun, @AutomatonSerial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @FullName, @Profession, @Phone, @Image)";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("Name", CashCollector.name),
        new SQLiteParameter("Gun", CashCollector.gun),
        new SQLiteParameter("AutomatonSerial", CashCollector.automaton_serial),
        new SQLiteParameter("Automaton", CashCollector.automaton),
        new SQLiteParameter("Permission", CashCollector.permission),
        new SQLiteParameter("Meaning", CashCollector.meaning),
        new SQLiteParameter("Certificate", CashCollector.certificate),
        new SQLiteParameter("Token", CashCollector.token),
        new SQLiteParameter("Power", CashCollector.power),
        new SQLiteParameter("FullName", CashCollector.fullname),
        new SQLiteParameter("Profession", CashCollector.profession),
        new SQLiteParameter("Phone", CashCollector.phone),
        new SQLiteParameter("Image", CashCollector.image),
    });

            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void Update(cashCollector CashCollector)
        {
            var commandString = "UPDATE cashCollectors SET name=@Name, gun=@Gun, automaton_serial=@AutomatonSerial, automaton=@Automaton, permission=@Permission, meaning=@Meaning, certificate=@Certificate, token=@Token, power=@Power, fullName=@FullName, profession=@Profession, phone=@Phone, image=@Image WHERE id = @Id";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);

            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("Name", CashCollector.name),
        new SQLiteParameter("Gun", CashCollector.gun),
        new SQLiteParameter("AutomatonSerial", CashCollector.automaton_serial),
        new SQLiteParameter("Automaton", CashCollector.automaton),
        new SQLiteParameter("Permission", CashCollector.permission),
        new SQLiteParameter("Meaning", CashCollector.meaning),
        new SQLiteParameter("Certificate", CashCollector.certificate),
        new SQLiteParameter("Token", CashCollector.token),
        new SQLiteParameter("Power", CashCollector.power),
        new SQLiteParameter("FullName", CashCollector.fullname),
        new SQLiteParameter("Profession", CashCollector.profession),
        new SQLiteParameter("Phone", CashCollector.phone),
        new SQLiteParameter("Image", CashCollector.image),
        new SQLiteParameter("Id", CashCollector.id),
    });

            connection.Open();
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void UpdateResponsibilities()
        {
            var commandString = "UPDATE journalCollectors SET  automaton_serial='', automaton=''  WHERE profession !='водитель автомобиля' and profession !='Дежурный водитель № 1' and profession !='Дежурный водитель № 2'";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            var commandString2 = "UPDATE journalCollectors SET  permission=''  WHERE profession !='инкассатор-сборщик'";
            SQLiteCommand updateCommand2 = new SQLiteCommand(commandString2, connection);
            var commandString3 = "UPDATE journalCollectors SET route = '', route2 = '' WHERE Route !='РЕЗЕРВ' and SUBSTRING(Route, 1, 7) != 'Маршрут'";
            SQLiteCommand updateCommand3 = new SQLiteCommand(commandString3, connection);
            var commandString4 = "UPDATE journalCollectors SET route = SUBSTRING(Route, 10, 5), route2 = SUBSTRING(Route, 10, 5) WHERE SUBSTRING(Route, 1, 7) = 'Маршрут';";
            SQLiteCommand updateCommand4 = new SQLiteCommand(commandString4, connection);
            var commandString5 = "UPDATE journalCollectors SET route = SUBSTR(route, 2), route = SUBSTR(route, 2), route2 = SUBSTR(route2, 2), route2 = SUBSTR(route2, 2) WHERE route LIKE ' %';";
            SQLiteCommand updateCommand5 = new SQLiteCommand(commandString5, connection);           
            var commandString6 = "UPDATE journalCollectors SET gun = profession, dateWork=profession, profession='' WHERE SUBSTRING(profession, 1, 7) = 'Маршрут'";
            SQLiteCommand updateCommand6 = new SQLiteCommand(commandString6, connection);
            var commandString9 = "UPDATE journalCollectors SET gun = profession, dateWork=profession, profession='' WHERE profession = 'РЕЗЕРВ' AND name =''";
            SQLiteCommand updateCommand9 = new SQLiteCommand(commandString9, connection);
            var commandString7 = "UPDATE journalCollectors SET permission = '.', appropriation='.', fullname ='.' WHERE SUBSTRING(gun, 1, 7) = 'Маршрут' OR gun='РЕЗЕРВ'";
            SQLiteCommand updateCommand7 = new SQLiteCommand(commandString7, connection);
            var commandString8 = "DELETE FROM journalCollectors WHERE SUBSTRING(gun, 1, 7) != 'Маршрут' and route='' and name = '' or name = ' ' or name = '  ' OR name GLOB '*[-9]*' OR name GLOB '*[!A-Za-z/\\]*';";
            SQLiteCommand updateCommand8 = new SQLiteCommand(commandString8, connection);
            var commandString10 = "UPDATE journalCollectors SET route2 = SUBSTR(route, 1, INSTR(route2, '/') - 1)WHERE route2 LIKE '%/%';";
            SQLiteCommand updateCommand10 = new SQLiteCommand(commandString10, connection);
            var commandString11 = "UPDATE journalCollectors SET  route = profession, route2 = profession WHERE route ==''";
            SQLiteCommand updateCommand11 = new SQLiteCommand(commandString2, connection);
            connection.Open();
            updateCommand.ExecuteNonQuery();
            updateCommand2.ExecuteNonQuery();
            updateCommand3.ExecuteNonQuery();
            updateCommand4.ExecuteNonQuery();
            updateCommand5.ExecuteNonQuery();
            updateCommand6.ExecuteNonQuery();
            updateCommand9.ExecuteNonQuery();
            updateCommand7.ExecuteNonQuery();
            updateCommand8.ExecuteNonQuery();
            updateCommand10.ExecuteNonQuery();
            connection.Close();
        }


        public void Delete(string route, int id)
        {if (route == "")
            {
                var commandString = "DELETE FROM journalCollectors WHERE (id = @Id)";
                SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);

                deleteCommand.Parameters.AddWithValue("@Id", id);

                connection.Open();
                deleteCommand.ExecuteNonQuery();
                connection.Close();
            }
            else
            {
                var commandString2 = "DELETE FROM journalCollectors WHERE (route = @Route)";
                SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);

                deleteCommand2.Parameters.AddWithValue("@Route", route);

                connection.Open();
                deleteCommand2.ExecuteNonQuery();
                connection.Close();
            }
        }


        public IEnumerable<journalCollector> SearchCollectorName(string name)
        {
            connection.Open();
            if (name != "")
            {
                string selectQuery = "SELECT COUNT(*) FROM journalCollectors WHERE Name = @Name";
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
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @" SELECT  jc.*, COALESCE(cc.image, @DefaultImage) AS image FROM journalCollectors jc  LEFT JOIN cashCollectors cc ON jc.id2 = cc.id  WHERE  jc.route LIKE @Name;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Name", "" + name + "%");
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));

            connection.Open();
            var reader = getAllCommand.ExecuteReader();

            while (reader.Read())
            {
                var Id = reader.GetInt32(reader.GetOrdinal("id"));
                var Name = reader.GetString(reader.GetOrdinal("name"));
                var Gun = reader.GetString(reader.GetOrdinal("gun"));
                var AutomatonSerial = reader.GetString(reader.GetOrdinal("automaton_serial"));
                var Automaton = reader.GetString(reader.GetOrdinal("automaton"));
                var Permission = reader.GetString(reader.GetOrdinal("permission"));
                var Meaning = reader.GetString(reader.GetOrdinal("meaning"));
                var Certificate = reader.GetString(reader.GetOrdinal("certificate"));
                var Token = reader.GetString(reader.GetOrdinal("token"));
                var Power = reader.GetString(reader.GetOrdinal("power"));
                var FullName = reader.GetString(reader.GetOrdinal("fullname"));
                var Profession = reader.GetString(reader.GetOrdinal("profession"));
                var Phone = reader.GetString(reader.GetOrdinal("phone"));
                var Id2 = reader.GetInt32(reader.GetOrdinal("id2"));
                var Route = reader.GetString(reader.GetOrdinal("route"));
                var Date = reader.GetDateTime(reader.GetOrdinal("date"));
                var DateWork = reader.GetString(reader.GetOrdinal("dateWork"));
                var Appropriation = reader.GetString(reader.GetOrdinal("appropriation"));
                var Route2 = reader.GetString(reader.GetOrdinal("route2"));
                var Image = (byte[])reader.GetValue(reader.GetOrdinal("image"));

                var JournalCollector = new journalCollector
                {
                    id = Id,
                    name = Name,
                    gun = Gun,
                    automaton_serial = AutomatonSerial,
                    automaton = Automaton,
                    permission = Permission,
                    meaning = Meaning,
                    certificate = Certificate,
                    token = Token,
                    power = Power,
                    fullname = FullName,
                    profession = Profession,
                    phone = Phone,
                    id2 = Id2,
                    route = Route,
                    date = Date,
                    dateWork = DateWork,
                    appropriation = Appropriation,
                    route2 = Route2, // Добавлено новое поле route2
                    image = Image
                };

                yield return JournalCollector;
            }

            connection.Close();
        }


        public bool IsCashCollectorExists(string Name, int? Id = null)
        {
            connection.Open();

            string commandText;
            SQLiteCommand command;

            if (Id.HasValue)
            {
                commandText = "SELECT COUNT(*) FROM cashCollectors WHERE name = @Name AND id <> @Id";
                command = new SQLiteCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", Id.Value);
            }
            else
            {
                commandText = "SELECT COUNT(*) FROM cashCollectors WHERE name = @Name";
                command = new SQLiteCommand(commandText, connection);
            }

            command.Parameters.AddWithValue("@Name", Name);

            int count = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return count > 0;
        }



        public void SearchFoto(int id)
        {
            connection.Open();
            var commandString = "SELECT image FROM cashCollectors WHERE id = @id;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@id", id);

            using (SQLiteDataReader reader = getAllCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    Add_СashCollector.image_bytes = (byte[])reader.GetValue(0); // Значение Id_Stocks (первый столбец)
                }
            }

            connection.Close();
        }

        //public void ImportExcelToDatabase(string filePath)
        //{
        //    Excel.Application excel = null;
        //    Excel.Workbook workbook = null;
        //    try
        //    {
        //        // строка подключения к базе данных SQLite
        //        string connectionString = @"Data Source=B.I.G.db;Version=3;";

        //        // создание объекта подключения
        //        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //        {
        //            // открытие подключения
        //            connection.Open();

        //            // создание объекта команды
        //            SQLiteCommand command = new SQLiteCommand();

        //            // привязка команды к объекту подключения
        //            command.Connection = connection;

        //            // создание объекта Excel
        //            excel = new Excel.Application();

        //            // открытие книги Excel по пути к файлу
        //            workbook = excel.Workbooks.Open(filePath);

        //            // выбор листа Excel для чтения данных
        //            Excel._Worksheet worksheet = workbook.Sheets[1];

        //            // получение диапазона ячеек для чтения данных
        //            Excel.Range range = worksheet.UsedRange;

        //            // определение количества строк в таблице Excel
        //            int rowCount = range.Rows.Count;

        //            // проход по строкам диапазона
        //            for (int row = 5; row <= rowCount; row++)
        //            {
        //                // получение значений из колонок B и C
        //                string profession = (range.Cells[row, 2].Value2 ?? "").ToString();
        //                string name = (range.Cells[row, 3].Value2 ?? "").ToString();
        //                DateTime date = DateTime.Now; // текущая дата и время

        //                // создание SQL-запроса для вставки данных в таблицу journalCollectors
        //                string query = "INSERT INTO journalCollectors (profession, name, date) VALUES (@Profession, @Name, @Date)";

        //                // привязка SQL-запроса к объекту команды
        //                command.CommandText = query;

        //                // создание параметров для SQL-запроса
        //                command.Parameters.Clear(); // очистка параметров

        //                command.Parameters.Add(new SQLiteParameter("@Profession", DbType.String) { Value = profession });
        //                command.Parameters.Add(new SQLiteParameter("@Name", DbType.String) { Value = name });
        //                command.Parameters.Add(new SQLiteParameter("@Date", DbType.String) { Value = date.ToString("yyyy-MM-dd HH:mm:ss") }); // Поменяйте формат даты по вашему желанию

        //                // выполнение SQL-запроса
        //                command.ExecuteNonQuery();
        //            }

        //            // закрытие книги Excel
        //            workbook.Close(false);

        //            // закрытие приложения Excel
        //            excel.Quit();
        //            MessageBox.Show("Данные загружены");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Произошла ошибка при загрузке данных из Excel: " + ex.Message);
        //    }
        //    finally
        //    {
        //        //// закрытие книги Excel
        //        //if (workbook != null)
        //        //{
        //        //    workbook.Close(false);
        //        //    Marshal.ReleaseComObject(workbook);
        //        //}

        //        // закрытие приложения Excel
        //        if (excel != null)
        //        {
        //            excel.Quit();
        //            Marshal.ReleaseComObject(excel);
        //        }
        //    }
        //}

        public void ImportExcelToDatabase(string filePath)
        {
            string raute = string.Empty;
            Excel.Application excel = null;
            Excel.Workbook workbook = null;
            try
            {
                // строка подключения к базе данных SQLite
                string connectionString = @"Data Source=B.I.G.db;Version=3;";

                // создание объекта подключения
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    // открытие подключения
                    connection.Open();

                    // создание объекта команды
                    SQLiteCommand command = new SQLiteCommand();

                    // привязка команды к объекту подключения
                    command.Connection = connection;

                    // создание объекта Excel
                    excel = new Excel.Application();

                    // открытие книги Excel по пути к файлу
                    workbook = excel.Workbooks.Open(filePath);

                    // выбор листа Excel для чтения данных
                    Excel._Worksheet worksheet = workbook.Sheets[1];

                    // получение диапазона ячеек для чтения данных
                    Excel.Range range = worksheet.UsedRange;

                    // определение количества строк в таблице Excel
                    int rowCount = range.Rows.Count;

                    // проход по строкам диапазона
                    for (int row = 5; row <= rowCount; row++)
                    {
                        // получение значений из колонок B и C
                       
                        string profession = (range.Cells[row, 2].Value2 ?? "").ToString();
                        string name = (range.Cells[row, 3].Value2 ?? "").ToString();
                        string dateWork = (range.Cells[row, 4].Value2 ?? "").ToString();
                        string appropriation = (range.Cells[row, 7].Value2 ?? "").ToString();
                        DateTime date = DateTime.Now; // текущая дата и время

                        // создание SQL-запроса для вставки данных в таблицу journalCollectors
                        string query = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2 ) VALUES (@Profession, @Name, @Gun, @Automaton_serial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @Fullname, @Phone, @Id2, @Route, @Date, @DateWork, @Appropriation,@Route2 )";

                        // привязка SQL-запроса к объекту команды
                        command.CommandText = query;

                        // создание параметров для SQL-запроса
                        command.Parameters.Clear(); // очистка параметров
                        if (name != "")
                        {
                            string selectQuery = "SELECT COUNT(*) FROM cashCollectors WHERE Name = @Name";
                            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                            {
                                selectCommand.Parameters.AddWithValue("@Name", name);
                                long existingRecords = (long)selectCommand.ExecuteScalar();
                                if (existingRecords == 0)
                                {
                                    dateWork = "Данные отсутствуют";

                                }
                            }
                        }

                        command.Parameters.Add(new SQLiteParameter("@Profession", DbType.String) { Value = profession });
                        command.Parameters.Add(new SQLiteParameter("@Name", DbType.String) { Value = name });
                        command.Parameters.Add(new SQLiteParameter("@Gun", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Automaton_serial", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Automaton", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Permission", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Meaning", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Certificate", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Token", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Power", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Fullname", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Phone", DbType.String) { Value = "" });
                        command.Parameters.Add(new SQLiteParameter("@Id2", DbType.Int32) { Value = 0 }); // Предполагая, что это int, иначе укажите правильный тип данных
                        if (profession != "старший бригады инкассаторов" && profession != "инкассатор-сборщик" && profession != "водитель автомобиля" ) { raute = profession; }
                        command.Parameters.Add(new SQLiteParameter("@Route", DbType.String) { Value = raute });
                        command.Parameters.Add(new SQLiteParameter("@Date", DbType.String) { Value = date.ToString("yyyy-MM-dd") });
                        command.Parameters.Add(new SQLiteParameter("@DateWork", DbType.String) { Value = dateWork });
                        command.Parameters.Add(new SQLiteParameter("@Appropriation", DbType.String) { Value = appropriation });
                        command.Parameters.Add(new SQLiteParameter("@Route2", DbType.String) { Value = raute });

                        // выполнение SQL-запроса
                        command.ExecuteNonQuery();
                    }

                    // закрытие книги Excel
                    workbook.Close(false);

                    // закрытие приложения Excel
                    excel.Quit();
                    MessageBox.Show("Данные загружены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных из Excel: " + ex.Message);
            }
            finally
            {
                //// закрытие книги Excel
                //if (workbook != null)
                //{
                //    workbook.Close(false);
                //    Marshal.ReleaseComObject(workbook);
                //}

                // закрытие приложения Excel
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }
            }
        }
    }
}
