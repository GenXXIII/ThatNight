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
        public CharacterController Body => body;
        public bool IsSeated { get; set; }
        public string Location { get; private set; } = "Forest edge";
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
            bool input=!GameplayReview.IsRunning;
            if (input && !ControlsLocked && Input.GetKeyDown(KeyCode.X)) SetWalking(!Walking);
            if (input && !ControlsLocked && Input.GetKeyDown(KeyCode.F8)) Visit(viewpoint++);
            if (input && !ControlsLocked && Input.GetKeyDown(KeyCode.L) && flashlight) flashlight.enabled = !flashlight.enabled;
            if (input && !ControlsLocked && Input.GetKeyDown(KeyCode.K) && inspection.lighting)
                inspection.lighting.ApplyStage(AtmosphereStage.NightHorror);
            if (Walking)
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
                // No sprint in this game: steady, deliberate atmospheric walk speed
                const float moveSpeed = 1.65f;
                body.Move((move * moveSpeed + Vector3.up * vertical) * Time.deltaTime);

                    // Restrict player to Wedding & House compound only in Chapter One (no forest access yet)
                    const float CompoundMinX = -23.0f;
                    const float CompoundMaxX = 23.0f;
                    const float CompoundMinZ = -19.0f;
                    const float CompoundMaxZ = 45.0f;
                    Vector3 clampedPos = rig.position;
                    clampedPos.x = Mathf.Clamp(clampedPos.x, CompoundMinX, CompoundMaxX);
                    clampedPos.z = Mathf.Clamp(clampedPos.z, CompoundMinZ, CompoundMaxZ);
                    rig.position = clampedPos;

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
