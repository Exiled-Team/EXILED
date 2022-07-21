// -----------------------------------------------------------------------
// <copyright file="ParseVisionInformation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp096.UpdateVision"/>.
    /// Adds the <see cref="Scp096.TurnedPlayers"/> support.
    /// </summary>
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
                // if (characterClassManager.CurClass == RoleType.Tutorial && !Exiled.Events.Events.Instance.Config.CanTutorialTriggerScp096)
                //      continue;
                // START
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager.CurClass))),
                new(OpCodes.Ldc_I4_S, (sbyte)RoleType.Tutorial),
                new(OpCodes.Bne_Un_S, secondCheckPointer),

                new(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanTutorialTriggerScp096))),
                new(OpCodes.Brfalse_S, continueLabel),

                // END
                // if (API.Features.Scp096.TurnedPlayers.Contains(Player.Get(referenceHub)))
                //      continue;
                // START
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.TurnedPlayers))).WithLabels(secondCheckPointer),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                new(OpCodes.Brtrue_S, continueLabel),

                // END
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
