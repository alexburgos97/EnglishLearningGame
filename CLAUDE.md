# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

EnglishLearningGame is a Unity project teaching English grammar and vocabulary at CEFR level A1-A2. It was originally built as a Spatial.io metaverse experience (avatars in a shared virtual space) and is currently being migrated to run standalone in **WebGL**.

The experience is a single scene (`Assets/Scenes/EnglishWorld/EnglishWorld.unity`) with three areas toggled on/off at runtime rather than loaded as separate Unity scenes:
- **Lobby** — hub area, includes the "Biblioteca" (library) with `RecursosAdicionales` links out to external resources.
- **GrammarWorld** — three grammar mini-games/zones: Crater (bridge/sentence-builder quiz), Laberinto (maze quiz), Volcan (volcano rock-dodging present-simple/past-simple quiz).
- **VocabWorld+** — vocabulary islands (Isla 1, Isla2, Isla3, Isla Central) with drag-and-drop, matching, and math mini-games.

`WorldManager` (`Assets/Scripts/WorldManager.cs`) owns the three top-level GameObjects (`espacioLobby`, `espacioGrammarWorld`, `espacioVocabWorld`) and switches between them via `SetActive`. There is no scene-loading/async-load flow — all three areas coexist in the loaded scene.

## Spatial.io migration (in progress)

The Spatial Creator Toolkit SDK package (`io.spatial.unitysdk`) is no longer resolved in `Packages/packages-lock.json` (it's gone from the effective dependency set even though the `package.openupm.com` scoped registry entry is still in `Packages/manifest.json`), so `SpatialSys.UnitySDK` types are unavailable. All gameplay code has been ported off it:

- `Assets/Scripts/NewAssembly.asmdef` no longer references `SpatialSys.UnitySDK` (removed — it was the source of the "assembly could not be resolved" state that caused a wave of `SpatialBridge`/`SpatialTriggerEvent` compile errors).
- All former `SpatialBridge.*` call sites were replaced: `RecursosAdicionales.cs` uses `Application.OpenURL`; `VocabWorld+/BridgeSpeedBoost.cs`, `VocabWorld+/Isla3/Isla3Manager.cs`, `VocabWorld+/Isla Central/IslandCentralManager.cs`, `GrammarWorld/Crater/RespawnTrigger.cs`, `GrammarWorld/Volcan/LavaBlocker.cs`, and `VocabWorld+/Isla2/RespawnPoint.cs` now route through `PlayerController.Instance` (see **Player system** below) instead of `SpatialBridge.actorService.localActor.avatar.*`. `IslandCentralManager.OnPlayerEnter()` dropped a `SpatialBridge.questService.quests[5].Start()` call with no in-house replacement (Spatial's built-in quest log has no equivalent here — use `GameProgressManager` if that tracking is needed later).
- `GrammarWorld/Volcan/VolcanoTrigger.cs` no longer calls `GetComponent<SpatialTriggerEvent>().enabled = false` — its own `activated`/`psCompleted` bool flags already guard against re-triggering, so the call was redundant once removed.
- **Trigger conversion**: Spatial's `SpatialTriggerEvent` component (used in the Editor to fire UnityEvents like `BlockTrigger.Activar()`, `MazeTrigger.OnPlayerEnter()`, `VolcanoTrigger.Activar()`, `IslandCentralManager.OnPlayerEnter()`, `RespawnTrigger.Teletransportar()`) is gone from the project too. The replacement is `Assets/Scripts/Player/PlayerTriggerZone.cs` — drop it on a `BoxCollider` (`Is Trigger` on) and wire its `onPlayerEnter`/`onPlayerExit` UnityEvents to the same methods those Spatial components used to call. **This scene-level rewiring (each former Spatial trigger GameObject → BoxCollider + PlayerTriggerZone + re-hook the UnityEvent) is manual Editor work that has not been done yet** — the C# side is ready but `EnglishWorld.unity` still has the old (now-broken/missing) Spatial trigger components on those GameObjects.
- `Assets/Spatial/` holds Spatial-specific config/data assets (`ProjectConfig.asset`, `SpaceTemplate_*.asset`, `Space_*.asset`, avatar/prefab definitions). `Assets/Spatial/Generated*` and `Assets/Spatial/Temp*` are gitignored build artifacts of the Spatial toolkit, not hand-authored.
- `Assets/Examples/` (Space_FeatureDemo, Space_GolfCourse_Driving, Space_HyperJump_VisualScripting, Avatar_Spatian, etc.) are Spatial SDK sample/template assets, not game content.
- A separate, untracked sibling folder `EnglishLearningGame_WebGL/` (its own git repo, not part of this repository) exists alongside this project — treat it as a working copy for the WebGL port, not something to edit from here unless the user points you at it explicitly.
- `WebGL_Build/` at the repo root is a raw WebGL build output (untracked, not gitignored by the current `.gitignore` since the pattern only matches a directory literally named `Build`/`build`).

Still open: decide what to do with avatar/space assets under `Assets/Spatial` and `Assets/Examples` that have no meaning outside Spatial, and finish the scene-level trigger rewiring described above.

## Player system (replaces the Spatial avatar)

`Assets/Scripts/Player/` holds the WebGL-native replacement for Spatial's built-in avatar, added during the migration:

- `PlayerController.cs` — `CharacterController`-based third-person movement, singleton via `Instance` (same pattern as `GameProgressManager`/`PassportManager`). Reads `Input.GetAxis("Horizontal"/"Vertical")` (WASD + arrow keys out of the box via the default Input Manager), moves relative to the assigned camera's flattened forward/right, applies gravity, and rotates the player to face the movement direction. Exposes `SetSpeed`/`ResetSpeed` (used by `BridgeSpeedBoost`) and `Teleport(position, rotation)` (safe respawn/teleport for a `CharacterController` — disables the controller before moving it, matching every other script that used to call Spatial's avatar teleport/position API).
- `ThirdPersonCamera.cs` — follows a `target` transform from behind/above with `SmoothDamp`, orbits via mouse when the cursor is locked (click on the game view to lock, `Esc` to release — required for WebGL's pointer-lock security model), and uses `Physics.Linecast` against `collisionMask` to pull the camera in when terrain/objects would otherwise clip it.
- `PlayerTriggerZone.cs` — generic Spatial-trigger replacement described above.
- The `Player` tag was added to `ProjectSettings/TagManager.asset` (the project previously had zero custom tags) — trigger/respawn scripts check `other.CompareTag("Player")` before acting.

**Not yet done** (Editor-only work, not expressible as file edits): building the actual `Player` GameObject (`CharacterController` + `PlayerController` + visible capsule mesh, tagged `Player`) and camera rig (`ThirdPersonCamera` on the scene's main camera, `target` pointed at the player) in `EnglishWorld.unity`, plus the trigger-zone rewiring noted above. Until that's done, `PlayerController.Instance` is `null` at runtime and every script that guards on it (`BridgeSpeedBoost`, `Isla3Manager`, `RespawnTrigger`, `LavaBlocker`, `RespawnPoint`) silently no-ops.

## Progress/state architecture

- `GameProgressManager` (`Assets/Scripts/GrammarWorld/GameProgressManager.cs`) is a scene-singleton (`Instance`) tracking medal/badge completion per world, persisted via `PlayerPrefs` (not a save file). GrammarWorld awards (`AwardBuildersMedal`, `AwardVerbMasterMedal`, `AwardPathfinderMedal`) roll up into the `SentenceBuilder` badge; VocabWorld+ awards (`AwardDailyLifeScoutMedal`, `AwardNumberCruncherMedal`, `AwardGlobalCitizenMedal`) roll up into the `LexiconLegend` badge. Each world's completion check pattern (`CheckSentenceBuilderBadge` / `CheckLexiconLegendBadge`) is: all sub-medals earned + final badge not yet earned → unblock the final badge and unlock a blocker GameObject.
- `PassportManager` (`Assets/Scripts/PassportManager.cs`) is a separate scene-singleton that renders the medal/badge state from `GameProgressManager` into a UI panel (colored sprite vs. grayed-out sprite per medal), toggled via a passport icon button.
- Each mini-game zone follows the same shape: a `*Manager` (game/quiz state machine, often itself a singleton via `Instance`), a `*UIManager` (panel/question display), a `*TutorialManager` (first-time instructions), and `*Trigger` MonoBehaviours on colliders that call into the manager. See `GrammarWorld/Volcan/` for the fullest example (`VolcanoQuizManager`, `VolcanoUIManager`, `VolcanoTutorialManager`, `VolcanoTrigger`, `VolcanoRock`, `LavaEffect`, `LavaBlocker`).
- Quiz content (sentences, verb forms, answers) is embedded directly in the manager scripts as `string[,]` banks (see `VolcanoQuizManager.presentSimple` / `.pastSimple`), not loaded from external data files.
- Scripts are not organized under C# namespaces — folder layout under `Assets/Scripts/` is the only grouping; class names must stay globally unique.

## Build / project setup

- Unity Editor version: **2021.3.44f1** (see `ProjectSettings/ProjectVersion.txt`) — open the project with this exact version.
- Render pipeline: Universal RP (URP) 12.1.15.
- There is no CLI build/test script in this repo; builds are produced from the Unity Editor (File > Build Settings, WebGL target for the current migration work) or via Unity's own automated build pipeline if the user has one configured outside this repo.
- `com.unity.test-framework` is a package dependency but no `*Tests` assembly/asmdef exists yet under `Assets/` — there are currently no automated tests to run.
- Two `.sln`/`.slnx` pairs exist (`EnglishLearningGame` and `EnglishLearningGame2`), both referencing the same two projects (`Assembly-CSharp.csproj`, `NewAssembly.csproj`); these `.csproj`/`.sln` files are regenerated by Unity/the IDE integration and are gitignored — don't hand-edit them.
