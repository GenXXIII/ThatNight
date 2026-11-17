# Environment validation — 2026-11-16

This review covers the separate Chapter One environment project. Gameplay is outside this stage.

## Current build and layout

- Unity 6000.3.28f1 rebuilt the Windows preview successfully after removing the right-side ceremony table and wooden bench, and rotating the couple's floor cushions toward the fruit display. The completed captures from November 15 were visually reviewed on November 16.
- The saved editor scene reports `dirty=False playing=False`.
- The latest native batch review completed 40 day/night captures at 1280×720 on Intel UHD Graphics 630, with 1,213 scene renderers. The capture run completed without reported runtime errors.
- Rendered [entrance](../Review/day-Entrance.png) and [overview](../Review/day-Site-overview.png) checks show three main roof sections. The left and right roof fronts align with the recessed fabric frontage at z=-10.5. The middle roof extends forward to z=-14.
- The two tied pink inner entrance curtains, their rosettes, crossbar and overhead doorway swags are removed. The remaining inner ivory panels, return curtains and their posts are also removed, along with the middle roof's front valance. The opening is six meters wide; the outer porch side covers and carpet remain.
- The frontage spans x=-22 to 22. Its side runs end at z=10.5 and the rear remains open toward the house.
- Twenty guest tables occupy the left section in four columns and five rows. A saved-scene hierarchy audit confirms 120 chair groups in that seating zone. A clear dance floor and low ceremonial platform occupy the right. The entrance photo corner has two chairs and flowers. The rear-right cooks' area uses matching pink-and-ivory canvas, with preparation, washing, storage and banquet cooking props. These areas were inspected in native rendered views during this revision.
- Plant generation excludes the covered area with additional branch clearance. The old small internal canopy is removed. No trees are visible inside the inspected seating, dance and cooking spaces.

- The loose bench, jar and basket beside the left guest tables are removed from the landscape builder. The marked inner doorway decorations remain absent from the saved scene and rendered entrance.
- The new [under-house sitting platform](../Review/day-Under-house-sitting-platform.png) was inspected in daylight and at night. Its slatted timber frame, reed-style mat and cushion sit under the front-left house bay. The B shortcut opens this inspection view.

- Both side frontages have pleated ivory fabric gable covers closing the gap between the horizontal curtains and pitched roof edges. The covers share the lower curtains' woven material and texture scale. The new [front fabric inspection view](../Review/day-Front-fabric-covers.png) was checked in daylight and at night; visible vertical folds and billowing replace the previous flat appearance. Press G to inspect.
- The [processional fruit trays](../Review/day-Processional-fruit-trays.png) are now on a floor mat beside the left seating (F). The [wrist-tying ceremony area](../Review/day-Wrist-tying-ceremony.png) occupies the right side (J), with two named floor cushions facing the fruit to the left. The right-side table and wooden bench are removed. Two cotton-thread bowls, candles and flowers rest directly on the floor mat. The rendered ceremony view confirms the open space between the couple's cushions and the fruit display. A saved-scene hierarchy audit confirms five trays under the left group, zero fruit trays under the right group, two thread bowls and two couple seats. The central walkway and bedroom approaches remain open.

## Existing layout and navigation

The road, house structure, household kitchen and cattle shelter were not redesigned in this revision. The approach remains approximately 110 meters long and 4.6 meters wide. The clearing elevation remains 21 meters; the cattle shelter is 3.6 by 12 meters.

Earlier navigation verification passed wheel zoom, middle-button pan and close-range zoom. That check was not repeated for these geometry-only changes. See the [beginner controls](../README.md#easier-dragging-and-zooming-in-the-scene-view) for manual inspection.

## Limits

The latest `Review/result.json` records `visualReviewComplete: true` and `timingComplete: false`. The first flag means capture completion, not user acceptance. This was a batch capture run without interactive frame timing; zero timing values are placeholders, not measured frame rates.

This is an environment study. Geometry, foliage, cloth, household detail and lighting still need production art refinement to approach photorealism. Build and rendered checks do not establish final art quality or gameplay performance.
