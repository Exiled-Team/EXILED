// -----------------------------------------------------------------------
// <copyright file="InteractingTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp079TeslaAbility.ServerProcessCmd(NetworkReader)" />.
    ///     Adds the <see cref="Scp079.InteractingTesla" /> event for SCP-079.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.InteractingTesla))]
    [HarmonyPatch(typeof(Scp079TeslaAbility), nameof(Scp079TeslaAbility.ServerProcessCmd))]
    internal static class InteractingTesla
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingTeslaEventArgs));

            int offset = 2;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;

            // InteractingTeslaEventArgs ev = new(Player.Get(base.Owner), teslaGate, (float)this._cost);
            //
            // Handlers.Map.OnInteractingTesla(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // aux = ev.AuxiliaryPowerCost
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp079Role>), nameof(StandardSubroutine<Scp079Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // teslaGate
                    new(OpCodes.Ldloc_1),

                    // (float)this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079TeslaAbility), nameof(Scp079TeslaAbility._cost))),
                    new(OpCodes.Conv_R4),

                    // InteractingTeslaEventArgs ev = new(Player, TeslaGate, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingTeslaEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Map.OnInteractingTesla(ev);
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnInteractingTesla))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingTeslaEventArgs), nameof(InteractingTeslaEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            // Replace "(float)this._cost" with "ev.AuxiliaryPowerCost"
            offset = -1;
            index = newInstructions.FindLastIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp079TeslaAbility), nameof(Scp079TeslaAbility._cost)))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingTeslaEventArgs), nameof(InteractingTeslaEventArgs.AuxiliaryPowerCost))),
               });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}