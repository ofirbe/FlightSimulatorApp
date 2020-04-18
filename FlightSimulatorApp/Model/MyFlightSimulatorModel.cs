using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Input;
using FlightSimulatorApp.ViewModel;
using System.Diagnostics;

namespace FlightSimulatorApp.Model
{
    public partial class MyFlightSimulatorModel : IFlightSimulatorModel
    {
        // fields
        private ITelnetClient telnetClientHandler;
        private bool isConnected;
        private Queue<string> setCommandsQueue = new Queue<string>();
        private Mutex mutex = new Mutex();

        // Map
        private Location location = null;

        // Joystick
        private double throttle;
        private double aileron;
        private double elevator;
        private double rudder;

        // The rest
        private double heading;
        private double verticalSpeed;
        private double groundSpeed;
        private double airSpeed;
        private double gPSAltitude;
        private double roll;
        private double pitch;
        private double altimeterAltitude;
        private double readbackGarbage;
        public event PropertyChangedEventHandler PropertyChanged;

        // errors properties
        private string mapEdgeError;
        private string connectionError;
        private string sendingMessageError;
        private string timeOutError;
        private string invalidValueError;

        // CTOR
        public MyFlightSimulatorModel(ITelnetClient telnetClientHandler)
        {
            this.telnetClientHandler = telnetClientHandler;
            this.isConnected = false;
        }

        // watches fields - responsible for the duration time that each error light would be enabled
        private Stopwatch invalidValueWatch = new Stopwatch();
        private Stopwatch timeOutWatch = new Stopwatch();
        private Stopwatch mapEdgeWatch = new Stopwatch();

        /*
         * Impelemnting the properties by using the NotifyPropertyChanged method.
         */

        public double Heading
        {
            get
            {
                return this.heading;
            }
            set
            {
                this.heading = value;
                NotifyPropertyChanged("Heading");
            }
        }
        public double VerticalSpeed
        {
            get
            {
                return this.verticalSpeed;
            }
            set
            {
                this.verticalSpeed = value;
                NotifyPropertyChanged("VerticalSpeed");
            }
        }
        public double GroundSpeed
        {
            get
            {
                return this.groundSpeed;
            }
            set
            {
                this.groundSpeed = value;
                NotifyPropertyChanged("GroundSpeed");
            }
        }
        public double AirSpeed
        {
            get
            {
                return this.airSpeed;
            }
            set
            {
                this.airSpeed = value;
                NotifyPropertyChanged("AirSpeed");
            }
        }
        public double GPSAltitude
        {
            get
            {
                return this.gPSAltitude;
            }
            set
            {
                this.gPSAltitude = value;
                NotifyPropertyChanged("GPSAltitude");
            }
        }
        public double Roll
        {
            get
            {
                return this.roll;
            }
            set
            {
                this.roll = value;
                NotifyPropertyChanged("Roll");
            }
        }
        public double Pitch
        {
            get
            {
                return this.pitch;
            }
            set
            {
                this.pitch = value;
                NotifyPropertyChanged("Pitch");
            }
        }
        public double AltimeterAltitude
        {
            get
            {
                return this.altimeterAltitude;
            }
            set
            {
                this.altimeterAltitude = value;
                NotifyPropertyChanged("AltimeterAltitude");
            }
        }
        public double Throttle
        {
            get
            {
                return this.throttle;
            }
            set
            {
                this.throttle = value;
                NotifyPropertyChanged("Throttle");
            }
        }
        public double Aileron
        {
            get
            {
                return this.aileron;
            }
            set
            {
                this.aileron = value;
                NotifyPropertyChanged("Aileron");
            }
        }
        public double Elevator
        {
            get
            {
                return this.elevator;
            }
            set
            {
                this.elevator = value;
                NotifyPropertyChanged("Elevator");
            }
        }
        public double Rudder
        {
            get
            {
                return this.rudder;
            }
            set
            {
                this.rudder = value;
                NotifyPropertyChanged("Rudder");
            }
        }
        public Location Location
        {
            get
            {
                return location;
            }
            set
            {
                if (value != null)
                {
                    this.location = new Location(value);
                    NotifyPropertyChanged("Location");
                }
            }
        }

        // errors properties
        public string MapEdgeError
        {
            get
            {
                return this.mapEdgeError;
            }
            set
            {
                this.mapEdgeError = value;
                NotifyPropertyChanged("MapEdgeError");
            }
        }
        public string ConnectionError
        {
            get
            {
                return this.connectionError;
            }
            set
            {
                this.connectionError = value;
                NotifyPropertyChanged("connectionError");
            }
        }
        public string SendingMessageError
        {
            get
            {
                return this.sendingMessageError;
            }
            set
            {
                this.sendingMessageError = value;
                NotifyPropertyChanged("SendingMessageError");
            }
        }
        public string TimeOutError
        {
            get
            {
                return this.timeOutError;
            }
            set
            {
                if (this.timeOutError != value)
                {
                    this.timeOutError = value;
                    NotifyPropertyChanged("TimeOutError");
                }
            }
        }
        public string InvalidValueError
        {
            get
            {
                return this.invalidValueError;
            }
            set
            {
                this.invalidValueError = value;
                NotifyPropertyChanged("InvalidValueError");
            }
        }

        /*
         * The method gets string that representes property name, and letting the PropertyChanged list to know about the change.
         */
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        /*
         * The method gets string and check if it can be converted to double, by using double.Parse.
         * If yes - return true.
         * If no - turn on the "invalid value" error, and return false.
         */
        private bool tryToConvert(string str)
        {
            try
            {
                double.Parse(str);
                return true;
            }
            catch
            {
                this.InvalidValueError = "Orange";

                // starting the timer for the error light
                invalidValueWatch.Start();

                return false;
            }
        }

        /*
         * The method gets the values from the simulator, in different thread that runs as long as the server is connteced.
         */
        public void start()
        {
            new Thread(delegate ()
            {
                if (this.isConnected == true)
                {
                    this.ConnectionError = "";
                    this.MapEdgeError = "";
                    this.SendingMessageError = "";
                    this.InvalidValueError = "";
                    this.TimeOutError = "";
                }

                while (isConnected == true)
                {
                    string tmp = "";
                    try
                    {
                        // lock the thread
                        mutex.WaitOne();

                        // get the Heading
                        this.telnetClientHandler.write("get /instrumentation/heading-indicator/indicated-heading-deg\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            Heading = double.Parse(tmp);
                        }

                        // get the VerticalSpeed
                        this.telnetClientHandler.write("get /instrumentation/gps/indicated-vertical-speed\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            VerticalSpeed = double.Parse(tmp);
                        }
                        // get the GroundSpeed
                        this.telnetClientHandler.write("get /instrumentation/gps/indicated-ground-speed-kt\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            GroundSpeed = double.Parse(tmp);
                        }
                        // get the AirSpeed
                        this.telnetClientHandler.write("get /instrumentation/airspeed-indicator/indicated-speed-kt\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            AirSpeed = double.Parse(tmp);
                        }
                        // get the GPSAltitude
                        this.telnetClientHandler.write("get /instrumentation/gps/indicated-altitude-ft\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            GPSAltitude = double.Parse(tmp);
                        }
                        // get the Roll
                        this.telnetClientHandler.write("get /instrumentation/attitude-indicator/internal-roll-deg\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            Roll = double.Parse(tmp);
                        }
                        // get the Pitch
                        this.telnetClientHandler.write("get /instrumentation/attitude-indicator/internal-pitch-deg\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            Pitch = double.Parse(tmp);
                        }
                        // get the AltimeterAltitude
                        this.telnetClientHandler.write("get /instrumentation/altimeter/indicated-altitude-ft\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            AltimeterAltitude = double.Parse(tmp);
                        }

                        double longtitude = 0, latitude = 0;
                        bool isValidLoc = true;
                        Location backUp = null;

                        // get the longtitude
                        this.telnetClientHandler.write("get /position/longitude-deg\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            longtitude = double.Parse(tmp);
                        }
                        else
                            isValidLoc = false;

                        // get the latitude
                        this.telnetClientHandler.write("get /position/latitude-deg\n");
                        tmp = this.telnetClientHandler.read();
                        if (tryToConvert(tmp) == true)
                        {
                            latitude = double.Parse(tmp);
                        }
                        else
                            isValidLoc = false;

                        // unlock the thread
                        mutex.ReleaseMutex();

                        if (isValidLoc == true)
                        {
                            // back up for checking errors
                            backUp = this.Location;

                            Location = new Location(latitude, longtitude);
                        }
                        else
                            Location = null;

                        // check if the plane is at the edge of the map                    
                        if (Location != null && (this.Location.Latitude > 90 || this.Location.Latitude < -90 || this.Location.Longitude > 180 || this.Location.Longitude < -180))
                        {
                            // set the location of the map to the last position
                            if (backUp != null)
                                this.Location = backUp;
                            else
                                this.Location = new Location(34.873331, 32.006333);

                            // let user know about the error
                            this.MapEdgeError = "Red";

                            // starting the timer for the error light
                            this.mapEdgeWatch.Start();
                        }
                    }
                    catch (IOException)
                    {
                        if (this.telnetClientHandler.isConnected() == true)
                        {
                            // clearing the buffer by getting the value
                            tmp = this.telnetClientHandler.read();

                            // unlock the thread
                            mutex.ReleaseMutex();

                            // set the light on
                            this.TimeOutError = "Yellow";

                            // starting the timer for the error light
                            this.timeOutWatch.Start();
                        }
                        else
                        {
                            this.ConnectionError = "Blue";

                            // unlock the thread
                            mutex.ReleaseMutex();

                            // disconnect in case of error
                            this.disconnect();
                        }
                    }

                    // checking if the light is on over 5 seconds - if it is, reset it and turn off the light
                    if (this.invalidValueWatch.ElapsedMilliseconds > 5000)
                    {
                        this.invalidValueWatch.Reset();
                        this.InvalidValueError = "";
                    }
                    if (this.timeOutWatch.ElapsedMilliseconds > 5000)
                    {
                        this.timeOutWatch.Reset();
                        this.TimeOutError = "";
                    }
                    if (this.mapEdgeWatch.ElapsedMilliseconds > 5000)
                    {
                        this.mapEdgeWatch.Reset();
                        this.MapEdgeError = "";
                    }

                    Thread.Sleep(250);
                }
            }).Start();
        }

        /*
         * The method sets the values to the simulator, in different thread that runs as long as the server is connteced.
         */
        public void startQueue()
        {
            new Thread(delegate ()
            {
                while (isConnected == true)
                {
                    // execute the command in the command list
                    while (this.setCommandsQueue.Count != 0)
                    {
                        try
                        {
                            // lock the thread
                            mutex.WaitOne();

                            string cmd = this.setCommandsQueue.Dequeue();
                            this.telnetClientHandler.write(cmd);
                            this.readbackGarbage = double.Parse(this.telnetClientHandler.read());

                            // unlock the thread
                            mutex.ReleaseMutex();
                        }
                        catch
                        {
                            this.SendingMessageError = "Green";

                            // unlock the thread
                            mutex.ReleaseMutex();

                            // disconnect from the server
                            this.disconnect();
                        }
                    }
                    Thread.Sleep(250);
                }
            }).Start();
        }


        /*
         * The method connect to the server by using the telnet client
         */
        public int connect(string ip, int port)
        {
            int check = 0;
            try
            {
                check = this.telnetClientHandler.connect(ip, port);
                if (check == 1)
                    isConnected = true;
            }
            catch
            {
                this.ConnectionError = "Blue";

                // disconnect in case of error
                this.disconnect();
            }
            return check;
        }

        /*
         * The method disconnect to the server by using the telnet client
         */
        public void disconnect()
        {
            if (isConnected == true)
            {
                this.telnetClientHandler.disconnect();
                isConnected = false;
            }
        }

        /*
         * The method set the aileron by the value it gets and by using the telnet client, and if there is any kind of prolbem in the command sending, the method turn on the exact error light.
         */
        public void setAileron(double aileron)
        {
            if (this.isConnected == true)
            {
                this.setCommandsQueue.Enqueue("set " + "/controls/flight/aileron " + aileron + "\n");
            }
        }

        /*
         * The method set the throttle by the value it gets and by using the telnet client, and if there is any kind of prolbem in the command sending, the method turn on the exact error light.
         */
        public void setThrottle(double throttle)
        {
            if (this.isConnected == true)
            {
                this.setCommandsQueue.Enqueue("set " + " /controls/engines/current-engine/throttle " + throttle + "\n");
            }
        }

        /*
         * The method set the elivator by the value it gets and by using the telnet client, and if there is any kind of prolbem in the command sending, the method turn on the exact error light.
         */
        public void setElevator(double elevator)
        {
            if (this.isConnected == true)
            {
                this.setCommandsQueue.Enqueue("set " + "/controls/flight/elevator " + elevator + "\n");
            }
        }

        /*
         * The method set the rudder by the value it gets and by using the telnet client, and if there is any kind of prolbem in the command sending, the method turn on the exact error light.
         */
        public void setRudder(double rudder)
        {
            if (this.isConnected == true)
            {
                this.setCommandsQueue.Enqueue("set " + "/controls/flight/rudder " + rudder + "\n");
            }
        }
    }
}