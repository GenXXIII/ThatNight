using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class ForestDetailCell : MonoBehaviour
    {
        public Renderer[] details;
        public float visibleDistance = 180;
        public void SetVisible(Vector3 eye)
        {
            var delta = eye - transform.position; delta.y = 0;
            bool visible = delta.sqrMagnitude < visibleDistance * visibleDistance;
            foreach (var item in details) if (item) item.enabled = visible;
        }
        public void ShowAll() { foreach (var item in details) if (item) item.enabled = true; }
    }
}
