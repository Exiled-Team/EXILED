using Mirror;

namespace EXILED.Extensions
{
	public static class Scp079
	{
		/// <summary>
		/// Gets the experience of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static float GetExperience(this ReferenceHub player) => player.scp079PlayerScript.Exp;

		/// <summary>
		/// Sets the experience of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void SetExperience(this ReferenceHub player, float amount)
		{
			player.scp079PlayerScript.Exp = amount;
			player.scp079PlayerScript.OnExpChange();
		}

		/// <summary>
		/// Adds experience to SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void AddExperience(this ReferenceHub player, float amount) => player.scp079PlayerScript.AddExperience(amount);

		/// <summary>
		/// Gets the level of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		public static int GetLevel(this ReferenceHub player) => player.scp079PlayerScript.Lvl;

		/// <summary>
		/// Sets the level of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="level"></param>
		/// <param name="notifyUser"></param>
		public static void SetLevel(this ReferenceHub player, int level, bool notifyUser = true)
		{
			if (player.scp079PlayerScript.Lvl == level)
				return;

			player.scp079PlayerScript.Lvl = level;

			if (notifyUser)
				player.scp079PlayerScript.TargetLevelChanged(player.scp079PlayerScript.connectionToClient, level);
		}

		/// <summary>
		/// Gets the energy of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		public static float GetEnergy(this ReferenceHub player) => player.scp079PlayerScript.Mana;

		/// <summary>
		/// Sets the energy of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void SetEnergy(this ReferenceHub player, float amount) => player.scp079PlayerScript.Mana = amount;

		/// <summary>
		/// Adds energy to SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void AddEnergy(this ReferenceHub player, float amount) => player.scp079PlayerScript.Mana += amount;

		/// <summary>
		/// Gets the max energy of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static float GetMaxEnergy(this ReferenceHub player) => player.scp079PlayerScript.NetworkmaxMana;

		/// <summary>
		/// Sets the max energy of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void SetMaxEnergy(this ReferenceHub player, float amount)
		{
			player.scp079PlayerScript.NetworkmaxMana = amount;
			player.scp079PlayerScript.levels[player.GetLevel()].maxMana = amount;
		}

		/// <summary>
		/// Gets the locked doors <see cref="SyncListString"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static SyncListString GetLockedDoors(this ReferenceHub player) => player.scp079PlayerScript.lockedDoors;

		/// <summary>
		/// Sets the locked doors <see cref="SyncListString"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="lockedDoors"></param>
		public static void SetLockedDoors(this ReferenceHub player, SyncListString lockedDoors) => player.scp079PlayerScript.lockedDoors = lockedDoors;

		/// <summary>
		/// Adds a door to the locked doors <see cref="SyncListString"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="doorName"></param>
		public static void AddLockedDoor(this ReferenceHub player, string doorName)
		{
			if (!player.scp079PlayerScript.lockedDoors.Contains(doorName))
				player.scp079PlayerScript.lockedDoors.Add(doorName);
		}

		/// <summary>
		/// Remove a door from the locked doors <see cref="SyncListString"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="doorName"></param>
		public static void RemoveLockedDoor(this ReferenceHub player, string doorName)
		{
			if (player.scp079PlayerScript.lockedDoors.Contains(doorName))
				player.scp079PlayerScript.lockedDoors.Remove(doorName);
		}

		/// <summary>
		/// Gets the speaker of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		public static string GetSpeaker(this ReferenceHub player) => player.scp079PlayerScript.Speaker;

		/// <summary>
		/// Sets the speaker of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="speaker"></param>
		public static void SetSpeaker(this ReferenceHub player, string speaker, bool lookAtRotation = false) => player.scp079PlayerScript.Speaker = speaker;

		/// <summary>
		/// Gets the camera of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static Camera079 GetCamera(this ReferenceHub player) => player.scp079PlayerScript.currentCamera;

		/// <summary>
		/// Gets all cameras of SCP-079.
		/// </summary>
		/// <returns></returns>
		public static Camera079[] GetCameras() => Scp079PlayerScript.allCameras;

		/// <summary>
		/// Sets the camera of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="camera"></param>
		/// <param name="lookAtRotation"></param>
		public static void SetCamera(this ReferenceHub player, Camera079 camera, bool lookAtRotation = false) => player.SetCamera(camera.cameraId, lookAtRotation);

		/// <summary>
		/// Sets the camera of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="cameraId"></param>
		/// <param name="lookAtRotation"></param>
		public static void SetCamera(this ReferenceHub player, ushort cameraId, bool lookAtRotation = false) => player.scp079PlayerScript.RpcSwitchCamera(cameraId, lookAtRotation);

		/// <summary>
		/// Gets the levels of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static Scp079PlayerScript.Level079[] GetLevels(this ReferenceHub player) => player.scp079PlayerScript.levels;

		/// <summary>
		/// Sets the levels of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="levels"></param>
		public static void SetLevels(this ReferenceHub player, Scp079PlayerScript.Level079[] levels) => player.scp079PlayerScript.levels = levels;

		/// <summary>
		/// Gets the abilities of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static Scp079PlayerScript.Ability079[] GetAbilities(this ReferenceHub player) => player.scp079PlayerScript.abilities;

		/// <summary>
		/// Sets the abilities of SCP-079.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="abilities"></param>
		public static void SetAbilities(this ReferenceHub player, Scp079PlayerScript.Ability079[] abilities) => player.scp079PlayerScript.abilities = abilities;
	}
}
