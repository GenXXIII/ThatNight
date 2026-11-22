using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne.Editor
{
    public static class FirstPersonHandsBuilder
    {
        const string HandsRootName = "First Person Animated Hands";

        public static FirstPersonHandsAnimator Build(Camera camera)
        {
            if (!camera) return null;

            CharacterArtBuilder.Prepare();

            // Materials
            var skinMat = SurfaceLibrary.Plain("Bride skin", new Color(0.74f, 0.53f, 0.39f), 0.25f, 0.1f);
            var sleeveMat = SurfaceLibrary.Plain("Bride sleepwear silk", new Color(0.93f, 0.91f, 0.86f), 0.45f, 0.05f);
            var goldMat = SurfaceLibrary.Plain("Bride gold bangle", new Color(0.88f, 0.70f, 0.20f), 0.82f, 0.75f);

            // Remove legacy or duplicate hands
            var old = camera.transform.Find(HandsRootName);
            if (old) Object.DestroyImmediate(old.gameObject);
            var oldLegacy = camera.transform.Find("First person hands");
            if (oldLegacy) Object.DestroyImmediate(oldLegacy.gameObject);

            var handsObj = new GameObject(HandsRootName);
            handsObj.transform.SetParent(camera.transform, false);
            handsObj.transform.localPosition = Vector3.zero;
            handsObj.transform.localRotation = Quaternion.identity;

            var animator = camera.GetComponent<FirstPersonHandsAnimator>();
            if (!animator) animator = camera.gameObject.AddComponent<FirstPersonHandsAnimator>();

            animator.cameraTransform = camera.transform;
            animator.handsRoot = handsObj.transform;

            // Build Left Arm
            animator.leftArmRoot = BuildArm(handsObj.transform, "Left Arm", -1, skinMat, sleeveMat, goldMat,
                out animator.leftShoulder, out animator.leftUpperArm, out animator.leftForearm,
                out animator.leftWrist, out animator.leftPalm, out animator.leftFingers, out _);

            // Build Right Arm
            animator.rightArmRoot = BuildArm(handsObj.transform, "Right Arm", 1, skinMat, sleeveMat, goldMat,
                out animator.rightShoulder, out animator.rightUpperArm, out animator.rightForearm,
                out animator.rightWrist, out animator.rightPalm, out animator.rightFingers, out animator.phoneSocket);

            // Set layers to Ignore Raycast (Layer 2) and disable shadow casting on first-person view
            foreach (var t in handsObj.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = 2;
            }
            foreach (var r in handsObj.GetComponentsInChildren<Renderer>())
            {
                r.shadowCastingMode = ShadowCastingMode.Off;
                r.receiveShadows = true;
            }

            Debug.Log("FIRST_PERSON_ANIMATED_HANDS_BUILT successfully with silk sleeves and gold bangles.");
            return animator;
        }

        static Transform BuildArm(Transform parent, string name, int side, Material skinMat, Material sleeveMat, Material goldMat,
            out Transform shoulder, out Transform upperArm, out Transform forearm,
            out Transform wrist, out Transform palm, out Transform[] fingers, out Transform phoneSocket)
        {
            var armRoot = new GameObject(name).transform;
            armRoot.SetParent(parent, false);

            shoulder = armRoot;

            // 1. Upper Arm with Silk Sleeve
            upperArm = Part("Upper Arm Sleeve", armRoot, new Vector3(side * 0.02f, 0.08f, -0.12f), new Vector3(0.085f, 0.22f, 0.085f), sleeveMat, PrimitiveType.Cylinder);
            upperArm.localRotation = Quaternion.Euler(-45f, side * 15f, side * -10f);

            // 2. Forearm (tapered female proportion)
            forearm = Part("Forearm", armRoot, new Vector3(side * 0.01f, 0.02f, -0.04f), new Vector3(0.065f, 0.24f, 0.065f), skinMat, PrimitiveType.Cylinder);
            forearm.localRotation = Quaternion.Euler(65f, side * -12f, side * 10f);

            // 3. Traditional Cambodian Gold Bangle around wrist
            wrist = new GameObject("Wrist").transform;
            wrist.SetParent(armRoot, false);
            wrist.localPosition = new Vector3(0f, -0.02f, 0.06f);
            wrist.localRotation = Quaternion.identity;

            var bangle = Part("Gold Bangle", wrist, Vector3.zero, new Vector3(0.076f, 0.022f, 0.076f), goldMat, PrimitiveType.Cylinder);
            bangle.localRotation = Quaternion.Euler(75f, 0f, 0f);

            // 4. Palm
            palm = Part("Palm", wrist, new Vector3(0f, 0.015f, 0.065f), new Vector3(0.075f, 0.028f, 0.085f), skinMat, PrimitiveType.Sphere);
            palm.localRotation = Quaternion.Euler(12f, side * -5f, 0f);

            // 5. Thumb
            var thumbRoot = new GameObject("Thumb").transform;
            thumbRoot.SetParent(palm, false);
            thumbRoot.localPosition = new Vector3(side * -0.038f, -0.005f, 0.015f);
            thumbRoot.localRotation = Quaternion.Euler(18f, side * -38f, 0f);

            Part("Thumb Base", thumbRoot, new Vector3(0f, 0f, 0.025f), new Vector3(0.022f, 0.02f, 0.045f), skinMat, PrimitiveType.Sphere);
            Part("Thumb Tip", thumbRoot, new Vector3(0f, -0.005f, 0.052f), new Vector3(0.018f, 0.016f, 0.032f), skinMat, PrimitiveType.Sphere);

            // 6. Fingers (Index, Middle, Ring, Pinky)
            fingers = new Transform[4];
            for (int i = 0; i < 4; i++)
            {
                float xOffset = (i - 1.5f) * 0.018f * side;
                float fLength = i == 1 ? 0.058f : (i == 2 ? 0.055f : (i == 0 ? 0.050f : 0.042f));

                var finger = new GameObject("Finger " + (i + 1)).transform;
                finger.SetParent(palm, false);
                finger.localPosition = new Vector3(xOffset, 0.002f, 0.042f);
                finger.localRotation = Quaternion.Euler(22f + i * 3f, 0f, 0f);
                fingers[i] = finger;

                // Proximal segment
                Part("Segment 1", finger, new Vector3(0f, 0f, fLength * 0.35f), new Vector3(0.015f, 0.014f, fLength * 0.65f), skinMat, PrimitiveType.Sphere);
                // Tip segment
                Part("Fingertip", finger, new Vector3(0f, -0.006f, fLength * 0.78f), new Vector3(0.013f, 0.012f, fLength * 0.45f), skinMat, PrimitiveType.Sphere);
            }

            // Phone Socket (Right hand)
            phoneSocket = null;
            if (side > 0)
            {
                var sock = new GameObject("Nokia Phone Socket").transform;
                sock.SetParent(palm, false);
                // Angled so the screen faces directly into the bride's eyes
                sock.localPosition = new Vector3(-0.025f, 0.035f, 0.04f);
                sock.localRotation = Quaternion.Euler(85f, -30f, 25f);
                phoneSocket = sock;
            }

            return armRoot;
        }

        static Transform Part(string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat, PrimitiveType primitive = PrimitiveType.Cube)
        {
            var obj = GameObject.CreatePrimitive(primitive);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = localScale;

            var col = obj.GetComponent<Collider>();
            if (col) Object.DestroyImmediate(col);

            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer && mat) renderer.sharedMaterial = mat;

            return obj.transform;
        }
    }
}
