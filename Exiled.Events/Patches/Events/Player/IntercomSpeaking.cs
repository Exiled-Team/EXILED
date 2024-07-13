// -----------------------------------------------------------------------
// <copyright file="IntercomSpeaking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.Voice;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Intercom.Update" />.
    /// Adds the <see cref="Handlers.Player.IntercomSpeaking" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.IntercomSpeaking))]
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.Update))]
    internal static class IntercomSpeaking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(x => x.StoresField(Field(typeof(Intercom), nameof(Intercom._curSpeaker)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this._curSpeaker)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Intercom), nameof(Intercom._curSpeaker))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // IntercomSpeakingEventArgs ev = new(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(IntercomSpeakingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnIntercomSpeaking(IntercomSpeakingEventArgs)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnIntercomSpeaking))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(IntercomSpeakingEventArgs), nameof(IntercomSpeakingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}