# PROJECT_RULES.md v0.6

Authoring notes for this document:

- This document must be written in English.
- This document is the single self-contained project rule file for Codex.
- Codex must be able to understand the current project direction, implementation rules, forbidden changes, Git rules, Unity rules, and current MVP scope by reading this file only.
- Do not write this document as a summary that depends on Google Drive documents, chat history, external links, or other planning files.
- Do not reference other documents as required reading.
- Do not use phrases such as "see the planning document", "refer to Drive", or "as discussed previously" as a substitute for actual rules.
- If a future GPT rewrites this document, it must preserve the self-contained nature of the file.
- If a future GPT updates this document, it must keep the document in English even if the user conversation is in Korean.
- Keep implementation rules explicit and operational, because Codex will use this file as a working standard.
- Do not remove forbidden items unless the user explicitly decides to change the project direction.

---

## 1. Purpose

This document defines the top-level working rules for the Anima Spire Unity project.

Codex must treat this file as the highest project-specific implementation standard.

Codex may not assume that it has access to Google Drive documents, previous chat history, planning discussions, or external project documents. This file is intentionally self-contained.

Codex must not perform broad redesigns, planning changes, SaveData changes, scene changes, or Git operations unless the current user task explicitly allows them.

---

## 2. Project Overview

Project name: Anima Spire

Engine: Unity

Target format: Mobile portrait 2D game

Primary screen ratio: 9:16 portrait

Current genre direction: automatic combat, collection, growth, and magic-build RPG

Anima Spire is no longer a spirit-collection RPG.

The current concept is a magic research and magic-scroll customization RPG.

The player character is a novice mage who has just graduated from a magic tower. On the way back to the hometown, the mage discovers that the village is under attack, saves the village, opens a personal magic laboratory, receives requests from nearby villages, and grows by researching and using magic scrolls.

The game is based on automatic combat, but the long-term depth comes from magic scroll loadouts, spell order, spell cooldowns, spell rarity, mastery, enchantments, chains, resonance, equipment growth, and challenge content.

---

## 3. Current Core Concept

The current core concept is as follows.

1. The spirit collection and growth concept is removed from this game.
2. The player character is a mage, not a spirit summoner.
3. The main collection and growth target is the magic scroll, not spirits.
4. Combat should ultimately contain only the hero and enemies.
5. The former structure of one hero skill plus five spirit skills is replaced by six magic scroll slots.
6. Hero equipment provides pure stat growth.
7. Magic scrolls are not equipment and must be managed in the magic system, not in the equipment system.
8. The former spirit headquarters concept is replaced by the magic laboratory.
9. Guild or clan concepts should be named and themed as an academic society.
10. Magic effects and impact feedback are the core visual appeal of combat.

Codex must not reintroduce spirit collection, spirit growth, spirit affection, spirit headquarters, spirit probability facilities, spirit character release plans, or spirit-based five-skill structures as active project systems.

---

## 4. Current Implementation Status

The project has completed the initial minimum MVP and the coordinate-based combat minimum MVP.

The following features are currently implemented or considered complete.

1. MainPrototype scene-based combat
2. 9:16 mobile portrait UI foundation
3. Hero and enemy combat
4. Enemy defeat
5. Gold acquisition
6. Stage progression
7. Hero defeat handling
8. Save and Load
9. Debug Reset
10. Minimal equipment summon and equip flow
11. Weapon Slot Upgrade
12. Hero attack power increase
13. Coordinate-based enemy movement
14. Hero and enemy range checks
15. Hero Projectile Pool
16. Projectile damage on arrival
17. StageToken-based ghost hit prevention
18. Stabilized combat end resolution
19. Android device testing completed

The coordinate-based combat minimum MVP was completed through step 030F.

The next major MVP is 031: Hero Equipment and Growth System Minimum MVP.

---

## 5. Current Unity Scene Standard

The current development scene is MainPrototype.

Do not modify SampleScene.

Codex must not open, edit, save, or intentionally modify SampleScene unless the user explicitly asks for it.

Any scene work must be based on MainPrototype.

After Unity work, Codex must check whether unintended scene changes occurred.

---

## 6. Screen and Platform Standard

Anima Spire is a mobile portrait game.

UI must be designed for a 9:16 portrait screen.

Android device testing is an important validation standard.

When working on UI, Codex must consider the following.

1. Text must be readable on a mobile portrait screen.
2. Buttons must be large enough for finger touch input.
3. Text must not be too small.
4. Information density must not be excessive.
5. Layout must not break in portrait mode.
6. The Android device screen must not crop or overlap important UI.
7. Touch targets must not conflict with each other.

---

## 7. MVP Numbering Standard

MVP numbers use three digits.

Substeps inside one MVP use letters A, B, C, and so on.

Example:

031: Hero Equipment and Growth System Minimum MVP
031A: Build the 8-slot hero equipment UI skeleton
031B: Connect existing equipment data to the UI
031C: Organize equipment detail information and owned quantity display

The same numbering must be used in branch names, work instructions, completion reports, and commit messages.

Codex must not work beyond the specified MVP substep unless the current user task explicitly allows it.

---

## 8. Current Next MVP: 031 Hero Equipment and Growth System Minimum MVP

The next work package is 031 Hero Equipment and Growth System Minimum MVP.

The purpose of 031 is to redefine hero equipment as a pure stat growth system after the concept transition.

031 is divided into the following steps.

1. 031A: Build the 8-slot hero equipment UI skeleton
2. 031B: Connect existing equipment data to the UI
3. 031C: Organize equipment detail information and owned quantity display
4. 031D: Review the equipmentId, tier, and count structure and prepare data transition
5. 031E: Implement the first version of tier-based fixed stat application
6. 031F: Implement minimal equipment promotion or synthesis
7. 031G: Stabilize equipment growth Save and Load and perform Android testing
8. 031H: Write the completion reflection and prepare the next phase

031A must not implement SaveData changes, synthesis, promotion, options, magic systems, or spirit-related UI.

---

## 9. 031A Working Standard

The goal of 031A is to build the visual skeleton of the new hero equipment UI.

031A included scope:

1. Create the equipment tab or equipment panel UI skeleton
2. Place a hero character placeholder in the center
3. Place four offensive equipment slots on the left
4. Place four defensive equipment slots on the right
5. Create eight equipment slot buttons
6. Display equipment slot names
7. Display placeholder equipment icons
8. Display a selected equipment summary card placeholder
9. Add a detail button placeholder
10. Add change or unequip button placeholders
11. Do not include any magic scroll slot in the equipment screen

031A excluded scope:

1. SaveData changes
2. EquipmentSaveData.cs changes
3. PlayerProgressData.cs changes
4. Equipment synthesis
5. Equipment promotion
6. Equipment options
7. Equipment summon probability correction
8. Magic scroll UI
9. Magic system implementation
10. Spirit-related UI
11. Long Press implementation
12. Real equipment data changes
13. Hero stat calculation changes

031A is a UI skeleton step. Codex must not change the data structure during 031A.

---

## 10. Hero Equipment Standard

Hero equipment is a pure stat growth system.

Magic scrolls are not hero equipment.

Do not add a magic scroll slot to the equipment screen.

Hero equipment consists of eight equipment types.

Four offensive equipment types:

1. Weapon
2. Necklace
3. Earring
4. Ring

Four defensive equipment types:

1. Hat
2. Outfit
3. Gloves
4. Shoes

The previously discussed Belt slot is removed from the eight equipment types.

The equipment system must not be mixed with the magic system.

Equipment provides pure stat growth such as attack, HP, defense, and similar base stats.

---

## 11. Equipment Growth Standard

Equipment is managed by equipmentId.

Equipment with the same equipmentId is treated as the same equipment.

Do not create and store unlimited individual equipment instances.

The core growth unit of equipment is tier.

Tier is the growth stage of the equipment.

Tier is not equipment level.

Owned quantity is treated like promotion material.

The fixed stat of the highest owned tier is applied.

Example:

Wooden Staff T0 owned quantity 5: apply T0 fixed stat
Wooden Staff T1 owned quantity 1: apply T1 fixed stat
Wooden Staff T0 owned quantity 20 and T2 owned quantity 1: apply T2 fixed stat

For MVP implementation, fixed stats should first use a simple absolute-value table.

Main options are not updated by promotion.

Selectable options are not updated by promotion.

Main options and selectable options may only be updated or acquired when obtaining or summoning equipment.

Do not implement main options or selectable options in early MVP steps unless the user explicitly assigns that step.

---

## 12. Equipment Forbidden Rules

The following are forbidden.

1. Do not add EquipmentSaveData.level.
2. Do not add equipment-level growth.
3. Do not add a magic scroll slot to the equipment UI.
4. Do not use the term MagicBook Slot.
5. Do not add spirit-related equipment slots.
6. Do not change SaveData in 031A.
7. Do not implement synthesis, promotion, or options in 031A.
8. Do not change Hero stat calculation in 031A.
9. Do not change real equipment data structure in 031A.

Existing code or data may still contain MagicBook-related implementation from older MVPs.

Codex must not delete or broadly refactor existing MagicBook-related code unless the user explicitly requests it.

For new user-facing UI and new project rules, do not use the term MagicBook Slot.

The current valid naming standard is Weapon Slot.

---

## 13. Magic Scroll Standard

Magic scrolls are the core collection, growth, and customization system of Anima Spire.

Magic scrolls are not equipment.

Magic scrolls are managed in a separate magic tab or magic system.

The magic scroll system follows these standards.

1. The final magic slot count is six.
2. Stage 1 starts with one magic slot.
3. Stage 2 unlocks the second magic slot.
4. Later stage progression gradually unlocks up to six slots.
5. The initial spell is one 1-star Fire Arrow.
6. The early game uses single casting only.
7. Double casting and triple casting are later growth systems.
8. Magic has star grades from 1-star to 6-star.
9. Native 1-star magic can be promoted up to 6-star.
10. A native 1-star magic promoted to 6-star should aim for roughly 80 to 90 percent efficiency of a native 6-star magic, depending on role and context.
11. Magic gains mastery through use.
12. One prefix enchantment and one suffix enchantment can be applied to one magic scroll.
13. Chain design starts with 1-chain and 2-chain.
14. 3-chain is a later expansion candidate.
15. Magic collection passive bonuses and magic resonance are long-term systems.

The 031 Hero Equipment MVP must not implement the magic scroll system.

---

## 14. Combat System Standard

The combat system is redefined around the hero and enemies.

Final combat should not contain spirits.

Current code may still contain SpiritUnit or other spirit-related structures from previous MVPs. Codex must not immediately delete them unless the current task explicitly says so.

Spirit removal must be handled in a separate safe step.

The completed coordinate-based combat MVP structure must be preserved.

Combat principles:

1. The hero fights enemies.
2. Enemies move toward the hero.
3. Range checks are distance-based.
4. Hero Projectile is managed by a pool.
5. Projectiles move toward their target after firing.
6. Projectiles apply damage on arrival.
7. Projectile damage is snapshotted at fire time.
8. currentStageToken prevents ghost hits.
9. Active projectiles are returned when combat resolution begins.
10. Gold is granted when an enemy dies.
11. Stage advances only when the enemy dies and the hero survives.
12. Hero death results in retreat or failure handling.
13. If the enemy and hero die at the same time, enemy Gold is granted but the Stage does not advance.

---

## 15. Projectile and Physics Standard

The current combat system does not use physics-engine collision detection.

Projectile movement and arrival checks are vector-based.

The following are forbidden unless the user explicitly approves a new design.

1. Physics2D
2. Rigidbody2D
3. Collider2D
4. OnTriggerEnter2D
5. FixedUpdate-based combat hit detection

The Hero Projectile structure may later be expanded into a magic Projectile structure.

Do not introduce physics-based combat unless the user specifically requests it and the design is reviewed first.

---

## 16. Laboratory Standard

The former spirit headquarters is removed and replaced by the magic laboratory.

The laboratory unlocks after clearing Stage 1-10.

The laboratory is the hero's home and magic research base.

For First Release, the laboratory should not be a complete complex system.

The first laboratory scope should focus on opening the base, showing the next request, and connecting to equipment and magic menus.

Long-term laboratory feature candidates include:

1. Request board
2. Equipment research desk
3. Magic research desk
4. Magic archive or research records
5. Training room or DPS test station
6. Special dungeon entry
7. Raid entry
8. Offline reward device
9. Laboratory decoration
10. Society communication device

The laboratory is not a spirit headquarters.

Do not add spirit-related facilities to the laboratory.

---

## 17. First Release Standard

First Release is not a fully complete live-service game.

First Release is the first externally playable candidate build.

The core First Release goal is to verify the magic research automatic combat RPG loop:

The mage progresses through stages, earns Gold and rewards, grows equipment and magic scrolls, and challenges higher stages.

First Release inclusion candidates:

1. Opening and early scenario
2. Normal stage combat
3. Hero and enemy combat structure
4. Basic magic scroll system
5. Gradual magic slot unlock
6. Single-casting automatic spell use
7. Three to five basic spells
8. Eight hero equipment slots and basic equipment growth
9. Gold acquisition and spending
10. First laboratory unlock
11. Save, Load, and Reset
12. Android build and device testing

First Release exclusions:

1. Spirit collection
2. Spirit growth
3. Spirit affection
4. Spirit headquarters
5. Actual magic enchantment implementation
6. Actual magic chain implementation
7. Double casting
8. Triple casting
9. Magic collection passive bonuses
10. Magic resonance
11. Raid
12. PvP
13. Society system
14. Actual season pass operation
15. Real-money purchase integration

---

## 18. Early Scenario Standard

The hero is a novice mage who has just graduated from a magic tower.

The hero is returning to the hometown and discovers that the village is being attacked by beasts or enemies.

The hero enters battle to save the villagers.

Stage 1-1 begins.

The initial owned magic is one 1-star Fire Arrow.

Stages 1-1 through 1-10 are centered around Fire Arrow.

Around Stage 1-3 or 1-4, when combat starts to feel slightly difficult, a villager gives the hero a wooden staff and the equipment tutorial begins.

After clearing Stage 1-10, the village chief thanks the hero and gives a reward.

The hero then enters the house and opens a magic laboratory.

News of the rescued hometown spreads to nearby villages, and another village sends a request, leading to Stage 2-1.

---

## 19. SaveData Principles

SaveData changes must be handled carefully.

Do not change SaveData unless the current step explicitly allows it.

Runtime combat state must not be saved.

Do not save:

1. Projectile position
2. Projectile target
3. Projectile timer
4. currentStageToken
5. Runtime position
6. Attack timers
7. Temporary combat state
8. Visual effect state

Save persistent progression and growth state only.

Examples of save candidates:

1. Gold
2. Stage progress
3. Equipment owned state, equipped state, and growth state
4. Magic owned state
5. Magic equipped slots
6. Magic slot unlock state
7. Laboratory unlock state
8. Tutorial progress state

When changing SaveData, review the following.

1. Whether dataVersion must change
2. Whether migration is required
3. Impact on existing Android installed data
4. Debug Reset behavior
5. UI update after Load
6. Combat runtime restart after Load

031A must not change SaveData.

---

## 20. Debug Reset Standard

Debug Reset must be preserved during development.

Debug Reset deletes save data and returns the game to the initial state for testing.

After Debug Reset, verify the following.

1. Gold resets
2. Stage resets
3. Equipment state resets
4. Magic state resets
5. Laboratory returns to locked state
6. Combat runtime restarts correctly
7. UI displays correctly

For public builds, Debug Reset may be hidden or limited to development builds.

---

## 21. Unity Auto-Modified File Warning

After Unity work, always check for unintended changes in the following files or folders.

1. Assets/DefaultVolumeProfile.asset
2. Assets/Settings/UniversalRP.asset
3. Assets/UniversalRenderPipelineGlobalSettings.asset
4. ProjectSettings/ProjectSettings.asset
5. .utmp/

If these files are changed unintentionally, do not include them in the commit target without user confirmation.

Android builds, Play Mode, or simply opening Unity settings may cause automatic changes.

Codex must not include unintended Unity setting changes in a commit target.

---

## 22. Git Operation Standard

Codex must not perform Git write operations.

Codex must not run:

1. git add
2. git commit
3. git push
4. git merge
5. git branch -d
6. git push origin --delete
7. local branch deletion
8. remote branch deletion

Codex may suggest commands for the user to run, but Codex must not run them.

The user creates the working branch.

Branch name examples:

feature/hero-equipment-031a
feature/hero-equipment-031b
feature/hero-equipment-031c

When merge commits are needed, commands must include the -m option so that an editor such as Vim does not open.

---

## 23. Codex Working Procedure

Before working, Codex must check:

1. Current step number
2. Included scope
3. Excluded scope
4. Files allowed to modify
5. Files forbidden to modify
6. Whether SaveData changes are allowed
7. Whether scene changes are allowed
8. Test requirements
9. Forbidden APIs
10. Git operation restrictions

Codex must not refactor outside the assigned scope.

Codex must not make broad architecture changes unless explicitly requested.

Codex must not pre-implement systems just because they may be useful later.

Codex must write a completion report after work.

The completion report must include:

1. Work summary
2. Modified files
3. Implemented content
4. Not implemented content
5. Save and Load impact
6. Test results
7. Unity Console results
8. Git status
9. Unintended changed files
10. Remaining issues
11. Next-step cautions

---

## 24. Code Implementation Principles

Code implementation principles:

1. Implement small MVP-scoped changes.
2. Do not create excessive generic systems.
3. Do not refactor unrelated code.
4. Do not break existing behavior.
5. Minimize SaveData changes.
6. Do not mix UI skeleton work and data structure changes in the same step unless explicitly instructed.
7. Use current naming standards.
8. Do not expose deprecated concepts in new UI.
9. Keep debug functions available for development.
10. Consider Android device testing.

---

## 25. UI Implementation Principles

UI implementation principles:

1. Use a mobile portrait 9:16 standard.
2. Avoid tiny text.
3. Use touchable button sizes.
4. Avoid overlap with bottom tabs and main panels.
5. Make placeholder states clear.
6. Do not make unimplemented features look functional.
7. Do not implement Long Press in early MVPs.
8. Prefer basic OnClick events.
9. Prevent conflicts between scroll and button events.
10. Do not display magic scroll slots in the equipment UI.

031A equipment UI layout standard:

Left offensive equipment slots:
Weapon, Necklace, Earring, Ring

Center:
Hero character or placeholder

Right defensive equipment slots:
Hat, Outfit, Gloves, Shoes

---

## 26. Forbidden APIs and Forbidden Structures

Unless explicitly requested and reviewed, do not use or introduce:

1. Physics2D
2. Rigidbody2D
3. Collider2D
4. OnTriggerEnter2D
5. FixedUpdate-based combat hit detection
6. Large generic ObjectPoolManager
7. Unnecessary generic Pool Manager
8. Unnecessary Singleton Manager
9. Broad generic Skill Engine
10. Large SaveData restructuring

Use simple MVP-scoped implementation first.

---

## 27. Removed Concepts

The following concepts are removed from the current game direction.

1. Spirit collection
2. Spirit growth
3. Spirit affection
4. Spirit headquarters
5. Spirit dispatch
6. Spirit probability correction
7. Spirit character illustration collection
8. Spirit-based five-skill structure
9. Mixed mage plus spirit-summoner concept

Old code or scene objects may still contain related remnants.

Do not introduce them into new UI or new systems.

Deletion must be handled in a separate safe step.

---

## 28. Pre-Work Checklist

Before implementing, Codex must check:

1. Whether the current branch is the intended work branch
2. Whether the task targets MainPrototype
3. Whether SampleScene is untouched
4. Whether SaveData changes are needed
5. Whether the current step allows SaveData changes
6. Which files must not be modified
7. Current naming standards
8. Whether new spirit-related content is being introduced
9. Whether magic scrolls are being added to equipment UI
10. Whether EquipmentSaveData.level is being added

---

## 29. Post-Work Checklist

After implementing, Codex must check and report:

1. Modified file list
2. Implemented content
3. Not implemented content
4. Console Error status
5. Console Warning status
6. SaveData change status
7. PlayerProgressData.cs change status
8. EquipmentSaveData.cs change status
9. MainPrototype change status
10. SampleScene change status
11. Unity auto-modified file status
12. .utmp/ status
13. Existing combat regression status
14. Save and Load regression status
15. Debug Reset regression status
16. Whether Android testing is needed
17. Git status result

Codex must not run git add, commit, push, merge, or branch deletion.

---

## 30. Current Next Task Summary

The next task is 031A: Build the 8-slot hero equipment UI skeleton.

031A must build UI skeleton only.

031A must not implement data structure changes or actual growth logic.

031A success conditions:

1. The equipment UI shows eight slots.
2. Four offensive equipment slots are on the left.
3. Four defensive equipment slots are on the right.
4. A hero placeholder is in the center.
5. Selecting an equipment slot displays a summary card placeholder.
6. No magic scroll slot appears in the equipment screen.
7. SaveData is not changed.
8. Existing combat, Save and Load, and Debug Reset are not broken.
9. SampleScene is not modified.
10. No unintended Unity setting file changes remain.

---

## 31. Final Summary

Anima Spire is now a magic research and magic-scroll customization automatic combat RPG.

The spirit concept is removed from this game.

Combat direction is hero versus enemies.

Magic scrolls provide skills and combat loadout depth.

Equipment provides pure stat growth.

The current next MVP is 031 Hero Equipment and Growth System Minimum MVP.

The first step is 031A: Build the 8-slot hero equipment UI skeleton.

Codex must strictly follow this document, stay within the assigned scope, and must not perform unrequested planning changes, SaveData changes, spirit reintroduction, magic-scroll equipment integration, or Git write operations.