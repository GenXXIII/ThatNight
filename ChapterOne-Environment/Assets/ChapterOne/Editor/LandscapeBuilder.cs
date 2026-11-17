using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class LandscapeBuilder
    {
        public static Transform BuildGround()
        {
            Parent=null;var root=Group("04 GROUND - soil lane and planted borders",Vector3.zero);Parent=root;
            for(int x=-5;x<5;x++)for(int z=-5;z<5;z++)
            {
                int cx=x,cz=z;
                Grid("Level forest terrain with road-only rise",12,12,(u,v)=>{float px=(cx+u)*12,pz=(cz+v)*12;return new Vector3(px,GroundDetailBuilder.HillHeight(px,pz),pz);},S.Grass,new Vector2(4,4),false,GroundDetailBuilder.Forest);
            }
            for(int x=0;x<6;x++)for(int z=0;z<12;z++)
            {
                int cx=x,cz=z;
                Grid("Packed soil courtyard",20,16,(u,v)=>{float px=-SiteLayout.HalfWidth+(cx+u)*SiteLayout.Width/6,pz=SiteLayout.Front+(cz+v)*SiteLayout.Depth/12;return new Vector3(px,GroundDetailBuilder.Height(px,pz),pz);},S.Soil,new Vector2(SiteLayout.Width/18,SiteLayout.Depth/36),false,GroundDetailBuilder.Yard);
            }
            Grid("Right-side forest approach terrain",48,120,(u,v)=>
            {
                float t=v*.79f;Vector2 center=GroundDetailBuilder.HillRoadCenter(t);
                Vector2 before=GroundDetailBuilder.HillRoadCenter(Mathf.Max(0,t-.003f)),after=GroundDetailBuilder.HillRoadCenter(Mathf.Min(1,t+.003f));
                Vector2 tangent=(after-before).normalized,side=new Vector2(tangent.y,-tangent.x),p=center+side*((u-.5f)*82);
                return new Vector3(p.x,GroundDetailBuilder.HillHeight(p.x,p.y)-.035f,p.y);
            },S.Grass,new Vector2(14,92),false,GroundDetailBuilder.Forest);
            Grid("Single rural road climbing to wedding house",18,140,GroundDetailBuilder.HillRoadPoint,S.Soil,new Vector2(2,33),false,GroundDetailBuilder.HillRoadShade);
            Random.InitState(2006);
            for(int i=0;i<240;i++)
            {
                float x=Random.Range(-SiteLayout.HalfWidth+.3f,SiteLayout.HalfWidth-.3f),z=Random.Range(SiteLayout.Front+.5f,SiteLayout.Back-.3f);
                if(Mathf.Abs(x)<8&&z<0&&i%4!=0)continue;
                float size=Random.Range(.018f,.075f);var rock=Ball("Scattered courtyard pebble",new Vector3(x,GroundDetailBuilder.Height(x,z)+.024f,z),new Vector3(size*1.6f,size*.65f,size),S.DarkWood);rock.transform.localEulerAngles=new Vector3(0,Random.Range(0,360),0);
            }
            GroundDetailBuilder.Dressing();
            Parent=null;return root;
        }
        public static Transform BuildVillage()
        {
            Parent=null;var root=Group("05 VILLAGE - wooded hometown with scattered houses",Vector3.zero);Parent=root;
            float edge=SiteLayout.HalfWidth,front=SiteLayout.Front,back=SiteLayout.Back;
            // Full-width ceremonial frontage based on the supplied wedding image.
            var villageRoot=Parent;BuildWeddingFrontage(front);Parent=villageRoot;
            // The approach is intentionally isolated: forest and one road, with no lower houses.
            Vector3[] homes={};
            DistantMountainView();
            LongApproachForest(homes);
            WoodedBorders(homes);
            HilltopForest(homes);
            HideWeddingFromLowerRoad(homes);
            UnderstoryBelt();
            for(int i=0;i<16;i++)
            {
                float a=i*Mathf.PI/8,r=32+i%3*7;var p=new Vector3(Mathf.Sin(a)*r,0,Mathf.Cos(a)*r+5);
                if(Mathf.Abs(p.x)<edge+2&&p.z>front-2&&p.z<back+2)p.x=(i%2==0?-1:1)*(edge+4);
                if(GroundDetailBuilder.DistanceToHillRoad(new Vector2(p.x,p.z))<6)continue;
                p.y=GroundDetailBuilder.HillHeight(p.x,p.z);
                Palm(p,9+i%3);
            }
            for(int i=0;i<10;i++)
            {
                var p=new Vector3(i%2==0?-edge+2.2f:edge-2,0,-10+i*5);
                var delta=p-SiteLayout.CattleShelterPosition;
                if(Mathf.Abs(delta.x)<SiteLayout.CattleShelterWidth/2+2&&Mathf.Abs(delta.z)<SiteLayout.CattleShelterDepth/2+2)p.z=SiteLayout.CattleShelterPosition.z-SiteLayout.CattleShelterDepth/2-2.5f;
                p.y=GroundDetailBuilder.HillHeight(p.x,p.z);
                Banana(p,i*63);
            }
            // Keep the guest seating perimeter free of the removed bench, jar and basket.
            Parent=null;return root;
        }
        static void BuildWeddingFrontage(float front)=>WeddingDraperyBuilder.Build(front);
        static void DistantMountainView()
        {
            var old=Parent;Parent=Group("Distant mountain view beyond the wedding",Vector3.zero);
            Grid("Lower forest valley beyond plateau",32,8,(u,v)=>
            {
                float x=Mathf.Lerp(-150,300,u),z=Mathf.Lerp(-430,-485,v);
                float y=Mathf.Lerp(1.2f,-2.5f,v)+(Mathf.PerlinNoise(x*.035f+9,z*.035f+13)-.5f)*1.2f;
                return new Vector3(x,y,z);
            },S.Grass,new Vector2(14,4),false,GroundDetailBuilder.Forest);
            Grid("Far forested mountain ridge",48,14,(u,v)=>
            {
                float x=Mathf.Lerp(-170,320,u),z=Mathf.Lerp(-475,-620,v);
                float ridge=SiteLayout.HillElevation+48+Mathf.Sin(x*.025f)*12+Mathf.Sin(x*.057f+1.7f)*7+(Mathf.PerlinNoise(x*.014f+33,7)-.5f)*18;
                float y=Mathf.Lerp(-2.4f,ridge,v*v*(3-2*v));
                return new Vector3(x,y,z);
            },S.Grass,new Vector2(16,6),false,GroundDetailBuilder.Forest);
            Parent=old;
        }
        static void LongApproachForest(Vector3[] homes)
        {
            Random.InitState(200615);int seed=1800;
            for(int i=0;i<28;i++)
            {
                float t=.01f+i*.0275f;Vector2 center=GroundDetailBuilder.HillRoadCenter(t);
                Vector2 before=GroundDetailBuilder.HillRoadCenter(Mathf.Max(0,t-.004f));
                Vector2 after=GroundDetailBuilder.HillRoadCenter(Mathf.Min(1,t+.004f));
                Vector2 tangent=(after-before).normalized,side=new Vector2(-tangent.y,tangent.x);
                for(int s=-1;s<=1;s+=2)
                {
                    float offset=12+(i%4)*5.3f;Vector2 p2=center+side*(s*offset);
                    ForestTree(new Vector3(p2.x,0,p2.y),homes,seed++);
                    if(i%3==0){Vector2 q=center+side*(s*8.4f);Shrub(new Vector3(q.x,GroundDetailBuilder.HillHeight(q.x,q.y),q.y),seed++);}
                }
            }
        }
        static void WoodedBorders(Vector3[] homes)
        {
            Random.InitState(2006);int seed=0;
            for(int x=0;x<17;x++)for(int z=0;z<4;z++)ForestTree(new Vector3(-56+x*7,0,-56+z*8),homes,seed++);
            for(int x=0;x<5;x++)for(int z=0;z<11;z++)
            {
                ForestTree(new Vector3(-56+x*7.5f,0,-18+z*7.4f),homes,seed++);
                ForestTree(new Vector3(30+x*6.5f,0,-18+z*7.4f),homes,seed++);
            }
            for(int x=0;x<9;x++)for(int z=0;z<2;z++)ForestTree(new Vector3(-23+x*6.3f,0,51+z*6),homes,seed++);
        }
        static void HilltopForest(Vector3[] homes)
        {
            int seed=900;Random.InitState(200612);
            for(int side=-1;side<=1;side+=2)for(int row=0;row<12;row++)
            {
                float z=-17+row*5.4f;ForestTree(new Vector3(side*(24+Random.Range(0,4f)),0,z),homes,seed++);
                if(row%2==0)ForestTree(new Vector3(side*(29+Random.Range(0,3f)),0,z+Random.Range(-2,2)),homes,seed++);
            }
            for(int col=0;col<13;col++)ForestTree(new Vector3(-24+col*4,0,49+Random.Range(0,3f)),homes,seed++);
            for(int col=0;col<12;col++)
            {
                float x=-28+col*5.2f;if(Mathf.Abs(x)<6)continue;
                ForestTree(new Vector3(x,0,-24+Random.Range(-2,1f)),homes,seed++);
            }
        }
        static void HideWeddingFromLowerRoad(Vector3[] homes)
        {
            // Dense trees on the inside of the right-hand approach conceal the
            // wedding until the player reaches the final front-right bend.
            Random.InitState(200614);int seed=1400;
            for(int i=0;i<19;i++)
            {
                float t=.70f+i*.013f;Vector2 center=GroundDetailBuilder.HillRoadCenter(t);
                Vector2 before=GroundDetailBuilder.HillRoadCenter(Mathf.Max(0,t-.012f));
                Vector2 after=GroundDetailBuilder.HillRoadCenter(Mathf.Min(1,t+.012f));
                Vector2 tangent=(after-before).normalized,left=new Vector2(-tangent.y,tangent.x);
                float offset=i%2==0?9.3f:12.2f;var p2=center+left*offset;
                ForestTree(new Vector3(p2.x,0,p2.y),homes,seed++);
                if(i%2==0)
                {
                    Vector2 shrub=center+left*7.2f;
                    Shrub(new Vector3(shrub.x,GroundDetailBuilder.HillHeight(shrub.x,shrub.y),shrub.y),seed++);
                }
            }
        }
        static void UnderstoryBelt()
        {
            Random.InitState(200613);int seed=0;
            for(int side=-1;side<=1;side+=2)for(int row=0;row<25;row++)
            {
                float z=-19+row*2.7f,x=side*Random.Range(22.5f,28.5f);
                if(GroundDetailBuilder.DistanceToHillRoad(new Vector2(x,z))<6)continue;
                Shrub(new Vector3(x,GroundDetailBuilder.HillHeight(x,z),z),seed++);
            }
            for(int col=0;col<28;col++)
            {
                float x=-27+col*2,z=Random.Range(48.5f,53f);
                var cattle=new Vector2(x-SiteLayout.CattleShelterPosition.x,z-SiteLayout.CattleShelterPosition.z);
                if(Mathf.Abs(cattle.x)<4&&Mathf.Abs(cattle.y)<8)continue;Shrub(new Vector3(x,GroundDetailBuilder.HillHeight(x,z),z),seed++);
            }
            for(int col=0;col<24;col++)
            {
                float x=-29+col*2.5f;if(Mathf.Abs(x)<6)continue;float z=Random.Range(-26f,-22.5f);
                if(GroundDetailBuilder.DistanceToHillRoad(new Vector2(x,z))<5)continue;Shrub(new Vector3(x,GroundDetailBuilder.HillHeight(x,z),z),seed++);
            }
        }
        static void Shrub(Vector3 p,int seed)
        {
            if(SiteLayout.InWeddingRoof(p.x,p.z,2.5f))return;
            var old=Parent;Parent=Group("Dense forest understory",p);
            float height=1.5f+(seed%5)*.32f;
            for(int stem=0;stem<5;stem++)
            {
                float a=stem*2.4f+seed,r=.2f+(stem%3)*.18f;var basePoint=new Vector3(Mathf.Sin(a)*r,0,Mathf.Cos(a)*r);
                Beam("Understory woody stem",basePoint,basePoint+new Vector3(Mathf.Sin(a)*.45f,height*(.68f+(stem%3)*.12f),Mathf.Cos(a)*.45f),.025f,S.Bark);
            }
            for(int cluster=0;cluster<9;cluster++)
            {
                float a=cluster*2.4f+seed,r=.3f+(cluster%4)*.28f;var center=new Vector3(Mathf.Sin(a)*r,height*(.55f+(cluster%3)*.18f),Mathf.Cos(a)*r);
                var rotation=Quaternion.Euler((cluster%3-1)*18,cluster*137+seed,(cluster%5-2)*11);
                Grid("Understory leaf mass",1,1,(u,v)=>center+rotation*new Vector3((u-.5f)*1.75f,(v-.5f)*1.45f,0),S.Foliage,Vector2.one,true);
            }
            Parent=old;
        }
        static void ForestTree(Vector3 p,Vector3[] homes,int seed)
        {
            p+=new Vector3(Random.Range(-1.1f,1.1f),0,Random.Range(-1.1f,1.1f));
            foreach(var home in homes)if(Mathf.Abs(p.x-home.x)<6.8f&&Mathf.Abs(p.z-home.z)<7.5f)return;
            if(GroundDetailBuilder.DistanceToHillRoad(new Vector2(p.x,p.z))<5.8f)return;
            var cattle=p-SiteLayout.CattleShelterPosition;if(Mathf.Abs(cattle.x)<5&&Mathf.Abs(cattle.z)<8)return;
            p.y=GroundDetailBuilder.HillHeight(p.x,p.z);
            Tree(p,Random.Range(9.5f,17.5f),seed);
            if(seed%3==0){var small=p+new Vector3(1.6f,0,-1.8f);small.y=GroundDetailBuilder.HillHeight(small.x,small.z);Tree(small,Random.Range(4.5f,7.2f),seed+400);}
            if(seed%9==0){var banana=p+new Vector3(-1.8f,0,1.5f);banana.y=GroundDetailBuilder.HillHeight(banana.x,banana.z);Banana(banana,seed*37);}
        }
        static void Fence(Vector3 a,Vector3 b,float height)
        {
            int n=Mathf.CeilToInt(Vector3.Distance(a,b)/.14f);
            for(int i=0;i<=n;i++){var p=Vector3.Lerp(a,b,i/(float)n);Box("Irregular bamboo fence slat",p+Vector3.up*(height/2),new Vector3(.065f,height+Mathf.Sin(i*13)*.12f,.055f),i%6==0?S.Bark:S.Wood);}
            Beam("Fence rail",a+Vector3.up*.5f,b+Vector3.up*.5f,.065f,S.DarkWood);Beam("Fence rail",a+Vector3.up*1.25f,b+Vector3.up*1.25f,.065f,S.DarkWood);
        }
        static void Neighbor(Vector3 p,float yaw,int color)
        {
            var old=Parent;Parent=Group("Neighboring Khmer home",p);Parent.localEulerAngles=new Vector3(0,yaw,0);
            Color[] colors={new Color(.34f,.13f,.09f),new Color(.16f,.30f,.30f),new Color(.48f,.45f,.34f)};
            var wall=S.Pbr("Neighbor painted boards "+color,"Planks037A",colors[color]);
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Box("Neighbor stilt",new Vector3(x*2.6f,1.1f,z*2.2f),new Vector3(.16f,2.2f,.16f),S.DarkWood);
            Box("Neighbor house",new Vector3(0,3.2f,0),new Vector3(5.7f,2.2f,4.8f),wall);
            StiltHouseBuilder.Roof(new Vector3(0,4.35f,0),6.7f,5.8f,1.6f,S.Rust);
            for(int s=-1;s<=1;s+=2)
            {
                Box("Neighbor shutter",new Vector3(s*1.7f,3.2f,-2.42f),new Vector3(.9f,1.2f,.05f),S.DarkWood);
                for(int j=0;j<9;j++)Box("Neighbor louvre",new Vector3(s*1.7f,2.72f+j*.11f,-2.47f),new Vector3(.82f,.06f,.03f),S.Wood);
            }
            Box("Neighbor doorway",new Vector3(0,3,-2.43f),new Vector3(.85f,1.7f,.05f),S.Black);
            for(int i=0;i<10;i++)Box("Neighbor stair tread",new Vector3(0,.12f+i*.21f,-5.2f+i*.27f),new Vector3(1.2f,.14f,.29f),S.Wood);
            Fence(new Vector3(-4,0,-5.5f),new Vector3(-.8f,0,-5.5f),1);Fence(new Vector3(.8f,0,-5.5f),new Vector3(4,0,-5.5f),1);
            Parent=old;
        }
        static void Tree(Vector3 p,float height,int seed)
        {
            if(SiteLayout.InWeddingRoof(p.x,p.z,6.5f))return;
            var old=Parent;Parent=Group(height>9?"Tall mature forest tree":"Forest understory tree",p);
            float trunk=Mathf.Lerp(.22f,.42f,Mathf.InverseLerp(5,18,height)),spread=Mathf.Lerp(1.7f,3.8f,Mathf.InverseLerp(5,18,height));
            Lathe("Tapered mature trunk",Vector3.zero,new[]{new Vector2(trunk*1.45f,0),new Vector2(trunk,.5f),new Vector2(trunk*.75f,height*.48f),new Vector2(trunk*.38f,height*.82f)},S.Bark,14);
            if(height>10)for(int root=0;root<5;root++)
            {float a=root*Mathf.PI*2/5+seed;Beam("Buttress root",new Vector3(Mathf.Cos(a)*trunk*1.25f,.06f,Mathf.Sin(a)*trunk*1.25f),new Vector3(Mathf.Cos(a)*trunk*.45f,1.15f,Mathf.Sin(a)*trunk*.45f),.16f,S.Bark);}
            for(int i=0;i<7;i++){float a=i*2.4f+seed;Beam("Spreading mature branch",Vector3.up*height*.48f,new Vector3(Mathf.Cos(a)*spread,height*(.73f+(i%3)*.045f),Mathf.Sin(a)*spread),Mathf.Lerp(.075f,.14f,height/18),S.Bark);}
            int clusters=height>9?30:18;
            for(int i=0;i<clusters;i++)
            {
                float a=i*2.4f+seed,r=spread*Mathf.Sqrt((i%15)/15f);var center=new Vector3(Mathf.Cos(a)*r,height*.76f+Mathf.Sin(i*2.6f)*height*.075f,Mathf.Sin(a)*r);
                var rotation=Quaternion.Euler((i%5-2)*22,i*137+seed,(i%3-1)*28);
                float clusterSize=Mathf.Lerp(2.5f,4.2f,Mathf.InverseLerp(5,18,height));
                Grid("Broad forest leaf cluster",1,1,(u,v)=>center+rotation*new Vector3((u-.5f)*clusterSize,(v-.5f)*clusterSize,0),S.Foliage,Vector2.one,true);
            }
            Parent=old;
        }
        static void Palm(Vector3 p,float height)
        {
            if(SiteLayout.InWeddingRoof(p.x,p.z,4))return;
            var old=Parent;Parent=Group("Village fan palm",p);
            Grid("Ringed palm trunk",12,18,(u,v)=>new Vector3(Mathf.Sin(u*Mathf.PI*2),0,Mathf.Cos(u*Mathf.PI*2))*(Mathf.Lerp(.24f,.17f,v)+Mathf.Sin(v*Mathf.PI*35)*.008f)+Vector3.up*v*height,S.Bark,new Vector2(1,height*.6f));
            for(int i=0;i<13;i++)
            {
                float a=i*2.4f;var dir=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));var side=new Vector3(dir.z,0,-dir.x);
                float elevation=i%3==0?1.15f:i%3==1?.55f:-.15f;
                var axis=dir*Mathf.Cos(elevation)+Vector3.up*Mathf.Sin(elevation);
                var hinge=Vector3.up*(height+(i%3-1)*.35f)+axis*(i%3==0?.35f:1.05f);
                Beam("Fan palm petiole",Vector3.up*(height-.2f),hinge,.035f,S.Leaf);
                Grid("Pleated fan palm leaf",32,5,(u,v)=>
                {
                    float fan=(u-.5f)*2.5f,r=v*(1.5f-.14f*Mathf.Abs(Mathf.Sin(u*Mathf.PI*16)));
                    return hinge+(axis*Mathf.Cos(fan)+side*Mathf.Sin(fan))*r+Vector3.up*(Mathf.Sin(v*Mathf.PI)*.22f-v*v*.30f+Mathf.Cos(u*Mathf.PI*32)*v*.045f);
                },S.Leaf,Vector2.one,true);
                for(int vein=0;vein<7;vein++){float fan=(vein/6f-.5f)*2.3f;Beam("Fan palm leaf rib",hinge,hinge+(axis*Mathf.Cos(fan)+side*Mathf.Sin(fan))*1.28f-Vector3.up*.2f,.012f,S.Leaf);}
            }
            Parent=old;
        }
        static void Banana(Vector3 p,float yaw)
        {
            if(SiteLayout.InWeddingRoof(p.x,p.z,2.5f))return;
            var old=Parent;Parent=Group("Banana garden",p);Parent.localEulerAngles=new Vector3(0,yaw,0);
            for(int stem=0;stem<2;stem++)
            {
                float height=2.3f+stem*.45f;var c=new Vector3(stem*.4f,0,0);Cylinder("Banana stem",c,.075f,height,S.Leaf);
                for(int leaf=0;leaf<6;leaf++)
                {
                    float a=leaf*Mathf.PI/3+stem;var d=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var side=new Vector3(d.z,0,-d.x);
                    Grid("Arched banana leaf",12,4,(u,v)=>c+Vector3.up*(height+Mathf.Sin(u*Mathf.PI)*.55f-u*.48f)+d*u*1.7f+side*(v-.5f)*.6f*Mathf.Sin(u*Mathf.PI),S.Leaf,Vector2.one,true);
                }
            }
            Parent=old;
        }
    }
}
