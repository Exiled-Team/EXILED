// -----------------------------------------------------------------------
// <copyright file="ChangingCamera.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles.Subroutines;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp079CurrentCameraSync.ServerProcessCmd(NetworkReader)" />.
    /// Adds the <see cref="Scp079.ChangingCamera" /> event.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.ChangingCamera))]
    [HarmonyPatch(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync.ServerProcessCmd))]
    internal static class ChangingCamera
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloca_S) + offset;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingCameraEventArgs));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp079Role>), nameof(StandardSubroutine<Scp079Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this._switchTarget
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync._switchTarget))),

                    // num (cost)
                    new(OpCodes.Ldloc_0),

                    // ChangingCameraEventArgs ev = new(Player, Scp079Camera, float)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingCameraEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnChangingCamera(ev)
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnChangingCamera))),

                    // if (ev.IsAllowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // num = ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.AuxiliaryPowerCost))),
                    new(OpCodes.Stloc_0),

                    // this._switchTarget = ev.Camera.Base
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.Camera))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Camera), nameof(API.Features.Camera.Base))),
                    new(OpCodes.Stfld, Field(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync._switchTarget))),
                });

            // return as the same way than NW does
            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;
            newInstructions[index].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}