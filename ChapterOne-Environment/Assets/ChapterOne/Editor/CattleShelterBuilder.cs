using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class CattleShelterBuilder
    {
        public static Transform Build()
        {
            Parent=null;var root=Group("08 CATTLE SHELTER - rectangular pen beside east road",SiteLayout.CattleShelterPosition);Parent=root;
            float w=SiteLayout.CattleShelterWidth/2,d=SiteLayout.CattleShelterDepth/2;
            var earth=S.Pbr("Cattle shelter packed earth","Ground084",new Color(.48f,.44f,.34f));
            var straw=S.Plain("Dry rice straw",new Color(.52f,.39f,.18f),.08f);
            Grid("Raised compacted earthen pen floor",12,24,(u,v)=>new Vector3((u-.5f)*SiteLayout.CattleShelterWidth,.045f,(v-.5f)*SiteLayout.CattleShelterDepth),earth,new Vector2(3,6));
            for(int side=-1;side<=1;side+=2)for(int bay=0;bay<4;bay++)
            {
                float z=-d+bay*4,height=side<0?3.35f:2.85f;
                var p=new Vector3(side*w,0,z);
                Box("Shelter post stone base",p+Vector3.up*.12f,new Vector3(.34f,.24f,.34f),S.Clay);
                Cylinder("Rough timber cattle shelter post",p,.105f,height,S.Bark);
                Beam("Shelter knee brace",p+Vector3.up*(height-.65f),p+new Vector3(-side*.6f,height-.03f,0),.085f,S.DarkWood);
                if(bay<3)Beam("Long roof bearer",p+Vector3.up*height,p+new Vector3(0,height,4),.14f,S.DarkWood);
                if(side>0)
                {
                    for(int rail=0;rail<3;rail++)if(bay<3)Beam("Road side cattle pen rail",p+Vector3.up*(.5f+rail*.37f),p+new Vector3(0,.5f+rail*.37f,4),.08f,S.Wood);
                    Tube("Tether ring",p+new Vector3(-.13f,1.1f,0),.07f,.012f,S.Metal,Quaternion.Euler(0,90,0),20);
                }
            }
            // Long rectangle follows the road; the house-facing side remains open.
            Grid("Rust streaked corrugated cattle shelter roof",90,28,(u,v)=>new Vector3((u-.5f)*(SiteLayout.CattleShelterWidth+.8f),3.4f-u*.55f+Mathf.Sin(u*Mathf.PI*96)*.027f,(v-.5f)*(SiteLayout.CattleShelterDepth+.8f)),S.Rust,new Vector2(5,7),true);
            for(int bay=0;bay<7;bay++)Beam("Exposed cattle shelter rafter",new Vector3(-w-.25f,3.34f,-d+bay*2),new Vector3(w+.25f,2.81f,-d+bay*2),.09f,S.DarkWood);
            for(int end=-1;end<=1;end+=2)for(int rail=0;rail<3;rail++)Beam("End pen rail",new Vector3(-w,.5f+rail*.37f,end*d),new Vector3(w,.5f+rail*.37f,end*d),.075f,S.Wood);
            for(int bay=0;bay<3;bay++)FeedingTrough(new Vector3(w-.65f,0,-4+bay*4));
            WaterTrough(new Vector3(-w+.95f,0,-4.8f));
            // Loose straw has real small blades; deterministic placement keeps rebuilds stable.
            var randomState=Random.state;Random.InitState(20061113);
            for(int i=0;i<650;i++)
            {
                Vector3 p=new Vector3(Random.Range(-w+.45f,w-.85f),.052f,Random.Range(-5.7f,5.7f));
                if(p.x<.1f&&p.z< -4)continue;
                float a=Random.Range(0,Mathf.PI*2),length=Random.Range(.09f,.42f);
                var forward=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var right=new Vector3(-forward.z,0,forward.x);
                Grid("Scattered rice straw",1,1,(u,v)=>p+forward*(u-.5f)*length+right*(v-.5f)*.012f+Vector3.up*Mathf.Sin(u*Mathf.PI)*.018f,straw,Vector2.one,true);
            }
            for(int stack=0;stack<2;stack++)
            {
                var p=new Vector3(-w+.65f,0,4.3f+stack*.85f);
                for(int i=0;i<70;i++)
                {
                    float a=i*2.4f,r=.5f*Mathf.Sqrt((i%17)/17f);
                    var c=p+new Vector3(Mathf.Cos(a)*r,.07f,Mathf.Sin(a)*r);
                    Beam("Bundled dry fodder",c,c+new Vector3(-Mathf.Cos(a)*r*.45f,.52f,0),.018f,straw);
                }
                Tube("Twine around fodder",p+Vector3.up*.28f,.32f,.014f,S.Cream,Quaternion.Euler(90,0,0),24);
            }
            Random.state=randomState;
            LightingBuilder.Bulb(new Vector3(0,2.65f,.3f),7,1.4f,false);
            Parent=null;return root;
        }
        static void FeedingTrough(Vector3 p)
        {
            Box("Trough bottom",p+Vector3.up*.48f,new Vector3(.76f,.07f,3.1f),S.Wood);
            for(int side=-1;side<=1;side+=2)
            {
                Box("Trough side board",p+new Vector3(side*.4f,.66f,0),new Vector3(.065f,.38f,3.1f),S.Wood);
                Box("Trough end board",p+new Vector3(0,.66f,side*1.55f),new Vector3(.86f,.38f,.065f),S.Wood);
                for(int edge=-1;edge<=1;edge+=2)Box("Trough support leg",p+new Vector3(side*.3f,.25f,edge*1.2f),new Vector3(.1f,.5f,.1f),S.DarkWood);
            }
        }
        static void WaterTrough(Vector3 p)
        {
            var concrete=S.Plain("Weathered trough cement",new Color(.36f,.35f,.29f),.09f);
            Box("Water trough base",p+Vector3.up*.10f,new Vector3(1.6f,.2f,.85f),concrete);
            for(int side=-1;side<=1;side+=2)
            {
                Box("Water trough long rim",p+new Vector3(0,.32f,side*.4f),new Vector3(1.6f,.43f,.1f),concrete);
                Box("Water trough short rim",p+new Vector3(side*.75f,.32f,0),new Vector3(.1f,.43f,.85f),concrete);
            }
            Box("Still trough water",p+Vector3.up*.36f,new Vector3(1.4f,.008f,.65f),S.Water);
        }
    }
}
