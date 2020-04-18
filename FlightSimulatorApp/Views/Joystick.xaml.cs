using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using FlightSimulatorApp.ViewModel;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlightSimulatorApp.Views
{
    public partial class Joystick : UserControl
    {
        // fields
        Storyboard _storyboard;
        private bool _isPress = false;

        // CTOR
        public Joystick()
        {
            InitializeComponent();
            _storyboard = Knob.FindResource("CenterKnob") as Storyboard;
        }

        public double Xval
        {
            get
            {
                return (double)GetValue(XvalProperty);
            }
            set
            {
                SetValue(XvalProperty, value);
            }
        }

        public static readonly DependencyProperty XvalProperty =
   DependencyProperty.Register("Xval", typeof(double), typeof(Joystick), new
      PropertyMetadata(0.0));

        public double Yval
        {
            get
            {
                return (double)GetValue(YvalProperty);
            }
            set
            {
                SetValue(YvalProperty, value);
            }
        }

        public static readonly DependencyProperty YvalProperty =
DependencyProperty.Register("Yval", typeof(double), typeof(Joystick), new
PropertyMetadata(0.0));

        private void Knob_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPress == true)
            {
                double distance;
                Point p = e.GetPosition(Base);
                double px = p.X - 170;
                double py = p.Y - 170;
                distance = Math.Sqrt(Math.Pow(px, 2) + Math.Pow(py, 2));
                distance = Math.Sqrt(Math.Pow(px, 2) + Math.Pow(py, 2));

                while (distance > 170)
                {
                    px = px - (px / 100);
                    py = py - (py / 100);
                    distance = Math.Sqrt(Math.Pow(px, 2) + Math.Pow(py, 2));
                }

                knobPosition.X = px;
                knobPosition.Y = py;

                double x = knobPosition.X;
                x = x * 100 / 170;
                int y = (int)x;
                x = (double)y;
                x = x / 100;
                //elevator=x
                this.Xval = x;
                x = knobPosition.Y;
                x = x * 100 / -170 ;
                y = (int)x;
                x = (double)y;
                x = x / 100;
                //rudder=y
                this.Yval = x;
            }
        }

        private void Knob_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPress = false;
            _storyboard.Begin();
        }

        private void Knob_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isPress = true;
        }
        private void centerKnob_Completed(object sender, EventArgs e)
        {
            knobPosition.X = 0;
            knobPosition.Y = 0;
            Xval = 0;
            Yval = 0;
            _storyboard.Stop();
        }

        private void Knob_MouseLeave(object sender, MouseEventArgs e)
        {
            _isPress = false;
            _storyboard.Begin();
        }
    }
}