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
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using TextBox = System.Windows.Controls.TextBox;

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для Add_User.xaml
    /// </summary>
    public partial class LookCollector : Window
    {
        private int Id;
        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;
        private string originalName;
        public journalCollector SelectedProduct { get; set; }
        ObservableCollection<journalCollector> JournalCollectors;
        private JournalCollectorController journalCollectorController;
        public static byte[] image_bytes;
        public LookCollector(journalCollector selectedCollector, int id)
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();
            JournalCollectors = new ObservableCollection<journalCollector>();
            InitializeComponent();
            grid.DataContext = selectedCollector;
            journalCollectorController = new JournalCollectorController();
            Id= id;
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


        private void Ubdate_Button(object sender, RoutedEventArgs e)
        {
            var JournalCollector = new journalCollector()
            {
                profession = Profession2.Text,
                appropriation = Appropriation.Text,
                dateWork = DateWork.Text,
                
            };
            journalCollectorController.UpdateColumn(JournalCollector, Id);
            Close();
        }
    }
}
