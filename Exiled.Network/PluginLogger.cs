namespace Exiled.Network
{
    using System.Reflection;
    using Exiled.Network.API;

    public class PluginLogger : NPLogger
    {
        public override void Debug(string message)
        {
            Exiled.API.Features.Log.Debug($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        public override void Error(string message)
        {
            Exiled.API.Features.Log.Error($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        public override void Info(string message)
        {
            Exiled.API.Features.Log.Info($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }
    }
}
