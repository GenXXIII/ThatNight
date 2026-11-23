using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class BridalJewelryCasket : WorldInteractable
    {
        public override float Reach => 1.30f;
        public override bool CanUse => true;
        public override string Prompt => "Inspect Bridal Jewelry Casket";

        public override void Use(PlayerInteraction player)
        {
            if (player != null)
            {
                player.ReachAndAct(transform.position, () =>
                {
                    var narrative = ChapterOneNarrative.Instance;
                    if (narrative != null)
                    {
                        var line = new DialogueLine
                        {
                            speaker = DialogueSpeaker.Bride,
                            speakerName = "Sophea (Thought)",
                            text = "Mother placed the heirloom gold wedding necklace here for tonight.",
                            duration = 3.5f
                        };
                        narrative.PlaySequence(new List<DialogueLine> { line });
                    }
                });
            }
        }
    }
}
