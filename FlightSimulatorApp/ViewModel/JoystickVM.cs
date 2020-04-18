using FlightSimulatorApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel
{
    public partial class JoystickVM : INotifyPropertyChanged, IVM
    {
        // fields
        private IFlightSimulatorModel model;
        private double aileron;
        private double throttle;
        private double elevator;
        private double rudder;
        public event PropertyChangedEventHandler PropertyChanged;

        // CTOR
        public JoystickVM(IFlightSimulatorModel model)
        {
            this.model = model;

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

        // Implementing the joystick proprties
        public double VM_Throttle
        {
            get
            {
                return this.throttle;
            }
            set
            {
                if (this.throttle != value)
                {
                    this.throttle = value;
                    this.model.setThrottle(this.throttle);
                }
            }
        }
        public double VM_Aileron
        {
            get
            {
                return this.aileron;
            }
            set
            {
                if (this.aileron != value)
                {
                    this.aileron = value;
                    this.model.setAileron(this.aileron);
                }
            }
        }
        public double VM_Elevator
        {
            get
            {
                return this.elevator;
            }
            set
            {
                if (this.elevator != value)
                {
                    this.elevator = value;
                    this.model.setElevator(this.elevator);
                }
            }
        }
        public double VM_Rudder
        {
            get
            {
                return this.rudder;
            }
            set
            {
                if (this.rudder != value)
                {
                    this.rudder = value;
                    this.model.setRudder(this.rudder);
                }
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
