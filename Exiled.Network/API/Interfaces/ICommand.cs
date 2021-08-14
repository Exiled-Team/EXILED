namespace Exiled.Network.API.Interfaces
{
    using System.Collections.Generic;

    public interface ICommand
    {
        string CommandName { get; }
        string Description { get; }
        string Permission { get; }
        bool IsRaCommand { get; }
        void Invoke(PlayerFuncs player, List<string> arguments);
    }
}
