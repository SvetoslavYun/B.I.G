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
    public partial class JournalCollectorWindow : System.Windows.Window
    {
        private string Are;
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
        public JournalCollectorWindow(DateTime date, string area)
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
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Y) || !string.IsNullOrEmpty(Properties.Settings.Default.routeOrder) || !string.IsNullOrEmpty(Properties.Settings.Default.dateOrder))
            {
                Name.Text = Properties.Settings.Default.Y;
                Route.Text = Properties.Settings.Default.routeOrder;
                Date.Text = Properties.Settings.Default.dateOrder;
            }
            dGridCollector.DataContext = JournalCollectors;
            Date.Text = date.ToString("dd.MM.yyyy") + " " + date.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"));
            Are= area;
            Area.Text = area;
            daTe = date;
            FillData();
            ImgBox.DataContext = this;
            Name.TextChanged += Search;
            Route.TextChanged += Search;
            SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
            AccesText.Text = MainWindow.acces;
            NameText.Text = MainWindow.LogS;
            Name.Text = MainWindow.NameJorunal;
            if (AccesText.Text != "Администратор")
            {

                UserButton.Visibility = Visibility.Collapsed;
                UserButton.IsEnabled = false;
                logButton.Visibility = Visibility.Collapsed;
                logButton.IsEnabled = false;
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

                if (rowContext.data == "Данные отсутствуют")
                {
                    backgroundBrush = new SolidColorBrush(Colors.Orange);
                }
                else if (rowContext.data.Contains("Повтор автомата"))
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
                foreach (var item in journalCollectorController.GetAllCashCollectors0(Convert.ToDateTime(Date.Text), Area.Text))
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
                var id = ((journalCollector)dGridCollector.SelectedItem).id;
                var selectedCollector = (journalCollector)dGridCollector.SelectedItem;
                JournalCollector = selectedCollector;

                LookCollector lookCollector = new LookCollector(selectedCollector, id);

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
                MainWindow.NameJorunal = Name.Text;
                var searchResults = journalCollectorController.SearchCollectorName0(Name.Text, Convert.ToDateTime(Date.Text), Route.Text, Are);

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
                    process = "Сформировал: Журнал оружия и боеприпасов",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);

                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Журнал оружия и боеприпасов");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridCollector.Items.Count + 1, dGridCollector.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине

                    cells.Style.Font.Size = 10; // Установите нужный размер шрифта

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
                    worksheet.Cells[i + 2, 2].Value = collectorItem.route;
                    worksheet.Cells[i + 2, 3].Value = collectorItem.profession;
                    worksheet.Cells[i + 2, 5].Value = collectorItem.fullname;
                    worksheet.Cells[i + 2, 6].Value = collectorItem.gun;
                    worksheet.Cells[i + 2, 7].Value = collectorItem.dateWork;
                    worksheet.Cells[i + 2, 8].Value = collectorItem.data;
                    worksheet.Cells[i + 2, 9].Value = collectorItem.automaton_serial;                  
                    worksheet.Cells[i + 2, 10].Value = collectorItem.automaton;
                    worksheet.Cells[i + 2, 11].Value = collectorItem.permission;                   

                    for (int col = 2; col <= 11; col++)
                    {
                        worksheet.Cells[i + 2, col].Style.Font.Size = 8; // Установите нужный размер шрифта
                    }

                    // Добавьте условие для проверки значения collectorItem.fullname
                    if (collectorItem.fullname == ".")
                    {
                        worksheet.Cells[i + 2, 2].Value = worksheet.Cells[i + 2, 7].Value;
                        worksheet.Cells[i + 2, 2, i + 2, 11].Merge = true;
                        // Установите стиль заливки для первых семь колонок
                        for (int col = 2; col <= 10; col++)
                        {
                            worksheet.Cells[i + 2, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 2, col].Style.Fill.BackgroundColor.SetColor(Color.Wheat);
                            worksheet.Cells[i + 2, col].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells[i + 2, col].Style.Font.Bold = true; // Установите шрифт жирным
                            worksheet.Cells[i + 2, col].Style.Font.Italic = true; // Установите шрифт курсивом
                        }
                    }
                }


                worksheet.DeleteColumn(1);
                worksheet.DeleteColumn(3);
               
                // Автоподгон ширины колонок
                worksheet.Cells.AutoFitColumns();
                worksheet.Column(1).Width = 9;
                worksheet.Column(2).Width = 21;
                worksheet.Column(3).Width = 25;
                worksheet.Column(5).Width = 0;
                worksheet.Column(7).Width = 9;
                worksheet.Column(6).Width = 16;
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 Журнал оружия и боеприпасов " + formattedDate2;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:1"];

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = "Журнал оружия и боеприпасов'"
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
            LogWindow logWindow = new LogWindow(daTe, Are);
            logWindow.Show();
            Close();
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow(daTe, Are);
            usersWindow.Show();
            Close();
        }

      

        private void Button_CollectorWindow(object sender, RoutedEventArgs e)
        {
            CashCollectorWindow cashCollectorWindow = new CashCollectorWindow(daTe, Are);
            cashCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }


        private void Button_OrderrWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow2 journalCollectorWindow2 = new JournalCollectorWindow2(Are);
            journalCollectorWindow2.Show();
            // Получить экземпляр текущего окна
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();


        }

        private void Inventory_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow3 journalCollectorWindow = new JournalCollectorWindow3(daTe, Are);
            journalCollectorWindow.Show();
            Close();
        }

        private void Briefing_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow4 journalCollectorWindow = new JournalCollectorWindow4(Convert.ToDateTime(Date.Text), Are);
            journalCollectorWindow.Show();
            Close();

        }

        private void Appearances_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow5 journalCollectorWindow = new JournalCollectorWindow5(daTe, Are);
            journalCollectorWindow.Show();
            Close();

        }

        private void Button_AtmWindow(object sender, RoutedEventArgs e)
        {
            AtmWindow atmWindow = new AtmWindow(Convert.ToDateTime(Date.Text), Are);
            atmWindow.Show();
            Close();
        }
    }
}
