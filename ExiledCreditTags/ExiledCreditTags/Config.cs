using System.ComponentModel;
using Exiled.API.Interfaces;

namespace ExiledCreditTags
{
    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("If true a badge will be given, if false custom player text will be given")]
        public bool UseBadge { get; set; } = true;

        [Description("Should badge override existing badges?")]
        public bool BadgeOverride { get; set; } = true;

        [Description("Should CPT override existing CPT?")]
        public bool CPTOverride { get; set; } = true;
    }
}