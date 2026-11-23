# Chapter One architecture

## Source of truth

The authored scene in `Assets/ChapterOne/Scenes` owns the current house, wedding,
forest and Chapter 1 gameplay. Scene edits are saved directly. The old procedural
full-environment generator has been removed; building never recreates the map.

## Runtime assembly

`Assets/ChapterOne/Scripts` is divided by feature, in the existing
`ThatNight.ChapterOne` assembly. Existing script GUIDs are preserved.

- `Environment`: inspection camera, lighting, layout constants, render markers and quality policy.
- `Forest`: walking, forest detail visibility and instanced foliage.
- `Player`: interactions, body, animated hands and eyelid overlay.
- `Story`: chapter progression, dialogue, family NPCs, ambience and phone.
  `ChapterOneDirector` is split into state/control, `Sequences` and `UI` partials;
  its original MonoBehaviour GUID and serialized fields are preserved.
- `Interactions`: doors, pickups, guests and preparation station.
- `Preview`: explicit launch modes and opt-in gameplay review.

## Editor assembly

`Assets/ChapterOne/Editor` is excluded from the player:

- `Build`: one build pipeline, command-line mailbox and build-copy stripping.
- `Authoring/Characters`: character and hands construction plus material repair.
- `Authoring/Gameplay`: installation and targeted story/door repair.
- `Authoring/Forest`: optional forest installation and shared terrain math.
- `Authoring/Shared`: material and model helpers still used by those tools.
- `Performance`: render batching and environment optimization.
- `Navigation`: Scene-view navigation and atmosphere menus.
- `Integration`: Editor integration startup.

The 25 old house, wedding, kitchen and landscape generation scripts were removed.
Terrain calculations still required by forest tools live in `ForestTerrainLayout`.
Existing meshes, materials, prefabs and serialized scene objects are preserved.
Removed source can be recovered from Git commit `378eaee` if historical reference
is needed. Backup folders outside `Assets` are not compiled into this project.

## Build and preview contract

Use **THAT NIGHT > Build current Chapter 1 preview** or `Tools/Build.ps1`.
An interactive build saves the active authored scene if dirty and builds that
scene. It rejects Play mode, additive scenes and scenes outside
`Assets/ChapterOne/Scenes`. Selecting a manual variant updates Build Profiles so
later batch builds use the same scene. Batch builds require exactly one enabled
saved scene and do not run any generator or authoring setup.

All launchers use `Builds/ChapterOne/THAT NIGHT.exe`:

- Play Chapter 1 / Play First Person: current story flow.
- Preview Environment: `-environmentPreview`, inspection without story startup.
- Preview Horror Forest: `-forestPreview`, forest inspection without story startup.
- Review Gameplay: `-gameplayReview`, opt-in review.
- Tools/Review.ps1: `-environmentReview`, environment captures.

Old executable folders are not launch targets. Rebuild after changing a scene.
`EnvironmentBuild.Generate()` only reports that legacy regeneration was removed;
there is no regeneration menu. Existing forest and Chapter 1 setup commands reject
reinstallation rather than replacing hand-edited content. Use targeted repair
commands for existing components.

## Editing and performance

1. Open the authored scene, stop Play, edit and save.
2. If editing batched geometry, use **Restore authored renderers** first.
3. Use **Optimize current environment** afterward to refresh render batches.
4. Build the current preview. No authoring or optimization runs implicitly.

Scene-navigation controls affect Scene-view visibility and camera settings only;
they no longer change global quality presets. Build-time stripping operates on
Unity's temporary player copy. It retains colliders and never saves that copy
back over the authored scene. FPS and final visual quality require separate
rendered measurements; source cleanup alone does not establish either.

Material helpers preserve existing authored materials. Play-mode atmosphere uses
a runtime sky instance so leaving Play cannot overwrite the shared sky asset.
The old standalone guest setup and bedroom-spawn menu were removed; current
Chapter 1 installation and targeted repairs own story wiring.
