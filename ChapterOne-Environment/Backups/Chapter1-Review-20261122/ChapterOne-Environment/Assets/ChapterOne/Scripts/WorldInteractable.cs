using UnityEngine;
namespace ThatNight.ChapterOne
{
    public abstract class WorldInteractable : MonoBehaviour
    {
        public string displayName;
        public virtual float Reach => 2.1f;
        public abstract string Prompt { get; }
        public abstract void Use(PlayerInteraction player);
    }
}
