using UnityEngine;
using UnityEngine.Rendering;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Traditional Cambodian bedroom fittings following Wat Kor / Battambang 2006 heritage references.
    // Richly adorned for the nuptial wedding night with flower garlands, jewelry, betel boxes, and silk swags.
    internal static class BedroomBuilder
    {
        public static void Build(string name, float centerX, float back, bool bridal)
        {
            var old = Parent;
            Parent = Group(name, new Vector3(centerX, 2.5f, back - 1.7f));
            var bedding = S.Pbr(bridal ? "Rose bedroom cotton" : "Indigo bedroom cotton", "Fabric035",
                bridal ? new Color(.65f, .30f, .29f) : new Color(.22f, .31f, .39f), .06f);
            bedding.SetFloat("_Desaturate", 1);

            // 1. Handcrafted Cambodian Timber Canopy Bed with Nuptial Decorations
            Bed(bedding, bridal);

            if (bridal)
            {
                // 2. Teak Wardrobe with stacked wedding gift baskets
                Wardrobe(new Vector3(2.846f, 0.000f, 1.379f), 356.9f, bridal: true);

                // 3. Dressing Table & Mirror (Scene 04 Preparation Area adorned with Wedding Relics)
                BuildDressingTable(new Vector3(3.322f, 0.000f, -2.331f), 96.1f, bridal: true);

                // 4. Bedside Table with Enamel Pitcher & Authentic Cambodian Kerosene Lantern (Changkheang Praeng Kart)
                BuildNightstand(new Vector3(-2.205f, 0.000f, 0.965f));
                BuildPitcher(new Vector3(-2.214f, 0.490f, 1.104f));
                BuildKeroseneLamp(new Vector3(-2.236f, 0.523f, 0.807f));

                // 5. Handwoven Wild Reed Floor Mat & Red Silk Runner
                ReedMat(new Vector3(-0.900f, 0.015f, -1.900f), bridal: true);

                // Traditional bamboo clothes hanging rail
                BuildClothesDryingRack(new Vector3(-2.200f, 0.000f, 1.350f));

                // 6. Travel Trunk / Chest
                Storage(new Vector3(3.386f, 0.000f, -1.168f), 90.0f, bridal: true);

                // Scalloped silk valances along bedroom wall
                BuildBridalWallSwags();

                // Older pedestal bedroom fan
                BedroomFanBuilder.Build(new Vector3(-2.907f, 0.000f, -0.780f), 251.2f, -3.55f);
            }
            else
            {
                // 2. Teak Wardrobe
                Wardrobe(new Vector3(3.322f, 0.000f, 1.093f), 270.0f, bridal: false);

                // 3. Dressing Table & Mirror
                BuildDressingTable(new Vector3(3.463f, 0.000f, -2.628f), 92.6f, bridal: false);

                // 4. Bedside Table with Enamel Pitcher & Authentic Cambodian Kerosene Lantern (Changkheang Praeng Kart)
                BuildNightstand(new Vector3(-2.242f, 0.000f, 1.286f));
                BuildPitcher(new Vector3(-2.212f, 0.490f, 1.315f));
                BuildKeroseneLamp(new Vector3(-2.388f, 0.522f, 1.121f));

                // 5. Handwoven Wild Reed Floor Mat
                ReedMat(new Vector3(-0.900f, 0.015f, -1.900f), bridal: false);

                // 6. Travel Trunk / Chest
                Storage(new Vector3(3.438f, 0.000f, -1.527f), 90.0f, bridal: false);

                // Older pedestal bedroom fan
                BedroomFanBuilder.Build(new Vector3(-2.906f, 0.000f, -0.780f), 251.8f, -3.55f);
            }

            // Warm interior lighting
            LightingBuilder.Bulb(new Vector3(.2f, 2.2f, -1), 5, 1.6f, false);

            Parent = old;
        }

        static void BuildNightstand(Vector3 p)
        {
            var tableGroup = ModelGroup("Bedside wooden nightstand table", p);
            var prevParent = Parent;
            Parent = tableGroup;
            Box("Worn table top", Vector3.up * .49f, new Vector3(.55f, .07f, .55f), S.DarkWood);
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Box("Table leg", new Vector3(x * (.55f / 2 - .1f), .49f / 2, z * (.55f / 2 - .1f)), new Vector3(.07f, .49f, .07f), S.DarkWood);
            Parent = prevParent;
            ModelCombiner.CombineModel(tableGroup, addCollider: true);
        }

        static void BuildPitcher(Vector3 p)
        {
            var pitcherGroup = ModelGroup("Bedside water pitcher and tea bowl", p);
            var prevParent = Parent;
            Parent = pitcherGroup;
            Lathe("Enamel bedside water pitcher", new Vector3(0, .04f, 0), new[]{ new Vector2(.07f, 0), new Vector2(.10f, .03f), new Vector2(.09f, .24f), new Vector2(.11f, .27f), new Vector2(.095f, .275f), new Vector2(.07f, .06f) }, S.Porcelain, 24);
            Tube("Pitcher handle", new Vector3(-.11f, .20f, 0), .085f, .012f, S.Metal, Quaternion.identity, 20);
            Lathe("Porcelain tea bowl", new Vector3(.16f, .04f, -.15f), new[]{ new Vector2(.035f, 0), new Vector2(.06f, .045f), new Vector2(.06f, .06f) }, S.Porcelain, 16);
            Parent = prevParent;
            ModelCombiner.CombineModel(pitcherGroup, addCollider: true);
        }

        static Material Mirror() => S.Plain("Aged bedroom mirror", new Color(.34f, .40f, .38f), .72f, .8f);

        static void Bed(Material bedding, bool bridal)
        {
            var old = Parent;
            Parent = ModelGroup(bridal ? "Handcrafted Cambodian bridal canopy bed" : "Traditional wooden family bed", new Vector3(-.9f, 0, 0));

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);

            // 1. Four carved canopy posts extending to 2.25m
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    var postPos = new Vector3(x * .88f, 0, z * 1.12f);
                    Box("Carved bed foot base", postPos + Vector3.up * .22f, new Vector3(.12f, .44f, .12f), S.DarkWood);
                    Cylinder("Canopy timber upright post", postPos + Vector3.up * .44f, .045f, 1.82f, S.DarkWood, 16);
                    Ball("Brass canopy post finial", postPos + Vector3.up * 2.28f, new Vector3(.08f, .11f, .08f), S.Brass);

                    if (bridal)
                    {
                        // Hanging jasmine flower garlands and marigolds wrapped around post tops
                        for (int g = 0; g < 4; g++)
                        {
                            Ball("Post flower bloom", postPos + new Vector3(0, 2.15f - g * .14f, 0), Vector3.one * .06f, g % 2 == 0 ? S.Cream : S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f));
                        }
                    }
                }

            // 2. Overhead polished bamboo canopy rails connecting the 4 posts
            Beam("Canopy rail left", new Vector3(-.88f, 2.22f, -1.12f), new Vector3(-.88f, 2.22f, 1.12f), .035f, S.Wood);
            Beam("Canopy rail right", new Vector3(.88f, 2.22f, -1.12f), new Vector3(.88f, 2.22f, 1.12f), .035f, S.Wood);
            Beam("Canopy rail head", new Vector3(-.88f, 2.22f, 1.12f), new Vector3(.88f, 2.22f, 1.12f), .035f, S.Wood);
            Beam("Canopy rail foot", new Vector3(-.88f, 2.22f, -1.12f), new Vector3(.88f, 2.22f, -1.12f), .035f, S.Wood);

            if (bridal)
            {
                // Floral garland draped across canopy head and foot rails
                for (int i = 0; i < 7; i++)
                {
                    float gx = -.72f + i * .24f;
                    Ball("Canopy rail flower", new Vector3(gx, 2.18f - Mathf.Sin(i / 6f * Mathf.PI) * .12f, -1.12f), Vector3.one * .055f, i % 2 == 0 ? S.Cream : S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f));
                }
            }

            // 3. Sturdy teak side frame & wooden slats
            Box("Bed timber side frame", Vector3.up * .40f, new Vector3(1.78f, .22f, 2.24f), S.DarkWood, true);

            // 4. Layered Cotton & Kapok Mattress
            Box("Kapok cotton mattress", Vector3.up * .54f, new Vector3(1.68f, .14f, 2.14f), S.Cream, true);
            Box("Cambodian floral cotton bedcover", Vector3.up * .615f, new Vector3(1.69f, .025f, 2.14f), bridal ? goldSilk : bedding);

            // 5. Sculpted High Headboard with carved lotus motifs
            Box("Timber carved headboard frame", new Vector3(0, .92f, 1.12f), new Vector3(1.82f, .78f, .085f), S.DarkWood);
            Grid("Carved lotus headboard arch", 16, 4, (u, v) =>
            {
                float x = (u - .5f) * 1.6f;
                float y = .92f + Mathf.Sin(u * Mathf.PI) * .28f + (v - .5f) * .35f;
                return new Vector3(x, y, 1.07f);
            }, S.Wood, Vector2.one, true);

            // 6. Pillows & Bolsters (Khnaoy Ko)
            for (int i = 0; i < (bridal ? 2 : 1); i++)
                Ball("Embroidered sleeping pillow", new Vector3(bridal ? (i - .5f) * .74f : 0, .74f, .78f), new Vector3(bridal ? .68f : 1.0f, .20f, .42f), bridal ? goldSilk : S.Cream);

            // Kapok cylinder bolsters laid horizontally flat along bed sides (Z axis)
            for (int side = -1; side <= 1; side += 2)
            {
                var bStart = new Vector3(side * .65f, .72f, -.55f);
                var bEnd = new Vector3(side * .65f, .72f, .55f);
                Grid("Kapok side bolster body", 16, 2, (u, v) =>
                {
                    float a = u * Mathf.PI * 2f;
                    Vector3 center = Vector3.Lerp(bStart, bEnd, v);
                    return center + new Vector3(Mathf.Cos(a) * .11f, Mathf.Sin(a) * .11f, 0);
                }, bridal ? redSilk : S.Pink, Vector2.one, true);
                Ball("Bolster silk cap front", bStart, new Vector3(.22f, .22f, .04f), bridal ? redSilk : S.Pink);
                Ball("Bolster silk cap back", bEnd, new Vector3(.22f, .22f, .04f), bridal ? redSilk : S.Pink);
                Tube("Bolster gold tassel ring front", bStart + Vector3.forward * .03f, .115f, .014f, goldSilk, Quaternion.identity, 16);
                Tube("Bolster gold tassel ring back", bEnd - Vector3.forward * .03f, .115f, .014f, goldSilk, Quaternion.identity, 16);
            }

            // 7. Folded Silk / Cotton Blanket
            Box("Folded woven blanket", new Vector3(0, .68f, -.75f), new Vector3(1.5f, .085f, .48f), bridal ? redSilk : bedding);

            // 8. Sheer White Mosquito Net (Mung) tied back at all 4 posts with silk rosettes
            MosquitoNet(bridal);

            var bedGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(bedGroup, addCollider: true);
        }

        static void MosquitoNet(bool bridal)
        {
            var net = S.Plain("Sheer bedroom mosquito gauze", new Color(.94f, .92f, .84f, .24f), .03f);
            net.shader = Shader.Find("Standard");
            net.SetFloat("_Mode", 2);
            net.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            net.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            net.SetInt("_ZWrite", 0);
            net.EnableKeyword("_ALPHABLEND_ON");
            net.renderQueue = 3000;

            // Overhead canopy roof net
            Grid("Canopy roof mosquito gauze", 16, 16, (u, v) => new Vector3((u - .5f) * 1.76f, 2.22f - Mathf.Sin(u * Mathf.PI) * Mathf.Sin(v * Mathf.PI) * .08f, (v - .5f) * 2.24f), net, Vector2.one, true);

            // 4 Gathered tie-back curtains at each canopy corner post
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    var postP = new Vector3(x * .84f, 0, z * 1.08f);
                    // Gathered net draped along post
                    Grid("Gathered corner mosquito curtain", 12, 16, (u, v) =>
                    {
                        float a = u * Mathf.PI * 2;
                        float r = Mathf.Lerp(.14f, .06f, Mathf.Sin(v * Mathf.PI));
                        return postP + new Vector3(Mathf.Cos(a) * r, 2.2f - v * 1.6f, Mathf.Sin(a) * r);
                    }, net, Vector2.one, true);

                    if (bridal)
                    {
                        // Crimson silk sash tie-back and rosette at 1.4m height
                        Tube("Canopy silk tie-back sash", postP + Vector3.up * 1.4f, .095f, .04f, redSilk, Quaternion.identity, 16);
                        Ball("Canopy silk sash rosette", postP + new Vector3(-x * .08f, 1.4f, -z * .08f), Vector3.one * .045f, redSilk);
                    }
                }
        }

        static void BuildDressingTable(Vector3 p, float yaw, bool bridal)
        {
            var dressingGroup = ModelGroup("Bridal dressing table mirror and jewelry casket", p);
            if (yaw != 0f) dressingGroup.transform.localEulerAngles = new Vector3(0, yaw, 0);
            var prevParent = Parent;
            Parent = dressingGroup;

            // Table base
            PropsBuilder.Table(Vector3.zero, new Vector2(1.15f, .55f), .70f, S.Wood);

            // Mirror frame and aged mirror glass
            Box("Dressing mirror timber frame", new Vector3(0, 1.15f, .21f), new Vector3(.65f, .83f, .075f), S.DarkWood, true);
            Box("Aged dressing mirror glass", new Vector3(0, 1.15f, .165f), new Vector3(.55f, .72f, .012f), Mirror());

            if (bridal)
            {
                // Jasmine garland framing the mirror
                for (int i = 0; i < 9; i++)
                {
                    float mx = -.28f + i * .07f;
                    Ball("Mirror garland jasmine flower", new Vector3(mx, 1.58f - Mathf.Pow((i / 8f - .5f) * 2, 2) * .08f, .15f), Vector3.one * .035f, S.Cream);
                }

                // Traditional 24k Gold Wedding Jewelry Display Tray (Khsae Kor Meas)
                var jewelryPos = new Vector3(-.28f, .72f, 0);
                Box("Red velvet jewelry tray", jewelryPos + Vector3.up * .015f, new Vector3(.26f, .03f, .20f), S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f));
                // Gold wedding necklace
                Tube("Gold wedding necklace (Khsae Kor Meas)", jewelryPos + Vector3.up * .035f, .065f, .008f, S.Brass, Quaternion.Euler(90, 0, 0), 16);
                Ball("Necklace gold lotus pendant", jewelryPos + new Vector3(0, .035f, -.075f), Vector3.one * .022f, S.Brass);
                // Twin gold filigree arm bangles
                Tube("Gold bangle left", jewelryPos + new Vector3(-.075f, .035f, .04f), .028f, .007f, S.Brass, Quaternion.identity, 12);
                Tube("Gold bangle right", jewelryPos + new Vector3(.075f, .035f, .04f), .028f, .007f, S.Brass, Quaternion.identity, 12);

                // Traditional Silver Matrimonial Betel Box (Hop Sla)
                var betelPos = new Vector3(.28f, .72f, 0);
                Box("Silver carved betel box", betelPos + Vector3.up * .045f, new Vector3(.18f, .09f, .13f), S.Metal, true);
                Ball("Betel box silver handle", betelPos + Vector3.up * .10f, Vector3.one * .018f, S.Metal);

                // Vintage glass perfume atomizer bottle
                var perfumePos = new Vector3(.10f, .72f, -.08f);
                Lathe("Bridal glass perfume flacon", perfumePos, new[]{ new Vector2(0, 0), new Vector2(.035f, .01f), new Vector2(.045f, .06f), new Vector2(.02f, .10f) }, S.Porcelain, 14);
                Ball("Atomizer bulb", perfumePos + new Vector3(.025f, .085f, 0), Vector3.one * .018f, S.Pink);
            }

            // Traditional carved comb & toiletry tin
            var comb = Box("Small dressing comb", new Vector3(-.05f, .72f, -.12f), new Vector3(.18f, .018f, .04f), S.DarkWood);
            for (int i = 0; i < 12; i++)
                Box("Comb tooth", new Vector3(-.13f + i * .014f, .72f, -.155f), new Vector3(.006f, .016f, .05f), S.DarkWood);
            Cylinder("Small toiletry tin", new Vector3(.28f, .72f, -.12f), .065f, .07f, S.Metal, 20);

            if (bridal)
            {
                // 2006 classic button mobile phone placed neatly on front-center of dressing table surface (free of jewelry tray)
                PropsBuilder.MobilePhone2006(new Vector3(-0.06f, .725f, -.06f), 15f);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(dressingGroup, addCollider: true);
        }

        static void BuildBridalWallSwags()
        {
            var wallSwagGroup = ModelGroup("Bridal bedroom wall silk valances", Vector3.zero);
            var prevParent = Parent;
            Parent = wallSwagGroup;

            // Scalloped dusty rose silk valance along bedroom header
            Grid("Bridal wall silk valance", 32, 6, (u, v) =>
            {
                float x = -1.8f + u * 3.6f;
                float y = 2.45f - Mathf.Sin(u * Mathf.PI * 2) * .18f - v * .14f;
                return new Vector3(x, y, -.02f);
            }, S.Pink, new Vector2(3, 1), true);

            WeddingYardBuilder.Rosette(new Vector3(0, 2.42f, -.04f), .14f);

            Parent = prevParent;
            ModelCombiner.CombineModel(wallSwagGroup, addCollider: false);
        }

        static void Wardrobe(Vector3 p, float yaw, bool bridal)
        {
            var old = Parent;
            Parent = ModelGroup("Teak wardrobe with louvered doors", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            Box("Wardrobe frame", Vector3.up * 1.1f, new Vector3(1.15f, 2.2f, .55f), S.DarkWood, true);
            Box("Wardrobe crown pediment", Vector3.up * 2.25f, new Vector3(1.22f, .1f, .62f), S.DarkWood);
            Box("Wardrobe drawer", Vector3.up * .22f, new Vector3(1.05f, .34f, .52f), S.Wood);
            for (int i = 0; i < 2; i++)
            {
                float x = (i - .5f) * .52f;
                Box("Wardrobe louver door", new Vector3(x, 1.25f, -.26f), new Vector3(.5f, 1.65f, .025f), S.Carving);
                Tube("Door handle", new Vector3(x + (i == 0 ? .2f : -.2f), 1.2f, -.29f), .035f, .008f, S.Brass, Quaternion.identity, 16);
            }

            if (bridal)
            {
                // Stack of traditional Cambodian woven wedding gift baskets (Kanhcheu) wrapped in cellophane
                var basketPos = new Vector3(0, 2.32f, 0);
                for (int b = 0; b < 3; b++)
                {
                    float by = b * .22f;
                    float br = .26f - b * .04f;
                    Cylinder("Wedding gift basket", basketPos + Vector3.up * by, br, .18f, S.Wood, 16);
                    Tube("Basket gold ribbon bow", basketPos + Vector3.up * (by + .09f), br + .01f, .02f, S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f), Quaternion.identity, 16);
                }
            }

            var wardrobeCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(wardrobeCombined, addCollider: true);
        }

        static void ReedMat(Vector3 p, bool bridal)
        {
            var old = Parent;
            Parent = ModelGroup("Handwoven wild reed floor mat", p);
            Grid("Woven reed mat floor surface", 16, 12, (u, v) => new Vector3((u - .5f) * 1.8f, 0, (v - .5f) * 2.4f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f), new Vector2(2, 3), true);

            if (bridal)
            {
                // Auspicious crimson red silk accent rug runner
                var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);
                Grid("Bridal crimson silk runner", 12, 16, (u, v) => new Vector3((u - .5f) * .85f, .008f, (v - .5f) * 2.2f), redSilk, new Vector2(1, 3), true);
            }

            var matCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(matCombined, addCollider: false);
        }

        static void Storage(Vector3 p, float yaw, bool bridal)
        {
            var old = Parent;
            Parent = ModelGroup("Cameraman style travel chest", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);
            Box("Travel trunk timber body", Vector3.up * .26f, new Vector3(.95f, .52f, .55f), S.Wood, true);
            for (int i = -1; i <= 1; i += 2)
            {
                Box("Iron reinforcement band", new Vector3(i * .38f, .26f, 0), new Vector3(.05f, .53f, .56f), S.Rust);
                Box("Trunk brass corner brace", new Vector3(i * .46f, .51f, 0), new Vector3(.06f, .06f, .57f), S.Brass);
            }
            Box("Trunk front iron latch", new Vector3(0, .38f, -.285f), new Vector3(.09f, .11f, .02f), S.Rust);

            var trunkCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(trunkCombined, addCollider: true);
        }

        static void BuildKeroseneLamp(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Cambodian bedside kerosene tin lantern - Changkheang Praeng Kart", p);

            // Pressed tin oil reservoir base
            Lathe("Lamp tin fuel font", Vector3.zero, new[]{
                new Vector2(.06f, 0), new Vector2(.075f, .025f), new Vector2(.07f, .08f), new Vector2(.04f, .11f), new Vector2(.04f, .13f)
            }, S.Metal, 18);
            // Brass burner collar and wick adjuster thumbwheel
            Cylinder("Brass burner collar", Vector3.up * .13f, .036f, .035f, S.Brass, 14);
            Box("Wick thumbscrew knob", new Vector3(-.042f, .145f, 0), new Vector3(.012f, .012f, .022f), S.Brass);

            // Clear fluted glass chimney (Tbong Changkheang)
            Lathe("Glass chimney", Vector3.up * .165f, new[]{
                new Vector2(.034f, 0), new Vector2(.052f, .045f), new Vector2(.045f, .12f), new Vector2(.030f, .24f), new Vector2(.032f, .26f)
            }, S.Porcelain, 16);

            // Warm burning wick flame glow
            Ball("Kerosene wick flame glow", Vector3.up * .18f, new Vector3(.018f, .032f, .018f), S.Bulb);

            // Box of Flying Pigeon / safety matches beside the lantern
            var matchPos = new Vector3(.11f, 0, .05f);
            Box("Cardboard matchbox", matchPos + Vector3.up * .01f, new Vector3(.075f, .02f, .048f), S.DarkWood);
            Box("Matchbox striker side", matchPos + new Vector3(0, .01f, -.024f), new Vector3(.072f, .016f, .002f), S.Black);
            Box("Matchbox red label top", matchPos + Vector3.up * .021f, new Vector3(.068f, .001f, .042f), S.Plain("Matchbox red cover", new Color(.78f, .18f, .18f), .1f));

            var lampCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(lampCombined, addCollider: true);
        }

        static void BuildBridalSlippers(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Bride velvet wedding slippers - Svak Meas", p);

            var crimsonVelvet = S.Plain("Nuptial crimson velvet", new Color(.68f, .10f, .18f), .35f);
            var goldThread = S.Plain("Gold embroidery thread", new Color(.88f, .72f, .18f), .50f);

            for (int foot = -1; foot <= 1; foot += 2)
            {
                var sp = new Vector3(foot * .085f, 0, 0);
                // Pointed curved oriental slipper sole
                Box("Slipper leather sole", sp + Vector3.up * .006f, new Vector3(.075f, .012f, .21f), S.DarkWood);
                // Velvet embroidered vamp covering forefoot
                Grid("Velvet slipper vamp", 8, 8, (u, v) =>
                {
                    float x = sp.x + (u - .5f) * .072f;
                    float y = .012f + Mathf.Sin(u * Mathf.PI) * .042f * (1f - v * .4f);
                    float z = sp.z + .01f + v * .10f;
                    return new Vector3(x, y, z);
                }, crimsonVelvet, Vector2.one, true);
                // Gold bullion embroidery crest on slipper toe
                Ball("Slipper gold toe crest", sp + new Vector3(0, .035f, .09f), new Vector3(.025f, .015f, .025f), goldThread);
            }

            var slipCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(slipCombined, addCollider: false);
        }

        static void BuildClothesDryingRack(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Traditional bamboo clothes hanging rail - Raneak Peuk Khao-Ao", p);

            var holSilk = S.Plain("Cambodian Ikat silk Sampot Hol", new Color(.72f, .22f, .30f), .25f);
            var phamungSilk = S.Plain("Emerald silk Phamung sarong", new Color(.16f, .48f, .34f), .25f);
            var kromaCheck = S.Plain("Checked Kroma scarf", new Color(.75f, .28f, .25f), .15f);

            // A-frame timber side uprights
            for (int side = -1; side <= 1; side += 2)
            {
                var sp = new Vector3(side * .65f, 0, 0);
                Beam("A-frame leg front", sp + new Vector3(0, 0, -.24f), sp + new Vector3(0, 1.45f, 0), .035f, S.Wood);
                Beam("A-frame leg back", sp + new Vector3(0, 0, .24f), sp + new Vector3(0, 1.45f, 0), .035f, S.Wood);
                Beam("A-frame cross stretcher", sp + new Vector3(0, .32f, -.16f), sp + new Vector3(0, .32f, .16f), .028f, S.DarkWood);
            }

            // Top horizontal hanging bamboo pole
            Beam("Top bamboo hanging rail", new Vector3(-.75f, 1.44f, 0), new Vector3(.75f, 1.44f, 0), .04f, S.Wood);

            // Garment 1: Draped Cambodian Ikat Silk Sampot Hol
            Grid("Draped Sampot Hol silk skirt", 12, 14, (u, v) =>
            {
                float x = -.35f + (u - .5f) * .42f;
                float y = 1.43f - v * .78f;
                float z = Mathf.Sin(u * Mathf.PI * 3f) * .022f;
                return new Vector3(x, y, z);
            }, holSilk, new Vector2(2, 4), true);

            // Garment 2: Emerald Silk Phamung
            Grid("Draped emerald Phamung silk", 12, 14, (u, v) =>
            {
                float x = .15f + (u - .5f) * .38f;
                float y = 1.43f - v * .72f;
                float z = Mathf.Sin(u * Mathf.PI * 3.5f) * .020f;
                return new Vector3(x, y, z);
            }, phamungSilk, new Vector2(2, 4), true);

            // Garment 3: Draped checked cotton Kroma
            Grid("Draped cotton Kroma", 8, 12, (u, v) =>
            {
                float x = .46f + (u - .5f) * .18f;
                float y = 1.43f - v * .88f;
                float z = -.02f + Mathf.Sin(u * Mathf.PI * 2f) * .015f;
                return new Vector3(x, y, z);
            }, kromaCheck, new Vector2(1, 4), true);

            var rackCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(rackCombined, addCollider: true);
        }

        static void BuildProtectiveYantra(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Buddhist protective wedding yantra amulet - Khaen Yan", p);

            var goldTrim = S.Plain("Yantra gold leaf border", new Color(.85f, .70f, .18f), .45f);
            var sacredCloth = S.Plain("Consecrated cotton cloth", new Color(.92f, .88f, .80f), .10f);
            var redInk = S.Plain("Consecrated cinnabar ink", new Color(.75f, .16f, .18f), .15f);

            // Wall mounting timber backboard
            Box("Yantra wooden frame", p, new Vector3(.46f, .56f, .025f), S.DarkWood);
            Box("Yantra gold inner bezel", p + new Vector3(0, 0, -.014f), new Vector3(.42f, .52f, .005f), goldTrim);

            // Consecrated sacred cloth inscribed with Pali incantations and Angkorian yantra
            Box("Sacred yantra white cloth", p + new Vector3(0, 0, -.017f), new Vector3(.38f, .48f, .002f), sacredCloth);

            // Yantra geometric concentric protection circles and temple spire emblem
            for (int r = 1; r <= 3; r++)
                Tube("Yantra protection circle", p + new Vector3(0, 0, -.019f), r * .052f, .003f, redInk, Quaternion.identity, 16);
            Ball("Yantra central lotus jewel", p + new Vector3(0, 0, -.021f), new Vector3(.035f, .035f, .004f), goldTrim);

            var yanCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(yanCombined, addCollider: false);
        }
    }
}
