using UnityEngine;
namespace ThatNight.ChapterOne
{
    public sealed class PickupItem : WorldInteractable
    {
        public Vector3 gripPosition,gripEuler;
        [TextArea] public string description;
        public bool Held { get; private set; }
        Transform restingParent;
        Vector3 restingPosition,restingScale;
        Quaternion restingRotation;
        Collider[] shapes;
        public override float Reach => 1.10f; // Physical arm reach to table / floor
        public override string Prompt=>"Pick up "+displayName;
        void Awake()
        {
            restingParent=transform.parent;restingPosition=transform.position;
            restingRotation=transform.rotation;restingScale=transform.localScale;
            shapes=GetComponentsInChildren<Collider>();
        }
        public override void Use(PlayerInteraction player)
        {
            if(Held||player.HeldItem)return;
            player.ReachAndAct(transform.position,()=>player.Hold(this));
        }
        public void Attach(Transform grip)
        {
            Held=true;foreach(var c in shapes)c.enabled=false;
            transform.SetParent(grip,true);transform.localPosition=gripPosition;transform.localRotation=Quaternion.Euler(gripEuler);
        }
        public void PutBack()
        {
            transform.SetParent(restingParent,false);transform.position=restingPosition;
            transform.rotation=restingRotation;transform.localScale=restingScale;
            foreach(var c in shapes)c.enabled=true;Held=false;
        }
    }
}
