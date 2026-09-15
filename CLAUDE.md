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
- **Trigger conversion**: Spatial's `SpatialTriggerEvent` component (used in the Editor to fire UnityEvents like `BlockTrigger.Activar()`, `MazeTrigger.OnPlayerEnter()`, `VolcanoTrigger.Activar()`, `IslandCentralManager.OnPlayerEnter()`, `RespawnTrigger.Teletransportar()`) is gone from the project too. The replacement is `Assets/Scripts/Player/PlayerTriggerZone.cs` — drop it on a `BoxCollider` (`Is Trigger` on) and wire its `onPlayerEnter`/`onPlayerExit` UnityEvents to the same methods those Spatial components used to call. **GrammarWorld's rewiring is done (see the 2026-09-15 session log below); VocabWorld+ and Lobby still have `PlayerTriggerZone` components sitting on their trigger GameObjects with empty `onPlayerEnter`/`onPlayerExit` — same conversion work, not started yet.**
- `Assets/Spatial/` holds Spatial-specific config/data assets (`ProjectConfig.asset`, `SpaceTemplate_*.asset`, `Space_*.asset`, avatar/prefab definitions). `Assets/Spatial/Generated*` and `Assets/Spatial/Temp*` are gitignored build artifacts of the Spatial toolkit, not hand-authored.
- `Assets/Examples/` (Space_FeatureDemo, Space_GolfCourse_Driving, Space_HyperJump_VisualScripting, Avatar_Spatian, etc.) are Spatial SDK sample/template assets, not game content.
- A separate, untracked sibling folder `EnglishLearningGame_WebGL/` (its own git repo, not part of this repository) exists alongside this project — treat it as a working copy for the WebGL port, not something to edit from here unless the user points you at it explicitly.
- `WebGL_Build/` at the repo root is a raw WebGL build output (untracked, not gitignored by the current `.gitignore` since the pattern only matches a directory literally named `Build`/`build`).

Still open: decide what to do with avatar/space assets under `Assets/Spatial` and `Assets/Examples` that have no meaning outside Spatial, and finish the scene-level trigger rewiring for VocabWorld+ and Lobby (GrammarWorld is done — see the session log at the bottom of this file).

## Player system (replaces the Spatial avatar)

`Assets/Scripts/Player/` holds the WebGL-native replacement for Spatial's built-in avatar, added during the migration:

- `PlayerController.cs` — `CharacterController`-based third-person movement, singleton via `Instance` (same pattern as `GameProgressManager`/`PassportManager`). Reads `Input.GetAxis("Horizontal"/"Vertical")` (WASD + arrow keys out of the box via the default Input Manager), moves relative to the assigned camera's flattened forward/right, applies gravity, and rotates the player to face the movement direction. Exposes `SetSpeed`/`ResetSpeed` (used by `BridgeSpeedBoost`) and `Teleport(position, rotation)` (safe respawn/teleport for a `CharacterController` — disables the controller before moving it, matching every other script that used to call Spatial's avatar teleport/position API).
- `ThirdPersonCamera.cs` — follows a `target` transform from behind/above with `SmoothDamp`, orbits via mouse when the cursor is locked (click on the game view to lock, `Esc` to release — required for WebGL's pointer-lock security model), and uses `Physics.Linecast` against `collisionMask` to pull the camera in when terrain/objects would otherwise clip it.
- `PlayerTriggerZone.cs` — generic Spatial-trigger replacement described above.
- The `Player` tag was added to `ProjectSettings/TagManager.asset` (the project previously had zero custom tags) — trigger/respawn scripts check `other.CompareTag("Player")` before acting.
- **Update 2026-09-15**: the `Player` GameObject (`CharacterController` + `CapsuleCollider` + `PlayerController`, tagged `Player`, root-level, fileID `1778491233` in `EnglishWorld.unity`) and its camera rig already exist in the scene — this is no longer pending. `PlayerController.Instance` is populated at runtime. `PlayerController.cs` also gained a raycast-based step-climbing assist (`HandleStepClimbing`, fields `enableStepAssist`/`maxStepHeight`/`stepCheckDistance`) because `CharacterController.stepOffset` is unreliable against non-convex `MeshCollider` staircases (e.g. `Escaleras_GrammarWorld`) — see session log.
- There is no scene-wide `EventSystem` GameObject checked into `EnglishWorld.unity`; `PassportManager.cs` creates one at runtime via `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` (`EnsureEventSystem`) since without it no `Button.onClick` in the scene fires at all. If `PassportManager`'s GameObject is ever removed, move that bootstrap method to whatever script replaces it as the "always loaded" object.

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

## Session log — 2026-09-15: GrammarWorld trigger conversion + fixes

No Unity Editor was available this session (no MCP/Editor bridge) — every fix below was made by directly hand-editing `EnglishWorld.unity` as plain-text YAML (reading `--- !u!114 &<fileID>` MonoBehaviour blocks, cross-referencing script GUIDs against the matching `.cs.meta` file, and writing `UnityEvent` persistent-call blocks by hand) plus normal C# edits. **None of this has been confirmed in Play mode.** Test everything below before trusting it.

### What works now (done this session)

- **PassportManager** (`Assets/Scripts/PassportManager.cs`): the scene had zero `EventSystem` GameObject, so no `Button.onClick` in the whole scene fired — fixed with a `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` bootstrap that creates one if missing. Also `GameProgressManager.LoadProgress()` was moved from `Start()` to `Awake()` (see below) because it fed stale medal state into the passport when opened from the Lobby.
- **GameProgressManager** (`Assets/Scripts/GrammarWorld/GameProgressManager.cs`):
  - `LoadProgress()` moved to `Awake()` — it lives inside `_EspacioGrammarWorld`, which `WorldManager.Start()` deactivates; `Start()` order between the two isn't guaranteed, so `Awake()` (which always runs before any `Start()`) is the only safe place.
  - `CheckSentenceBuilderBadge()` is now also called once from `Awake()` (after `LoadProgress()`). Previously it only ran inside `AwardBuildersMedal()`/`AwardVerbMasterMedal()`/`AwardPathfinderMedal()`, which early-return if the medal was already `true` — so a player who already had all 3 GrammarWorld medals saved in `PlayerPrefs` from an earlier session would never re-trigger the check, and `bloqueadorInsignia` (GameObject `bloqueoInsigniaFinalGrammarWorld`, fileID `1556233324`) would stay blocking forever. **`CheckLexiconLegendBadge()` has the identical bug but was NOT fixed the same way** — `LexiconLegendTrigger.Start()` unconditionally does `insigniaSprite.SetActive(false)`, and since `Awake()` always precedes `Start()`, calling `CheckLexiconLegendBadge()` → `insigniaFinal2.MostrarInsignia()` from `GameProgressManager.Awake()` would get clobbered by that later `Start()` call. Needs its own fix (e.g. move that line out of `LexiconLegendTrigger.Start()`, or gate it) before applying the same pattern there.
- **GrammarWorld/Crater**: full trigger flow wired via `PlayerTriggerZone.onPlayerEnter`/`onPlayerExit` → `BlockTrigger.Activar()` (×10, `Trigger_Bloque_01`..`_10`), `BridgeQuizManager.LlegadaAMeta()` (`TriggerFinPuente`), `RespawnTrigger.Teletransportar()` (×5 crater-boundary walls: `ParedCrater2`-`5`, `ZonaDeRetorno`). `TriggerInicioPuente` calls `CraterTutorialManager.StartTutorial()`.
  - Found and fixed a real bug along the way: the tutorial's interactive demo answers a *practice* question and moves `bridgeBlocks[0]` for show, then `OnPlayClicked()` resets it — but nothing then asked the *real* question for block 0, so `BridgeQuizManager.currentIndex` (0) never advanced and the first real edge trigger (`Trigger_Bloque_01`, index 1) would silently no-op forever. Fixed by adding a `BlockTrigger` (index 0) to `TriggerInicioPuente` and a `CraterTutorialManager.firstBlockTrigger` field that `OnPlayClicked()` fires directly (not via the trigger's `onPlayerEnter`, to avoid it firing simultaneously with `StartTutorial()`). Also added `QuizUIManager.MoverCanvasAPosicion`'s optional second `rotacionTrigger` param and `BlockTrigger.canvasRotationOverride` (pointed at `Trigger_Bloque_01`'s transform) because `TriggerInicioPuente` itself was never oriented to face the player, unlike the real edge triggers.
  - Deleted `Assets/Scripts/GrammarWorld/Crater/EdgeTriggerZone.cs` (dead duplicate of `BlockTrigger.cs` with its own ungated `OnTriggerEnter`, not referenced anywhere in the scene) at the user's request.
- **GrammarWorld/Volcan**: `VolcanoTrigger.Activar()` wired for both platform triggers (`isPresentSimple: 1` and `: 0` instances), `TriggerFinVolcano.Activar()` wired. `LavaBlocker` (×5, one per `ColliderLava`) needed no wiring — it has its own self-contained `OnTriggerEnter` with a tag check, not routed through `PlayerTriggerZone`.
  - No entrance trigger existed anywhere in the `Volcan` hierarchy for `VolcanoTutorialManager.StartTutorial()` (unlike Crater, which already had an empty one). Created a new GameObject `TriggerInicioVolcano` (BoxCollider trigger, 8×6×8 local scale) positioned at `PuntoJugadorVerbVolcano`'s coordinates (next to `SyntaxSprite (Volcano)`, opposite end from `TriggerFinVolcano`) — best-effort placement, unverified in Play mode.
- **GrammarWorld/Laberinto**: entrance trigger `TriggerEntradaLaberinto ` → `MazeManager.StartMaze()` (this alone also handles moving the mascot sprite + playing the first instruction audio — no separate wiring needed for that). The 10 sequential triggers (`Trigger_1`..`_10`, confirmed their `stepIndex` 0-9 matches `MazeManager.triggers[]` array order exactly) → `MazeTrigger.OnPlayerEnter()`. Exit trigger `TriggerSalidaLaberinto ` → `MazeManager.OnMazeCompleted()`.
  - Stairs at the end of the maze (`Escaleras_GrammarWorld`) use a non-convex `MeshCollider` — `CharacterController.stepOffset` is known-unreliable against that collider type, independent of Slope Limit. Fixed two ways: bumped the `Player`'s `CharacterController` `Slope Limit` 45→60 and `Step Offset` 0.3→0.5 in the scene, and added a raycast-based step-climbing assist to `PlayerController.cs` (`HandleStepClimbing`: low/high forward raycasts, nudges the controller up when a low obstruction has clear space above it). Tunable via `enableStepAssist`/`maxStepHeight`/`stepCheckDistance` on the `Player`'s `PlayerController` component.
- **`JumpBoost.cs`** (new, `Assets/Scripts/GrammarWorld/JumpBoost.cs`): wired to `PowerUpSalto`, which had an orphaned missing-script `MonoBehaviour` (guid not matching any asset in the project — the leftover Spatial "climbable" component) that was removed. Only visible/functional when `GameProgressManager` reports all 3 GrammarWorld medals; gives `PlayerController.ApplyJumpBoost()` (new method, coroutine-based, reverts after a fixed duration regardless of whether the player is still touching the trigger) for 5s on pickup. Uses a *second*, larger trigger `BoxCollider` alongside the cube's original solid one (so the player can still stand on the cube after boosting up — didn't want to convert the only collider to a trigger). Pulses via material `_EmissionColor`, not `transform.localScale`, specifically because the object's `Transform` also drives the solid collider the player stands on.
- **GrammarWorld → Lobby return portals**: two independent ones now work.
  - `PortalRegresoLobby` (inside `_EspacioGrammarWorld`) was already wired to `WorldManager.ReturnToLobby()` but positioned ~8000 units away from everything else in GrammarWorld on the Z axis (clearly a stray/mistaken coordinate) — repositioned near `SpawnGrammarWorld`'s coordinates (same parent transform, so directly comparable).
  - A second, previously-unwired `PlayerTriggerZone` was found on a root-level leftover Spatial object literally named **"Avatar Teleporter-Regreso_a_lobby"** (fileID `338768592`), which is the direct parent of both `bloqueoInsigniaFinalGrammarWorld` and `PowerUpSalto` — i.e. it sits exactly where the player ends up after claiming the final badge. Wired its `onPlayerEnter` to `WorldManager.ReturnToLobby()` too.
  - Note: there is a *third*, unrelated object also named `PortalRegresoLobby` living inside `_EspacioVocabWorld` — that's VocabWorld+'s own return portal, already correctly wired, not a duplicate/bug.

### Still pending

- **Nothing above has been playtested.** Priority order for verification: Crater block-0 fix (highest risk of subtle breakage), Laberinto stairs, Volcano entrance trigger placement, JumpBoost jump height/trigger size, both GrammarWorld portals' exact position relative to floor geometry.
- VocabWorld+ and Lobby trigger conversion (same `PlayerTriggerZone`-with-empty-events pattern) not started.
- `CheckLexiconLegendBadge()` load-order bug (see above) not fixed.
- Deciding what to do with `Assets/Spatial` and `Assets/Examples` leftovers (unchanged from before this session).

### Technique notes for future sessions doing more of this by hand-editing the scene YAML

- Every script's stable GUID lives in its `.meta` file (`guid:` line) — match that against `m_Script: {fileID: 11500000, guid: ..., type: 3}` in a scene `MonoBehaviour` block to identify which component is which; don't rely on `m_Component` list ordering alone (it's consistent per-object-type in this scene but not guaranteed).
- A `PlayerTriggerZone`'s empty state looks like `onPlayerEnter:\n    m_PersistentCalls:\n      m_Calls: []`. To wire it, replace `m_Calls: []` with a list containing one entry per call:
  ```yaml
  m_Calls:
  - m_Target: {fileID: <target component's fileID>}
    m_TargetAssemblyTypeName: <ClassName>, NewAssembly
    m_MethodName: <MethodName>
    m_Mode: 1
    m_Arguments:
      m_ObjectArgument: {fileID: 0}
      m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
      m_IntArgument: 0
      m_FloatArgument: 0
      m_StringArgument:
      m_BoolArgument: 0
    m_CallState: 2
  ```
  (`NewAssembly` is this project's assembly name — confirmed from an existing wired `OnClick` in the scene.) Multiple entries under one `m_Calls:` fire in order.
- Adding a **brand-new** script (not already in the project) needs its `.cs.meta` hand-created too, with a manually-chosen 32-hex-char lowercase `guid` — Unity treats that file as authoritative on next import, so the scene can reference it by that guid immediately without ever having opened the project in the Editor. Always `Grep` the chosen guid against `Assets/` first to confirm it isn't already in use.
- When adding a whole new GameObject by hand (not just editing an existing empty event): pick unused fileIDs (grep `&<number>\b` across the scene file to confirm), add the new component fileIDs to the parent's `m_Children` list *and* the owning GameObject's `m_Component` list, and don't worry about `m_RootOrder` staying perfectly sequential among siblings after an insertion — Unity treats the parent's `m_Children` array order as authoritative and doesn't seem to care about small `m_RootOrder` mismatches in practice, but it's untested here whether that holds across an Editor re-save.
- Before reusing any existing trigger's `transform` for something beyond a boolean "player entered" check (e.g. positioning a UI canvas, as `BlockTrigger`/`QuizUIManager` do), verify that trigger was actually authored with a sensible rotation/position for that purpose — `TriggerInicioPuente` in Crater was not, which caused a visible bug (see above).
