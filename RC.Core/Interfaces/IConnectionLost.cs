using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC.Core.Silverlit
{
    public interface ILostConnection
    {
        event ConnectionLostEventHandler ConnectionLost;
    }
}
