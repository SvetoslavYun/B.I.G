using System;
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
    internal class СashCollectorController
    { private SQLiteConnection connection;

        public СashCollectorController()
        {
            // Получение строки подключения из файла конфигурации
            var connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            // Создание объекта подключения
            connection = new SQLiteConnection(connString);
        }


        public IEnumerable<cashCollector> GetAllCashCollectors()
        {
            var commandString = "SELECT * FROM cashCollectors";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            connection.Open();

            var reader = getAllCommand.ExecuteReader();

            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Name = reader.GetString(1);
                var Gun = reader.GetString(2);
                var AutomatonSerial = reader.GetString(3);
                var Automaton = reader.GetString(4);
                var Permission = reader.GetString(5);
                var Meaning = reader.GetString(6);
                var Certificate = reader.GetString(7);
                var Token = reader.GetString(8);
                var Power = reader.GetString(9);
                var FullName = reader.GetString(10);
                var Profession = reader.GetString(11);
                var Phone = reader.GetString(12);
                var Image = (byte[])reader.GetValue(13);

                var CashCollector = new cashCollector
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
                    image = Image
                };

                yield return CashCollector;
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


        public void Update2(string name, string gun, string automaton_serial, string automaton, string permission, string meaning, string certificate, string token, string power, string fullname, string profession, string phone)
        {
            var commandString = "UPDATE cashCollectors SET name=@Name, gun=@Gun, automaton_serial=@AutomatonSerial, automaton=@Automaton, permission=@Permission, meaning=@Meaning, certificate=@Certificate, token=@Token, power=@Power, fullName=@FullName, profession=@Profession, phone=@Phone WHERE name=@Name";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);

            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
            new SQLiteParameter("Name", name),
            new SQLiteParameter("Gun", gun),
            new SQLiteParameter("AutomatonSerial", automaton_serial),
            new SQLiteParameter("Automaton", automaton),
            new SQLiteParameter("Permission", permission),
            new SQLiteParameter("Meaning", meaning),
            new SQLiteParameter("Certificate", certificate),
            new SQLiteParameter("Token", token),
            new SQLiteParameter("Power", power),
             new SQLiteParameter("FullName",fullname),
        new SQLiteParameter("Profession", profession),
        new SQLiteParameter("Phone", phone),

        });

            connection.Open();
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void Delete(int id, string name)
        {
            var commandString = "DELETE FROM cashCollectors WHERE (id = @Id) AND (name != @Name)";
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);

            deleteCommand.Parameters.AddWithValue("@Id", id);
            deleteCommand.Parameters.AddWithValue("@Name", name);

            connection.Open();
            deleteCommand.ExecuteNonQuery();
            connection.Close();
        }


        public IEnumerable<cashCollector> SearchCollectorName(string name)
        {
            connection.Open();
            if (name != "")
            {
                string selectQuery = "SELECT COUNT(*) FROM cashCollectors WHERE Name = @Name";
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
            var commandString = "SELECT * FROM cashCollectors WHERE name LIKE @Name;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Name", "%" + name + "%");

            connection.Open();
            var reader = getAllCommand.ExecuteReader();

            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Name = reader.GetString(1);
                var Gun = reader.GetString(2);
                var AutomatonSerial = reader.GetString(3);
                var Automaton = reader.GetString(4);
                var Permission = reader.GetString(5);
                var Meaning = reader.GetString(6);
                var Certificate = reader.GetString(7);
                var Token = reader.GetString(8);
                var Power = reader.GetString(9);
                var Fullname = reader.GetString(10);
                var Profession = reader.GetString(11);
                var Phone = reader.GetString(12);
                var Image = (byte[])reader.GetValue(13);

                var CashCollector = new cashCollector
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
                    fullname = Fullname,
                    profession = Profession,
                    phone = Phone,
                    image = Image
                };

                yield return CashCollector;
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



        public void ImportExcelToDatabase(string filePath)
        {
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

                    // определение количества колонок в таблице Excel
                    int columnCount = range.Columns.Count;

                    // создание SQL-запроса для вставки данных в таблицу cashCollectors
                    string query = "INSERT INTO cashCollectors (name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, profession, phone, image) " +
                                   "VALUES (@Name, @Gun, @Automaton_serial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @Fullname, @Profession, @Phone, @Image)";

                    // привязка SQL-запроса к объекту команды
                    command.CommandText = query;

                    // создание параметров для SQL-запроса
                    command.Parameters.Add(new SQLiteParameter("@Name", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Gun", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Automaton_serial", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Automaton", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Permission", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Meaning", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Certificate", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Token", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Power", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Fullname", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Profession", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Phone", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Image", DbType.Binary)); // Если это поле изображения

                    // проход по строкам диапазона
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        // создание массива для хранения значений ячеек строки
                        object[] rowValues = new object[columnCount];

                        // проход по ячейкам строки и заполнение массива rowValues
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


                        // Здесь вы можете добавить проверку наличия записи в базе данных по полю @Name. Это можно сделать путем выполнения запроса SELECT перед выполнением INSERT.

                        // проверка наличия записи в базе данных по полю @Name
                        string selectQuery = "SELECT COUNT(*) FROM cashCollectors WHERE Name = @Name";
                        using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@Name", rowValues[0].ToString());
                            long existingRecords = (long)selectCommand.ExecuteScalar();
                            if (existingRecords > 0)
                            {
                                
                               Update2(rowValues[0].ToString(), rowValues[1].ToString(), rowValues[2].ToString(), rowValues[3].ToString(), rowValues[4].ToString(), rowValues[5].ToString(), rowValues[6].ToString(), rowValues[7].ToString(), rowValues[8].ToString(), rowValues[9].ToString(), rowValues[10].ToString(), rowValues[11].ToString());

                                continue;
                            }
                        }

                        // проверка, что все необходимые ячейки в строке не пустые
                        if (rowValues[0] != null && rowValues[1] != null && rowValues[2] != null && rowValues[3] != null && rowValues[4] != null && rowValues[5] != null && rowValues[6] != null && rowValues[7] != null && rowValues[8] != null && rowValues[9] != null && rowValues[10] != null && rowValues[11] != null && rowValues[12] != null)
                        {
                            command.Parameters["@Name"].Value = rowValues[0].ToString();
                            command.Parameters["@Gun"].Value = rowValues[1]?.ToString() ?? "";
                            command.Parameters["@Automaton_serial"].Value = rowValues[2]?.ToString() ?? "";
                            command.Parameters["@Automaton"].Value = rowValues[3]?.ToString() ?? "";
                            command.Parameters["@Permission"].Value = rowValues[4]?.ToString() ?? "";
                            command.Parameters["@Meaning"].Value = rowValues[5]?.ToString() ?? "";
                            command.Parameters["@Certificate"].Value = rowValues[6]?.ToString() ?? "";
                            command.Parameters["@Token"].Value = rowValues[7]?.ToString() ?? "";
                            command.Parameters["@Power"].Value = rowValues[8]?.ToString() ?? "";
                            command.Parameters["@Fullname"].Value = rowValues[9]?.ToString() ?? "";
                            command.Parameters["@Profession"].Value = rowValues[10]?.ToString() ?? "";
                            command.Parameters["@Phone"].Value = rowValues[11]?.ToString() ?? "";
                            string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");
                            byte[] imageBytes = File.ReadAllBytes(defaultImagePath);
                            command.Parameters["@Image"].Value = imageBytes;

                            // выполнение SQL-запроса
                            command.ExecuteNonQuery();
                        }
                    }

                    // закрытие книги Excel
                    workbook.Close(false);

                    // закрытие приложения Excel
                    excel.Quit();
                    MessageBox.Show("Данные добавлены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Документ имеет не верный формат");
            }
            finally
            {
                // блок finally будет выполнен в любом случае, даже если произойдет исключение
                // закрытие книги Excel
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
