using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Household-scale fittings for the larger circa-2006 rural cooking shelter.
    internal static class KitchenPropsBuilder
    {
        public static void Build()
        {
            // 1. Raised earth and brick hearth
            var hearthGroup = ModelGroup("Raised earth and brick hearth", new Vector3(-1.1f, 0, 2.25f));
            var prevParent = Parent;
            Parent = hearthGroup;
            Box("Raised earth hearth base", Vector3.up * .23f, new Vector3(3.1f, .46f, 1.4f), S.Clay, true);
            for (int row = 0; row < 3; row++)
                for (int brick = 0; brick < 11; brick++)
                    Box("Exposed hearth brick", new Vector3(-1.38f + brick * .275f + (row % 2) * .07f, .075f + row * .145f, -.715f), new Vector3(.26f, .13f, .08f), S.Roof);
            Parent = prevParent;
            ModelCombiner.CombineModel(hearthGroup, addCollider: true);

            // 2. Clay wood and charcoal stoves (Chongkran Kroap)
            for (int i = 0; i < 2; i++)
            {
                var p = new Vector3(-1.85f + i * 1.45f, .46f, 2.2f);
                var stoveGroup = ModelGroup("Traditional clay cooking stove", p);
                prevParent = Parent;
                Parent = stoveGroup;
                Lathe("Clay wood and charcoal stove", Vector3.zero, new[]{ new Vector2(.23f, 0), new Vector2(.31f, .12f), new Vector2(.29f, .36f), new Vector2(.22f, .38f), new Vector2(.2f, .12f) }, S.Clay);
                Box("Dark stove fuel opening", new Vector3(0, .12f, -.28f), new Vector3(.20f, .17f, .035f), S.Black);
                for (int log = 0; log < 3; log++)
                    Beam("Firewood entering stove", new Vector3(-.09f + log * .08f, .08f, -.65f), new Vector3(-.07f + log * .07f, .09f, -.17f), .065f, S.Bark);
                PropsBuilder.Pot(Vector3.up * .38f, .35f);
                Parent = prevParent;
                ModelCombiner.CombineModel(stoveGroup, addCollider: true);
            }

            // 3. Main prep table with chopping board, cleaver, and mortar
            var prepTableGroup = ModelGroup("Kitchen food preparation table set", new Vector3(.2f, 0, -1.25f));
            prevParent = Parent;
            Parent = prepTableGroup;
            PropsBuilder.Table(Vector3.zero, new Vector2(2.6f, 1.05f), .82f, S.Wood);
            Cylinder("Round wooden chopping board", new Vector3(.15f, .855f, 0), .28f, .06f, S.DarkWood, 24);
            Box("Kitchen cleaver blade", new Vector3(.20f, .89f, -.01f), new Vector3(.21f, .015f, .09f), S.Metal);
            Box("Kitchen cleaver wood handle", new Vector3(.36f, .895f, -.01f), new Vector3(.13f, .025f, .035f), S.DarkWood);
            Lathe("Wooden mortar", new Vector3(-.75f, .85f, .05f), new[]{ new Vector2(.09f, 0), new Vector2(.15f, .07f), new Vector2(.17f, .22f), new Vector2(.12f, .24f), new Vector2(.09f, .06f) }, S.DarkWood);
            Beam("Mortar pestle", new Vector3(-.75f, .95f, .05f), new Vector3(-.56f, 1.27f, .05f), .045f, S.Wood);
            Parent = prevParent;
            ModelCombiner.CombineModel(prepTableGroup, addCollider: true);

            PropsBuilder.Basket(new Vector3(.9f, .85f, -1.25f), .48f);

            // 4. Side serving table
            var sideTableGroup = ModelGroup("Kitchen serving table and cookware", new Vector3(2.45f, 0, 1.35f));
            prevParent = Parent;
            Parent = sideTableGroup;
            PropsBuilder.Table(Vector3.zero, new Vector2(1.05f, 3.6f), .86f, S.Wood);
            for (int i = 0; i < 3; i++)
                PropsBuilder.Pot(new Vector3(0, .90f, -.15f + i * .55f), .17f + i * .025f);
            Parent = prevParent;
            ModelCombiner.CombineModel(sideTableGroup, addCollider: true);

            // 5. Dish washing station
            var washTableGroup = ModelGroup("Dish washing table and basin", new Vector3(-2.3f, 0, -1.2f));
            prevParent = Parent;
            Parent = washTableGroup;
            PropsBuilder.Table(Vector3.zero, new Vector2(1.55f, 1), .70f, S.DarkWood);
            Basin(Vector3.up * .74f, .48f);
            Parent = prevParent;
            ModelCombiner.CombineModel(washTableGroup, addCollider: true);

            // Water storage jars
            PropsBuilder.Jar(new Vector3(-2.8f, 0, -2.8f), 1.3f);
            PropsBuilder.Jar(new Vector3(-1.65f, 0, -3.0f), 1.05f);
            Basin(new Vector3(-3.0f, .03f, -.1f), .37f);

            // 6. Dish storage rack
            Rack(new Vector3(2.35f, 0, 3.2f));

            // 7. Hanging woks
            for (int i = 0; i < 3; i++)
                HangingWok(new Vector3(-2.15f + i * .92f, 2.14f, 3.55f), .30f + i * .025f);
            Beam("Pan hanging rail", new Vector3(-2.7f, 2.5f, 3.54f), new Vector3(.45f, 2.5f, 3.54f), .055f, S.DarkWood);

            // 8. Firewood stack
            var woodGroup = ModelGroup("Stacked cooking firewood pile", new Vector3(-3.05f, 0, 1.7f));
            prevParent = Parent;
            Parent = woodGroup;
            for (int i = 0; i < 18; i++)
                Beam("Dry stacked cooking firewood", new Vector3((i % 3) * .14f, .13f + (i / 3) * .10f, -.6f), new Vector3((i % 3) * .14f, .13f + (i / 3) * .10f, .6f), .085f, S.Bark);
            Parent = prevParent;
            ModelCombiner.CombineModel(woodGroup, addCollider: true);

            for (int i = 0; i < 3; i++)
                PropsBuilder.Basket(new Vector3(2.3f + i * .8f, .01f, -2.9f), .8f + i * .15f);
            PropsBuilder.Bench(new Vector3(.1f, 0, -3.25f), 1.35f);
        }

        static void Basin(Vector3 p, float radius)
        {
            var plastic = S.Plain("Faded blue basin plastic", new Color(.12f, .31f, .39f), .25f);
            Lathe("Blue plastic washing basin", p, new[]{ new Vector2(radius * .65f, 0), new Vector2(radius, .22f), new Vector2(radius, .25f), new Vector2(radius - .025f, .25f), new Vector2(radius * .63f, .035f) }, plastic, 32);
            Cylinder("Basin water", p + Vector3.up * .13f, radius * .8f, .004f, S.Water, 24);
        }

        static void Rack(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Open wooden dish rack", p);
            for (int side = -1; side <= 1; side += 2)
                Box("Dish rack upright", new Vector3(side * .7f, .92f, 0), new Vector3(.07f, 1.84f, .08f), S.Wood);
            for (int shelf = 0; shelf < 3; shelf++)
            {
                float y = .55f + shelf * .55f;
                Box("Open kitchen storage shelf", new Vector3(0, y, 0), new Vector3(1.55f, .055f, .65f), S.Wood);
                if (shelf > 0)
                    for (int i = 0; i < 6; i++)
                    {
                        var dish = new Vector3(-.57f + i * .22f, y + .035f, 0);
                        Lathe("Stacked porcelain bowl", dish, new[]{ new Vector2(.06f, 0), new Vector2(.1f, .08f), new Vector2(.095f, .085f), new Vector2(.05f, .02f) }, S.Porcelain, 16);
                    }
            }
            var rackGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(rackGroup, addCollider: true);
        }

        static void HangingWok(Vector3 p, float radius)
        {
            var previous = Parent;
            Parent = ModelGroup("Hanging blackened cooking wok", p);
            Parent.localRotation = Quaternion.Euler(-90, 0, 0);
            Lathe("Wok bowl", Vector3.zero, new[]{ new Vector2(0, 0), new Vector2(radius * .6f, .035f), new Vector2(radius, .14f), new Vector2(radius * .97f, .15f), new Vector2(radius * .57f, .06f), new Vector2(0, .035f) }, S.Black, 28);
            Tube("Wok hanging handle", new Vector3(0, .14f, radius + .055f), .08f, .014f, S.Metal, Quaternion.Euler(90, 0, 0));
            var wokGroup = Parent;
            Parent = previous;
            ModelCombiner.CombineModel(wokGroup, addCollider: true);
        }
    }
}
