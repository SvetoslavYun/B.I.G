using B.I.G.Controller;
using B.I.G.Model;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace B.I.G.View
{
    /// <summary>
    /// Логика взаимодействия для RouteADD.xaml
    /// </summary>
    public partial class RouteADD : Window
    {
        ObservableCollection<journalCollector> JournalCollectors;
        private JournalCollectorController journalCollectorController;
        private DateTime Date;
        private string Area;
        public RouteADD(DateTime date, string area)
        {
            JournalCollectors = new ObservableCollection<journalCollector>();
            InitializeComponent();
            journalCollectorController = new JournalCollectorController();
            Circle.Items.Add("1");
            Circle.Items.Add("2");
            Date = date;
            Area = area;
        }
        // Метод для проверки ввода текста
        private void Route_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string newText = GetNewText(textBox, e.Text);

            // Разрешаем ввод только цифр и проверяем итоговую длину текста
            e.Handled = !IsTextAllowed(e.Text) || newText.Length > 3;
        }

        // Метод для проверки нажатия клавиш
        private void Route_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Разрешаем использование клавиш Backspace и Delete
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                return;
            }

            // Проверка длины текста при выделении и замене текста
            if (textBox != null && (textBox.SelectionLength > 0 && textBox.Text.Length - textBox.SelectionLength + 1 > 3))
            {
                e.Handled = true;
            }
        }

     

        // Вспомогательный метод для получения нового текста
        private static string GetNewText(TextBox textBox, string newText)
        {
            if (textBox.SelectionLength > 0)
            {
                string currentText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
                return currentText.Insert(textBox.SelectionStart, newText);
            }
            return textBox.Text.Insert(textBox.SelectionStart, newText);
        }


        private void DateWork_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits and colon
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void DateWork_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Block the space key
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private static bool IsTextAllowed(string text)
        {
            // Regular expression to match only digits and colon, disallow spaces
            Regex regex = new Regex("^[0-9:]*$");
            return regex.IsMatch(text);
        }

        private void DateWork_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;

            // Ensure the text always includes a colon at the third position
            if (text.Length > 2 && text[2] != ':')
            {
                text = text.Insert(2, ":");
            }

            // Handle input of the first character greater than 2
            if (text.Length > 0 && text[0] > '2')
            {
                text = "0" + text[0] + (text.Length > 1 ? text.Substring(1) : "");
            }

            // Handle input of the fourth character greater than 5
            if (text.Length > 4 && text[3] > '5')
            {
                text = text.Substring(0, 3) + "0" + text[3] + (text.Length > 4 ? text.Substring(4) : "");
            }

            // Correct length if necessary
            if (text.Length > 5)
            {
                text = text.Substring(0, 5);
            }

            // Ensure the TextBox is not empty
            if (string.IsNullOrEmpty(text) || text == ":")
            {
                text = "00:00";
            }

            // Set the corrected text back to the TextBox
            textBox.TextChanged -= DateWork_TextChanged; // Temporarily unsubscribe to avoid recursive calls
            textBox.Text = text;
            textBox.TextChanged += DateWork_TextChanged; // Re-subscribe
            textBox.CaretIndex = textBox.Text.Length;
        }

        private static bool IsValidTimeFormat(string text)
        {
            // Regular expression to validate time in HH:mm format and valid time values
            Regex regex = new Regex(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$");
            return regex.IsMatch(text);
        }

        private void DateWork_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus(); // Set focus to the TextBox
        }

        private void DateWork_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll(); // Выбираем весь текст, когда TextBox получает фокус
        }

        private void DateWork_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectionLength = 0; // Отменяем выделение при потере фокуса
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            //DateTime date, string area, string route, string circle

            if (string.IsNullOrWhiteSpace(Route.Text))
            {
                MessageBox.Show("Добавьте маршрут.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Route.BorderBrush = Brushes.Red;
                if (string.IsNullOrWhiteSpace(Route.Text)) { Route.BorderBrush = Brushes.Red; } else { Route.BorderBrush = Brushes.Black; }

                return;
            }

            if (string.IsNullOrWhiteSpace(Circle.Text))
            {
                MessageBox.Show("Выберите куг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(DateWork.Text))
            {
                MessageBox.Show("Укажите время.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DateWork.BorderBrush = Brushes.Red;
                if (string.IsNullOrWhiteSpace(DateWork.Text)) { DateWork.BorderBrush = Brushes.Red; } else { DateWork.BorderBrush = Brushes.Black; }

                return;
            }
            journalCollectorController.InsertRoute(Date,Area,Route.Text,Circle.Text);
            journalCollectorController.InsertRoute2(Date, Area, Route.Text, Circle.Text, DateWork.Text);
            journalCollectorController.InsertRoute3(Date, Area, Route.Text, Circle.Text, DateWork.Text);
            journalCollectorController.InsertRoute4(Date, Area, Route.Text, Circle.Text, DateWork.Text);
            Close();

        }
    }
}
