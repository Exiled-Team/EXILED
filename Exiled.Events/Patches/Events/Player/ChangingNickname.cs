// -----------------------------------------------------------------------
// <copyright file="ChangingNickname.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="NicknameSync.set_Network_displayName"/> to add the <see cref="Handlers.Player.ChangingNickname"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.Network_displayName), MethodType.Setter)]
    internal static class ChangingNickname
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingNicknameEventArgs));

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Brtrue) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player player = Player.Get(this._hub);
                // if (player == null)
                //     return;
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(NicknameSync), nameof(NicknameSync._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, ret),
                new(OpCodes.Ldloc_S, player.LocalIndex),

                // value;
                new(OpCodes.Ldarg_1),

                // var ev = new ChangingNicknameEventArgs(Player.Get(this._hub, value)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingNicknameEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnChangingNickname(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingNickname))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingNicknameEventArgs), nameof(ChangingNicknameEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingNicknameEventArgs), nameof(ChangingNicknameEventArgs.NewName))),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}