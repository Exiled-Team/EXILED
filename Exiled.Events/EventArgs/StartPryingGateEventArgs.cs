using Exiled.API.Features;

namespace Exiled.Events.EventArgs
{
    using System;

    /// <summary>
    /// Contains all information before SCP-096 starts prying a gate open.
    /// </summary>
    public class StartPryingGateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartPryingGateEventArgs"/> class.
        /// </summary>
        /// <param name="scp096">The Scp096 who is triggering the event.</param>
        /// <param name="gate">The gate to be pried open.</param>
        public StartPryingGateEventArgs(Player scp096, Door gate)
        {
            Player = scp096;
            Gate = gate;
        }

        /// <summary>
        /// Gets the <see cref="Player"/> object of the SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="Door"/> to be pried open.
        /// </summary>
        public Door Gate { get; }

        /// <summary>
        /// Gets or Sets a value indicating whether or not they should be allowed to pry the gate open.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
