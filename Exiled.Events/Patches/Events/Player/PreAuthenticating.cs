// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using LiteNetLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)" />.
    /// Adds the <see cref="Handlers.Player.PreAuthenticating" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PreAuthenticating))]
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal static class PreAuthenticating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();
            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            LocalBuilder ev = generator.DeclareLocal(typeof(PreAuthenticatingEventArgs));
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "{0};{1};{2};{3}");

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // userid
                    new CodeInstruction(OpCodes.Ldloc_S, 10),

                    // ipaddress
                    new (OpCodes.Ldloc_S, 15),

                    // expiration
                    new (OpCodes.Ldloc_S, 11),

                    // flags
                    new (OpCodes.Ldloc_S, 12),

                    // country
                    new (OpCodes.Ldloc_S, 13),

                    // signature
                    new (OpCodes.Ldloc_S, 14),

                    // request
                    new (OpCodes.Ldarg_1),

                    // position
                    new (OpCodes.Ldloc_S, 9),

                    // PreAuthenticatingEventArgs ev = new (userid, ipaddress, expiration, flags, country, signature, request, position)
                    new (OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAuthenticatingEventArgs))[0]),
                    new (OpCodes.Dup),
                    new (OpCodes.Stloc_S, ev.LocalIndex),

                    // OnPreAuthenticating(ev)
                    new (OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPreAuthenticating))),
                    new (OpCodes.Ldloc_S, ev.LocalIndex),

                    // ev.isallowed
                    new (OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.IsAllowed))),

                    // ret
                    new (OpCodes.Brfalse_S, ret),
                });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}