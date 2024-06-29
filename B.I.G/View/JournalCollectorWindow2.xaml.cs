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
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Path = System.IO.Path;
using System.Threading;
using System.Data.SQLite;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace B.I.G

{
    public partial class JournalCollectorWindow2 : System.Windows.Window
    {    
        private Puth_Controller puth_Controller;
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
        private bool flag2;
        public static bool flag;
        public static bool flagEdit;
        public JournalCollectorWindow2(string area)
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
            if (string.IsNullOrEmpty(Date.Text))
            {
                Date.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            Area.Text = area;
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
            Area.Items.Add("пр.Дзержинского, 69");
            Area.Items.Add("ул.Фабрициуса, 8б");
            Area.Items.Add("Все");
            Area.Items.Add("Загрузить наряд");
            RouteButton.Visibility = Visibility.Collapsed;
            RouteButton.IsEnabled = false;
            ReserveButton.Visibility = Visibility.Collapsed;
            ReserveButton.IsEnabled = false;
        }

        // Event handler for preventing text input
        private void Area_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = true; // This will prevent any text input
        }

        // Event handler for preventing certain key presses
        private void Area_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Space)
            {
                e.Handled = true; // This will prevent Backspace, Delete, and Space keys
            }
        }

     


        private void Area_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.IsEditable)
            {
                var textBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
                if (textBox != null)
                {
                    textBox.TextChanged += Search;
                }
            }
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
                AtmButton.Visibility = Visibility.Collapsed;
                AtmButton.IsEnabled = false;
                AppearancesButton.Visibility = Visibility.Collapsed;
                AppearancesButton.IsEnabled = false;
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
                BriefingButton.Visibility = Visibility.Collapsed;
                BriefingButton.IsEnabled = false;
                InventoryButton.Visibility = Visibility.Collapsed;
                InventoryButton.IsEnabled = false;
                RouteButton.Visibility = Visibility.Collapsed;
                RouteButton.IsEnabled = false;
                ReserveButton.Visibility = Visibility.Collapsed;
                ReserveButton.IsEnabled = false;
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
                foreach (var item in journalCollectorController.GetAllCashCollectors(Convert.ToDateTime(Date.Text), Area.Text))
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
            if (Area.Text == "" || Area.Text == "Все")
            {
                MessageBox.Show("Выберите площадку");
                return;
            }

            if (ReserveButton.Visibility == Visibility.Visible && ReserveButton.IsEnabled)
            {
                ReserveButton.Visibility = Visibility.Collapsed;
                ReserveButton.IsEnabled = false;
                RouteButton.Visibility = Visibility.Collapsed;
                RouteButton.IsEnabled = false;
            }
            else
            {
                ReserveButton.Visibility = Visibility.Visible;
                ReserveButton.IsEnabled = true;
                RouteButton.Visibility = Visibility.Visible;
                RouteButton.IsEnabled = true;
            }
        }


        private void ScrollToLastRow(DataGrid dataGrid)
        {
            if (dataGrid.Items.Count > 0)
            {
                object item = dataGrid.Items[dataGrid.Items.Count - 1];
                dataGrid.ScrollIntoView(item);
                dataGrid.SelectedItem = item;
                dataGrid.UpdateLayout();
            }
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
                journalCollectorController.UpdateJournalBase2(Convert.ToDateTime(Date.Text));
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
                if (AccesText.Text != "Пользователь")
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
                    journalCollectorController.UpdateJournalBase2(Convert.ToDateTime(Date.Text));
                    Search(sender, e);
                    JournalCollector = null;
                }
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
                if (AccesText.Text != "Пользователь")
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
                        journalCollectorController.UpdateJournalBase2(Convert.ToDateTime(Date.Text));
                        Search(sender, e);
                        JournalCollector = null;
                    }
                    else { MessageBox.Show("Данные по сотруднику отсутствуют"); }
                }
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
                if (AccesText.Text != "Пользователь")
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
                                journalCollectorController.UpdateJournalBase2(Convert.ToDateTime(Date.Text));
                                Search(sender, e);
                            }
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
                if (flag2 == true) { Empty(); }
                else
                {
                    SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
                    AccesText.Text = MainWindow.acces;
                    NameText.Text = MainWindow.LogS;
                    MainWindow.NameJorunal = Name.Text;
                    var searchResults = journalCollectorController.SearchCollectorName(Name.Text, Convert.ToDateTime(Date.Text), Route.Text, Area.Text);

                    JournalCollectors.Clear();
                    foreach (var result in searchResults)
                    {
                        JournalCollectors.Add(result);
                    }

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
                    process = "Сформировал: Наряд на работу",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);

                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Наряд на работу");

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
                    worksheet.Cells[1, i].Style.Font.Bold = true;
                }

                int I = 0;
                // Добавление данных
                for (int i = 0; i < dGridCollector.Items.Count; i++)
                {
                    var collectorItem = (journalCollector)dGridCollector.Items[i];

                    // Создание строки
                    var row = worksheet.Row(i + 2);
                    row.Height = 20;
                    worksheet.Cells[i + 2, 3].Value = collectorItem.profession;
                    worksheet.Cells[i + 2, 4].Value = collectorItem.name;
                    worksheet.Cells[i + 2, 5].Value = collectorItem.dateWork;
                    worksheet.Cells[i + 2, 8].Value = collectorItem.appropriation;
                    I = i;

                    for (int col = 2; col <= 7; col++)
                    {
                        worksheet.Cells[i + 2, col].Style.Font.Size = 7; // Установите нужный размер шрифта
                    }

                    // Добавьте условие для проверки значения collectorItem.fullname
                    if (collectorItem.fullname == ".")
                    {
                        worksheet.Cells[i + 2, 3].Style.Font.Size = 10; // Установите нужный размер шрифта
                        row.Height = 15;
                        worksheet.Cells[i + 2, 3].Value = worksheet.Cells[i + 2, 5].Value;
                        worksheet.Cells[i + 2, 3, i + 2, 8].Merge = true;
                        // Установите стиль заливки для первых семь колонок
                        for (int col = 2; col <= 8; col++)
                        {
                            worksheet.Cells[i + 2, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 2, col].Style.Fill.BackgroundColor.SetColor(Color.White);
                            worksheet.Cells[i + 2, col].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells[i + 2, col].Style.Font.Bold = true; // Установите шрифт жирным
                            worksheet.Cells[i + 2, col].Style.Font.Italic = true; // Установите шрифт курсивом
                        }
                    }
                }

                I = I + 4;

                worksheet.Cells[I, 1, I, 8].Merge = true;
                string Spaces = new string(' ', 116);
                string spaces = new string(' ', 53);
                worksheet.Cells[I, 3].Value = "\n\n\nНачальник службы инкассации    \n___________________         ___________________________________";
                worksheet.Cells[I+1, 1, I+1, 8].Merge = true;
                string Spaces2 = new string(' ', 116);
                string spaces2 = new string(' ', 53);   
                worksheet.Cells[I+1, 3].Value = "                                                                                  (подпись)                                   (инициалы, фамилия)";

                worksheet.DeleteColumn(1,2);
                

                // Автоподгон ширины колонок
                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 8;
                worksheet.Column(4).Width = 11;
                worksheet.Column(5).Width = 16;
                worksheet.Column(6).Width = 15;
                if (Area.Text == "Все" || Area.Text == "") Area.Text = "Минск";
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&08&K000000\nНАРЯД НА РАБОТУ \nна " + formattedDate2;
                worksheet.HeaderFooter.OddHeader.LeftAlignedText = "&\"Arial\"&07&K000000Служба инкассации  Региональное управление №1 " + Area.Text;

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
                if (Area.Text == "Минск" || Area.Text == "") Area.Text = "Все";
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

        private async Task<DataTable> GetTableDataAsync(string databasePath, string tableName)
        {
            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM {tableName}";

            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }

        private async Task ClearTableAsync(string databasePath, string tableName)
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand($"DELETE FROM {tableName}", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task InsertTableDataAsync(string databasePath, string tableName, DataTable tableData)
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (DataRow row in tableData.Rows)
                    {
                        string query = $"INSERT OR REPLACE INTO {tableName} ({string.Join(", ", tableData.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}) " +
                                       $"VALUES ({string.Join(", ", tableData.Columns.Cast<DataColumn>().Select(c => "@" + c.ColumnName))})";

                        using (var command = new SQLiteCommand(query, connection))
                        {
                            foreach (DataColumn column in tableData.Columns)
                            {
                                command.Parameters.AddWithValue("@" + column.ColumnName, row[column.ColumnName]);
                            }

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        private async Task CopyDatabaseTablesAsync(string sourcePath, string destinationPath)
        {
            string[] tableNames = { "atms", "cashCollectors", "journalCollectors", "logs", "puths", "user_accounts" };

            foreach (string tableName in tableNames)
            {
                DataTable tableData = await GetTableDataAsync(sourcePath, tableName);
                await ClearTableAsync(destinationPath, tableName); // Очистка таблицы перед вставкой новых данных
                await InsertTableDataAsync(destinationPath, tableName, tableData);
            }

            // Заполнить данные на экране после обновления базы данных (если необходимо)
            // Dispatcher.Invoke(FillData);
        }

        public async Task OverwriteDatabaseAsync()
        {
            try
            {
                // Получение пути к директории базы данных из текстового поля
                string sourceDirectory = MainWindow.puth;

                // Проверка, пустое ли текстовое поле
                if (string.IsNullOrWhiteSpace(sourceDirectory))
                {
                    MessageBox.Show("Путь к директории базы данных не указан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Добавление имени файла базы данных к указанному пути
                string sourcePath = Path.Combine(sourceDirectory, "B.I.G.db");

                // Путь к файлу базы данных в целевом расположении (корень программы)
                string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "B.I.G.db");

                // Проверка наличия файла базы данных в источнике
                if (!File.Exists(sourcePath))
                {
                    MessageBox.Show("Файл базы данных в указанном источнике не найден, дальнейшие сохранения будут на локальном диске", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Выполнение копирования данных из всех таблиц
                await CopyDatabaseTablesAsync(sourcePath, destinationPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void Button_cleaning(object sender, RoutedEventArgs e)
        {
            flag2 = false;
            Name.Text = string.Empty;
            Route.Text = string.Empty;

                await OverwriteDatabaseAsync();


            FillData();
        }



        private void Button_LogWindow(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow(Convert.ToDateTime(Date.Text), Area.Text);
            logWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow(Convert.ToDateTime(Date.Text), Area.Text);
            usersWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Button_import_to_excel(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Area.Text == "" || Area.Text == "Все") { MessageBox.Show("Выберите площадку"); return; }
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

        private void ImportButton_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                string area=Area.Text;
                
                DateTime date = DateTime.Now;
                if (Calendar2.SelectedDate.HasValue)
                {
                    ; date = Calendar2.SelectedDate.Value;
                }
                if (!journalCollectorController.ImportSerchData(date, Area.Text))
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
                        journalCollectorController.ImportExcelToDatabase(openFileDialog.FileName, date, area, sender as BackgroundWorker, (progressPercentage) =>
                        {
                            (sender as BackgroundWorker).ReportProgress(progressPercentage);
                        });
                        journalCollectorController.UpdateResponsibilities(date,area);
                        journalCollectorController.DeleteRound2(date);
                        journalCollectorController.UpdateJournalBase2(date);
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
                            journalCollectorController.DeleteToDate(date,area);
                            journalCollectorController.ImportExcelToDatabase(openFileDialog.FileName, date,area, sender as BackgroundWorker, (progressPercentage) =>
                            {
                                (sender as BackgroundWorker).ReportProgress(progressPercentage);
                            });
                            journalCollectorController.UpdateResponsibilities(date, area);
                            journalCollectorController.DeleteRound2(date);
                            journalCollectorController.UpdateJournalBase2(date);
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



        private void SetButtonsEnabled(DependencyObject parent, bool isEnabled)
        {
            if (parent == null)
                return;

            // Обходим всех детей в визуальном дереве
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Если ребенок - кнопка, то изменяем свойство IsEnabled
                if (child is Button button)
                {
                    button.IsEnabled = isEnabled;
                }

                // Рекурсивно обходим детей текущего элемента
                SetButtonsEnabled(child, isEnabled);
            }
        }


        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {               
                DateTime date = DateTime.Now;
                if (Calendar2.SelectedDate.HasValue)
                {
                    date = Calendar2.SelectedDate.Value;
                }

                if (!journalCollectorController.ImportSerchData(date, Area.Text))
                {
                              
                    JournalCollectors.Clear();
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        if (Calendar2.SelectedDate.HasValue)
                        {
                            date = Calendar2.SelectedDate.Value;
                        }

                        StartImport(openFileDialog.FileName, date);
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
                    var result = MessageBox.Show("Наряд с этой датой и площадкой уже сформирован.\nПереформировать заново?", "", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    { 
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                        if (openFileDialog.ShowDialog() == true)
                        {
                            JournalCollectors.Clear();
                            if (Calendar2.SelectedDate.HasValue)
                            {
                                date = Calendar2.SelectedDate.Value;
                            }                        
                            journalCollectorController.DeleteToDate2(date,Area.Text);
                            StartImport(openFileDialog.FileName, date);
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

        private void StartImport(string filePath, DateTime date)
        {     // Заблокировать все кнопки
            SetButtonsEnabled(this, false);
            ProgressBar.Visibility = Visibility.Visible;
            ProgressText.Visibility = Visibility.Visible;
            string area = Area.Text;
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += (sender, e) =>
            {
                dynamic arguments = e.Argument;

                try
                {
                    if (area == "Загрузить наряд")
                    {
                        journalCollectorController.ImportExcelToDatabase2(filePath, date, area, sender as BackgroundWorker, (progressPercentage) =>
                        {
                            (sender as BackgroundWorker).ReportProgress(progressPercentage);
                        });
                    }
                    else
                    {
                        journalCollectorController.ImportExcelToDatabase(filePath, date, area, sender as BackgroundWorker, (progressPercentage) =>
                        {
                            (sender as BackgroundWorker).ReportProgress(progressPercentage);
                        });
                    }

                }
                catch (Exception ex)
                {
                    e.Result = ex.Message;
                }
            };
            backgroundWorker.ProgressChanged += (sender, e) =>
            {
                ProgressBar.Value = e.ProgressPercentage;
                ProgressText.Text = $"{e.ProgressPercentage}%";
            };
            backgroundWorker.RunWorkerCompleted += (sender, e) =>
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                ProgressText.Visibility = Visibility.Collapsed;

                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                }
                else if (e.Result != null)
                {
                    MessageBox.Show($"Операция завершена с результатом: {e.Result}");
                }
                else
                {
                    MessageBox.Show("Операция успешно завершена.");
                }
                if (area == "Загрузить наряд")
                {                   
                    journalCollectorController.UpdateResponsibilities22(date, area);
                    journalCollectorController.UpdateRoute2(date);
                    journalCollectorController.DeleteRound22(date);                   
                }
                journalCollectorController.UpdateResponsibilities(date,area);
                journalCollectorController.DeleteRound2(date);             
                journalCollectorController.UpdateJournalBase2(date);              
                Date.Text = date.ToString("yyyy-MM-dd");
                // Разблокировать все кнопки
                SetButtonsEnabled(this, true);
                Area.Text = "Все";
                FillData();
            };

            backgroundWorker.RunWorkerAsync(new { filePath, date });
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
            CashCollectorWindow cashCollectorWindow = new CashCollectorWindow(Convert.ToDateTime(Date.Text), Area.Text);
            cashCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void LookCollectoButton_LogWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow journalCollectorWindow = new JournalCollectorWindow(Convert.ToDateTime(Date.Text), Area.Text);
            journalCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Inventory_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow3 journalCollectorWindow = new JournalCollectorWindow3(Convert.ToDateTime(Date.Text), Area.Text);
            journalCollectorWindow.Show();
            Close();
        }

        private void Briefing_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow4 journalCollectorWindow = new JournalCollectorWindow4(Convert.ToDateTime(Date.Text), Area.Text);
            journalCollectorWindow.Show();
            Close();
        }

        private void Appearances_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow5 journalCollectorWindow = new JournalCollectorWindow5(Convert.ToDateTime(Date.Text), Area.Text);
            journalCollectorWindow.Show();
            Close();

        }

        private void Button_DelDate(object sender, RoutedEventArgs e)
        {
            if (AccesText.Text != "Пользователь")
            {
                var result = MessageBox.Show("Вы уверены?", "Удалить наряд на " + Date.Text, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    journalCollectorController.DeleteToDate(Convert.ToDateTime(Date.Text),Area.Text);
                    journalCollectorController.UpdateJournalBase2(Convert.ToDateTime(Date.Text));
                    Search(sender, e);
                }
            }
        }

        private void Empty()
        {
            try

            {
                SelectedProduct = new journalCollector { image = MainWindow.image_Profil };
                AccesText.Text = MainWindow.acces;
                NameText.Text = MainWindow.LogS;
                MainWindow.NameJorunal = Name.Text;
                var searchResults = journalCollectorController.SearchEmpty(Convert.ToDateTime(Date.Text));

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

        private void Button_Empty(object sender, RoutedEventArgs e)
        {
            if (flag2 == true) { flag2 = false; Search(sender, e); }
            else 
            { 
            flag2 = true;
            Empty();
            }
        }

        private void Button_AtmWindow(object sender, RoutedEventArgs e)
        {
            AtmWindow atmWindow = new AtmWindow(Convert.ToDateTime(Date.Text), Area.Text);
            atmWindow.Show();
            Close();
        }

        private void Button_export_to_Base(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Now;
            if (Calendar2.SelectedDate.HasValue)
            {
                date = Calendar2.SelectedDate.Value;
            }
            journalCollectorController.UpdateJournalBase2(date);
        }

     

        public void OverwriteDatabase()
        {
            try
            {
                // Получение пути к директории базы данных из текстового поля
                string sourceDirectory = MainWindow.puth;

                // Проверка, пустое ли текстовое поле
                if (string.IsNullOrWhiteSpace(sourceDirectory))
                {
                    MessageBox.Show("Путь к директории базы данных не указан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Добавление имени файла базы данных к указанному пути
                string sourcePath = Path.Combine(sourceDirectory, "B.I.G.db");

                // Путь к файлу базы данных в целевом расположении (корень программы)
                string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "B.I.G.db");

                // Проверка наличия файла базы данных в источнике
                if (!File.Exists(sourcePath))
                {
                    MessageBox.Show("Файл базы данных в указанном источнике не найден, дальнейшие сохранения будут на локальном диске", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Копирование файла базы данных с заменой существующего файла
                File.Copy(sourcePath, destinationPath, true);

                // Обновление пути в базе данных
                var Puth = new puth()
                {
                    adres = MainWindow.puth,
                };

                //MessageBox.Show("База данных успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка подключения к серверу: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Route_Button(object sender, RoutedEventArgs e)
        {
            RouteButton.Visibility = Visibility.Collapsed;
            RouteButton.IsEnabled = false;
            ReserveButton.Visibility = Visibility.Collapsed;
            ReserveButton.IsEnabled = false;
            RouteADD routeADD = new RouteADD(Convert.ToDateTime(Date.Text), Area.Text);           
            routeADD.ShowDialog();
            journalCollectorController.UpdateJournalBase2(Convert.ToDateTime(Date.Text));
            Search(sender, e);
        }

        private void Reserve_Button(object sender, RoutedEventArgs e)
        {
            RouteButton.Visibility = Visibility.Collapsed;
            RouteButton.IsEnabled = false;
            ReserveButton.Visibility = Visibility.Collapsed;
            ReserveButton.IsEnabled = false;
            int empty = journalCollectorController.EmptyRouteCount(Convert.ToDateTime(Date.Text)); // Вызов метода и присвоение результата переменной empty
            if (empty == 0)
            {
                journalCollectorController.Insert2(Convert.ToDateTime(Date.Text), Area.Text);
            }
            journalCollectorController.Insert(Convert.ToDateTime(Date.Text), Area.Text);
            journalCollectorController.UpdateJournalBase2(Convert.ToDateTime(Date.Text));
            Search(sender, e);
            // прокручиваем к последней строке
            ScrollToLastRow(dGridCollector);
        }
    }
}
