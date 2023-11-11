// -----------------------------------------------------------------------
// <copyright file="StartingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
	using System.Collections.Generic;
	using System.Reflection.Emit;

	using API.Features;
	using API.Features.Pools;
	using Exiled.Events.Attributes;
	using Exiled.Events.EventArgs.Scp049;

	using HarmonyLib;

	using PlayerRoles.PlayableScps.Scp049;

	using static HarmonyLib.AccessTools;

	/// <summary>
	///		Patches <see cref="Scp049ResurrectAbility.ServerValidateBegin" />.
	///		Adds the <see cref="Handlers.Scp049.StartingRecall" /> event.
	/// </summary>
	[EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.StartingRecall))]
	[HarmonyPatch(typeof(RagdollAbilityBase<Scp049Role>), nameof(RagdollAbilityBase<Scp049Role>.ServerProcessCmd))]
	internal static class StartingRecall
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

			Label retLabel = generator.DefineLabel();
			
			int index = newInstructions.FindIndex(instruction =>
				instruction.opcode == OpCodes.Callvirt
				&& (MethodInfo)instruction.operand == Method(typeof(ScpSubroutineBase), nameof(ScpSubroutineBase.ServerProcessCmd)));
			index += 1;


			newInstructions.InsertRange(index, new CodeInstruction[]
			{
				// base.CurRagdoll
				new(OpCodes.Ldarg_1),
				new(OpCodes.Callvirt, PropertyGetter(typeof(RagdollAbilityBase<Scp049Role>), nameof(RagdollAbilityBase<Scp049Role>.CurRagdoll))),
				
				
				// Player player = Player.Get(this.Owner);
				new(OpCodes.Ldarg_0),
				new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.Owner))),
				new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
				
				// true
				new(OpCodes.Ldc_I4_1),

				// StartingRecallEventArgs ev = new(player, true);
				new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingRecallEventArgsMod))[0]),
				new(OpCodes.Dup),

				// Handlers.Scp049.OnStartingRecall(ev);
				new(OpCodes.Call, Method(typeof(LimitZombie.Events.Handlers.Scp049), nameof(LimitZombie.Events.Handlers.Scp049.OnStartingRecallMod))),

				// if (!ev.IsAllowed)
				//		return;
				new(OpCodes.Callvirt, PropertyGetter(typeof(StartingRecallEventArgsMod), nameof(StartingRecallEventArgsMod.IsAllowed))),
				new(OpCodes.Brfalse_S, retLabel),
			});
			newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

			foreach (var instruction in newInstructions)
				yield return instruction;

			ListPool<CodeInstruction>.Pool.Return(newInstructions);
		}
	}
}