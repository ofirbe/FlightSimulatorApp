using FlightSimulatorApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel
{
    class ConnectionViewModel : IVM
    {
        private IFlightSimulatorModel model;

        // CTOR
        public ConnectionViewModel(IFlightSimulatorModel model)
        {
            this.model = model;
        }

        // Implementing the property
        IFlightSimulatorModel IVM.model
        {
            get
            {
                return this.model;
            }
        }
    }
}
