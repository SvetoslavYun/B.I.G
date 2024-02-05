using B.I.G.Controller;
using B.I.G.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.Data;
using System.Drawing;
using System.Windows.Media;
using Color = System.Drawing.Color;
using DocumentFormat.OpenXml.Drawing;

namespace B.I.G

{
    public partial class JournalCollectorWindow2 : System.Windows.Window
    {
        public static journalCollector JournalCollector;
        ObservableCollection<journalCollector> JournalCollectors;
        private JournalCollectorController journalCollectorController;

        public static cashCollector CashCollector;
        ObservableCollection<cashCollector> CashCollectors;
        private СashCollectorController сashCollectorController;
        public journalCollector SelectedProduct { get; set; }


        ObservableCollection<user_account> User_Accounts;
        private User_accountController user_AccountController;

        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;
        public static bool flag;
        public static bool flagEdit;         
        public JournalCollectorWindow2()
        {
            JournalCollectors = new ObservableCollection<journalCollector>();
            journalCollectorController = new JournalCollectorController();

            CashCollectors = new ObservableCollection<cashCollector>();
            сashCollectorController = new СashCollectorController();

            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();

            User_Accounts = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            
            InitializeComponent();
            dGridCollector.DataContext = JournalCollectors;
            FillData();
            ImgBox.DataContext = this;
            Name.TextChanged += Search;
            SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
            AccesText.Text = MainWindow.acces;
            NameText.Text = MainWindow.LogS;
            Name.Text = MainWindow.nameUser;
            if (AccesText.Text != "Администратор")
            {
                UserButton.Visibility = Visibility.Collapsed;
                UserButton.IsEnabled = false;
                logButton.Visibility = Visibility.Collapsed;
                logButton.IsEnabled = false;
            }
        }


        private void dGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            journalCollector rowContext = e.Row.DataContext as journalCollector;
            if (rowContext != null)
            {
                if (rowContext.dateWork == "Данные отсутствуют")
                    e.Row.Background = new SolidColorBrush(Colors.Orange);
                else { e.Row.Background = new SolidColorBrush(Colors.White); }
            }
            e.Row.Header = e.Row.GetIndex() + 1;
        }      

        public void FillData()
        {
            try

            {
                JournalCollectors.Clear();
                foreach (var item in journalCollectorController.GetAllCashCollectors())
                {
                    JournalCollectors.Add(item);
                }

            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
        }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            flag = true;
            Add_СashCollector add_СashCollector = new Add_СashCollector();
            add_СashCollector.Owner = this;
            add_СashCollector.ShowDialog();
            Search(sender, e);        
        }

        private void DoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {                
                if (dGridCollector.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var id = ((journalCollector)dGridCollector.SelectedItem).id2;
                UsersWindow.flag = false;
                CashCollector = (cashCollector)dGridCollector.SelectedItem;
                Add_СashCollector add_СashCollector = new Add_СashCollector();
                add_СashCollector.Owner = this;
                add_СashCollector.ShowDialog();
                Search(sender, e);                            
                CashCollector = null;
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
        }

        private void EditMenuItem(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dGridCollector.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var id = ((cashCollector)dGridCollector.SelectedItem).id;
                flag = false;
                CashCollector = (cashCollector)dGridCollector.SelectedItem;
                Add_СashCollector add_СashCollector = new Add_СashCollector();
                add_СashCollector.Owner = this;
                add_СashCollector.ShowDialog();
                    Search(sender, e);               
                CashCollector = null;
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
        }

        private void DeleteMenuItem(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dGridCollector.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var result = MessageBox.Show("Вы уверены?", "Удалить запись", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                { // получение выбранных строк
                    List<journalCollector> journalCollectors = dGridCollector.SelectedItems.Cast<journalCollector>().ToList();
                    {
                        // проход по списку выбранных строк
                        foreach (journalCollector JournalCollectors in journalCollectors)
                        {
                            var Route = JournalCollectors.route;
                            var Id = JournalCollectors.id;
                            string name = JournalCollectors.fullname;
                            journalCollectorController.Delete(Route,Id);

                            DateTime Date = DateTime.Now;
                            string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                            string formattedDate2 = Date.ToString("dd.MM.yyyy");
                            var Log = new log()
                            {
                                username = MainWindow.LogS,
                                process = "Удалил сотрудника: " + name + "",
                                date = Convert.ToDateTime(formattedDate),
                                date2 = Convert.ToDateTime(formattedDate2)
                            };
                            log_Controller.Insert(Log);
                            Search(sender, e);
                        }
                    }
                }
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }

        }

        private void Search(object sender, RoutedEventArgs e)
        {
            try

            {
                SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
                AccesText.Text = MainWindow.acces;
                NameText.Text = MainWindow.LogS;
                MainWindow.nameUser = Name.Text;
                var searchResults = journalCollectorController.SearchCollectorName(Name.Text);

                JournalCollectors.Clear();
                    foreach (var result in searchResults)
                    {
                    JournalCollectors.Add(result);
                    }
                    
                
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
        }


        private void Button_export_to_excel(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime Date = DateTime.Now;
                string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                string formattedDate2 = Date.ToString("dd.MM.yyyy");
                var Log2 = new log()
                {
                    username = MainWindow.LogS,
                    process = "Сформировал: Наряд на работу",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);
               
                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("CashCollectors");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridCollector.Items.Count + 1, dGridCollector.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине
                    cells.Style.WrapText = true; // Разрешаем перенос текста
                    cells.Style.Font.Size = 8; // Установите нужный размер шрифта

                }

                // Добавление заголовков столбцов и порядковых номеров

                for (int i = 1; i <= dGridCollector.Columns.Count; i++)
                {
                    worksheet.Cells[1, i].Value = dGridCollector.Columns[i - 1].Header;
                }


                // Добавление данных
                for (int i = 0; i < dGridCollector.Items.Count; i++)
                {
                    var collectorItem = (journalCollector)dGridCollector.Items[i];

                    // Создание строки
                    var row = worksheet.Row(i + 2);

                    worksheet.Cells[i + 2, 2].Value = collectorItem.profession;
                    worksheet.Cells[i + 2, 3].Value = collectorItem.name;
                    worksheet.Cells[i + 2, 4].Value = collectorItem.dateWork;
                    worksheet.Cells[i + 2, 7].Value = collectorItem.appropriation;
                   

                    for (int col = 2; col <= 7; col++)
                    {
                        worksheet.Cells[i + 2, col].Style.Font.Size = 6; // Установите нужный размер шрифта
                    }

                    // Добавьте условие для проверки значения collectorItem.fullname
                    if (collectorItem.fullname == ".")
                    {
                        // Установите стиль заливки для первых семь колонок
                        for (int col = 2; col <= 7; col++)
                        {
                            worksheet.Cells[i + 2, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 2, col].Style.Fill.BackgroundColor.SetColor(Color.Black);
                            worksheet.Cells[i + 2, col].Style.Font.Color.SetColor(Color.White);
                        }
                    }
                }


                worksheet.DeleteColumn(1);
              
                // Автоподгон ширины колонок
                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 13;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 10;
                worksheet.Column(5).Width = 10;
                worksheet.Column(6).Width = 11;

                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 Наряд на работу";
              
                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:1"];

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = "Наряд на работу"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    SaveExcelWithPageLayoutView(excelPackage, saveFileDialog.FileName);
                }             
               
                Search(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте в Excel: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





      

        private void SaveExcelWithPageLayoutView(ExcelPackage excelPackage, string filePath)
        {
            try
            {
                // Сохранение Excel-пакета в файл
                File.WriteAllBytes(filePath, excelPackage.GetAsByteArray());

                // Открытие Excel-приложения
                var excelApp = new Excel.Application();
                excelApp.Visible = true;

                // Открытие сохраненного файла
                var workbook = excelApp.Workbooks.Open(filePath);

                // Установка вида "Разметка страницы"
                excelApp.ActiveWindow.View = Excel.XlWindowView.xlPageLayoutView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении и открытии Excel: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_cleaning(object sender, RoutedEventArgs e)
        {
            Name.Text = string.Empty;
           
            FillData();
        }

        private void Button_LogWindow(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow();
            logWindow.Show();
            Close();
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow();
            usersWindow.Show();
            Close();
        }

        private void Button_import_to_excel(object sender, RoutedEventArgs e)
        {
            try
            {
                // создание диалогового окна для выбора файла Excel
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                // проверка, был ли выбран файл
                if (openFileDialog.ShowDialog() == true)
                {

                    journalCollectorController.ImportExcelToDatabase(openFileDialog.FileName);
                    journalCollectorController.UpdateResponsibilities();
                    FillData();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

      
    }
}
