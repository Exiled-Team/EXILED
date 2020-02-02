using EXILED;

public static class Extenstions
{
    public static bool CheckPermission(this ReferenceHub rh, string permission)
    {
        if (rh.gameObject == PlayerManager.localPlayer)
            return true;
        return PermissionPlugin.CheckPermission(rh, permission);
    }
}