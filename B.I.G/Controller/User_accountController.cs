using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using B.I.G.Model;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

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


        public IEnumerable<user_account> GetAllUsers()
        {
            var commandString = "SELECT * FROM user_accounts ";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            connection.Open();
            var reader = getAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Username = reader.GetString(1);
                var Password_hash = reader.GetString(2);
                var Access = reader.GetString(3);
                var Image = (byte[])reader.GetValue(4);
                var User_account = new user_account
                {
                    id = Id,
                    username = Username,
                    password_hash = Password_hash,
                    access= Access,
                    image = Image
                };
                yield return User_account;
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
  

        public void Delete(int id)
        {
            var commandString = "DELETE FROM user_accounts WHERE(id = @Id)";
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
            deleteCommand.Parameters.AddWithValue("Id", id);
            connection.Open();
            deleteCommand.ExecuteNonQuery();
            connection.Close();
        }


        public IEnumerable<user_account> Authorization(string login, string password)
        {
            var commandString = "SELECT * FROM user_accounts WHERE username LIKE @login AND password_hash LIKE @password ;";

            using (SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection))
            {
                getAllCommand.Parameters.AddWithValue("@login", "%" + login + "%");
                getAllCommand.Parameters.AddWithValue("@password", "%" + password + "%");

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
                        yield return User;
                    }
                }
            }
            // connection.Close(); // Закрытие соединения не требуется, так как using автоматически закроет его при выходе из блока.
        }


    }
}
