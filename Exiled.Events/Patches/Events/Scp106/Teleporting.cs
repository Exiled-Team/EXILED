// -----------------------------------------------------------------------
// <copyright file="Teleporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    

    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp106HuntersAtlasAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Teleporting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106HuntersAtlasAbility), nameof(Scp106HuntersAtlasAbility.ServerProcessCmd))]
    internal static class Teleporting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // The index offset.
            const int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + offset;

            // Declare TeleportingEventArgs, to be able to store its instance.
            LocalBuilder ev = generator.DeclareLocal(typeof(TeleportingEventArgs));

            // Get the count to find the previous index
            int oldCount = newInstructions.Count;

            // Define the return label for the last instruction.
            Label returnLabel = generator.DefineLabel();

            // TeleportingEventArgs ev = new TeleportingEventArgs(Player.Get(base.Owner), this.Position, true);
            //
            // Handlers.Scp106.OnTeleporting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // position = ev.Position;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp106Role>), nameof(ScpStandardSubroutine<Scp106Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // position
                    new(OpCodes.Ldloc_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // TeleportingEventArgs ev = new(Player, Vector3, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TeleportingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp106.OnTeleporting(ev)
                    new(OpCodes.Call, Method(typeof(Scp106), nameof(Scp106.OnTeleporting))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // position = ev.Position
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.Position))),
                    new(OpCodes.Stloc_0),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}