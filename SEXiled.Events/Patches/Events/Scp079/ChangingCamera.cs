// -----------------------------------------------------------------------
// <copyright file="ChangingCamera.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.UserCode_CmdSwitchCamera(ushort, bool)"/>.
    /// Adds the <see cref="ChangingCamera"/> event.
    /// </summary>
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
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // camera
                new CodeInstruction(OpCodes.Ldloc_0),

                // num (auxiliary power cost)
                new CodeInstruction(OpCodes.Ldloc_1),

                // !(num > this.curMana) --> num <= this.curMana
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript._curMana))),
                new CodeInstruction(OpCodes.Cgt),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),

                // var ev = new ChangingCameraEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingCameraEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, changingCameraEv.LocalIndex),

                // Handlers.Scp079.OnChangingCamera(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnChangingCamera))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),

                // num = ev.AuxiliaryPowerCost
                new CodeInstruction(OpCodes.Ldloc_S, changingCameraEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.AuxiliaryPowerCost))),
                new CodeInstruction(OpCodes.Stloc_1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
