using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ExiledCreditTags
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Exiled.API.Features;
    using MEC;
    using Player = Exiled.Events.Handlers.Player;

    public class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public override string Name { get; } = "Exiled tags";
        public override string Author { get; } = "Babyboucher20";
        public override Version Version { get; } = new Version(1, 1, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 29);
        public override string Prefix { get; } = "Exiled tags";

        public EventHandlers.EventHandlers PlayerHandlers;

        public Dictionary<String, string> CreditTags { get; set; }  = GetURL();

        public override void OnEnabled()
        {
            PlayerHandlers = new EventHandlers.EventHandlers(this);
            Player.Verified += PlayerHandlers.OnPlayerVerify;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.Verified -= PlayerHandlers.OnPlayerVerify;
            base.OnDisabled();
        }

        public static Dictionary<string, string> GetURL()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://gist.githubusercontent.com/babyboucher/89b23cd569e759c57f458df0411588fe/raw/PublicIDs");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                Dictionary<string, string> dict = Utf8Json.JsonSerializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd());
                return dict;
            }
        }
    }
}
