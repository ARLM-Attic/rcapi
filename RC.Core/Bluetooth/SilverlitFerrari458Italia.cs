using RC.Core.Interfaces;
using RC.Core.Silverlit;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RC.Core.Bluetooth
{
    public sealed class Ferrari458Italia : SilverlitBluetoothDevice, ICar, ICarBlinkers, ICarBreakLights, ICarHeadlight
    {
        public Ferrari458Italia():base("Ferrari458Italia","{00001101-0000-1000-8000-00805F9B34FB}", 100)
        {
            this.Match = 0x40;
            this.Trimmer = 8;
            LightSequenceDelay = 1;
        }

        #region Properties
        public float Speed { get; set; }

        public float Steering { get; set; }

        public bool LeftBlinkerOn { get; set; }

        public bool RightBlinkerOn { get; set; }

        public bool BreakLightOn { get; set; }

        public bool HeadLightOn { get; set; }

        public byte[] LightSequence { get; set; }
        public int LightSequenceDelay { get; set; }

        public new byte Trimmer 
        {
            get { return base.Trimmer; }
            set { base.Trimmer=value; }
        }
#endregion



        int lightSequenceCounter = 0;
        int lightSequenceDelayCounter = 0;
        protected override byte[] GetBytesToSend()
        {

            Lights = 0;
            if (HeadLightOn)
            {
                Lights += (byte)LightEnum.Head;
            }
            if (BreakLightOn)
            {
                Lights += (byte)LightEnum.Break;
            }
            if (LeftBlinkerOn)
            {
                Lights += (byte)LightEnum.LeftBlinker;
            }
            if (RightBlinkerOn)
            {
                Lights += (byte)LightEnum.RightBlinker;
            }


            if (LightSequence != null && LightSequence.Length>0)
            {
                if (lightSequenceDelayCounter++ >= LightSequenceDelay)
                {
                    lightSequenceDelayCounter = 0;
                    if (lightSequenceCounter >= LightSequence.Length)
                        lightSequenceCounter = 0;
                    Lights |= LightSequence[lightSequenceCounter++];
                }
            }
           

            Pitch = (byte)Math.Floor(((Speed * -1) * 127) + 127);
            Yaw = (byte)Math.Floor(((Steering*-1) * 127) + 127);

            byte trimmerLightByte = (byte)(Trimmer | Lights << 4);
            byte[] bytesProtocol = new byte[] { trimmerLightByte, Yaw, Pitch, Rotor, Match };
            string protocolString = "r";
            for (int i = 0; i < bytesProtocol.Length; i++)
            {
                int index = (bytesProtocol.Length - 1) - i;
                protocolString += GetHexString(bytesProtocol[index]);
            }
            Debug.WriteLine(protocolString);
            List<byte> bytelist = new List<byte>();
            foreach (var c in protocolString)
            {
                bytelist.Add((byte)c);
            }
            return bytelist.ToArray();
        }
    }
}
