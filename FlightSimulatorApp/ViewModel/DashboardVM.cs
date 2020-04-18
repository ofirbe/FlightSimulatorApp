using FlightSimulatorApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel
{
    public partial class DashboardVM : INotifyPropertyChanged, IVM
    {
        // fields
        private IFlightSimulatorModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        // CTOR
        public DashboardVM(IFlightSimulatorModel model)
        {
            this.model = model;

            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        /*
         *  The method gets string that representes property name, and letting the PropertyChanged list to know about the change.
         */
        public void NotifyPropertyChanged(string propname)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propname));
        }

        // Properties for the dashbord values
        public double VM_Heading
        {
            get
            {
                return this.model.Heading;
            }
        }
        public double VM_VerticalSpeed
        {
            get
            {
                return this.model.VerticalSpeed;
            }
        }
        public double VM_GroundSpeed
        {
            get
            {
                return this.model.GroundSpeed;
            }
        }
        public double VM_AirSpeed
        {
            get
            {
                return this.model.AirSpeed;
            }
        }
        public double VM_GPSAltitude
        {
            get
            {
                return this.model.GPSAltitude;
            }
        }
        public double VM_Roll
        {
            get
            {
                return this.model.Roll;
            }
        }
        public double VM_Pitch
        {
            get
            {
                return this.model.Pitch;
            }
        }
        public double VM_AltimeterAltitude
        {
            get
            {
                return this.model.AltimeterAltitude;
            }
        }

        // Properties for the errors light
        public string VM_MapEdgeError
        {
            get
            {
                return this.model.MapEdgeError;
            }

            set
            {
                this.model.MapEdgeError = value;
            }
        }
        public string VM_ConnectionError
        {
            get
            {
                return this.model.ConnectionError;
            }

            set
            {
                this.model.ConnectionError = value;
            }
        }
        public string VM_SendingMessageError
        {
            get
            {
                return this.model.SendingMessageError;
            }

            set
            {
                this.model.SendingMessageError = value;
            }
        }
        public string VM_TimeOutError
        {
            get
            {
                return this.model.TimeOutError;
            }

            set
            {
                this.model.TimeOutError = value;
            }
        }
        public string VM_InvalidValueError
        {
            get
            {
                return this.model.InvalidValueError;
            }

            set
            {
                this.model.InvalidValueError = value;
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
