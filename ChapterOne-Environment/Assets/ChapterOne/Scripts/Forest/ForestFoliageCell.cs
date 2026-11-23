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
        void OnEnable() { Camera.onPreCull += Render; matrices = null; }
        void OnDisable() { Camera.onPreCull -= Render; }
        void OnValidate() { matrices = null; }
        void Render(Camera camera)
        {
            if (!mesh || !material || positions == null || rotationScale == null || positions.Length != rotationScale.Length) return;
            var delta = camera.transform.position - transform.position; delta.y = 0;
            if (delta.sqrMagnitude > (distance+80)*(distance+80)) return;
            if (matrices == null || matrices.Length != positions.Length)
            {
                matrices = new Matrix4x4[positions.Length];
                for (int i=0;i<matrices.Length;i++)
                    matrices[i]=Matrix4x4.TRS(positions[i],Quaternion.Euler(0,rotationScale[i].x,0),Vector3.one*rotationScale[i].y);
            }
            int count=0;
            for(int i=0;i<matrices.Length;i++)
            {
                var d=positions[i]-camera.transform.position;
                if(d.sqrMagnitude>distance*distance)continue;
                batch[count++]=matrices[i];
                if(count==1023){Draw(camera,count);count=0;}
            }
            if(count>0)Draw(camera,count);
        }
        void Draw(Camera camera,int count)
        {
            if(SystemInfo.supportsInstancing)
                Graphics.DrawMeshInstanced(mesh,0,material,batch,count,null,ShadowCastingMode.Off,true,gameObject.layer,camera,LightProbeUsage.Off);
            else for(int i=0;i<count;i++)
                Graphics.DrawMesh(mesh,batch[i],material,gameObject.layer,camera,0,null,ShadowCastingMode.Off,true,null,LightProbeUsage.Off);
        }
    }
}
