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
using DocumentFormat.OpenXml.Math;

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для Add_User.xaml
    /// </summary>
    public partial class Add_СashCollector : Window
    {
        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;
        private string originalName;
        public cashCollector SelectedProduct { get; set; }
        ObservableCollection<cashCollector> CashCollectors;
        private СashCollectorController cashCollectorController;
        public static byte[] image_bytes;
        public Add_СashCollector()
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();
            CashCollectors = new ObservableCollection<cashCollector>();
            cashCollectorController = new СashCollectorController();
            InitializeComponent();
            Loaded += AddUserWindow_Loaded;
            grid.DataContext = CashCollectorWindow.CashCollector;
            Loaded += BD_Form_Loaded;
        }

        private void BD_Form_Loaded(object sender, RoutedEventArgs e)
        {
            originalName = Name.Text;
        }

        private void AddUserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Profession.Items.Add("Инкассатор");
            Profession.Items.Add("Водитель");
        }
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Name.Text) || string.IsNullOrWhiteSpace(Fullname.Text) || string.IsNullOrWhiteSpace(Gun.Text) || string.IsNullOrWhiteSpace(Automaton_serial.Text) || string.IsNullOrWhiteSpace(Automaton.Text) || string.IsNullOrWhiteSpace(Permission.Text) || string.IsNullOrWhiteSpace(Certificate.Text) || string.IsNullOrWhiteSpace(Token.Text) || string.IsNullOrWhiteSpace(Power.Text) || string.IsNullOrWhiteSpace(Profession.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Name.BorderBrush = Brushes.Red;
                if (string.IsNullOrWhiteSpace(Name.Text)) { Name.BorderBrush = Brushes.Red; } else { Name.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Fullname.Text)) { Fullname.BorderBrush = Brushes.Red; } else { Fullname.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Gun.Text)) { Gun.BorderBrush = Brushes.Red; } else { Gun.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Automaton_serial.Text)) { Automaton_serial.BorderBrush = Brushes.Red; } else { Automaton_serial.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Automaton.Text)) { Automaton.BorderBrush = Brushes.Red; } else { Automaton.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Permission.Text)) { Permission.BorderBrush = Brushes.Red; } else { Permission.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Certificate.Text)) { Certificate.BorderBrush = Brushes.Red; } else { Certificate.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Token.Text)) { Token.BorderBrush = Brushes.Red; } else { Token.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Power.Text)) { Power.BorderBrush = Brushes.Red; } else { Power.BorderBrush = Brushes.Black; }
                if (string.IsNullOrWhiteSpace(Profession.Text)) { Profession.BorderBrush = Brushes.Red; } else { Profession.BorderBrush = Brushes.Black; }
                return;
            }

                if (CashCollectorWindow.flag)
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
                    var CashCollector = new cashCollector()
                    {
                        name = Name.Text,
                        fullname = Fullname.Text,
                        profession = Profession.Text,
                        phone = Phone.Text,
                        gun = Gun.Text,
                        automaton_serial = Automaton_serial.Text,
                        automaton = Automaton.Text,
                        permission = Permission.Text,
                        meaning = Meaning.Text,
                        certificate = Certificate.Text,
                        token = Token.Text,
                        power = Power.Text,
                        image = image_bytes
                    };

                    if (!cashCollectorController.IsCashCollectorExists(CashCollector.name))
                    {

                        cashCollectorController.Insert(CashCollector);
                        DateTime Date = DateTime.Now;
                        string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                        string formattedDate2 = Date.ToString("dd.MM.yyyy");
                        var Log = new log()
                        {
                            username = MainWindow.LogS,
                            process = "Добавил пользователя: " + Name.Text + "",
                            date = Convert.ToDateTime(formattedDate),
                            date2 = Convert.ToDateTime(formattedDate2)
                        };
                        log_Controller.Insert(Log);
                        Close();

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
                        int id = CashCollectorWindow.CashCollector.id;
                        cashCollectorController.SearchFoto(id);
                    }
                    CashCollectorWindow.CashCollector.name = Name.Text;
                    CashCollectorWindow.CashCollector.fullname = Fullname.Text;
                    CashCollectorWindow.CashCollector.phone = Phone.Text;
                    CashCollectorWindow.CashCollector.gun = Gun.Text;
                    CashCollectorWindow.CashCollector.automaton_serial = Automaton_serial.Text;
                    CashCollectorWindow.CashCollector.automaton = Automaton.Text;
                    CashCollectorWindow.CashCollector.permission = Permission.Text;
                    CashCollectorWindow.CashCollector.meaning = Meaning.Text;
                    CashCollectorWindow.CashCollector.certificate = Certificate.Text;
                    CashCollectorWindow.CashCollector.token = Token.Text;
                    CashCollectorWindow.CashCollector.power = Power.Text;
                    CashCollectorWindow.CashCollector.image = image_bytes;

                    if (!cashCollectorController.IsCashCollectorExists(CashCollectorWindow.CashCollector.name, CashCollectorWindow.CashCollector.id))
                    {

                        cashCollectorController.Update(CashCollectorWindow.CashCollector);
                        DateTime Date = DateTime.Now;
                        string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                        string formattedDate2 = Date.ToString("dd.MM.yyyy");
                        var Log = new log()
                        {
                            username = MainWindow.LogS,
                            process = "Изменил пользователя: " + Name.Text + "",
                            date = Convert.ToDateTime(formattedDate),
                            date2 = Convert.ToDateTime(formattedDate2)
                        };
                        log_Controller.Insert(Log);
                        image_bytes = null;
                        Close();

                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }


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
