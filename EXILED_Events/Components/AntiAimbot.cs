using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.Extensions;
using MEC;
using Mirror;
using RemoteAdmin;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED.Components
{
	public class AntiAimbot : MonoBehaviour
	{
		private ReferenceHub hub;
		private Scp049PlayerScript script;
		private GameObject obj;
		private PlyMovementSync sync;
		private NetworkIdentity ident;

		public void Awake()
		{
			Log.Error("Aimbot awake.");
			try
			{
				hub = gameObject.GetPlayer();
				script = gameObject.GetComponent<Scp049PlayerScript>();
				obj = Instantiate(
					NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
				CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
				ccm.CurClass = RoleType.ClassD;
				ccm.RefreshPlyModel();
				List<ReferenceHub> players = Player.GetHubs().ToList();
				int r = EventPlugin.Gen.Next(players.Count);
				obj.GetComponent<NicknameSync>().Network_myNickSync = players[r].GetNickname();
				obj.GetComponent<QueryProcessor>().PlayerId = players[r].GetPlayerId();
				obj.transform.localScale *= 0.0000001f;
				obj.transform.position = hub.GetPosition();
				NetworkServer.Spawn(obj);
				sync = obj.GetComponent<PlyMovementSync>();
				ident = obj.GetComponent<NetworkIdentity>();
				ReferenceHub fakeHub = obj.GetComponent<ReferenceHub>();
				if (fakeHub != null)
					Destroy(hub);

				Timing.RunCoroutine(RefreshAimbotLocation(), gameObject);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public void OnDestroy()
		{
			Timing.KillCoroutines(gameObject);
		}

		public IEnumerator<float> RefreshAimbotLocation()
		{
			for (;;)
			{
				try
				{
					Vector3 pos = new Vector3(hub.GetPosition().x, hub.GetPosition().y + 2f, hub.GetPosition().z);
					obj.transform.position = pos;
					ObjectDestroyMessage msg = new ObjectDestroyMessage{ netId = ident.netId};
					NetworkConnection conn = hub.scp079PlayerScript.connectionToClient;
					conn.Send(msg);
				
					object[] parameters = new object[]{ident, conn};
					typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}

				yield return Timing.WaitForSeconds(5f);
			}
		}
	}
}