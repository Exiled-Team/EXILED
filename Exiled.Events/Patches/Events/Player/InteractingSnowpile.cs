using HarmonyLib;

namespace Exiled.Events.Patches.Events.Player
{
    [HarmonyPatch(typeof(Snowpile), nameof(Snowpile.ServerInteract))]
    public class InteractingSnowpile
    {

    }
}