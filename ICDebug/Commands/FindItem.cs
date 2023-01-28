using ItemChanger;
using ItemChanger.Internal;
using ItemChanger.Tags;
using ModTerminal;
using System.Collections.Generic;
using System.Text;

namespace ICDebug.Commands
{
    internal static class FindItem
    {
        [HelpDocumentation("Finds the locations where a given item is placed.")]
        public static string FindItemCommand(
            [HelpDocumentation("The name of the item to look for.")] string itemName,
            [HelpDocumentation("The maximum amount of results to show.")] uint? limit = null,
            [HelpDocumentation("The number of results to skip past.")] uint offset = 0,
            [HelpDocumentation("Whether or not to follow progressive items "
                + "(i.e. should searching Mothwing_Cloak also find Shade_Cloak).")] 
            bool followChains = false, 
            [HelpDocumentation("Whether to show the count of the item only (avoid location spoilers).")]
            bool onlyCount = false, 
            [HelpDocumentation("Whether to skip instances of the item which have been found already.")] 
            bool skipFound = false
        )
        {
            List<(AbstractPlacement, AbstractItem)> foundItems = new();
            void Iterate()
            {
                foreach (AbstractPlacement plt in Ref.Settings.Placements.Values)
                {
                    foreach (AbstractItem item in plt.Items)
                    {
                        if (!(skipFound && item.WasEverObtained()))
                        {
                            if (offset == 0)
                            {
                                if (item.name == itemName)
                                {
                                    foundItems.Add((plt, item));
                                }
                                else if (followChains && item.GetTag(out ItemChainTag ict) && ChainContains(itemName, ict))
                                {
                                    foundItems.Add((plt, item));
                                }
                            }
                            else
                            {
                                offset--;
                            }

                            if (foundItems.Count >= limit)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            Iterate();
            if (onlyCount || foundItems.Count == 0)
            {
                return $"Found {foundItems.Count} items matching the search parameters.";
            }
            else if (!followChains)
            {
                StringBuilder sb = new($"Found {itemName} at the following locations:");
                sb.AppendLine();
                foreach (var (plt, _) in foundItems)
                {
                    sb.Append("  - ");
                    sb.AppendLine(plt.Name);
                }
                return sb.ToString();
            }
            else
            {
                StringBuilder sb = new($"Found the following occurrences:");
                sb.AppendLine();
                foreach (var (plt, item) in foundItems)
                {
                    sb.Append("  - ");
                    sb.Append(item.name);
                    sb.Append(" at ");
                    sb.AppendLine(plt.Name);
                }
                return sb.ToString();
            }
        }

        private static bool ChainContains(string itemName, ItemChainTag ict)
        {
            if (ChainContainsDirectly(itemName, ict))
            {
                return true;
            }

            string? predecessor = ict.predecessor;
            while (predecessor != null)
            {
                AbstractItem? pred = Finder.GetItem(predecessor);
                if (pred != null && pred.GetTag(out ItemChainTag pict))
                {
                    if (ChainContainsDirectly(itemName, pict))
                    {
                        return true;
                    }
                    predecessor = pict.predecessor;
                }
                else
                {
                    predecessor = null;
                }
            }

            string? successor = ict.successor;
            while (successor != null)
            {
                AbstractItem? succ = Finder.GetItem(successor);
                if (succ != null && succ.GetTag(out ItemChainTag sict))
                {
                    if (ChainContainsDirectly(itemName, sict))
                    {
                        return true;
                    }
                    successor = sict.successor;
                }
                else
                {
                    successor = null;
                }
            }

            return false;
        }

        private static bool ChainContainsDirectly(string itemName, ItemChainTag ict) => 
            ict.predecessor == itemName || ict.successor == itemName;
    }
}
