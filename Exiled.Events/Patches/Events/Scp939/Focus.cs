// -----------------------------------------------------------------------
// <copyright file="Focus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using Mirror;

    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp939FocusKeySync.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.ChangingFocus" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.ChangingFocus))]
    [HarmonyPatch(typeof(Scp939FocusKeySync), nameof(Scp939FocusKeySync.ServerProcessCmd))]
    internal static class Focus
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder state = generator.DeclareLocal(typeof(bool));

            Label ret = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(i => i.operand == (object)PropertySetter(typeof(Scp939FocusKeySync), nameof(Scp939FocusKeySync.FocusKeyHeld))) + offset;

            // Remove argument for this.FocusKeyHeld(reader.ReadBool()) to overwrite them in InsertRange
            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(index, new[]
            {
                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp939FocusKeySync), nameof(Scp939FocusKeySync.Owner))),

                // reader.ReadBool()
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(NetworkReaderExtensions), nameof(NetworkReaderExtensions.ReadBool))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, state.LocalIndex),

                // true
                new(OpCodes.Ldc_I4_1),

                // ChangingFocusEventArgs ev = new(referenceHub, state, isAllowed);
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingFocusEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp939.OnChangingFocus(ev);
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnChangingFocus))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingFocusEventArgs), nameof(ChangingFocusEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),

                // this
                // ev.State
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, state.LocalIndex),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}