using ItemChanger;
using ItemChanger.Internal;
using ItemChanger.Placements;
using ItemChanger.Tags;
using ModTerminal.Commands;
using System.Collections.Generic;
using System.Text;

namespace ICDebug.Commands
{
    internal static class PreviewPlacement
    {
        [HelpDocumentation("Shows the items at a placement and their costs. " +
            "Does not add the previewed visit state or fire the OnPreview event.")]
        public static string PreviewPlacementCommand(
            [HelpDocumentation("The name of the placement to preview.")] string placementName, 
            [HelpDocumentation("Whether to ignore tags that hide item names and costs.")] bool ignorePreviewHiding = false
        )
        {
            if (Ref.Settings.Placements.TryGetValue(placementName, out AbstractPlacement plt))
            {
                List<Tag> ignoredPlacementTags = RemoveAndCacheDisableItemPreviewTag(plt, ignorePreviewHiding);
                StringBuilder sb = new($"Items at placement ");
                sb.Append(placementName);
                if (plt is ISingleCostPlacement icsp && icsp.Cost != null)
                {
                    sb.Append(" (");
                    if (icsp.Cost.Paid)
                    {
                        sb.Append("Paid");
                    }
                    else if (!ignorePreviewHiding && plt.HasTag<DisableCostPreviewTag>())
                    {
                        sb.Append("???");
                    }
                    else
                    {
                        sb.Append(icsp.Cost.GetCostText());
                    }
                    sb.Append(")");
                }
                sb.Append(":");
                foreach (AbstractItem item in plt.Items)
                {
                    List<Tag> ignoredItemTags = RemoveAndCacheDisableItemPreviewTag(item, ignorePreviewHiding);
                    sb.AppendLine();
                    sb.Append("  - ");
                    sb.Append(item.GetPreviewName(plt));
                    if (item.GetTag(out CostTag ct) && ct.Cost != null)
                    {
                        sb.Append(" (");
                        if (item.IsObtained())
                        {
                            sb.Append("Obtained");
                        }
                        else if (ct.Cost.Paid)
                        {
                            sb.Append("Paid");
                        }
                        else if (!ignorePreviewHiding && 
                            (item.HasTag<DisableCostPreviewTag>() || plt.HasTag<DisableCostPreviewTag>()))
                        {
                            sb.Append("???");
                        }
                        else
                        {
                            sb.Append(ct.Cost.GetCostText());
                        }
                        sb.Append(")");
                    }
                    ReapplyIgnoredTags(item, ignoredItemTags);
                }
                ReapplyIgnoredTags(plt, ignoredPlacementTags);

                return sb.ToString();
            }
            return $"No placement named {placementName} exists.";
        }

        private static List<Tag> RemoveAndCacheDisableItemPreviewTag(TaggableObject to, bool shouldIgnore)
        {
            List<Tag> ignoredTags = new();
            if (shouldIgnore && to.tags != null)
            {
                for (int i = 0; i < to.tags.Count; i++)
                {
                    if (to.tags[i] is DisableItemPreviewTag t)
                    {
                        ignoredTags.Add(t);
                        to.tags.Remove(t);
                        i--;
                    }
                }
            }
            return ignoredTags;
        }

        private static void ReapplyIgnoredTags(TaggableObject to, List<Tag> ignoredTags)
        {
            if (to.tags != null)
            {
                foreach (Tag t in ignoredTags)
                {
                    to.tags.Add(t);
                }
            }
        }
    }
}
