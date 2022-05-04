namespace Exiled.API.Extensions
{
    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="ElevatorType"/>.
    /// </summary>
    public static class ElevatorExtensions
    {
        /// <summary>
        /// Checks if a <see cref="ElevatorType">elevator type</see> is a gate.
        /// </summary>
        /// <param name="elevator">The elevator to be checked.</param>
        /// <returns>Returns whether the <see cref="ElevatorType"/> is a gate or not.</returns>
        public static bool IsGate(this ElevatorType elevator)
        {
            return elevator is ElevatorType.GateA or ElevatorType.GateB;
        }

        /// <summary>
        /// Checks if a <see cref="ElevatorType">elevator type</see> is between Hcz and Lcz.
        /// </summary>
        /// <param name="elevator">The elevator to be checked.</param>
        /// <returns>Returns whether the <see cref="ElevatorType"/> is between Hcz and Lcz.</returns>
        public static bool IsBetweenHczAndLcz(this ElevatorType elevator)
        {
            return elevator is ElevatorType.LczA or ElevatorType.LczB;
        }
    }
}
