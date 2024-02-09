﻿using B.I.G.Controller;
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
        public JournalCollectorWindow3()
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
                Name.Text = Properties.Settings.Default.Y;
                Route.Text = Properties.Settings.Default.routeOrder;
                Date.Text = Properties.Settings.Default.dateOrder;
            }
            dGridCollector.DataContext = JournalCollectors;
            if (string.IsNullOrEmpty(Date.Text))
            {
                Date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            FillData();            
            ImgBox.DataContext = this;
            Name.TextChanged += Search;
            Route.TextChanged += Search;
            SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
            AccesText.Text = MainWindow.acces;
            NameText.Text = MainWindow.LogS;   
            Name.Text = MainWindow.NameJorunal;
            Access();
            Calendar2.Visibility = Visibility.Collapsed;
            Calendar2.IsEnabled = false;
            ImportButton.Visibility = Visibility.Collapsed;
            ImportButton.IsEnabled = false;
            X.Visibility = Visibility.Collapsed;
            X.IsEnabled = false;          

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
                BatenOrder.Visibility = Visibility.Collapsed;
                BatenOrder.IsEnabled = false;
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
            Properties.Settings.Default.Y = Name.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.routeOrder = Route.Text;
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
                foreach (var item in journalCollectorController.GetAllCashCollectors(Convert.ToDateTime(Date.Text)))
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
              
                lookCollector.Show();
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
                MainWindow.NameJorunal = Name.Text;
                var searchResults = journalCollectorController.SearchCollectorName(Name.Text, Convert.ToDateTime(Date.Text), Route.Text);

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
                    cells.Style.Font.Size = 7;
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
                for (int i = 1; i < dGridCollector.Items.Count; i++)
                {
                    var collectorItem = (journalCollector)dGridCollector.Items[i];

                    // Создание строки
                    var row = worksheet.Row(i + 3);

                    worksheet.Cells[i + 3, 2].Value = collectorItem.name;
                    worksheet.Cells[i + 3, 3].Value = collectorItem.route;
                    worksheet.Cells[i + 3, 6].Value = collectorItem.route2;
                    worksheet.Cells[i + 3, 7].Value = collectorItem.meaning;

                    worksheet.Cells[i + 3, 11].Value = collectorItem.certificate;
                    worksheet.Cells[i + 3, 12].Value = collectorItem.token;
                    worksheet.Cells[i + 3, 17].Value = collectorItem.route2;
                    worksheet.Cells[i + 3, 18].Value = collectorItem.meaning;
                    worksheet.Cells[i + 3, 22].Value = collectorItem.certificate;
                    worksheet.Cells[i + 3, 12].Value = collectorItem.token;


                    for (int col = 2; col <= 8; col++)
                    {
                        worksheet.Cells[i + 2, col].Style.Font.Size = 8; // Установите нужный размер шрифта
                    }

                  
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
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 НАРЯД НА РАБОТУ на " + Date2;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:3"];

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
            Route.Text = string.Empty;
            Date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            FillData();
        }

        private void Button_LogWindow(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow();
            logWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow();
            usersWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Button_import_to_excel(object sender, RoutedEventArgs e)
        {
            try
            {
              
                    Calendar2.Visibility = Visibility.Visible;
                    Calendar2.IsEnabled = true;
                    ImportButton.Visibility = Visibility.Visible;
                    ImportButton.IsEnabled = true;
                    X.Visibility = Visibility.Visible;
                    X.IsEnabled = true;
           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

              
                DateTime date = DateTime.Now;
                if (Calendar2.SelectedDate.HasValue)
                {
                    ; date = Calendar2.SelectedDate.Value;
                }
                if (!journalCollectorController.ImportSerchData(date))
                {
                    // создание диалогового окна для выбора файла Excel
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                    // проверка, был ли выбран файл
                    if (openFileDialog.ShowDialog() == true)
                    {

                        if (Calendar2.SelectedDate.HasValue)
                        {
                            ; date = Calendar2.SelectedDate.Value;
                        }
                        journalCollectorController.ImportExcelToDatabase(openFileDialog.FileName, date);
                        journalCollectorController.UpdateResponsibilities(date);
                        Date.Text = date.ToString("yyyy-MM-dd");
                        FillData();
                    }
                    Calendar2.Visibility = Visibility.Collapsed;
                    Calendar2.IsEnabled = false;
                    ImportButton.Visibility = Visibility.Collapsed;
                    ImportButton.IsEnabled = false;
                    X.Visibility = Visibility.Collapsed;
                    X.IsEnabled = false;
                    ImportButton.Content = "Выбрать дату";
                }
                else
                {
                    var result = MessageBox.Show("      Наряд с этой датой уже сформирован.\n                Переформировать заново?", "", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // создание диалогового окна для выбора файла Excel
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                        // проверка, был ли выбран файл
                        if (openFileDialog.ShowDialog() == true)
                        {

                            if (Calendar2.SelectedDate.HasValue)
                            {
                                ; date = Calendar2.SelectedDate.Value;
                            }
                            journalCollectorController.DeleteToDate(date);
                            journalCollectorController.ImportExcelToDatabase(openFileDialog.FileName, date);
                            journalCollectorController.UpdateResponsibilities(date);
                            Date.Text = date.ToString("yyyy-MM-dd");
                            journalCollectorController.UpdateNullValues(Convert.ToDateTime(Date.Text));
                            journalCollectorController.DeleteNUL();
                            FillData();
                        }
                        Calendar2.Visibility = Visibility.Collapsed;
                        Calendar2.IsEnabled = false;
                        ImportButton.Visibility = Visibility.Collapsed;
                        ImportButton.IsEnabled = false;
                        X.Visibility = Visibility.Collapsed;
                        X.IsEnabled = false;
                        ImportButton.Content = "Выбрать дату";     
                    }
                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Calendar2.Visibility = Visibility.Collapsed;
            Calendar2.IsEnabled = false;
            ImportButton.Visibility = Visibility.Collapsed;
            ImportButton.IsEnabled = false;
            X.Visibility = Visibility.Collapsed;
            X.IsEnabled = false;
            ImportButton.Content = "Выбрать дату";
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
{
    // Проверяем, выбрана ли хоть одна дата
    if (Calendar2.SelectedDate.HasValue)
    {
        // Устанавливаем выбранную дату в качестве содержимого кнопки ImportButton
        ImportButton.Content = Calendar2.SelectedDate.Value.ToShortDateString();
        
        // Здесь также вы можете добавить логику для автоматического выполнения каких-либо действий после выбора даты
    }
}

        private void Button_CollectorWindow(object sender, RoutedEventArgs e)
        {
            CashCollectorWindow cashCollectorWindow = new CashCollectorWindow();
            cashCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void LookCollectoButton_LogWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow journalCollectorWindow = new JournalCollectorWindow();
            journalCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }
    }
}
