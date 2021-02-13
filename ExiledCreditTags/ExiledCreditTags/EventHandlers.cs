namespace ExiledCreditTags.EventHandlers
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.API.Extensions;
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Hints;
    using System.Collections.Generic;

    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        Random random = new Random();
        public void OnPlayerVerify(VerifiedEventArgs ev)
        {
            string PlayerID = HashSh1(ev.Player.UserId);
            if (plugin.CreditTags.ContainsKey(PlayerID))
            {
                if ((ev.Player.RankName == null || plugin.Config.BadgeOverride) && plugin.Config.UseBadge)
                {
                    ev.Player.RankName = plugin.CreditTags[PlayerID];
                    switch (plugin.CreditTags[PlayerID])
                    {
                    case "Exiled Plugin Dev":
                        ev.Player.RankColor = "crimson";
                        break;
                    case "Exiled Contributor":
                        ev.Player.RankColor = "cyan";
                        break;
                    case "Exiled Dev":
                        ev.Player.RankColor = "magenta";
                        break;
                    };
                }

                if ((ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString == null || plugin.Config.CPTOverride) && !(plugin.Config.UseBadge))
                {
                    switch (plugin.CreditTags[PlayerID])
                    {
                    case "Exiled Plugin Dev":
                        ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = "<color=#DC143C>Exiled Plugin Dev</color>";
                        break;
                    case "Exiled Contributor":
                        ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = "<color=#800080>Exiled Contributor</color>";
                        break;
                    case "Exiled Dev":
                        ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = "<color=#00FFFF>Exiled Dev</color>";
                        break;
                    };
                }
            }
        }
        static string HashSh1(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hashSh1 = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

                // declare stringbuilder
                var sb = new StringBuilder(hashSh1.Length * 2);

                // computing hashSh1
                foreach (byte b in hashSh1)
                {
                    // "x2"
                    sb.Append(b.ToString("X2").ToLower());
                }

                // final output
                Console.WriteLine(string.Format("The SHA1 hash of {0} is: {1}", input, sb.ToString()));

                return sb.ToString();
            }
        }
    }
}
