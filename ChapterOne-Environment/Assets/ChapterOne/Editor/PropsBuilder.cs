using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    public static class PropsBuilder
    {
        public static void Table(Vector3 p, Vector2 size, float height, Material mat)
        {
            Box("Worn table top", p + Vector3.up * height, new Vector3(size.x, .07f, size.y), mat);
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Box("Table leg", p + new Vector3(x * (size.x / 2 - .1f), height / 2, z * (size.y / 2 - .1f)), new Vector3(.07f, height, .07f), S.DarkWood);
        }

        public static void Bench(Vector3 p, float length)
        {
            var old = Parent;
            Parent = ModelGroup("Wooden bench", p);
            Box("Bench seat", Vector3.up * .45f, new Vector3(length, .075f, .51f), S.Wood);
            for (int s = -1; s <= 1; s += 2)
            {
                Box("Bench leg", new Vector3(s * (length / 2 - .16f), .23f, 0), new Vector3(.085f, .46f, .42f), S.DarkWood);
                Box("Back upright", new Vector3(s * (length / 2 - .12f), .7f, .25f), new Vector3(.065f, .9f, .065f), S.DarkWood);
            }
            Box("Bench back crest", new Vector3(0, 1.08f, .25f), new Vector3(length, .095f, .08f), S.Wood);
            for (int i = 0; i < 14; i++)
                Box("Backrest spindle", new Vector3(-length * .43f + i * length * .86f / 13, .78f, .25f), new Vector3(.038f, .53f, .035f), S.Wood);
            var benchGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(benchGroup, addCollider: true);
        }

        public static void Cabinet(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Carved teak cupboard", p);
            Box("Carved teak cupboard body", Vector3.up * .9f, new Vector3(1.5f, 1.55f, .5f), S.DarkWood);
            Box("Cupboard crown", Vector3.up * 1.72f, new Vector3(1.62f, .09f, .6f), S.Wood);
            for (int s = -1; s <= 1; s += 2)
            {
                int sign = s;
                Grid("Carved door panel", 1, 1, (u, v) => new Vector3(sign * .365f + (u - .5f) * .7f, .2f + v * 1.4f, -.26f), S.Carving, Vector2.one, true);
                Tube("Brass ring handle", new Vector3(sign * .09f, .94f, -.29f), .045f, .008f, S.Brass, Quaternion.identity, 16);
                Box("Cupboard foot", new Vector3(sign * .6f, .09f, 0), new Vector3(.12f, .18f, .4f), S.Wood);
            }
            var cabGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(cabGroup, addCollider: true);
        }

        public static void Jar(Vector3 p, float scale)
        {
            var old = Parent;
            Parent = ModelGroup("Large earthen water jar", p);
            Parent.localScale = Vector3.one * scale;
            var glaze = S.Pbr("Worn earthenware glaze", "Ground054", new Color(.39f, .26f, .14f), .22f);
            glaze.SetFloat("_BumpScale", .12f);
            Lathe("Glazed water jar", Vector3.zero, new[]{
                new Vector2(.25f, 0), new Vector2(.30f, .025f), new Vector2(.35f, .07f), new Vector2(.40f, .14f),
                new Vector2(.44f, .24f), new Vector2(.46f, .36f), new Vector2(.455f, .46f), new Vector2(.435f, .56f),
                new Vector2(.40f, .65f), new Vector2(.35f, .73f), new Vector2(.29f, .79f), new Vector2(.24f, .82f),
                new Vector2(.24f, .86f), new Vector2(.225f, .88f), new Vector2(.2f, .87f), new Vector2(.21f, .81f)
            }, glaze, 36);
            Cylinder("Water inside jar", new Vector3(0, .76f, 0), .2f, .008f, S.Water);
            var jarGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(jarGroup, addCollider: true);
        }

        public static void Basket(Vector3 p, float scale)
        {
            var old = Parent;
            Parent = ModelGroup("Woven basket", p);
            Parent.localScale = Vector3.one * scale;
            Lathe("Basket body", Vector3.zero, new[]{ new Vector2(.23f, 0), new Vector2(.32f, .08f), new Vector2(.37f, .55f), new Vector2(.35f, .59f) }, S.Wood);
            for (int i = 0; i < 13; i++)
                Tube("Horizontal wicker weave", new Vector3(0, .05f + i * .042f, 0), .24f + i * .009f, .008f, S.DarkWood, Quaternion.Euler(90, 0, 0), 24);
            for (int i = 0; i < 18; i++)
            {
                float a = i * Mathf.PI / 9;
                Beam("Basket vertical weave", new Vector3(Mathf.Cos(a) * .23f, .02f, Mathf.Sin(a) * .23f), new Vector3(Mathf.Cos(a) * .36f, .58f, Mathf.Sin(a) * .36f), .011f, S.Cream);
            }
            var bGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(bGroup, addCollider: true);
        }

        public static void CeramicSet(Vector3 p)
        {
            Lathe("Tea tray", p, new[]{ new Vector2(0, 0), new Vector2(.28f, 0), new Vector2(.3f, .025f), new Vector2(.29f, .04f) }, S.Metal);
            Lathe("Small teapot", p + new Vector3(0, .04f, 0), new[]{ new Vector2(.04f, 0), new Vector2(.095f, .04f), new Vector2(.08f, .11f), new Vector2(.04f, .14f), new Vector2(0, .14f) }, S.Porcelain);
            Beam("Teapot spout", p + new Vector3(.06f, .08f, 0), p + new Vector3(.16f, .16f, 0), .035f, S.Porcelain);
            Tube("Teapot handle", p + new Vector3(-.085f, .11f, 0), .055f, .009f, S.Porcelain, Quaternion.identity, 16);
            for (int s = -1; s <= 1; s += 2) Cup(p + new Vector3(s * .19f, .035f, -.11f));
        }

        public static void Cup(Vector3 p)
        {
            Lathe("Porcelain tea bowl", p, new[]{ new Vector2(.025f, 0), new Vector2(.042f, .012f), new Vector2(.052f, .065f), new Vector2(.046f, .066f), new Vector2(.037f, .015f) }, S.Porcelain, 20);
        }

        public static void Thermos(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Cambodian hot water thermos", p);
            Lathe("Thermos tin body", Vector3.zero, new[]{ new Vector2(0, 0), new Vector2(.06f, 0), new Vector2(.065f, .28f), new Vector2(.055f, .31f), new Vector2(.03f, .32f) }, S.Pink, 16);
            Cylinder("Thermos chrome cap", new Vector3(0, .32f, 0), .032f, .06f, S.Metal, 16);
            Tube("Thermos plastic handle", new Vector3(-.07f, .16f, 0), .05f, .008f, S.Black, Quaternion.Euler(0, 90, 0), 16);
            var thermosGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(thermosGroup, addCollider: true);
        }

        public static void AltarShelf(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Ancestral altar shelf - Ban-lor", p);
            Box("Altar shelf deck", Vector3.zero, new Vector3(.24f, .045f, 1.1f), S.DarkWood, true);
            for (int s = -1; s <= 1; s += 2)
                Beam("Altar shelf bracket", new Vector3(0, -.22f, s * .42f), new Vector3(.08f, -.02f, s * .42f), .035f, S.DarkWood);
            Box("Ancestral portrait frame", new Vector3(.05f, .22f, 0), new Vector3(.025f, .34f, .28f), S.DarkWood);
            Box("Portrait photo print", new Vector3(.065f, .22f, 0), new Vector3(.008f, .28f, .22f), S.Cream);
            Lathe("Altar incense cup", new Vector3(.04f, .045f, -.24f), new[]{ new Vector2(0, 0), new Vector2(.035f, 0), new Vector2(.045f, .045f) }, S.Brass, 14);
            for (int i = -1; i <= 1; i++)
                Beam("Red altar incense", new Vector3(.04f, .08f, -.24f + i * .012f), new Vector3(.04f, .24f, -.24f + i * .018f), .003f, S.Pink);
            Beam("Small altar candle", new Vector3(.04f, .045f, .24f), new Vector3(.04f, .14f, .24f), .014f, S.Pink);
            Lathe("Brass water cup", new Vector3(.04f, .045f, .35f), new[]{ new Vector2(0, 0), new Vector2(.025f, 0), new Vector2(.03f, .035f) }, S.Brass, 12);
            var altarGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(altarGroup, addCollider: true);
        }


        public static void FootWashStation(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional foot washing station", p);
            Box("Wash deck slats", new Vector3(0, .05f, 0), new Vector3(1.1f, .10f, 1.1f), S.DarkWood, true);
            for (int i = -2; i <= 2; i++)
                Box("Slatted plank", new Vector3(i * .22f, .11f, 0), new Vector3(.18f, .02f, 1.05f), S.Wood);
            Jar(new Vector3(.32f, .02f, -.05f), .88f);
            Beam("Water ladle handle", new Vector3(.22f, .78f, -.05f), new Vector3(-.12f, .78f, -.05f), .02f, S.Wood);
            Lathe("Water ladle cup", new Vector3(.25f, .76f, -.05f), new[]{ new Vector2(0, 0), new Vector2(.065f, 0), new Vector2(.075f, .075f), new Vector2(.07f, .08f) }, S.Black, 16);
            var washGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(washGroup, addCollider: true);
        }

        public static void SpiritHouse(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Cambodian spirit house - Rong Preah Phum", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);
            Box("Shrine post base", new Vector3(0, .12f, 0), new Vector3(.32f, .24f, .32f), S.DarkWood, true);
            Beam("Shrine timber pillar", new Vector3(0, .24f, 0), new Vector3(0, 1.45f, 0), .12f, S.DarkWood);
            for (int s = -1; s <= 1; s += 2)
            {
                Beam("Post bracket X", new Vector3(s * .06f, 1.25f, 0), new Vector3(s * .24f, 1.44f, 0), .04f, S.Wood);
                Beam("Post bracket Z", new Vector3(0, 1.25f, s * .06f), new Vector3(0, 1.44f, s * .24f), .04f, S.Wood);
            }
            Box("Offering platform deck", new Vector3(0, 1.48f, 0), new Vector3(.72f, .06f, .62f), S.Wood, true);
            for (int side = -1; side <= 1; side += 2)
            {
                Beam("Deck rail side", new Vector3(side * .34f, 1.56f, -.29f), new Vector3(side * .34f, 1.56f, .29f), .025f, S.DarkWood);
                Beam("Deck rail front", new Vector3(-.34f, 1.56f, side * .29f), new Vector3(.34f, 1.56f, side * .29f), .025f, S.DarkWood);
            }
            Box("Shrine chamber body", new Vector3(0, 1.75f, .06f), new Vector3(.44f, .42f, .38f), S.DarkWood, true);
            Box("Shrine doorway opening", new Vector3(0, 1.72f, -.135f), new Vector3(.22f, .32f, .02f), S.Black);
            Box("Shrine pitched roof", new Vector3(0, 2.02f, .06f), new Vector3(.62f, .16f, .54f), S.Roof, true);
            Box("Shrine roof ridge", new Vector3(0, 2.14f, .06f), new Vector3(.52f, .10f, .12f), S.Roof);
            for (int finial = -1; finial <= 1; finial += 2)
                Beam("Roof finial horn", new Vector3(finial * .27f, 2.16f, .06f), new Vector3(finial * .33f, 2.28f, .06f), .025f, S.Brass);
            Lathe("Incense burner bowl", new Vector3(0, 1.52f, -.14f), new[]{ new Vector2(0, 0), new Vector2(.055f, 0), new Vector2(.07f, .045f), new Vector2(.06f, .05f) }, S.Brass, 16);
            for (int i = -1; i <= 1; i++)
                Beam("Red incense stick", new Vector3(i * .018f, 1.54f, -.14f), new Vector3(i * .025f, 1.72f, -.14f + i * .012f), .004f, S.Pink);
            for (int c = -1; c <= 1; c += 2)
            {
                Beam("Red offering candle", new Vector3(c * .12f, 1.51f, -.14f), new Vector3(c * .12f, 1.61f, -.14f), .016f, S.Pink);
                Lathe("Offering water cup", new Vector3(c * .22f, 1.51f, -.14f), new[]{ new Vector2(0, 0), new Vector2(.03f, 0), new Vector2(.038f, .045f), new Vector2(.035f, .05f) }, S.Porcelain, 12);
            }
            var spiritGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(spiritGroup, addCollider: true);
        }

        public static void Pot(Vector3 p, float r)
        {
            Lathe("Aluminum cooking pot", p, new[]{ new Vector2(r * .7f, 0), new Vector2(r, .04f), new Vector2(r, r * .7f), new Vector2(r * .98f, r * .72f), new Vector2(0, r * .77f) }, S.Metal, 32);
            Ball("Pot lid handle", p + Vector3.up * (r * .81f), new Vector3(.13f, .055f, .07f), S.Black);
            for (int s = -1; s <= 1; s += 2)
                Tube("Pot side handle", p + new Vector3(s * r, r * .53f, 0), r * .18f, .015f, S.Black, Quaternion.Euler(0, 90, 0), 16);
        }

        public static void Chair(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Red plastic chair", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);
            var plastic = S.Plain("Faded red plastic", new Color(.33f, .075f, .055f), .22f);
            Box("Molded seat", new Vector3(0, .44f, 0), new Vector3(.43f, .045f, .44f), plastic);
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Beam("Splayed chair leg", new Vector3(x * .22f, 0, z * .23f), new Vector3(x * .17f, .44f, z * .17f), .044f, plastic);
            for (int s = -1; s <= 1; s += 2)
                Beam("Back upright", new Vector3(s * .18f, .43f, .17f), new Vector3(s * .20f, .94f, .26f), .045f, plastic);
            for (int i = 0; i < 5; i++)
            {
                float x = (i - 2) * .07f;
                Beam("Slotted chair back", new Vector3(x, .5f, .19f), new Vector3(x, .88f + .07f * (1 - Mathf.Abs(x) / .2f), .25f), .035f, plastic);
            }
            Grid("Rounded chair back crest", 16, 2, (u, v) => new Vector3((u - .5f) * .42f, .88f + Mathf.Sin(u * Mathf.PI) * .08f + v * .045f, .26f), plastic, Vector2.one, true);
            var chairGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(chairGroup, addCollider: true);
        }

        public static void Motorbike(Vector3 position, float yaw, Color color)
        {
            var old = Parent;
            Parent = ModelGroup("2000s step-through motorcycle", position);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);
            var paint = S.Plain("Motorcycle paint " + color.ToString(), color, .48f, .25f);
            for (int s = -1; s <= 1; s += 2)
            {
                var c = new Vector3(0, .34f, s * .64f);
                Tube("Rubber tire", c, .29f, .045f, S.Black, Quaternion.Euler(0, 90, 0), 40);
                Tube("Wheel rim", c, .245f, .015f, S.Metal, Quaternion.Euler(0, 90, 0), 32);
                for (int j = 0; j < 16; j++)
                {
                    float a = j * Mathf.PI / 8;
                    Beam("Wheel spoke", c, c + new Vector3(0, Mathf.Cos(a) * .24f, Mathf.Sin(a) * .24f), .0045f, S.Metal);
                }
                Ball("Wheel hub", c, new Vector3(.17f, .095f, .095f), S.Metal);
            }
            Beam("Motorbike frame", new Vector3(0, .4f, -.6f), new Vector3(0, .78f, .32f), .09f, S.Metal);
            Beam("Lower frame", new Vector3(0, .43f, .53f), new Vector3(0, .43f, -.3f), .1f, S.Metal);
            Ball("Engine casing", new Vector3(0, .41f, -.03f), new Vector3(.3f, .3f, .37f), S.Metal);
            Box("Black saddle", new Vector3(0, .83f, -.22f), new Vector3(.36f, .1f, .78f), S.Black);
            Ball("Painted rear body", new Vector3(0, .64f, -.23f), new Vector3(.34f, .28f, .74f), paint);
            Ball("Step through shield", new Vector3(0, .68f, .34f), new Vector3(.39f, .55f, .22f), paint);
            for (int side = -1; side <= 1; side += 2)
                Beam("Front suspension", new Vector3(side * .075f, .33f, .64f), new Vector3(side * .075f, .98f, .44f), .035f, S.Metal);
            Beam("Handlebar", new Vector3(-.34f, 1.05f, .42f), new Vector3(.34f, 1.05f, .42f), .035f, S.Metal);
            for (int side = -1; side <= 1; side += 2)
            {
                Beam("Rubber grip", new Vector3(side * .23f, 1.05f, .42f), new Vector3(side * .37f, 1.05f, .42f), .045f, S.Black);
                Beam("Mirror stem", new Vector3(side * .21f, 1.05f, .44f), new Vector3(side * .28f, 1.31f, .48f), .012f, S.Metal);
                Ball("Rear view mirror", new Vector3(side * .28f, 1.32f, .48f), new Vector3(.13f, .09f, .035f), S.Black);
            }
            Ball("Headlamp", new Vector3(0, 1.0f, .52f), new Vector3(.22f, .14f, .10f), S.Porcelain);
            Beam("Exhaust pipe", new Vector3(.18f, .28f, .08f), new Vector3(.18f, .36f, -.8f), .07f, S.Metal);
            Beam("Side stand", new Vector3(0, .38f, -.1f), new Vector3(-.25f, .02f, -.1f), .018f, S.Metal);
            var bikeGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(bikeGroup, addCollider: true);
        }

        public static void Generator(Vector3 position)
        {
            var old = Parent;
            Parent = ModelGroup("Portable petrol generator", position);
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Beam("Steel roll cage upright", new Vector3(x * .42f, .05f, z * .31f), new Vector3(x * .42f, .75f, z * .31f), .035f, S.Black);
            for (int y = 0; y <= 1; y++)
                for (int s = -1; s <= 1; s += 2)
                {
                    Beam("Generator frame rail", new Vector3(-.42f, .08f + y * .65f, s * .31f), new Vector3(.42f, .08f + y * .65f, s * .31f), .035f, S.Black);
                    Beam("Frame cross rail", new Vector3(s * .42f, .08f + y * .65f, -.31f), new Vector3(s * .42f, .08f + y * .65f, .31f), .035f, S.Black);
                }
            Box("Fuel tank", new Vector3(0, .66f, 0), new Vector3(.74f, .13f, .52f), S.Rust);
            Cylinder("Fuel cap", new Vector3(.16f, .735f, 0), .045f, .025f, S.Black);
            Ball("Alternator casing", new Vector3(.18f, .34f, 0), new Vector3(.34f, .37f, .43f), S.Metal);
            Box("Engine block", new Vector3(-.18f, .34f, 0), new Vector3(.25f, .32f, .32f), S.Black);
            for (int j = 0; j < 7; j++)
                Box("Cooling fin", new Vector3(-.18f, .22f + j * .035f, -.03f), new Vector3(.3f, .012f, .34f), S.Metal);
            Box("Generator control panel", new Vector3(.16f, .46f, -.235f), new Vector3(.34f, .19f, .035f), S.Black);
            for (int i = 0; i < 2; i++)
                Ball("Power outlet", new Vector3(.08f + i * .14f, .46f, -.26f), new Vector3(.065f, .065f, .018f), S.Porcelain);
            var genGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(genGroup, addCollider: true);
        }

        public static void MobilePhone2006(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("2006 classic button mobile phone", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var bodyMat = S.Plain("Phone casing", new Color(.12f, .13f, .14f), .35f);
            var screenMat = S.Plain("Phone monochrome LCD", new Color(.22f, .38f, .22f), .90f);
            var keyMat = S.Plain("Phone keypad button", new Color(.24f, .25f, .26f), .18f);

            Box("Phone body casing", Vector3.up * .009f, new Vector3(.052f, .018f, .115f), bodyMat);
            Box("Screen bezel", new Vector3(0, .0185f, .022f), new Vector3(.040f, .002f, .038f), S.Metal);
            Box("Monochrome LCD display", new Vector3(0, .0196f, .022f), new Vector3(.034f, .001f, .030f), screenMat);
            Box("Earpiece speaker slit", new Vector3(0, .0185f, .047f), new Vector3(.014f, .002f, .003f), S.Black);
            Box("Nav key", new Vector3(0, .0192f, -.005f), new Vector3(.022f, .003f, .014f), S.Metal);

            for (int r = 0; r < 4; r++)
                for (int c = -1; c <= 1; c++)
                {
                    Box("Numeric key", new Vector3(c * .012f, .0195f, -.019f - r * .0085f), new Vector3(.010f, .0025f, .0065f), keyMat);
                }

            var phoneGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(phoneGroup, addCollider: true);
        }

        public static void BridalAttire(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional Cambodian bridal wedding attire", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.78f, .62f, .18f), .45f, .25f);
            var redCloth = S.Plain("Pleated wedding sampot", new Color(.62f, .18f, .22f), .25f);

            Box("Garment stand base", Vector3.up * .03f, new Vector3(.45f, .06f, .35f), S.DarkWood);
            Beam("Garment upright post", Vector3.up * .06f, Vector3.up * 1.35f, .04f, S.DarkWood);
            Beam("Garment cross bar", new Vector3(-.35f, 1.32f, 0), new Vector3(.35f, 1.32f, 0), .035f, S.DarkWood);

            Grid("Draped bridal gold Sbai", 18, 12, (u, v) => new Vector3((u - .5f) * .48f, 1.34f - v * .95f - Mathf.Sin(u * Mathf.PI) * .05f, Mathf.Sin(v * Mathf.PI * 4) * .02f), goldSilk, new Vector2(2, 4), true);
            Box("Folded wedding sampot", Vector3.up * .55f, new Vector3(.42f, .18f, .28f), redCloth);
            Box("Bridal jewellery casket", Vector3.up * .68f, new Vector3(.22f, .08f, .16f), S.DarkWood);
            Box("Casket brass clasp", new Vector3(0, .68f, -.082f), new Vector3(.03f, .04f, .01f), S.Brass);

            var attireGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(attireGroup, addCollider: true);
        }


        public static void MountainRockOutcrop(Vector3 p, Vector3 size, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Weathered mountain rock outcrop", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var rockMat = S.Plain("Mountain granite rock", new Color(.46f, .44f, .40f), .12f);
            rockMat.shader = Shader.Find("ThatNight/WeatheredSurface");

            // Natural multi-faceted boulder
            Ball("Granite rock core", Vector3.up * (size.y * .35f), size, rockMat);
            // Secondary embedded rock facets
            Ball("Rock facet left", new Vector3(-size.x * .25f, size.y * .22f, size.z * .15f), size * .75f, rockMat);
            Ball("Rock facet right", new Vector3(size.x * .22f, size.y * .25f, -size.z * .18f), size * .80f, rockMat);

            var rockGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(rockGroup, addCollider: true);
        }
    }
}
