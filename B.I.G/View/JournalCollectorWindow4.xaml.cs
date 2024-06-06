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
    public partial class JournalCollectorWindow4 : System.Windows.Window
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
        public JournalCollectorWindow4(DateTime date)
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
                foreach (var item in journalCollectorController.GetAllCashCollectors4(Convert.ToDateTime(Date.Text)))
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
           
                var searchResults = journalCollectorController.SearchCollectorName4(Convert.ToDateTime(Date.Text));

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
                    process = "Сформировал: Журнал инструктажа",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);

                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Журнал инструктажа");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridCollector.Items.Count + 2, dGridCollector.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине
                    cells.Style.WrapText = true; // Разрешаем перенос текста
                    cells.Style.Font.Size = 8;
                }

                // Добавление сетки после последней строки данных
                using (var cells = worksheet.Cells[dGridCollector.Items.Count + 2, 1, dGridCollector.Items.Count + 2, dGridCollector.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине
                    cells.Style.WrapText = true; // Разрешаем перенос текста
                }

                for (int i = 1; i <= dGridCollector.Columns.Count; i++)
                {
                    worksheet.Cells[1, i].Value = dGridCollector.Columns[i - 1].Header;
                    worksheet.Cells[2, i].Value = i;
                }

             
                // Добавление данных
                for (int i = 0; i < dGridCollector.Items.Count; i++)
                {
                    var collectorItem = (journalCollector)dGridCollector.Items[i];

                    // Создание строки
                    var row = worksheet.Row(i + 3);
                    row.Height = 19;
                    worksheet.Cells[i + 3, 1].Value = collectorItem.date.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 3, 2].Value = collectorItem.dateWork;
                    worksheet.Cells[i + 3, 4].Value = collectorItem.name;
                    worksheet.Cells[i + 3, 5].Value = collectorItem.profession;
                    // Условие для проверки времени
                    TimeSpan time;
                    if (TimeSpan.TryParse(collectorItem.dateWork, out time))
                    {
                        if (time < new TimeSpan(8, 30, 0))
                        {
                            worksheet.Cells[i + 3, 7].Value = Name.Text;
                        }
                        else
                        {
                            worksheet.Cells[i + 3, 7].Value = Name2.Text;
                        }
                    }
                    else
                    {
                        worksheet.Cells[i + 3, 7].Value = "Некорректное время";
                    }
                

            }


                // Добавление 10 пустых строк
                int rowCount = dGridCollector.Items.Count + 3;
                for (int i = 0; i < 33; i++)
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


                worksheet.Cells[3, 3].Value = "Тема №1 - Меры безопасности";
                worksheet.Cells[4, 3].Value = "при обращении с оружием и";
                worksheet.Cells[5, 3].Value = "боеприпасам к нему.";
                worksheet.Cells[6, 3].Value = "Порядок применения и ";
                worksheet.Cells[7, 3].Value = "использования оружия.";

                worksheet.Cells[9, 3].Value = "Тема №2 - Порядок получения и";
                worksheet.Cells[10, 3].Value = "использование средств";
                worksheet.Cells[11, 3].Value = "индивидуальной защиты";
                worksheet.Cells[12, 3].Value = "и средств связи.";

                worksheet.Cells[14, 3].Value = "Тема №3 - Соблюдение условий";
                worksheet.Cells[15, 3].Value = "обслуживание клиентов";
                worksheet.Cells[16, 3].Value = "в соответствии";
                worksheet.Cells[17, 3].Value = "с договорными отношениями.";

                worksheet.Cells[19, 3].Value = "Тема №4 - Строгое соблюдение";
                worksheet.Cells[20, 3].Value = "порядка и правил";
                worksheet.Cells[21, 3].Value = "инкассации и перевозки";
                worksheet.Cells[22, 3].Value = "ценностей.";

                worksheet.Cells[24, 3].Value = "Тема №5 - Строгое выполнение";
                worksheet.Cells[25, 3].Value = "требований, предъявляемых к";
                worksheet.Cells[26, 3].Value = "обеспечению безопасности";
                worksheet.Cells[27, 3].Value = "членов бригады инкассаторов";
                worksheet.Cells[28, 3].Value = "при работе на маршрутах";
                worksheet.Cells[29, 3].Value = "и к обеспечению";
                worksheet.Cells[30, 3].Value = "сохранности ценностей.";

                worksheet.Cells[32, 3].Value = "Тема №6 - Соблюдение правил";
                worksheet.Cells[33, 3].Value = "дорожного движения.";
                worksheet.Cells[34, 3].Value = "Особое внимание";
                worksheet.Cells[35, 3].Value = "к соблюдению требований,";
                worksheet.Cells[36, 3].Value = "предъявляемых к скоростному";
                worksheet.Cells[37, 3].Value = "режиму движения и";
                worksheet.Cells[38, 3].Value = "безопасной дистанции";
                worksheet.Cells[39, 3].Value = "до впереди";
                worksheet.Cells[40, 3].Value = "движущего автомобиля.";

                worksheet.Cells[42, 3].Value = "Тема №7 - Особенность работы";
                worksheet.Cells[43, 3].Value = "службы инкассации с учетом";
                worksheet.Cells[44, 3].Value = "погодных условий,";
                worksheet.Cells[45, 3].Value = "указаний региональных";
                worksheet.Cells[46, 3].Value = "органов власти.";

                worksheet.Cells[48, 3].Value = "Тема №8 - Доведение";
                worksheet.Cells[49, 3].Value = "информации о проводимых";
                worksheet.Cells[50, 3].Value = "в районе работы бригад";
                worksheet.Cells[51, 3].Value = "инкассаторов митингов,";
                worksheet.Cells[52, 3].Value = "шествий или других";
                worksheet.Cells[53, 3].Value = "общественных мероприятий в";
                worksheet.Cells[54, 3].Value = "связи, проведением которых";
                worksheet.Cells[55, 3].Value = "может быть перекрыто либо";
                worksheet.Cells[56, 3].Value = "ограничено движение";
                worksheet.Cells[57, 3].Value = "транспортных средств.";
  

            
                    worksheet.Column(1).Width = 9;
                    worksheet.Column(2).Width = 9;
                    worksheet.Column(3).Width = 23;
                    worksheet.Column(4).Width = 17;
                    worksheet.Column(5).Width = 23;
                    worksheet.Column(6).Width = 14;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 11;

                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                    worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 " + formattedDate2;
                    worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:1"];

                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        DefaultExt = ".xlsx",
                        FileName = "Журнал инструктажа"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        SaveExcelWithPageLayoutView(excelPackage, saveFileDialog.FileName);
                    }

                FillData();
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

        private void Inventory_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow3 journalCollectorWindow = new JournalCollectorWindow3(daTe);
            journalCollectorWindow.Show();
            Close();
        }

        private void Appearances_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow5 journalCollectorWindow = new JournalCollectorWindow5(daTe);
            journalCollectorWindow.Show();
            Close();
        }

        private void Button_AtmWindow(object sender, RoutedEventArgs e)
        {
            AtmWindow atmWindow = new AtmWindow(daTe);
            atmWindow.Show();
            Close();
        }
    }
}
