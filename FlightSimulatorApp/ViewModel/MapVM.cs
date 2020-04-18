using FlightSimulatorApp.Model;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel
{
    public partial class MapVM : INotifyPropertyChanged, IVM
    {
        private IFlightSimulatorModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        // CTOR
        public MapVM(IFlightSimulatorModel newModel)
        {
            this.model = newModel;

            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        /*
         * The method gets string that representes property name, and letting the PropertyChanged list to know about the change.
         */
        public void NotifyPropertyChanged(string propname)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propname));
        }

        // Impelemting the map property
        public Location VM_Location
        {
            get
            {
                return this.model.Location;
            }
        }

        IFlightSimulatorModel IVM.model
        {
            get
            {
                return this.model;
            }
        }
    }
}
