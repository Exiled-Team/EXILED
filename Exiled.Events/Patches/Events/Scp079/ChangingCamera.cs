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

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.UserCode_CmdSwitchCamera(ushort, bool)"/>.
    /// Adds the <see cref="Handlers.Scp079.ChangingCamera"/> event.
    ///     Patches <see cref="Scp079PlayerScript.UserCode_CmdSwitchCamera(ushort, bool)" />.
    ///     Adds the <see cref="ChangingCamera" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.ChangingCamera))]
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.UserCode_CmdSwitchCamera))]
    internal static class ChangingCamera
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for the first "ldloc.1".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            // Define the first label of the last "ret" and retrieve it.
            Label returnLabel = newInstructions[newInstructions.Count - 1].WithLabels(generator.DefineLabel()).labels[0];

            // Declare a local variable of the type "ChangingCameraEventArgs"
            LocalBuilder changingCameraEv = generator.DeclareLocal(typeof(ChangingCameraEventArgs));

            // var ev = new ChangingCameraEventArgs(Player.Get(this.gameObject), camera, num,  num <= this.curMana)
            //
            // Handlers.Scp079.OnChangingCamera(ev)
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // num = ev.AuxiliaryPowerCost
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(this.gameObject)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // camera
                new(OpCodes.Ldloc_0),

                // num (auxiliary power cost)
                new(OpCodes.Ldloc_1),

                // !(num > this.curMana) --> num <= this.curMana
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript._curMana))),
                new(OpCodes.Cgt),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),

                // var ev = new ChangingCameraEventArgs(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingCameraEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, changingCameraEv.LocalIndex),

                // Handlers.Scp079.OnChangingCamera(ev)
                new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnChangingCamera))),

                // if (!ev.IsAllowed)
                //   return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // num = ev.AuxiliaryPowerCost
                new(OpCodes.Ldloc_S, changingCameraEv.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.AuxiliaryPowerCost))),
                new(OpCodes.Stloc_1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
