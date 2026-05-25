# Anima Spire Project Rules v0.4

## Project Overview
Anima Spire is a 2D idle RPG prototype made in Unity 6000.4.8f1.

The current goal is not to build the full game.
The current goal is to build the smallest playable combat and growth loop.

Anima Spire's core fun is not infinite repetition by itself.
The core loop is that enemies grow stronger, the player gets blocked, and the player breaks through again through growth, setup, and strategy.
All level design and growth systems should support making this loop sustainable.

Core MVP loop:

- Enemy gets stronger.
- Player eventually gets blocked.
- Player farms previous cleared or repeatable Stage.
- Player grows through equipment and other growth systems.
- Player breaks through the blocked Stage.
- Player reaches a higher wall.
- The loop repeats.

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
- Stage advances when Enemy dies.
- Enemy grows stronger as Stage increases.
- Hero eventually fails against stronger Enemy.
- Failed Stage falls back to a previous Stage when allowed.
- Hero and Enemy recover after success or failure.
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
- Spirit has no HP
- Spirit is not an enemy attack target
- Gold reward when Enemy is defeated
- StageManager manages current stage
- Stage starts at 1-1
- Enemy defeat advances Stage
- X-10 stages are marked as Boss stages
- HeaderPanel displays current Stage and Gold
- HeaderPanel shows Boss label when current stage is a boss stage
- Hero HP text is displayed in InfoPanel
- Hero overhead HP bar
- Enemy overhead HP bar
- Damage Popup UI
- StageDifficultyCalculator
- Enemy maxHp and attackPower scale by current Stage
- Hero defeat causes Stage failure
- Stage failure retreats to previous Stage when `currentStage > 1`
- X-1 failure does not retreat to the previous Area
- Stage failure does not give Gold
- Stage failure does not advance Stage
- After failure, Hero and Enemy recover and combat restarts
- Current MVP loop supports enemy scaling, player blocking, failed Stage fallback, and farming previous Stage

Not implemented:

- Battle / Equipment tab switching
- Equipment system
- MagicBook summon
- MagicBook equip
- Equipment collection state
- Equipment synthesis
- Equipment disassembly
- Equipment slot upgrade
- Equipment fixed options
- Equipment random options
- Equipment detail page
- Spirit menu
- Spirit stones
- Save system
- Offline rewards
- Ads
- In-app purchases
- PVP
- Guilds
- Clan
- Co-op
- Networking

## Currency Rules
Gold:

- Main use: equipment summon and future equipment slot upgrade.
- Main source: normal combat and enemy defeat.
- Currently implemented as the first MVP currency.
- Gold is not currently used for a direct Hero attack upgrade button.
- Gold is not currently used for save system, offline reward, ads, or monetization.

Nature Essence:

- Main use: spirit summon and spirit growth.
- Main source: boss reward, spirit sanctuary, spirit missions, and spirit refund.
- Not implemented yet.

Spiritstone Shard:

- Main use: spirit stone refining, spirit stone synthesis, and spirit stone upgrade.
- Main source: spirit stone dungeon, special enemies, boss reward, spirit stone dismantle, and spirit stone altar.
- Not implemented yet.

Spire Stone:

- Main use: village building upgrade.
- Main source: first boss clear, area breakthrough reward, village expedition, and long-term missions.
- Not implemented yet.

## Currency Unlock Order
1. Gold only
2. Gold is used for equipment summon and future equipment slot upgrade
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
- Offline rewards are not implemented in the MVP.

## Equipment Direction
Equipment is the next MVP growth direction.
The previous direct Gold-to-Hero-attack Upgrade Attack button is no longer the next development priority.
Direct Hero attack upgrade is deprecated for now or moved to a lower priority.

Equipment uses a collection-style equipment structure.
Equipment is not an infinite inventory of duplicated individual option items.
Instead, each equipment detail page should eventually manage owned quantities by tier and accumulated option state.

Example:

A MagicBook

- T0 owned count
- T1 owned count
- T2 owned count
- equipped or not
- future fixed option best values
- future random option bank
- future active random option

## Equipment Types
All 8 equipment types should use one shared equipment system.
Only MagicBook is implemented in the first equipment MVP.
The other 7 equipment types are future content.

Attack equipment:

- MagicBook
- Necklace
- Ring
- Belt

Defense / HP equipment:

- Hat
- Robe
- Gloves
- Shoes

## Equipment Tier Rules
- Equipment has tier.
- Equipment does not have individual item level.
- Same equipment and same tier can be collected in quantity.
- Future synthesis uses same equipment and same tier quantity to create a higher tier.
- MVP does not implement synthesis yet.

## Equipment Slot Upgrade Rules
- Equipment slot upgrade is a future system.
- Slot upgrade uses Gold.
- Slot upgrade provides fixed, predictable growth.
- Slot upgrade cost increases as level rises.
- Gold can be spent either on equipment summon or equipment slot upgrade in the future.
- MVP does not implement slot upgrade yet.

## Equipment Option Direction
Each equipment may have fixed options and random options in the future.
However, random options are not stored per individual duplicated item.
Instead, each equipment detail page manages accumulated unlocked options.

Fixed option concept:

- Fixed options represent the identity of the equipment.
- Example: A MagicBook may have bonus attack or Firebolt-related fixed effect.
- Future design may apply the best fixed option value ever obtained for that equipment.
- MVP does not implement fixed option best value yet.

Random option concept:

- When summoning equipment, random option candidates may appear in the future.
- Player can save a limited number of desired options into that equipment's option bank.
- Player can activate a limited number of saved options.
- Example:
  - A MagicBook random option bank:
    - Attack Speed +24%
    - Critical Rate +7%
    - Critical Damage +35%
  - Active option: Critical Rate +7%
- If a better option appears later, the player can replace one saved option.
- MVP does not implement random option bank, active option, option reroll, option replacement, or equipment detail page yet.

## MagicBook MVP Scope
For the first equipment MVP:

- Only MagicBook category is implemented.
- A MagicBook and B MagicBook exist.
- Summon result is A or B with 50% / 50% probability.
- Only T0 appears.
- Player can own A/B MagicBook counts.
- Player can equip A or B MagicBook.
- Equipped MagicBook affects Hero attack power or skill damage.
- Verify blocked Stage can be broken through after equipment growth.

Not included in the first MagicBook MVP:

- Synthesis
- Disassembly
- Slot upgrade
- Random options
- Fixed option best value
- Equipment detail page
- Full 8 equipment type UI
- Save/load

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
- MVP does not implement skill UI, auto/semi-auto/manual skill modes, or action algorithms yet.

## HP UI Rules
- Hero has HP.
- Enemy has HP.
- Spirit has no HP.
- Spirit must not display an HP bar.
- Hero HP is displayed in InfoPanel.
- Hero HP is displayed with an overhead HP bar.
- Enemy HP is displayed above the Enemy unit.
- Enemy HP should be displayed above each Enemy unit because multiple enemies may appear in future stages.
- If multiple enemies exist in the future, each Enemy must have its own HP bar.
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
- Ally skill UI is not implemented in the current MVP.

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
- Current MVP uses simple direct damage with Hero/Spirit on the left and Enemy on the right.
- Future combat may consider:
  - top-view 2D combat
  - enemies appearing from different directions
  - automatic movement
  - action algorithms
  - targeting priorities
  - auto / semi-auto / manual skill usage
  - multiple enemies
  - melee enemies
  - ranged enemies
  - enemy spawn positions
  - Hero and enemies starting from opposite sides
  - movement based on position and range
  - attack range
  - projectile movement
  - damage application when projectile reaches target
- Anima Spire is not a direct movement-control action RPG.
- Long-term movement should be automatic based on configured action algorithms or strategy presets.
- Do not implement top-view combat, movement, range, projectiles, collisions, or multiple enemies in the current MVP.
- Current simple direct damage structure should not be rewritten into a complex projectile or collision system yet.
- However, future code changes should avoid hardcoding that blocks multi-enemy, range, targeting, movement, or projectile expansion.

## Story Direction
Anima Spire is not a heavy long-form story RPG.
It should aim for light narrative and a strong world atmosphere.

- MVP does not implement cutscenes.
- MVP does not implement long dialogue.
- MVP does not implement complex scenario systems.
- Long-term atmosphere can come from area names, boss names, spirit descriptions, first-clear messages, and memory fragments.

## PVP / Guild Long-Term Direction
PVP, guild, clan, and cooperative content are long-term candidates only.
MVP must not implement them.

Long-term content candidates:

- personal PVE stage climbing
- personal challenge content
- cooperative PVE
- guild / clan
- guild PVE
- personal PVP
- guild or clan PVP

PVP Arena long-term candidate:

1. Growth-reflecting arena
   - uses the player's actual growth
   - seasonal matching by similar combat power
2. Fair draft arena
   - fixed tier / normalized equipment, spirits, and spiritstones
   - ban/pick system
   - strategy-focused
   - not MVP

## Strict Rules
- Do not add systems that were not requested.
- Do not add monetization.
- Do not add ads.
- Do not add in-app purchases.
- Do not add PVP.
- Do not add guilds.
- Do not add clan.
- Do not add co-op.
- Do not add networking or server features.
- Do not add inventory systems unless explicitly requested.
- Do not add save/load systems unless explicitly requested.
- Do not add save or offline reward systems until explicitly requested.
- Do not add equipment synthesis yet.
- Do not add equipment disassembly yet.
- Do not add equipment slot upgrade yet.
- Do not add equipment random options yet.
- Do not add equipment detail page yet.
- Do not add spirit stones yet.
- Do not add spirit stones or village systems yet.
- Do not add Mana, MP, Energy, or skill cost systems unless explicitly requested.
- Do not add Spirit HP.
- Do not display Spirit HP bars.
- Do not make Spirit an enemy attack target.
- Do not add multi-enemy, range, movement, projectile, or collision systems until explicitly requested.
- Do not add complex status effects yet.
- Do not modify `SampleScene`.
- Do not introduce external packages unless explicitly requested.
- Do not make large `ProjectSettings` changes unless explicitly requested.

## Coding Rules
- Keep scripts small and readable.
- Prefer simple MonoBehaviour scripts for the first prototype.
- Use placeholder sprites or simple Unity objects first.
- Do not optimize prematurely.
- Keep changes scoped to the requested task.
- MVP features must be small.
- However, code should avoid hardcoding that blocks future expansion.
- Keep responsibilities separated.
- Equipment data, inventory/collection state, UI, and Hero stat calculation should not be tightly coupled.
- For equipment MVP, do not implement all future systems at once.
- Build equipment MVP in small steps:
  1. Battle / Equipment tab
  2. MagicBook data
  3. MagicBook summon
  4. MagicBook owned count display
  5. MagicBook equip
  6. Hero stat reflection
- After each change, explain:
  1. what files were created,
  2. what files were modified,
  3. how to test the feature,
  4. what remains incomplete.

## Test Rules
- Test in `MainPrototype` unless the task explicitly names another scene.
- For Unity Console tests, turn off Collapse before testing.
- Clear the Console before pressing Play.
- If Collapse is enabled, repeated logs may be grouped instead of appearing in time order.
- After feature testing, confirm there are no red Console errors.
- Confirm there are no new yellow Console warnings.
- When testing combat loop changes, confirm Stage fail fallback, Stage scaling, HP bars, Damage Popup, and Stage / Gold UI are not broken.
- When testing Equipment MVP, confirm:
  - Gold is spent.
  - A/B MagicBook count increases.
  - Equipped MagicBook changes Hero attack output.
  - Blocked Stage can become easier after equipment growth.

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
After `PROJECT_RULES.md` v0.4, the next feature candidates are:

- Battle / Equipment tab basic switching
- MagicBook summon and equip MVP

Then:

- MagicBook owned count display
- MagicBook equip and Hero stat reflection
- Equipment collection structure
- Equipment synthesis later
- Equipment slot upgrade later
- Equipment options later
- Save system later

Do not implement direct Gold-to-Hero-attack Upgrade Attack button as the next priority.
Do not implement equipment synthesis, disassembly, slot upgrade, random options, save/load, offline rewards, ads, in-app purchases, PVP, guild, clan, co-op, networking, or full 8 equipment type UI in the first equipment MVP.
