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


namespace B.I.G

{
    public partial class LogWindow : System.Windows.Window
    {
        private string Area;
        private DateTime daTe;
        public static string names= MainWindow.LogS;
        ObservableCollection<log> Logs;
        private Log_Controller log_Controller;
        public user_account SelectedProduct { get; set; }
        public LogWindow(DateTime date, string area)
        {
            Logs = new ObservableCollection<log>();
            log_Controller= new Log_Controller();
            InitializeComponent();
            dGridLog.DataContext = Logs;
            daTe = date;
            Area = area;
            FillData();
            ImgBox.DataContext = this;
            Name.Text = MainWindow.LognameUser;
            Date.Text = MainWindow.LogDate;
            Date2.Text = MainWindow.LogDate2;
            Name.TextChanged += Search;
            SelectedProduct = new user_account { image = MainWindow.image_Profil };
            AccesText.Text = MainWindow.acces;
            NameText.Text = MainWindow.LogS;
                     
        }

        private void Date_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true; // Отменить обработку события, чтобы предотвратить ввод текста
        }

        private void dGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void FillData()
        {
            try

            {
                Logs.Clear();
                foreach (var item in log_Controller.GetAllLogs())
                {
                    Logs.Add(item);
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
                if (dGridLog.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var result = MessageBox.Show("Вы уверены?", "Удалить запись", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                { // получение выбранных строк
                    List<log> logs = dGridLog.SelectedItems.Cast<log>().ToList();
                    {
                        DateTime Date = DateTime.Now;
                        string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                        string formattedDate2 = Date.ToString("dd.MM.yyyy");
                        var Log2 = new log()
                        {
                            username = MainWindow.LogS,
                            process = "Удалил историю событий",
                            date = Convert.ToDateTime(formattedDate),
                            date2 = Convert.ToDateTime(formattedDate2)
                        };
                        log_Controller.Insert(Log2);
                        // проход по списку выбранных строк
                        foreach (log Log in logs)
                        {
                            var Id = Log.id;
                            log_Controller.Delete(Id);
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
                if (string.IsNullOrEmpty(Name.Text)) { MainWindow.LognameUser = Name.Text; }
               

                if (!string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && !string.IsNullOrEmpty(Date2.Text))
                {
                    MainWindow.LogDate = Date.Text;
                    MainWindow.LogDate2 = Date2.Text;
                    MainWindow.LognameUser = Name.Text;
                    var searchResults = log_Controller.Search_Name_Between_dates (Name.Text,Convert.ToDateTime(Date.Text), Convert.ToDateTime(Date2.Text));

                    Logs.Clear();
                    foreach (var result in searchResults)
                    {
                        Logs.Add(result);
                    }
                }
                if (string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && !string.IsNullOrEmpty(Date2.Text))
                {
                    MainWindow.LogDate = Date.Text;
                    MainWindow.LogDate2 = Date2.Text;
                    var searchResults = log_Controller.Search_Between_dates(Convert.ToDateTime(Date.Text), Convert.ToDateTime(Date2.Text));

                    Logs.Clear();
                    foreach (var result in searchResults)
                    {
                        Logs.Add(result);
                    }
                }

                if (string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
                {
                    MainWindow.LogDate = Date.Text;
                    var searchResults = log_Controller.SearchDate(Convert.ToDateTime(Date.Text));

                    Logs.Clear();
                    foreach (var result in searchResults)
                    {
                        Logs.Add(result);
                    }
                }
                if (!string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
                {
                    MainWindow.LognameUser = Name.Text;
                    MainWindow.LogDate = Date.Text;
                    var searchResults = log_Controller.SearchNameDate(Name.Text,Convert.ToDateTime(Date.Text));

                    Logs.Clear();
                    foreach (var result in searchResults)
                    {
                        Logs.Add(result);
                    }
                }
                if (string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
                {
                    MainWindow.LogDate = Date.Text;
                    var searchResults = log_Controller.SearchDate(Convert.ToDateTime(Date.Text));

                    Logs.Clear();
                    foreach (var result in searchResults)
                    {
                        Logs.Add(result);
                    }
                }
                if (string.IsNullOrEmpty(Name.Text) && string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
                {
                    MainWindow.LogDate = string.Empty;
                    MainWindow.LogDate2 = string.Empty;
                    var allResults = log_Controller.GetAllLogs();

                    Logs.Clear();
                    foreach (var result in allResults)
                    {
                        Logs.Add(result);
                    }
                }
                else if (!string.IsNullOrEmpty(Name.Text) && string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
                {
                    MainWindow.LognameUser = Name.Text;
                    var searchResults = log_Controller.SearchUsername(Name.Text);

                    Logs.Clear();
                    foreach (var result in searchResults)
                    {
                        Logs.Add(result);
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
                    process = "Сформировал: Журнал событий 'B.I.G''",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);

                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("log");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridLog.Items.Count + 1, dGridLog.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине

                }

                // Добавление заголовков столбцов и порядковых номеров

                for (int i = 1; i <= dGridLog.Columns.Count; i++)
                {
                    worksheet.Cells[1, i].Value = dGridLog.Columns[i - 1].Header;
                }

                // Добавление данных
                for (int i = 0; i < dGridLog.Items.Count; i++)
                {
                    var logItem = (log)dGridLog.Items[i];

                    worksheet.Cells[i + 2, 1].Value = logItem.id;
                    worksheet.Cells[i + 2, 2].Value = logItem.username;
                    worksheet.Cells[i + 2, 3].Value = logItem.process;
                    worksheet.Cells[i + 2, 4].Value = logItem.date.ToString("dd.MM.yyyy HH:mm");

                }

                // Автоподгон ширины колонок
                worksheet.Cells.AutoFitColumns();

                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 Журнал событий 'B.I.G'";

                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;


                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:1"];

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = "Журнал событий 'B.I.G'"
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





        //для журнала жетонов
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("Logs");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridLog.Items.Count + 1, dGridLog.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине
                    cells.Style.WrapText = true; // Разрешаем перенос текста
                }

                // Добавление сетки после последней строки данных
                using (var cells = worksheet.Cells[dGridLog.Items.Count + 2, 1, dGridLog.Items.Count + 3, dGridLog.Columns.Count])
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
                worksheet.Cells[1, 2, 1, 3].Merge = true;

                // Установка значения в объединенной ячейке
                worksheet.Cells[1, 2].Value = "Выдано инкассаторам";
                // Добавление заголовков столбцов и порядковых номеров

                for (int i = 1; i <= dGridLog.Columns.Count; i++)
                {
                    worksheet.Cells[2, i].Value = dGridLog.Columns[i - 1].Header;
                    worksheet.Cells[3, i].Value = i;
                }


                // Добавление данных
                for (int i = 0; i < dGridLog.Items.Count; i++)
                {
                    var logItem = (log)dGridLog.Items[i];

                    worksheet.Cells[i + 4, 1].Value = logItem.id;
                    worksheet.Cells[i + 4, 2].Value = logItem.username;
                    worksheet.Cells[i + 4, 3].Value = logItem.process;
                    worksheet.Cells[i + 4, 4].Value = logItem.date.ToString("dd.MM.yyyy HH:mm");
                }


                // Автоподгон ширины колонок
                worksheet.Cells.AutoFitColumns();
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;

                // Установка текста в колонтитуле
                worksheet.HeaderFooter.OddHeader.RightAlignedText = "&\"Arial\"&10&K000000 sviatoslavyun@gmail.com";

                // Установка альбомной ориентации
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                // Установка строк заголовка, которые будут повторяться при выводе на печать
                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:3"];
                // Установка текста в колонтитуле для повторяющихся страниц
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    SaveExcelWithPageLayoutView(excelPackage, saveFileDialog.FileName);
                }
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
            Date.Text = string.Empty;
            Date2.Text = string.Empty;
            await OverwriteDatabaseAsync();
            Search(sender, e);
        }

        private void Button_UsersWindow(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow(daTe, Area);
            usersWindow.Show();
            Close();
        }

        private void Button_CollectorWindow(object sender, RoutedEventArgs e)
        {
            CashCollectorWindow cashCollectorWindow = new CashCollectorWindow(daTe, Area);
            cashCollectorWindow.Show();
             Close();
        }

        private void Button_OrderrWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow2 journalCollectorWindow = new JournalCollectorWindow2(Area);
            journalCollectorWindow.Show();
                  Close();
        }

        private void LookCollectoButton_LogWindow(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow journalCollectorWindow = new JournalCollectorWindow(daTe, Area);
            journalCollectorWindow.Show();
           Close();

        }

        private void Inventory_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow3 journalCollectorWindow = new JournalCollectorWindow3(daTe, Area);
            journalCollectorWindow.Show();
            Close();
        }

        private void Briefing_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow4 journalCollectorWindow = new JournalCollectorWindow4(Convert.ToDateTime(Date.Text), Area);
            journalCollectorWindow.Show();
            Close();
        }

        private void Appearances_Button(object sender, RoutedEventArgs e)
        {
            JournalCollectorWindow5 journalCollectorWindow = new JournalCollectorWindow5(daTe, Area);
            journalCollectorWindow.Show();
            Close();
        }

        private void Button_AtmWindow(object sender, RoutedEventArgs e)
        {
            AtmWindow atmWindow = new AtmWindow(daTe, Area);
            atmWindow.Show();
            Close();
        }
    }
}
