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

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106MovementModule), nameof(Scp106MovementModule.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Door), nameof(Door.Get), new[] { typeof(DoorVariant) })),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PassingDoorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    new(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnPassingDoor))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(PassingDoorEventArgs), nameof(PassingDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[index].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}