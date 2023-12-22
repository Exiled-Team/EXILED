using Exiled.Events.EventArgs.Scp559;
using Exiled.Events.Features;

namespace Exiled.Events.Handlers
{
    /// <summary>
    /// A SCP-559 related events.
    /// </summary>
    public static class Scp559
    {
        public static Event<SpawningEventArgs> Spawning { get; set; } = new();

        public static Event<InteractingScp559EventArgs> Interacting { get; set; } = new();

        public static void OnSpawning(SpawningEventArgs ev) => Spawning.InvokeSafely(ev);

        public static void OnInteracting(InteractingScp559EventArgs ev) => Interacting.InvokeSafely(ev);
    }
}