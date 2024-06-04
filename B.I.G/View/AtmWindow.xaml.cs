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
    public partial class AtmWindow : System.Windows.Window
    {
        private DateTime daTe;
        public static journalCollector JournalCollector;
        ObservableCollection<journalCollector> JournalCollectors;
        private JournalCollectorController journalCollectorController;

        public static atm Atm;
        ObservableCollection<atm> Atms;
        private Atm_Controller atm_Controller;

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
        public AtmWindow(DateTime date)
        {
            Atms = new ObservableCollection<atm>();
            atm_Controller = new Atm_Controller();

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
            dGridCollector.DataContext = Atms;
            Date.Text = date.ToString("dd.MM.yyyy") + " " + date.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"));
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
                Atms.Clear();
                foreach (var item in atm_Controller.GetAllAtm(Convert.ToDateTime(Date.Text)))
                {
                    Atms.Add(item);
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
                    List<atm> atms = dGridCollector.SelectedItems.Cast<atm>().ToList();
                    {
                        // проход по списку выбранных строк
                        foreach (atm Atms in atms)
                        {
                            var Id = Atms.id;
                            atm_Controller.Delete( Id);
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
                var searchResults = atm_Controller.SearchAtmName(Name.Text, Route.Text, Convert.ToDateTime(Date.Text));

                Atms.Clear();
                foreach (var result in searchResults)
                {
                    Atms.Add(result);
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
                    process = "Сформировал: Журнал устройств самообслуживания",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);

                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Журнал устройств самообслуживания");

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

                worksheet.Row(1).Height = 25;
                worksheet.Row(2).Height = 40;

                worksheet.Cells[1, 1, 2, 1].Merge = true; // Объединение колонки 1 в строках 1 и 2
                worksheet.Cells[1, 1].Value = "№ п/п";

                worksheet.Cells[1, 4, 2, 4].Merge = true; 
                worksheet.Cells[1, 4].Value = "Номер УС (рабочего комплекта ключей УС, конверта с кодом сейфа УС)";

                worksheet.Cells[1, 11, 2, 11].Merge = true; 
                worksheet.Cells[1, 11].Value = "Расписка лица, принявшего рабочий комплект ключей УС, конверт с кодом сейфа УС";

                worksheet.Cells[1, 12, 2, 12].Merge = true; 
                worksheet.Cells[1, 12].Value = "Примечание";
                worksheet.Cells[1, 12].Style.TextRotation = 90;

                // Объединение второй и третьей колонок в первой строке
                worksheet.Cells[1, 2, 1, 3].Merge = true;

                // Установка значения в объединенной ячейке
                worksheet.Cells[1, 2].Value = "Рабочий комплект ключей УС, конверт с кодом сейфа УС выдан";
                // Добавление заголовков столбцов и порядковых номеров

                // Объединение второй и третьей колонок в первой строке
                worksheet.Cells[1, 5, 1, 6].Merge = true;

                // Установка значения в объединенной ячейке
                worksheet.Cells[1, 5].Value = "Рабочий комплект ключей УС, конверт с кодом сейфа УС получил";
                // Добавление заголовков столбцов и порядковых номеров

                // Объединение второй и третьей колонок в первой строке
                worksheet.Cells[1, 7, 1, 8].Merge = true;

                // Установка значения в объединенной ячейке
                worksheet.Cells[1, 7].Value = "Ключ от сейфа из рабочего комплекта УС получил";
                // Добавление заголовков столбцов и порядковых номеров

                // Объединение второй и третьей колонок в первой строке
                worksheet.Cells[1, 9, 1, 10].Merge = true;

                // Установка значения в объединенной ячейке
                worksheet.Cells[1, 9].Value = "Рабочий комплект ключей УС, конверт с кодом сейфа УС принят";
                // Добавление заголовков столбцов и порядковых номеров

                for (int i = 1; i <= dGridCollector.Columns.Count; i++)
                {
                    worksheet.Cells[2, i].Value = dGridCollector.Columns[i - 1].Header;
                    worksheet.Cells[3, i].Value = i;
                }



                // Добавление данных
                for (int i = 0; i < dGridCollector.Items.Count; i++)
                {
                    var collectorItem = (atm)dGridCollector.Items[i];

                    // Создание строки
                    var row = worksheet.Row(i + 4);
                    row.Height = 19;
                    worksheet.Cells[i + 4, 1].Value = collectorItem.route;
                    worksheet.Cells[i + 4, 2].Value = collectorItem.date.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 4, 9].Value = collectorItem.date.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 4, 4].Value = collectorItem.atmname;
                    worksheet.Cells[i + 4, 5].Value = collectorItem.name;

                    worksheet.Cells[i + 4, 7].Value = collectorItem.name2;
                  


                    for (int col = 2; col <= 12; col++)
                    {
                        worksheet.Cells[i + 4, col].Style.Font.Size = 8; // Установите нужный размер шрифта
                    }

                   
                }

                // Добавление 20 пустых строк
                int rowCount = dGridCollector.Items.Count + 4;
                for (int i = 0; i < 40; i++)
                {
                    var row = worksheet.Row(rowCount + i);
                    row.Height = 19;

                    // Устанавливаем стили для линий ячеек
                    using (var cells = worksheet.Cells[rowCount + i, 1, rowCount + i, dGridCollector.Columns.Count])
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
                }



                worksheet.Column(1).Width = 6;
                worksheet.Column(2).Width = 10;
                worksheet.Column(3).Width = 9;
                worksheet.Column(4).Width = 23;
                worksheet.Column(5).Width = 12;
                worksheet.Column(6).Width = 8;
                worksheet.Column(7).Width = 12;
                worksheet.Column(8).Width = 8;
                worksheet.Column(9).Width = 10;
                worksheet.Column(10).Width = 9;
                worksheet.Column(11).Width = 10;
                worksheet.Column(12).Width = 5;


                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 " + formattedDate2;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:3"];

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = "Журнал устройств самообслуживания"
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
            LogWindow logWindow = new LogWindow(daTe);
            logWindow.Show();
            Close();
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow(daTe);
            usersWindow.Show();
            Close();
        }

      

        private void Button_CollectorWindow(object sender, RoutedEventArgs e)
        {
            CashCollectorWindow cashCollectorWindow = new CashCollectorWindow(daTe);
            cashCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }


        private void Button_OrderrWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow2 journalCollectorWindow2 = new JournalCollectorWindow2();
            journalCollectorWindow2.Show();
            // Получить экземпляр текущего окна
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();


        }

        private void Inventory_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow3 journalCollectorWindow = new JournalCollectorWindow3(daTe);
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

        private void LookCollectoButton_LogWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow journalCollectorWindow = new JournalCollectorWindow(Convert.ToDateTime(Date.Text));
            journalCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
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
                    DateTime selectedDate = Convert.ToDateTime(Date.Text);
                    atm_Controller.DeleteToDate(selectedDate);
                    atm_Controller.ImportExcelToDatabase(openFileDialog.FileName, selectedDate);
                    atm_Controller.UpdateNull();
                    int empty = atm_Controller.EmptyRouteCount(selectedDate); // Вызов метода и присвоение результата переменной empty

                    if (empty == 0)
                    {
                        atm_Controller.UpdateJournalBase2(selectedDate);
                    }
                    else
                    {
                        MessageBox.Show("Данные не могут быть опубликованы на сервере, так как имеют несоответствие", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                 
                    Search(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Button_DelDate(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены?", "Удалить наряд на " + Date.Text, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                atm_Controller.DeleteToDate(Convert.ToDateTime(Date.Text));
                Search(sender, e);
            }
        }
    }
}
