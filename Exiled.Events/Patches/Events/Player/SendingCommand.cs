namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using AdminAbuse.API.EventArgs;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using RemoteAdmin;
    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="CommandProcessor.ProcessQuery(string, CommandSender)" />.
    ///     Adds the <see cref="Handlers.Player.SendingCommand" /> event.
    /// </summary>
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    public class SendingCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SendingCommandEventArgs));
            
            newInstructions.InsertRange(9, new CodeInstruction[]
            {
                //command
                //in stack already
                //new(OpCodes.Ldarg_0),
                
                //args
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, Method(typeof(System.String), nameof(System.String.Trim))),
                new(OpCodes.Ldsfld, Field(typeof(QueryProcessor), nameof(QueryProcessor.SpaceArray))),
                new(OpCodes.Ldc_I4, 512),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, Method(typeof(System.String), nameof(System.String.Split), new []{typeof(char[]), typeof(int), typeof(System.StringSplitOptions)})),

                //player
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(CommandSender) })),

                //errorText
                new(OpCodes.Ldstr, "Prevented by Exiled"),
                
                //true
                new(OpCodes.Ldc_I4_1),
                
                //SendingCommandEventArgs ev = new(string command, string[] args, Player player, string errorText, bool isAllowed)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCommandEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),
                
                //Handlers.Player.OnSendingCommand(ev)
                new(OpCodes.Call, Method(typeof(Handlers.SendingCommandHandler), nameof(Handlers.SendingCommandHandler.OnSendingCommand))),
                
                //get errorText
                new(OpCodes.Call, PropertyGetter(typeof(SendingCommandEventArgs),nameof(SendingCommandEventArgs.ErrorText))),
                new(OpCodes.Stloc, 0),
                
                //isAllowed continueLabel
                new(OpCodes.Callvirt, PropertyGetter(typeof(SendingCommandEventArgs), nameof(SendingCommandEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                
                //get PlayerCommandSender
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SendingCommandEventArgs),nameof(SendingCommandEventArgs.Player))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Sender))),
                
                new(OpCodes.Ldstr, "SYSTEM#Command execution failed! Error: "),
                new(OpCodes.Ldloc, 0),
                new(OpCodes.Callvirt, Method(typeof(System.String),nameof(System.String.Concat), new []{typeof(string),typeof(string)})),
                
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldstr, ""),
                
                //Send error to player console
                new(OpCodes.Call, Method(typeof(PlayerCommandSender), nameof(PlayerCommandSender.RaReply), new []{typeof(string),typeof(bool),typeof(bool),typeof(string)})),
                
                //return null
                new(OpCodes.Ldnull),
                new(OpCodes.Ret),
                
                //continue
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                
                //return Ldarg_0 to original stack
                new(OpCodes.Ldarg_0)
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}