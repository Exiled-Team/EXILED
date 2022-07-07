using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.Events.Features;

namespace Exiled.Events.Interfaces
{
    // subject to change
    interface IEventPatch<T> where T : System.EventArgs
    {
        internal Event<T> Event { get; }
    }
}
