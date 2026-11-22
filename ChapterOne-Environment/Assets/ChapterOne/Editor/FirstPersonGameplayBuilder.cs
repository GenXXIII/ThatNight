using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    public static class FirstPersonGameplayBuilder
    {
        const string RootName="10 FIRST PERSON GAMEPLAY";
        [MenuItem("THAT NIGHT/Gameplay/Apply first person interactions to current scene")]
        public static void Apply()
        {
            if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Stop Play before applying gameplay.");
            var camera=Object.FindFirstObjectByType<EnvironmentCamera>();
            if(!camera||!camera.GetComponent<ForestExploration>())throw new System.InvalidOperationException("Apply the forest exploration patch first.");
            var existing=GameObject.Find(RootName);
            if(existing)throw new System.InvalidOperationException("Gameplay is already installed. Edit the existing components instead of duplicating the scene.");
            CharacterArtBuilder.Prepare();
            var root=new GameObject(RootName).transform;
            camera.tag="MainCamera";
            camera.GetComponent<Camera>().fieldOfView=65;camera.GetComponent<Camera>().nearClipPlane=.04f;
            var player=camera.GetComponent<PlayerInteraction>();if(!player)player=camera.gameObject.AddComponent<PlayerInteraction>();
            player.explorer=camera.GetComponent<ForestExploration>();player.body=CharacterArtBuilder.Player(camera,root);
            player.arrival=CharacterArtBuilder.Group("Wedding arrival",root,new Vector3(0,SiteLayout.HillElevation+1.72f,-17));
            player.arrival.rotation=Quaternion.identity;
            if(!camera.GetComponent<IntegratedGraphicsProfile>())camera.gameObject.AddComponent<IntegratedGraphicsProfile>();
            AddGuest(root,"Dara","Family greeter",new Vector3(-1.8f,21,-13.4f),155,new Color(.82f,.8f,.75f),
                "You made it! Your cousin will be glad you came. Come in and greet the family.",
                "The family is gathering for the wrist-tying ceremony inside the house. Take your time and look around.",
                "Follow the central aisle toward the house. The bedrooms are beyond the living room. I will stay here to welcome the guests.");
            AddGuest(root,"Sokha","Wedding helper",new Vector3(5,21,-9),210,new Color(.52f,.63f,.67f),
                "Welcome, cousin. I am helping with the wedding preparations today.",
                "Everyone has brought something for the ceremony. Please leave the fruit trays ready for the family.",
                "There is a welcome note and a cup near the entrance. You can pick them up to take a closer look.");
            AddGuest(root,"Vannak","Kitchen helper",new Vector3(13,21,18),240,new Color(.65f,.63f,.51f),
                "Hello! We are getting the food ready. Your family has kept us busy today.",
                "We will bring out the meal after the ceremony. The cooking area is behind the house.",
                "Follow the side path to the rear kitchen. Keep the narrow path clear so we can carry the pots safely.");
            AddGuest(root,"Rith","Family neighbour",new Vector3(-8,21,-16),110,new Color(.64f,.54f,.49f),
                "It is good to see you back in the village. You have arrived before dark.",
                "Your cousin's wedding has brought everyone together. I have known the family for years.",
                "The forest gets very dark after sunset. Keep your flashlight close, and follow the road when you return.");
            AddPickups(root);
            int openings=GameplayOpeningBuilder.Apply();
            camera.transform.SetPositionAndRotation(player.arrival.position,player.arrival.rotation);
            camera.lighting.ApplyStage(AtmosphereStage.AfternoonCelebration);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            Debug.Log("FIRST_PERSON_APPLIED guests=4 pickups=3 openings="+openings);
        }

        [MenuItem("THAT NIGHT/Gameplay/Spawn Bride in Bedroom (4-00 AM Opening)")]
        public static void SetBrideBedroomOpening()
        {
            var camera = Object.FindFirstObjectByType<EnvironmentCamera>();
            if (!camera) return;
            var player = camera.GetComponent<PlayerInteraction>();
            if (!player)
            {
                Apply();
                player = camera.GetComponent<PlayerInteraction>();
            }

            // Bridal bedroom position: inside stilt house beside the bridal canopy bed
            Vector3 bedroomPos = SiteLayout.HousePoint(new Vector3(-3.45f, 2.5f, 4.2f));
            Quaternion bedroomRot = SiteLayout.HouseRotation * Quaternion.Euler(0, 160f, 0);

            if (player.arrival != null)
            {
                player.arrival.SetPositionAndRotation(bedroomPos, bedroomRot);
            }
            camera.transform.SetPositionAndRotation(bedroomPos, bedroomRot);
            camera.lighting.ApplyStage(AtmosphereStage.Dawn4AM);

            if (!camera.GetComponent<ChapterOneNarrative>())
                camera.gameObject.AddComponent<ChapterOneNarrative>();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            Debug.Log("BRIDE_BEDROOM_SPAWN_APPLIED at 4:00 AM Dawn: pos=" + bedroomPos);
        }
        static void AddGuest(Transform parent,string name,string role,Vector3 position,float yaw,Color shirt,string greeting,string wedding,string help)
        {
            var root=CharacterArtBuilder.Group(name+" - "+role,parent,position);root.eulerAngles=new Vector3(0,yaw,0);
            // Set feet on the actual authored ground, excluding canopies and roofs.
            if(Physics.Raycast(position+Vector3.up*.8f,Vector3.down,out var hit,4))root.position=hit.point+Vector3.up*.02f;
            var guest=CharacterArtBuilder.Guest(root,name,shirt);guest.role=role;guest.greeting=greeting;guest.weddingAnswer=wedding;guest.helpAnswer=help;
            Directory.CreateDirectory("Assets/ChapterOne/Gameplay/Prefabs");
            PrefabUtility.SaveAsPrefabAssetAndConnect(root.gameObject,"Assets/ChapterOne/Gameplay/Prefabs/Guest "+name+".prefab",InteractionMode.AutomatedAction);
        }
        static void AddPickups(Transform parent)
        {
            var stand=CharacterArtBuilder.Group("Entrance welcome stand",parent,new Vector3(2.2f,21,-15.4f));
            var wood=CharacterArtBuilder.Mat("Welcome stand wood",new Color(.27f,.16f,.085f));
            var top=CharacterArtBuilder.Part("Welcome stand top",stand,new Vector3(0,.78f,0),new Vector3(.85f,.06f,.48f),wood,PrimitiveType.Cube);
            top.gameObject.AddComponent<BoxCollider>();
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)
                CharacterArtBuilder.Part("Stand leg",stand,new Vector3(x*.35f,.38f,z*.18f),new Vector3(.055f,.76f,.055f),wood,PrimitiveType.Cube);
            var paper=CharacterArtBuilder.Mat("Warm paper",new Color(.88f,.82f,.66f));
            var ceramic=CharacterArtBuilder.Mat("Tea cup ceramic",new Color(.63f,.71f,.68f));
            AddPickup(stand,"Wedding welcome note",new Vector3(-.22f,.821f,0),new Vector3(.20f,.006f,.25f),paper,PrimitiveType.Cube,new Vector3(0,.035f,.05f));
            AddPickup(stand,"Tea cup",new Vector3(.10f,.875f,0),new Vector3(.09f,.065f,.09f),ceramic,PrimitiveType.Cylinder,new Vector3(0,.07f,.02f));
            var key=CharacterArtBuilder.Group("Spare key",stand,new Vector3(.30f,.825f,0));
            CharacterArtBuilder.Part("Key stem",key,Vector3.zero,new Vector3(.012f,.008f,.09f),CharacterArtBuilder.Metal,PrimitiveType.Cube);
            CharacterArtBuilder.Part("Key bow",key,new Vector3(0,0,-.05f),new Vector3(.035f,.007f,.035f),CharacterArtBuilder.Metal,PrimitiveType.Sphere);
            CharacterArtBuilder.Part("Key teeth",key,new Vector3(.01f,0,.025f),new Vector3(.024f,.008f,.025f),CharacterArtBuilder.Metal,PrimitiveType.Cube);
            var c=key.gameObject.AddComponent<BoxCollider>();c.size=new Vector3(.05f,.02f,.13f);
            var pickup=key.gameObject.AddComponent<PickupItem>();pickup.displayName="spare key";pickup.gripPosition=new Vector3(0,.03f,.04f);
        }
        static void AddPickup(Transform stand,string name,Vector3 position,Vector3 size,Material mat,PrimitiveType shape,Vector3 grip)
        {
            var item=CharacterArtBuilder.Part(name,stand,position,size,mat,shape);
            item.gameObject.AddComponent<BoxCollider>();var pickup=item.gameObject.AddComponent<PickupItem>();pickup.displayName=name;pickup.gripPosition=grip;
        }
        [MenuItem("THAT NIGHT/Gameplay/Build current gameplay preview")]
        public static void BuildPreview()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
            Directory.CreateDirectory("Builds/ChapterOneGameplay");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{SceneManager.GetActiveScene().path},locationPathName="Builds/ChapterOneGameplay/THAT NIGHT.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
            Debug.Log("GAMEPLAY_BUILD "+report.summary.result+" bytes="+report.summary.totalSize);
            if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Gameplay build failed.");
        }
    }
}
