# Anima Spire Project Rules v0.3

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
- StageManager manages current stage
- Stage starts at 1-1 and advances after Enemy defeat
- X-10 stages are marked as Boss stages
- HeaderPanel displays current Stage and Gold
- HeaderPanel shows Boss label when current stage is a boss stage
- Hero and Enemy recover after 1 second
- Same combat repeats
- Spirit has no HP and is not an enemy target
- Current latest completed implementation includes Header Stage / Gold UI

Not implemented:

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

## Currency Rules
Gold:

- Korean name: 골드
- Main use: equipment crafting, equipment draw, equipment upgrade
- Main source: normal combat, enemy defeat, offline reward
- Currently implemented as the first MVP currency

Nature Essence:

- Korean name: 자연력
- Main use: spirit summon, spirit growth
- Main source: boss reward, spirit sanctuary, spirit missions, spirit refund
- Not implemented yet

Spiritstone Shard:

- Korean name: 정령석 파편
- Main use: spirit stone refining, spirit stone synthesis, spirit stone upgrade
- Main source: spirit stone dungeon, special enemies, boss reward, spirit stone dismantle, spirit stone altar
- Not implemented yet

Spire Stone:

- Korean name: 첨탑석
- Main use: village building upgrade
- Main source: first boss clear, area breakthrough reward, village expedition, long-term missions
- Not implemented yet

## Currency Unlock Order
1. Gold only
2. Gold is used for equipment crafting and basic growth
3. Nature Essence is added when spirit system is unlocked
4. Spiritstone Shard is added when spirit stone system is unlocked
5. Spire Stone is added when village system is unlocked

## Offline Reward Rules
- Offline reward is calculated based on the highest cleared stage, not the highest reached stage.
- A stage that the player entered but failed to clear must not affect offline reward calculation.
- Example:
  - If the player cleared 10-3 but failed at 10-4, offline reward is calculated based on 10-3.
- Offline reward initially grants Gold only.
- After the spirit system is unlocked, offline reward may also grant a small amount of Nature Essence.
- Spiritstone Shard is not included in the basic offline reward.
- Spire Stone is not included in the basic offline reward.
- Spiritstone Shard and Spire Stone should come from their own content sources such as dungeons, boss first clears, village buildings, expeditions, or missions.

## Skill and Mana Rules
- The game does not use Mana.
- Do not add MP, Mana, Energy, or skill cost systems unless explicitly requested in the future.
- Hero has HP and uses one skill.
- Each companion Spirit has no HP and uses one skill.
- Each Enemy has HP and uses one skill.
- Skills are used repeatedly based on cooldown or cast interval.
- Skill use does not consume Mana.
- Ally skills may support Auto ON/OFF in the future.
- If Auto is ON, the skill is used automatically when its cooldown is ready.
- If Auto is OFF, the skill can be manually activated by the player when ready.
- Healing, defense, shield, and buff skills may use conditional auto logic in the future.
  - Example: use heal only when Hero HP is below a defined threshold.

## HP UI Rules
- Hero has HP.
- Enemy has HP.
- Spirit has no HP.
- Spirit must not display an HP bar.
- Hero HP should be displayed in InfoPanel or a dedicated Hero HP area.
- Enemy HP should be displayed above each Enemy unit because multiple enemies may appear in future stages.
- If multiple enemies exist, each Enemy must have its own HP bar.
- Do not implement a single shared Enemy HP display as the long-term structure.

## Ally Skill UI Rules
- Ally skill icons should be displayed near the lower part of the game screen.
- The skill icons should be circular and arranged horizontally.
- The initial MVP may show only Hero Skill and one Spirit Skill.
- Each ally skill icon should eventually show cooldown state.
- Candidate cooldown display styles:
  - circular radial fill like a pie slice
  - vertical fill from bottom to top
  - dark overlay with remaining seconds
  - recommended: radial or dark overlay plus remaining seconds in the center
- Ally skill icons are not only passive indicators.
- They should eventually support Auto ON/OFF toggle and manual activation.

## Damage UI Rules
- Damage numbers should appear above the damaged unit.
- If an Enemy is damaged, the damage number appears above that Enemy.
- If Hero is damaged, the damage number appears above Hero.
- Spirit is not a damage number target because Spirit has no HP and is not attacked.
- Damage number should float upward and disappear after a short time.

Damage Number Style Rules:

- Normal hit:
  - white basic font
  - normal size
- Critical hit:
  - red font
  - slightly larger than normal hit
- Extra hit / double hit:
  - show each hit as a separate damage number
  - do not merge the two hits into one total number
  - two damage numbers should be slightly overlapped vertically
- If only one hit in a double hit is critical, only that hit uses the critical style.
- If both hits are critical, both damage numbers use the critical style.

## Future Combat Expansion Rules
- Current combat is still a simple single Enemy prototype.
- Future combat should support:
  - multiple enemies
  - melee enemies
  - ranged enemies
  - enemy spawn positions
  - Hero and enemies starting from opposite sides
  - movement based on position and range
  - attack range
  - targeting rules
  - projectile movement
  - damage application when projectile reaches target
- Do not implement these systems until explicitly requested.
- Current simple direct damage structure should not be rewritten into a complex projectile or collision system yet.
- However, future code changes should avoid hardcoding that blocks multi-enemy, range, targeting, or projectile expansion.

## Strict Rules
- Do not add systems that were not requested.
- Do not add monetization.
- Do not add ads.
- Do not add PVP.
- Do not add guilds.
- Do not add inventory systems unless explicitly requested.
- Do not add save systems unless explicitly requested.
- Do not add save or offline reward systems until explicitly requested.
- Do not add equipment random options.
- Do not add spirit stones yet.
- Do not add spirit stones, village systems, ads, in-app purchases, PVP, or guilds yet.
- Do not add Mana, MP, Energy, or skill cost systems unless explicitly requested.
- Do not add Spirit HP.
- Do not display Spirit HP bars.
- Do not add multi-enemy, range, projectile, or collision systems until explicitly requested.
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
After `PROJECT_RULES.md` v0.3, Header Stage / Gold UI is complete.

Next feature candidate:

- Hero HP UI display

Then:

- Enemy HP bar above Enemy
- Damage popup UI
- Hero defeat and stage fallback
- Upgrade Attack button

Do not implement equipment, spirit stones, save, ads, in-app purchases, PVP, or guilds yet.
