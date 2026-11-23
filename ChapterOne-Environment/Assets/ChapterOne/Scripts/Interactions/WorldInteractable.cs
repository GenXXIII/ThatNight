using UnityEngine;

namespace ThatNight.ChapterOne
{
    public abstract class WorldInteractable : MonoBehaviour
    {
        public string displayName;
        public virtual float Reach => 1.30f; // Close natural arm reach
        public virtual bool CanUse => true;
        public abstract string Prompt { get; }
        public abstract void Use(PlayerInteraction player);
    }
}
