using System;
using System.Collections;
using UnityEngine;
namespace ThatNight.ChapterOne
{
    [DefaultExecutionOrder(20)]
    public sealed class PlayerInteraction : MonoBehaviour
    {
        public FirstPersonBody body;
        public ForestExploration explorer;
        public Transform arrival;
        public PickupItem HeldItem { get; private set; }
        public WorldInteractable Focus { get; private set; }
        public bool Busy { get; private set; }
        public WeddingGuest Speaker { get; private set; }
        string speech;
        GUIStyle text,title,prompt,button;
        bool help=true;
        HingedOpening activeDoorHold = null;

        void Start()
        {
            // If ChapterOneDirector is active, let Director manage scene flow, camera & spawning
            if (ChapterOneDirector.Instance != null) return;
            if(!Array.Exists(Environment.GetCommandLineArgs(),x=>x=="-forestPreview"))EnterGameplay();
        }
        public void EnterGameplay()
        {
            if (arrival == null)
            {
                var spawn = GameObject.Find("PlayerArrival") ?? GameObject.Find("02 ARRIVAL - bridal arrival vehicle");
                if (spawn) arrival = spawn.transform;
            }
            if (explorer && arrival != null)
            {
                explorer.SetWalking(false);
                transform.SetPositionAndRotation(arrival.position, arrival.rotation);
            }
            var envCam = GetComponent<EnvironmentCamera>();
            if (envCam && envCam.lighting) envCam.lighting.ApplyStage(AtmosphereStage.AfternoonCelebration);
            if (explorer) explorer.SetWalking(true);
        }
        void Update()
        {
            if(GameplayReview.IsRunning)return;
            if(Input.GetKeyDown(KeyCode.H))help=!help;
            if(Input.GetKeyDown(KeyCode.F7)&&!Busy){CloseDialogue();EnterGameplay();}
            if(!explorer || !explorer.Walking){CloseDialogue();Focus=null;return;}
            if (ChapterOneNarrative.Instance && ChapterOneNarrative.Instance.IsPlaying)
            { Focus = null; return; }
            if (ChapterOneDirector.Instance && ChapterOneDirector.Instance.SequenceBusy)
            { Focus = null; return; }
            if(Speaker)
            {
                if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.E))CloseDialogue();
                if(Input.GetKeyDown(KeyCode.Alpha1))Choose(0);
                if(Input.GetKeyDown(KeyCode.Alpha2))Choose(1);
                return;
            }
            Focus=null;

            // Handle continuous door hold [E] and Esc to let go
            if (activeDoorHold != null)
            {
                float distToDoor = Vector3.Distance(transform.position, activeDoorHold.transform.position);
                if (Input.GetKeyDown(KeyCode.Escape) || distToDoor > 2.8f)
                {
                    activeDoorHold.OnReleaseHold(this);
                    activeDoorHold = null;
                    return;
                }

                if ((Input.GetKey(KeyCode.E) || Input.GetMouseButton(0)) && Cursor.lockState == CursorLockMode.Locked)
                {
                    activeDoorHold.OnContinueHold(this, Time.deltaTime);
                    if (!activeDoorHold.isBeingHeld) activeDoorHold = null;
                }
                else
                {
                    // Released [E] before reaching max: door automatically springs back to closed!
                    activeDoorHold.OnReleaseHold(this);
                    activeDoorHold = null;
                }
                return;
            }

            if(Busy)return;

            // Multi-tier interactable detection up to 2.40m:
            // 1. RaycastAll to pierce through non-interactable table mesh colliders
            // 2. SphereCastAll (0.22m radius) for small tabletop props like the phone
            // 3. Proximity / view cone fallback (within 45 deg)
            const float maxArmReach = 2.40f;
            Ray ray = new Ray(transform.position, transform.forward);
            WorldInteractable bestTarget = null;
            float closestDist = float.MaxValue;

            RaycastHit[] hits = Physics.RaycastAll(ray, maxArmReach, ~(1 << 2), QueryTriggerInteraction.Ignore);
            foreach (var h in hits)
            {
                var interactable = h.collider.GetComponentInParent<WorldInteractable>();
                if (interactable != null && interactable.CanUse && h.distance <= interactable.Reach)
                {
                    if (h.distance < closestDist)
                    {
                        closestDist = h.distance;
                        bestTarget = interactable;
                    }
                }
            }

            if (bestTarget == null)
            {
                RaycastHit[] sphereHits = Physics.SphereCastAll(ray, 0.22f, maxArmReach, ~(1 << 2), QueryTriggerInteraction.Ignore);
                foreach (var h in sphereHits)
                {
                    var interactable = h.collider.GetComponentInParent<WorldInteractable>();
                    if (interactable != null && interactable.CanUse && h.distance <= interactable.Reach)
                    {
                        if (h.distance < closestDist)
                        {
                            closestDist = h.distance;
                            bestTarget = interactable;
                        }
                    }
                }
            }

            if (bestTarget == null)
            {
                Collider[] nearCols = Physics.OverlapSphere(transform.position, maxArmReach, ~(1 << 2), QueryTriggerInteraction.Ignore);
                float bestDot = 0.65f;
                foreach (var col in nearCols)
                {
                    var interactable = col.GetComponentInParent<WorldInteractable>();
                    if (interactable != null && interactable.CanUse)
                    {
                        Vector3 dirToItem = (interactable.transform.position - transform.position).normalized;
                        float dot = Vector3.Dot(transform.forward, dirToItem);
                        float dist = Vector3.Distance(transform.position, interactable.transform.position);
                        if (dot > bestDot && dist <= interactable.Reach)
                        {
                            bestDot = dot;
                            bestTarget = interactable;
                        }
                    }
                }
            }

            Focus = bestTarget;

            if((Input.GetKeyDown(KeyCode.E)||Input.GetMouseButtonDown(0))&&Cursor.lockState==CursorLockMode.Locked&&Focus)
            {
                if (Focus is HingedOpening door)
                {
                    activeDoorHold = door;
                    door.OnStartHold(this);
                }
                else
                {
                    Focus.Use(this);
                }
            }
            if(Input.GetKeyDown(KeyCode.Q))ReturnHeldItem();
        }
        public void ReturnHeldItem()
        {
            if(!HeldItem)return;
            var item=HeldItem;ReachAndAct(transform.position,()=>{item.PutBack();HeldItem=null;if(body)body.Holding=false;});
        }
        public void ReachAndAct(Vector3 target, Action action)
        {
            if (Busy || Speaker) return;
            StartCoroutine(Reach(target, action));
        }
        IEnumerator Reach(Vector3 target, Action action)
        {
            Busy = true;
            try
            {
                if (body != null)
                {
                    body.ReachTarget = target;
                    bool done = false;
                    body.PlayInteractHand(() =>
                    {
                        action?.Invoke();
                        done = true;
                    });
                    // Wait for hand reach to finish before accepting new interaction
                    while (body.interactProgress > 0.001f || !done)
                    {
                        yield return null;
                    }
                }
                else
                {
                    action?.Invoke();
                    yield return new WaitForSeconds(0.12f);
                }
            }
            finally
            {
                Busy = false;
                if (body) body.ReachTarget = Vector3.zero;
            }
        }
        public void Hold(PickupItem item){HeldItem=item;item.Attach(body.holdSocket);body.Holding=true;}
        public void Talk(WeddingGuest guest)
        {
            Speaker=guest;speech=guest.greeting;
            if (explorer)
            {
                explorer.MovementLocked = true;
                explorer.LookClamped = true;
                explorer.ClampCenterYaw = explorer.transform.eulerAngles.y;
                explorer.ClampYawRange = 35f;
            }
        }
        public void Choose(int choice){if(Speaker)speech=choice==0?Speaker.weddingAnswer:Speaker.helpAnswer;}
        public void CloseDialogue()
        {
            if(!Speaker)return;Speaker=null;
            if (explorer)
            {
                explorer.ControlsLocked=false;
                explorer.MovementLocked=false;
                explorer.LookClamped=false;
            }
            if(explorer&&explorer.Walking){Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;}
        }
        void OnDisable(){StopAllCoroutines();Busy=false;if(body)body.ReachBlend=0;if(explorer)explorer.ControlsLocked=false;}
        void OnGUI()
        {
            if(!explorer||!explorer.Walking)return;
            if(text==null)
            {
                text=new GUIStyle(GUI.skin.label){fontSize=18,wordWrap=true};text.normal.textColor=new Color(.91f,.89f,.81f);
                title=new GUIStyle(text){fontSize=22,fontStyle=FontStyle.Bold};
                prompt=new GUIStyle(text){alignment=TextAnchor.MiddleCenter};
                button=new GUIStyle(GUI.skin.button){fontSize=17,alignment=TextAnchor.MiddleLeft,padding=new RectOffset(15,12,8,8)};
            }
            float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);var previous=GUI.matrix;GUI.matrix=Matrix4x4.Scale(Vector3.one*scale);
            float w=Screen.width/scale,h=Screen.height/scale;
            if(!Speaker)
            {
                if (activeDoorHold != null)
                {
                    GUI.color = new Color(.95f, .90f, .72f, .95f);
                    GUI.DrawTexture(new Rect(w * .5f - 4, h * .5f - 4, 8, 8), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                    string doorPrompt = "[Hold E] " + (activeDoorHold.isOpen ? "Push to Close " : "Pull to Open ") + activeDoorHold.displayName + "   •   [Esc] Let Go";
                    GUI.Label(new Rect(w * .5f - 300, h * .58f, 600, 50), doorPrompt, prompt);
                }
                else if(Focus)
                {
                    GUI.color=new Color(.95f,.90f,.72f,.95f);
                    GUI.DrawTexture(new Rect(w*.5f-4,h*.5f-4,8,8),Texture2D.whiteTexture);
                    GUI.color=Color.white;
                    string pText = Focus.Prompt;
                    if (!pText.StartsWith("[") && !pText.StartsWith("Hold")) pText = "[E]  " + pText;
                    GUI.Label(new Rect(w*.5f-300,h*.58f,600,50), pText, prompt);
                }
                else
                {
                    GUI.color=new Color(.85f,.85f,.85f,.40f);
                    GUI.DrawTexture(new Rect(w*.5f-1.5f,h*.5f-1.5f,3,3),Texture2D.whiteTexture);
                    GUI.color=Color.white;
                }
                if(HeldItem)GUI.Label(new Rect(28,h-75,600,35),HeldItem.displayName+"    Q  Put back",text);
            }
            else
            {
                float x=(w-800)*.5f,y=h-264;
                GUI.color=new Color(.025f,.035f,.033f,.96f);GUI.DrawTexture(new Rect(x,y,800,240),Texture2D.whiteTexture);GUI.color=Color.white;
                GUI.Label(new Rect(x+24,y+18,752,32),Speaker.displayName+"  /  "+Speaker.role,title);
                GUI.Label(new Rect(x+24,y+58,752,70),speech,text);
                if(GUI.Button(new Rect(x+24,y+140,365,40),"1  About the wedding",button))Choose(0);
                if(GUI.Button(new Rect(x+411,y+140,365,40),"2  Can you help me?",button))Choose(1);
                if(GUI.Button(new Rect(x+24,y+190,752,32),"E / Esc   Leave conversation",button))CloseDialogue();
            }
            GUI.matrix=previous;
        }
    }
}
