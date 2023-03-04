using ICDebug.Commands;
using Modding;
using ModTerminal;
using ModTerminal.Commands;
using System;

namespace ICDebug
{
    public class ICDebugMod : Mod
    {
        private static ICDebugMod? _instance;

        internal static ICDebugMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(ICDebugMod)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public ICDebugMod() : base("ICDebug")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            CommandTable icCommands = new("Commands for interacting with ItemChanger saves.");
            ModTerminalMod.Instance.PrimaryCommandTable.RegisterGroup("ic", icCommands);

            CommandTable itemCommands = new("Commands for interacting with ItemChanger items.");
            itemCommands.RegisterCommand(new("give", GiveItem.GiveItemCommand));
            itemCommands.RegisterCommand(new("find", FindItem.FindItemCommand));
            itemCommands.RegisterCommand(new("findbytype", FindItem.FindItemByTypeCommand));
            itemCommands.RegisterCommand(new("place", PlaceItem.PlaceItemCommand));
            icCommands.RegisterGroup("item", itemCommands);

            CommandTable placementCommands = new("Commands for interacting with ItemChanger placements.");
            placementCommands.RegisterCommand(new("preview", PreviewPlacement.PreviewPlacementCommand));
            placementCommands.RegisterCommand(new("reset", ResetPlacement.ResetPlacementCommand));
            icCommands.RegisterGroup("placement", placementCommands);

            CommandTable moduleCommands = new("Commands for interacting with ItemChanger modules.");
            moduleCommands.RegisterCommand(new("add", LoadModule.LoadModuleCommand));
            icCommands.RegisterGroup("module", moduleCommands);

            Log("Initialized");
        }
    }
}
