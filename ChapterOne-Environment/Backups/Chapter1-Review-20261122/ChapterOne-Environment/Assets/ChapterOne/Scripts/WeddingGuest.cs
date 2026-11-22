using UnityEngine;
namespace ThatNight.ChapterOne
{
    public sealed class WeddingGuest : WorldInteractable
    {
        public string role;
        [TextArea]public string greeting, weddingAnswer, helpAnswer;
        public Transform head,leftArm,rightArm;
        public override string Prompt=>"Talk to "+displayName+" · "+role;
        public override void Use(PlayerInteraction player)=>player.Talk(this);
        Quaternion headRest;
        Vector3 headPosition;
        void Start(){if(head){headRest=head.localRotation;headPosition=head.localPosition;}}
        void LateUpdate()
        {
            float phase=Time.time+transform.position.x;
            if(head)
            {
                head.localPosition=headPosition+Vector3.up*Mathf.Sin(phase*1.5f)*.006f;
                var camera=Camera.main;
                if(camera && Vector3.Distance(camera.transform.position,transform.position)<4)
                {
                    Vector3 dir=transform.InverseTransformDirection(camera.transform.position-head.position);
                    float yaw=Mathf.Clamp(Mathf.Atan2(dir.x,dir.z)*Mathf.Rad2Deg,-45,45);
                    head.localRotation=Quaternion.Slerp(head.localRotation,headRest*Quaternion.Euler(0,yaw,0),Time.deltaTime*3);
                }
                else head.localRotation=Quaternion.Slerp(head.localRotation,headRest,Time.deltaTime*3);
            }
            if(leftArm)leftArm.localRotation=Quaternion.Euler(Mathf.Sin(phase)*2,0,5);
            if(rightArm)rightArm.localRotation=Quaternion.Euler(Mathf.Sin(phase+1)*2,0,-5);
        }
    }
}
