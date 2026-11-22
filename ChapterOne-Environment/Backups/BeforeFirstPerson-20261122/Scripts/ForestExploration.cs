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
        public CharacterController Body => body;
        public string Location { get; private set; } = "Forest edge";
        CharacterController body;
        Transform rig;
        EnvironmentCamera inspection;
        AudioSource windSource, insectSource;
        float yaw, pitch, vertical, nextCull;
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X)) SetWalking(!Walking);
            if (Input.GetKeyDown(KeyCode.F8)) Visit(viewpoint++);
            if (Input.GetKeyDown(KeyCode.L) && flashlight) flashlight.enabled = !flashlight.enabled;
            if (Input.GetKeyDown(KeyCode.K) && inspection.lighting)
                inspection.lighting.ApplyStage(AtmosphereStage.NightHorror);
            if (Walking)
            {
                if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
                if (Input.GetMouseButtonDown(1)) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    yaw += Input.GetAxisRaw("Mouse X") * 1.6f;
                    pitch = Mathf.Clamp(pitch - Input.GetAxisRaw("Mouse Y") * 1.6f, -80, 80);
                    rig.rotation = Quaternion.Euler(0, yaw, 0);
                    transform.rotation = Quaternion.Euler(pitch, yaw, 0);
                    var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                    move = rig.TransformDirection(Vector3.ClampMagnitude(move, 1));
                    if (body.isGrounded && vertical < 0) vertical = -2;
                    vertical = Mathf.Max(-30, vertical - 22 * Time.deltaTime);
                    body.Move((move * (Input.GetKey(KeyCode.LeftShift) ? 4.5f : 2.2f) + Vector3.up * vertical) * Time.deltaTime);
                }
                else if (!body.isGrounded) body.Move(Vector3.down * 3 * Time.deltaTime);
                transform.position=rig.position+Vector3.up*1.7f;
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
                rig.rotation = Quaternion.Euler(0, yaw, 0);
                body = rig.gameObject.AddComponent<CharacterController>();
                body.height = 1.8f; body.radius = .28f; body.center = Vector3.up * .9f;
                body.stepOffset = .3f; body.slopeLimit = 46; body.skinWidth = .035f;
                transform.position=rig.position+Vector3.up*1.7f;
                transform.rotation = Quaternion.Euler(pitch, yaw, 0);
                vertical = -2; Walking = true;
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
