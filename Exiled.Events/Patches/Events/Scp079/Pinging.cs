// -----------------------------------------------------------------------
// <copyright file="Pinging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.EventArgs.Scp079;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Pinging;
    using PlayerRoles.PlayableScps.Subroutines;
    using RelativePositioning;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079PingAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="PingingEventArgs" /> event for  SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PingAbility), nameof(Scp079PingAbility.ServerProcessCmd))]
    internal static class Pinging
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(PingingEventArgs));

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Stfld) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Owner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),

                    // this._syncPos
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._syncPos))),

                    // this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._cost))),

                    // this._syncProcessorIndex
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._syncProcessorIndex))),

                    // this._syncNormal
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._syncNormal))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PingingEventArgs ev = new(ReferenceHub, RelativePosition, int, byte, Vector3, bool)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PingingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnPinging(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnPinging))),

                    // if (ev.IsAllowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // this._syncPos = new RelativePosition(ev.Position)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.Position))),
                    new(OpCodes.Newobj, DeclaredConstructor(typeof(RelativePosition), new[] { typeof(Vector3), })),
                    new(OpCodes.Stfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._syncPos))),

                    // this._syncProcessorIndex = ev.Type
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.Type))),
                    new(OpCodes.Stfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._syncProcessorIndex))),
                });

            // replace "base.AuxManager.CurrentAux -= (float)this._cost;"
            // with
            // "base.AuxManager.CurrentAux -= ev.AuxiliaryPowerCost;"
            offset = -1;
            index = newInstructions.FindLastIndex(x => x.operand == (object)Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._cost))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.AuxiliaryPowerCost))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}