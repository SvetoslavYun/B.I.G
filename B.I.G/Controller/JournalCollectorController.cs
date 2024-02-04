﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using B.I.G.Model;


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

            var commandString = @"SELECT jc.*, CASE WHEN cc.image IS NULL THEN @DefaultImage ELSE cc.image END AS image   FROM journalCollectors jc  LEFT JOIN cashCollectors cc ON jc.id2 = cc.id";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));

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
                var Id2 = reader.GetInt32(13);
                var Route = reader.GetString(14);
                var Date = reader.GetDateTime(15);
                var Image = (byte[])reader.GetValue(16);

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


        public IEnumerable<journalCollector> SearchCollectorName(string name)
        {
            connection.Close();
            var defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

            var commandString = @" SELECT  jc.*, COALESCE(cc.image, @DefaultImage) AS image FROM journalCollectors jc  LEFT JOIN cashCollectors cc ON jc.id2 = cc.id  WHERE  jc.name LIKE @Name;";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
            getAllCommand.Parameters.AddWithValue("@DefaultImage", File.ReadAllBytes(defaultImagePath));

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
                var Id2 = reader.GetInt32(13);
                var Route = reader.GetString(14);
                var Date = reader.GetDateTime(15);
                var Image = (byte[])reader.GetValue(16);

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
                    image = Image ?? File.ReadAllBytes(defaultImagePath)
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




    }
}
