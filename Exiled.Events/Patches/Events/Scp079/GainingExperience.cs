// -----------------------------------------------------------------------
// <copyright file="GainingExperience.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp079TierManager.ServerGrantExperience(int, Scp079HudTranslation, RoleTypeId)" />.
    ///     Adds the <see cref="Scp079.GainingExperience" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079TierManager), nameof(Scp079TierManager.ServerGrantExperience))]
    internal static class GainingExperience
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(GainingExperienceEventArgs));

            Label returnLabel = generator.DefineLabel();

            // GainingExperienceEventArgs ev = new(Player.Get(this.gameObject), reason, amount, true)
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // reason
                    new(OpCodes.Ldarg_2),

                    // amount
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // GainingExperienceEventArgs ev = new(Player, Scp079HudTranslation, int, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GainingExperienceEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnGainingExperience(ev);
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnGainingExperience))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // reason = ev.GainType
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.GainType))),
                    new(OpCodes.Starg_S, 2),

                    // amount = ev.Amount
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.Amount))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}