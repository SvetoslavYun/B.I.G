using B.I.G.Controller;
using B.I.G.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace B.I.G.View
{
    /// <summary>
    /// Логика взаимодействия для EditJournal.xaml
    /// </summary>
    public partial class EditJournal : Window
    {   private int Id;
        private int Id2;
        private DateTime Date;
        private string Route2;
        private string Profession;
        public static cashCollector CashCollector;
        ObservableCollection<cashCollector> CashCollectors;
        private СashCollectorController сashCollectorController;

        public static journalCollector journalCollector;
        ObservableCollection<journalCollector> JournalCollectors;
        private JournalCollectorController journalCollectorController;
        public EditJournal(int id, string route2, DateTime data, string profession)
        {
            CashCollectors = new ObservableCollection<cashCollector>();
            сashCollectorController = new СashCollectorController();

            JournalCollectors = new ObservableCollection<journalCollector>();
            journalCollectorController = new JournalCollectorController();
            InitializeComponent();
            Id = id;
            Date = data;
            Route2 = route2;
            Profession = profession;
            dGridCollector.DataContext = CashCollectors;
            Name.TextChanged += Search;
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            try
            {

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

        private void dGridCollector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedCollector = dGridCollector.SelectedItem as cashCollector;
                if (selectedCollector != null)
                {
                    сashCollectorController.SearchFoto(selectedCollector.id);
                    // Создаем новый BitmapImage
                    BitmapImage imageSource = new BitmapImage();

                    // Конвертируем массив байтов в поток и загружаем его в BitmapImage
                    using (MemoryStream stream = new MemoryStream(Add_СashCollector.image_bytes))
                    {
                        imageSource.BeginInit();
                        imageSource.StreamSource = stream;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();
                    }

                    // Присваиваем BitmapImage свойству Source вашего Image
                    imgBox.Source = imageSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool isDataGridClick = false;

        private void dGridCollector_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDataGridClick = true;

            var selectedCollector = dGridCollector.SelectedItem as cashCollector;
            if (selectedCollector != null)
            {
                Name.Text = selectedCollector.name;
                Id2 = selectedCollector.id;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDataGridClick)
            {
                // Создаем окно сообщения с вопросом пользователю
                MessageBoxResult result = MessageBox.Show($"Заменить на {Name.Text}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Проверяем выбор пользователя
                if (result == MessageBoxResult.Yes)
                {
                    // Если пользователь выбрал "Да", обновляем данные
                    journalCollectorController.Update(Id2, Id, Route2, Date, Profession);
                    journalCollectorController.UpdateResponsibilities2(Date);
                    Close();
                }
                // Если пользователь выбрал "Нет", ничего не делаем
            }
        }

    }
}
