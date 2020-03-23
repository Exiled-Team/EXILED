using Mirror;
using Scp914;
using System.Collections.Generic;
using UnityEngine;
using Utils.ConfigHandler;

namespace EXILED.Extensions
{
	public class SCP914
	{
		/// <summary>
		/// Sets SCP-914 knob state.
		/// </summary>
		/// <returns></returns>
		public static void SetKnobStatus(Scp914Knob scp914Knob) => Scp914Machine.singleton.knobState = scp914Knob;

		/// <summary>
		/// Gets SCP-914 knob state.
		/// </summary>
		/// <returns></returns>
		public static Scp914Knob GetKnobStatus() => Scp914Machine.singleton.knobState;

		/// <summary>
		/// Starts SCP-914.
		/// </summary>
		public static void Start() => Scp914Machine.singleton.RpcActivate(NetworkTime.time);

		/// <summary>
		/// Gets the SCP-914 working status.
		/// </summary>
		public static bool IsWorking => Scp914Machine.singleton.working;

		/// <summary>
		/// Gets SCP-914 recipes.
		/// </summary>
		/// <param name="recipes"></param>
		public static Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> GetRecipes() => Scp914Machine.singleton.recipesDict;

		/// <summary>
		/// Sets SCP-914 recipes.
		/// </summary>
		/// <param name="recipes"></param>
		public static void SetRecipes(Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> recipes) => Scp914Machine.singleton.recipesDict = recipes;

		/// <summary>
		/// Gets SCP-914 configs.
		/// </summary>
		/// <param name="configs"></param>
		public static ConfigEntry<Scp914Mode> GetConfig() => Scp914Machine.singleton.configMode;

		/// <summary>
		/// Sets SCP-914 configs.
		/// </summary>
		/// <param name="configs"></param>
		public static void SetConfig(ConfigEntry<Scp914Mode> config) => Scp914Machine.singleton.configMode = config;

		/// <summary>
		/// Gets the intake booth <see cref="Transform">trasform</see>
		/// </summary>
		/// <returns></returns>
		public static Transform GetIntakeBooth() => Scp914Machine.singleton.intake;

		/// <summary>
		///  Gets the output booth<see cref="Transform">trasform</see>
		/// </summary>
		/// <returns></returns>
		public static Transform GetOutputBooth() => Scp914Machine.singleton.output;
	}
}
