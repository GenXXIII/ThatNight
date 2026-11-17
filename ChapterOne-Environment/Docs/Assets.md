# Environment assets

## Reference

`Docs/Reference/House-Wedding-Reference.png` is the user-selected image. It guides layout and atmosphere and is not used as a background pasted over the 3D scene.

## Materials

Downloaded from ambientCG's official asset API as 1K JPG sets. ambientCG provides these under [CC0](https://docs.ambientcg.com/license/), including distribution of raw files in a game project. Checked 2026-11-13.

- [Ground054](https://ambientcg.com/a/Ground054): packed courtyard and road soil; subtle weathering on earthenware. Downloaded as 1K JPG on 2026-11-13 through the official API; CC0 license rechecked.
- [Ground084](https://ambientcg.com/a/Ground084): dry litter on the cattle shelter floor; retained from the earlier terrain pass.
- [Ground037](https://ambientcg.com/a/Ground037): surrounding planted earth.
- [Planks037A](https://ambientcg.com/a/Planks037A): weathered boards and timber joinery.
- [RoofingTiles014B](https://ambientcg.com/a/RoofingTiles014B): clay roof.
- [WoodFloor051](https://ambientcg.com/a/WoodFloor051): upper floorboards.
- [Wood066](https://ambientcg.com/a/Wood066): bark and rough posts.
- [Fabric035](https://ambientcg.com/a/Fabric035): cloth weave; the shader desaturates its base color before applying pink/ivory tints.
- [Metal033](https://ambientcg.com/a/Metal033): aged metal roofing.

`Tools/Download-Materials.ps1` records the reproducible download procedure. Unity imports only image/text entries from the archives. Texture sizes are capped at 1024 for the integrated-graphics environment study.

`MangoFoliage.png` and `CarvedTeak.png` reuse original images generated earlier in this conversation with the built-in image-generation tool. They were copied into this project from the local generated-image collection. No new image generation was needed for this environment stage. Geometry is authored by the C# builder files in this project.

There are no downloaded commercial game models, Home Sweet Home assets, character meshes, voices or music in this project.
