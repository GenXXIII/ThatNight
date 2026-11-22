using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    public static class GameplayOpeningBuilder
    {
        public static int Apply()
        {
            var house = GameObject.Find("01 HOUSE - raised timber home") ?? GameObject.Find("01 HOUSE");
            if (!house) return 0;
            int count = 0;
            foreach (var filter in house.GetComponentsInChildren<MeshFilter>())
            {
                if (filter.GetComponentsInChildren<HingedOpening>().Length > 0) continue;
                if (filter.name == "Double wooden entrance doors")
                {
                    for (int side = -1; side <= 1; side += 2)
                    {
                        int s = side;
                        var leaf = OpeningMeshSplitter.Extract(filter, "Entrance door " + (side < 0 ? "left" : "right"), new Vector3(side * .54f, 1.18f, 0), p => p.x * s > 0);
                        // Sits completely flush and tight when closed (0 degrees).
                        // Swings INWARD into living room (+Z) when opened (-90 for left, +90 for right).
                        float closed = 0f;
                        float open = side * 90f;
                        count += Opening(leaf, "entrance door", closed, open, false);
                    }
                }
                else if (filter.name == "Bedroom partition wall and headers")
                {
                    for (int side = -1; side <= 1; side += 2)
                    {
                        int s = side;
                        var pivot = new Vector3(side * (SiteLayout.HouseWidth * .25f + .59f), 2.5f, 1);
                        var leaf = OpeningMeshSplitter.Extract(filter, "Bedroom door " + (side < 0 ? "bridal" : "family"), pivot, p => {
                            var q = p - pivot;
                            return q.x * -s > -.015f && q.x * -s < 1.205f && q.y > -.01f && q.y < 2.26f && Mathf.Abs(q.z) < .13f;
                        });
                        // Sits flush in doorway when closed (0 degrees).
                        // Swings INWARD into bedrooms (+Z) when opened (+90 for bridal, -90 for family).
                        float closed = 0f;
                        float open = -side * 90f;
                        count += Opening(leaf, side < 0 ? "bridal bedroom door" : "family bedroom door", closed, open, false);
                    }
                }
                else if (filter.name == "Vertical plank wall and shutters")
                {
                    float width = filter.sharedMesh.bounds.size.x;
                    for (int window = -1; window <= 1; window += 2)
                    {
                        for (int side = -1; side <= 1; side += 2)
                        {
                            float center = window * width * .28f;
                            int s = side;
                            var leaf = OpeningMeshSplitter.Extract(filter, "Window " + window + " shutter " + side, new Vector3(center + side * .41f, 1.6f, -.01f), p => p.z < -.01f && p.y > 1.115f && p.y < 2.085f && Mathf.Abs(p.x - center) < .65f && (p.x - center) * s > 0);
                            // Windows swing OUTWARD into exterior (0 = open outward, side*90 = closed flat against wall)
                            count += Opening(leaf, "window shutter", side * 90, 0, true);
                        }
                    }
                }
                else if (filter.name == "Teak wardrobe with louvered doors")
                {
                    for (int side = -1; side <= 1; side += 2)
                    {
                        int s = side;
                        var leaf = OpeningMeshSplitter.Extract(filter, "Wardrobe door " + side, new Vector3(side * .51f, 1.25f, -.26f), p => p.x * s > 0 && Mathf.Abs(p.x) < .52f && p.z > -.31f && p.z < -.24f && p.y > .42f && p.y < 2.08f);
                        count += Opening(leaf, "wardrobe door", 0, -side * 100, false);
                    }
                    var front = OpeningMeshSplitter.Extract(filter, "Remove solid wardrobe front", Vector3.zero, p => Mathf.Abs(p.z + .275f) < .001f && p.y < 2.21f);
                    if (front) Object.DestroyImmediate(front.gameObject);
                    var interior = CharacterArtBuilder.Group("Wardrobe interior", filter.transform, Vector3.zero);
                    var woodMat = CharacterArtBuilder.CharcoalTrousers ?? CharacterArtBuilder.Mat("Wardrobe Dark Teak", new Color(0.22f, 0.14f, 0.08f), 0.35f);
                    CharacterArtBuilder.Part("Interior back", interior, new Vector3(0, 1.25f, .24f), new Vector3(1.08f, 1.9f, .03f), woodMat, PrimitiveType.Cube);
                    foreach (float y in new[] { .43f, 1.2f, 2.12f })
                        CharacterArtBuilder.Part("Wardrobe shelf", interior, new Vector3(0, y, 0), new Vector3(1.08f, .035f, .48f), woodMat, PrimitiveType.Cube);
                }
            }
            // Replace the old room-sized collision boxes with the visible geometry.
            foreach(var filter in house.GetComponentsInChildren<MeshFilter>())
            {
                if(filter.GetComponentInParent<HingedOpening>())continue;
                var box=filter.GetComponent<BoxCollider>();if(!box)continue;box.enabled=false;
                var collision=filter.GetComponent<MeshCollider>();if(!collision)collision=filter.gameObject.AddComponent<MeshCollider>();collision.sharedMesh=filter.sharedMesh;
            }
            return count;
        }
        static int Opening(Transform leaf,string displayName,float closed,float open,bool initiallyOpen=true)
        {
            if(!leaf)return 0;
            var hinge=leaf.gameObject.AddComponent<HingedOpening>();hinge.displayName=displayName;hinge.closedAngle=closed;hinge.openAngle=open;hinge.isOpen=initiallyOpen;
            hinge.handleLocal=leaf.GetComponent<MeshFilter>().sharedMesh.bounds.center;
            if(displayName.Contains("bridal bedroom door")) hinge.triggerDirectorOnOpen = true;
            leaf.localRotation=Quaternion.Euler(0,initiallyOpen?open:closed,0);
            return 1;
        }
    }
}
