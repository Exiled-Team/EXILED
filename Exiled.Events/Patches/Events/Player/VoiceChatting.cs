// -----------------------------------------------------------------------
// <copyright file="VoiceChatting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Radio.UserCode_CmdSyncVoiceChatStatus(bool)" />.
    ///     Adds the <see cref="Handlers.Player.VoiceChatting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Radio), nameof(Radio.UserCode_CmdSyncVoiceChatStatus))]
    internal static class VoiceChatting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Radio), nameof(Radio._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Dup),
                    new(OpCodes.Ldfld, Field(typeof(Radio), nameof(Radio._dissonanceSetup))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(VoiceChattingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnVoiceChatting))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceChattingEventArgs), nameof(VoiceChattingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}