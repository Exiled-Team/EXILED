using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exiled.Events.Handlers
{
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using static Exiled.Events.Events;

    /// <summary>
    /// SCP-049 related events.
    /// </summary>
    public class Scp049
    {
        /// <summary>
        /// Invoked before a player is infected.
        /// </summary>
        public static event CustomEventHandler<InfectPlayerArgs> InfectPlayer;

        /// <summary>
        /// Invoked before a player is infected.
        /// </summary>
        /// <param name="ev">The <see cref="InfectPlayerArgs"/> instance.</param>
        public static void OnInfectPlayer(InfectPlayerArgs ev) => InfectPlayer.InvokeSafely(ev);
    }
}
