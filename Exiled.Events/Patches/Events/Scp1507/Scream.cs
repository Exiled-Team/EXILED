// -----------------------------------------------------------------------
// <copyright file="Scream.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1507
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1507;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp1507;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1507VocalizeAbility.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Scp1507.Screaming"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.Screaming))]
    [HarmonyPatch(typeof(Scp1507VocalizeAbility), nameof(Scp1507VocalizeAbility.ServerProcessCmd))]
    public class Scream
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldarg_0);

            Label retLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1507VocalizeAbility), nameof(Scp1507VocalizeAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ScreamingEventArgs ev = new(Player, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ScreamingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp1507.OnScreaming(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.OnScreaming))),

                    // if (!ev.IsAllowed)
                    //    goto retLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ScreamingEventArgs), nameof(ScreamingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}