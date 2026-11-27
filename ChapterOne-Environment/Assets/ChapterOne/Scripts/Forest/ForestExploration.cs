using UnityEngine;

namespace ThatNight.ChapterOne
{
    // Optional walking inspection mode. Keeps the existing fly camera and review shortcuts.
    [RequireComponent(typeof(Camera), typeof(EnvironmentCamera))]
    public sealed class ForestExploration : MonoBehaviour
    {
        public Transform[] viewpoints;
        public Light flashlight;
        public AudioClip wind, insects;
        public bool Walking { get; private set; }
        public bool ControlsLocked { get; set; }
        public bool MovementLocked { get; set; }
        public bool GlanceLocked { get; set; }
        public bool LookClamped { get; set; }
        public float ClampCenterYaw { get; set; }
        public float ClampYawRange { get; set; } = 40f;
        public bool RestrictToCompound { get; set; } = true;
        public float WalkSpeedMultiplier { get; set; } = 1.0f;
        public bool FlashlightUnlocked { get; set; } = true;
        public bool CinematicOverride { get; set; }
        public CharacterController Body => body;
        public bool IsSeated { get; set; }
        public string Location { get; private set; } = "Forest edge";
        Vector2[] trailWaypoints;
        float trailHalfWidth = 22f;
        float trailMaxDistance = float.PositiveInfinity;
        float trailTotalLength;
        CharacterController body;
        Transform rig;
        EnvironmentCamera inspection;
        AudioSource windSource, insectSource;
        float yaw, pitch, vertical, nextCull;
        float targetYaw, targetPitch;
        float bobCycle, currentBobY, currentSwayX;
        int viewpoint;
        ForestDetailCell[] cells;

        void OnEnable()
        {
            if (!Application.isPlaying) return;
            inspection = GetComponent<EnvironmentCamera>();
            cells = FindObjectsByType<ForestDetailCell>(FindObjectsSortMode.None);
            windSource = MakeAudio(wind, .12f);
            insectSource = MakeAudio(insects, .075f);
            if (flashlight) flashlight.enabled = false;
        }

        AudioSource MakeAudio(AudioClip clip, float volume)
        {
            string name = clip ? clip.name : "Forest ambience";
            var child=transform.Find(name);
            if(!child){child=new GameObject(name).transform;child.SetParent(transform,false);}
            var source = child.GetComponent<AudioSource>();
            if(!source)source=child.gameObject.AddComponent<AudioSource>();
            source.clip = clip; source.loop = true; source.volume = volume;
            source.spatialBlend = 0; source.playOnAwake = false;
            if (clip) source.Play();
            return source;
        }

        bool prevSubtleGlance = false;

        void Update()
        {
            bool storyActive = ChapterOneDirector.Instance != null;
            bool input = !GameplayReview.IsRunning;
            if (input && !storyActive && !ControlsLocked && Input.GetKeyDown(KeyCode.X)) SetWalking(!Walking);
            if (input && !storyActive && !ControlsLocked && Input.GetKeyDown(KeyCode.F8)) Visit(viewpoint++);
            if (input && !storyActive && !ControlsLocked && FlashlightUnlocked && Input.GetKeyDown(KeyCode.L) && flashlight) flashlight.enabled = !flashlight.enabled;
            if (input && !storyActive && !ControlsLocked && Input.GetKeyDown(KeyCode.K) && inspection.lighting)
                inspection.lighting.ApplyStage(AtmosphereStage.NightHorror);
            if (Walking && !CinematicOverride)
            {
                if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
                if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.anyKeyDown) && !Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                // Check if subtle clamped glance is needed (during cutscenes, talking, inspecting, previewing)
                bool isSubtleGlance = ControlsLocked || LookClamped;
                if (isSubtleGlance && !prevSubtleGlance)
                {
                    if (ClampCenterYaw == 0f && LookClamped == false)
                    {
                        ClampCenterYaw = targetYaw;
                    }
                }
                prevSubtleGlance = isSubtleGlance;

                if (!GlanceLocked && Cursor.lockState == CursorLockMode.Locked)
                {
                    if (isSubtleGlance)
                    {
                        // Subtle, smooth glance (+/- 18 to 22 degrees yaw, +/- 12 degrees pitch)
                        float yawRange = LookClamped ? ClampYawRange : 18f;
                        float pitchRange = LookClamped ? 15f : 12f;
                        targetYaw = Mathf.Clamp(targetYaw + Input.GetAxisRaw("Mouse X") * 0.9f, ClampCenterYaw - yawRange, ClampCenterYaw + yawRange);
                        targetPitch = Mathf.Clamp(targetPitch - Input.GetAxisRaw("Mouse Y") * 0.9f, -pitchRange, pitchRange);
                    }
                    else
                    {
                        targetYaw += Input.GetAxisRaw("Mouse X") * 1.6f;
                        targetPitch = Mathf.Clamp(targetPitch - Input.GetAxisRaw("Mouse Y") * 1.6f, -80, 80);
                    }
                }
                yaw = Mathf.Lerp(yaw, targetYaw, Time.deltaTime * 24f);
                pitch = Mathf.Lerp(pitch, targetPitch, Time.deltaTime * 24f);

                rig.rotation = Quaternion.Euler(0, yaw, 0);

                var move = Vector3.zero;
                bool dialoguePlaying = (ChapterOneNarrative.Instance != null && ChapterOneNarrative.Instance.IsPlaying);
                bool sequenceBusy = (ChapterOneDirector.Instance != null && ChapterOneDirector.Instance.SequenceBusy);
                bool phoneHeld = (ChapterOneDirector.Instance != null && ChapterOneDirector.Instance.nokia != null && ChapterOneDirector.Instance.nokia.isHeld);
                if (!ControlsLocked && !MovementLocked && !dialoguePlaying && !sequenceBusy && !phoneHeld)
                {
                    move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                    move = rig.TransformDirection(Vector3.ClampMagnitude(move, 1));
                }
                if (body.isGrounded && vertical < 0) vertical = -2;
                vertical = Mathf.Max(-30, vertical - 22 * Time.deltaTime);
                // Steady atmospheric walk speed (1.65 indoors/compound, scaled smoothly in open forest)
                float moveSpeed = 1.65f * Mathf.Max(0.5f, WalkSpeedMultiplier);
                body.Move((move * moveSpeed + Vector3.up * vertical) * Time.deltaTime);

                if (RestrictToCompound)
                {
                    // Restrict player to Wedding & House compound only in Chapter One morning/wedding phases
                    const float CompoundMinX = -23.0f;
                    const float CompoundMaxX = 23.0f;
                    const float CompoundMinZ = -19.0f;
                    const float CompoundMaxZ = 45.0f;
                    Vector3 clampedPos = rig.position;
                    clampedPos.x = Mathf.Clamp(clampedPos.x, CompoundMinX, CompoundMaxX);
                    clampedPos.z = Mathf.Clamp(clampedPos.z, CompoundMinZ, CompoundMaxZ);
                    rig.position = clampedPos;
                }
                else if (trailWaypoints != null && trailWaypoints.Length >= 2)
                {
                    // Keep the Bride strictly inside the winding left-right forest trail corridor
                    // so she follows the crying voice to the Big Tree and can NEVER return to the wedding house.
                    Vector2 pos2 = new(rig.position.x, rig.position.z);
                    Vector2 clamped2 = ClampToTrailCorridor(pos2, out _);
                    if ((clamped2 - pos2).sqrMagnitude > 0.0001f)
                    {
                        Vector3 p = rig.position;
                        p.x = clamped2.x;
                        p.z = clamped2.y;
                        rig.position = p;
                    }
                }

                // Organic head bobbing tied to movement
                float walkMag = Mathf.Clamp01(move.magnitude);
                bobCycle += walkMag * moveSpeed * Time.deltaTime * 3.8f;
                float targetBob = Mathf.Sin(bobCycle * 2f) * 0.012f * walkMag;
                float targetSway = Mathf.Cos(bobCycle) * 0.007f * walkMag;
                currentBobY = Mathf.Lerp(currentBobY, targetBob, Time.deltaTime * 12f);
                currentSwayX = Mathf.Lerp(currentSwayX, targetSway, Time.deltaTime * 12f);

                transform.rotation = Quaternion.Euler(pitch, yaw, currentSwayX * -18f);
                float eyeHeight = IsSeated ? 0.95f : 1.54f;
                transform.position = rig.position + Vector3.up * (eyeHeight + currentBobY) + rig.right * currentSwayX;
            }
            if (Time.unscaledTime >= nextCull)
            {
                nextCull = Time.unscaledTime + .4f;
                foreach (var cell in cells) if (cell) cell.SetVisible(transform.position);
            }
            float forest = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(35, 90, new Vector2(transform.position.x, transform.position.z - 15).magnitude));
            bool night = inspection.lighting && inspection.lighting.currentStage == AtmosphereStage.NightHorror;
            windSource.volume = Mathf.Lerp(.035f, .13f, forest);
            insectSource.volume = forest * (night ? .10f : .035f);
        }

        public bool SetWalking(bool enabled)
        {
            if(!Application.isPlaying)return false;
            if(!inspection)inspection=GetComponent<EnvironmentCamera>();
            if (enabled == Walking) return true;
            if (enabled)
            {
                // Require ground below the camera; reject sky/terrain voids instead of dropping the player.
                if (!Physics.Raycast(transform.position + Vector3.up * .2f, Vector3.down, out var hit, 600, ~0, QueryTriggerInteraction.Ignore)) return false;
                rig = new GameObject("Forest walking rig").transform;
                rig.position = hit.point + Vector3.up * .12f;
                yaw = transform.eulerAngles.y; pitch = Mathf.DeltaAngle(0, transform.eulerAngles.x);
                targetYaw = yaw; targetPitch = pitch;
                rig.rotation = Quaternion.Euler(0, yaw, 0);
                body = rig.gameObject.AddComponent<CharacterController>();
                body.height = 1.62f; body.radius = .28f; body.center = Vector3.up * .81f;
                body.stepOffset = .3f; body.slopeLimit = 46; body.skinWidth = .035f;
                transform.position=rig.position+Vector3.up*1.54f;
                transform.rotation = Quaternion.Euler(pitch, yaw, 0);
                vertical = -2; Walking = true; IsSeated = false;
                Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
            }
            else
            {
                if (rig) Destroy(rig.gameObject);
                Walking = false; inspection.SyncLook();
                Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            }
            return true;
        }

        public Vector3 FeetPosition => rig ? rig.position : (transform.position - Vector3.up * 1.54f);
        public float CurrentYaw => yaw;
        public float CurrentPitch => pitch;
        public float TrailTotalLength => trailTotalLength;

        public void SetTrailCorridor(Vector2[] waypoints, float halfWidth = 24f, float maxDistance = float.PositiveInfinity)
        {
            trailWaypoints = waypoints;
            trailHalfWidth = Mathf.Max(6f, halfWidth);
            trailMaxDistance = maxDistance;
            trailTotalLength = 0f;
            if (waypoints != null)
            {
                for (int i = 1; i < waypoints.Length; i++)
                    trailTotalLength += Vector2.Distance(waypoints[i - 1], waypoints[i]);
            }
        }

        public void ClearTrailCorridor()
        {
            trailWaypoints = null;
            trailMaxDistance = float.PositiveInfinity;
            trailTotalLength = 0f;
        }

        public float GetTrailProgress(Vector2 pos)
        {
            ClampToTrailCorridor(pos, out float progress);
            return progress;
        }

        public Vector2 SampleTrailAtDistance(float dist)
        {
            if (trailWaypoints == null || trailWaypoints.Length == 0) return Vector2.zero;
            if (trailWaypoints.Length == 1 || dist <= 0f) return trailWaypoints[0];

            float accum = 0f;
            for (int i = 1; i < trailWaypoints.Length; i++)
            {
                float segLen = Vector2.Distance(trailWaypoints[i - 1], trailWaypoints[i]);
                if (accum + segLen >= dist && segLen > 0.001f)
                {
                    float u = Mathf.Clamp01((dist - accum) / segLen);
                    return Vector2.Lerp(trailWaypoints[i - 1], trailWaypoints[i], u);
                }
                accum += segLen;
            }
            return trailWaypoints[trailWaypoints.Length - 1];
        }

        Vector2 ClampToTrailCorridor(Vector2 p, out float bestProgress)
        {
            bestProgress = 0f;
            if (trailWaypoints == null || trailWaypoints.Length < 2) return p;

            float bestDistSq = float.PositiveInfinity;
            Vector2 bestClosest = trailWaypoints[0];
            float accum = 0f;

            for (int i = 1; i < trailWaypoints.Length; i++)
            {
                Vector2 a = trailWaypoints[i - 1];
                Vector2 b = trailWaypoints[i];
                Vector2 ab = b - a;
                float segLen = ab.magnitude;
                if (segLen < 0.001f) continue;

                float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / (segLen * segLen));
                float segProg = accum + t * segLen;

                if (segProg > trailMaxDistance)
                {
                    float maxT = Mathf.Clamp01((trailMaxDistance - accum) / segLen);
                    t = maxT;
                    segProg = accum + t * segLen;
                }

                Vector2 proj = a + ab * t;
                float dSq = (p - proj).sqrMagnitude;
                if (dSq < bestDistSq)
                {
                    bestDistSq = dSq;
                    bestClosest = proj;
                    bestProgress = segProg;
                }

                accum += segLen;
                if (accum >= trailMaxDistance) break;
            }

            Vector2 offset = p - bestClosest;
            float dist = offset.magnitude;
            if (dist > trailHalfWidth && dist > 0.0001f)
            {
                return bestClosest + offset * (trailHalfWidth / dist);
            }
            return p;
        }

        public void SetLookDirection(float newYaw, float newPitch)
        {
            yaw = newYaw;
            targetYaw = newYaw;
            pitch = newPitch;
            targetPitch = newPitch;
            ClampCenterYaw = newYaw;
            if (rig) rig.rotation = Quaternion.Euler(0, yaw, 0);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }

        public void SetFeetPosition(Vector3 feetPos)
        {
            if (rig)
            {
                if (body) body.enabled = false;
                rig.position = feetPos;
                if (body) body.enabled = true;
            }
            float eyeH = IsSeated ? 0.95f : 1.54f;
            transform.position = feetPos + Vector3.up * eyeH;
        }

        public void MoveRigAndCamera(Vector3 cameraPos, Quaternion cameraRot, bool seated = false)
        {
            IsSeated = seated;
            float eyeH = seated ? 0.95f : 1.54f;
            if (rig)
            {
                if (body) body.enabled = false;
                rig.position = cameraPos - Vector3.up * eyeH;
                rig.rotation = Quaternion.Euler(0, cameraRot.eulerAngles.y, 0);
                if (body) body.enabled = true;
            }
            yaw = cameraRot.eulerAngles.y;
            targetYaw = yaw;
            pitch = Mathf.DeltaAngle(0, cameraRot.eulerAngles.x);
            targetPitch = pitch;
            ClampCenterYaw = yaw;
            transform.position = cameraPos;
            transform.rotation = cameraRot;
        }

        public void Visit(int index)
        {
            if(!inspection)inspection=GetComponent<EnvironmentCamera>();
            if (viewpoints == null || viewpoints.Length == 0) return;
            bool walking = Walking; if (walking) SetWalking(false);
            var point = viewpoints[Mathf.Abs(index) % viewpoints.Length];
            transform.SetPositionAndRotation(point.position, point.rotation);
            Location = point.name; inspection.SyncLook();
            if (walking) SetWalking(true);
        }

        void OnDisable()
        {
            // The camera stays at its authored parent, even while walking. Safe during Play teardown.
            if(rig){if(Application.isPlaying)Destroy(rig.gameObject);else DestroyImmediate(rig.gameObject);}
            Walking=false;
            Cursor.lockState=CursorLockMode.None;Cursor.visible=true;
            if (cells != null) foreach (var cell in cells) if (cell) cell.ShowAll();
            if (windSource) windSource.Stop();
            if (insectSource) insectSource.Stop();
        }
    }
}
