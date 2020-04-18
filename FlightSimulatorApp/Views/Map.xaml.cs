using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.Views
{
    public partial class Map
    {

        // CTOR
        public Map()
        {
            InitializeComponent();

            myMap.Focus();
            //Set the map mode to Aerial with labels
            myMap.Mode = new AerialMode(true);

            // creating the pushpin of the map
            myPushpin.Template = (System.Windows.Controls.ControlTemplate)FindResource("PushpinControlTemplate");
        }
    }
}