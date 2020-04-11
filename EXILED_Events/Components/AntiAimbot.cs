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
				obj.GetComponent<NicknameSync>().Network_myNickSync = "No Aimbot for you!";
				obj.GetComponent<QueryProcessor>().PlayerId = 75911;
				obj.transform.localScale *= 0.0000001f;
				obj.transform.position = hub.GetPosition();
				NetworkServer.Spawn(obj);
				sync = obj.GetComponent<PlyMovementSync>();
				ident = obj.GetComponent<NetworkIdentity>();

				Timing.RunCoroutine(Thing(), "thing");
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public void OnDestroy()
		{
			Timing.KillCoroutines("thing");
		}

		public IEnumerator<float> Thing()
		{
			for (;;)
			{
				Log.Error("Doing thingy thing.");
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