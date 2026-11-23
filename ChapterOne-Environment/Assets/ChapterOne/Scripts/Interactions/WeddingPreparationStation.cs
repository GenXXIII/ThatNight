using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class WeddingPreparationStation : WorldInteractable
    {
        public override string Prompt => "Prepare for the wedding with Mother";
        public override bool CanUse => ChapterOneDirector.Instance &&
            ChapterOneDirector.Instance.currentScene == GameScene.Scene05_Preparation;

        public override void Use(PlayerInteraction player)
        {
            if (CanUse) ChapterOneDirector.Instance.OnPreparationStarted();
        }
    }
}
