using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class StiltHouseBuilder
    {
        public static Transform Build()
        {
            Parent=null;var root=Group("01 HOUSE - raised timber home",SiteLayout.HousePosition);root.localRotation=SiteLayout.HouseRotation;root.localScale=Vector3.one*SiteLayout.HouseScale;Parent=root;
            float halfWidth=SiteLayout.HouseWidth/2,halfDepth=SiteLayout.HouseDepth/2;
            for(int x=-1;x<=1;x++)for(int z=-1;z<=1;z++)
            {
                var p=new Vector3(x*(halfWidth-.5f),0,z*(halfDepth-.55f));
                Box("Concrete footing",p+Vector3.up*.2f,new Vector3(.5f,.4f,.5f),S.Metal,true);
                Box("Weathered structural post",p+Vector3.up*2.6f,new Vector3(.23f,5.2f,.23f),S.DarkWood,true);
                Beam("Diagonal timber brace",p+Vector3.up*1.65f,p+new Vector3(.85f,2.42f,0),.11f,S.DarkWood);
            }
            Box("Floor collision",new Vector3(0,2.36f,0),new Vector3(SiteLayout.HouseWidth,.16f,SiteLayout.HouseDepth),S.DarkWood,true);
            int boards=Mathf.RoundToInt(SiteLayout.HouseWidth/.25f);float boardWidth=SiteLayout.HouseWidth/boards;
            for(int i=0;i<boards;i++)Box("Upper floor board",new Vector3(-halfWidth+(i+.5f)*boardWidth,2.455f,0),new Vector3(boardWidth-.006f,.09f,SiteLayout.HouseDepth),S.Floor,false,2.8f);
            // Vertical boards have real narrow gaps and varied tones, rather than a single flat wall.
            Wall(new Vector3(0,2.5f,-halfDepth),0,SiteLayout.HouseWidth,true,true);
            Wall(new Vector3(0,2.5f,halfDepth),180,SiteLayout.HouseWidth,false,true);
            Wall(new Vector3(-halfWidth,2.5f,0),90,SiteLayout.HouseDepth,false,true);
            Wall(new Vector3(halfWidth,2.5f,0),-90,SiteLayout.HouseDepth,false,true);
            for(int j=0;j<9;j++)
            {
                float x=Mathf.Lerp(-halfWidth+.4f,halfWidth-.4f,j/8f);
                Beam("Roof tie beam",new Vector3(x,5.05f,-halfDepth-.1f),new Vector3(x,5.05f,halfDepth+.1f),.16f,S.DarkWood);
                float h=5.18f+2.2f*Mathf.Clamp01((halfWidth+.5f-Mathf.Abs(x))/((SiteLayout.HouseDepth+1.2f)*.35f));
                Beam("Exposed rafter",new Vector3(x,5.14f,-halfDepth-.2f),new Vector3(x,h,0),.095f,S.DarkWood);
                Beam("Exposed rafter",new Vector3(x,5.14f,halfDepth+.2f),new Vector3(x,h,0),.095f,S.DarkWood);
            }
            Roof(new Vector3(0,5.25f,0),SiteLayout.HouseWidth+1.2f,SiteLayout.HouseDepth+1.2f,2.2f,S.Roof,true);
            // Short raised entrance landing and a full stair flight.
            Box("Entrance landing",new Vector3(0,2.4f,-halfDepth-.48f),new Vector3(2.0f,.18f,1),S.Floor,true);
            for(int i=0;i<14;i++)Box("Solid stair tread",new Vector3(0,.085f+i*.175f,-halfDepth-4.35f+i*.255f),new Vector3(1.65f,.16f,.28f),S.Wood,true);
            for(int side=-1;side<=1;side+=2)
            {
                Beam("Stair side stringer",new Vector3(side*.76f,.02f,-halfDepth-4.5f),new Vector3(side*.76f,2.42f,-halfDepth-.55f),.13f,S.DarkWood);
                Beam("Worn stair handrail",new Vector3(side*.91f,.87f,-halfDepth-4.5f),new Vector3(side*.91f,3.27f,-halfDepth-.55f),.08f,S.Wood);
                for(int i=0;i<7;i++)Box("Stair baluster",new Vector3(side*.91f,.55f+i*.35f,-halfDepth-4.32f+i*.54f),new Vector3(.045f,.8f,.045f),S.Wood);
            }
            // The front is one large living room; two separate bedrooms share the rear.
            const float partitionZ=1,doorHalfWidth=.6f;
            float bedroomCenter=halfWidth/2;
            for(int side=-1;side<=1;side+=2)
            {
                float doorX=side*bedroomCenter;
                Box("Bedroom doorway header",new Vector3(doorX,4.975f,partitionZ),new Vector3(1.2f,.45f,.12f),S.Wood,true);
                for(int edge=-1;edge<=1;edge+=2)Box("Bedroom door jamb",new Vector3(doorX+edge*.64f,3.62f,partitionZ),new Vector3(.08f,2.24f,.16f),S.DarkWood,true);
                float outerWidth=halfWidth-bedroomCenter-doorHalfWidth;
                Box("Bedroom outer partition",new Vector3(side*(halfWidth-outerWidth/2),3.85f,partitionZ),new Vector3(outerWidth,2.7f,.12f),S.Wood,true);
            }
            Box("Bedroom central partition",new Vector3(0,3.85f,partitionZ),new Vector3(2*(bedroomCenter-doorHalfWidth),2.7f,.12f),S.Wood,true);
            Box("Wall separating the two bedrooms",new Vector3(0,3.85f,(partitionZ+halfDepth)/2),new Vector3(.12f,2.7f,halfDepth-partitionZ),S.Wood,true);
            var roomParent=Parent;Parent=Group("Large living room",Vector3.zero);
            PropsBuilder.Bench(new Vector3(-halfWidth+2,2.5f,-3.8f),2.5f);
            PropsBuilder.Table(new Vector3(-halfWidth+2.1f,2.5f,-2.4f),new Vector2(1.7f,.8f),.52f,S.DarkWood);
            PropsBuilder.Cabinet(new Vector3(-halfWidth+1.3f,2.5f,.5f));
            PropsBuilder.CeramicSet(new Vector3(-halfWidth+2.1f,3.04f,-2.4f));
            // The right bay is reserved for the couple's floor ceremony; no bench here.
            LightingBuilder.Bulb(new Vector3(0,4.7f,-1.5f),7,1.7f,false);
            Parent=roomParent;
            HouseWeddingBuilder.Build();
            BedroomBuilder.Build("Bedroom 1 - bride room",-bedroomCenter,halfDepth,true);
            BedroomBuilder.Build("Bedroom 2 - family room",bedroomCenter,halfDepth,false);
            UnderHouseSittingBuilder.Build(new Vector3(-4,.03f,-3.8f));
            PropsBuilder.Jar(new Vector3(-3.5f,.03f,-2),.9f);
            PropsBuilder.Basket(new Vector3(-3,.03f,.5f),.75f);
            PropsBuilder.Basket(new Vector3(-2.1f,.03f,.7f),.95f);
            PropsBuilder.Bench(new Vector3(-2,0,2.4f),2.5f);
            Grid("Hanging hammock",32,12,(u,v)=>new Vector3(-.1f+u*3.4f,1.55f-Mathf.Sin(u*Mathf.PI)*.68f+Mathf.Abs(v-.5f)*.23f,1.8f+(v-.5f)*.9f),S.Cream,new Vector2(3,1),true);
            LightingBuilder.Bulb(new Vector3(-.25f,4.8f,-halfDepth-.2f),6.8f,3.5f,true);
            LightingBuilder.Bulb(new Vector3(1.2f,2.18f,-1.8f),5,1.8f,false);
            Parent=null;return root;
        }
        static void Wall(Vector3 p,float yaw,float width,bool doorway,bool windows)
        {
            var previous=Parent;Parent=Group("Vertical plank wall and shutters",p);Parent.localEulerAngles=new Vector3(0,yaw,0);
            int count=Mathf.RoundToInt(width/.19f);
            for(int i=0;i<count;i++)
            {
                float x=-width/2+(i+.5f)*width/count;bool door=doorway&&Mathf.Abs(x)<.63f;
                bool win=windows&&(Mathf.Abs(x-width*.31f)<.53f||Mathf.Abs(x+width*.31f)<.53f);
                if(door)Box("Over-door boarding",new Vector3(x,2.5f,0),new Vector3(width/count-.006f,.4f,.10f),S.Wood);
                else if(win)
                {Box("Window lower boarding",new Vector3(x,.42f,0),new Vector3(width/count-.006f,.84f,.10f),S.Wood);Box("Window upper boarding",new Vector3(x,2.43f,0),new Vector3(width/count-.006f,.54f,.10f),S.Wood);}
                else Box("Vertical weathered board",new Vector3(x,1.35f,Mathf.Sin(i*5.3f)*.012f),new Vector3(width/count-.006f,2.7f,.10f),i%7==0?S.DarkWood:S.Wood);
            }
            for(int side=-1;side<=1;side+=2)
            {
                float x=side*width*.31f;
                Box("Window sill",new Vector3(x,.83f,-.06f),new Vector3(1.22f,.08f,.24f),S.DarkWood);
                Box("Window header",new Vector3(x,2.18f,-.04f),new Vector3(1.22f,.09f,.17f),S.DarkWood);
                for(int hinge=-1;hinge<=1;hinge+=2)
                {
                    var save=Parent;Parent=Group("Louvred shutter",new Vector3(x+hinge*.57f,1.5f,-.08f));Parent.localEulerAngles=new Vector3(0,hinge*38,0);
                    for(int j=0;j<14;j++){var l=Box("Timber louvre",new Vector3(hinge*.27f,-.58f+j*.089f,0),new Vector3(.53f,.082f,.026f),S.Wood);l.transform.localEulerAngles=new Vector3(-25,0,0);}
                    for(int edge=0;edge<2;edge++)Box("Shutter upright",new Vector3(hinge*edge*.54f,0,0),new Vector3(.045f,1.28f,.06f),S.DarkWood);
                    Parent=save;
                }
            }
            Box("Wall base rail",new Vector3(0,.02f,0),new Vector3(width,.18f,.19f),S.DarkWood);
            if(doorway)
            {
                for(int s=-1;s<=1;s+=2)Box("Entrance frame",new Vector3(s*.67f,1.18f,-.06f),new Vector3(.12f,2.36f,.22f),S.DarkWood);
                Box("Entrance lintel",new Vector3(0,2.35f,-.06f),new Vector3(1.5f,.14f,.23f),S.DarkWood);
            }
            Parent=previous;
        }
        public static void Roof(Vector3 p,float width,float depth,float height,Material mat,bool detailed=false)
        {
            float ridge=Mathf.Max(.1f,width/2-depth*.35f);
            for(int side=-1;side<=1;side+=2)
            {
                int s=side;
                Grid("Hipped roof front and rear",12,8,(u,v)=>p+new Vector3(Mathf.Lerp(-width/2,width/2,u)*(1-v)+Mathf.Lerp(-ridge,ridge,u)*v,height*v,s*depth/2*(1-v)),mat,new Vector2(width/2,depth/3),true);
                Grid("Hipped roof end",8,8,(u,v)=>p+new Vector3(s*(width/2*(1-v)+ridge*v),height*v,(u-.5f)*depth*(1-v)),mat,new Vector2(depth/2,2),true);
                Beam("Fascia",p+new Vector3(-width/2,-.06f,s*depth/2),p+new Vector3(width/2,-.06f,s*depth/2),.16f,S.DarkWood);
                for(int e=-1;e<=1;e+=2)Beam("Hip tile capping",p+new Vector3(e*width/2,.03f,s*depth/2),p+new Vector3(e*ridge,height+.06f,0),.12f,mat);
                if(detailed)for(int i=0;i<50;i++)
                {
                    float x=-width/2+(i+.5f)*width/50;
                    Box("Uneven clay tile eave",p+new Vector3(x,-.025f,s*(depth/2+.02f)),new Vector3(width/50-.008f,.055f,.23f),mat);
                }
            }
            Beam("Clay ridge cap",p+new Vector3(-ridge,height+.07f,0),p+new Vector3(ridge,height+.07f,0),.19f,mat);
            if(detailed)for(int side=-1;side<=1;side+=2)
            {Beam("Gable finial",p+new Vector3(side*ridge,height,0),p+new Vector3(side*(ridge+.18f),height+.5f,0),.075f,S.DarkWood);}
        }
    }
}
