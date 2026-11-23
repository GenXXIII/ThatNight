using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    public static class ChapterOneSceneRepair
    {
        [MenuItem("THAT NIGHT/Chapter 1/Repair story interaction wiring")]
        public static void Apply()
        {
            if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Stop Play first.");
            var director = Object.FindFirstObjectByType<ChapterOneDirector>();
            if (!director) throw new System.InvalidOperationException("Chapter director is missing.");
            var phone = Object.FindFirstObjectByType<NokiaHandPhone>();
            Transform table = null;
            float nearest = float.PositiveInfinity;
            foreach (var candidate in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
            {
                if (!candidate.name.StartsWith("Bridal dressing table")) continue;
                float distance = phone ? (candidate.position - phone.transform.position).sqrMagnitude : candidate.position.x;
                if (distance < nearest) { nearest = distance; table = candidate; }
            }
            if (!table) throw new System.InvalidOperationException("Bridal dressing table is missing.");
            var station = table.GetComponent<WeddingPreparationStation>();
            if (!station) station = Undo.AddComponent<WeddingPreparationStation>(table.gameObject);
            station.displayName = "Bridal dressing table";
            Undo.RecordObject(director, "Wire preparation station");
            director.preparationStation = station;
            if (phone)
            {
                Undo.RecordObject(phone, "Set Chapter 1 caller label");
                phone.callerName = "Brother";
                if (phone.transform.parent != table)
                {
                    Undo.SetTransformParent(phone.transform, table, "Place phone on bedroom mirror table");
                }
                Undo.RecordObject(phone.transform, "Rest phone on mirror table");
                phone.transform.localPosition = new Vector3(-0.06f, 0.725f, -0.06f);
                phone.transform.localRotation = Quaternion.Euler(0, 15f, 0);
                EditorUtility.SetDirty(phone);
            }
            EditorUtility.SetDirty(director);
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}
