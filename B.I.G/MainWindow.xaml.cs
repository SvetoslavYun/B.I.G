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

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        private JournalCollectorController journalCollectorController;
        ObservableCollection<user_account> Users;
        private User_accountController user_AccountController;
        public MainWindow()
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();
 
            journalCollectorController = new JournalCollectorController();
            Users = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            InitializeComponent();
            GetUsernames();

            log_Controller.DeleteAfterSixMonthsLog();
            journalCollectorController.DeleteAfterSixMonthsLog();
            journalCollectorController.DeleteNUL();
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

            JournalCollectorWindow journalCollectorWindow = new JournalCollectorWindow();
            journalCollectorWindow.Show();

            //LookCollector lookCollector = new LookCollector();
            //lookCollector.Show();

            //EditJournal editJournal = new EditJournal();
            //editJournal.Show();

            Close(); // Закрыть текущее окно авторизации
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckBox.IsChecked = false;
            string Login = login.Text;
            string Password = passwordBox.Password;

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
                    log_Controller.Insert(Log);                  
                    LogS = login.Text;
                    App.nameUserApp = LogS;
                    user_AccountController.MainPhoto(LogS);
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

    }
}
