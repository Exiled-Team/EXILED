// -----------------------------------------------------------------------
// <copyright file="ProcessQuery.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CommandSystem;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Interfaces;
    using Exiled.Permissions.Extensions;
    using HarmonyLib;
    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ICommand.Execute" />.
    /// Implements <see cref="IPermissionCommand.Permission" />.
    /// </summary>
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal class ProcessQuery
    {
        private static bool CheckPermissions(ICommand command, CommandSender sender)
        {
            if (command is IPermissionCommand permissionCommand && !sender.CheckPermission(permissionCommand.Permission))
            {
                sender.RaReply($"{command.Command}#You do not have permissions to use this command", false, true, string.Empty);
                return false;
            }

            return true;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder hasPermissions = generator.DeclareLocal(typeof(bool));

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ICommand), nameof(ICommand.Execute))));
            const int offset = -6;

            newInstructions.InsertRange(index + offset, new CodeInstruction[]
            {
                // command
                new(OpCodes.Ldloc_1),

                // sender
                new(OpCodes.Ldarg_1),

                // bool hasPermissions = CheckPermissions(command, sender)
                new(OpCodes.Call, Method(typeof(ProcessQuery), nameof(CheckPermissions))),
                new(OpCodes.Stloc_S, hasPermissions),

                // hasPermissions
                new(OpCodes.Ldloc_S, hasPermissions),

                // if (!hasPermissions) return;
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}