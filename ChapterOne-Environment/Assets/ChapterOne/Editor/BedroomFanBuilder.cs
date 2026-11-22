using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class BedroomFanBuilder
    {
        public static void Build(Vector3 p, float yaw = 0f, float wallX = -3.74f)
        {
            var old = Parent;
            Parent = ModelGroup("Older pedestal bedroom fan", p);
            var fanGroup = Parent;
            if (yaw != 0f) fanGroup.localEulerAngles = new Vector3(0, yaw, 0);
            var green = S.Plain("Faded green fan enamel", new Color(.29f, .39f, .23f), .24f);
            Ball("Heavy fan base", new Vector3(0, .07f, 0), new Vector3(.48f, .14f, .39f), green);
            Cylinder("Fan pedestal", new Vector3(0, .10f, 0), .045f, .85f, green);
            var hub = new Vector3(0, 1.17f, -.03f);
            Ball("Fan motor housing", hub + Vector3.forward * .12f, new Vector3(.18f, .18f, .26f), green);
            for (int i = 0; i < 3; i++)
            {
                float a = i * Mathf.PI * 2 / 3;
                var blade = Ball("Three blade fan rotor", hub + new Vector3(Mathf.Sin(a) * .13f, Mathf.Cos(a) * .13f, 0), new Vector3(.13f, .30f, .025f), green);
                blade.transform.localRotation = Quaternion.Euler(0, 0, -a * Mathf.Rad2Deg);
            }
            for (int side = -1; side <= 1; side += 2)
            {
                var centre = hub + Vector3.forward * (side * .065f);
                for (int ring = 1; ring <= 5; ring++) Tube("Fan wire guard ring", centre, .06f * ring, .0035f, S.Metal, Quaternion.identity, 48);
                for (int i = 0; i < 24; i++)
                {
                    float a = i * Mathf.PI / 12;
                    Beam("Fan guard spoke", centre, centre + new Vector3(Mathf.Sin(a) * .30f, Mathf.Cos(a) * .30f, 0), .0035f, S.Metal);
                }
            }
            Ball("Fan central cap", hub - Vector3.forward * .085f, new Vector3(.11f, .11f, .035f), S.Cream);

            // Cord exit on fan base
            Cable("Fan electrical cord stub", new Vector3(0, .07f, .12f), new Vector3(0, .02f, .24f), .02f, .008f, S.Black);

            Parent = old;
            ModelCombiner.CombineModel(fanGroup, addCollider: true, useCapsuleCollider: true);

            // Vintage surface-mounted wall electrical outlet socket and fan power plug
            BuildWallSocketAndPlug(p, wallX);
        }

        static void BuildWallSocketAndPlug(Vector3 fanPos, float wallX)
        {
            var old = Parent;
            // Place socket on the timber wall beside the fan
            Parent = ModelGroup("Bedroom wall electrical socket and fan plug", new Vector3(wallX, fanPos.y, fanPos.z));
            var socketGroup = Parent;

            var socketP = new Vector3(0.015f, .48f, 0);

            // 1. Contrasting teak timber mounting backplate on the timber wall (0.13m x 0.13m x 0.02m)
            Box("Socket timber mounting block", socketP + new Vector3(.012f, 0, 0), new Vector3(.022f, .13f, .13f), S.Wood);

            // 2. Vintage ivory/cream Bakelite surface duplex wall outlet (0.095m x 0.095m x 0.035m)
            Box("Surface wall socket body", socketP + new Vector3(.036f, 0, 0), new Vector3(.030f, .095f, .095f), S.Cream, true);
            // Front faceplate screw
            Ball("Faceplate center screw", socketP + new Vector3(.052f, 0, 0), Vector3.one * .006f, S.Metal);

            // Upper and lower round socket receptacles
            for (int s = -1; s <= 1; s += 2)
            {
                var recP = socketP + new Vector3(.052f, s * .026f, 0);
                Cylinder("Socket round receptacle rim", recP, .016f, .003f, S.DarkWood, 12);
                Ball("Socket pinhole 1", recP + new Vector3(.002f, 0, -.008f), Vector3.one * .005f, S.Black);
                Ball("Socket pinhole 2", recP + new Vector3(.002f, 0, .008f), Vector3.one * .005f, S.Black);
            }

            // 3. Exposed twin electrical wiring conduit running vertically up the timber wall to ceiling/rafter
            Beam("Wall wiring conduit wire", socketP + new Vector3(.018f, .065f, 0), socketP + new Vector3(.018f, 2.1f, 0), .007f, S.Cream);

            // 4. Two-Pin Fan Power Plug inserted into lower socket
            var plugP = socketP + new Vector3(.052f, -.026f, 0);
            Box("Fan 2-pin power plug body", plugP + new Vector3(.016f, 0, 0), new Vector3(.026f, .032f, .026f), S.Black);
            Cylinder("Plug rubber strain relief", plugP + new Vector3(.029f, 0, 0), .006f, .012f, S.Black, 8);

            // 5. Fan Power Cable: runs from fan base across floor and curves gracefully up the wall into the plug
            float dx = fanPos.x - wallX; // distance from wall to fan (~0.65m to 0.85m)
            Vector3 cordFanBase = new Vector3(dx, .02f, 0);
            Vector3 cordFloorMid = new Vector3(dx * 0.45f, .018f, .06f);
            Vector3 cordWallBase = new Vector3(.04f, .02f, .02f);
            Vector3 cordPlugEntry = plugP + new Vector3(.040f, 0, 0);

            // Smooth 3-segment drooping cable path connecting fan directly to plug
            Cable("Power cord segment 1 (fan to floor)", cordFanBase, cordFloorMid, .025f, .007f, S.Black);
            Cable("Power cord segment 2 (floor run)", cordFloorMid, cordWallBase, .025f, .007f, S.Black);
            Cable("Power cord segment 3 (wall climb to plug)", cordWallBase, cordPlugEntry, .035f, .007f, S.Black);

            Parent = old;
            ModelCombiner.CombineModel(socketGroup, addCollider: true);
        }
    }
}
