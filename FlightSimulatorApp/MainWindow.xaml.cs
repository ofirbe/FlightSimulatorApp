using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;
using FlightSimulatorApp.Views;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp
{
    /// <summary>
    /// The main window of the app is the conenction window.
    /// The client can either connect or to close the app.
    /// </summary>
    public partial class MainWindow : Window
    {
        // fields
        private IVM vm;

        // CTOR
        public MainWindow()
        {
            InitializeComponent();

            // set the ip and port from the textbox to the defult
            this.IPTextbox.Text = ConfigurationManager.AppSettings["defult_ip"].ToString();
            this.PortTextbox.Text = ConfigurationManager.AppSettings["defult_port"].ToString();
        }


        /*
         * The cconection button - responsible for connecting properly, and showing the right windows.
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // connection
            MyTelnetClient telnetCli = new MyTelnetClient();
            MyFlightSimulatorModel model = new MyFlightSimulatorModel(telnetCli);
            this.vm = new ConnectionViewModel(model);

            // bind the error light to the vm
            this.errorLight.DataContext = new DashboardVM(model);

            // get the ip and port from the textbox
            string ip = this.IPTextbox.Text;
            string port = this.PortTextbox.Text;

            try
            {
                // converting the port from string to int
                int intPort = int.Parse(port);

                // connecting to the server
                int isSucceed = this.vm.model.connect(ip, intPort);
                if (isSucceed == 1)
                {
                    this.vm.model.start();
                    this.vm.model.startQueue();
                    AppWindow app = new AppWindow(model, this, this.vm);
                    app.Show();
                    this.Hide();
                }
            }
            catch
            {
                this.vm.model.ConnectionError = "Blue";
            }
        }

        /*
         * The close button - responsible for closing the app properly.
         */
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}