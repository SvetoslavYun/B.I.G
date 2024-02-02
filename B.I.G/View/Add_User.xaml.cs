using B.I.G.Controller;
using B.I.G.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
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
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;
using Rectangle = System.Drawing.Rectangle;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Microsoft.Graph.Models;

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для Add_User.xaml
    /// </summary>
    public partial class Add_User : Window
    {
        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;
        private string originalName;
        public user_account SelectedProduct { get; set; }
        ObservableCollection<user_account> Users;
        private User_accountController user_AccountController;
        public static byte[] image_bytes;
        public Add_User()
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();
            Users = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            InitializeComponent();
            Loaded += AddUserWindow_Loaded;
            grid.DataContext = UsersWindow.User;
            Loaded += BD_Form_Loaded;
        }

        private void BD_Form_Loaded(object sender, RoutedEventArgs e)
        {
            originalName = Name.Text;
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
            if (UsersWindow.flag)
            {
                if (image_bytes == null)
            {
                
                string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");

                if (File.Exists(defaultImagePath))
                {
                    image_bytes = File.ReadAllBytes(defaultImagePath);
                }
                else
                {
                    MessageBox.Show("Изображение по умолчанию не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }        
                var User = new user_account()
                {
                    username = Name.Text,
                    password_hash = Password.Text,
                    access = Access.Text,
                    image = image_bytes
                };

                if (!user_AccountController.IsUsernameExists(User.username))
                {
                    if (IsPasswordValid(User.password_hash))
                    {
                                          
                        user_AccountController.Insert(User);
                        DateTime Date = DateTime.Now;
                        string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                        string formattedDate2 = Date.ToString("dd.MM.yyyy");
                        var Log = new log()
                        {
                            username = MainWindow.LogS,
                            process = "Добавил пользователя " + "'" + Name.Text + "'",
                            date = Convert.ToDateTime(formattedDate),
                            date2 = Convert.ToDateTime(formattedDate2)
                        };
                        log_Controller.Insert(Log);
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (image_bytes == null)
                {                    
                    int id = UsersWindow.User.id;
                    user_AccountController.SearchFoto(id);
                }
                UsersWindow.User.username = Name.Text;
                UsersWindow.User.password_hash = Password.Text;
                UsersWindow.User.access = Access.Text;
                UsersWindow.User.image = image_bytes;

               if (!user_AccountController.IsUsernameExists(UsersWindow.User.username, UsersWindow.User.id))
    {
        if (IsPasswordValid(UsersWindow.User.password_hash))
                    {
                                        
                        if (originalName == MainWindow.LogS)
                        {
                            UsersWindow.flagEdit = true;
                            MainWindow.LogS = Name.Text;
                            MainWindow.image_Profil = image_bytes;
                            MainWindow.acces = Access.Text;
                        }
                        user_AccountController.Update(UsersWindow.User);
                        DateTime Date = DateTime.Now;
                        string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                        string formattedDate2 = Date.ToString("dd.MM.yyyy");
                        var Log = new log()
                        {
                            username = MainWindow.LogS,
                            process = "Изменил пользователя " + "'" + Name.Text + "'",
                            date = Convert.ToDateTime(formattedDate),
                            date2 = Convert.ToDateTime(formattedDate2)
                        };
                        log_Controller.Insert(Log);
                        image_bytes = null;
                  Close();
        }
    }
    else
    {
        MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
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

        private void Button_Foto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var originalImage = System.Drawing.Image.FromFile(openFileDialog.FileName))
                    {
                        int targetSize = 800; // Желаемый размер (ширина и высота одинаковые)

                        int sourceWidth = originalImage.Width;
                        int sourceHeight = originalImage.Height;

                        int cropX = 0;
                        int cropY = 0;

                        if (sourceWidth > sourceHeight)
                        {
                            // Исходное изображение шире, обрезаем сверху и снизу
                            sourceWidth = sourceHeight;
                            cropX = (originalImage.Width - sourceWidth) / 2;
                        }
                        else
                        {
                            // Исходное изображение выше, обрезаем справа и слева
                            sourceHeight = sourceWidth;
                            cropY = (originalImage.Height - sourceHeight) / 2;
                        }

                        using (var croppedImage = new Bitmap(sourceWidth, sourceHeight))
                        using (var graphics = Graphics.FromImage(croppedImage))
                        {
                            graphics.DrawImage(originalImage, new Rectangle(0, 0, sourceWidth, sourceHeight), cropX, cropY, sourceWidth, sourceHeight, GraphicsUnit.Pixel);
                            graphics.Save();

                            // Преобразовать Bitmap в массив байтов
                            using (MemoryStream ms = new MemoryStream())
                            {
                                croppedImage.Save(ms, ImageFormat.Jpeg); // Замените Jpeg на нужный вам формат
                                image_bytes = ms.ToArray();
                            }
                        }

                        // Загрузить изображение в Image элемент
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(image_bytes);
                        bitmapImage.EndInit();
                        imgBox.Source = bitmapImage;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обработке изображения: " + ex.Message);
                }
            }
        }


    }
}
