namespace RueI.Patches;

using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using static HarmonyLib.AccessTools;

using Hints;
using Mirror;
using NorthwoodLib.Pools;

using RueI.Displays;
using RueI.Displays.Scheduling;
using static RueI.Patches.PatchHelper;

using UnityEngine;

/// <summary>
/// Patches <see cref="HintDisplay.Show"/> to detect when hints are shown.
/// </summary>
/// <remarks>
/// RueI patches the <see cref="HintDisplay.Show"/> method so that it can get an accurate state of the player's hint display at any given time.
/// When a hint outside of RueI is shown, it schedules an update that occurs after 3 seconds.
/// </remarks>
[HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
public static class HintPatch
{
    private const double MAXANONYMOUSHINTTIME = 3;
    private const int UPDATEPRIORITY = 10;

    private static readonly JobToken UpdateToken = new();

    private delegate bool TryGetHub(GameObject player, out ReferenceHub hub);

    /// <summary>
    /// Patches <see cref="HintDisplay.Show"/> to detect when hints are shown.
    /// </summary>
    /// <param name="instructions">The original instructions.</param>
    /// <param name="generator">An <see cref="ILGenerator"/> to use.</param>
    /// <returns>The new instructions.</returns>
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        int index = newInstructions.FindLastIndex(instruction =>
        {
            if (instruction.opcode == OpCodes.Callvirt)
            {
                MethodInfo info = (MethodInfo)instruction.operand;
                if (info.DeclaringType == typeof(NetworkConnection) && info.Name == nameof(NetworkConnection.Send) && info.IsGenericMethod)
                {
                    return true;
                }
            }

            return false;
        });

        index -= 1;

        LocalBuilder refHubLocal = generator.DeclareLocal(typeof(ReferenceHub));
        LocalBuilder schedulerLocal = generator.DeclareLocal(typeof(Scheduler));

        Label skipLabel = generator.DefineLabel();

        CodeInstruction[] collection =
        {
                // NetworkConnection
                new(OpCodes.Ldloc_0),

                // connection.identity.gameObject
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.gameObject))),

                // out ReferenceHub hub
                new(OpCodes.Ldloca_S, refHubLocal.LocalIndex),

                // if (ReferenceHub.TryGetHub(connection.identity.gameObject, out ReferenceHub hub))
                // {
                new(OpCodes.Call, DelegeteMatch<TryGetHub>(typeof(ReferenceHub), nameof(ReferenceHub.TryGetHub))),
                new(OpCodes.Brfalse_S, skipLabel),
                new(OpCodes.Ldloc_S, refHubLocal.LocalIndex),

                // Scheduler scheduler = core.Scheduler
                new(OpCodes.Call, Method(typeof(DisplayCore), nameof(DisplayCore.Get))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DisplayCore), nameof(DisplayCore.Scheduler))),
                new(OpCodes.Stloc_S, schedulerLocal.LocalIndex),

                // scheduler.Delay(Constants.HintRateLimit)
                new(OpCodes.Ldloc_S, schedulerLocal.LocalIndex),
                new(OpCodes.Ldsfld, Field(typeof(Constants), nameof(Constants.HintRateLimit))),
                new(OpCodes.Callvirt, Method(typeof(Scheduler), nameof(Scheduler.Delay))),

                // scheduler
                new(OpCodes.Ldloc_S, schedulerLocal.LocalIndex),

                // core.Scheduler.ScheduleUpdate(3, 10, token);
                new(OpCodes.Ldc_R8, MAXANONYMOUSHINTTIME),
                new(OpCodes.Call, Method(typeof(TimeSpan), nameof(TimeSpan.FromSeconds))),
                new(OpCodes.Ldc_I4, UPDATEPRIORITY),
                new(OpCodes.Ldsfld, Field(typeof(HintPatch), nameof(UpdateToken))),
                new(OpCodes.Callvirt, Method(typeof(Scheduler), nameof(Scheduler.ScheduleUpdateToken))),

                // }
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
        };

        newInstructions.InsertRange(index, collection);

        foreach (CodeInstruction instruction in newInstructions)
        {
            yield return instruction;
        }

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

#if DEBUG
    private static void LogHint(float duration)
    {
        UnityAlternative.Provider.LogDebug($"External hint shown, duration is {duration} seconds");
    }
#endif

}