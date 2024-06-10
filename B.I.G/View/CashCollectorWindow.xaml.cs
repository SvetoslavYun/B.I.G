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
using System.Data.SQLite;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace B.I.G

{
    public partial class CashCollectorWindow : System.Windows.Window
    {   private DateTime Date;
        public static cashCollector CashCollector;
        ObservableCollection<cashCollector> CashCollectors;
        private СashCollectorController сashCollectorController;
        public cashCollector SelectedProduct { get; set; }
        СashCollectorController cashCollectorController = new СashCollectorController();

        ObservableCollection<user_account> User_Accounts;
        private User_accountController user_AccountController;

        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;
        public static bool flag;
        public static bool flagEdit;         
        public CashCollectorWindow(DateTime date)
        {
            CashCollectors = new ObservableCollection<cashCollector>();
            сashCollectorController = new СashCollectorController();

            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();

            User_Accounts = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            
            InitializeComponent();
            dGridCollector.DataContext = CashCollectors;
            Date = date;
            FillData();
            ImgBox.DataContext = this;
            Name.TextChanged += Search;
            SelectedProduct = new cashCollector { image = MainWindow.image_Profil };
            AccesText.Text = MainWindow.acces;
            NameText.Text = MainWindow.LogS;
            Name.Text = MainWindow.NameCollector;
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
            e.Row.Header = e.Row.GetIndex() + 1;
        }      

        public void FillData()
        {
            try

            {
                CashCollectors.Clear();
                foreach (var item in сashCollectorController.GetAllCashCollectors())
                {
                    CashCollectors.Add(item);
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
                    List<cashCollector> cashCollectors = dGridCollector.SelectedItems.Cast<cashCollector>().ToList();
                    {
                        // проход по списку выбранных строк
                        foreach (cashCollector CashCollectors in cashCollectors)
                        {
                            var Id = CashCollectors.id;
                            string name = CashCollectors.fullname;
                            сashCollectorController.Delete(Id, NameText.Text);
                            сashCollectorController.Delete2(Id, NameText.Text);

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
                SelectedProduct = new cashCollector { image = MainWindow.image_Profil };
                AccesText.Text = MainWindow.acces;
                NameText.Text = MainWindow.LogS;
                MainWindow.NameCollector = Name.Text;
                var searchResults = сashCollectorController.SearchCollectorName(Name.Text);

                CashCollectors.Clear();
                    foreach (var result in searchResults)
                    {
                    CashCollectors.Add(result);
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
                    process = "Сформировал: Список инкассаторов и водителей 'B.I.G'",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);
               
                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("cashCollector");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridCollector.Items.Count + 1, dGridCollector.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине

                    cells.Style.Font.Size = 12; // Установите нужный размер шрифта

                }

                // Добавление заголовков столбцов и порядковых номеров

                for (int i = 1; i <= dGridCollector.Columns.Count; i++)
                {
                    worksheet.Cells[1, i].Value = dGridCollector.Columns[i - 1].Header;
                }

             
                // Добавление данных
                for (int i = 0; i < dGridCollector.Items.Count; i++)
                {               
                    var collectorItem = (cashCollector)dGridCollector.Items[i];
                    worksheet.Cells[i + 2, 2].Value = collectorItem.name;
                    worksheet.Cells[i + 2, 3].Value = collectorItem.fullname;
                    worksheet.Cells[i + 2, 4].Value = collectorItem.phone;
                    worksheet.Cells[i + 2, 5].Value = collectorItem.profession;
                    worksheet.Cells[i + 2, 6].Value = collectorItem.gun;
                    worksheet.Cells[i + 2, 7].Value = collectorItem.automaton_serial;
                    worksheet.Cells[i + 2, 8].Value = collectorItem.automaton;
                    worksheet.Cells[i + 2, 9].Value = collectorItem.permission;
                    worksheet.Cells[i + 2, 10].Value = collectorItem.meaning;
                    worksheet.Cells[i + 2, 11].Value = collectorItem.certificate;
                    worksheet.Cells[i + 2, 12].Value = collectorItem.token;
                    worksheet.Cells[i + 2, 13].Value = collectorItem.power;
                    for (int col = 2; col <= 13; col++)
                    {
                        worksheet.Cells[i + 2, col].Style.Font.Size = 10; // Установите нужный размер шрифта
                    }

                }
                worksheet.DeleteColumn(1);
                // Автоподгон ширины колонок
                worksheet.Cells.AutoFitColumns();
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 Список инкассаторов и водителей 'B.I.G'";
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:1"];

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = "Список инкассаторов и водителей 'B.I.G'"
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

            }
        }








        private async void Button_cleaning(object sender, RoutedEventArgs e)
        {
            Name.Text = string.Empty;
            await OverwriteDatabaseAsync();
            FillData();
        }

        private void Button_LogWindow(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow(Date);
            logWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow(Date);
            usersWindow.Show();
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
                   
                    cashCollectorController.ImportExcelToDatabase(openFileDialog.FileName);
                    Search(sender, e);
                }
               
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void Button_OrderrWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow2 journalCollectorWindow = new JournalCollectorWindow2();
            journalCollectorWindow.Show();

            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void LookCollectoButton_LogWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow journalCollectorWindow = new JournalCollectorWindow(Date);
            journalCollectorWindow.Show();
            var currentWindow = Window.GetWindow(this);

            // Закрыть текущее окно
            currentWindow.Close();
        }

        private void Inventory_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow3 journalCollectorWindow = new JournalCollectorWindow3(Date);
            journalCollectorWindow.Show();
            Close();
        }

        private void Briefing_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow4 journalCollectorWindow = new JournalCollectorWindow4(Date);
            journalCollectorWindow.Show();
            Close();
        }

        private void Appearances_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow5 journalCollectorWindow = new JournalCollectorWindow5(Date);
            journalCollectorWindow.Show();
            Close();
        }

        private void Button_AtmWindow(object sender, RoutedEventArgs e)
        {
            AtmWindow atmWindow = new AtmWindow(Date);
            atmWindow.Show();
            Close();
        }
    }
}
