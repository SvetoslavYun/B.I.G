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
namespace B.I.G

{
    public partial class UsersWindow : System.Windows.Window
    {
        public static bool flag;
        public static string names = MainWindow.LogS;
        ObservableCollection<user_account> User_Accounts;
        private User_accountController user_AccountController;
        public UsersWindow()
        {
            User_Accounts = new ObservableCollection<user_account>();
            user_AccountController = new User_accountController();
            InitializeComponent();
            dGridUser.DataContext = User_Accounts;
            FillData();
            Name.TextChanged += Search;

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
                            user_AccountController.Delete(Id);
                            FillData();
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
            //try

            //{
            //    if (!string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && !string.IsNullOrEmpty(Date2.Text))
            //    {
            //        var searchResults = log_Controller.Search_Name_Between_dates(Name.Text, Convert.ToDateTime(Date.Text), Convert.ToDateTime(Date2.Text));

            //        Logs.Clear();
            //        foreach (var result in searchResults)
            //        {
            //            Logs.Add(result);
            //        }
            //    }
            //    if (string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && !string.IsNullOrEmpty(Date2.Text))
            //    {
            //        var searchResults = log_Controller.Search_Between_dates(Convert.ToDateTime(Date.Text), Convert.ToDateTime(Date2.Text));

            //        Logs.Clear();
            //        foreach (var result in searchResults)
            //        {
            //            Logs.Add(result);
            //        }
            //    }

            //    if (string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
            //    {
            //        var searchResults = log_Controller.SearchDate(Convert.ToDateTime(Date.Text));

            //        Logs.Clear();
            //        foreach (var result in searchResults)
            //        {
            //            Logs.Add(result);
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
            //    {
            //        var searchResults = log_Controller.SearchNameDate(Name.Text, Convert.ToDateTime(Date.Text));

            //        Logs.Clear();
            //        foreach (var result in searchResults)
            //        {
            //            Logs.Add(result);
            //        }
            //    }
            //    if (string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
            //    {
            //        var searchResults = log_Controller.SearchDate(Convert.ToDateTime(Date.Text));

            //        Logs.Clear();
            //        foreach (var result in searchResults)
            //        {
            //            Logs.Add(result);
            //        }
            //    }
            //    if (string.IsNullOrEmpty(Name.Text) && string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
            //    {
            //        var allResults = log_Controller.GetAllLogs();

            //        Logs.Clear();
            //        foreach (var result in allResults)
            //        {
            //            Logs.Add(result);
            //        }
            //    }
            //    else if (!string.IsNullOrEmpty(Name.Text) && string.IsNullOrEmpty(Date.Text) && string.IsNullOrEmpty(Date2.Text))
            //    {
            //        var searchResults = log_Controller.SearchUsername(Name.Text);

            //        Logs.Clear();
            //        foreach (var result in searchResults)
            //        {
            //            Logs.Add(result);
            //        }
            //    }
            //}
            //catch (Exception h)
            //{
            //    MessageBox.Show(h.Message);
            //}
        }


        private void Button_export_to_excel(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    var excelPackage = new ExcelPackage();
            //    var worksheet = excelPackage.Workbook.Worksheets.Add("Logs");

            //    // Установка стилей для линий ячеек, ширины колонок и выравнивания
            //    using (var cells = worksheet.Cells[1, 1, dGridLog.Items.Count + 1, dGridLog.Columns.Count])
            //    {
            //        cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //        cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //        cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //        cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            //        cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Выравнивание по середине

            //    }

            //    // Добавление заголовков столбцов и порядковых номеров

            //    for (int i = 1; i <= dGridLog.Columns.Count; i++)
            //    {
            //        worksheet.Cells[1, i].Value = dGridLog.Columns[i - 1].Header;
            //    }

            //    // Добавление данных
            //    for (int i = 0; i < dGridLog.Items.Count; i++)
            //    {
            //        var logItem = (log)dGridLog.Items[i];

            //        worksheet.Cells[i + 2, 1].Value = logItem.id;
            //        worksheet.Cells[i + 2, 2].Value = logItem.username;
            //        worksheet.Cells[i + 2, 3].Value = logItem.process;
            //        worksheet.Cells[i + 2, 4].Value = logItem.date.ToString("dd.MM.yyyy HH:mm");

            //    }

            //    // Автоподгон ширины колонок
            //    worksheet.Cells.AutoFitColumns();

            //    worksheet.HeaderFooter.OddHeader.CenteredText = "&\"Arial\"&10&K000000 Журнал событий 'B.I.G'";

            //    worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:1"];

            //    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            //    {
            //        Filter = "Excel Files|*.xlsx",
            //        DefaultExt = ".xlsx",
            //        FileName = "Журнал событий 'B.I.G'"
            //    };

            //    if (saveFileDialog.ShowDialog() == true)
            //    {
            //        SaveExcelWithPageLayoutView(excelPackage, saveFileDialog.FileName);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Ошибка при экспорте в Excel: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
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

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            flag = true;
            Add_User add_User = new Add_User();
            add_User.Owner = this;
            add_User.ShowDialog();
            Search(sender, e);

        }
    }
}
