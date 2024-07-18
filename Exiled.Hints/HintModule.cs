using Exiled.API.Features;
using Exiled.CustomModules.API.Features;
using HarmonyLib;

namespace Exiled.Hints
{
    public class CustomHints : CustomModule
    {
        public override string Name { get; set; } = "CustomHints";
        public override uint Id { get; set; }
        public override bool IsEnabled { get; set; }
        public override ModulePointer Config { get; set; }
    }

    public class CustomHintsPlugin : Plugin<HintConfig>
    {

        private Harmony harmony;

        public override void OnEnabled()
        {
            harmony = new Harmony("exiled.hints");
            if (CustomHints.OnEnabled.BoundDelegates.ContainsKey(this)) return;
            CustomHints.OnEnabled += OnModuleEnable;
            CustomHints.OnDisabled += OnModuleDisable;
        }

        private void OnModuleEnable(ModuleInfo info)
        {
            harmony.PatchAll();
        }

        private void OnModuleDisable(ModuleInfo info)
        {
            harmony.UnpatchAll();
        }
    }
}