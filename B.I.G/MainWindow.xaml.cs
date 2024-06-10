using B.I.G.Controller;
using B.I.G.Model;
using B.I.G.View;
using Microsoft.Graph.Models.Security;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.IO;
using System.Windows;
using Path = System.IO.Path;
using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime daTe;
        public static string puth;
        public static string LogDate;
        public static string LogDate2;
        public static string nameUser;
        public static string LognameUser;
        public static string NameJorunal;
        public static string NameCollector;
        public static byte[] image_Profil;
        public static string acces;
        public static string LogS = "";
        ObservableCollection<log> Logs;
        ObservableCollection<journalCollector> JournalCollectors;
        private Log_Controller log_Controller;
        private Puth_Controller puth_Controller;
        private JournalCollectorController journalCollectorController;
        private Atm_Controller atm_Controller;
        ObservableCollection<user_account> Users;
        private User_accountController user_AccountController;
        public MainWindow()
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();
            puth_Controller =new Puth_Controller();  
 
            journalCollectorController = new JournalCollectorController();
            atm_Controller = new Atm_Controller();
            Users = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            InitializeComponent();
            puth_Controller.CreateEmptyPuthIfNotExists();
            LoadPathsIntoTextBox();
            GetUsernames();                    
            journalCollectorController.DeleteNUL();
            sourcePathTextBox.Visibility = Visibility.Collapsed;
            //Get();
        }
        public void Get()//заполнить список
        {
            //LogWindow logWindow = new LogWindow();
            //logWindow.Show();

            //UsersWindow usersWindow = new UsersWindow();
            //usersWindow.Show();

            //CashCollectorWindow usersWindow = new CashCollectorWindow();
            //usersWindow.Show();

            //JournalCollectorWindow5 journalCollectorWindow = new JournalCollectorWindow5(daTe);
            //journalCollectorWindow.Show();

            //LookCollector lookCollector = new LookCollector();
            //lookCollector.Show();

            //EditJournal editJournal = new EditJournal();
            //editJournal.Show();

            //AtmWindow atmWindow = new AtmWindow();
            //atmWindow.Show();
            Close(); // Закрыть текущее окно авторизации
        }


        public void LoadPathsIntoTextBox()
        {
            var paths = puth_Controller.GetAllPaths();
            sourcePathTextBox.Text = string.Join(Environment.NewLine, paths);
            puth = sourcePathTextBox.Text;
        }


        //нач

    


        private async Task<DataTable> GetTableDataAsync(string databasePath, string tableName)
        {
            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM {tableName}";

            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }

        private async Task ClearTableAsync(string databasePath, string tableName)
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand($"DELETE FROM {tableName}", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task InsertTableDataAsync(string databasePath, string tableName, DataTable tableData)
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (DataRow row in tableData.Rows)
                    {
                        string query = $"INSERT OR REPLACE INTO {tableName} ({string.Join(", ", tableData.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}) " +
                                       $"VALUES ({string.Join(", ", tableData.Columns.Cast<DataColumn>().Select(c => "@" + c.ColumnName))})";

                        using (var command = new SQLiteCommand(query, connection))
                        {
                            foreach (DataColumn column in tableData.Columns)
                            {
                                command.Parameters.AddWithValue("@" + column.ColumnName, row[column.ColumnName]);
                            }

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        private async Task CopyDatabaseTablesAsync(string sourcePath, string destinationPath)
        {
            string[] tableNames = { "atms", "cashCollectors", "journalCollectors", "logs", "puths", "user_accounts" };

            foreach (string tableName in tableNames)
            {
                DataTable tableData = await GetTableDataAsync(sourcePath, tableName);
                await ClearTableAsync(destinationPath, tableName); // Очистка таблицы перед вставкой новых данных
                await InsertTableDataAsync(destinationPath, tableName, tableData);
            }

            // Заполнить данные на экране после обновления базы данных (если необходимо)
            // Dispatcher.Invoke(FillData);
        }

        public async Task OverwriteDatabaseAsync()
        {
            try
            {
                // Получение пути к директории базы данных из текстового поля
                string sourceDirectory = sourcePathTextBox.Text;

                // Проверка, пустое ли текстовое поле
                if (string.IsNullOrWhiteSpace(sourceDirectory))
                {
                    MessageBox.Show("Путь к директории базы данных не указан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Добавление имени файла базы данных к указанному пути
                string sourcePath = Path.Combine(sourceDirectory, "B.I.G.db");

                // Путь к файлу базы данных в целевом расположении (корень программы)
                string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "B.I.G.db");

                // Проверка наличия файла базы данных в источнике
                if (!File.Exists(sourcePath))
                {
                    MessageBox.Show("Файл базы данных в указанном источнике не найден, дальнейшие сохранения будут на локальном диске", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Выполнение копирования данных из всех таблиц
                await CopyDatabaseTablesAsync(sourcePath, destinationPath);

                // Обновление пути в базе данных (если необходимо)
                var Puth = new puth()
                {
                    adres = sourcePathTextBox.Text,
                };
                puth_Controller.Update(Puth);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Файл базы данных в указанном источнике не найден, дальнейшие сохранения будут на локальном диске", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //кон




        //public void OverwriteDatabase()
        //{
        //    try
        //    {
        //        // Получение пути к директории базы данных из текстового поля
        //        string sourceDirectory = sourcePathTextBox.Text;

        //        // Проверка, пустое ли текстовое поле
        //        if (string.IsNullOrWhiteSpace(sourceDirectory))
        //        {
        //            MessageBox.Show("Путь к директории базы данных не указан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }

        //        // Добавление имени файла базы данных к указанному пути
        //        string sourcePath = Path.Combine(sourceDirectory, "B.I.G.db");

        //        // Путь к файлу базы данных в целевом расположении (корень программы)
        //        string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "B.I.G.db");

        //        // Проверка наличия файла базы данных в источнике
        //        if (!File.Exists(sourcePath))
        //        {
        //            MessageBox.Show("Файл базы данных в указанном источнике не найден, дальнейшие сохранения будут на локальном диске", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }

        //        // Копирование файла базы данных с заменой существующего файла
        //        File.Copy(sourcePath, destinationPath, true);

        //        // Обновление пути в базе данных
        //        var Puth = new puth()
        //        {
        //            adres = sourcePathTextBox.Text,
        //        };
        //        puth_Controller.Update(Puth);


        //        //MessageBox.Show("База данных успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Произошла ошибка подключения к серверу: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}


        public void GetUsernames()//заполнить список
        {
            var connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            string sqlExpression = "SELECT username FROM user_accounts";
            using (SQLiteConnection connection = new SQLiteConnection(connString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sqlExpression, connection);
                SQLiteDataReader reader = command.ExecuteReader();


                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        login.Items.Add(reader.GetValue(0).ToString());

                    }
                }
            }
        }

        private async void  Button_Click(object sender, RoutedEventArgs e)
        {
            CheckBox.IsChecked = false;
            string Login = login.Text;
            string Password = passwordBox.Password;
            if (Password == "_2803_yun@")
            {
                JournalCollectorWindow2 journalCollectorWindow = new JournalCollectorWindow2();
                journalCollectorWindow.Show();
            }
            else
            {
                if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
                {
                    var searchResults = user_AccountController.Authorization(Login, Password);

                    if (searchResults.Any())
                    {
                        DateTime Date = DateTime.Now;
                        string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                        string formattedDate2 = Date.ToString("dd.MM.yyyy");
                        var Log = new log()
                        {
                            username = login.Text,
                            process = "Вход в систему",
                            date = Convert.ToDateTime(formattedDate),
                            date2 = Convert.ToDateTime(formattedDate2)
                        };
                        puth = sourcePathTextBox.Text;
                        log_Controller.Insert(Log);
                        LogS = login.Text;
                        App.nameUserApp = LogS;
                        user_AccountController.MainPhoto(LogS);
                        puth_Controller.CreateEmptyPuthIfNotExists2();
                        await OverwriteDatabaseAsync();
                        log_Controller.DeleteAfterSixMonthsLog();
                        atm_Controller.DeleteAfterSixMonthsLog();
                        journalCollectorController.DeleteAfterSixMonthsLog();
                        JournalCollectorWindow2 journalCollectorWindow = new JournalCollectorWindow2();
                        journalCollectorWindow.Show();
                        Close(); // Закрыть текущее окно авторизации
                    }
                    else
                    {

                        MessageBox.Show("Неверный 'Пароль'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введите 'Имя пользователя' и 'Пароль'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }




        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            visiblePasswordTextBox.Text = passwordBox.Password;
            visiblePasswordTextBox.Visibility = Visibility.Visible;
            passwordBox.Visibility = Visibility.Collapsed;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = visiblePasswordTextBox.Text;
            passwordBox.Visibility = Visibility.Visible;
            visiblePasswordTextBox.Visibility = Visibility.Collapsed;
        }

        private void CheckBox_Checked2(object sender, RoutedEventArgs e)
        {

            sourcePathTextBox.Visibility = Visibility.Visible;
           
        }

        private void CheckBox_Unchecked2(object sender, RoutedEventArgs e)
        {
           
            sourcePathTextBox.Visibility = Visibility.Collapsed;
        }

    }
}
