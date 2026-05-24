# Anima Spire Project Rules v0.2

## Project Overview
Anima Spire is a 2D idle RPG prototype made in Unity 6000.4.8f1.

The current goal is not to build the full game.
The current goal is to build the smallest playable combat and growth loop.

## Development Premise
- The project owner is a non-programmer using AI coding tools.
- GPT is used for planning, design, and PM work.
- Gemini Pro is used to validate design proposals and implementation request consistency.
- Claude is used for counter-review and risk review.
- Codex / Claude Code are used for implementation and debugging.
- All work must be split into small, safe, readable, and easy-to-test units.

## Current Prototype Goal
Build the smallest playable loop:

- Hero and Spirit attack Enemy.
- Enemy attacks Hero.
- Enemy takes damage and dies.
- Gold increases when Enemy dies.
- After a short delay, Hero and Enemy recover.
- The same combat repeats.

Do not build the full game at once.

## Spirit Combat Rules
- Spirit does not have HP.
- `SpiritUnit` must not contain HP-related fields such as `maxHp`, `currentHp`, `hp`, or `health`.
- Spirit is not an enemy attack target.
- Spirit does not die during combat.
- Spirit only performs attack or support roles.
- The only defeat condition is Hero HP reaching 0.
- Enemy always attacks Hero, not Spirit.

## Screen Structure
Anima Spire is a portrait mobile game.

- The whole Game View target is a 9:16 portrait screen.
- The combat window itself is not 9:16.
- The whole screen is divided from top to bottom into four areas:
  - `HeaderPanel`
  - `CombatPanel`
  - `InfoPanel`
  - `BottomMenuPanel`
- Recommended area ratio:
  - `HeaderPanel`: 10%
  - `CombatPanel`: 55%
  - `InfoPanel`: 25%
  - `BottomMenuPanel`: 10%
- Hero, Spirit, and Enemy are world objects, not children of the UI Canvas.
- Combat objects should be placed so they appear inside the `CombatPanel` area.

## Scene Rules
- The main prototype scene is `Assets/AnimaSpire/Scenes/MainPrototype.unity`.
- Do not modify `SampleScene` unless explicitly requested.
- `MainPrototype` currently contains:
  - Main Camera
  - Global Light 2D
  - GameManager
  - StageManager
  - CombatManager
  - UI_Canvas
  - HeaderPanel
  - CombatPanel
  - InfoPanel
  - BottomMenuPanel
  - Hero_Placeholder
  - Spirit_Placeholder
  - Enemy_Placeholder

## Current Implementation Status
Implemented:

- Unity project base structure
- `PROJECT_RULES.md`
- `MainPrototype` scene
- Portrait mobile UI frame
- `HeaderPanel` / `CombatPanel` / `InfoPanel` / `BottomMenuPanel` layout
- Hero / Spirit / Enemy placeholders
- `HeroUnit` / `SpiritUnit` / `EnemyUnit` basic stats
- Basic automatic attack loop
- Hero and Spirit attack Enemy
- Enemy attacks Hero
- Gold reward when Enemy is defeated
- Hero and Enemy recover after 1 second
- Same combat repeats
- Spirit has no HP and is not an enemy target

Not implemented:

- Stage number
- Stage progression from 1-1, 1-2, through 1-10
- Boss stage
- Gold UI display
- Upgrade button
- Equipment system
- Spirit menu
- Spirit stones
- Save system
- Offline rewards
- Ads
- In-app purchases
- PVP
- Guilds

## Strict Rules
- Do not add systems that were not requested.
- Do not add monetization.
- Do not add ads.
- Do not add PVP.
- Do not add guilds.
- Do not add inventory systems unless explicitly requested.
- Do not add save systems unless explicitly requested.
- Do not add equipment random options.
- Do not add spirit stones yet.
- Do not add complex status effects yet.
- Do not add networking or server features.
- Do not introduce external packages unless explicitly requested.
- Do not make large `ProjectSettings` changes unless explicitly requested.

## Coding Rules
- Keep scripts small and readable.
- Prefer simple MonoBehaviour scripts for the first prototype.
- Use placeholder sprites or simple Unity objects first.
- Do not optimize prematurely.
- Keep changes scoped to the requested task.
- After each change, explain:
  1. what files were created,
  2. what files were modified,
  3. how to test the feature,
  4. what remains incomplete.

## Test Rules
- For Unity Console tests, turn off Collapse before testing.
- Clear the Console before pressing Play.
- If Collapse is enabled, repeated logs may be grouped instead of appearing in time order.
- After feature testing, confirm there are no red Console errors.
- Test in `MainPrototype` unless the task explicitly names another scene.

## Git Workflow Rules
- Do not work directly on `main`.
- Start from the latest `main`.
- Create a work branch for one feature or one document update.
- Make one small feature or document change at a time.
- Test in Unity.
- Commit the change.
- Push the remote branch.
- Merge into `main`.
- Push `main`.
- Delete merged work branches.

## Folder Rules
Use this folder structure under `Assets/AnimaSpire` when possible:

- Scenes
- Scripts/Core
- Scripts/Combat
- Scripts/Stage
- Scripts/Units
- Scripts/UI
- Scripts/Data
- Prefabs/Units
- Prefabs/UI
- Prefabs/Effects
- Art/Characters
- Art/Spirits
- Art/Enemies
- Art/UI
- Art/Backgrounds
- ScriptableObjects/Stages
- ScriptableObjects/Enemies
- ScriptableObjects/Skills
- ScriptableObjects/Equipment
- ScriptableObjects/Spirits
- Audio
- Localization

## Next Development Priority
After `PROJECT_RULES.md` v0.2, the next feature is the stage number and progression structure.

The next goal is a minimum stage loop:

- Start at 1-1.
- When Enemy is defeated, advance to 1-2, then 1-3.
- Continue toward 1-10.
- Keep the system minimal and testable.

Do not implement equipment, spirit stones, save, ads, in-app purchases, PVP, or guilds yet.
