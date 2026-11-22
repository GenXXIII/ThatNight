using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    public static partial class PropsBuilder
    {
        // =========================================================================================
        // AUTHENTIC 2006 CAMBODIAN HOUSEHOLD & WEDDING PROPS
        // =========================================================================================

        public static void CambodianWallClock(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Cambodian wooden wall clock", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            // Wooden rectangular case with pediment top
            Box("Clock wooden case", Vector3.zero, new Vector3(.24f, .42f, .06f), S.DarkWood);
            Box("Clock top crown", Vector3.up * .22f, new Vector3(.26f, .035f, .07f), S.Wood);
            Box("Clock base ledge", Vector3.down * .22f, new Vector3(.26f, .035f, .07f), S.Wood);

            // White clock face dial
            Box("Clock white dial", new Vector3(0, .08f, -.032f), new Vector3(.18f, .18f, .005f), S.Porcelain);
            Tube("Clock brass bezel", new Vector3(0, .08f, -.034f), .088f, .008f, S.Brass, Quaternion.identity, 20);
            Ball("Hand center hub", new Vector3(0, .08f, -.037f), Vector3.one * .014f, S.Brass);
            Beam("Hour hand", new Vector3(0, .08f, -.036f), new Vector3(.035f, .11f, -.036f), .005f, S.Black);
            Beam("Minute hand", new Vector3(0, .08f, -.036f), new Vector3(-.02f, .145f, -.036f), .004f, S.Black);

            // Lower pendulum window
            Box("Pendulum glass panel", new Vector3(0, -.11f, -.032f), new Vector3(.14f, .15f, .004f), S.Metal);
            Beam("Pendulum brass rod", new Vector3(0, -.02f, -.025f), new Vector3(0, -.14f, -.025f), .004f, S.Brass);
            Ball("Pendulum brass bob", new Vector3(0, -.14f, -.025f), new Vector3(.05f, .05f, .012f), S.Brass);

            var clockGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(clockGroup, addCollider: false);
        }

        public static void CambodianLunarCalendar(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Cambodian Buddhist lunar wall calendar", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var redHeader = S.Plain("Calendar crimson header", new Color(.78f, .14f, .16f), .25f);
            var goldTrim = S.Plain("Calendar golden emblem", new Color(.88f, .72f, .18f), .40f);
            var paperBlock = S.Plain("Calendar paper block", new Color(.94f, .92f, .86f), .08f);

            // Stiff cardboard mounting backboard
            Box("Calendar backboard", Vector3.zero, new Vector3(.26f, .48f, .008f), redHeader);
            // Golden temple emblem on top header
            Box("Header gold border", new Vector3(0, .14f, -.005f), new Vector3(.22f, .14f, .002f), goldTrim);
            Ball("Temple spire motif", new Vector3(0, .17f, -.007f), new Vector3(.06f, .08f, .004f), goldTrim);

            // Thick daily tear-off paper pad
            Box("Tear-off date pad", new Vector3(0, -.10f, -.014f), new Vector3(.20f, .22f, .020f), paperBlock);
            Box("Today date crimson digit", new Vector3(0, -.10f, -.025f), new Vector3(.09f, .11f, .002f), redHeader);

            var calGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(calGroup, addCollider: false);
        }

        public static void KromaPegRack(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional Cambodian Kroma peg rack", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var redKroma = S.Plain("Red checked Kroma scarf", new Color(.72f, .24f, .22f), .15f);
            var blueKroma = S.Plain("Indigo checked Kroma scarf", new Color(.24f, .34f, .55f), .15f);

            // Wall mounting wood strip
            Box("Wood peg rail", Vector3.zero, new Vector3(.55f, .07f, .02f), S.DarkWood);

            // 3 Turned wooden wall pegs
            for (int i = -1; i <= 1; i++)
            {
                var pegPos = new Vector3(i * .18f, 0, -.035f);
                Beam("Turned peg", pegPos, pegPos + new Vector3(0, .02f, -.06f), .018f, S.Wood);
                Ball("Peg round knob", pegPos + new Vector3(0, .02f, -.065f), Vector3.one * .024f, S.Wood);
            }

            // Scarf 1: Red checked Kroma draped over left peg
            Grid("Red Kroma draped scarf", 8, 14, (u, v) =>
            {
                float x = -.18f + (u - .5f) * .12f;
                float y = .02f - v * .45f - Mathf.Sin(u * Mathf.PI) * .02f;
                float z = -.07f - Mathf.Sin(v * Mathf.PI * 3f) * .015f - (1f - v) * .02f;
                return new Vector3(x, y, z);
            }, redKroma, new Vector2(2, 6), true);

            // Scarf 2: Indigo checked Kroma draped over right peg
            Grid("Indigo Kroma draped scarf", 8, 14, (u, v) =>
            {
                float x = .18f + (u - .5f) * .13f;
                float y = .02f - v * .48f - Mathf.Sin(u * Mathf.PI) * .025f;
                float z = -.07f - Mathf.Sin(v * Mathf.PI * 2.5f) * .018f - (1f - v) * .02f;
                return new Vector3(x, y, z);
            }, blueKroma, new Vector2(2, 6), true);

            var rackGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(rackGroup, addCollider: false);
        }

        public static void KhmerThermosAndTeaSet(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional Cambodian thermos flask and tea tray", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var redTin = S.Plain("Chinese floral red tin flask", new Color(.78f, .16f, .18f), .45f);

            // Round galvanized / brass serving tray
            Lathe("Tea serving tray", Vector3.zero, new[]{
                new Vector2(0, 0), new Vector2(.22f, 0), new Vector2(.24f, .025f)
            }, S.Metal, 20);

            // Iconic Chinese floral Red Vacuum Thermos Flask (Phnoung Teuk Kdao)
            var flaskPos = new Vector3(-.08f, .01f, 0);
            Cylinder("Thermos tin body", flaskPos, .072f, .28f, redTin, 18);
            Lathe("Thermos aluminum shoulder", flaskPos + Vector3.up * .28f, new[]{
                new Vector2(.072f, 0), new Vector2(.038f, .045f), new Vector2(.038f, .07f)
            }, S.Metal, 16);
            Ball("Thermos red drinking cup lid", flaskPos + Vector3.up * .37f, new Vector3(.07f, .05f, .07f), redTin);
            // Chrome carry handle
            Tube("Thermos chrome side handle", flaskPos + new Vector3(-.075f, .18f, 0), .08f, .008f, S.Metal, Quaternion.Euler(0, 0, 90), 16);

            // Porcelain Chinese Teapot
            var potPos = new Vector3(.08f, .01f, -.04f);
            Lathe("Porcelain teapot body", potPos, new[]{
                new Vector2(0, 0), new Vector2(.06f, .02f), new Vector2(.075f, .06f), new Vector2(.04f, .10f)
            }, S.Porcelain, 16);
            Ball("Teapot lid knob", potPos + Vector3.up * .11f, Vector3.one * .015f, S.Porcelain);
            Beam("Teapot curved spout", potPos + new Vector3(.06f, .05f, 0), potPos + new Vector3(.11f, .09f, 0), .012f, S.Porcelain);

            // 3 Small Porcelain Teacups
            for (int i = 0; i < 3; i++)
            {
                float a = i * Mathf.PI * 2f / 3f + .5f;
                var cupPos = new Vector3(.08f + Mathf.Cos(a) * .09f, .01f, .07f + Mathf.Sin(a) * .07f);
                Lathe("Porcelain teacup", cupPos, new[]{
                    new Vector2(.018f, 0), new Vector2(.028f, .035f), new Vector2(.026f, .040f)
                }, S.Porcelain, 12);
            }

            var teaGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(teaGroup, addCollider: true);
        }

        public static void CambodianFootwear(Vector3 p, float yaw, int pairCount = 2)
        {
            var old = Parent;
            Parent = ModelGroup("Cambodian sandals and slippers", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var blueRubber = S.Plain("Blue rubber sandal", new Color(.20f, .40f, .75f), .20f);
            var greenRubber = S.Plain("Green foam slipper", new Color(.25f, .65f, .35f), .20f);
            var redRubber = S.Plain("Red flip-flop", new Color(.78f, .22f, .18f), .20f);

            Material[] slipperMats = { blueRubber, greenRubber, redRubber };

            for (int pair = 0; pair < pairCount; pair++)
            {
                float pz = pair * .32f;
                var mat = slipperMats[pair % slipperMats.Length];

                for (int foot = -1; foot <= 1; foot += 2)
                {
                    float px = foot * .075f + (pair * .03f);
                    var footPos = new Vector3(px, .008f, pz);

                    // Oval flat foam sole
                    Box("Sandal foam sole", footPos, new Vector3(.095f, .016f, .24f), mat);
                    // Y-strap flip-flop bands
                    Beam("Thong toe strap", footPos + new Vector3(0, .008f, .06f), footPos + new Vector3(0, .038f, .035f), .012f, S.Black);
                    Beam("Left instep strap", footPos + new Vector3(0, .038f, .035f), footPos + new Vector3(-.042f, .01f, -.03f), .012f, mat);
                    Beam("Right instep strap", footPos + new Vector3(0, .038f, .035f), footPos + new Vector3(.042f, .01f, -.03f), .012f, mat);
                }
            }

            var footGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(footGroup, addCollider: false);
        }

        public static void WeddingReceptionTable(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Wedding reception guestbook and money donation table", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var pinkSilk = S.Plain("Reception pink silk skirt", new Color(.88f, .48f, .58f), .30f);
            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redCloth = S.Plain("Ceremony red cloth", new Color(.72f, .16f, .18f), .25f);

            // Table base and draped silk tablecloth
            Box("Table top wooden board", Vector3.up * .78f, new Vector3(1.6f, .05f, .85f), S.Wood);
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Box("Table steel leg", new Vector3(x * .72f, .38f, z * .36f), new Vector3(.04f, .76f, .04f), S.Metal);

            // Silk skirt draped all around
            Box("Pleated pink tablecloth skirt", Vector3.up * .39f, new Vector3(1.64f, .78f, .89f), pinkSilk);
            Box("Gold top runner overlay", Vector3.up * .79f, new Vector3(1.58f, .015f, .55f), goldSilk);

            // 1. Signature Cambodian Heart-Shaped Wedding Donation Box (Prabop Cham Noung)
            var boxPos = new Vector3(-.42f, .80f, 0);
            Box("Wedding money box pedestal", boxPos + Vector3.up * .02f, new Vector3(.38f, .04f, .32f), goldSilk);
            Box("Money donation box body", boxPos + Vector3.up * .22f, new Vector3(.32f, .36f, .28f), pinkSilk);
            // Gold trim & roof-shaped lid
            Box("Money box golden lid", boxPos + Vector3.up * .42f, new Vector3(.35f, .04f, .30f), goldSilk);
            // Cash envelope deposit slot
            Box("Envelope deposit slot", boxPos + Vector3.up * .442f, new Vector3(.14f, .005f, .015f), S.Black);
            Ball("Top golden blessing rosette", boxPos + Vector3.up * .465f, Vector3.one * .055f, goldSilk);

            // 2. Guest Signature Book (Sievphov Cham Noung)
            var bookPos = new Vector3(.18f, .80f, -.05f);
            Box("Open guestbook left page", bookPos + new Vector3(-.12f, .01f, 0), new Vector3(.22f, .012f, .28f), S.Cream);
            Box("Open guestbook right page", bookPos + new Vector3(.12f, .01f, 0), new Vector3(.22f, .012f, .28f), S.Cream);
            Box("Red silk book cover trim", bookPos + Vector3.down * .004f, new Vector3(.48f, .014f, .30f), redCloth);
            // Ruled name lines on guestbook
            for (int line = 0; line < 5; line++)
            {
                Box("Guest signature line", bookPos + new Vector3(-.12f, .017f, -.09f + line * .042f), new Vector3(.16f, .001f, .003f), S.Black);
                Box("Guest signature line R", bookPos + new Vector3(.12f, .017f, -.09f + line * .042f), new Vector3(.16f, .001f, .003f), S.Black);
            }

            // 3. Golden Pen & Stand
            var penPos = bookPos + new Vector3(.28f, 0, .08f);
            Lathe("Pen brass holder base", penPos, new[]{ new Vector2(.035f, 0), new Vector2(.02f, .03f) }, S.Brass, 12);
            Beam("Golden signature ballpen", penPos + Vector3.up * .02f, penPos + new Vector3(-.03f, .16f, .02f), .007f, S.Brass);

            // 4. Welcome Betel & Jasmine Flower Tray
            var welcomeTray = new Vector3(.52f, .80f, -.12f);
            Lathe("Brass welcome tray", welcomeTray, new[]{ new Vector2(0, 0), new Vector2(.11f, 0), new Vector2(.13f, .025f) }, S.Brass, 16);
            for (int j = 0; j < 6; j++)
            {
                float ja = j * Mathf.PI * 2f / 6f;
                Ball("Welcome jasmine garland bud", welcomeTray + new Vector3(Mathf.Cos(ja) * .075f, .035f, Mathf.Sin(ja) * .075f), Vector3.one * .025f, S.Cream);
            }

            var recGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(recGroup, addCollider: true);
        }

        public static void WeddingSoundSystemStack(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Wedding PA sound system and speaker stack", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var spkMat = S.Plain("Black loudspeaker textured vinyl", new Color(.11f, .11f, .12f), .20f);
            var grillMat = S.Plain("Steel speaker grille", new Color(.18f, .18f, .19f), .35f);

            // Heavy 15-inch Dual Subwoofer Bass Cabinet on ground
            Box("Subwoofer cabinet", Vector3.up * .52f, new Vector3(.68f, 1.04f, .55f), spkMat);
            Box("Steel front grille", new Vector3(0, .52f, -.28f), new Vector3(.62f, .96f, .015f), grillMat);
            // Dual 15-inch speaker cones behind grille
            Tube("Woofer 1 cone surround", new Vector3(0, .30f, -.27f), .18f, .02f, S.Black, Quaternion.Euler(90, 0, 0), 20);
            Tube("Woofer 2 cone surround", new Vector3(0, .74f, -.27f), .18f, .02f, S.Black, Quaternion.Euler(90, 0, 0), 20);

            // Top Mid/High Horn Cabinet stacked above
            Box("Mid-high horn cabinet", Vector3.up * 1.38f, new Vector3(.58f, .62f, .45f), spkMat);
            Box("Horn front grille", new Vector3(0, 1.38f, -.23f), new Vector3(.52f, .56f, .015f), grillMat);
            Tube("Mid driver cone", new Vector3(0, 1.25f, -.22f), .12f, .016f, S.Black, Quaternion.Euler(90, 0, 0), 16);
            Box("High frequency acoustic horn flare", new Vector3(0, 1.52f, -.22f), new Vector3(.24f, .12f, .02f), S.Black);

            // Audio Amplifier & Karaoke Mixer Rack beside speaker
            var rackPos = new Vector3(.85f, 0, 0);
            Box("Mixer folding table top", rackPos + Vector3.up * .74f, new Vector3(.85f, .04f, .60f), S.DarkWood);
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Box("Rack table leg", rackPos + new Vector3(x * .38f, .36f, z * .25f), new Vector3(.035f, .72f, .035f), S.Metal);

            // Power amp 19-inch rack unit
            Box("Power amplifier 2000W chassis", rackPos + Vector3.up * .82f, new Vector3(.52f, .12f, .38f), S.Black);
            Box("Amp front faceplate", rackPos + new Vector3(0, .82f, -.195f), new Vector3(.50f, .11f, .01f), S.Metal);
            // Glowing LED indicator dots
            Ball("Power LED green", rackPos + new Vector3(-.20f, .82f, -.202f), Vector3.one * .012f, S.Bulb);
            Ball("VU meter LED red", rackPos + new Vector3(.18f, .82f, -.202f), Vector3.one * .012f, S.Bulb);

            // Analog karaoke audio mixer with rotary knobs
            Box("Audio mixer console", rackPos + new Vector3(0, .91f, -.02f), new Vector3(.48f, .06f, .34f), S.Black);
            for (int ch = -2; ch <= 2; ch++)
            {
                Box("Mixer fader slot", rackPos + new Vector3(ch * .075f, .942f, -.11f), new Vector3(.008f, .002f, .08f), S.Metal);
                Ball("Knob EQ treble", rackPos + new Vector3(ch * .075f, .945f, .02f), Vector3.one * .016f, S.Metal);
                Ball("Knob EQ bass", rackPos + new Vector3(ch * .075f, .945f, .08f), Vector3.one * .016f, S.Metal);
            }

            // Classic Dynamic Microphone on Chrome Boom Stand
            var micStandPos = rackPos + new Vector3(-.42f, 0, -.35f);
            Lathe("Cast iron round mic base", micStandPos, new[]{ new Vector2(.14f, 0), new Vector2(.02f, .03f) }, S.Black, 16);
            Beam("Mic vertical chrome shaft", micStandPos + Vector3.up * .03f, micStandPos + Vector3.up * 1.35f, .016f, S.Metal);
            Beam("Mic boom arm", micStandPos + Vector3.up * 1.35f, micStandPos + new Vector3(.15f, 1.55f, .18f), .014f, S.Metal);
            Cylinder("Microphone handle", micStandPos + new Vector3(.15f, 1.55f, .18f), .014f, .10f, S.Black, 10);
            Ball("Chrome mesh mic windscreen ball", micStandPos + new Vector3(.18f, 1.63f, .22f), Vector3.one * .038f, S.Metal);

            var spkGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(spkGroup, addCollider: true);
        }

        public static void BeverageCrateStack(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Angkor beer and soda beverage crates", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var redCrate = S.Plain("Red soda bottle crate", new Color(.78f, .16f, .18f), .25f);
            var greenCrate = S.Plain("Angkor beer green crate", new Color(.16f, .52f, .24f), .25f);
            var yellowCrate = S.Plain("Tiger beer yellow crate", new Color(.88f, .68f, .12f), .25f);

            Material[] crateMats = { greenCrate, redCrate, yellowCrate };

            // 3 Crates stacked high, plus a side stack of 2 crates
            for (int s = 0; s < 2; s++)
            {
                float sx = s * .46f;
                int count = (s == 0) ? 3 : 2;

                for (int h = 0; h < count; h++)
                {
                    var cp = new Vector3(sx, h * .31f, 0);
                    var mat = crateMats[(s + h) % crateMats.Length];

                    // Molded plastic beverage crate
                    Box("Crate plastic wall L", cp + new Vector3(-.21f, .15f, 0), new Vector3(.025f, .30f, .34f), mat);
                    Box("Crate plastic wall R", cp + new Vector3(.21f, .15f, 0), new Vector3(.025f, .30f, .34f), mat);
                    Box("Crate plastic wall F", cp + new Vector3(0, .15f, -.16f), new Vector3(.44f, .30f, .025f), mat);
                    Box("Crate plastic wall B", cp + new Vector3(0, .15f, .16f), new Vector3(.44f, .30f, .025f), mat);
                    Box("Crate grid bottom", cp + Vector3.up * .015f, new Vector3(.43f, .025f, .33f), mat);

                    // Glass bottles inside top crate
                    if (h == count - 1)
                    {
                        for (int bx = -1; bx <= 1; bx++)
                            for (int bz = -1; bz <= 1; bz++)
                            {
                                var bp = cp + new Vector3(bx * .11f, .03f, bz * .09f);
                                Cylinder("Beer bottle body", bp, .032f, .16f, (mat == greenCrate) ? S.Plain("Green glass", new Color(.15f, .45f, .20f), .7f) : S.Plain("Amber glass", new Color(.55f, .32f, .12f), .7f), 10);
                                Cylinder("Bottle neck", bp + Vector3.up * .16f, .013f, .08f, S.Metal, 8);
                                Ball("Crown cap gold", bp + Vector3.up * .24f, new Vector3(.028f, .008f, .028f), S.Brass);
                            }
                    }
                }
            }

            var crateGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(crateGroup, addCollider: true);
        }

        public static void CambodianSalonSet(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional Cambodian teak salon seating set - Salon Chheu", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            // 1. Two sculpted solid teakwood armchairs (Kaei-ey Salon Chheu) - 100% solid carved wood, no yellow cushions
            for (int s = -1; s <= 1; s += 2)
            {
                var chairRoot = Group("Salon chair " + (s < 0 ? "left" : "right"), new Vector3(s * .92f, 0, 0));
                chairRoot.localEulerAngles = new Vector3(0, -s * 12f, 0);
                var prevP = Parent;
                Parent = chairRoot;

                // 2 Front turned legs: start at floor y = 0 and rise to seat bottom y = 0.40m
                for (int x = -1; x <= 1; x += 2)
                {
                    var fx = x * .27f;
                    var fz = -.24f;
                    // Turned leg body from y = 0 to y = 0.40m
                    Cylinder("Front turned leg", new Vector3(fx, 0f, fz), .032f, .40f, S.DarkWood, 14);
                    // Decorative turned bead at mid-height
                    Ball("Front leg bead", new Vector3(fx, .20f, fz), new Vector3(.074f, .045f, .074f), S.Carving);
                }

                // 2 Continuous Back Stile / Legs: rise seamlessly from floor y = 0 all the way to top crest y = 1.04m
                for (int x = -1; x <= 1; x += 2)
                {
                    var bx = x * .27f;
                    var bz = .26f;
                    Box("Back continuous stile", new Vector3(bx, .52f, bz), new Vector3(.05f, 1.04f, .05f), S.DarkWood, true);
                }

                // Lower perimeter leg stretchers for structural stability
                Box("Stretcher left", new Vector3(-.27f, .12f, .01f), new Vector3(.035f, .035f, .50f), S.DarkWood);
                Box("Stretcher right", new Vector3(.27f, .12f, .01f), new Vector3(.035f, .035f, .50f), S.DarkWood);
                Box("Stretcher front", new Vector3(0, .12f, -.24f), new Vector3(.54f, .035f, .035f), S.DarkWood);
                Box("Stretcher back", new Vector3(0, .12f, .26f), new Vector3(.54f, .035f, .035f), S.DarkWood);

                // Carved seat apron frame
                Box("Seat frame front apron", new Vector3(0, .38f, -.24f), new Vector3(.56f, .06f, .04f), S.Carving);
                Box("Seat frame back", new Vector3(0, .38f, .26f), new Vector3(.56f, .06f, .04f), S.DarkWood);
                Box("Seat frame left", new Vector3(-.27f, .38f, .01f), new Vector3(.04f, .06f, .50f), S.DarkWood);
                Box("Seat frame right", new Vector3(.27f, .38f, .01f), new Vector3(.04f, .06f, .50f), S.DarkWood);

                // SOLID CARVED HARDWOOD SEAT (Traditional smooth polished teak plank - NO yellow cushion!)
                Box("Solid teak seat plank", new Vector3(0, .415f, .01f), new Vector3(.62f, .03f, .54f), S.Wood, true);
                Box("Front seat beveled lip", new Vector3(0, .415f, -.265f), new Vector3(.62f, .025f, .03f), S.Carving);

                // Armrests: curved sculpted teak arms connecting front post to back stile
                for (int arm = -1; arm <= 1; arm += 2)
                {
                    var ax = arm * .285f;
                    // Front armrest support post rising from seat y = .43m to arm rail y = .64m
                    Cylinder("Arm support post", new Vector3(ax, .43f, -.24f), .024f, .21f, S.DarkWood, 12);
                    // Polished horizontal armrest rail
                    Box("Armrest rail", new Vector3(ax, .65f, .01f), new Vector3(.065f, .035f, .52f), S.Wood);
                    // Front carved scroll terminal
                    Ball("Armrest scroll terminal", new Vector3(ax, .65f, -.26f), new Vector3(.07f, .045f, .07f), S.Carving);
                }

                // Carved Backrest: solid wooden cross rails and vertical turned spindles
                // Bottom back rail directly resting on seat plank
                Box("Backrest bottom rail", new Vector3(0, .45f, .26f), new Vector3(.54f, .045f, .04f), S.DarkWood);
                // Middle carved floral banner rail
                Box("Backrest mid carved rail", new Vector3(0, .72f, .26f), new Vector3(.54f, .06f, .035f), S.Carving);
                // Top arched crest rail
                Box("Backrest top crest rail", new Vector3(0, .98f, .26f), new Vector3(.58f, .07f, .045f), S.Carving);
                // Traditional Angkorian Kbach crown pediment
                Box("Kbach crown finial", new Vector3(0, 1.04f, .26f), new Vector3(.24f, .06f, .03f), S.Carving);

                // Lower section vertical spindles
                for (int b = -2; b <= 2; b++)
                {
                    float sx = b * .105f;
                    Cylinder("Lower spindle", new Vector3(sx, .47f, .26f), .013f, .22f, S.Wood, 10);
                }
                // Upper section vertical spindles
                for (int b = -2; b <= 2; b++)
                {
                    float sx = b * .105f;
                    Cylinder("Upper spindle", new Vector3(sx, .75f, .26f), .013f, .20f, S.Wood, 10);
                    Ball("Upper spindle bead", new Vector3(sx, .85f, .26f), Vector3.one * .022f, S.Carving);
                }

                Parent = prevP;
            }

            // 2. Wide, elegant carved coffee table (Tok Salon Chheu)
            var tp = Vector3.zero;
            float tableH = 0.47f; // top surface at y = 0.47m

            // Tabletop: polished teak with beveled edge
            Box("Salon table top polished teak", tp + Vector3.up * (tableH - 0.015f), new Vector3(.96f, .03f, .58f), S.Wood, true);
            Box("Salon table beveled edge rim", tp + Vector3.up * (tableH - 0.035f), new Vector3(.98f, .015f, .60f), S.DarkWood);

            // Carved apron underneath with traditional scroll moldings
            Box("Table front carved apron", tp + new Vector3(0, tableH - 0.07f, -.27f), new Vector3(.88f, .05f, .02f), S.Carving);
            Box("Table back carved apron", tp + new Vector3(0, tableH - 0.07f, .27f), new Vector3(.88f, .05f, .02f), S.Carving);
            Box("Table left carved apron", tp + new Vector3(-.43f, tableH - 0.07f, 0), new Vector3(.02f, .05f, .52f), S.Carving);
            Box("Table right carved apron", tp + new Vector3(.43f, tableH - 0.07f, 0), new Vector3(.02f, .05f, .52f), S.Carving);

            // 4 Turned legs: start at floor y = 0 and rise cleanly to tableH - 0.04m (y = 0.43m)
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    var legP = tp + new Vector3(x * .41f, 0f, z * .23f);
                    Cylinder("Table turned leg", legP, .030f, tableH - 0.04f, S.DarkWood, 14);
                    Ball("Table leg turned bead", legP + Vector3.up * .22f, new Vector3(.068f, .040f, .068f), S.Carving);
                }

            // Lower shelf with vintage 2006 Cambodian newspapers
            Box("Table lower shelf", tp + Vector3.up * .14f, new Vector3(.84f, .02f, .46f), S.DarkWood);
            Box("Cambodian newspaper stack 2006", tp + new Vector3(-.22f, .16f, 0), new Vector3(.24f, .025f, .16f), S.Plain("News print", new Color(.85f, .84f, .80f), .1f));

            // Complete authentic Cambodian hospitality tea service on table top:
            // 1. Raised round tea tray
            Lathe("Tea serving tray", tp + new Vector3(0, tableH + .002f, 0), new[]{ new Vector2(0, 0), new Vector2(.24f, 0), new Vector2(.255f, .022f) }, S.Metal, 20);

            // 2. Iconic Red Vacuum Thermos Flask (Phnoung Teuk Kdao)
            var redTin = S.Plain("Chinese floral red tin flask", new Color(.78f, .16f, .18f), .45f);
            var flaskP = tp + new Vector3(-.10f, tableH + .005f, .02f);
            Cylinder("Thermos tin body", flaskP, .065f, .28f, redTin, 18);
            Lathe("Thermos chrome shoulder", flaskP + Vector3.up * .28f, new[]{ new Vector2(.065f, 0), new Vector2(.034f, .04f), new Vector2(.034f, .065f) }, S.Metal, 16);
            Ball("Thermos cup cap", flaskP + Vector3.up * .365f, new Vector3(.062f, .045f, .062f), redTin);
            Tube("Thermos black handle", flaskP + new Vector3(-.075f, .15f, 0), .055f, .010f, S.Black, Quaternion.Euler(0, 90f, 0), 16);

            // 3. Ceramic Teapot with lid and spout
            var potP = tp + new Vector3(.08f, tableH + .005f, -.03f);
            Lathe("Ceramic teapot body", potP + Vector3.up * .02f, new[]{ new Vector2(.035f, 0), new Vector2(.085f, .04f), new Vector2(.072f, .10f), new Vector2(.035f, .125f), new Vector2(0, .125f) }, S.Porcelain, 18);
            Beam("Teapot spout", potP + new Vector3(.05f, .07f, 0), potP + new Vector3(.13f, .13f, 0), .028f, S.Porcelain);
            Tube("Teapot handle", potP + new Vector3(-.075f, .08f, 0), .045f, .008f, S.Porcelain, Quaternion.identity, 16);
            Ball("Teapot lid knob", potP + Vector3.up * .145f, Vector3.one * .016f, S.Carving);

            // 4. Porcelain tea cups on saucers
            for (int c = 0; c < 4; c++)
            {
                float ca = c * Mathf.PI * 2f / 4f + 0.3f;
                var cp = tp + new Vector3(Mathf.Sin(ca) * .17f, tableH + .005f, Mathf.Cos(ca) * .17f);
                Lathe("Tea bowl saucer", cp, new[]{ new Vector2(0, 0), new Vector2(.048f, 0), new Vector2(.055f, .012f) }, S.Porcelain, 14);
                Lathe("Porcelain tea bowl", cp + Vector3.up * .012f, new[]{ new Vector2(.022f, 0), new Vector2(.038f, .012f), new Vector2(.046f, .045f), new Vector2(.040f, .046f) }, S.Porcelain, 16);
            }

            // 5. Carved wooden tissue box on table edge
            Box("Teak tissue box", tp + new Vector3(.33f, .535f, -.14f), new Vector3(.14f, .07f, .10f), S.DarkWood);
            Box("White paper tissue top", tp + new Vector3(.33f, .575f, -.14f), new Vector3(.05f, .015f, .04f), S.Cream);

            // (Brass spittoon removed per user request)

            var group = Parent;
            Parent = old;
            ModelCombiner.CombineModel(group, addCollider: true);
        }

        public static void CambodianDisplayCupboard(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Carved teak glass display cupboard - Tu Kanchok", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            // Main timber cupboard carcass (1.6m wide, 2.1m tall, 0.48m deep)
            Box("Cupboard base plinth", Vector3.up * .08f, new Vector3(1.62f, .16f, .50f), S.DarkWood, true);
            Box("Cupboard main body frame", Vector3.up * 1.05f, new Vector3(1.58f, 1.82f, .46f), S.DarkWood, true);
            Box("Cupboard top carved cornice", Vector3.up * 2.02f, new Vector3(1.68f, .12f, .54f), S.Carving);
            Ball("Cornice carved center crest", Vector3.up * 2.12f, new Vector3(.28f, .16f, .08f), S.Carving);

            // Lower wooden storage drawers
            for (int d = -1; d <= 1; d += 2)
            {
                Box("Lower drawer face", new Vector3(d * .38f, .32f, -.24f), new Vector3(.68f, .28f, .025f), S.Wood);
                Ball("Drawer brass pull knob", new Vector3(d * .38f, .32f, -.26f), Vector3.one * .022f, S.Brass);
            }

            // Upper glass showcase doors
            for (int door = -1; door <= 1; door += 2)
            {
                var dp = new Vector3(door * .38f, 1.25f, -.235f);
                Box("Door timber frame", dp, new Vector3(.72f, 1.35f, .035f), S.DarkWood);
                Box("Door glass window pane", dp, new Vector3(.60f, 1.22f, .008f), S.Porcelain);
                Tube("Door brass drop handle", dp + new Vector3(-door * .28f, -.05f, -.025f), .028f, .006f, S.Brass, Quaternion.identity, 12);
            }

            // Interior shelves with display relics
            for (int shelf = 0; shelf < 3; shelf++)
            {
                float sy = .75f + shelf * .42f;
                Box("Cupboard timber shelf", Vector3.up * sy, new Vector3(1.48f, .025f, .40f), S.Wood);

                if (shelf == 0)
                {
                    // Bottom shelf: Stack of wrapped wedding gift boxes
                    Box("Wedding gift hamper red", new Vector3(-.35f, sy + .10f, 0), new Vector3(.32f, .18f, .26f), S.Plain("Gift box red silk", new Color(.78f, .14f, .18f), .2f));
                    Box("Wedding gift hamper gold", new Vector3(.35f, sy + .10f, 0), new Vector3(.30f, .18f, .24f), S.Plain("Gift box gold silk", new Color(.85f, .70f, .18f), .4f));
                }
                else if (shelf == 1)
                {
                    // Middle shelf: Fine porcelain tea service
                    Lathe("Display teapot", new Vector3(0, sy + .02f, 0), new[]{ new Vector2(0, 0), new Vector2(.055f, .02f), new Vector2(.07f, .06f), new Vector2(.035f, .10f) }, S.Porcelain, 14);
                    for (int c = -1; c <= 1; c += 2)
                        Cup(new Vector3(c * .22f, sy + .02f, 0));
                }
                else if (shelf == 2)
                {
                    // Top shelf: Small brass Buddha amulet shrine
                    Lathe("Mini Buddha shrine", new Vector3(0, sy + .02f, 0), new[]{ new Vector2(.06f, 0), new Vector2(.08f, .02f), new Vector2(.04f, .12f) }, S.Brass, 12);
                    Ball("Mini Buddha head", new Vector3(0, sy + .16f, 0), Vector3.one * .04f, S.Brass);
                    for (int f = -1; f <= 1; f += 2)
                    {
                        Box("Framed ancestor mini photo", new Vector3(f * .32f, sy + .11f, .05f), new Vector3(.14f, .18f, .015f), S.DarkWood);
                        Box("Photo print", new Vector3(f * .32f, sy + .11f, .04f), new Vector3(.11f, .15f, .002f), S.Cream);
                    }
                }
            }

            var cupGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(cupGroup, addCollider: true);
        }

        public static void FloorStandingFlowerUrn(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Ceremonial floor standing floral urn - Tho Phka Rongkar", p);

            // Carved wooden pedestal stand (0.95m tall)
            Box("Pedestal base", Vector3.up * .04f, new Vector3(.38f, .08f, .38f), S.DarkWood);
            Cylinder("Pedestal carved column", Vector3.up * .08f, .09f, .82f, S.DarkWood, 16);
            Box("Pedestal top plate", Vector3.up * .92f, new Vector3(.34f, .04f, .34f), S.DarkWood);

            // Ceramic glazed urn vase
            var vasePos = Vector3.up * .94f;
            Lathe("Ceramic urn vase", vasePos, new[]{
                new Vector2(.08f, 0), new Vector2(.14f, .12f), new Vector2(.11f, .28f), new Vector2(.16f, .36f), new Vector2(.12f, .38f)
            }, S.Porcelain, 18);

            // Cascading wedding floral arrangement: lotus buds, white jasmine, and marigolds
            var bloomTop = vasePos + Vector3.up * .38f;
            Ball("Central floral dome core", bloomTop + Vector3.up * .12f, new Vector3(.28f, .24f, .28f), S.Leaf);

            var marigoldMat = S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f);
            for (int b = 0; b < 10; b++)
            {
                float a = b * Mathf.PI * 2f / 10f;
                float r = .16f + (b % 2) * .06f;
                var fp = bloomTop + new Vector3(Mathf.Cos(a) * r, .08f + Mathf.Sin(b * 1.5f) * .06f, Mathf.Sin(a) * r);
                Ball("Arrangement blossom", fp, Vector3.one * .055f, (b % 3 == 0) ? S.Pink : (b % 3 == 1) ? S.Cream : marigoldMat);
            }

            // Cascading hanging jasmine garlands
            for (int g = 0; g < 4; g++)
            {
                float ga = g * Mathf.PI / 2f + .3f;
                var gStart = bloomTop + new Vector3(Mathf.Cos(ga) * .18f, .02f, Mathf.Sin(ga) * .18f);
                for (int node = 0; node < 5; node++)
                {
                    Ball("Hanging jasmine bud", gStart + new Vector3(0, -node * .065f, 0), Vector3.one * .035f, S.Cream);
                }
            }

            var urnGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(urnGroup, addCollider: true);
        }

        public static void RolledSleepingMats(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Rolled Cambodian reed guest mats - Kanthiel Romchek", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var matWeave = S.Plain("Rolled reed mat weave", new Color(.56f, .42f, .24f), .08f);
            var redTie = S.Plain("Mat crimson tie ribbon", new Color(.72f, .14f, .18f), .15f);

            // Two thick cylindrical rolled mats standing upright, slightly tilted together
            for (int i = 0; i < 2; i++)
            {
                var mp = new Vector3((i - .5f) * .22f, 0, 0);
                var tilt = Quaternion.Euler((i == 0 ? 4f : -3f), 0, (i == 0 ? 5f : -4f));
                Cylinder("Rolled reed mat cylinder", mp, .11f, 1.85f, matWeave, 16);

                // Two red binding cords wrapped around each roll
                Tube("Mat red cord lower", mp + tilt * (Vector3.up * .55f), .116f, .014f, redTie, tilt, 14);
                Tube("Mat red cord upper", mp + tilt * (Vector3.up * 1.35f), .116f, .014f, redTie, tilt, 14);
                Ball("Tie knot", mp + tilt * (new Vector3(0, 1.35f, -.118f)), Vector3.one * .028f, redTie);
            }

            var matGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(matGroup, addCollider: true);
        }

        public static void CambodianHatAndCoatStand(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional Cambodian coat and hat stand", p);

            var palmHatMat = S.Plain("Conical palm leaf hat Muok Sleuk Tnaot", new Color(.74f, .62f, .40f), .08f);
            var redKroma = S.Plain("Red checked Kroma scarf", new Color(.72f, .24f, .22f), .15f);

            // Turned timber post with round tripod base (1.8m tall)
            Box("Stand base leg 1", Vector3.up * .04f, new Vector3(.44f, .06f, .08f), S.DarkWood);
            Box("Stand base leg 2", Vector3.up * .04f, new Vector3(.08f, .06f, .44f), S.DarkWood);
            Cylinder("Turned stand central pillar", Vector3.up * .06f, .038f, 1.76f, S.DarkWood, 14);
            Ball("Top turned finial knob", Vector3.up * 1.85f, Vector3.one * .055f, S.DarkWood);

            // Wooden turned hanging peg arms
            for (int peg = 0; peg < 4; peg++)
            {
                float a = peg * Mathf.PI / 2f + .35f;
                float py = 1.55f + (peg % 2) * .14f;
                var pegDir = new Vector3(Mathf.Cos(a), .35f, Mathf.Sin(a)).normalized;
                Beam("Hanging peg arm", Vector3.up * py, Vector3.up * py + pegDir * .18f, .018f, S.Wood);
                Ball("Peg ball tip", Vector3.up * py + pegDir * .19f, Vector3.one * .024f, S.Wood);

                if (peg == 0 || peg == 2)
                {
                    // Conical Cambodian palm leaf farmer hats (Muok Sleuk Tnaot) hung on pegs
                    var hatPos = Vector3.up * py + pegDir * .19f + Vector3.down * .12f;
                    Lathe("Conical palm leaf hat", hatPos, new[]{
                        new Vector2(0, .14f), new Vector2(.24f, 0), new Vector2(.24f, .008f), new Vector2(0, .145f)
                    }, palmHatMat, 18);
                }
                else if (peg == 1)
                {
                    // Draped checked cotton Kroma
                    var kPos = Vector3.up * py + pegDir * .19f;
                    Grid("Draped stand kroma", 8, 12, (u, v) =>
                    {
                        float x = (u - .5f) * .16f;
                        float y = -v * .65f;
                        float z = Mathf.Sin(v * Mathf.PI * 2.5f) * .02f;
                        return kPos + new Vector3(x, y, z);
                    }, redKroma, new Vector2(1, 3), true);
                }
            }

            var standGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(standGroup, addCollider: true);
        }

        public static void HangingWeddingLantern(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional Cambodian wedding paper lantern - Khom Rongkar", p);

            var pinkPaper = S.Plain("Festive pink paper lantern", new Color(.88f, .38f, .52f), .20f);
            var goldTrim = S.Plain("Lantern gold trim", new Color(.88f, .72f, .18f), .45f);

            // Ceiling hanging cord
            Beam("Lantern ceiling cord", Vector3.up * .65f, Vector3.zero, .005f, goldTrim);

            // Octagonal diamond faceted paper lantern body
            Lathe("Paper lantern faceted body", Vector3.down * .22f, new[]{
                new Vector2(.05f, 0), new Vector2(.20f, .18f), new Vector2(.20f, .26f), new Vector2(.05f, .44f)
            }, pinkPaper, 8);

            // Top and bottom golden collars
            Tube("Top golden collar", Vector3.up * .22f, .055f, .014f, goldTrim, Quaternion.identity, 12);
            Tube("Bottom golden collar", Vector3.down * .22f, .055f, .014f, goldTrim, Quaternion.identity, 12);

            // Warm interior candle/bulb glow
            Ball("Interior warm lantern glow", Vector3.zero, Vector3.one * .06f, S.Bulb);

            // Hanging golden silk fringe tassels
            for (int t = 0; t < 6; t++)
            {
                float a = t * Mathf.PI / 3f;
                var tPos = new Vector3(Mathf.Cos(a) * .045f, -.24f, Mathf.Sin(a) * .045f);
                Beam("Hanging silk tassel", tPos, tPos + Vector3.down * .24f, .004f, goldTrim);
            }

            var lanternGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(lanternGroup, addCollider: false);
        }
    }
}
