using HarmonyLib;
using MapGeneration.Distributors;

namespace Exiled.Events.Patches.Events.Map
{
    [HarmonyPatch(typeof(LockerChamber), nameof(LockerChamber.SpawnItem))]
    internal class SpawningItemInLocker
    {

    }
}