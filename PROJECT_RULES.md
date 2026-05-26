# Anima Spire Project Rules v0.5

Last updated: 2026-05-27

Current baseline: Minimum MVP completed on main branch

## 1. Project Overview

Anima Spire is a 2D portrait mobile idle RPG prototype made in Unity.

The current baseline is not the full game.

The current baseline is the completed minimum MVP that verifies the following loop:

- Automatic combat
- Gold reward
- Stage progression
- MagicBook summon
- MagicBook equip
- Weapon Slot Upgrade
- Hero damage growth
- Save / load
- Debug Reset
- Android device execution

Anima Spire's core fun is not infinite repetition by itself.

The core loop is:

- Enemy gets stronger.
- Player eventually gets blocked.
- Player grows through rewards, equipment, and other growth systems.
- Player breaks through the blocked Stage.
- Player reaches a higher wall.
- The loop repeats.

Do not build the full game at once.

Every implementation must be small, safe, readable, and testable.

## 2. Development Premise

- The project owner is a non-programmer using AI coding tools.
- GPT is used for planning, design, document writing, and PM work.
- Gemini Pro is used to validate design proposals and implementation request consistency.
- Claude is used for counter-review and risk review.
- Codex / Claude Code are used for implementation and debugging.
- Codex must follow this PROJECT_RULES.md and the user-provided task instruction.
- Google Drive documents under the Anima Spire folder are project references.
- For implementation, prioritize this PROJECT_RULES.md, the current task instruction, and the current code state.
- AI review documents are reference records from the decision-making process. They may not be the latest source of truth.
- All work must be split into small, safe, readable, and easy-to-test units.

## 3. Source of Truth

Use the following priority order when references conflict:

1. Current main branch implementation
2. Current user task instruction
3. This PROJECT_RULES.md
4. Latest Google Drive planning / operation / handoff documents
5. AI review documents and old task instructions

AI review documents are not implementation commands.

Use only the user's final decision and current task instruction for implementation.

## 4. Current MVP Baseline

Current minimum MVP loop:

- Hero and Spirit attack Enemy.
- Enemy attacks Hero.
- Spirit has no HP and is not an enemy attack target.
- Enemy takes damage and dies.
- Enemy death grants Gold.
- Enemy death advances Stage.
- Gold is used for MagicBook summon and Weapon Slot Upgrade.
- Equipped MagicBook increases Hero damage.
- Weapon Slot Level increases Hero damage when a MagicBook is equipped.
- Progress is saved and loaded automatically.
- Debug Reset can clear progress for testing.
- Android device execution has been verified.

Do not casually change Stage / Area progression.

Current new Area softlock prevention is handled by enemy difficulty overlap, not by returning to the previous Area.

Do not implement previous Area fallback farming unless explicitly requested.

## 5. Spirit Combat Rules

- Spirit does not have HP.
- SpiritUnit must not contain HP-related fields such as maxHp, currentHp, hp, or health.
- Spirit is not an enemy attack target.
- Spirit does not die during combat.
- Spirit only performs attack or support roles.
- The only defeat condition is Hero HP reaching 0.
- Enemy always attacks Hero, not Spirit.
- Do not display Spirit HP bars.

## 6. Screen Structure

Anima Spire is a portrait mobile game.

Current Battle tab MVP layout:

- Combat area: top 50%
- Info area: middle 40%
- Bottom menu: bottom 10%

Current Equipment tab MVP layout:

- Bottom menu remains at bottom 10%.
- Equipment overlay uses the upper 90%.
- Combat background may remain dimmed behind the overlay.

Current HUD:

- Stage / Gold / DEBUG RESET are displayed as top combat HUD cards.
- The old HeaderPanel-centered four-area layout is not the current active MVP layout.
- Safe Area top inset is minimally considered for HUD placement.

Current assumptions:

- Portrait mobile screen is the primary target.
- Rotation is not supported in the current MVP.
- Android status bar policy is not finalized.
- Landscape, tablet, foldable, and full Safe Area policies are future work.

Hero, Spirit, and Enemy are world objects, not children of the UI Canvas.

## 7. Scene Rules

- The main prototype scene is Assets/AnimaSpire/Scenes/MainPrototype.unity.
- Do not modify SampleScene unless explicitly requested.
- Test in MainPrototype unless the task explicitly names another scene.
- Do not modify scene or prefab references casually.
- Runtime UI may be created or adjusted by scripts.
- Do not make large ProjectSettings changes unless explicitly requested.

## 8. Current Implementation Status

Implemented:

- Unity project base structure
- PROJECT_RULES.md
- MainPrototype scene
- Portrait mobile MVP UI
- Battle / Equipment tab switching
- Automatic combat loop
- Hero and Spirit attack Enemy
- Enemy attacks Hero
- Spirit has no HP
- Spirit is not an enemy attack target
- Gold reward on Enemy defeat
- Stage progression
- Stage starts at 1-1
- StageDifficultyCalculator
- Hero HP text / UI display
- Hero overhead HP bar
- Enemy overhead HP bar
- Damage Popup
- A MagicBook T0
- B MagicBook T0
- MagicBook summon
- MagicBook equip
- Equipment owned count state
- A/B MagicBook attack bonus +3
- Weapon Slot Upgrade
- weaponSlotLevel save/load
- Local JSON save/load
- Dirty-based autosave
- Debug Reset Progress
- Android save/load validation
- Area 2 entry
- New Area first-stage softlock prevention

Not implemented unless explicitly requested:

- Equipment synthesis
- T1+ equipment
- Equipment disassembly
- Equipment fixed options
- Equipment random options
- Equipment detail page
- Full 9 equipment slot UI
- Staff / Wand / Magic Scroll
- Spirit collection / growth system
- Spirit menu
- Spirit stones
- Boss system
- Offline rewards
- Server save
- Account / login
- Ads
- In-app purchases
- PVP
- Guilds
- Clan
- Co-op
- Networking
- Coordinate-based combat
- Multiple enemies
- Active chain

## 9. Currency Rules

Gold:

- Current MVP currency.
- Main source: Enemy defeat.
- Current uses:
  - MagicBook summon
  - Weapon Slot Upgrade
- Gold is not a paid currency.
- Gold is not used for a direct Hero attack upgrade button.
- Do not add a direct Gold-to-Hero attack upgrade button unless explicitly requested.

Future currencies such as Nature Essence, Spiritstone Shard, and Spire Stone are not implemented.

Do not add additional currencies unless explicitly requested.

## 10. Offline Reward Rules

Offline rewards are not implemented.

Do not implement offline rewards unless explicitly requested.

If offline rewards are implemented later, handle them as a separate feature because they affect save/load, time calculation, and balance.

## 11. Equipment Direction

Equipment is already part of the current MVP baseline.

Current MVP equipment:

- A MagicBook T0
- B MagicBook T0
- A/B summon
- A/B equip
- Owned count save/load
- Equipped MagicBook save/load
- Weapon Slot Upgrade

Equipment uses a collection-style equipment structure.

Equipment is not an infinite inventory of duplicated individual option items.

Equipment does not use individual item levels.

Do not add EquipmentSaveData.level.

Current predictable growth is Weapon Slot Upgrade.

Do not add a direct Gold-to-Hero attack upgrade button unless explicitly requested.

## 12. Equipment Types

Final equipment structure target:

- 8 stat equipment slots
- 1 central special equipment slot

Stat equipment slots:

Attack-side:

- Weapon
- Necklace
- Ring
- Belt

Defense / HP-side:

- Hat
- Robe
- Gloves
- Shoes

Central special slot:

- Magic Scroll

Current MVP only implements A/B MagicBook as weapon-type test equipment.

MagicBook is not a separate final equipment slot.

MagicBook belongs under Weapon Slot.

Use Weapon Slot as the current slot name.

Do not create a MagicBook-specific slot name.

## 13. Equipment Tier Rules

- Equipment has tier.
- Current MVP implements only T0 MagicBooks.
- T1+ equipment is not implemented yet.
- Equipment does not have individual item level.
- Same equipment and same tier can be collected in quantity.
- Future synthesis uses same equipment and same tier quantity to create a higher tier.
- MVP does not implement synthesis yet.

## 14. Weapon Slot Upgrade Rules

Weapon Slot Upgrade is implemented in the current MVP.

Current rules:

- Field: weaponSlotLevel
- Default value: 0
- Upgrade cost: 10 * (currentLevel + 1)
- Bonus: +1 attack per Weapon Slot Level
- Bonus applies only when a MagicBook is equipped.
- Saved and loaded through PlayerProgressData.weaponSlotLevel
- Debug Reset resets weaponSlotLevel to 0.

Rules:

- Do not add MagicBook-specific slot logic.
- Do not use obsolete MagicBook-specific slot field names.
- Do not add individual equipment levels.
- UI must not directly modify Gold or slot level.
- Use EquipmentManager domain methods for upgrade behavior.
- Loading saved slot level must not spend Gold.
- Loading saved slot level must not trigger dirty/save/gameplay events.

## 15. Equipment Option Direction

Equipment options are future content.

Do not implement the following unless explicitly requested:

- Fixed options
- Random options
- Option bank
- Option reroll
- Option replacement
- Equipment detail page

## 16. MagicBook MVP Scope

Current MagicBook MVP:

- A MagicBook and B MagicBook exist.
- Summon result is A or B with 50% / 50% probability.
- Only T0 appears.
- A MagicBook T0 attack bonus: +3
- B MagicBook T0 attack bonus: +3
- Player can own A/B MagicBook counts.
- Player can equip A or B MagicBook.
- Equipped MagicBook affects Hero attack damage.
- Owned counts are saved/loaded.
- Equipped MagicBook state is saved/loaded.

Not implemented:

- Synthesis
- Disassembly
- T1+ equipment
- Random options
- Fixed option best value
- Equipment detail page
- Full 9 equipment slot UI

## 17. Stage / Difficulty / Gold Rules

Enemy difficulty index:

enemyDifficultyIndex = ((area - 1) * 8) + stage

Purpose:

- New Area Stage 1 should not be harder than the previous Area Stage 9.
- Example: 1-9 and 2-1 have the same enemy difficulty.
- This prevents new Area first-stage softlock without returning to the previous Area.

Gold reward index:

globalStageIndex = (area - 1) * 10 + stage

Gold reward:

Gold Reward = 10 + (globalStageIndex - 1) * 5

Examples:

- 1-10 = 55 Gold
- 2-1 = 60 Gold

Important:

- Enemy difficulty index and Gold reward index are intentionally different.
- Do not change 2-1 into a harder stage than 1-10 unless explicitly requested.
- Do not make 2-1 reward fall back to 10 Gold.
- Do not change Stage / Area progression casually.
- X-10 is intended as a future Boss Stage position, but the current Boss system is not implemented.

## 18. Save / Load Rules

Current save system:

- PlayerProgressData
- EquipmentSaveData
- dataVersion = 1
- LocalProgressSaveRepository
- Application.persistentDataPath
- player_progress.json
- Dirty-based autosave

Current saved fields:

- dataVersion
- lastClearedArea
- lastClearedStage
- gold
- weaponSlotLevel
- ownedEquipment
- equippedMagicBookKey

Save/load rules:

- Do not change dataVersion unless explicitly instructed.
- Use ForLoad-style methods when applying saved data.
- Loading must not spend Gold.
- Loading must not call MarkDirty.
- Loading must not call ScheduleSaveSoon.
- Loading must not trigger gameplay events.
- UI must not directly edit save data.
- Save events should be handled through ProgressSaveManager.

Current autosave policy:

- Gold change: dirty
- Stage progress: dirty
- MagicBook summon: dirty + SaveSoon
- MagicBook equip: dirty
- Weapon Slot Upgrade: dirty + SaveSoon
- OnApplicationPause(true): save pending dirty data
- OnApplicationQuit: backup save

## 19. Debug Reset Rules

Debug Reset is a development/test feature.

Current reset result:

- Stage returns to 1-1.
- Gold becomes 0.
- ownedEquipment becomes empty.
- equippedMagicBookKey becomes empty.
- weaponSlotLevel becomes 0.
- Save file is deleted.
- Scene reloads.

Rules:

- Reset must prevent ghost save.
- Do not call MarkDirty, ScheduleSaveSoon, SaveNow, or SaveIfDirty inside the destructive reset flow unless explicitly designed.
- After reset and scene reload, autosave should work again normally.
- Debug Reset is not a production account deletion feature.

## 20. Skill and Mana Rules

- The game does not use Mana.
- Do not add MP, Mana, Energy, or skill cost systems unless explicitly requested.
- Hero has HP and uses repeated attacks or skills based on interval/cooldown.
- Each companion Spirit has no HP and uses repeated attacks or support behavior.
- Each Enemy has HP and attacks Hero.
- Skill use does not consume Mana.
- Ally skill UI, active skills, auto/manual skill modes, and active chain are not implemented.
- Do not implement skill icons, active chain, or manual skill modes unless explicitly requested.

## 21. HP UI Rules

- Hero has HP.
- Enemy has HP.
- Spirit has no HP.
- Spirit must not display an HP bar.
- Hero HP is displayed in the Battle UI and with an overhead HP bar.
- Enemy HP is displayed above the Enemy unit.
- If multiple enemies are implemented later, each Enemy should have its own HP bar.
- Do not implement a single shared Enemy HP display as the long-term structure.

## 22. Damage UI Rules

- Damage numbers should appear above the damaged unit.
- If an Enemy is damaged, the damage number appears above that Enemy.
- If Hero is damaged, the damage number appears above Hero.
- Spirit is not a damage number target because Spirit has no HP and is not attacked.
- Damage number should float upward and disappear after a short time.
- Extra hit / double hit may show each hit as a separate damage number in the future.
- Critical hit style is future content unless explicitly requested.

## 23. Future Combat Expansion Rules

Current combat is still a simple single Enemy prototype.

Current MVP uses simple direct damage with Hero/Spirit on the left and Enemy on the right.

Future combat may consider:

- Coordinate-based combat
- Enemies appearing from different directions
- Automatic movement
- Targeting priorities
- Auto / semi-auto / manual skill usage
- Multiple enemies
- Melee enemies
- Ranged enemies
- Enemy spawn positions
- Movement based on position and range
- Attack range
- Projectile movement
- Damage application when projectile reaches target
- Boss patterns

Anima Spire is not a direct movement-control action RPG.

Long-term movement should be automatic based on configured action algorithms or strategy presets.

Do not implement coordinate-based combat, movement, range, projectiles, collisions, or multiple enemies unless explicitly requested.

Current simple direct damage structure should not be rewritten into a complex projectile or collision system without explicit instruction.

Future code changes should avoid hardcoding that blocks multi-enemy, range, targeting, movement, or projectile expansion.

## 24. Story Direction

Anima Spire is not a heavy long-form story RPG.

It should aim for light narrative and a strong world atmosphere.

- MVP does not implement cutscenes.
- MVP does not implement long dialogue.
- MVP does not implement complex scenario systems.
- Long-term atmosphere can come from area names, boss names, spirit descriptions, first-clear messages, and memory fragments.

Do not implement story systems unless explicitly requested.

## 25. PVP / Guild Long-Term Direction

PVP, guild, clan, and cooperative content are long-term candidates only.

Do not implement them unless explicitly requested.

Long-term content candidates:

- Personal PVE stage climbing
- Personal challenge content
- Cooperative PVE
- Guild / clan
- Guild PVE
- Personal PVP
- Guild or clan PVP

## 26. UI / Android Rules

Current MVP UI:

- Battle tab uses Combat 50%, Info 40%, Bottom Menu 10%.
- Equipment tab uses upper 90% overlay and keeps Bottom Menu 10%.
- Stage / Gold / DEBUG RESET are top combat HUD cards.
- Safe Area top inset is minimally considered.

UI rules:

- UI must call domain methods instead of directly changing Gold, equipment count, or slot level.
- Preserve Button onClick and serialized references.
- Prioritize mobile readability and touch area.
- Do not perform a full UI redesign unless explicitly requested.

Android verified:

- App launch
- Combat
- UI touch
- Save/load
- App restart load
- Background return
- Debug Reset

Not finalized:

- Android status bar policy
- Unity splash removal
- Tablet / foldable / landscape layout
- Google Play integration
- Ads
- In-app purchases

## 27. Strict Rules

Do not add systems that were not requested.

Do not implement without explicit user instruction:

- Server save
- Account / login
- Ads
- In-app purchases
- Offline rewards
- Equipment synthesis
- T1+ equipment
- Equipment disassembly
- Equipment fixed options
- Equipment random options
- Equipment detail page
- Full 9 equipment slots
- Staff / Wand / Magic Scroll
- Spirit collection / growth system
- Spirit stones
- Village systems
- Boss system
- Coordinate-based combat
- Multiple enemies
- Range / movement / projectile / collision systems
- Active chain
- Complex status effects
- PVP
- Guilds
- Clan
- Co-op
- Networking
- Google Play integration
- Full UI redesign
- External packages
- Large ProjectSettings changes

Always keep these rules:

- Do not add Mana, MP, Energy, or skill cost systems unless explicitly requested.
- Do not add Spirit HP.
- Do not display Spirit HP bars.
- Do not make Spirit an enemy attack target.
- Do not modify SampleScene.
- Do not use obsolete MagicBook-specific slot names.
- Do not add individual equipment levels.
- Do not add EquipmentSaveData.level.

## 28. Coding Rules

- Keep scripts small and readable.
- Prefer simple MonoBehaviour scripts for the prototype.
- Use placeholder sprites or simple Unity objects first.
- Do not optimize prematurely.
- Keep changes scoped to the requested task.
- MVP features must be small.
- Code should avoid hardcoding that blocks future expansion.
- Keep responsibilities separated.
- UI, domain logic, save/load logic, and Hero stat calculation should not be tightly coupled.
- UI must not directly modify Gold, equipment count, equipped state, or slot level.
- Use domain managers such as EquipmentManager, StageManager, GameManager, and ProgressSaveManager.
- Save/load work must preserve ForLoad separation.
- For equipment work, do not implement all future systems at once.

After each Codex task, explain:

1. What files were created
2. What files were modified
3. What was implemented
4. What values or logic changed
5. What existing behavior was preserved
6. How to test the feature
7. What remains incomplete or untested

## 29. Test Rules

- Test in MainPrototype unless the task explicitly names another scene.
- For Unity Console tests, turn off Collapse before testing.
- Clear the Console before pressing Play.
- If Collapse is enabled, repeated logs may be grouped instead of appearing in time order.
- After feature testing, confirm there are no red Console errors.
- Confirm there are no new yellow Console warnings.
- When testing combat loop changes, confirm Stage scaling, HP bars, Damage Popup, and Stage / Gold UI are not broken.
- When testing equipment changes, confirm:
  - Gold is spent correctly.
  - A/B MagicBook count changes correctly.
  - Equipped MagicBook changes Hero damage output.
  - Weapon Slot Upgrade still works.
  - Save/load still works.
  - Debug Reset still works.
- When testing Stage / Gold changes, confirm:
  - 1-9 and 2-1 enemy difficulty relationship is preserved unless explicitly changed.
  - 2-1 Gold reward does not fall back to 10.
- When testing save-related changes, confirm:
  - App restart load works.
  - Debug Reset works.
  - No ghost save occurs.
- Android device tests are required for UI layout, Safe Area, app lifecycle, save/load lifecycle, and touch interaction changes.

## 30. Git Workflow Rules

Default rule for Codex:

- Do not run git add.
- Do not run git commit.
- Do not run git push.
- Do not run git merge.
- Do not delete branches.

Exception:

- Only perform Git operations if the user explicitly instructs Codex to do so.

User workflow:

- Do not work directly on main for feature work unless explicitly choosing a no-code-change test.
- Start from the latest main.
- Create a work branch for one feature or one document update.
- Make one small feature or document change at a time.
- Test in Unity when code changes are made.
- Commit the change after user verification.
- Push the remote branch.
- Merge into main.
- Push main.
- Delete merged work branches.

Unity may modify unrelated files during testing. Before commit, check for unintended changes such as:

- Assets/DefaultVolumeProfile.asset
- Assets/Settings/UniversalRP.asset
- Assets/UniversalRenderPipelineGlobalSettings.asset
- ProjectSettings/ProjectSettings.asset
- .utmp/

Do not commit unrelated Unity-generated changes unless explicitly intended.

## 31. Folder Rules

Use this folder structure under Assets/AnimaSpire when possible:

- Scenes
- Scripts/Core
- Scripts/Combat
- Scripts/Stage
- Scripts/Units
- Scripts/UI
- Scripts/Data
- Scripts/Data/Equipment
- Scripts/Equipment
- Scripts/Save
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

Keep folder additions minimal and task-driven.

## 32. Project Document Rules

Google Drive under the Anima Spire folder contains project reference documents.

Current document operation principles:

- 01_기획문서: planning and design baseline documents
- 02_운영규칙: operation rules and project rules
- 03_AI작업지시: Codex task instructions and completion reports
- 04_AI검토결과: AI review results and integrated summaries
- 05_버그기록: major bug records
- 06_인수인계: handoff documents

Rules:

- AI review documents are decision-support records and may not be the latest source of truth.
- Codex task instruction documents and completion reports should both be saved from future work onward.
- New project document names should use YYMMDD_순번_문서명 format.
- Old documents do not need to be renamed unless explicitly requested.
- Do not edit Google Drive documents unless explicitly requested.

## 33. Next Development Priority

After the minimum MVP, likely next development candidates are:

1. Equipment synthesis MVP
2. Equipment UI first pass
3. Boss Stage first pass
4. Offline Gold reward MVP
5. Android / build packaging cleanup

Do not implement any of these without an explicit user task instruction.

Do not implement direct Gold-to-Hero-attack Upgrade Attack button as the next priority.

Do not implement server save, account/login, ads, in-app purchases, PVP, guild, clan, co-op, networking, or full 9 equipment type UI unless explicitly requested.

## 34. Codex Completion Report Requirements

After every Codex task, report:

1. Created files
2. Modified files
3. Implementation summary
4. Changed values or logic
5. Preserved existing behavior
6. Unity batchmode result, if run
7. Play Mode test result, if run
8. Android device test result, if run
9. Untested items
10. Git operation status

Git operation status must explicitly state:

- Git add not performed
- Git commit not performed
- Git push not performed
- Git merge not performed
- Branch delete not performed

If the user explicitly instructed Codex to perform Git operations, report the actual Git operations performed.