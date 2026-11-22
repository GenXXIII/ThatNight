var sb = new System.Text.StringBuilder();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
var midRoof = wedding.transform.Find("Middle roof over entrance aisle");
if (midRoof != null) {
    var mf = midRoof.GetComponent<UnityEngine.MeshFilter>();
    if (mf != null && mf.sharedMesh != null) {
        var mesh = mf.sharedMesh;
        sb.AppendLine("Mesh vertex count: " + mesh.vertexCount);
        // Find min/max and any vertices with X > 2.0 or Z between -4 and 2
        var verts = mesh.vertices;
        var tris = mesh.triangles;
        sb.AppendLine("Mesh bounds: " + mesh.bounds);
        
        // Let's find distinct connected clusters or check where poles are
    }
}

return sb.ToString();
