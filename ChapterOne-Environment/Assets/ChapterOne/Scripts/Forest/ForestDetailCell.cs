using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class ForestDetailCell : MonoBehaviour
    {
        public Renderer[] details;
        public float visibleDistance = 180;

        int lastState = -1;
        Vector3 cachedPos;
        bool posCached;

        public void SetVisible(Vector3 eye)
        {
            if (details == null || details.Length == 0) return;
            if (!posCached) { cachedPos = transform.position; posCached = true; }

            float dx = eye.x - cachedPos.x;
            float dz = eye.z - cachedPos.z;
            int nextState = (dx * dx + dz * dz < visibleDistance * visibleDistance) ? 1 : 0;
            if (nextState == lastState) return;
            lastState = nextState;

            bool visible = nextState == 1;
            for (int i = 0; i < details.Length; i++)
                if (details[i]) details[i].enabled = visible;
        }

        public void ShowAll()
        {
            lastState = 1;
            if (details == null) return;
            for (int i = 0; i < details.Length; i++)
                if (details[i]) details[i].enabled = true;
        }
    }
}
