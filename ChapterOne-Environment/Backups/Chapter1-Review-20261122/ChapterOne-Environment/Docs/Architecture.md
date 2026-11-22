# Chapter One environment architecture

The environment is generated from code so the designed house, wedding site,
terrain, and village can be rebuilt consistently.

## Runtime assembly

`Assets/ChapterOne/Scripts` contains only code required by the built game:

- `EnvironmentCamera` handles inspection controls and review captures.
- `EnvironmentLighting` switches the five authored atmosphere stages.
- `EnvironmentPerformance` owns camera culling and runtime quality policy.
- `SiteLayout` is the shared source of site dimensions and landmarks.
- `ModelRoot` marks selectable generated models.

## Editor assembly

`Assets/ChapterOne/Editor` contains scene generation and is excluded from
player builds:

- `EnvironmentBuild` orchestrates generation and Windows builds.
- Feature builders own one part of the environment.
- `LandscapeBuilder` is split by responsibility: the main file owns the site
  shell, `Trees` owns tree geometry, `ForestFloor` owns ground dressing, and
  `ForestLayout` owns placement. All four remain one partial builder type.
- `PropsBuilder` keeps shared props in the main file and Cambodia-specific
  household and wedding props in `PropsBuilder.Cambodian`.
- `MeshFactory` creates procedural meshes.
- `ModelCombiner` consolidates each editable model.
- `EnvironmentPerformanceOptimizer` applies distant-forest culling and
  practical-light policy without changing authored transforms or materials.
- `SpatialRenderBatcher` creates render-only material batches while retaining
  the authored hierarchy and colliders. Use **THAT NIGHT > Restore authored
  renderers** to remove those batches for detailed manual editing.
- `EnvironmentBuildSceneProcessor` strips disabled authoring renderers only
  from the temporary player-build copy, avoiding duplicate mesh data while the
  editable scene remains intact.

## Safe workflow

1. Change the owning feature builder.
2. Run **THAT NIGHT > Regenerate environment scene**.
3. Run **THAT NIGHT > Optimize current environment** after manual scene edits.
4. Check the Console and compare rendered views.
5. Profile a normal rendered interval without screenshot readback.

The generated scene remains the output of the builders. Save manual variants
under a different scene name before regenerating.
