using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model
{
    public interface IFlightSimulatorModel : INotifyPropertyChanged
    {
        // connection, disconnection to the server
        int connect(string ip, int port);
        void disconnect();
        void start();

        // Map properties
        Location Location { get; set; }

        // Joystick properties
        double Throttle { set; get; }
        double Aileron { set; get; }
        double Elevator { set; get; }
        double Rudder { set; get; }

        // The rest of the properties
        double Heading { set; get; }
        double VerticalSpeed { set; get; }
        double GroundSpeed { set; get; }
        double AirSpeed { set; get; }
        double GPSAltitude { set; get; }
        double Roll { set; get; }
        double Pitch { set; get; }
        double AltimeterAltitude { set; get; }

        // errors properties
        string MapEdgeError { set; get; }
        string ConnectionError { set; get; }
        string SendingMessageError { set; get; }
        string TimeOutError { set; get; }
        string InvalidValueError { set; get; }

        //Plane-Control func
        void setAileron(double aileron);
        void setThrottle(double throttle);
        void setElevator(double elevator);
        void setRudder(double rudder);
    }
}