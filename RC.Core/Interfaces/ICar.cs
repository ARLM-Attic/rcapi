using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace RC.Core.Interfaces
{
    public interface ICar
    {
        float Speed { get; set; }
        float Steering { get; set; }
    }
}
