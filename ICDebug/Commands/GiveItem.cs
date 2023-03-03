using ItemChanger;
using ItemChanger.Internal;
using ModTerminal.Commands;
using System;
using System.Collections.Generic;

namespace ICDebug.Commands
{
    internal static class GiveItem
    {
        [HelpDocumentation("Gives the player an item by name.")]
        public static string? GiveItemCommand(
            [HelpDocumentation("The name of the item to give.")] string itemName,
            [HelpDocumentation("Whether to give the item from a placement")] bool fromPlacement = false,
            [HelpDocumentation("The name of the placement to give item from, if giving from a placement. " 
                + "If not specified, the item will be given from the first available placement that has a matching item.")] 
            string? placementName = null
        )
        {
            if (placementName != null && !fromPlacement)
            {
                return $"{nameof(placementName)} can only be specified if {nameof(fromPlacement)} is true.";
            }

            if (!fromPlacement)
            {
                if (!TryGiveItemFromFinder(itemName))
                {
                    return $"Failed to give {itemName}";
                }
            }
            else
            {
                if (!TryGiveItemFromPlacement(itemName, out string finalPlacement, placementName))
                {
                    return $"Failed to give {itemName} from {finalPlacement}";
                }
            }
            return null;
        }

        private static bool TryGiveItemFromFinder(string itemName)
        {
            AbstractItem? item = Finder.GetItem(itemName);
            if (item == null)
            {
                return false;
            }

            item.Load();
            GiveInfo inf = new()
            {
                FlingType = FlingType.DirectDeposit,
                Container = Container.Unknown,
                MessageType = MessageType.Corner
            };

            try
            {
                item.Give(null, inf);
                item.Unload();
            }
            catch (Exception ex)
            {
                ICDebugMod.Instance.LogError(ex);
                return false;
            }

            return true;
        }

        private static bool TryGiveItemFromPlacement(string itemName, out string finalPlacement, string? placementName = null)
        {
            if (placementName != null)
            {
                finalPlacement = placementName;
                try
                {
                    AbstractPlacement plt = Ref.Settings.Placements[finalPlacement];
                    return TryGiveItemFromPlacement(itemName, plt);
                }
                catch (Exception ex)
                {
                    if (ex is not KeyNotFoundException)
                    {
                        ICDebugMod.Instance.LogError(ex);
                    }
                    return false;
                }
            }
            else
            {
                foreach (AbstractPlacement plt in Ref.Settings.Placements.Values)
                {
                    if (TryGiveItemFromPlacement(itemName, plt))
                    {
                        finalPlacement = plt.Name;
                        return true;
                    }
                }
                finalPlacement = "<Placement Not Found>";
                return false;
            }
        }

        private static bool TryGiveItemFromPlacement(string itemName, AbstractPlacement plt)
        {
            GiveInfo inf = new()
            {
                FlingType = FlingType.DirectDeposit,
                Container = Container.Unknown,
                MessageType = MessageType.Corner
            };

            foreach (AbstractItem item in plt.Items)
            {
                if (item.name == itemName)
                {
                    item.Give(plt, inf);
                    return true;
                }
            }
            return false;
        }
    }
}
