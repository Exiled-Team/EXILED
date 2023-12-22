namespace Exiled.Events.Patches.Events.Scp559
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp559;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp559Cake.SetPosition"/>
    /// to add <see cref="Handlers.Scp559.Spawning"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp559), nameof(Handlers.Scp559.Spawning))]
    [HarmonyPatch(typeof(Scp559Cake), nameof(Scp559Cake.SetPosition))]
    internal class Spawning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningEventArgs));

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Scp559.Get(this);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Scp559), nameof(Scp559.Get), new[] { typeof(Scp559Cake) })),

                    // prev
                    new(OpCodes.Ldarg_1),

                    // cur
                    new(OpCodes.Ldarg_2),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // SpawningEventArgs ev = new(Scp559, Vector3, Vector3, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp559.OnSpawning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp559), nameof(Handlers.Scp559.OnSpawning))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // cur = ev.NewPosition;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.NewPosition))),
                    new(OpCodes.Starg_S, 2)
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }
    }
}