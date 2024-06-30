using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using B.I.G.Model;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Graph.Models.TermStore;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
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


        public void DeleteAfterSixMonthsLog()
        {
            var commandString = "DELETE FROM journalCollectors  WHERE date <= date('now', '-14 days')";
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
                        var commandString = "DELETE FROM journalCollectors  WHERE date <= date('now', '-14 days')";
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

        public void DeleteRound2(DateTime date)
        {
            var commandString = "DELETE FROM journalCollectors WHERE route IN (SELECT route FROM journalCollectors  WHERE route LIKE '%/2%' AND dateWork LIKE '%АТМ%' and date= @Date);";
            var commandString2 = "DELETE FROM journalCollectors WHERE route IN (SELECT route FROM journalCollectors  WHERE route LIKE '%/2%' AND (dateWork LIKE '%перевозка%' OR dateWork LIKE '%Перевозка%') and date= @Date);";
            var commandString3 = "DELETE FROM journalCollectors WHERE (profession LIKE 'старший бригады инкассаторов' and route ='') or (profession LIKE 'инкассатор-сборщик' and route ='') or (profession LIKE 'водитель автомобиля' and route ='');";
           
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
            SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);
            SQLiteCommand deleteCommand3 = new SQLiteCommand(commandString3, connection);
          
            deleteCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            deleteCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            deleteCommand3.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
          
            connection.Open();
            deleteCommand.ExecuteNonQuery();
            deleteCommand2.ExecuteNonQuery();
            deleteCommand3.ExecuteNonQuery();
          
            connection.Close();
        }


        public void DeleteRound22(DateTime date)
        {
          
            var commandString4 = "UPDATE journalCollectors SET area ='ул.Фабрициуса, 8б' WHERE CAST(route2 AS INT) >= 70  AND date = @Date AND profession NOT LIKE '%РЕЗЕРВ%'  AND profession NOT LIKE '%Стажер%' AND dateWork NOT LIKE '%РЕЗЕРВ%';";
            var commandString5 = "UPDATE journalCollectors SET area = area2  WHERE profession NOT LIKE '%одитель%'  AND profession NOT LIKE '%арший%' and profession NOT LIKE '%орщик%' AND date= @Date and dateWork NOT LIKE '%Маршрут%' and dateWork NOT LIKE '%РЕЗЕРВ%' and area2 !='';";
           
            SQLiteCommand deleteCommand4 = new SQLiteCommand(commandString4, connection);
            SQLiteCommand deleteCommand5 = new SQLiteCommand(commandString5, connection);
           
            deleteCommand4.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            deleteCommand5.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            connection.Open();
           
            deleteCommand4.ExecuteNonQuery();
            deleteCommand5.ExecuteNonQuery();
            connection.Close();
        }

        public IEnumerable<journalCollector> GetAllCashCollectors0(DateTime date, string area)
        {
            string area2="";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT jc.*, CASE WHEN cc.image IS NULL THEN @DefaultImage ELSE cc.image END AS image FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id WHERE jc.date= @Date and jc.route NOT LIKE '%/2%' and jc.route NOT LIKE '%\2%' AND (jc.area = @Area or jc.area = @Area2 or jc.area2 = @Area) and (jc.area2 = @Area or jc.dateWork LIKE '%аршрут%' or jc.dateWork LIKE '%РЕЗЕРВ%') ORDER BY CAST(jc.route2 AS INT)";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));
            getAllCommand.Parameters.AddWithValue("@Area", area);
            getAllCommand.Parameters.AddWithValue("@Area2", area2);
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data= Data
                };

                yield return JournalCollector;
            }

            connection.Close();
        }

        public IEnumerable<journalCollector> GetAllCashCollectors(DateTime date, string area)
        {
         
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");
            string area2 = "";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var commandString = @"SELECT jc.*, CASE WHEN cc.image IS NULL THEN @DefaultImage ELSE cc.image END AS image FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id WHERE jc.date= @Date AND (jc.area = @Area or jc.area = @Area2  or jc.dateWork LIKE '%РЕЗЕРВ%') ORDER BY CAST(jc.route2 AS INT)";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));
            getAllCommand.Parameters.AddWithValue("@Area", area);
            getAllCommand.Parameters.AddWithValue("@Area2", area2);
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data = Data
                };

                yield return JournalCollector;
            }

            connection.Close();
        }

        public IEnumerable<journalCollector> GetAllCashCollectors3(DateTime date, string area)
        {
            string area2 = "";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"
        SELECT jc.*, 
               CASE WHEN cc.image IS NULL THEN @DefaultImage ELSE cc.image END AS image, 
               cc.power AS cc_power 
        FROM journalCollectors jc 
        LEFT JOIN cashCollectors cc ON jc.id2 = cc.id 
        WHERE jc.date = @Date 
          AND jc.permission != '.' 
          AND jc.name != '' AND jc.data != 'Данные отсутствуют' AND (jc.area = @Area or jc.area = @Area2 or jc.area2 = @Area) and jc.area2 = @Area
        ORDER BY CAST(jc.route2 AS INT)";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));
            getAllCommand.Parameters.AddWithValue("@Area", area);
            getAllCommand.Parameters.AddWithValue("@Area2", area2);
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
                var Data = reader.GetString(reader.GetOrdinal("data"));
                var CCPower = reader.GetString(reader.GetOrdinal("cc_power")); // Новое поле power из cashCollectors

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
                    route2 = Route2,
                    image = Image,
                    data = Data,
                    cc_power = CCPower // Добавлено новое поле cc_power
                };

                yield return JournalCollector;
            }

            connection.Close();
        }




        public IEnumerable<journalCollector> GetAllCashCollectors4(DateTime date, string area)
        {
            string area2 = "";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT 
    jc.*, 
    CASE 
        WHEN cc.image IS NULL THEN @DefaultImage 
        ELSE cc.image 
    END AS image 
FROM 
    journalCollectors jc 
LEFT JOIN 
    cashCollectors cc ON jc.id2 = cc.id 
WHERE 
    jc.date = @Date 
    AND jc.permission != '.' 
    AND jc.name != ''  AND (jc.area = @Area or jc.area = @Area2 or jc.area2 = @Area) and jc.area2 = @Area
GROUP BY 
    jc.name ;
 ";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));
            getAllCommand.Parameters.AddWithValue("@Area", area);
            getAllCommand.Parameters.AddWithValue("@Area2", area2);
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data = Data
                };

                yield return JournalCollector;
            }

            connection.Close();
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
                    var commandString = "SELECT COUNT(*) FROM journalCollectors WHERE date = @Date AND dateWork = 'Резерв'";
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

        public void Insert(DateTime date, string area)
        {
            // Получаем максимальное значение из столбца route2
            int maxRoute2 = GetMaxRoute2();

            // Увеличиваем значение на 1
            int newRoute2 = maxRoute2 + 1;

            // Вставляем новую запись
            var commandString = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES ('', '', '', '', '', '', '', '', '', '', '', '', '0', 'Резерв', @Date, '00:00', '','" + newRoute2 + "', 'Данные отсутствуют', @Area, @Area )";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
         new SQLiteParameter("@Area", area)
    });
            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }

        // Метод для получения максимального значения из столбца route2
        private int GetMaxRoute2()
        {
            int maxRoute2 = 0;

            try
            {
                var commandString = "SELECT MAX(CAST(route2 AS INT)) FROM journalCollectors";
                SQLiteCommand command = new SQLiteCommand(commandString, connection);

                connection.Open();
                var result = command.ExecuteScalar();
                connection.Close();

                if (result != DBNull.Value)
                {
                    maxRoute2 = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine("Произошла ошибка при получении максимального значения route2: " + ex.Message);
            }

            return maxRoute2;
        }


        public void Insert2(DateTime date, string area)
        {
            var commandString = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES ('', '', '', '', '', '.', '', '', '', '', '.', '', '0', '', @Date, 'Резерв', '.','998', '', @Area, @Area )";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
             new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
              new SQLiteParameter("@Area", area)
            });
            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void InsertRoute(DateTime date, string area, string route, string circle)
        {
            var commandString = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES ('', '', '', '', '', '.', '', '', '', '', '.', '', '0', @Route || '/' || @Circle, @Date, 'Маршрут ' || @Route || '/' || @Circle, '.',@Route2, '', @Area, @Area  )";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
             new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
             new SQLiteParameter("@Route", route),
             new SQLiteParameter("@Route2", route),
             new SQLiteParameter("@Circle", circle),
              new SQLiteParameter("@Area", area)
            });
            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void InsertRoute2(DateTime date, string area, string route, string circle, string dateWorkt)
        {


            // Вставляем новую запись
            var commandString = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES ('Старший бригады (инкассатор/водитель/контролер)', '', '', '', '', '', '', '', '', '', '', '', '0', @Route || '/' || @Circle,@Date, @DateWorkt, '',@Route2, 'Данные отсутствуют', @Area , @Area)";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
         new SQLiteParameter("@Route", route),
         new SQLiteParameter("@Route2", route),
          new SQLiteParameter("@Circle", circle),
           new SQLiteParameter("@DateWorkt", dateWorkt),
         new SQLiteParameter("@Area", area)
    });
            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void InsertRoute3(DateTime date, string area, string route, string circle, string dateWorkt)
        {


            // Вставляем новую запись
            var commandString = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES ('Инкассатор - сборщик/оператор', '', '', '', '', '', '', '', '', '', '', '', '0',  @Route || '/' || @Circle,@Date, @DateWorkt, '',@Route, 'Данные отсутствуют', @Area, @Area  )";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
         new SQLiteParameter("@Route", route),
         new SQLiteParameter("@Route2", route),
          new SQLiteParameter("@Circle", circle),
          new SQLiteParameter("@DateWorkt", dateWorkt),
         new SQLiteParameter("@Area", area)
    });
            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void InsertRoute4(DateTime date, string area, string route, string circle, string dateWorkt)
        {


            // Вставляем новую запись
            var commandString = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES ('Водитель', '', '', '', '', '', '', '', '', '', '', '', '0',  @Route || '/' || @Circle,@Date, @DateWorkt, '',@Route, 'Данные отсутствуют', @Area, @Area  )";
            SQLiteCommand insertCommand = new SQLiteCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
         new SQLiteParameter("@Route", route),
         new SQLiteParameter("@Route2", route),
          new SQLiteParameter("@Circle", circle),
          new SQLiteParameter("@DateWorkt", dateWorkt),
         new SQLiteParameter("@Area", area)
    });
            connection.Open();
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void UpdateColumn(journalCollector JournalCollector, int id)
        {
            var commandString = "UPDATE journalCollectors SET profession = @Profession, appropriation = @Appropriation, dateWork=@DateWork WHERE id =@Id ";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("@Profession", JournalCollector.profession),
        new SQLiteParameter("@Appropriation", JournalCollector.appropriation),
         new SQLiteParameter("@DateWork", JournalCollector.dateWork),
        new SQLiteParameter("@Id",id)
    });
            connection.Open();
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void UpdateCollector(int id, int id2, DateTime date)
        {
            var commandString = "UPDATE journalCollectors SET profession = (SELECT profession FROM cashCollectors WHERE id=@Id) WHERE id =@Id2 and date = @Date and route = 'Резерв'";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
        new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
        new SQLiteParameter("@Id", id),
        new SQLiteParameter("@Id2", id2)
    });
            connection.Open();
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void Update(int idColl, int idJourn,string route, DateTime date, string profession)
        {
            var commandString = "UPDATE journalCollectors" +
                " SET name = cashCollectors.name,  gun = cashCollectors.gun," +
                " automaton_serial = cashCollectors.automaton_serial,  automaton = cashCollectors.automaton, " +
                " permission = cashCollectors.permission,data ='',  meaning = cashCollectors.meaning," +
                " certificate = cashCollectors.certificate, token = cashCollectors.token, power = cashCollectors.power," +
                " fullname = cashCollectors.fullname, phone = cashCollectors.phone, id2 = cashCollectors.id, area2 = (SELECT area FROM cashCollectors WHERE  id=@IdColl) " +
                " FROM cashCollectors WHERE cashCollectors.id = @IdColl AND journalCollectors.id = @IdJourn  ;";
            var commandString2 = "UPDATE journalCollectors SET data =''," +
                " name = (SELECT name FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date), " +
                " gun = (SELECT gun FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date)," +
                " automaton_serial = (SELECT automaton_serial FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date), " +
                " automaton = (SELECT automaton FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date)," +
                " permission = (SELECT permission FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date)," +
                " meaning = (SELECT meaning FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date)," +
                " certificate = (SELECT certificate FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date)," +
                " token = (SELECT token FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date), " +
                " power = (SELECT power FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date), " +
                " fullname = (SELECT fullname FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date)," +
                " phone = (SELECT phone FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date)," +
                " id2 = (SELECT id2 FROM journalCollectors WHERE route2 = @Route and id = @IdJourn and route !='' and date = @Date), area2 = (SELECT area FROM cashCollectors WHERE  id=@IdColl) " +
                " WHERE route2 = @Route AND profession = @Profession and date = @Date and route !='' AND route2 != 'РЕЗЕРВ' AND route2 != 'стажер' AND route2 != 'Стажер';";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            SQLiteCommand updateCommand2 = new SQLiteCommand(commandString2, connection);

            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
             new SQLiteParameter("@IdColl", idColl),
             new SQLiteParameter("@IdJourn", idJourn),
           
            
            });

            updateCommand2.Parameters.AddRange(new SQLiteParameter[] {
    new SQLiteParameter("@IdJourn", idJourn),
    new SQLiteParameter("@Route", route),
    new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),
     new SQLiteParameter("@IdColl", idColl),
    new SQLiteParameter("@Profession", profession)
});

            connection.Open();
            updateCommand.ExecuteNonQuery();
            updateCommand2.ExecuteNonQuery();
            connection.Close();
        }



    


        public void EditAutomate(int idColl, string name, DateTime date, string rote)
        {
            var commandString = "UPDATE journalCollectors SET " +
                "automaton_serial = ( SELECT automaton_serial  FROM cashCollectors WHERE cashCollectors.id = @IdColl  ), data ='Автомат не повторяется'," +
                " automaton = ( SELECT automaton FROM cashCollectors WHERE cashCollectors.id = @IdColl ) " +
                "WHERE date = @Date and journalCollectors.name = @Name;";

            var commandString2 = "UPDATE journalCollectors SET " +
               "automaton_serial = ''," +
               " automaton = '' " +
               "WHERE date = @Date and route2 = @Route2;";

            var commandString3 = "UPDATE journalCollectors SET " +
              "data =''" +
              "WHERE date = @Date and route2 = @Route2 and fullname !='.' AND data != 'Данные отсутствуют';";


            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
             new SQLiteParameter("@IdColl", idColl),
             new SQLiteParameter("@Name", name),
             new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),});

            SQLiteCommand updateCommand2 = new SQLiteCommand(commandString2, connection);
            updateCommand2.Parameters.AddRange(new SQLiteParameter[] {
             new SQLiteParameter("@Route2", rote),
             new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),});

            SQLiteCommand updateCommand3 = new SQLiteCommand(commandString3, connection);
            updateCommand3.Parameters.AddRange(new SQLiteParameter[] {
             new SQLiteParameter("@Route2", rote),
             new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")),});

            connection.Open();
            updateCommand3.ExecuteNonQuery();
            updateCommand2.ExecuteNonQuery();
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }


        public void UpdateResponsibilities(DateTime date,string area)
        {
            var commandString = "UPDATE journalCollectors SET automaton_serial='', automaton='' WHERE profession NOT LIKE '%Водитель%' and profession != 'Дежурный водитель № 1' and profession != 'Дежурный водитель № 2'AND date = @Date";
            var commandString2 = "UPDATE journalCollectors SET meaning='' WHERE profession NOT LIKE '%борщик%' AND date = @Date";
            var commandString3 = "UPDATE journalCollectors SET route = '', route2 = '' WHERE Route != 'РЕЗЕРВ' and Route != 'стажер ' and Route != 'стажер' and SUBSTRING(Route, 1, 7) != 'Маршрут' and date = @Date AND area = @Area";
            var commandString4 = "UPDATE journalCollectors SET route = SUBSTRING(Route, 9, 6), route2 = SUBSTRING(Route, 9, 6) WHERE SUBSTRING(Route, 1, 7) = 'Маршрут' AND date = @Date";
            var commandString5 = "UPDATE journalCollectors SET route = SUBSTR(route, 2), route = SUBSTR(route, 2), route2 = SUBSTR(route2, 2), route2 = SUBSTR(route2, 2) WHERE route LIKE ' %' AND date = @Date";
            var commandString6 = "UPDATE journalCollectors SET dateWork=profession, profession='' WHERE SUBSTRING(profession, 1, 7) = 'Маршрут' and date = @Date";
            var commandString7 = "UPDATE journalCollectors SET permission = '.', appropriation='.', fullname ='.' WHERE SUBSTRING(dateWork, 1, 7) = 'Маршрут' OR gun='РЕЗЕРВ' AND date = @Date";
            var commandString8 = "DELETE FROM journalCollectors WHERE SUBSTRING(dateWork, 1, 7) != 'Маршрут' and date = @Date and route='' and name = '' or name = ' ' or name = '  ' OR name GLOB '*[-9]*'";
            var commandString9 = "UPDATE journalCollectors SET dateWork=profession, profession='' WHERE profession = 'РЕЗЕРВ' AND name ='' AND date = @Date";
            var commandString10 = "UPDATE journalCollectors SET route2 = SUBSTR(route, 1, INSTR(route2, '/') - 1) WHERE route2 LIKE '%/%'AND date = @Date";
            var commandString15 = "UPDATE journalCollectors SET route2 = SUBSTR(route, 1, INSTR(route2, '\\') - 1) WHERE REPLACE(route2, '\\', '/') LIKE '%/%' AND date = @Date;";
            var commandString11 = "UPDATE journalCollectors SET route2 = route WHERE route2 ='' AND date = @Date";
            var commandString12 = "UPDATE journalCollectors SET permission = '.', appropriation='.', fullname ='.' WHERE SUBSTRING(dateWork, 1, 7) = 'РЕЗЕРВ' and date = @Date";
            var commandString13 = "UPDATE journalCollectors SET route = SUBSTR(route, 1, INSTR(route || ' ', ' ') - 1), route2 = SUBSTR(route2, 1, INSTR(route2 || ' ', ' ') - 1) WHERE route LIKE '% %' OR route2 LIKE '% %' and date = @Date;";
            //var commandString14 = "UPDATE journalCollectors AS j1 SET dateWork = 'Повтор автомата -  М.' || (SELECT j3.route2 FROM journalCollectors AS j3 WHERE j1.automaton_serial = j3.automaton_serial AND j1.name <> j3.name AND j3.name <> '' LIMIT 1) || ', ' || (SELECT j2.name FROM journalCollectors AS j2 WHERE j1.automaton_serial = j2.automaton_serial AND j1.name <> j2.name AND j2.name <> '' LIMIT 1) WHERE j1.automaton_serial != '' AND j1.name <> '' AND EXISTS (SELECT 1 FROM journalCollectors AS j2 WHERE j1.automaton_serial = j2.automaton_serial AND j1.name <> j2.name AND j2.name <> '' and j2.date = @Date);";
            //var commandString14 = "UPDATE journalCollectors AS j1 SET dateWork = 'Повтор автомата' WHERE j1.automaton_serial != '' AND j1.name <> '' AND EXISTS (SELECT 1 FROM journalCollectors AS j2 WHERE j1.automaton_serial = j2.automaton_serial AND j1.name <> j2.name AND j2.name <> '');";
            var commandString14 = "UPDATE journalCollectors AS j1 SET data = 'Повтор автомата' WHERE j1.automaton_serial IN (SELECT automaton_serial FROM journalCollectors  WHERE automaton_serial != '' and date = @Date GROUP BY automaton_serial  HAVING COUNT(DISTINCT name) > 1);";
            var commandString16 = "UPDATE journalCollectors AS j1 SET data = data || ' М.' || (SELECT route2 || ' ' || name FROM journalCollectors AS j2   WHERE j1.automaton_serial = j2.automaton_serial AND j2.name <> j1.name AND j2.date = j1.date) WHERE data = 'Повтор автомата' AND automaton_serial IN (SELECT automaton_serial FROM journalCollectors  WHERE date = @Date AND data = 'Повтор автомата'  GROUP BY automaton_serial HAVING COUNT(DISTINCT name) > 1);";
            connection.Open();

            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            SQLiteCommand updateCommand2 = new SQLiteCommand(commandString2, connection);
            SQLiteCommand updateCommand3 = new SQLiteCommand(commandString3, connection);
            SQLiteCommand updateCommand4 = new SQLiteCommand(commandString4, connection);
            SQLiteCommand updateCommand5 = new SQLiteCommand(commandString5, connection);
            SQLiteCommand updateCommand6 = new SQLiteCommand(commandString6, connection);
            SQLiteCommand updateCommand7 = new SQLiteCommand(commandString7, connection);
            SQLiteCommand deleteCommand8 = new SQLiteCommand(commandString8, connection);
            SQLiteCommand updateCommand9 = new SQLiteCommand(commandString9, connection);
            SQLiteCommand updateCommand10 = new SQLiteCommand(commandString10, connection);
            SQLiteCommand updateCommand11 = new SQLiteCommand(commandString11, connection);
            SQLiteCommand updateCommand12 = new SQLiteCommand(commandString12, connection);
            SQLiteCommand updateCommand13 = new SQLiteCommand(commandString13, connection);
            SQLiteCommand updateCommand14 = new SQLiteCommand(commandString14, connection);
            SQLiteCommand updateCommand15 = new SQLiteCommand(commandString15, connection);
            SQLiteCommand updateCommand16 = new SQLiteCommand(commandString16, connection);

            updateCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand3.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand3.Parameters.AddWithValue("@Area", area);
            updateCommand4.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand5.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand6.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand7.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            deleteCommand8.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand9.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand10.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand11.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand12.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand13.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand14.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand15.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand16.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            // Выполнение команд
            updateCommand.ExecuteNonQuery();
            updateCommand2.ExecuteNonQuery();
            updateCommand3.ExecuteNonQuery();
            updateCommand4.ExecuteNonQuery();
            updateCommand5.ExecuteNonQuery();
            updateCommand6.ExecuteNonQuery();
            updateCommand7.ExecuteNonQuery();
            deleteCommand8.ExecuteNonQuery(); 
            updateCommand9.ExecuteNonQuery();
            updateCommand10.ExecuteNonQuery();
            updateCommand11.ExecuteNonQuery();
            updateCommand12.ExecuteNonQuery();
            updateCommand13.ExecuteNonQuery();
            updateCommand14.ExecuteNonQuery();
            updateCommand16.ExecuteNonQuery();
            updateCommand15.ExecuteNonQuery();
            connection.Close();
        }


        public void UpdateResponsibilities22(DateTime date, string area)
        {
            var commandString = "UPDATE journalCollectors SET automaton_serial='', automaton='' WHERE profession NOT LIKE '%одитель%' and profession != 'Дежурный водитель № 1' and profession != 'Дежурный водитель № 2'AND date = @Date";
            var commandString2 = "UPDATE journalCollectors SET meaning='' WHERE profession NOT LIKE '%борщик%' AND date = @Date";
            var commandString3 = "UPDATE journalCollectors SET route = '', route2 = '' WHERE Route != 'РЕЗЕРВ' and Route != 'стажер ' and Route != 'стажер' and SUBSTRING(Route, 1, 7) != 'Маршрут' and date = @Date AND area = @Area";
            var commandString4 = "UPDATE journalCollectors SET route = SUBSTRING(Route, 10, 6), route2 = SUBSTRING(Route, 10, 6) WHERE SUBSTRING(Route, 1, 7) = 'Маршрут' AND date = @Date";
            var commandString5 = "UPDATE journalCollectors SET route = SUBSTR(route, 2), route = SUBSTR(route, 2), route2 = SUBSTR(route2, 2), route2 = SUBSTR(route2, 2) WHERE route LIKE ' %' AND date = @Date";
            var commandString6 = "UPDATE journalCollectors SET dateWork=profession, profession='' WHERE SUBSTRING(profession, 1, 7) = 'Маршрут' and date = @Date";
            var commandString7 = "UPDATE journalCollectors SET permission = '.', appropriation='.', fullname ='.' WHERE SUBSTRING(dateWork, 1, 7) = 'Маршрут' OR gun='РЕЗЕРВ' AND date = @Date";
            var commandString8 = "DELETE FROM journalCollectors WHERE SUBSTRING(dateWork, 1, 7) != 'Маршрут' and date = @Date and route='' and name = '' or name = ' ' or name = '  ' OR name GLOB '*[-9]*'";
            var commandString9 = "UPDATE journalCollectors SET dateWork=profession, profession='' WHERE profession = 'РЕЗЕРВ' AND name ='' AND date = @Date";
            var commandString10 = "UPDATE journalCollectors SET route2 = SUBSTR(route, 1, INSTR(route2, '/') - 1) WHERE route2 LIKE '%/%'AND date = @Date";
            var commandString15 = "UPDATE journalCollectors SET route2 = SUBSTR(route, 1, INSTR(route2, '\\') - 1) WHERE REPLACE(route2, '\\', '/') LIKE '%/%' AND date = @Date;";
            var commandString11 = "UPDATE journalCollectors SET route2 = route WHERE route2 ='' AND date = @Date";
            var commandString12 = "UPDATE journalCollectors SET permission = '.', appropriation='.', fullname ='.' WHERE SUBSTRING(dateWork, 1, 7) = 'РЕЗЕРВ' and date = @Date";
            var commandString13 = "UPDATE journalCollectors SET route = SUBSTR(route, 1, INSTR(route || ' ', ' ') - 1), route2 = SUBSTR(route2, 1, INSTR(route2 || ' ', ' ') - 1) WHERE route LIKE '% %' OR route2 LIKE '% %' and date = @Date;";
            //var commandString14 = "UPDATE journalCollectors AS j1 SET dateWork = 'Повтор автомата -  М.' || (SELECT j3.route2 FROM journalCollectors AS j3 WHERE j1.automaton_serial = j3.automaton_serial AND j1.name <> j3.name AND j3.name <> '' LIMIT 1) || ', ' || (SELECT j2.name FROM journalCollectors AS j2 WHERE j1.automaton_serial = j2.automaton_serial AND j1.name <> j2.name AND j2.name <> '' LIMIT 1) WHERE j1.automaton_serial != '' AND j1.name <> '' AND EXISTS (SELECT 1 FROM journalCollectors AS j2 WHERE j1.automaton_serial = j2.automaton_serial AND j1.name <> j2.name AND j2.name <> '' and j2.date = @Date);";
            //var commandString14 = "UPDATE journalCollectors AS j1 SET dateWork = 'Повтор автомата' WHERE j1.automaton_serial != '' AND j1.name <> '' AND EXISTS (SELECT 1 FROM journalCollectors AS j2 WHERE j1.automaton_serial = j2.automaton_serial AND j1.name <> j2.name AND j2.name <> '');";
            var commandString14 = "UPDATE journalCollectors AS j1 SET data = 'Повтор автомата' WHERE j1.automaton_serial IN (SELECT automaton_serial FROM journalCollectors  WHERE automaton_serial != '' and date = @Date GROUP BY automaton_serial  HAVING COUNT(DISTINCT name) > 1);";
            var commandString16 = "UPDATE journalCollectors AS j1 SET data = data || ' М.' || (SELECT route2 || ' ' || name FROM journalCollectors AS j2   WHERE j1.automaton_serial = j2.automaton_serial AND j2.name <> j1.name AND j2.date = j1.date) WHERE data = 'Повтор автомата' AND automaton_serial IN (SELECT automaton_serial FROM journalCollectors  WHERE date = @Date AND data = 'Повтор автомата'  GROUP BY automaton_serial HAVING COUNT(DISTINCT name) > 1);";
            connection.Open();

            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            SQLiteCommand updateCommand2 = new SQLiteCommand(commandString2, connection);
            SQLiteCommand updateCommand3 = new SQLiteCommand(commandString3, connection);
            SQLiteCommand updateCommand4 = new SQLiteCommand(commandString4, connection);
            SQLiteCommand updateCommand5 = new SQLiteCommand(commandString5, connection);
            SQLiteCommand updateCommand6 = new SQLiteCommand(commandString6, connection);
            SQLiteCommand updateCommand7 = new SQLiteCommand(commandString7, connection);
            SQLiteCommand deleteCommand8 = new SQLiteCommand(commandString8, connection);
            SQLiteCommand updateCommand9 = new SQLiteCommand(commandString9, connection);
            SQLiteCommand updateCommand10 = new SQLiteCommand(commandString10, connection);
            SQLiteCommand updateCommand11 = new SQLiteCommand(commandString11, connection);
            SQLiteCommand updateCommand12 = new SQLiteCommand(commandString12, connection);
            SQLiteCommand updateCommand13 = new SQLiteCommand(commandString13, connection);
            SQLiteCommand updateCommand14 = new SQLiteCommand(commandString14, connection);
            SQLiteCommand updateCommand15 = new SQLiteCommand(commandString15, connection);
            SQLiteCommand updateCommand16 = new SQLiteCommand(commandString16, connection);

            updateCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand3.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand3.Parameters.AddWithValue("@Area", area);
            updateCommand4.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand5.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand6.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand7.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            deleteCommand8.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand9.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand10.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand11.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand12.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand13.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand14.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand15.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand16.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            // Выполнение команд
            updateCommand.ExecuteNonQuery();
            updateCommand2.ExecuteNonQuery();
            updateCommand3.ExecuteNonQuery();
            updateCommand4.ExecuteNonQuery();
            updateCommand5.ExecuteNonQuery();
            updateCommand6.ExecuteNonQuery();
            updateCommand7.ExecuteNonQuery();
            deleteCommand8.ExecuteNonQuery();
            updateCommand9.ExecuteNonQuery();
            updateCommand10.ExecuteNonQuery();
            updateCommand11.ExecuteNonQuery();
            updateCommand12.ExecuteNonQuery();
            updateCommand13.ExecuteNonQuery();
            updateCommand14.ExecuteNonQuery();
            updateCommand16.ExecuteNonQuery();
            updateCommand15.ExecuteNonQuery();
            connection.Close();
        }


        public void UpdateRoute2(DateTime date)
        {
            // Получаем максимальное значение из столбца route2
            int maxRoute2 = GetMaxRoute2();

            // Подготовка строк команд для выборки и обновления
            var selectCommandString2 = @"
        SELECT id, route2 
        FROM journalCollectors 
        WHERE route2 LIKE '%РЕЗЕРВ%' 
        AND date = @Date 
        ORDER BY id";

            var selectCommandString = @"
        SELECT id, route2 
        FROM journalCollectors 
        WHERE (profession LIKE '%РЕЗЕРВ%' OR profession LIKE '%Стажер%') 
        AND date = @Date 
        ORDER BY id";

            var updateCommandString = "UPDATE journalCollectors SET route2 = @Route2 WHERE id = @Id";

            connection.Open();

            // Подготовка команды для выбора по второму условию (как указано)
            SQLiteCommand selectCommand2 = new SQLiteCommand(selectCommandString2, connection);
            selectCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            // Выполнение команды выбора по второму условию
            SQLiteDataReader reader2 = selectCommand2.ExecuteReader();

            // Подготовка команды для обновления
            SQLiteCommand updateCommand = new SQLiteCommand(updateCommandString, connection);

            // Проходим по всем выбранным записям и обновляем их
            while (reader2.Read())
            {
                int id = reader2.GetInt32(0);
                maxRoute2++;
                string newRoute2 = maxRoute2.ToString();

                // Обновляем значение route2 для текущей записи
                updateCommand.Parameters.Clear();
                updateCommand.Parameters.AddWithValue("@Route2", newRoute2);
                updateCommand.Parameters.AddWithValue("@Id", id);
                updateCommand.ExecuteNonQuery();
            }

            reader2.Close();

            // Подготовка команды для выбора по первому условию
            SQLiteCommand selectCommand = new SQLiteCommand(selectCommandString, connection);
            selectCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            // Выполнение команды выбора по первому условию
            SQLiteDataReader reader = selectCommand.ExecuteReader();

            // Проходим по всем выбранным записям и обновляем их
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                maxRoute2++;
                string newRoute2 = maxRoute2.ToString();

                // Обновляем значение route2 для текущей записи
                updateCommand.Parameters.Clear();
                updateCommand.Parameters.AddWithValue("@Route2", newRoute2);
                updateCommand.Parameters.AddWithValue("@Id", id);
                updateCommand.ExecuteNonQuery();
            }

            reader.Close();

            connection.Close();
        }


        public void UpdateResponsibilities2(DateTime date)
        {
            var commandString1 = "UPDATE journalCollectors SET data ='' WHERE date = @Date AND data != 'Данные отсутствуют' and data !='Автомат не повторяется' AND dateWork != 'РЕЗЕРВ' and Route != 'стажер ' and Route != 'стажер' and  SUBSTRING(dateWork, 1, 7) != 'Маршрут' ";
            var commandString2 = "UPDATE journalCollectors SET automaton_serial='', automaton='' WHERE (profession NOT LIKE '%одитель%' or profession  LIKE '%арший%') and data !='Автомат не повторяется' and profession != 'Дежурный водитель № 1' and profession != 'Дежурный водитель № 2'AND date = @Date";
            var commandString3 = "UPDATE journalCollectors SET meaning='' WHERE profession NOT LIKE '%борщик%' AND date = @Date";
            var commandString4 = "UPDATE journalCollectors AS j1 SET data = 'Повтор автомата' WHERE j1.automaton_serial IN (SELECT automaton_serial FROM journalCollectors  WHERE automaton_serial != '' and date = @Date GROUP BY automaton_serial  HAVING COUNT(DISTINCT name) > 1);";
            var commandString5 = "UPDATE journalCollectors AS j1 SET data = data || ' М.' || (SELECT route2 || ' ' || name FROM journalCollectors AS j2   WHERE j1.automaton_serial = j2.automaton_serial AND j2.name <> j1.name AND j2.date = j1.date) WHERE data = 'Повтор автомата' AND automaton_serial IN (SELECT automaton_serial FROM journalCollectors  WHERE date = @Date AND data = 'Повтор автомата'  GROUP BY automaton_serial HAVING COUNT(DISTINCT name) > 1);";
            var commandString6 = "UPDATE journalCollectors SET permission = '.', appropriation='.', fullname ='.' WHERE SUBSTRING(dateWork, 1, 7) = 'Маршрут' OR SUBSTRING(dateWork, 1, 7) = 'РЕЗЕРВ' and date = @Date";

            connection.Open();
            SQLiteCommand updateCommand1 = new SQLiteCommand(commandString1, connection);
            SQLiteCommand updateCommand2 = new SQLiteCommand(commandString2, connection);
            SQLiteCommand updateCommand3 = new SQLiteCommand(commandString3, connection);
            SQLiteCommand updateCommand4 = new SQLiteCommand(commandString4, connection);
            SQLiteCommand updateCommand5 = new SQLiteCommand(commandString5, connection);
            SQLiteCommand updateCommand6 = new SQLiteCommand(commandString6, connection);

            updateCommand1.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand3.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));         
            updateCommand4.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand5.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            updateCommand6.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            updateCommand1.ExecuteNonQuery();
            updateCommand2.ExecuteNonQuery();
            updateCommand3.ExecuteNonQuery();
            updateCommand4.ExecuteNonQuery();
            updateCommand5.ExecuteNonQuery();
            updateCommand6.ExecuteNonQuery();
            connection.Close();
        }


        public void Delete(string route, int id, DateTime date)
        { if (route == "" || route == "РЕЗЕРВ" || route == "резерв" || route == "Резерв" || route == "стажер" || route == "стажер")
            {
                var commandString = "DELETE FROM journalCollectors WHERE (id = @Id) and date=@Date";
                SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);

                deleteCommand.Parameters.AddWithValue("@Id", id);
                deleteCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                connection.Open();
                deleteCommand.ExecuteNonQuery();
                connection.Close();
            }
            else
            {
                var commandString2 = "DELETE FROM journalCollectors WHERE (route = @Route and date=@Date)";
                SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);

                deleteCommand2.Parameters.AddWithValue("@Route", route);
                deleteCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                connection.Open();
                deleteCommand2.ExecuteNonQuery();
                connection.Close();
            }
        }


        public void DeleteNUL()
        {
         
                var commandString = "DELETE FROM journalCollectors WHERE name IS NULL OR  gun IS NULL OR automaton_serial IS NULL OR automaton IS NULL OR permission IS NULL OR meaning  IS NULL OR certificate  IS NULL OR token IS NULL OR power  IS NULL OR fullname IS NULL OR profession IS NULL OR phone IS NULL OR id2 IS NULL OR route  IS NULL OR date  IS NULL OR dateWork  IS NULL OR appropriation  IS NULL OR   route2 IS NULL OR   area IS NULL OR area2 IS NULL;";
                SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
                connection.Open();
                deleteCommand.ExecuteNonQuery();
                connection.Close();
           
              
        }


        public void UpdateNullValues(DateTime date)
        {
            var updateCommandString = @"UPDATE journalCollectors 
                                SET name = COALESCE(name, 'Данные отсутствуют'),
                                    gun = COALESCE(gun, 'Данные отсутствуют'),
                                    automaton_serial = COALESCE(automaton_serial, 'Данные отсутствуют'),
                                    automaton = COALESCE(automaton, 'Данные отсутствуют'),
                                    permission = COALESCE(permission, 'Данные отсутствуют'),
                                    meaning = COALESCE(meaning, 'Данные отсутствуют'),
                                    certificate = COALESCE(certificate, 'Данные отсутствуют'),
                                    token = COALESCE(token, 'Данные отсутствуют'),
                                    power = COALESCE(power, 'Данные отсутствуют'),
                                    fullname = COALESCE(fullname, 'Данные отсутствуют'),
                                    profession = COALESCE(profession, 'Данные отсутствуют'),
                                    phone = COALESCE(phone, 'Данные отсутствуют'),
                                    id2 = COALESCE(id2, 0),
                                    route = COALESCE(route, 'Данные отсутствуют'),
                                    date = COALESCE(date, @Date),
                                    dateWork = COALESCE(dateWork, 'Данные отсутствуют'),
                                    appropriation = COALESCE(appropriation, 'Данные отсутствуют'),
                                    route2 = COALESCE(route2, 'Данные отсутствуют'),
                                    data = COALESCE(data, ''), area = COALESCE(area, '') , area2 = COALESCE(area2, '');";
          
            SQLiteCommand updateCommand = new SQLiteCommand(updateCommandString, connection);
            updateCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
           
                connection.Open();
                updateCommand.ExecuteNonQuery();
                connection.Close();
            
        }


        public void DeleteToDate(DateTime date, string area)
        {
            string area2="";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var commandString2 = "DELETE FROM journalCollectors WHERE (date = @Date) and (area = @Area or area = @Area2)";
                SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);

                deleteCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
               deleteCommand2.Parameters.AddWithValue("@Area", area);
            deleteCommand2.Parameters.AddWithValue("@Area2", area2);

            connection.Open();
                deleteCommand2.ExecuteNonQuery();
                connection.Close();          
        }


        public void DeleteToDate2(DateTime date, string area)
        {

            var commandString2 = "DELETE FROM journalCollectors WHERE (date = @Date) and area = @Area";
            SQLiteCommand deleteCommand2 = new SQLiteCommand(commandString2, connection);

            deleteCommand2.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            deleteCommand2.Parameters.AddWithValue("@Area", area);

            connection.Open();
            deleteCommand2.ExecuteNonQuery();
            connection.Close();
        }


        public IEnumerable<journalCollector> SearchEmpty(DateTime date)
        {
           


            connection.Close();


            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT  jc.*, COALESCE(cc.image, @DefaultImage) AS image FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id WHERE jc.data ='Данные отсутствуют' AND jc.date= @Date ";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data = Data
                };

                yield return JournalCollector;
            }

            connection.Close();
        }


        public IEnumerable<journalCollector> SearchCollectorName(string name, DateTime date, string route, string area)
        {
            connection.Open();
            if (!string.IsNullOrEmpty(name))
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



            connection.Close();
            string area2 = "";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT  jc.*, COALESCE(cc.image, @DefaultImage) AS image FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id WHERE jc.name LIKE @Name AND jc.date= @Date AND jc.route LIKE @Route and (jc.area = @Area or jc.area = @Area2 or jc.dateWork LIKE '%РЕЗЕРВ%') ORDER BY CAST(jc.route2 AS INT);";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            getAllCommand.Parameters.AddWithValue("@Name", "" + name + "%");
            getAllCommand.Parameters.AddWithValue("@Route", "" + route + "%");
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));
            getAllCommand.Parameters.AddWithValue("@Area", area);
            getAllCommand.Parameters.AddWithValue("@Area2", area2);
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data = Data
                };

                yield return JournalCollector;
            }

            connection.Close();
        }


        public IEnumerable<journalCollector> SearchCollectorName0(string name, DateTime date, string route, string area)
        {
            connection.Open();
            if (!string.IsNullOrEmpty(name))
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

          

            connection.Close();

            string area2 = "";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT  jc.*, COALESCE(cc.image, @DefaultImage) AS image FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id WHERE jc.name LIKE @Name AND jc.date= @Date and jc.route NOT LIKE '%/2%' and jc.route NOT LIKE '%\2%' AND jc.route LIKE @Route  AND (jc.area = @Area or jc.area = @Area2 or jc.area2 = @Area) and (jc.area2 = @Area or jc.dateWork LIKE '%аршрут%' or jc.dateWork LIKE '%РЕЗЕРВ%') ORDER BY CAST(jc.route2 AS INT)";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            getAllCommand.Parameters.AddWithValue("@Name", "" + name + "%");
            getAllCommand.Parameters.AddWithValue("@Route", "" + route + "%");
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));
            getAllCommand.Parameters.AddWithValue("@Area", area);
            getAllCommand.Parameters.AddWithValue("@Area2", area2);
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data = Data
                };

                yield return JournalCollector;
            }

            connection.Close();
        }


        public IEnumerable<journalCollector> SearchCollectorName3(DateTime date)
        {
           
            connection.Close();


            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT  jc.*, COALESCE(cc.image, @DefaultImage) AS image FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id WHERE jc.date= @Date and jc.permission !='.' and jc.name !='';";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data = Data
                };

                yield return JournalCollector;
            }

            connection.Close();
        }


        public IEnumerable<journalCollector> SearchCollectorName4(DateTime date)
        {

            connection.Close();


            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @"SELECT  jc.*, COALESCE(cc.image, @DefaultImage) AS image FROM journalCollectors jc LEFT JOIN cashCollectors cc ON jc.id2 = cc.id WHERE jc.date= @Date and jc.permission !='.' and jc.name !='' GROUP BY jc.name  ORDER BY jc.name ;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
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
                var Data = reader.GetString(reader.GetOrdinal("data"));

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
                    image = Image,
                    data = Data
                };

                yield return JournalCollector;
            }

            connection.Close();
        }


        public IEnumerable<journalCollector> SearchCollectorName5(DateTime date, string area)
        {
            connection.Close();
            string area2 = "";
            if (area == "Все") { area = "пр.Дзержинского, 69"; area2 = "ул.Фабрициуса, 8б"; }
            var commandString = @"SELECT route2,
       COALESCE(GROUP_CONCAT(DISTINCT CASE WHEN profession LIKE '%тарший%' THEN name END), '') AS names_starshego,
       COALESCE(GROUP_CONCAT(DISTINCT CASE WHEN profession LIKE '%борщик%' THEN name END), '') AS names_sborschika
FROM journalCollectors 
WHERE 
    date = @Date AND (profession LIKE '%тарший%' OR profession LIKE '%борщик%') AND (area = @Area or area = @Area2) and name != '' 
                              AND date = @Date
                              AND name NOT LIKE '%[^a-zA-Z0-9]%' 
                              AND route2 != 'РЕЗЕРВ' 
                              AND route2 != 'стажер'  
                              AND name IS NOT NULL AND route NOT IN (SELECT route FROM journalCollectors  WHERE dateWork LIKE '%АТМ%' or dateWork LIKE '%еревозка%')
GROUP BY route2
ORDER BY 
    CAST(route2 AS INT)
;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            getAllCommand.Parameters.AddWithValue("@Area", area);
            getAllCommand.Parameters.AddWithValue("@Area2", area2);
            connection.Open();
            var reader = getAllCommand.ExecuteReader();

            while (reader.Read())
            {
                // Получение значений из результата запроса
                var Route2 = reader.GetString(reader.GetOrdinal("route2"));
                var NamesStarshego = reader.GetString(reader.GetOrdinal("names_starshego"));
                var NamesSborschika = reader.GetString(reader.GetOrdinal("names_sborschika"));

                // Создание объекта journalCollector и заполнение его данными из результата запроса
                var JournalCollector = new journalCollector
                {
                    route2 = Route2,
                    name = NamesStarshego,
                    name2 = NamesSborschika
                };

                // Возвращение объекта journalCollector в качестве результата
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



        public bool ImportSerchData(DateTime date, string area)
        {
            connection.Open();

            string selectQuery = "SELECT COUNT(*) FROM journalCollectors WHERE date = @Date AND area = @Area";
            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
            {
                selectCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                selectCommand.Parameters.AddWithValue("@Area", area);
                int count = Convert.ToInt32(selectCommand.ExecuteScalar());

                connection.Close();

                return count > 0;
            }

        }

        public delegate void ProgressUpdateDelegate(int progressPercentage);

        //public void ImportExcelToDatabase(string filePath, DateTime date, string area, BackgroundWorker worker, ProgressUpdateDelegate progressCallback)
        //{
        //    string raute = string.Empty;
        //    Excel.Application excel = null;
        //    Excel.Workbook workbook = null;

        //    try
        //    {
        //        // Создание объекта подключения SQLite
        //        using (SQLiteConnection connection = new SQLiteConnection("Data Source=B.I.G.db"))
        //        {
        //            // Открытие подключения
        //            connection.Open();

        //            // Создание объекта команды SQLite
        //            SQLiteCommand command = new SQLiteCommand();

        //            // Привязка команды к объекту подключения
        //            command.Connection = connection;

        //            // Создание объекта Excel
        //            excel = new Excel.Application();

        //            // Открытие книги Excel по пути к файлу
        //            workbook = excel.Workbooks.Open(filePath);

        //            // Выбор листа Excel для чтения данных
        //            Excel._Worksheet worksheet = workbook.Sheets[1];

        //            // Получение диапазона ячеек для чтения данных
        //            Excel.Range range = worksheet.UsedRange;

        //            // Определение количества строк в таблице Excel
        //            int rowCount = range.Rows.Count;
        //            string data="";
        //            // Проход по строкам диапазона
        //            for (int row = 10; row <= rowCount; row++)
        //            {
        //                data = "";
        //                // Получение значений из колонок B и C
        //                string profession = (range.Cells[row, 2].Value2 ?? "").ToString();
        //                string name = (range.Cells[row, 3].Value2 ?? "").ToString();                      
                       

        //                string dateWork = string.Empty;
        //                object cellValue = range.Cells[row, 4].Value2;

        //                if (cellValue != null)
        //                {
        //                    if (double.TryParse(cellValue.ToString(), out double oaDate))
        //                    {
        //                        // Преобразование числа в DateTime и форматирование в строку времени
        //                        dateWork = DateTime.FromOADate(oaDate).ToString("HH:mm");
        //                    }
        //                    else
        //                    {
        //                        dateWork = cellValue.ToString();
        //                    }
        //                }


        //                string appropriation = string.Empty;
        //                Excel.Range cell = range.Cells[row, 7];

        //                if (cell.Value2 != null)
        //                {
        //                    if (cell.HasFormula)
        //                    {
        //                        appropriation = string.Empty;
        //                    }
        //                    else
        //                    {
        //                        string cellValueString = cell.Value2.ToString();

        //                        // Проверяем длину строки
        //                        if (cellValueString.Length == 4)
        //                        {
        //                            // Если строка состоит из четырех символов, оставляем её как есть
        //                            appropriation = cellValueString;
        //                        }
        //                        else if (double.TryParse(cellValueString, out double oaDate))
        //                        {
        //                            // Преобразование числа в DateTime и форматирование в строку времени
        //                            appropriation = DateTime.FromOADate(oaDate).ToString("HH:mm");
        //                        }
        //                        else if (DateTime.TryParse(cellValueString, out DateTime parsedDate))
        //                        {
        //                            // Преобразование строки, которая является временем, в строку времени
        //                            appropriation = parsedDate.ToString("HH:mm");
        //                        }
        //                        else
        //                        {
        //                            // Оставляем строку как есть, если это не число и не время
        //                            appropriation = cellValueString;
        //                        }
        //                    }
        //                }


        //                // Создание SQL-запроса для вставки данных в таблицу journalCollectors
        //                string query = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES (@Profession, @Name, @Gun, @Automaton_serial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @Fullname, @Phone, @Id2, @Route, @Date, @DateWork, @Appropriation,@Route2, @Data, @Area, @Area2 )";

        //                // Привязка SQL-запроса к объекту команды
        //                command.CommandText = query;

        //                // Создание параметров для SQL-запроса
        //                command.Parameters.Clear(); // Очистка параметров
        //                if (name != "")
        //                {
        //                    string selectQuery = "SELECT COUNT(*) FROM cashCollectors WHERE REPLACE(REPLACE(REPLACE(name, ' ', ''), '.', ','), ',', '.') = REPLACE(REPLACE(REPLACE(@Name, ' ', ''), '.', ','), ',', '.')";
        //                    using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
        //                    {
        //                        selectCommand.Parameters.AddWithValue("@Name", name);
        //                        long existingRecords = (long)selectCommand.ExecuteScalar();
        //                        if (existingRecords == 0)
        //                        {
        //                          data = "Данные отсутствуют";
        //                        }
        //                    }
        //                }

        //                command.Parameters.Add(new SQLiteParameter("@Profession", DbType.String) { Value = profession });
        //                command.Parameters.Add(new SQLiteParameter("@Name", DbType.String) { Value = name });
        //                command.Parameters.Add(new SQLiteParameter("@Gun", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Automaton_serial", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Automaton", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Permission", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Meaning", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Certificate", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Token", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Power", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Fullname", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Phone", DbType.String) { Value = "" });
        //                command.Parameters.Add(new SQLiteParameter("@Id2", DbType.Int32) { Value = 0 }); // Предполагая, что это int, иначе укажите правильный тип данных
        //                if (!profession.Contains("тарший") && !profession.Contains("орщик") && !profession.Contains("одитель")) { raute = profession; }
        //                command.Parameters.Add(new SQLiteParameter("@Route", DbType.String) { Value = raute });
        //                command.Parameters.Add(new SQLiteParameter("@Date", DbType.String) { Value = date.ToString("yyyy-MM-dd") });
        //                command.Parameters.Add(new SQLiteParameter("@DateWork", DbType.String) { Value = dateWork });
        //                command.Parameters.Add(new SQLiteParameter("@Appropriation", DbType.String) { Value = appropriation });
        //                command.Parameters.Add(new SQLiteParameter("@Route2", DbType.String) { Value = raute });
        //                command.Parameters.Add(new SQLiteParameter("@Data", DbType.String) { Value = data });
        //                command.Parameters.Add(new SQLiteParameter("@Area", DbType.String) { Value = area });
        //                command.Parameters.Add(new SQLiteParameter("@Area2", DbType.String) { Value = area });

        //                // Выполнение SQL-запроса
        //                command.ExecuteNonQuery();
        //                UpdateJournalCollectorsFromCashCollectors(name);
        //                // Сообщение о прогрессе через делегат
        //                int progressPercentage = (int)((row - 5) / (float)(rowCount - 5) * 100);
        //                progressCallback(progressPercentage);
        //            }

        //            // Закрытие подключения SQLite
        //            connection.Close();

        //            // Закрытие книги Excel
        //            workbook.Close(false);
        //        }

        //        // Закрытие приложения Excel
        //        if (excel != null)
        //        {
        //            excel.Quit();
        //            Marshal.ReleaseComObject(excel);
        //        }

              
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Произошла ошибка при загрузке данных из Excel: " + ex.Message);
        //    }
        //}

        public void ImportExcelToDatabase(string filePath, DateTime date, string area, BackgroundWorker worker, ProgressUpdateDelegate progressCallback)
        {
            string raute = string.Empty;
            Excel.Application excel = null;
            Excel.Workbook workbook = null;

            try
            {
                // Создание объекта подключения SQLite
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=B.I.G.db"))
                {
                    // Открытие подключения
                    connection.Open();

                    // Создание объекта команды SQLite
                    SQLiteCommand command = new SQLiteCommand();

                    // Привязка команды к объекту подключения
                    command.Connection = connection;

                    // Создание объекта Excel
                    excel = new Excel.Application();

                    // Открытие книги Excel по пути к файлу
                    workbook = excel.Workbooks.Open(filePath);

                    // Выбор листа Excel для чтения данных
                    Excel._Worksheet worksheet = workbook.Sheets[1];

                    // Получение диапазона ячеек для чтения данных
                    Excel.Range range = worksheet.UsedRange;

                    // Определение количества строк в таблице Excel
                    int rowCount = range.Rows.Count;
                    string data = "";

                    // Проход по строкам диапазона
                    for (int row = 10; row <= rowCount; row++)
                    {
                        data = "";
                        // Получение значений из колонок B и C
                        string profession = (range.Cells[row, 2].Value2 ?? "").ToString();
                        string name = (range.Cells[row, 3].Value2 ?? "").ToString();

                        string dateWork = string.Empty;
                        object cellValue = range.Cells[row, 4].Value2;

                        if (cellValue != null)
                        {
                            if (double.TryParse(cellValue.ToString(), out double oaDate))
                            {
                                // Преобразование числа в DateTime и форматирование в строку времени
                                dateWork = DateTime.FromOADate(oaDate).ToString("HH:mm");
                            }
                            else
                            {
                                dateWork = cellValue.ToString();
                            }
                        }

                        string appropriation = string.Empty;
                        Excel.Range cell = range.Cells[row, 7];

                        if (cell.Value2 != null)
                        {
                            if (cell.HasFormula)
                            {
                                appropriation = string.Empty;
                            }
                            else
                            {
                                string cellValueString = cell.Value2.ToString();

                                // Проверяем длину строки
                                if (cellValueString.Length == 4)
                                {
                                    // Если строка состоит из четырех символов, оставляем её как есть
                                    appropriation = cellValueString;
                                }
                                else if (double.TryParse(cellValueString, out double oaDate))
                                {
                                    // Преобразование числа в DateTime и форматирование в строку времени
                                    appropriation = DateTime.FromOADate(oaDate).ToString("HH:mm");
                                }
                                else if (DateTime.TryParse(cellValueString, out DateTime parsedDate))
                                {
                                    // Преобразование строки, которая является временем, в строку времени
                                    appropriation = parsedDate.ToString("HH:mm");
                                }
                                else
                                {
                                    // Оставляем строку как есть, если это не число и не время
                                    appropriation = cellValueString;
                                }
                            }
                        }

                        // Создание SQL-запроса для вставки данных в таблицу journalCollectors
                        string query = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES (@Profession, @Name, @Gun, @Automaton_serial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @Fullname, @Phone, @Id2, @Route, @Date, @DateWork, @Appropriation,@Route2, @Data, @Area, @Area2 )";

                        // Привязка SQL-запроса к объекту команды
                        command.CommandText = query;

                        // Создание параметров для SQL-запроса
                        command.Parameters.Clear(); // Очистка параметров
                        if (name != "")
                        {
                            string selectQuery = "SELECT COUNT(*) FROM cashCollectors WHERE REPLACE(REPLACE(REPLACE(name, ' ', ''), '.', ','), ',', '.') = REPLACE(REPLACE(REPLACE(@Name, ' ', ''), '.', ','), ',', '.')";
                            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                            {
                                selectCommand.Parameters.AddWithValue("@Name", name);
                                long existingRecords = (long)selectCommand.ExecuteScalar();
                                if (existingRecords == 0)
                                {
                                    data = "Данные отсутствуют";
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
                        if (!profession.Contains("тарший") && !profession.Contains("орщик") && !profession.Contains("одитель")) { raute = profession; }
                        command.Parameters.Add(new SQLiteParameter("@Route", DbType.String) { Value = raute });
                        command.Parameters.Add(new SQLiteParameter("@Date", DbType.String) { Value = date.ToString("yyyy-MM-dd") });
                        command.Parameters.Add(new SQLiteParameter("@DateWork", DbType.String) { Value = dateWork });
                        command.Parameters.Add(new SQLiteParameter("@Appropriation", DbType.String) { Value = appropriation });
                        command.Parameters.Add(new SQLiteParameter("@Route2", DbType.String) { Value = raute });
                        command.Parameters.Add(new SQLiteParameter("@Data", DbType.String) { Value = data });
                        command.Parameters.Add(new SQLiteParameter("@Area", DbType.String) { Value = area });
                        command.Parameters.Add(new SQLiteParameter("@Area2", DbType.String) { Value = area });

                        // Выполнение SQL-запроса
                        command.ExecuteNonQuery();

                        // Вставка дополнительной строки, если profession содержит "орщик"
                        if (profession.Contains("орщик"))
                        {
                            string queryDriver = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES (@ProfessionDriver, @Name, @Gun, @Automaton_serial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @Fullname, @Phone, @Id2, @Route, @Date, @DateWork, @Appropriation,@Route2, @Data, @Area, @Area2 )";

                            SQLiteCommand commandDriver = new SQLiteCommand(queryDriver, connection);

                            commandDriver.Parameters.Add(new SQLiteParameter("@ProfessionDriver", DbType.String) { Value = "Водитель" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Name", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Gun", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Automaton_serial", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Automaton", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Permission", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Meaning", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Certificate", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Token", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Power", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Fullname", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Phone", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Id2", DbType.Int32) { Value = 0 });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Route", DbType.String) { Value = raute });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Date", DbType.String) { Value = date.ToString("yyyy-MM-dd") });
                            commandDriver.Parameters.Add(new SQLiteParameter("@DateWork", DbType.String) { Value = dateWork });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Appropriation", DbType.String) { Value = appropriation });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Route2", DbType.String) { Value = raute });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Data", DbType.String) { Value = "" });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Area", DbType.String) { Value = area });
                            commandDriver.Parameters.Add(new SQLiteParameter("@Area2", DbType.String) { Value = area });

                            commandDriver.ExecuteNonQuery();
                        }

                        UpdateJournalCollectorsFromCashCollectors(name);

                        // Сообщение о прогрессе через делегат
                        int progressPercentage = (int)((row - 5) / (float)(rowCount - 5) * 100);
                        progressCallback(progressPercentage);
                    }

                    // Закрытие подключения SQLite
                    connection.Close();

                    // Закрытие книги Excel
                    workbook.Close(false);
                }

                // Закрытие приложения Excel
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных из Excel: " + ex.Message);
            }
        }


        public void ImportExcelToDatabase2(string filePath, DateTime date, string area, BackgroundWorker worker, ProgressUpdateDelegate progressCallback)
        {
            string raute = string.Empty;
            Excel.Application excel = null;
            Excel.Workbook workbook = null;

            try
            {
                // Создание объекта подключения SQLite
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=B.I.G.db"))
                {
                    // Открытие подключения
                    connection.Open();

                    // Создание объекта команды SQLite
                    SQLiteCommand command = new SQLiteCommand();

                    // Привязка команды к объекту подключения
                    command.Connection = connection;

                    // Создание объекта Excel
                    excel = new Excel.Application();

                    // Открытие книги Excel по пути к файлу
                    workbook = excel.Workbooks.Open(filePath);

                    // Выбор листа Excel для чтения данных
                    Excel._Worksheet worksheet = workbook.Sheets[1];

                    // Получение диапазона ячеек для чтения данных
                    Excel.Range range = worksheet.UsedRange;

                    // Определение количества строк в таблице Excel
                    int rowCount = range.Rows.Count;
                    string data = "";
                    // Проход по строкам диапазона
                    for (int row = 5; row <= rowCount; row++)
                    {
                        data = "";
                        // Получение значений из колонок B и C
                        string profession = (range.Cells[row, 2].Value2 ?? "").ToString();
                        string name = (range.Cells[row, 3].Value2 ?? "").ToString();


                        string dateWork = string.Empty;
                        object cellValue = range.Cells[row, 4].Value2;

                        if (cellValue != null)
                        {
                            if (double.TryParse(cellValue.ToString(), out double oaDate))
                            {
                                // Преобразование числа в DateTime и форматирование в строку времени
                                dateWork = DateTime.FromOADate(oaDate).ToString("HH:mm");
                            }
                            else
                            {
                                dateWork = cellValue.ToString();
                            }
                        }


                        string appropriation = string.Empty;
                        Excel.Range cell = range.Cells[row, 7];

                        if (cell.Value2 != null)
                        {
                            if (cell.HasFormula)
                            {
                                appropriation = string.Empty;
                            }
                            else
                            {
                                string cellValueString = cell.Value2.ToString();

                                // Проверяем длину строки
                                if (cellValueString.Length == 4)
                                {
                                    // Если строка состоит из четырех символов, оставляем её как есть
                                    appropriation = cellValueString;
                                }
                                else if (double.TryParse(cellValueString, out double oaDate))
                                {
                                    // Преобразование числа в DateTime и форматирование в строку времени
                                    appropriation = DateTime.FromOADate(oaDate).ToString("HH:mm");
                                }
                                else if (DateTime.TryParse(cellValueString, out DateTime parsedDate))
                                {
                                    // Преобразование строки, которая является временем, в строку времени
                                    appropriation = parsedDate.ToString("HH:mm");
                                }
                                else
                                {
                                    // Оставляем строку как есть, если это не число и не время
                                    appropriation = cellValueString;
                                }
                            }
                        }


                        // Создание SQL-запроса для вставки данных в таблицу journalCollectors
                        string query = "INSERT INTO journalCollectors (profession, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2 ) VALUES (@Profession, @Name, @Gun, @Automaton_serial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @Fullname, @Phone, @Id2, @Route, @Date, @DateWork, @Appropriation,@Route2, @Data, @Area, @Area2 )";

                        // Привязка SQL-запроса к объекту команды
                        command.CommandText = query;

                        // Создание параметров для SQL-запроса
                        command.Parameters.Clear(); // Очистка параметров
                        if (name != "")
                        {
                            string selectQuery = "SELECT COUNT(*) FROM cashCollectors WHERE REPLACE(REPLACE(REPLACE(name, ' ', ''), '.', ','), ',', '.') = REPLACE(REPLACE(REPLACE(@Name, ' ', ''), '.', ','), ',', '.')";
                            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                            {
                                selectCommand.Parameters.AddWithValue("@Name", name);
                                long existingRecords = (long)selectCommand.ExecuteScalar();
                                if (existingRecords == 0)
                                {
                                    data = "Данные отсутствуют";
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
                        if (!profession.Contains("тарший") && !profession.Contains("орщик") && !profession.Contains("одитель") && !profession.Contains("Деж")) { raute = profession; }
                        command.Parameters.Add(new SQLiteParameter("@Route", DbType.String) { Value = raute });
                        command.Parameters.Add(new SQLiteParameter("@Date", DbType.String) { Value = date.ToString("yyyy-MM-dd") });
                        command.Parameters.Add(new SQLiteParameter("@DateWork", DbType.String) { Value = dateWork });
                        command.Parameters.Add(new SQLiteParameter("@Appropriation", DbType.String) { Value = appropriation });
                        command.Parameters.Add(new SQLiteParameter("@Route2", DbType.String) { Value = raute });
                        command.Parameters.Add(new SQLiteParameter("@Data", DbType.String) { Value = data });
                        command.Parameters.Add(new SQLiteParameter("@Area", DbType.String) { Value = "пр.Дзержинского, 69" });
                        command.Parameters.Add(new SQLiteParameter("@Area2", DbType.String) { Value = ""});

                        // Выполнение SQL-запроса
                        command.ExecuteNonQuery();
                        UpdateJournalCollectorsFromCashCollectors(name);
                        // Сообщение о прогрессе через делегат
                        int progressPercentage = (int)((row - 5) / (float)(rowCount - 5) * 100);
                        progressCallback(progressPercentage);
                    }

                    // Закрытие подключения SQLite
                    connection.Close();

                    // Закрытие книги Excel
                    workbook.Close(false);
                }

                // Закрытие приложения Excel
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных из Excel: " + ex.Message);
            }
        }



        private void UpdateJournalCollectorsFromCashCollectors(string name)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=B.I.G.db"))
                {
                    connection.Open();

                    string updateQuery = @"
                UPDATE journalCollectors
                SET
                    gun = c.gun,
                    automaton_serial = c.automaton_serial,
                    automaton = c.automaton,
                    permission = c.permission,
                    meaning = c.meaning,
                    certificate = c.certificate,
                    token = c.token,
                    power = c.power,
                    fullname = c.fullname,
                    phone = c.phone,
                    id2 = c.id,
                    area2 = c.area
                FROM cashCollectors c
                WHERE
                    REPLACE(REPLACE(REPLACE(c.name, ' ', ''), '.', ','), ',', '.') = REPLACE(REPLACE(REPLACE(@Name, ' ', ''), '.', ','), ',', '.') 
                    AND journalCollectors.name = @Name";

                    SQLiteCommand command = new SQLiteCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@Name", name);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении данных из cashCollectors: " + ex.Message);
            }
        }


        public void UpdateJournalBase2(DateTime date)
        {
            // Путь к исходной базе данных (корень программы)
            string sourceDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "B.I.G.db");

            // Путь к целевой базе данных (из переменной MainWindow.puth)
            string destinationDbPath = Path.Combine(MainWindow.puth, "B.I.G.db");

            var journalEntries = new List<journalCollector>();

            try
            {
                // Чтение данных из исходной базы данных
                using (SQLiteConnection sourceConnection = new SQLiteConnection($"Data Source={sourceDbPath};Version=3;"))
                {
                    sourceConnection.Open();
                    var commandString = "SELECT * FROM journalCollectors WHERE date = @Date";
                    using (SQLiteCommand selectCommand = new SQLiteCommand(commandString, sourceConnection))
                    {
                        selectCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

                        using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var entry = new journalCollector
                                {
                                    id = reader.GetInt32(0),
                                    name = reader.GetString(1),
                                    gun = reader.GetString(2),
                                    automaton_serial = reader.GetString(3),
                                    automaton = reader.GetString(4),
                                    permission = reader.GetString(5),
                                    meaning = reader.GetString(6),
                                    certificate = reader.GetString(7),
                                    token = reader.GetString(8),
                                    power = reader.GetString(9),
                                    fullname = reader.GetString(10),
                                    profession = reader.GetString(11),
                                    phone = reader.GetString(12),
                                    id2 = reader.GetInt32(13),
                                    route = reader.GetString(14),
                                    date = reader.GetDateTime(15),
                                    dateWork = reader.GetString(16),
                                    appropriation = reader.GetString(17),
                                    route2 = reader.GetString(18),
                                    data = reader.GetString(19),
                                    area = reader.GetString(20),
                                    area2= reader.GetString(21),    
                                };
                                journalEntries.Add(entry);
                            }
                        }
                    }
                }

                // Перезапись данных в целевой базе данных
                using (SQLiteConnection destinationConnection = new SQLiteConnection($"Data Source={destinationDbPath};Version=3;"))
                {
                    destinationConnection.Open();

                    using (var transaction = destinationConnection.BeginTransaction())
                    {
                        try
                        {
                            // Удаление существующих данных с той же датой
                            var deleteCommandString = "DELETE FROM journalCollectors WHERE date = @Date";
                            using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteCommandString, destinationConnection))
                            {
                                deleteCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Вставка новых данных
                            var insertCommandString = @"
                    INSERT INTO journalCollectors (
                        id, name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, profession, phone, id2, route, date, dateWork, appropriation, route2, data, area, area2
                    ) VALUES (
                        @Id, @Name, @Gun, @AutomatonSerial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @FullName, @Profession, @Phone, @Id2, @Route, @Date, @DateWork, @Appropriation, @Route2, @Data, @Area, @Area2
                    )";

                            foreach (var entry in journalEntries)
                            {
                                using (SQLiteCommand insertCommand = new SQLiteCommand(insertCommandString, destinationConnection))
                                {
                                    insertCommand.Parameters.AddWithValue("@Id", entry.id);
                                    insertCommand.Parameters.AddWithValue("@Name", entry.name);
                                    insertCommand.Parameters.AddWithValue("@Gun", entry.gun);
                                    insertCommand.Parameters.AddWithValue("@AutomatonSerial", entry.automaton_serial);
                                    insertCommand.Parameters.AddWithValue("@Automaton", entry.automaton);
                                    insertCommand.Parameters.AddWithValue("@Permission", entry.permission);
                                    insertCommand.Parameters.AddWithValue("@Meaning", entry.meaning);
                                    insertCommand.Parameters.AddWithValue("@Certificate", entry.certificate);
                                    insertCommand.Parameters.AddWithValue("@Token", entry.token);
                                    insertCommand.Parameters.AddWithValue("@Power", entry.power);
                                    insertCommand.Parameters.AddWithValue("@FullName", entry.fullname);
                                    insertCommand.Parameters.AddWithValue("@Profession", entry.profession);
                                    insertCommand.Parameters.AddWithValue("@Phone", entry.phone);
                                    insertCommand.Parameters.AddWithValue("@Id2", entry.id2);
                                    insertCommand.Parameters.AddWithValue("@Route", entry.route);
                                    insertCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                                    insertCommand.Parameters.AddWithValue("@DateWork", entry.dateWork);
                                    insertCommand.Parameters.AddWithValue("@Appropriation", entry.appropriation);
                                    insertCommand.Parameters.AddWithValue("@Route2", entry.route2);
                                    insertCommand.Parameters.AddWithValue("@Data", entry.data);
                                    insertCommand.Parameters.AddWithValue("@Area", entry.area);
                                    insertCommand.Parameters.AddWithValue("@Area2", entry.area2);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                          
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Откат транзакции в случае ошибки
                            throw; // Переброс исключения для обработки ниже
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении данных из cashCollectors: " + ex.Message);
            }
        }



       
    }
}
