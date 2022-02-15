// -----------------------------------------------------------------------
// <copyright file="ParseVisionInformation.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using SEXiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line

    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.UpdateVision))]
    internal static class ParseVisionInformation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            const int offset = -3;
            const int continueLabelOffset = -3;

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int index = newInstructions.FindIndex(ci => ci.opcode == OpCodes.Div);

            // Quick check if it's the end
            if (index + 1 >= newInstructions.Count)
            {
                Log.Error($"Couldn't patch '{typeof(PlayableScps.Scp096).FullName}.{nameof(PlayableScps.Scp096.UpdateVision)}': invalid index - {index}");
                ListPool<CodeInstruction>.Shared.Return(newInstructions);
                yield break;
            }

            index += offset;

            // Continuation pointer
            // Used to continue execution
            // if both checks fail
            Label continueLabel = generator.DefineLabel();
            newInstructions[newInstructions.FindIndex(ci => ci.opcode == OpCodes.Leave_S) + continueLabelOffset].WithLabels(continueLabel);

            // Second check pointer
            // We use it to pass execution
            // to the second check if the first check fails,
            // otherwise the second check won't be executed
            Label secondCheckPointer = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // if (characterClassManager.CurClass == RoleType.Tutorial && !SEXiled.Events.Events.Instance.Config.CanTutorialTriggerScp096)
                //      continue;
                // START
                new CodeInstruction(OpCodes.Ldloc_S, 4),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager.CurClass))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleType.Tutorial),
                new CodeInstruction(OpCodes.Bne_Un_S, secondCheckPointer),

                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SEXiled.Events.Events), nameof(SEXiled.Events.Events.Instance))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanTutorialTriggerScp096))),
                new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
                // END
                // if (API.Features.Scp096.TurnedPlayers.Contains(Player.Get(referenceHub)))
                //      continue;
                // START
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.TurnedPlayers))).WithLabels(secondCheckPointer),
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
                // END
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
