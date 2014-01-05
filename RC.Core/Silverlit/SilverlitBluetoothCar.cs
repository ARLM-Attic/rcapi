using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace RC.Core.Silverlit
{
    public abstract class SilverlitBluetoothCar : SilverlitBluetoothDevice,ICar,ICarBlinkers,ICarBreakLights,ICarHeadlight
    {

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Speed <= 1 && this.Speed>=-1);
            Contract.Invariant(this.Steering <= 1 && this.Steering >= -1);
            Contract.Invariant(this.LightSequence==null || this.LightSequence.Length>0);
        }

        public SilverlitBluetoothCar()
        {
            LightSequence=new byte[0];
        }

      
        public byte[] LightSequence { get; set; }
        public int LightSequenceDelay { get; set; }

        public float Speed { get; set; }

        public float Steering { get; set; }

        public bool LeftBlinkerOn { get; set; }

        public bool RightBlinkerOn { get; set; }

        public bool BreakLightOn { get; set; }

        public bool HeadLightOn { get; set; }
    }
}
