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
using Microsoft.Graph.Models;
using System.Windows.Input;
using B.I.G.View;

namespace B.I.G

{
    public partial class JournalCollectorWindow3 : System.Windows.Window
    {
        private DateTime daTe;
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
        public JournalCollectorWindow3(DateTime date)
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
            // Загрузка сохраненного значения переменной
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Y)|| !string.IsNullOrEmpty(Properties.Settings.Default.routeOrder) || !string.IsNullOrEmpty(Properties.Settings.Default.dateOrder))
            {
              
                Date.Text = Properties.Settings.Default.dateOrder;
            }
            dGridCollector.DataContext = JournalCollectors;
            Date.Text = date.ToString("dd.MM.yyyy") + " " + date.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"));
            daTe = date;
            FillData();            
            ImgBox.DataContext = this;
           
            SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
            AccesText.Text = MainWindow.acces;
            NameText.Text = MainWindow.LogS;
            Access();
        }

        public void Access()
        {
            if (AccesText.Text != "Администратор")
            {

                UserButton.Visibility = Visibility.Collapsed;
                UserButton.IsEnabled = false;
                logButton.Visibility = Visibility.Collapsed;
                logButton.IsEnabled = false;
            }
            if (AccesText.Text == "Пользователь")
            {
               
                CollectoButton.Visibility = Visibility.Collapsed;
                CollectoButton.IsEnabled = false;
                LookCollectoButton.Visibility = Visibility.Collapsed;
                LookCollectoButton.IsEnabled = false;
                CollectoButton.Visibility = Visibility.Collapsed;
                CollectoButton.IsEnabled = false;
                LookCollectoButton.Visibility = Visibility.Collapsed;
                LookCollectoButton.IsEnabled = false;

            }
        }

        // Сохранение значения переменной при закрытии окна
        private void Window_Closing(object sender, EventArgs e)
        {
           
            Properties.Settings.Default.Save();
           
            Properties.Settings.Default.Save();
            Properties.Settings.Default.dateOrder = Date.Text;
            Properties.Settings.Default.Save();
        }


        private void dGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            journalCollector rowContext = e.Row.DataContext as journalCollector;
            if (rowContext != null)
            {
                SolidColorBrush backgroundBrush = new SolidColorBrush(Colors.White);

                if (rowContext.dateWork == "Данные отсутствуют")
                {
                    backgroundBrush = new SolidColorBrush(Colors.Orange);
                }
                else if (rowContext.dateWork.Contains("Повтор автомата")) 
                {
                    backgroundBrush = new SolidColorBrush(Colors.RosyBrown);
                }

                e.Row.Background = backgroundBrush;
            }

            e.Row.Header = e.Row.GetIndex() + 1;
        }


        public void FillData()
        {
            try

            {
                JournalCollectors.Clear();
                foreach (var item in journalCollectorController.GetAllCashCollectors3(Convert.ToDateTime(Date.Text)))
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
            //flag = true;
            //Add_СashCollector add_СashCollector = new Add_СashCollector();
            //add_СashCollector.Owner = this;
            //add_СashCollector.ShowDialog();
            //Search(sender, e);
        }

        private void DoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dGridCollector.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");

                var selectedCollector = (journalCollector)dGridCollector.SelectedItem;
                JournalCollector = selectedCollector;

                LookCollector lookCollector = new LookCollector(selectedCollector);
              
                lookCollector.ShowDialog();
                journalCollectorController.UpdateNullValues(Convert.ToDateTime(Date.Text));
                journalCollectorController.DeleteNUL();
                Search(sender, e);
                JournalCollector = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void EditMenuItem(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dGridCollector.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var Id = ((journalCollector)dGridCollector.SelectedItem).id;
                var Route2 = ((journalCollector)dGridCollector.SelectedItem).route2;
                var Profession = ((journalCollector)dGridCollector.SelectedItem).profession;
                string Permission = ((journalCollector)dGridCollector.SelectedItem).permission;
                if (Permission != ".")
                {
                    EditJournal editJournal = new EditJournal(Id, Route2, Convert.ToDateTime(Date.Text), Profession);
                    editJournal.Owner = this;
                    editJournal.ShowDialog();
                }
                journalCollectorController.UpdateNullValues(Convert.ToDateTime(Date.Text));
                journalCollectorController.DeleteNUL();
                Search(sender, e);
                JournalCollector = null;
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
        }

        private void EditAutomate(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dGridCollector.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var Id = ((journalCollector)dGridCollector.SelectedItem).id2;
                var Name = ((journalCollector)dGridCollector.SelectedItem).name;
                var Route2 = ((journalCollector)dGridCollector.SelectedItem).route2;
                var Profession = ((journalCollector)dGridCollector.SelectedItem).profession;
                string Permission = ((journalCollector)dGridCollector.SelectedItem).permission;
                if (Id != 0)
                {
                    journalCollectorController.EditAutomate(Id, Name, Convert.ToDateTime(Date.Text), Route2);
                    journalCollectorController.UpdateResponsibilities2(Convert.ToDateTime(Date.Text));
                    journalCollectorController.UpdateNullValues(Convert.ToDateTime(Date.Text));
                    journalCollectorController.DeleteNUL();
                    Search(sender, e);
                    JournalCollector = null;
                }
                else { MessageBox.Show("Данные по сотруднику отсутствуют"); }
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
                            journalCollectorController.Delete(Route, Id, Convert.ToDateTime(Date.Text));
                            journalCollectorController.UpdateResponsibilities2(Convert.ToDateTime(Date.Text));
                            journalCollectorController.UpdateNullValues(Convert.ToDateTime(Date.Text));
                            journalCollectorController.DeleteNUL();
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


        private void Date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //Обработки изменения даты
            Search(sender, e);
        }


        private void Date_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true; // Отменить обработку события, чтобы предотвратить ввод текста
        }


        private void Search(object sender, RoutedEventArgs e)
        {
            try

            {
                SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
                AccesText.Text = MainWindow.acces;
                NameText.Text = MainWindow.LogS;
           
                var searchResults = journalCollectorController.SearchCollectorName3(Convert.ToDateTime(Date.Text));

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
            DateTime Date2 = Convert.ToDateTime(Date.Text);
            try
            {
                DateTime Date = DateTime.Now;
                string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                string formattedDate2 = Date2.ToString("dd.MM.yyyy") + " " + Date2.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"));
                var Log2 = new log()
                {
                    username = MainWindow.LogS,
                    process = "Сформировал: Журнал выдачи инвентаря",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);

                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Журнал выдачи инвентаря");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridCollector.Items.Count + 3, dGridCollector.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине
                    cells.Style.WrapText = true; // Разрешаем перенос текста
                    cells.Style.Font.Size = 7;
                    cells.Style.Font.Bold = true;
                }

                // Добавление сетки после последней строки данных
                using (var cells = worksheet.Cells[dGridCollector.Items.Count + 2, 1, dGridCollector.Items.Count + 3, dGridCollector.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине
                    cells.Style.WrapText = true; // Разрешаем перенос текста
                }

                // Объединение второй и третьей колонок в первой строке
                worksheet.Cells[1, 4, 1, 14].Merge = true;

                // Установка значения в объединенной ячейке
                worksheet.Cells[1, 4].Value = "Выдано инкассаторам";
                // Добавление заголовков столбцов и порядковых номеров

                // Объединение второй и третьей колонок в первой строке
                worksheet.Cells[1, 16, 1, 26].Merge = true;

                // Установка значения в объединенной ячейке
                worksheet.Cells[1, 16].Value = "Получено от инкассаторов";
                // Добавление заголовков столбцов и порядковых номеров

                for (int i = 1; i <= dGridCollector.Columns.Count; i++)
                {
                    worksheet.Cells[2, i].Value = dGridCollector.Columns[i - 1].Header;
                    worksheet.Cells[3, i].Value = i;
                }

              

                // Добавление данных
                for (int i = 0; i < dGridCollector.Items.Count; i++)
                {
                    var collectorItem = (journalCollector)dGridCollector.Items[i];

                    // Создание строки
                    var row = worksheet.Row(i + 4);
                    row.Height = 19;
                    worksheet.Cells[i + 4, 2].Value = collectorItem.name;
                    worksheet.Cells[i + 4, 3].Value = collectorItem.route;
                    worksheet.Cells[i + 4, 6].Value = collectorItem.route2;
                    worksheet.Cells[i + 4, 7].Value = collectorItem.meaning;

                    worksheet.Cells[i + 4, 11].Value = collectorItem.certificate;
                    worksheet.Cells[i + 4, 12].Value = collectorItem.token;
                    worksheet.Cells[i + 4, 17].Value = collectorItem.route2;
                    worksheet.Cells[i + 4, 18].Value = collectorItem.meaning;
                    worksheet.Cells[i + 4, 22].Value = collectorItem.certificate;
                    worksheet.Cells[i + 4, 23].Value = collectorItem.token;


                    for (int col = 2; col <= 8; col++)
                    {
                        worksheet.Cells[i + 4, col].Style.Font.Size = 8; // Установите нужный размер шрифта
                    }

                    if (worksheet.Cells[i + 4, 2].Value?.ToString() != "")
                    {

                        worksheet.Cells[i + 4, 9].Value = "Алмаз";
                        worksheet.Cells[i + 4, 20].Value = "Алмаз";                      
                    }


                    if (worksheet.Cells[i + 4, 7].Value?.ToString() != "")
                    {

                      
                        worksheet.Cells[i + 4, 10].Value = "T2 DP2400";
                        worksheet.Cells[i + 4, 21].Value = "T2 DP2400";
                        worksheet.Cells[i + 4, 13].Value = "Honeywell";
                        worksheet.Cells[i + 4, 24].Value = "Honeywell";
                    }
                    else
                    {
                        worksheet.Cells[i + 4, 6].Value = "";
                        worksheet.Cells[i + 4, 17].Value = "";
                    }

                   
                        worksheet.Cells[i + 4, 8].Value = "-/-";
                        worksheet.Cells[i + 4, 14].Value = "-/-";
                        worksheet.Cells[i + 4, 19].Value = "-/-";
                        worksheet.Cells[i + 4, 25].Value = "-/-";
                    
                }   

                // Автоподгон ширины колонок
                worksheet.Column(1).Width = 8;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 8;
                worksheet.Column(4).Width = 8;
                worksheet.Column(5).Width = 10;
                worksheet.Column(6).Width = 8;
                worksheet.Column(7).Width = 8;
                worksheet.Column(8).Width = 9;
                worksheet.Column(9).Width = 7;
                worksheet.Column(10).Width = 8;
                worksheet.Column(11).Width = 7;
                worksheet.Column(12).Width = 8;
                worksheet.Column(13).Width = 8;
                worksheet.Column(14).Width = 8;
                worksheet.Column(15).Width = 11;
                worksheet.Column(16).Width = 8;
                worksheet.Column(17).Width = 8;
                worksheet.Column(18).Width = 8;
                worksheet.Column(19).Width = 9;
                worksheet.Column(20).Width = 7;
                worksheet.Column(21).Width = 7;
                worksheet.Column(22).Width = 8;
                worksheet.Column(23).Width = 8;
                worksheet.Column(24).Width = 8;
                worksheet.Column(25).Width = 8;
                worksheet.Column(26).Width = 8;
                worksheet.Column(27).Width = 11;
                worksheet.Column(28).Width = 8;

                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 " + formattedDate2;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:3"];

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = "Журнал выдачи инвентаря"
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
           
            Date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            FillData();
        }

        private void Button_LogWindow(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow(daTe);
            logWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow(daTe);
            usersWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

 

    


        private void Button_CollectorWindow(object sender, RoutedEventArgs e)
        {
            CashCollectorWindow cashCollectorWindow = new CashCollectorWindow(daTe);
            cashCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void LookCollectoButton_LogWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow journalCollectorWindow = new JournalCollectorWindow(daTe);
            journalCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Button_OrderrWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow2 journalCollectorWindow = new JournalCollectorWindow2();
            journalCollectorWindow.Show();
            Close();
        }

        private void Briefing_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow4 journalCollectorWindow = new JournalCollectorWindow4(Convert.ToDateTime(Date.Text));
            journalCollectorWindow.Show();
            Close();
        }

        private void Appearances_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow5 journalCollectorWindow = new JournalCollectorWindow5(daTe);
            journalCollectorWindow.Show();
            Close();
        }
    }
}
