# Chapter 1 quality review - 22 November 2026

This is a repair pass on the existing project, not acceptance of the complete chapter.

## Confirmed direction

- Realistic but simpler visuals, with performance taking priority over excessive detail.
- The bride is the first-person protagonist. Mother is her biological mother.
- Display the caller only as **Brother**. His personal name is unspecified. The underlying male-cousin relationship remains in continuity notes.
- Opening order: wake up, answer Brother on the bedroom mirror/dressing table, hear Mother calling, leave the bedroom and meet Mother, return to the mirror table for preparation.
- Preserve the authored house, room layout, rear kitchen, and floor ceremony arrangement. Keep the disappearance unexplained.

## Repairs implemented

- Corrected the third material slot on four existing skinned renderers. Their source meshes have body, head, and opacity submeshes; the opacity submesh previously received the opaque face texture. It now uses the supplied opacity texture with a cutout material. Future character builds use the same mapping.
- Added clamped head tracking relative to the skeleton's rest orientation. Heads return to rest when the player leaves the supported range.
- Corrected a world/local rotation mismatch in the bride's reach animation and connected its target to the object being used. This remains procedural animation.
- Added an exclusive story-sequence guard, preventing repeated interactions from launching simultaneous chapter transitions.
- Queued dialogue requests rather than stopping the previous coroutine and stranding its completion callback. Added playback of assigned voice clips, reading-time-aware subtitles, and Space to advance a line. No voice recordings were created.
- Kept Brother's call before Mother, retained the bedroom mirror-table location, prevented answering an idle phone or replaying an ended call, and allowed free mouse look during the call.
- Connected an actual dressing-table interaction to preparation. Mother is repositioned during a fade for that conversation. Dressing, makeup, jewelry handling, and a bride reflection are still missing.
- Hid the groom and the bride's visible body before the empty-room reveal, while keeping the camera and director active.
- Updated both copies of the story and first-person production documents to the latest user corrections.

## Verification

- Unity 6000.3.28f1, Built-in rendering pipeline, live `ChapterOne-Environment` project.
- Editor compilation completed with zero reported console errors after the final code changes.
- Accelerated Editor runtime check used the real opening coroutine, phone interaction, door interaction, Mother conversation, dressing-table interaction, ceremony, and reception handlers. Repeated interaction calls exercised the sequence guard. This checks state transitions, not ordinary-speed movement or animation quality.
- Observed: ringing phone in `Scene02_WakingUpAndPhone`, caller `Brother`, parent `Bridal dressing table mirror and jewelry casket`, local position approximately `(-0.06, 0.725, -0.06)`.
- Observed: call ended before `Scene03_MotherCallsDoor`; opening the bridal door reached the living-room stage; Mother conversation reached preparation; dressing-table interaction reached ceremony; ceremony reached reception; the disappearance hid both spouse models.
- Play mode stopped before the final title assertion. The ending/title, full collision traversal, recorded voice playback, and a standalone frame-rate benchmark are not verified by that run.
- Runtime time scale was returned to 1. Editor is outside Play mode.
- Final Windows build succeeded with zero errors and zero warnings. Build job `build-a3174ccffc`, reported size 413.03 MB, executable `Builds/ChapterOne-Review/THAT NIGHT.exe` (667,136 bytes; supporting data lives beside it). Built directly from the saved scene without regeneration.
- Headless startup check loaded player data and assemblies, initialized PhysX, and remained running without error/exception matches in `Logs/chapter1-review-startup.log`; the test process was then stopped. This is startup evidence only, not a rendered gameplay or frame-rate test.
- Captures are in `Captures/Review-20261122`. The file named `chapter-ending.png` was captured after Play mode stopped and is **not** evidence of the ending.

## Production gaps

| Area | Current evidence | Required work |
| --- | --- | --- |
| Principal characters | Two imported FBX assets; the bride reuses the female model | Distinct bride and mother, suitable ages, Cambodian clothing, bridal variants, hair, facial rigs, and asset provenance |
| Guests and officiants | Primitive-built figures and limited idle movement | Consistent realistic models, seated/eating/dancing variants, appropriate ceremony presentation |
| Animation | No separate `.anim` clips or Animator controllers found in project Assets | Authored locomotion, phone grip, doors, dressing, ceremony, reaction, running and searching; verify any embedded FBX clips before reuse |
| Preparation | A station and conversation | Genuine mirror reflection, clothing change, jewelry and mother-daughter animation |
| Ceremony and reception | Existing environment plus dialogue/state changes | Staged ritual actions, arrivals, guest movement, dancing, and meaningful player participation |
| Disappearance | Sound/camera sequence and hidden spouse models | Visible guest rush, door action, search behavior, mother's performance, final title verification |
| Audio | Existing SFX/ambience; voice playback support | Recorded dialogue, lip sync, suitable wedding music and crowd soundscape |
| Performance | Existing low-power profile; very large serialized scene | Standalone CPU/GPU measurement, asset budgets, texture/material cleanup, measured crowd/render costs |

The current cast and scenes do not meet the requested realistic production standard. Correct material slots and a green compile do not establish that standard. No final visual score, finished-chapter claim, or smooth-FPS claim is made.

## Preservation and references

Pre-edit copies: `Backups/Chapter1-Review-20261122`. An additional scene copy is `BeforeSceneRepairs.unity` in that folder. The environment generator was not run. The normal `EnvironmentBuild.Build()` invokes regeneration, so packaging uses the saved scene directly instead.

Guidance read: Unity `using-unity`, `craft-game-floors`, `character-motion`, `visual-scorecard`, and `unity-mcp-orchestrator`. Existing Built-in rendering was retained. Reviewed Unity 6.3 documentation for [AudioSource.Play](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/AudioSource.Play.html) and [Standard shader rendering mode](https://docs.unity3d.com/6000.3/Documentation/Manual/StandardShaderMaterialParameterRenderingMode.html).
