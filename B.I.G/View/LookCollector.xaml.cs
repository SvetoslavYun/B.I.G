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

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для Add_User.xaml
    /// </summary>
    public partial class LookCollector : Window
    {
        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;
        private string originalName;
        public journalCollector SelectedProduct { get; set; }
        ObservableCollection<journalCollector> JournalCollectors;
        private JournalCollectorController journalCollectorController;
        public static byte[] image_bytes;
        public LookCollector(journalCollector selectedCollector)
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();
            JournalCollectors = new ObservableCollection<journalCollector>();
            InitializeComponent();
            grid.DataContext = selectedCollector;
        }


    }
}
