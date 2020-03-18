using EXILED;

public static class Extenstions
{
	public static bool CheckPermission(this ReferenceHub rh, string permission) => rh.gameObject == PlayerManager.localPlayer || PermissionPlugin.CheckPermission(rh, permission);
}