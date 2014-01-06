using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Diagnostics;


namespace VirtualJoystick
{
    public partial class VirtualJoystick : UserControl
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

#if WINDOWS_PHONE
            Knob.ManipulationStarted += Knob_ManipulationStarted;
#else
            Knob.MouseLeftButtonDown += Knob_MouseLeftButtonDown;
            Knob.MouseMove += Knob_MouseMove;
            Knob.MouseLeftButtonUp += Knob_MouseLeftButtonUp;
#endif
        }

#if WINDOWS_PHONE
        void Knob_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _prevAngle = _prevDistance = 0;
            Touch.FrameReported += Touch_FrameReported;
            if (StickCaptured != null) StickCaptured(this, new EventArgs());
        }

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            TouchPoint touchPoint = e.GetPrimaryTouchPoint(Base);
            Point newPoint = new Point(touchPoint.Position.X - Base.ActualWidth / 2, touchPoint.Position.Y - Base.ActualHeight / 2);

            switch (touchPoint.Action)
            {
                case TouchAction.Down:
                    _startPos = newPoint;
                    break;

                case TouchAction.Move:
                    moveKnob(newPoint);
                    break;

                case TouchAction.Up:
                    Touch.FrameReported -= Touch_FrameReported;
                    centerKnob.Begin();
                    break;
            }
        }
#else
        void Knob_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPos = e.GetPosition(Base);
            _prevAngle = _prevDistance = 0;
            Knob.CaptureMouse();
            if (StickCaptured != null) StickCaptured(this, new EventArgs());
        }

        private void Knob_MouseMove(object sender, MouseEventArgs e)
        {
            moveKnob(e.GetPosition(Base));
        }

        private void Knob_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Knob.ReleaseMouseCapture();
            centerKnob.Begin();
        }
#endif

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

        private void centerKnob_Completed(object sender, EventArgs e)
        {
            Angle = Distance = _prevAngle = _prevDistance = 0;
            if (StickMove != null) StickMove(this, new EventArgs());
            if (StickReleased != null) StickReleased(this, new EventArgs());
        }

        public Microsoft.Xna.Framework.Input.GamePadState GetState()
        {
           

            //float x = Math.Sign(x1 - x2) * Math.Min(MaxSwipeDistance, Math.Abs(x1 - x2)) / MaxSwipeDistance;
            //float y = Math.Sign(y1 - y2) * Math.Min(MaxSwipeDistance, Math.Abs(y1 - y2)) / MaxSwipeDistance;


            float y = ((float)(Distance * Math.Sin((90 + Angle) / 180 * Math.PI))/100);
            float x = ((float)(Distance * Math.Cos((90 + Angle) / 180 * Math.PI))/100);
            Microsoft.Xna.Framework.Vector2 left = Microsoft.Xna.Framework.Vector2.Zero;
            Microsoft.Xna.Framework.Vector2 right = Microsoft.Xna.Framework.Vector2.Zero;

            Microsoft.Xna.Framework.Input.ButtonState dpadUp = Microsoft.Xna.Framework.Input.ButtonState.Released;
            Microsoft.Xna.Framework.Input.ButtonState dpadDown = Microsoft.Xna.Framework.Input.ButtonState.Released;
            Microsoft.Xna.Framework.Input.ButtonState dpadLeft = Microsoft.Xna.Framework.Input.ButtonState.Released;
            Microsoft.Xna.Framework.Input.ButtonState dpadRight = Microsoft.Xna.Framework.Input.ButtonState.Released;

            if (Math.Round(x) > 0.5f)
                dpadLeft = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            if (Math.Round(x) < -0.5f)
                dpadRight = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            if (Math.Round(y) < -0.5f)
                dpadDown = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            if (Math.Round(y) > 0.5f)
                dpadUp = Microsoft.Xna.Framework.Input.ButtonState.Pressed;

            Microsoft.Xna.Framework.Input.GamePadDPad dpad = new Microsoft.Xna.Framework.Input.GamePadDPad(dpadUp, dpadDown, dpadLeft, dpadRight);

            var buttons = new Microsoft.Xna.Framework.Input.GamePadButtons();
            var sticks = new Microsoft.Xna.Framework.Input.GamePadThumbSticks(new Microsoft.Xna.Framework.Vector2(x, y), Microsoft.Xna.Framework.Vector2.Zero);

            return new Microsoft.Xna.Framework.Input.GamePadState(sticks, new Microsoft.Xna.Framework.Input.GamePadTriggers(), buttons, dpad);
        }
    }
}
