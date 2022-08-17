namespace Exiled.Events.EventArgs.Interfaces
{
    /// <summary>
    /// Event args for all <see cref="global::EnvironmentalHazard"/> related events.
    /// </summary>
    public interface IHazardEvent : IExiledEvent
    {
        /// <summary>
        /// Gets the environmental hazard that the player is interacting with.
        /// </summary>
        public EnvironmentalHazard EnvironmentalHazard { get; }
    }
}
