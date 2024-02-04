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
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Media.Media3D;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Data;
using Microsoft.Graph.Models;
using System.Runtime.InteropServices;

namespace B.I.G

{
    public partial class JournalCollectorWindow : System.Windows.Window
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
        public JournalCollectorWindow()
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
                    process = "Сформировал: Список инкассаторов и водителей 'B.I.G'",
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
                    worksheet.Cells[i + 2, 10].Value = collectorItem.power;
                    worksheet.Cells[i + 2, 11].Value = collectorItem.certificate;
                    worksheet.Cells[i + 2, 12].Value = collectorItem.token;
                    worksheet.Cells[i + 2, 13].Value = collectorItem.meaning;
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
                    // вызов метода для импорта данных из Excel в базу данных
                    ImportExcelToDatabase(openFileDialog.FileName);
                }
                Search(sender, e);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ImportExcelToDatabase(string filePath)
        {
            Excel.Application excel = null;
            Excel.Workbook workbook = null;
            try
            {
                // строка подключения к базе данных SQLite
                string connectionString = @"Data Source=B.I.G.db;Version=3;";

                // создание объекта подключения
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    // открытие подключения
                    connection.Open();

                    // создание объекта команды
                    SQLiteCommand command = new SQLiteCommand();

                    // привязка команды к объекту подключения
                    command.Connection = connection;

                    // создание объекта Excel
                    excel = new Excel.Application();

                    // открытие книги Excel по пути к файлу
                   workbook = excel.Workbooks.Open(filePath);

                    // выбор листа Excel для чтения данных
                    Excel._Worksheet worksheet = workbook.Sheets[1];

                    // получение диапазона ячеек для чтения данных
                    Excel.Range range = worksheet.UsedRange;

                    // определение количества колонок в таблице Excel
                    int columnCount = range.Columns.Count;

                    // создание SQL-запроса для вставки данных в таблицу cashCollectors
                    string query = "INSERT INTO cashCollectors (name, gun, automaton_serial, automaton, permission, meaning, certificate, token, power, fullname, profession, phone, image) " +
                                   "VALUES (@Name, @Gun, @Automaton_serial, @Automaton, @Permission, @Meaning, @Certificate, @Token, @Power, @Fullname, @Profession, @Phone, @Image)";

                    // привязка SQL-запроса к объекту команды
                    command.CommandText = query;

                    // создание параметров для SQL-запроса
                    command.Parameters.Add(new SQLiteParameter("@Name", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Gun", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Automaton_serial", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Automaton", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Permission", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Meaning", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Certificate", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Token", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Power", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Fullname", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Profession", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Phone", DbType.String));
                    command.Parameters.Add(new SQLiteParameter("@Image", DbType.Binary)); // Если это поле изображения

                    // проход по строкам диапазона
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        // создание массива для хранения значений ячеек строки
                        object[] rowValues = new object[columnCount];

                        // проход по ячейкам строки и заполнение массива rowValues
                        for (int col = 1; col <= columnCount; col++)
                        {
                            if (range.Cells[row, col].Value2 != null)
                            {
                                rowValues[col - 1] = (range.Cells[row, col] as Excel.Range).Value2.ToString();
                            }
                            else
                            {
                                rowValues[col - 1] = "";
                            }
                        }


                        // Здесь вы можете добавить проверку наличия записи в базе данных по полю @Name. Это можно сделать путем выполнения запроса SELECT перед выполнением INSERT.

                        // проверка наличия записи в базе данных по полю @Name
                        string selectQuery = "SELECT COUNT(*) FROM cashCollectors WHERE Name = @Name";
                        using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@Name", rowValues[0].ToString());
                            long existingRecords = (long)selectCommand.ExecuteScalar();
                            if (existingRecords > 0)
                            {
                                СashCollectorController cashCollectorController = new СashCollectorController();
                                cashCollectorController.Update2(rowValues[0].ToString(), rowValues[1].ToString(), rowValues[2].ToString(), rowValues[3].ToString(), rowValues[4].ToString(), rowValues[5].ToString(), rowValues[6].ToString(), rowValues[7].ToString(), rowValues[8].ToString(), rowValues[9].ToString(), rowValues[10].ToString(), rowValues[11].ToString());

                                continue;
                            }
                        }

                        // проверка, что все необходимые ячейки в строке не пустые
                        if (rowValues[0] != null && rowValues[1] != null && rowValues[2] != null && rowValues[3] != null && rowValues[4] != null && rowValues[5] != null && rowValues[6] != null && rowValues[7] != null && rowValues[8] != null && rowValues[9] != null && rowValues[10] != null && rowValues[11] != null && rowValues[12] != null)
                        {
                            command.Parameters["@Name"].Value = rowValues[0].ToString();
                            command.Parameters["@Gun"].Value = rowValues[1]?.ToString() ?? "";
                            command.Parameters["@Automaton_serial"].Value = rowValues[2]?.ToString() ?? "";
                            command.Parameters["@Automaton"].Value = rowValues[3]?.ToString() ?? "";
                            command.Parameters["@Permission"].Value = rowValues[4]?.ToString() ?? "";
                            command.Parameters["@Meaning"].Value = rowValues[5]?.ToString() ?? "";
                            command.Parameters["@Certificate"].Value = rowValues[6]?.ToString() ?? "";
                            command.Parameters["@Token"].Value = rowValues[7]?.ToString() ?? "";
                            command.Parameters["@Power"].Value = rowValues[8]?.ToString() ?? "";
                            command.Parameters["@Fullname"].Value = rowValues[9]?.ToString() ?? "";
                            command.Parameters["@Profession"].Value = rowValues[10]?.ToString() ?? "";
                            command.Parameters["@Phone"].Value = rowValues[11]?.ToString() ?? "";
                            string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "NoFoto.jpg");
                            byte[] imageBytes = File.ReadAllBytes(defaultImagePath);
                            command.Parameters["@Image"].Value = imageBytes;

                            // выполнение SQL-запроса
                            command.ExecuteNonQuery();
                        }
                    }

                    // закрытие книги Excel
                    workbook.Close(false);

                    // закрытие приложения Excel
                    excel.Quit();
                    MessageBox.Show("Данные добавлены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Документ имеет не верный формат");
            }
            finally
            {
                // блок finally будет выполнен в любом случае, даже если произойдет исключение
                // закрытие книги Excel
                //if (workbook != null)
                //{
                //    workbook.Close(false);
                //    Marshal.ReleaseComObject(workbook);
                //}

                // закрытие приложения Excel
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }
            }

        }
    }
}
