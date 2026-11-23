# THAT NIGHT — Chapter One environment

Unity 6000.3.28f1 project for the current Chapter 1 wedding, house, forest and story gameplay. The saved scene is the source of truth. Builds preserve your scene edits.

Build with **THAT NIGHT > Build current Chapter 1 preview**, then launch **Play Chapter 1 - The Wedding.cmd** for the story or **Preview Environment.cmd** for inspection. All launchers use the same current executable.

## See the environment

1. Double-click **Preview Environment.cmd** in this folder.
2. Press **1** for the summit entrance, **2** house, **3** canopy, **4** rear kitchen, **5** living room, **6** overview, **7** bedroom one, **8** bedroom two, **9** cattle shelter, or **0** for the lower hidden forest road. Press **T** to inspect the house landmark on the upper climb, **R** for the final wedding reveal, and **M** to look outward from the mountaintop wedding.
3. Hold the **right mouse button** and move the mouse to look around. While holding it, use **WASD** to move and **Q/E** to move down/up. Hold **Shift** to move faster.
4. Press **N** to compare evening and daylight, **H** to hide the small controls panel, or **F12** to save a screenshot under `Review`.
5. Close the window when finished. This camera can fly through objects because it is an environment inspection tool.

Wedding layout views: **P** shows the entrance photo corner, **V** shows dancing and traditional events on the right, and **C** shows the cooks' banquet kitchen under the rear-right extension. **3** shows guest seating on the left and **6** shows the three joined roof sections. **B** shows the low wooden sitting platform beneath the house. **J** shows the bride and groom's wrist-tying ceremony area on the right. **F** shows the processional fruit trays on floor mats beside the left seating. **G** shows the pleated fabric covers above the front curtains.

## Open and edit in Unity

1. Double-click **Open Unity Project.cmd**. Alternatively, in Unity Hub choose **Projects → Add → Add project from disk** and select this `ChapterOne-Environment` folder.
2. In Unity's Project panel, open **Assets → ChapterOne → Scenes → ChapterOneEnvironment**.
3. The Hierarchy contains named groups: **01 House**, **02 Wedding**, **03 Kitchen and yard props**, **04 Ground**, **05 Village**, **06 Lighting**, **07 Review camera**, **08 Cattle shelter**.
4. Press Unity's Play button to inspect. Stop Play before editing; changes made during Play are temporary.
5. Before manually editing the generated scene, use **File → Save As** and give your copy a different name, such as `ChapterOneEnvironment_YourEdit`.

### Easier dragging and zooming in the Scene view

The small **CHAPTER ONE** panel inside the Scene view has **House**, **Courtyard**, **Kitchen**, **Cattle shed** and **Forest road** buttons. Click one to set a useful camera pivot and distance.

- **Mouse wheel:** zoom. The easy mode continues moving when you reach the orbit pivot, so close-up zoom does not stall.
- **Middle mouse + drag:** pan sideways/up/down.
- **Alt + left mouse + drag:** orbit around the camera pivot.
- **Right mouse + WASD:** fly around; **Q/E** move down/up and **Shift** increases speed.
- Select an object, move the cursor over the Scene view and press **F** to frame it.

**Easy editing: ON** uses the editor's simpler lighting and hides distant village scenery only in the Scene view. This reduces work while building the house. **Artwork view** restores the full village and scene lighting for visual review. These controls do not remove game objects or change the standalone preview's scene lighting. The same commands are under **THAT NIGHT → Scene navigation**.

## Organization

`Assets/ChapterOne/Scenes` contains the authored scene. `Scripts` is organized into Environment, Forest, Player, Story, Interactions and Preview. `Editor` separates Build, Navigation, Performance, Integration and feature-specific Authoring tools. Existing models, materials, prefabs, textures and shaders retain their asset identities.

The old full-scene generator and its obsolete builders have been removed. There is no regeneration step. Existing forest/story setup cannot overwrite your edited content. See [architecture and safe editing](Docs/Architecture.md) for the complete structure and repair workflow.

## Build and review

Run `powershell -ExecutionPolicy Bypass -File .\Tools\Build.ps1` from this folder, or use **THAT NIGHT > Build current Chapter 1 preview**. Stop Play first. The open Editor saves and builds the active authored scene. With Unity closed, the script builds the single enabled scene in Build Profiles. Output: `Builds/ChapterOne/THAT NIGHT.exe`. It never regenerates the environment.

Run `powershell -ExecutionPolicy Bypass -File .\Tools\Review.ps1` to capture forty day/night views and sample the entrance view's rendering performance. Leave its visible window focused until it closes. Results and actual Unity images are written to `Review`. This is a short fixed-view measurement, not a complete performance benchmark.

Add `-CaptureOnly` to save the same forty images in the background without an interactive frame-rate measurement. The result identifies this as batch capture. An interrupted review must produce a fresh result; the script rejects stale results from previous runs.

The source image is an art-direction reference. The delivered 3D scene remains a prototype; it does not yet match the reference's photographic detail, fabric simulation, clutter or final baked lighting. Review materials, silhouettes and gameplay separately. See [reference decisions](Docs/Reference-Decisions.md), [asset provenance](Docs/Assets.md) and [verified results and limits](Docs/Validation.md).
