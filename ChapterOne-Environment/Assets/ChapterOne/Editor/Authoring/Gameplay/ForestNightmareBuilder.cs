// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// ForestNightmareBuilder.cs — Single Authoritative Setup for Forest Nightmare
//
// Places the Forest Nightmare Opening cleanly inside the active scene:
// - Removes any duplicate Player visible body or old Forest Nightmare objects.
// - Places ForestFlashlightPickup on the ground at the deep-forest wake spot.
// - Places subtle beeswax candles & jasmine petals along the winding S-curve bends.
// - Places ShadowGhostWoman beneath the Colossal Sacred World Tree facing the trunk
//   (so the approaching Bride sees ONLY her shadowy back until she turns in rage).
// - Wires all references and audio clips onto ChapterOneDirector cleanly.
// ============================================================================

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    public static class ForestNightmareBuilder
    {
        const string DirectorRootName = "12 CHAPTER ONE STORY DIRECTOR";
        const string NightmareRootName = "Forest Nightmare Opening";

        [MenuItem("THAT NIGHT/Chapter 1/Setup Forest Nightmare Opening")]
        public static void ApplyAndSave()
        {
            if (EditorApplication.isPlaying)
                throw new System.InvalidOperationException("Stop Play before setting up the Forest Nightmare opening.");

            Apply();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }

        public static void Apply()
        {
            var director = Object.FindFirstObjectByType<ChapterOneDirector>();
            if (!director)
                throw new System.InvalidOperationException("ChapterOneDirector is missing from the scene.");

            var directorRoot = GameObject.Find(DirectorRootName);
            if (!directorRoot)
                throw new System.InvalidOperationException(DirectorRootName + " is missing from the scene.");

            // 0. Clean up any duplicate "Player visible body" objects in the scene!
            int removedBodies = CleanupDuplicatePlayerBodies(directorRoot.transform);

            // Remove ALL existing Forest Nightmare Opening roots so zero duplicates can accumulate
            foreach (var existing in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (existing && existing.name == NightmareRootName)
                    Object.DestroyImmediate(existing.gameObject);
            }

            var root = new GameObject(NightmareRootName).transform;
            root.SetParent(directorRoot.transform, false);

            // 1. Flashlight pickup lying on the dark forest floor beside Bride's wake-up spot
            Vector2 spawnXZ = ChapterOneDirector.ForestNightmareTrail[0];
            Vector2 bend1XZ = ChapterOneDirector.ForestNightmareTrail[1];
            Vector3 toBend1 = new Vector3(bend1XZ.x - spawnXZ.x, 0f, bend1XZ.y - spawnXZ.y).normalized;
            Vector3 rightDir = Vector3.Cross(Vector3.up, toBend1).normalized;
            float wakeYaw = Mathf.Atan2(toBend1.x, toBend1.z) * Mathf.Rad2Deg;

            Vector3 flashXZ = new Vector3(spawnXZ.x, 0f, spawnXZ.y) + toBend1 * 0.95f + rightDir * 0.28f;
            Vector3 flashPos = SnapToGround(flashXZ.x, flashXZ.z) + Vector3.up * 0.045f;
            var flashlightPickup = BuildFlashlightPickup(root, flashPos, Quaternion.Euler(0f, wakeYaw + 32f, 90f));

            // 2. Subtle eerie Cambodian candle & jasmine markers along the winding left-right bends
            BuildWindingTrailMarkers(root);

            // 3. Shadow Ghost Woman in the open moonlit glade bay beneath the Colossal Sacred Tree (facing trunk)
            Vector3 ghostPos = SnapToGround(ChapterOneDirector.SacredTreeGhostPos.x, ChapterOneDirector.SacredTreeGhostPos.z);
            var ghost = BuildShadowGhostWoman(root, ghostPos);

            // 4. Wire serialized references onto ChapterOneDirector
            Undo.RecordObject(director, "Wire Forest Nightmare Opening");
            director.currentScene = GameScene.Scene00_ForestNightmare;
            director.forestNightmareRoot = root;
            director.forestFlashlight = flashlightPickup;
            director.ghostWoman = ghost;
            director.clipWomanCrying = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/WomanCrying.wav");
            director.clipNeckCrack = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NeckCrack.wav");
            director.clipShockGasp = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/ShockGasp.wav");
            if (!director.clipRumble) director.clipRumble = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/DeepRumble.wav");
            if (!director.clipScream) director.clipScream = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/Scream.wav");
            if (!director.clipRooster) director.clipRooster = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/DistantRooster.wav");

            // Keep nightmare props deactivated in edit mode until Scene00 starts
            root.gameObject.SetActive(false);

            EditorUtility.SetDirty(director);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log($"FOREST_NIGHTMARE_SETUP: Cleanly wired flashlight, winding trail, ghost, and audio (removed {removedBodies} duplicate player body).");
        }

        static int CleanupDuplicatePlayerBodies(Transform directorRoot)
        {
            var fpBody = Object.FindFirstObjectByType<FirstPersonBody>(FindObjectsInactive.Include);
            Transform keep = fpBody ? fpBody.lowerBody : null;
            int removed = 0;

            var bodies = new System.Collections.Generic.List<GameObject>();
            foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (t && t.name == "Player visible body")
                    bodies.Add(t.gameObject);
            }

            if (keep == null && bodies.Count > 0) keep = bodies[0].transform;

            foreach (var go in bodies)
            {
                if (keep != null && go.transform == keep)
                {
                    go.SetActive(false);
                    continue;
                }
                Object.DestroyImmediate(go);
                removed++;
            }

            if (fpBody && keep != null)
            {
                fpBody.lowerBody = keep;
                EditorUtility.SetDirty(fpBody);
            }
            return removed;
        }

        static Vector3 SnapToGround(float x, float z)
        {
            if (Physics.Raycast(new Vector3(x, 120f, z), Vector3.down, out var hit, 200f, ~0, QueryTriggerInteraction.Ignore))
                return hit.point;
            return new Vector3(x, 20.88f, z);
        }

        static Material EnsureMat(string name, Color color, float smoothness = 0.1f, Color? emission = null)
        {
            string folder = "Assets/ChapterOne/Gameplay/Materials/";
            System.IO.Directory.CreateDirectory(folder);
            string path = folder + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!mat)
            {
                mat = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = color;
            mat.SetFloat("_Glossiness", smoothness);
            mat.SetFloat("_Metallic", 0f);
            if (emission.HasValue)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emission.Value);
            }
            EditorUtility.SetDirty(mat);
            return mat;
        }

        static void BuildWindingTrailMarkers(Transform parent)
        {
            var trailRoot = new GameObject("Winding Trail Bend Markers").transform;
            trailRoot.SetParent(parent, false);

            var candleMat = EnsureMat("Nightmare Red Candle", new Color(0.68f, 0.10f, 0.10f), 0.35f);
            var flameMat = EnsureMat("Nightmare Flame", new Color(1.0f, 0.72f, 0.24f), 0f, new Color(2.4f, 1.1f, 0.35f));
            var jasmineMat = EnsureMat("Nightmare Clue Jasmine", new Color(0.95f, 0.94f, 0.88f), 0.1f, new Color(0.08f, 0.08f, 0.06f));

            var trail = ChapterOneDirector.ForestNightmareTrail;
            for (int i = 1; i <= 5 && i < trail.Length; i++)
            {
                Vector2 wp = trail[i];
                Vector3 pos = SnapToGround(wp.x, wp.y);

                var bendGo = new GameObject($"Trail Bend Marker {i}");
                bendGo.transform.SetParent(trailRoot, false);
                bendGo.transform.position = pos;

                var candle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                candle.name = "Ritual Candle";
                candle.transform.SetParent(bendGo.transform, false);
                candle.transform.localPosition = new Vector3(0f, 0.10f, 0f);
                candle.transform.localScale = new Vector3(0.05f, 0.10f, 0.05f);
                candle.GetComponent<Renderer>().sharedMaterial = candleMat;
                Object.DestroyImmediate(candle.GetComponent<Collider>());

                var flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                flame.name = "Candle Flame";
                flame.transform.SetParent(bendGo.transform, false);
                flame.transform.localPosition = new Vector3(0f, 0.23f, 0f);
                flame.transform.localScale = new Vector3(0.03f, 0.055f, 0.03f);
                flame.GetComponent<Renderer>().sharedMaterial = flameMat;
                Object.DestroyImmediate(flame.GetComponent<Collider>());

                var lightGo = new GameObject("Candle Glow");
                lightGo.transform.SetParent(bendGo.transform, false);
                lightGo.transform.localPosition = new Vector3(0f, 0.38f, 0f);
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = new Color(1.0f, 0.56f, 0.22f);
                light.range = 8.5f;
                light.intensity = 1.35f;
                light.shadows = LightShadows.None;

                Vector2 nextWp = trail[Mathf.Min(i + 1, trail.Length - 1)];
                Vector3 nextDir = new Vector3(nextWp.x - wp.x, 0f, nextWp.y - wp.y).normalized;
                Vector3 perp = Vector3.Cross(Vector3.up, nextDir);
                for (int p = 0; p < 5; p++)
                {
                    var petal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    petal.name = "Jasmine Petal " + p;
                    petal.transform.SetParent(bendGo.transform, false);
                    float side = ((p % 2 == 0) ? 1f : -1f) * (0.22f + p * 0.10f);
                    petal.transform.localPosition = nextDir * (0.55f + p * 0.95f) + perp * side + Vector3.up * 0.025f;
                    petal.transform.localScale = new Vector3(0.075f, 0.02f, 0.075f);
                    petal.GetComponent<Renderer>().sharedMaterial = jasmineMat;
                    Object.DestroyImmediate(petal.GetComponent<Collider>());
                }
            }
        }

        static ForestFlashlightPickup BuildFlashlightPickup(Transform parent, Vector3 pos, Quaternion rot)
        {
            var go = new GameObject("Forest Nightmare Flashlight");
            go.transform.SetParent(parent, false);
            go.transform.SetPositionAndRotation(pos, rot);

            var metalMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/Flashlight black metal.mat")
                        ?? EnsureMat("Nightmare Flashlight Body", new Color(0.08f, 0.09f, 0.10f), 0.55f);
            var lensMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/Flashlight Lens Glass.mat")
                       ?? EnsureMat("Nightmare Flashlight Lens", new Color(0.85f, 0.88f, 0.80f), 0.9f, new Color(0.55f, 0.50f, 0.35f));

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.name = "Flashlight Handle";
            barrel.transform.SetParent(go.transform, false);
            barrel.transform.localPosition = Vector3.zero;
            barrel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            barrel.transform.localScale = new Vector3(0.044f, 0.11f, 0.044f);
            barrel.GetComponent<Renderer>().sharedMaterial = metalMat;
            Object.DestroyImmediate(barrel.GetComponent<Collider>());

            var bezel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bezel.name = "Flashlight Head";
            bezel.transform.SetParent(go.transform, false);
            bezel.transform.localPosition = new Vector3(0f, 0f, 0.12f);
            bezel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            bezel.transform.localScale = new Vector3(0.064f, 0.03f, 0.064f);
            bezel.GetComponent<Renderer>().sharedMaterial = metalMat;
            Object.DestroyImmediate(bezel.GetComponent<Collider>());

            var lens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lens.name = "Flashlight Lens";
            lens.transform.SetParent(go.transform, false);
            lens.transform.localPosition = new Vector3(0f, 0f, 0.148f);
            lens.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            lens.transform.localScale = new Vector3(0.052f, 0.004f, 0.052f);
            lens.GetComponent<Renderer>().sharedMaterial = lensMat;
            Object.DestroyImmediate(lens.GetComponent<Collider>());

            var box = go.AddComponent<BoxCollider>();
            box.size = new Vector3(0.32f, 0.28f, 0.42f);
            box.center = new Vector3(0f, 0f, 0.04f);

            var glintObj = new GameObject("Flashlight Glint Light");
            glintObj.transform.SetParent(go.transform, false);
            glintObj.transform.localPosition = new Vector3(0f, 0.20f, 0.14f);
            var glint = glintObj.AddComponent<Light>();
            glint.type = LightType.Point;
            glint.color = new Color(1.0f, 0.88f, 0.65f);
            glint.range = 3.8f;
            glint.intensity = 1.25f;
            glint.shadows = LightShadows.None;

            // Single authoritative SpotLight attached to the held flashlight (explorer.flashlight stays OFF)
            var beamObj = new GameObject("Flashlight Beam");
            beamObj.transform.SetParent(go.transform, false);
            beamObj.transform.localPosition = new Vector3(0f, 0f, 0.16f);
            beamObj.transform.localRotation = Quaternion.identity;
            var beam = beamObj.AddComponent<Light>();
            beam.type = LightType.Spot;
            beam.color = new Color(1.0f, 0.92f, 0.80f);
            beam.range = 32f;
            beam.intensity = 4.2f;
            beam.spotAngle = 54f;
            beam.innerSpotAngle = 24f;
            beam.shadows = LightShadows.Soft;
            beam.enabled = false;

            var pickup = go.AddComponent<ForestFlashlightPickup>();
            pickup.groundGlintLight = glint;
            pickup.beamLight = beam;
            return pickup;
        }

        static ShadowGhostWoman BuildShadowGhostWoman(Transform parent, Vector3 pos)
        {
            var root = new GameObject("Shadow Ghost Woman");
            root.transform.SetParent(parent, false);
            root.transform.position = pos;
            // Face North-Northeast toward the giant tree trunk so ONLY her back is visible while crying
            root.transform.rotation = Quaternion.Euler(0f, 12f, 0f);

            var ghost = root.AddComponent<ShadowGhostWoman>();
            ghost.cryingClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/WomanCrying.wav");
            ghost.neckCrackClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NeckCrack.wav");
            ghost.screamClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/Scream.wav");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ChapterOne/Characters/Mother/Female_Adult_01.fbx");
            if (prefab != null)
            {
                var inst = Object.Instantiate(prefab, root.transform);
                inst.name = "Shadow Silhouette Model";
                inst.transform.localPosition = Vector3.zero;
                inst.transform.localRotation = Quaternion.identity;
                ghost.modelRoot = inst.transform;

                var matPath = "Assets/ChapterOne/Gameplay/Materials/GhostShadowBody.mat";
                var bodyShadowMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (!bodyShadowMat)
                {
                    var shader = Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
                    bodyShadowMat = new Material(shader);
                    AssetDatabase.CreateAsset(bodyShadowMat, matPath);
                }
                else
                {
                    var unlit = Shader.Find("Unlit/Color");
                    if (unlit && bodyShadowMat.shader != unlit) bodyShadowMat.shader = unlit;
                }
                bodyShadowMat.color = new Color(0.01f, 0.01f, 0.014f, 1f);
                EditorUtility.SetDirty(bodyShadowMat);

                var hairShadowMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/GhostShadowHair.mat");
                if (!hairShadowMat)
                {
                    var baseHair = CharacterMaterialRepair.OpacityMaterial(true);
                    hairShadowMat = new Material(baseHair) { name = "GhostShadowHair" };
                    AssetDatabase.CreateAsset(hairShadowMat, "Assets/ChapterOne/Gameplay/Materials/GhostShadowHair.mat");
                }
                hairShadowMat.color = new Color(0.012f, 0.012f, 0.016f, 1f);
                hairShadowMat.SetFloat("_Glossiness", 0f);
                hairShadowMat.SetFloat("_SpecularHighlights", 0f);
                EditorUtility.SetDirty(hairShadowMat);

                var smr = inst.GetComponentInChildren<SkinnedMeshRenderer>(true);
                if (smr != null)
                {
                    smr.sharedMaterials = new Material[] { bodyShadowMat, bodyShadowMat, hairShadowMat };
                }
            }

            // Cold moonlight rim light behind her illuminating the misty glade & giant tree bark
            var rimObj = new GameObject("Sacred Tree Shadow Rim Light");
            rimObj.transform.SetParent(parent, false);
            rimObj.transform.position = pos + new Vector3(-1.5f, 3.8f, 6.2f);
            var rim = rimObj.AddComponent<Light>();
            rim.type = LightType.Point;
            rim.color = new Color(0.42f, 0.66f, 0.92f);
            rim.range = 28f;
            rim.intensity = 3.0f;
            rim.shadows = LightShadows.None;
            ghost.backRimLight = rim;

            var faceLightObj = new GameObject("Shadow Face Rim Light");
            faceLightObj.transform.SetParent(root.transform, false);
            faceLightObj.transform.localPosition = new Vector3(0.25f, 1.85f, 1.15f);
            var faceRim = faceLightObj.AddComponent<Light>();
            faceRim.type = LightType.Point;
            faceRim.color = new Color(0.36f, 0.50f, 0.70f);
            faceRim.range = 3.2f;
            faceRim.intensity = 0.65f;
            faceRim.shadows = LightShadows.None;
            faceRim.enabled = false;
            ghost.faceSilhouetteLight = faceRim;

            ghost.CacheBones();
            return ghost;
        }
    }
}
