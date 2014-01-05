using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace RC.Core.Interfaces
{
    public interface ICarBlinkers
    {
        bool LeftBlinkerOn { get; set; }
        bool RightBlinkerOn { get; set; }
    }
}
