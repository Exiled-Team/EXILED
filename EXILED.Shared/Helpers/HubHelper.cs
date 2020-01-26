using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Shared.Helpers
{
    public static class HubHelper
    {

        //Gets a list of all current player ReferenceHubs.
        public static List<ReferenceHub> GetHubs()
        {
            List<ReferenceHub> hubs = new List<ReferenceHub>();
            foreach (GameObject obj in PlayerManager.players)
                hubs.Add(ReferenceHub.GetHub(obj));

            return hubs;
        }
    }
}
