using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXILED.Extensions
{
    public static class CassieExtensions
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
        public void CassieMessage(string msg, bool makeHold, bool makeNoise)
        {
            mtfRespawn.RpcPlayCustomAnnouncement(msg, makeHold, makeNoise);
        }

        /// <summary>
        /// Plays a cassie message with a delay.
        /// </summary>
        public IEnumerator<float> DelayedCassieMessage(string msg, bool makeHold, bool makeNoise, float delay)
        {
            yield return Timing.WaitForSeconds(delay);
            mtfRespawn.RpcPlayCustomAnnouncement(msg, makeHold, makeNoise);
        }
    }
}
