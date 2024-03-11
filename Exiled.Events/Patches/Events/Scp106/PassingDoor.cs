// -----------------------------------------------------------------------
// <copyright file="PassingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.Doors;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;
    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    [EventPatch(typeof(Handlers.Scp106), nameof(Handlers.Scp106.PassingDoor))]
    [HarmonyPatch(typeof(Scp106MovementModule), nameof(Scp106MovementModule.GetSlowdownFromCollider))]
    internal class PassingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_0);

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Hub);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106MovementModule), nameof(Scp106MovementModule.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Door.Get(comp);
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Door), nameof(Door.Get), new[] { typeof(DoorVariant) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PassingDoorEventArgs ev = new(Player, Door, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PassingDoorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Handlers.Scp106.OnPassingDoor(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnPassingDoor))),

                    // if (ev.IsAllowed)
                    //    goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PassingDoorEventArgs), nameof(PassingDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // return 0;
                    new(OpCodes.Ldc_R4, 0.0f),
                    new(OpCodes.Ret),
                });

            newInstructions[index].labels.Add(continueLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}