using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WindowsStoreSample
{
    public sealed partial class VirtualJoystick : UserControl
    {

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(VirtualJoystick), null);

        /// <summary>
        /// Current distance (from 0 to 100)
        /// </summary>
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(VirtualJoystick), null);

        /// <summary>
        /// Delta angle to raise event StickMove
        /// </summary>
        public static readonly DependencyProperty AngleStepProperty = DependencyProperty.Register("AngleStep", typeof(double), typeof(VirtualJoystick), new PropertyMetadata(1.0));

        /// <summary>
        /// Delta distance to raise event StickMove
        /// </summary>
        public static readonly DependencyProperty DistanceStepProperty = DependencyProperty.Register("DistanceStep", typeof(double), typeof(VirtualJoystick), new PropertyMetadata(1.0));

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public double Angle
        {
            get { return Convert.ToDouble(GetValue(AngleProperty)); }
            private set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Current distanse (from 0 to 100)
        /// </summary>
        public double Distance
        {
            get { return Convert.ToDouble(GetValue(DistanceProperty)); }
            private set { SetValue(DistanceProperty, value); }
        }

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public double AngleStep
        {
            get { return Convert.ToDouble(GetValue(AngleStepProperty)); }
            set
            {
                if (value < 1) value = 1; else if (value > 90) value = 90;
                SetValue(AngleStepProperty, Math.Round(value));
            }
        }

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public double DistanceStep
        {
            get { return Convert.ToDouble(GetValue(DistanceStepProperty)); }
            set
            {
                if (value < 1) value = 1; else if (value > 50) value = 50;
                SetValue(DistanceStepProperty, value);
            }
        }

        /// <summary>
        /// Stick captured event
        /// </summary>
        public event EventHandler StickCaptured;

        /// <summary>
        /// Stick moved event
        /// </summary>
        public event EventHandler StickMove;

        /// <summary>
        /// Stick released event
        /// </summary>
        public event EventHandler StickReleased;

        private Point _startPos;
        private double _prevAngle, _prevDistance;

        public VirtualJoystick()
        {
            InitializeComponent();
            this.Loaded += VirtualJoystick_Loaded;

        }

        void VirtualJoystick_Loaded(object sender, RoutedEventArgs e)
        {
            //((FrameworkElement)Parent).PointerPressed += VirtualJoystick_PointerPressed;
            this.PointerPressed += VirtualJoystick_PointerPressed;
            ((FrameworkElement)Parent).PointerReleased += VirtualJoystick_PointerReleased;
            ((FrameworkElement)Parent).PointerMoved += VirtualJoystick_PointerMoved;
            ((FrameworkElement)Parent).PointerExited += VirtualJoystick_PointerReleased;
        }

        
        
        void VirtualJoystick_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (touch)
            {
                moveKnob(e.GetCurrentPoint(Base).Position);
            }
        }

        void VirtualJoystick_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            centerKnob.Begin();
            touch = false;
        }
        bool touch = false;
        void VirtualJoystick_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
                touch = true;
                _startPos = e.GetCurrentPoint(Base).Position;
                _prevAngle = _prevDistance = 0;
                if (StickCaptured != null) StickCaptured(this, new EventArgs());           
        }

   

        //void Knob_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        //{
        //    moveKnob(e.GetPosition(Base));
        //}

        //void Knob_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        //{
        //    //_startPos = base.TransformToVisual(e.;
        //    _prevAngle = _prevDistance = 0;
        //   // Knob.CaptureMouse();
        //    if (StickCaptured != null) StickCaptured(this, new EventArgs());           
        //}



        private void moveKnob(Point newPos)
        {
            Point p = new Point(newPos.X - _startPos.X, newPos.Y - _startPos.Y);

            double alpha = Math.Atan2(p.Y, p.X);

            double angle = alpha * 180 / Math.PI;
            if (angle > 0) angle += 90;
            else
            {
                angle = 270 + (180 + angle);
                if (angle >= 360) angle -= 360;
            }
            Angle = angle;

            double distance = Math.Min(Math.Round(Math.Sqrt(p.X * p.X + p.Y * p.Y) / 150 * 100), 150);
            knobPosition.X = distance * Math.Cos(alpha);
            knobPosition.Y = distance * Math.Sin(alpha);

            Distance = distance / 150 * 100;

            if (StickMove != null && (Math.Abs(_prevAngle - Angle) > AngleStep || Math.Abs(_prevDistance - Distance) > DistanceStep))
            {
                StickMove(this, new EventArgs());
                _prevAngle = Angle;
                _prevDistance = Distance;
            }
        }

        private void centerKnob_Completed(object sender, object e)
        {
            Angle = Distance = _prevAngle = _prevDistance = 0;
            if (StickMove != null) StickMove(this, new EventArgs());
            if (StickReleased != null) StickReleased(this, new EventArgs());
        }
        public Vector2 GetState()
        {
            return new Vector2() { Y = ((float)(Distance * Math.Sin((90 + Angle) / 180 * Math.PI)) / 100), X = ((float)(Distance * Math.Cos((90 + Angle) / 180 * Math.PI)) / 100) };
        }
    }

    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
