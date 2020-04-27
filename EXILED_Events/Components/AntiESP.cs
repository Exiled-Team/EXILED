using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.ApiObjects;
using EXILED.Extensions;
using MEC;
using Mirror;
using RemoteAdmin;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED.Components
{
	public class AntiESP : MonoBehaviour
	{
		private List<GameObject> objects = new List<GameObject>();
		private ObjectDestroyMessage msg;
		List<ItemType> items = new List<ItemType>
		{
			ItemType.MicroHID, ItemType.GunE11SR, ItemType.GunCOM15, ItemType.Coin, ItemType.KeycardO5, ItemType.Ammo9mm, ItemType.Ammo556, ItemType.KeycardScientist, ItemType.KeycardFacilityManager, ItemType.KeycardZoneManager, ItemType.Adrenaline
		};

		public void Awake()
		{
			if (EventPlugin.RespawningESPBreaker)
				Timing.RunCoroutine(MoveEspDummies(), "antiesp");
			for (int i = 0; i < 35; i++)
			{
				try
				{
					GameObject obj =
						Instantiate(
							NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
					CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
					ccm.CurClass = RoleType.ClassD;
					ccm.RefreshPlyModel();
					obj.GetComponent<NicknameSync>().Network_myNickSync = "No ESP for you!";
					obj.GetComponent<QueryProcessor>().PlayerId = 11975 + i;
					obj.transform.localScale *= 0.0000001f;
					obj.transform.position = Map.Rooms[EventPlugin.Gen.Next(Map.Rooms.Count)].Position;
					objects.Add(obj);
					NetworkServer.Spawn(obj);
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}
			}

			foreach (Room room in Map.Rooms)
			{
				var item = Map.SpawnItem(items[EventPlugin.Gen.Next(items.Count)], float.NegativeInfinity,
					room.Position + Vector3.up * 2f);
				item.gameObject.transform.localScale *= 0.0000001f;
			}
		}

		public void OnDestroy()
		{
			Timing.KillCoroutines("antiesp");
		}

		public IEnumerator<float> MoveEspDummies()
		{
			for (;;)
			{
				IEnumerable<ReferenceHub> hubs = Player.GetHubs();
				IEnumerable<ReferenceHub> referenceHubs = hubs as ReferenceHub[] ?? hubs.ToArray();
				if (!referenceHubs.Any())
				{
					yield return Timing.WaitForSeconds(1f);
					continue;
				}

				ObjectDestroyMessage msg = new ObjectDestroyMessage();
				foreach (GameObject obj in objects)
				{
					NetworkIdentity ident = obj.GetComponent<NetworkIdentity>();
					msg.netId = ident.netId;
					obj.transform.position = Map.Rooms[EventPlugin.Gen.Next(Map.Rooms.Count)].Position;

					foreach (ReferenceHub hub in Player.GetHubs())
					{
						NetworkConnection conn = hub.scp079PlayerScript.connectionToClient;
						conn.Send(msg);
				
						object[] parameters = new object[]{ident, conn};
						typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
					}

					yield return Timing.WaitForSeconds(1f);
				}

				yield return Timing.WaitForSeconds(120f);
			}
		}
	}
}