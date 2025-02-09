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
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Media.Media3D;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Data.SQLite;
using System.Data;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;

namespace B.I.G

{
    public partial class UsersWindow : System.Windows.Window
    {
        private string Area;
        private DateTime daTe;
        public static user_account User;
        public static bool flag;
        public static bool flagEdit;
        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;
        ObservableCollection<user_account> User_Accounts;
        private User_accountController user_AccountController;
        public user_account SelectedProduct { get; set; }
        public UsersWindow(DateTime date, string area)
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();
            User_Accounts = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            InitializeComponent();
            dGridUser.DataContext = User_Accounts;
            daTe = date;
            Area = area;
            FillData();
            ImgBox.DataContext = this;
            Name.TextChanged += Search;
            SelectedProduct = new user_account { image = MainWindow.image_Profil };
            AccesText.Text = MainWindow.acces;
            NameText.Text = MainWindow.LogS;
            Name.Text = MainWindow.nameUser;
            if (AccesText.Text != "Администратор")
            {
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
                User_Accounts.Clear();
                foreach (var item in user_AccountController.GetAllUsers())
                {
                    User_Accounts.Add(item);
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
            Add_User add_User = new Add_User();
            add_User.Owner = this;
            add_User.ShowDialog();
            Search(sender, e);        
        }

        private void DoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                

                if (dGridUser.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var id = ((user_account)dGridUser.SelectedItem).id;
                flag = false;
                User = (user_account)dGridUser.SelectedItem;
                Add_User add_User = new Add_User();
                add_User.Owner = this;
                add_User.ShowDialog();
                if (flagEdit)
                {
                    flagEdit = false;
                    UsersWindow usersWindow = new UsersWindow(daTe, Area);
                    usersWindow.Show();
                    Close();                   
                }
                else 
                { 
                    Search(sender, e); 
                }
               
                User = null;

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

                if (dGridUser.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var id = ((user_account)dGridUser.SelectedItem).id;
                flag = false;
                User = (user_account)dGridUser.SelectedItem;
                Add_User add_User = new Add_User();
                add_User.Owner = this;
                add_User.ShowDialog();
                if (flagEdit)
                {
                    flagEdit = false;
                    UsersWindow usersWindow = new UsersWindow(daTe, Area);
                    usersWindow.Show();
                    Close();
                }
                else
                {
                    Search(sender, e);
                }
                User = null;

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
                if (dGridUser.SelectedItem == null) throw new Exception("Не выбрана строка, произведите выбор");
                var result = MessageBox.Show("Вы уверены?", "Удалить запись", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                { // получение выбранных строк
                    List<user_account> user = dGridUser.SelectedItems.Cast<user_account>().ToList();
                    {
                        // проход по списку выбранных строк
                        foreach (user_account Users in user)
                        {
                            var Id = Users.id;
                            string name = Users.username;
                            user_AccountController.Delete(Id, NameText.Text);

                            DateTime Date = DateTime.Now;
                            string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
                            string formattedDate2 = Date.ToString("dd.MM.yyyy");
                            var Log = new log()
                            {
                                username = MainWindow.LogS,
                                process = "Удалил пользователя: " + name + "",
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
                SelectedProduct = new user_account { image = MainWindow.image_Profil };
                AccesText.Text = MainWindow.acces;
                NameText.Text = MainWindow.LogS;
                MainWindow.nameUser = Name.Text;
                var searchResults = user_AccountController.SearchUsername(Name.Text);

                User_Accounts.Clear();
                    foreach (var result in searchResults)
                    {
                    User_Accounts.Add(result);
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
                    process = "Сформировал: Список пользователей 'B.I.G'",
                    date = Convert.ToDateTime(formattedDate),
                    date2 = Convert.ToDateTime(formattedDate2)
                };
                log_Controller.Insert(Log2);
               
                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("user_account");

                // Установка стилей для линий ячеек, ширины колонок и выравнивания
                using (var cells = worksheet.Cells[1, 1, dGridUser.Items.Count + 1, dGridUser.Columns.Count])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине

                }

                // Добавление заголовков столбцов и порядковых номеров

                for (int i = 1; i <= dGridUser.Columns.Count; i++)
                {
                    worksheet.Cells[1, i].Value = dGridUser.Columns[i - 1].Header;
                }

                // Добавление данных
                for (int i = 0; i < dGridUser.Items.Count; i++)
                {
                    var logItem = (user_account)dGridUser.Items[i];
                    worksheet.Cells[i + 2, 1].Value = logItem.id;
                    worksheet.Cells[i + 2, 2].Value = logItem.username;
                    worksheet.Cells[i + 2, 3].Value = logItem.password_hash;
                    worksheet.Cells[i + 2, 4].Value = logItem.access;

                }
                worksheet.DeleteColumn(1);
                // Автоподгон ширины колонок
                worksheet.Cells.AutoFitColumns();
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "&\"Arial\"&06&K000000 Сформировал: " + MainWindow.LogS + ". " + Date;
                worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial,Bold Italic\"&10&K000000 Список пользователей 'B.I.G'";

                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:1"];

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = "Список пользователей 'B.I.G'"
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
            string[] tableNames = { "user_accounts" };

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
            LogWindow logWindow = new LogWindow(daTe, Area);
            logWindow.Show();
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

       

            // Закрыть текущее окно
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
            JournalCollectorWindow4 journalCollectorWindow = new JournalCollectorWindow4(daTe, Area);
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
