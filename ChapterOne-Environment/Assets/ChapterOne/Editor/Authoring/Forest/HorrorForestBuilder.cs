using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace ThatNight.ChapterOne.Editor
{
    public static class HorrorForestBuilder
    {
        const string RootName = "09 HORROR FOREST - ecology and exploration";
        const string AssetsRoot = "Assets/ChapterOne/Forest";
        static readonly Vector2 Sacred = new(-880, 1020);
        static readonly Vector2[] Trail = {
            new(-32, 45), new(-65, 70), new(-130, 115), new(-180, 180),
            new(-280, 240), new(-390, 350), new(-470, 480), new(-600, 600),
            new(-710, 690), new(-830, 760), new(-880, 850), new(-880, 970)
        };

        [MenuItem("THAT NIGHT/Forest/Apply horror forest to current scene")]
        public static void ApplyAndSave()
        {
            if (Application.isPlaying) throw new InvalidOperationException("Stop Play before authoring the forest.");
            // Installation only: existing authored forest objects are never replaced.
            Apply();
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }

        public static void Apply()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var inspection = UnityEngine.Object.FindFirstObjectByType<EnvironmentCamera>();
            if (!inspection) throw new InvalidOperationException("The environment review camera is required.");
            var old = GameObject.Find(RootName);
            if (old) throw new InvalidOperationException("Forest already exists. Edit its objects; setup will not replace your authored forest.");
            Directory.CreateDirectory(AssetsRoot + "/Meshes");
            Directory.CreateDirectory(AssetsRoot + "/Materials");
            var root = new GameObject(RootName).transform;
            var fern = Material("Moonlit fern foliage", "ThatNight/ForestFoliage", new Color(.27f,.39f,.24f));
            var bark = CopyMaterial("Wet fallen timber", "Dark hardwood", new Color(.43f,.45f,.36f));
            var rock = CopyMaterial("Mossy forest stone", "Rough bark", new Color(.46f,.49f,.44f));
            rock.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Textures/Ground037/Ground037_1K-JPG_Color.jpg");
            var litter = CopyMaterial("Forest leaf litter", "Dark hardwood", new Color(.28f,.25f,.16f));
            var previousRandom = Random.state; Random.InitState(21112006);
            int cellCount = 0, fernCount = 0;
            var prototype=new ForestMesh();Fern(prototype,Vector3.zero,1);Grass(prototype,Vector3.zero,1);
            string prototypePath=AssetsRoot+"/Meshes/Fern prototype.asset";
            var fernMesh=prototype.Create("Sword fern and forest grass",Vector3.zero);
            var savedFern=AssetDatabase.LoadAssetAtPath<Mesh>(prototypePath);
            if(savedFern){EditorUtility.CopySerialized(fernMesh,savedFern);UnityEngine.Object.DestroyImmediate(fernMesh);fernMesh=savedFern;}
            else AssetDatabase.CreateAsset(fernMesh,prototypePath);
            AssetDatabase.StartAssetEditing();
            try
            {
                // Covers the existing west, north, east foothills and the sacred glade.
                // No extra canopy trees are introduced inside the sacred 300m exclusion.
                for (int x = -1740; x < 180; x += 96)
                for (int z = -140; z < 1840; z += 96)
                {
                    var center = Point(x + 48, z + 48);
                    var plants = new List<Vector3>(); var wood = new ForestMesh(); var stones = new ForestMesh(); var debris = new ForestMesh();
                    var cell = new GameObject($"Forest floor [{x}, {z}]").transform;
                    cell.SetParent(root); cell.position = center;
                    for (int p = 0; p < 38; p++)
                    {
                        var at = Point(x + Random.Range(2f,94f), z + Random.Range(2f,94f));
                        if (!Allowed(at, 3.2f)) continue;
                        float size = Random.Range(.65f, 1.4f);
                        for (int f = 0; f < 3; f++)
                        {
                            var fp = Point(at.x + Random.Range(-2.5f,2.5f), at.z + Random.Range(-2.5f,2.5f));
                            if (!Allowed(fp, 2.6f)) continue;
                            plants.Add(fp); fernCount++;
                        }
                        for (int d = 0; d < 6; d++)
                        {
                            var lp = Point(at.x + Random.Range(-3f,3f), at.z + Random.Range(-3f,3f)) + Vector3.up * .025f;
                            float a = Random.value * 6.28f;
                            var dir = new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));
                            debris.Leaf(lp, lp+dir*Random.Range(.18f,.38f), Vector3.Cross(dir,Vector3.up)*.06f, Color.white);
                        }
                        if (p % 7 == 0)
                        {
                            var scale = new Vector3(Random.Range(.7f,1.8f),Random.Range(.3f,.85f),Random.Range(.8f,2f));
                            stones.Rock(at + Vector3.up * scale.y * .25f, scale, p);
                            var obstacle = new GameObject("Mossy stone collision").transform;
                            obstacle.SetParent(cell); obstacle.position=at;
                            var collider=obstacle.gameObject.AddComponent<BoxCollider>(); collider.center=Vector3.up*scale.y*.35f; collider.size=new Vector3(scale.x*1.5f,scale.y*1.3f,scale.z*1.5f);
                        }
                        if (p % 11 == 0)
                        {
                            float a=Random.value*6.28f;
                            var end=Point(at.x+Mathf.Sin(a)*5,at.z+Mathf.Cos(a)*5);
                            if (!Allowed(end,3.2f))continue;
                            var a0=at+Vector3.up*.25f; var b0=end+Vector3.up*.22f;
                            wood.Rod(a0,b0,.32f,.2f,new Color(.75f,.83f,.7f),9);
                            wood.Rod(Vector3.Lerp(a0,b0,.45f),Vector3.Lerp(a0,b0,.45f)+new Vector3(.6f,1.1f,.3f),.11f,.035f,Color.white);
                            AddLogCollider(cell,a0,b0);
                        }
                        if (p == 5 && Vector2.Distance(new Vector2(at.x,at.z),Sacred)>310 && new Vector2(at.x,at.z-15).magnitude>75)
                        {
                            var top=at+new Vector3(.6f,Random.Range(6f,10f),.5f);
                            wood.Rod(at,top,.32f,.1f,Color.white);
                            wood.Rod(Vector3.Lerp(at,top,.6f),Vector3.Lerp(at,top,.6f)+new Vector3(2,1.5f,.5f),.13f,.025f,Color.white);
                            var snag=new GameObject("Broken snag collision").transform;snag.SetParent(cell);snag.position=at;
                            var capsule=snag.gameObject.AddComponent<CapsuleCollider>();capsule.height=6;capsule.radius=.34f;capsule.center=Vector3.up*3;
                        }
                    }
                    if (plants.Count == 0) { UnityEngine.Object.DestroyImmediate(cell.gameObject); continue; }
                    var renderers = new List<Renderer>();
                    AddPlants(cell,plants,fernMesh,fern); Emit(cell,wood,bark,"Deadwood",renderers);
                    Emit(cell,stones,rock,"Moss stones",renderers); Emit(cell,debris,litter,"Leaf litter",renderers);
                    var culling=cell.gameObject.AddComponent<ForestDetailCell>();culling.details=renderers.ToArray();culling.visibleDistance=205;
                    cellCount++;
                }
                // Rich foreground at the existing forest edge, so the first walk has readable detail.
                var edge=new GameObject("Forest edge fern beds").transform;edge.SetParent(root);edge.position=Point(-80,100);
                var edgePlants=new List<Vector3>();
                for (int i=0;i<1100;i++)
                {
                    var at=Point(Random.Range(-220f,-30f),Random.Range(45f,220f));
                    if(!Allowed(at,2.6f))continue;
                    edgePlants.Add(at);fernCount++;
                }
                AddPlants(edge,edgePlants,fernMesh,fern);
                AddViewpoints(root,inspection);
                EnsureTerrainCollisions();
                int trunks=ForestTreeCollision.Add(root);
                var trunkMaterial=Material("Forest trunk bark","ThatNight/ForestBark",new Color(.48f,.52f,.45f));
                trunkMaterial.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Textures/Wood066/Wood066_1K-JPG_Color.jpg");
                var village=GameObject.Find("05 VILLAGE - wooded hometown with scattered houses");
                if(village)foreach(var renderer in village.GetComponentsInChildren<MeshRenderer>())
                {
                    // Only forest tree meshes; road fence, house and wedding timber retain their materials.
                    if(!renderer.name.Contains("Tree")&&!renderer.name.Contains("Forest")&&!renderer.name.Contains("Chunk"))continue;
                    if(renderer.name.Contains("Roadside"))continue;
                    var materials=renderer.sharedMaterials;
                    for(int i=0;i<materials.Length;i++)if(materials[i]&&(materials[i].name=="Dark hardwood"||materials[i].name=="Forest trunk bark"))materials[i]=trunkMaterial;
                    renderer.sharedMaterials=materials;
                }
                inspection.lighting.ApplyStage(AtmosphereStage.NightHorror);
                QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowDistance=60;
                EditorSceneManager.MarkSceneDirty(scene);
                Debug.Log($"HORROR_FOREST cells={cellCount} ferns={fernCount} distantTrunkColliders={trunks} sacred clearing preserved; existing architecture retained.");
            }
            finally { Random.state=previousRandom; AssetDatabase.StopAssetEditing(); }
        }

        static void AddPlants(Transform cell,List<Vector3> plants,Mesh mesh,Material material)
        {
            var data=cell.gameObject.AddComponent<ForestFoliageCell>();data.mesh=mesh;data.material=material;
            data.positions=plants.ToArray();data.rotationScale=new Vector2[plants.Count];
            for(int i=0;i<plants.Count;i++)data.rotationScale[i]=new Vector2(Random.Range(0,360f),Random.Range(.65f,1.4f));
        }

        public static void EnsureTerrainCollisions()
        {
            var ground=GameObject.Find("04 GROUND - unified soil terrain and approach road");
            if(!ground)return;
            foreach(var filter in ground.GetComponentsInChildren<MeshFilter>())
            {
                if(!filter.name.StartsWith("Terrain Region Chunk"))continue;
                // The original inspection scene used bounding boxes spanning whole mountains.
                // Walking needs the actual ground surface, not those volume bounds.
                var box=filter.GetComponent<BoxCollider>();if(box)box.enabled=false;
                var collider=filter.GetComponent<MeshCollider>();if(!collider)collider=filter.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh=filter.sharedMesh;
            }
        }

        public static void BuildCurrentPreview() => EnvironmentBuild.Build();

        static Vector3 Point(float x,float z)=>new(x,ForestTerrainLayout.HillHeight(x,z),z);
        static bool Allowed(Vector3 p,float trailMargin)
        {
            if(p.y>210 || ForestTerrainLayout.IsInsideWeddingOrHouseZone(p.x,p.z,9))return false;
            if(p.z<25 && ForestTerrainLayout.DistanceToHillRoad(new Vector2(p.x,p.z))<16)return false;
            if(Mathf.Abs(ForestTerrainLayout.HillHeight(p.x+2,p.z)-p.y)>1.9f || Mathf.Abs(ForestTerrainLayout.HillHeight(p.x,p.z+2)-p.y)>1.9f)return false;
            var q=new Vector2(p.x,p.z);
            for(int i=1;i<Trail.Length;i++)
            {
                var d=Trail[i]-Trail[i-1];
                if(Vector2.Distance(q,Trail[i-1]+d*Mathf.Clamp01(Vector2.Dot(q-Trail[i-1],d)/d.sqrMagnitude))<trailMargin)return false;
            }
            return true;
        }
        static void Fern(ForestMesh mesh,Vector3 p,float size)
        {
            for(int f=0;f<6;f++)
            {
                float a=f*Mathf.PI/3+Random.value*.5f;
                var dir=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var side=Vector3.Cross(dir,Vector3.up);
                float length=Random.Range(1.0f,1.6f)*size;
                for(int l=1;l<=7;l++)
                {
                    float t=l/8f;var stem=p+dir*t*length+Vector3.up*(Mathf.Sin(t*2.5f)*.7f*size+.06f);
                    float width=Mathf.Sin(t*Mathf.PI)*length*.28f;
                    var tint=Color.Lerp(new Color(.6f,.72f,.53f),new Color(1.05f,1.12f,.85f),t);
                    mesh.Leaf(stem,stem+side*width+dir*.15f*size,dir*.075f*size,tint);
                    mesh.Leaf(stem,stem-side*width+dir*.15f*size,dir*.075f*size,tint);
                    var next=p+dir*(t+.125f)*length+Vector3.up*(Mathf.Sin((t+.125f)*2.5f)*.7f*size+.06f);
                    mesh.Rod(stem,next,.013f*size,.009f*size,tint,3);
                }
            }
        }
        static void Grass(ForestMesh mesh,Vector3 p,float size)
        {
            for(int g=0;g<8;g++)
            {
                float a=Random.value*6.28f;var d=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));
                var basePoint=p+d*Random.Range(.1f,.5f);
                mesh.Leaf(basePoint,basePoint+d*.35f+Vector3.up*Random.Range(.3f,.9f)*size,Vector3.Cross(d,Vector3.up)*.025f,new Color(.65f,.75f,.43f));
            }
        }
        static void AddLogCollider(Transform parent,Vector3 a,Vector3 b)
        {
            var go=new GameObject("Fallen timber collision");go.transform.SetParent(parent);go.transform.position=(a+b)*.5f;
            go.transform.rotation=Quaternion.FromToRotation(Vector3.up,(b-a).normalized);
            var c=go.AddComponent<CapsuleCollider>();c.radius=.27f;c.height=Vector3.Distance(a,b)+.4f;
        }
        static void Emit(Transform cell,ForestMesh data,Material material,string name,List<Renderer> renderers)
        {
            if(data.VertexCount==0)return;
            string path=$"{AssetsRoot}/Meshes/{cell.name.Replace('[','_').Replace(']','_')}_{name}.asset";
            var mesh=data.Create(name,cell.position);var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(saved){EditorUtility.CopySerialized(mesh,saved);UnityEngine.Object.DestroyImmediate(mesh);}else{saved=mesh;AssetDatabase.CreateAsset(saved,path);}
            var go=new GameObject(name);go.transform.SetParent(cell,false);go.isStatic=false;
            go.AddComponent<MeshFilter>().sharedMesh=saved;
            var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;
            renderer.shadowCastingMode=name=="Ferns"||name=="Leaf litter"?ShadowCastingMode.Off:ShadowCastingMode.On;
            renderers.Add(renderer);
        }
        static Material Material(string name,string shader,Color tint)
        {
            string path=$"{AssetsRoot}/Materials/{name}.mat";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(!mat){mat=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(mat,path);}
            mat.color=tint;mat.enableInstancing=true;EditorUtility.SetDirty(mat);return mat;
        }
        static Material CopyMaterial(string name,string source,Color tint)
        {
            string path=$"{AssetsRoot}/Materials/{name}.mat";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(!mat){mat=new Material(AssetDatabase.LoadAssetAtPath<Material>($"Assets/ChapterOne/Materials/{source}.mat"));AssetDatabase.CreateAsset(mat,path);}
            mat.color=tint;mat.SetFloat("_Glossiness",.24f);mat.enableInstancing=true;EditorUtility.SetDirty(mat);return mat;
        }
        static void AddViewpoints(Transform root,EnvironmentCamera inspection)
        {
            var explorer=inspection.GetComponent<ForestExploration>();if(!explorer)explorer=inspection.gameObject.AddComponent<ForestExploration>();
            var points=new List<Transform>();
            string[] names={"Forest edge","Deep forest","Western wilderness","Sacred tree approach","Northern foothills"};
            Vector2[] places={new(-65,70),new(-180,180),new(-600,600),new(-880,800),new(-380,1430)};
            float[] angles={315,310,310,0,320};
            for(int i=0;i<places.Length;i++)
            {
                var t=new GameObject(names[i]).transform;t.SetParent(root);t.position=Point(places[i].x,places[i].y)+Vector3.up*1.72f;t.rotation=Quaternion.Euler(2,angles[i],0);points.Add(t);
            }
            explorer.viewpoints=points.ToArray();
            var lamp=inspection.transform.Find("Forest flashlight");
            if(!lamp){lamp=new GameObject("Forest flashlight").transform;lamp.SetParent(inspection.transform,false);}
            lamp.localPosition=new Vector3(.18f,-.12f,.15f);lamp.localRotation=Quaternion.identity;
            var light=lamp.GetComponent<Light>();if(!light)light=lamp.gameObject.AddComponent<Light>();
            light.type=LightType.Spot;light.color=new Color(1,.91f,.79f);light.intensity=3.2f;light.range=18;
            light.spotAngle=48;light.innerSpotAngle=22;light.shadows=LightShadows.Soft;light.shadowResolution=LightShadowResolution.Medium;light.enabled=false;
            explorer.flashlight=light;
            explorer.wind=AssetDatabase.LoadAssetAtPath<AudioClip>(AssetsRoot+"/Audio/ForestWind.wav");
            explorer.insects=AssetDatabase.LoadAssetAtPath<AudioClip>(AssetsRoot+"/Audio/NightInsects.wav");
        }
    }
}
