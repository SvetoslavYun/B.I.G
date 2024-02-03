using B.I.G.Controller;
using B.I.G.Model;
using Microsoft.Graph.Models.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace B.I.G
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string nameUserApp;
        private Log_Controller log_Controller;
        ObservableCollection<log> Logs;

        public App()
        {
            Logs = new ObservableCollection<log>();
            log_Controller = new Log_Controller();

            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            string Process = "Выход из системы";
            DateTime Date = DateTime.Now;
            string formattedDate = Date.ToString("dd.MM.yyyy HH:mm");
            string formattedDate2 = Date.ToString("dd.MM.yyyy");
            if (nameUserApp == null) { nameUserApp = "Неизвестный пользователь"; Process = "Попытка входа"; }
            var Log = new log()
            { 
                username = nameUserApp,
                process = Process,
                date = Convert.ToDateTime(formattedDate),
                date2 = Convert.ToDateTime(formattedDate2)
            };

            log_Controller.Insert(Log);
        }
    }

}

