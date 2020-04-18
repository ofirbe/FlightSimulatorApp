using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;
using System;
using System.Collections.Generic;
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

namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for AppWindow1.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        // fields
        private MainWindow _main;
        private IVM vm;

        public AppWindow(MyFlightSimulatorModel model, MainWindow main, IVM vm)
        {
            InitializeComponent();

            // initializing the main wondow fields and the vm field
            this._main = main;
            this.vm = vm;

            // creating the view model of the dashboard
            DashboardVM dashboardVM = new DashboardVM(model);
            myDashboard.DataContext = dashboardVM;

            // creating the view model of the map
            MapVM mapVM = new MapVM(model);
            myMap.DataContext = mapVM;

            // creating the view model of the joystick
            JoystickVM joystickVm = new JoystickVM(model);
            myControlPlane.DataContext = joystickVm;
            myControlPlane.myJoystick.DataContext = joystickVm;

            // creating the view model of the dashboard errors
            myDashboardErrors.DataContext = dashboardVM;
        }

        /*
         * The discconection button - responsible for disconnecting properly, and showing the right windows.
         */
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // disconnection by using the vm
            this.vm.model.disconnect();

            // reseting the errors light
            this.vm.model.ConnectionError = "";
            this.vm.model.MapEdgeError = "";
            this.vm.model.SendingMessageError = "";

            // closing the app window and showing the connection (main) window
            _main.Show();
            this.Close();
        }

        /*
         * The event is responsible to disconnect properly by using the vm in case of closing the app window without using the "close" button
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.vm.model.disconnect();
        }

        /*
         * The event is responsible to disconnect properly by using the vm in case of closing the app window without using the "close" button
         */
        private void Window_Closed(object sender, EventArgs e)
        {
            this.vm.model.disconnect();
        }
    }
}
