using Exiled.Events.EventArgs.Interfaces;
using Interactables.Interobjects.DoorUtils;

namespace Exiled.Events.EventArgs.Map
{
    public class DoorDestroyingEventArgs : IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoorDestroyingEventArgs" /> class.
        /// </summary>
        /// <param name="doorVariant">
        ///     <inheritdoc cref="DoorVariant" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DoorDestroyingEventArgs(DoorVariant doorVariant, bool isAllowed = true)
        {
            Door = API.Features.Doors.Door.Get(doorVariant);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the door can be destroyed
        /// </summary>
        public bool IsAllowed { get; set; }
        /// <summary>
        ///     Gets a value indicating the door that will be destroyed
        /// </summary>
        public API.Features.Doors.Door Door { get; }
    }
}
