using UnityEngine;

namespace EXILED.Shared.Helpers
{
    public static class PositionHelper
    {
        //Gets the spawn point of the selected role.
        public static Vector3 GetRandomSpawnPoint(RoleType role)
        {
            GameObject randomPosition = UnityEngine.Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(role);
            if (randomPosition == null)
                return Vector3.zero;
            return randomPosition.transform.position;
        }
    }
}
