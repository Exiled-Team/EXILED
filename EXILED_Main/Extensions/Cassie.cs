using MEC;

namespace EXILED.Extensions
{
	public static class Cassie
	{
		private static MTFRespawn _mtfRespawn;
		public static MTFRespawn mtfRespawn
		{
			get
			{
				if (_mtfRespawn == null)
				{
					_mtfRespawn = PlayerManager.localPlayer.GetComponent<MTFRespawn>();
				}
				return _mtfRespawn;
			}
		}

		/// <summary>
		/// Plays a cassie message.
		/// </summary>
		public static void CassieMessage(string msg, bool makeHold, bool makeNoise) => mtfRespawn.RpcPlayCustomAnnouncement(msg, makeHold, makeNoise);

		/// <summary>
		/// Plays a cassie message with a delay.
		/// </summary>
		public static void DelayedCassieMessage(string msg, bool makeHold, bool makeNoise, float delay)
		{
			Timing.CallDelayed(delay, () => mtfRespawn.RpcPlayCustomAnnouncement(msg, makeHold, makeNoise));
		}
	}
}
