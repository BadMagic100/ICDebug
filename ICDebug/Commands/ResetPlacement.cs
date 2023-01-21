using ItemChanger;
using ItemChanger.Internal;
using ItemChanger.Placements;
using Modding;
using ModTerminal;

namespace ICDebug.Commands
{
    internal static class ResetPlacement
    {
        [HelpDocumentation("Resets a placement to its initial state. Useful for when you "
            + "obtain an item and then load a save state, or similar scenarios.")]
        public static string ResetPlacementCommand(
            [HelpDocumentation("The name of the placement to reset.")] string placementName,
            [HelpDocumentation("Whether to reset the placement's visit state.")] bool visitState = true,
            [HelpDocumentation("Whether to reset the obtain state of items at the placement.")] bool obtainState = true,
            [HelpDocumentation("Whether to reset costs associated with the placement.")] bool costs = true
        )
        {
            if (!Ref.Settings.Placements.TryGetValue(placementName, out AbstractPlacement plt))
            {
                return $"No placement with name {placementName} was found.";
            }

            if (visitState)
            {
                ReflectionHelper.SetProperty(plt, nameof(AbstractPlacement.Visited), VisitState.None);
            }
            if (obtainState)
            {
                foreach (AbstractItem item in plt.Items)
                {
                    ReflectionHelper.SetField(item, "obtainState", ObtainState.Unobtained);
                }
            }
            if (costs)
            {
                if (plt is ISingleCostPlacement icsp && icsp.Cost != null)
                {
                    icsp.Cost.Paid = false;
                    if (icsp.Cost is MultiCost mc)
                    {
                        foreach (Cost c in mc)
                        {
                            c.Paid = false;
                        }
                    }
                }
                foreach (AbstractItem item in plt.Items)
                {
                    if (item.GetTag(out CostTag ct) && ct.Cost != null)
                    {
                        ct.Cost.Paid = false;
                        if (ct.Cost is MultiCost mc)
                        {
                            foreach (Cost c in mc)
                            {
                                c.Paid = false;
                            }
                        }
                    }
                }
            }

            return $"Successfully reset placement {placementName}.";
        }
    }
}
