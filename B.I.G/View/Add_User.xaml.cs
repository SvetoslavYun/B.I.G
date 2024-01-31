using B.I.G.Controller;
using B.I.G.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для Add_User.xaml
    /// </summary>
    public partial class Add_User : Window
    {
        ObservableCollection<user_account> Users;
        private User_accountController user_AccountController;
        public Add_User()
        {
            Users = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            InitializeComponent();

            Loaded += AddUserWindow_Loaded;
        }

        private void AddUserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Access.Items.Add("Администратор");
            Access.Items.Add("Оператор");
        }
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Name.Text) || string.IsNullOrWhiteSpace(Password.Text) || string.IsNullOrWhiteSpace(Access.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Name.BorderBrush = Brushes.Red;
                if (string.IsNullOrWhiteSpace(Name.Text)){ Name.BorderBrush = Brushes.Red; } else { Name.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Password.Text)) { Password.BorderBrush = Brushes.Red; } else { Password.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Access.Text)) { Access.BorderBrush = Brushes.Red; } else { Access.BorderBrush = Brushes.Black; }
                return;
            }

            var User = new user_account()
            {
                username = Name.Text,
                password_hash = Password.Text,
                access = Access.Text
            };

            if (IsPasswordValid(User.password_hash))
            {
                user_AccountController.Insert(User);
            }
        }


        private bool IsPasswordValid(string password)
        {
            // Проверка на длину пароля (не менее 8 символов)
            if (password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка на наличие букв в верхнем и нижнем регистрах, цифр и спец символов
            //string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$";
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";

            if (!Regex.IsMatch(password, pattern))
            {
                MessageBox.Show("Пароль должен содержать не упорядоченный набор букв латинского алфавита в верхнем и нижнем регистрах и цифр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Пароль прошел все проверки
            return true;
        }

    }
}
