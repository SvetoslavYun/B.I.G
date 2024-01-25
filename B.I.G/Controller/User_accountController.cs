using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using B.I.G.Model;
using Microsoft.Data.Sqlite;

namespace B.I.G.Controller
{
    internal class User_accountController
    { private SQLiteConnection connection;

        public User_accountController()
        {
            // Получение строки подключения из файла конфигурации
            var connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            // Создание объекта подключения
            connection = new SQLiteConnection(connString);
        }

        public IEnumerable<user_account> Authorization(string login, string password)
        {
            var commandString = "SELECT * FROM user_accounts WHERE username LIKE @login AND password_hash LIKE @password ;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@login", "%" + login + "%");
            getAllCommand.Parameters.AddWithValue("@password", "%" + password + "%");
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Password_hash = reader.GetString(2);
                var User = new user_account
                {
                    id = Id,
                    username = Username,
                    password_hash= Password_hash
                };
                yield return User;
            }
            connection.Close();
        }
    }
}
