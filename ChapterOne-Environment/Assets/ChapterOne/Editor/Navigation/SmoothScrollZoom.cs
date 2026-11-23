#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    /// <summary>
    /// SmoothScrollZoom:
    /// Makes normal mouse scroll wheel behave like Alt-Dolly zoom.
    /// It smoothly translates the SceneView camera forward and backward without
    /// ever hitting the pivot wall or getting stuck when zooming out or in.
    /// </summary>
    [InitializeOnLoad]
    internal static class SmoothScrollZoom
    {
        static SmoothScrollZoom()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (sceneView == null || sceneView.camera == null) return;
            Event e = Event.current;
            if (e == null) return;

            // When scrolling the mouse wheel without Alt or Ctrl
            if (e.type == EventType.ScrollWheel && !e.alt && !e.control && !e.shift)
            {
                float delta = e.delta.y; // > 0 = scroll back (zoom out), < 0 = scroll forward (zoom in)
                if (Mathf.Abs(delta) < 0.001f) return;

                var camTransform = sceneView.camera.transform;
                Vector3 forward = camTransform.forward;

                // Responsive translation step based on current camera elevation / distance
                float currentDist = Mathf.Max(sceneView.size, 20.0f);
                float stepSpeed = Mathf.Clamp(currentDist * 0.18f, 3.5f, 50.0f);

                // Move pivot and camera through 3D space directly (like Alt-Dolly)
                Vector3 translation = forward * (-delta * stepSpeed);
                sceneView.pivot += translation;

                // Adjust size smoothly and keep it in a healthy responsive range
                float sizeMultiplier = delta < 0 ? 0.90f : 1.10f;
                sceneView.size = Mathf.Clamp(sceneView.size * sizeMultiplier, 15.0f, 600.0f);

                sceneView.Repaint();
                e.Use(); // Consumes the event so Unity's stuck default pivot lock never triggers!
            }
        }
    }
}
#endif
