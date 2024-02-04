// -----------------------------------------------------------------------
// <copyright file="TryUseBody.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.Ragdolls;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp3114Disguise.OnProgressSet()" />.
    /// Adds the <see cref="Handlers.Scp3114.TryUseBody" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.TryUseBody))]
    [HarmonyPatch(typeof(Scp3114Disguise), nameof(Scp3114Disguise.ServerValidateBegin))]
    internal class TryUseBody
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Disguise), nameof(Scp3114Disguise.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // Ragdoll.Get(this.CurRagdoll);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Disguise), nameof(Scp3114Disguise.CurRagdoll))),
                new(OpCodes.Call, Method(typeof(API.Features.Ragdoll), nameof(API.Features.Ragdoll.Get), new[] { typeof(BasicRagdoll) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // TryUseBodyEventArgs ev = new TryUseBodyEventArgs(Player, Ragdoll, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TryUseBodyEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp3114.OnTryUseBody(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnTryUseBody))),

                // if (!ev.IsAllowed)
                //      return (sbyte) Scp3114HudTranslation.RagdollErrorPreviouslyUsed;
                // else
                //      continue;
                new(OpCodes.Callvirt, PropertyGetter(typeof(TryUseBodyEventArgs), nameof(TryUseBodyEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Ldc_I4_S, (sbyte)Scp3114HudTranslation.RagdollErrorPreviouslyUsed),
                new(OpCodes.Ret),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
