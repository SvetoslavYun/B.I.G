using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using B.I.G.Model;

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

    
        public IEnumerable<atm> GetAllAtm()
        {
            var commandString = "SELECT * FROM atms ";
            SQLiteCommand getAllCommand = new SQLiteCommand(commandString, connection);
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
                var Atm = new atm
                {
                    id = Id,
                    route = Route,
                    atmname = Atmname,
                    name = Name,
                    name2 = Name2,
                    date = Date
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

        public void Delete(int id, string username)
        {
            var commandString = "DELETE FROM user_accounts WHERE(id = @Id) and username !=@Username";
            SQLiteCommand deleteCommand = new SQLiteCommand(commandString, connection);
            deleteCommand.Parameters.AddWithValue("Id", id);
            deleteCommand.Parameters.AddWithValue("@Username", username);
            connection.Open();
            deleteCommand.ExecuteNonQuery();
            connection.Close();
        }

        public IEnumerable<user_account> SearchUsername(string name)
        {
            connection.Open();
            if (name != "")
            {
                string selectQuery = "SELECT COUNT(*) FROM user_accounts WHERE username = @Name";
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

    }
}
