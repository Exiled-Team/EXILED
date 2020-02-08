using System;
using System.Threading;
using EXILED;
using UnityEngine;
using Exception = Mono.WebBrowser.Exception;

namespace EXILED_Idler
{
	public class Plugin : EXILED.Plugin
	{
		private static Thread _thread;

		public static DateTime LastActive;

		public static bool WasLastCheckIdle;
		
		public override void OnEnable()
		{
			_thread = new Thread(DoIdleCheck);
			_thread.Start();
			Events.PreAuthEvent += OnPreAuthEvent;
		}

		private void OnPreAuthEvent(ref PreauthEvent ev)
		{
			Log.Info("Server process resuming..");
			WasLastCheckIdle = false;
			LastActive = DateTime.UtcNow;
		}

		public override void OnDisable()
		{
			_thread = null;
		}

		public override void OnReload()
		{
			
		}

		public override string getName => "EXILED_Idler";



		public static void DoIdleCheck()
		{
			while(_thread != null)
			{
				try
				{
					bool idle = IsServerIdle();

					if (idle && !WasLastCheckIdle)
					{
						LastActive = DateTime.UtcNow;
						Log.Debug("cool, the server is idle for the first time");
					}

					if (idle && WasLastCheckIdle && LastActive != null && DateTime.UtcNow.Subtract(LastActive).TotalMinutes > 3)
					{
						Log.Info("The server is now idle..");
						Time.timeScale = 0.01f;
						Application.targetFrameRate = 1;
					}

					if (!idle || (LastActive != null && DateTime.UtcNow.Subtract(LastActive).TotalMinutes < 1))
					{
						Time.timeScale = 1f;
						Application.targetFrameRate = 60;
						Log.Debug("oh no, the server isnt idle");
					}

					WasLastCheckIdle = idle;
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}
				Thread.Sleep(5000);
			}
		}


		public static bool IsServerIdle()
		{
			lock(PlayerManager.players)
			{
				if (PlayerManager.players.Count == 0)
					return true;
			}

			return false;
		}
	}
}