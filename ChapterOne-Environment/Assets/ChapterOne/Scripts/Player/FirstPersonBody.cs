using UnityEngine;

namespace ThatNight.ChapterOne
{
    [DefaultExecutionOrder(50)]
    public sealed class FirstPersonBody : MonoBehaviour
    {
        public Transform lowerBody;
        public ForestExploration explorer;
        public Transform flashlightModel, holdSocket;
        public float ReachBlend { get; set; }
        public Vector3 ReachTarget { get; set; }
        public bool Holding { get; set; }

        public Transform leftHand => transform;
        public Transform rightHand => transform;

        void Awake()
        {
            if (lowerBody) lowerBody.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            if (lowerBody) lowerBody.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            if (lowerBody && lowerBody.gameObject.activeSelf)
            {
                lowerBody.gameObject.SetActive(false);
            }
        }
    }
}
