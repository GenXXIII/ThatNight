using UnityEngine;

namespace ThatNight.ChapterOne
{
    /// <summary>
    /// When attached to a parent GameObject, tells the Unity Editor Scene View
    /// to always select this root object when any child component or mesh is clicked.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class ModelRoot : MonoBehaviour
    {
        [Tooltip("Descriptive model category or name.")]
        public string modelName;
    }
}
