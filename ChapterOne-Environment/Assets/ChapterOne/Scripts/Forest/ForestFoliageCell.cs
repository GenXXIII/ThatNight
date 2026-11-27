using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne
{
    [ExecuteAlways]
    public sealed class ForestFoliageCell : MonoBehaviour
    {
        public Mesh mesh;
        public Material material;
        public Vector3[] positions;
        public Vector2[] rotationScale;
        public float distance = 180;

        Matrix4x4[] matrices;
        readonly Matrix4x4[] batch = new Matrix4x4[1023];
        Vector3 cachedCellPos;
        bool supportsInstancing;

        void OnEnable()
        {
            cachedCellPos = transform.position;
            supportsInstancing = SystemInfo.supportsInstancing;
            matrices = null;
            Camera.onPreCull += Render;
        }

        void OnDisable() => Camera.onPreCull -= Render;
        void OnValidate() { matrices = null; cachedCellPos = transform.position; }

        void Render(Camera camera)
        {
            if (!mesh || !material || positions == null || rotationScale == null || positions.Length != rotationScale.Length) return;

            Vector3 camPos = camera.transform.position;
            float dx = camPos.x - cachedCellPos.x;
            float dz = camPos.z - cachedCellPos.z;
            float cullR = distance + 80f;
            if (dx * dx + dz * dz > cullR * cullR) return;

            int len = positions.Length;
            if (matrices == null || matrices.Length != len)
            {
                matrices = new Matrix4x4[len];
                for (int i = 0; i < len; i++)
                    matrices[i] = Matrix4x4.TRS(positions[i], Quaternion.Euler(0, rotationScale[i].x, 0), Vector3.one * rotationScale[i].y);
            }

            float distSq = distance * distance;
            int layer = gameObject.layer;
            int count = 0;
            for (int i = 0; i < len; i++)
            {
                Vector3 p = positions[i];
                float px = p.x - camPos.x;
                float py = p.y - camPos.y;
                float pz = p.z - camPos.z;
                if (px * px + py * py + pz * pz > distSq) continue;

                batch[count++] = matrices[i];
                if (count == 1023)
                {
                    Draw(camera, count, layer);
                    count = 0;
                }
            }
            if (count > 0) Draw(camera, count, layer);
        }

        void Draw(Camera camera, int count, int layer)
        {
            if (supportsInstancing)
                Graphics.DrawMeshInstanced(mesh, 0, material, batch, count, null, ShadowCastingMode.Off, true, layer, camera, LightProbeUsage.Off);
            else
                for (int i = 0; i < count; i++)
                    Graphics.DrawMesh(mesh, batch[i], material, layer, camera, 0, null, ShadowCastingMode.Off, true, null, LightProbeUsage.Off);
        }
    }
}
