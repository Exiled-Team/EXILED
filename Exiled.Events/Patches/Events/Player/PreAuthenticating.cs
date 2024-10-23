

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    
    using API.Features.Pools;
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using Hazards;
    using LiteNetLib;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)" />.
    /// Adds the <see cref="Handlers.Player.PreAuthenticating" /> event.
    /// </summary>
    ///
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
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldstr && instruction.operand == "{0};{1};{2};{3}");

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {

                    new CodeInstruction(OpCodes.Ldloc_S, 10),
                    //ipaddress
                    new CodeInstruction(OpCodes.Ldloc_S, 15),
                    //expiration
                    new CodeInstruction(OpCodes.Ldloc_S, 11),
                    //flags
                    new CodeInstruction(OpCodes.Ldloc_S, 12),
                    //country
                    new CodeInstruction(OpCodes.Ldloc_S, 13),
                    //signature
                    new CodeInstruction(OpCodes.Ldloc_S, 14),
                    //request
                    new CodeInstruction(OpCodes.Ldarg_1),
                    //position
                    new CodeInstruction(OpCodes.Ldloc_S, 9),
                    
                    //PreAuthenticatingEventArgs ev = new (userid, ipaddress, expiration, flags, country, signature, request, position)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAuthenticatingEventArgs))[0]),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPreAuthenticating))),
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    // if ev.IsAllowed==false 


                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, ret),



                });


            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
