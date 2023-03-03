using ItemChanger;
using ItemChanger.Internal;
using ModTerminal.Commands;
using System;

namespace ICDebug.Commands
{
    internal static class PlaceItem
    {
        [HelpDocumentation("Places an item at a given location, creating a new placement if needed.")]
        public static string PlaceItemCommand(
            [HelpDocumentation("The Finder name of the item to place.")] string itemName,
            [HelpDocumentation("The Finder name of the location to place the item at.")] string locationName
        )
        {
            AbstractItem? item = Finder.GetItem(itemName);
            if (item == null)
            {
                return $"No item named {itemName} was found.";
            }
            
            if (Ref.Settings.Placements.TryGetValue(locationName, out AbstractPlacement plt))
            {
                try
                {
                    item.Load();
                    plt.Add(item);
                }
                catch (Exception ex)
                {
                    ICDebugMod.Instance.LogError(ex);
                    return $"Failed to place {itemName} at existing placement {locationName}.";
                }
            }
            else
            {
                AbstractLocation? loc = Finder.GetLocation(locationName);
                if (loc == null)
                {
                    return $"No location or placement {locationName} was found.";
                }

                try
                {
                    plt = loc.Wrap();
                    plt.Add(item);
                    plt.Load();
                }
                catch (Exception ex)
                {
                    ICDebugMod.Instance.LogError(ex);
                    return $"Failed to place {itemName} at created placement {locationName}.";
                }
            }

            return $"Successfully placed {itemName} at {locationName}.";
        }
    }
}
