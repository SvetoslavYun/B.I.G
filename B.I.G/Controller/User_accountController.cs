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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Microsoft.Graph.Models.Security;
using DocumentFormat.OpenXml.Office2010.Excel;

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

        public void Update(user_account User)
        {
            var commandString = "UPDATE user_accounts SET username=@Username, password_hash=@Password_hash, access=@Access, image=@Image WHERE id = @Id";
            SQLiteCommand updateCommand = new SQLiteCommand(commandString, connection);
            updateCommand.Parameters.AddRange(new SQLiteParameter[] {
                 new SQLiteParameter("Username", User.username),
                new SQLiteParameter("Password_hash", User.password_hash),
                new SQLiteParameter("Access", User.access),
                new SQLiteParameter("Image", User.image),
                new SQLiteParameter("Id", User.id),
            });
            connection.Open();
            updateCommand.ExecuteNonQuery();
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

        public IEnumerable<user_account> SearchUsername(string name)
        {
            var commandString = "SELECT * FROM user_accounts WHERE username LIKE @Name;";

            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
            getAllCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
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
                    access = Access,
                    image = Image
                };
                yield return User_account;
            }
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
    }
}
