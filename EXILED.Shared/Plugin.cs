namespace EXILED.Shared
{
    public abstract class Plugin
    {
        public static YamlConfig Config;
        public abstract string GetName { get; }
        public abstract void OnEnable();
        public abstract void OnDisable();
        public abstract void OnReload();

    }
}