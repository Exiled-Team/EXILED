// -----------------------------------------------------------------------
// <copyright file="HazardListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using PlayerRoles.PlayableScps.Scp939;

#pragma warning disable SA1313

    using Exiled.API.Features.Hazards;
    using HarmonyLib;
    using Hazards;

    /// <summary>
    /// Patch for adding hazards.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.Start))]
    internal class HazardListAdd
    {
        private static void Postfix(EnvironmentalHazard __instance)
        {
            if (__instance is Hazards.TemporaryHazard thazard)
            {
                if (thazard.IsActive)
                {
                    _ = thazard switch
                    {
                        TantrumEnvironmentalHazard tantrumEnvironmentalHazard => new TantrumHazard(tantrumEnvironmentalHazard),
                        Scp939AmnesticCloudInstance scp939AmnesticCloudInstance => new AmnesticCloudHazard(scp939AmnesticCloudInstance),
                        _ => new Exiled.API.Features.Hazards.TemporaryHazard(thazard)
                    };

                    return;
                }

                _ = new Hazard(thazard);
                return;
            }

            _ = __instance switch
            {
                SinkholeEnvironmentalHazard sinkholeEnvironmentalHazard => new SinkholeHazard(sinkholeEnvironmentalHazard),
                _ => new Hazard(__instance)
            };
        }
    }
}