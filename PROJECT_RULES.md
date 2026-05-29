# PROJECT_RULES.md v0.7

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

Codex must not treat this file as a design suggestion. It is a working rule document.

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

The project has completed the initial minimum MVP, the coordinate-based combat minimum MVP, and the 031 Hero Equipment and Growth System MVP.

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
10. Coordinate-based enemy movement
11. Hero and enemy range checks
12. Hero Projectile Pool
13. Projectile damage on arrival
14. StageToken-based ghost hit prevention
15. Stabilized combat end resolution
16. Equipment category and catalog data foundation
17. Equipment catalog grid display
18. Equipment catalog category filtering
19. Global bottom tab foundation
20. Laboratory shell
21. Wardrobe entry from the laboratory
22. Synthesis room shell
23. Minimal equipment synthesis using Gold
24. Equipment synthesis Save and Load validation
25. Weapon Slot Upgrade
26. Weapon Slot Upgrade Save and Load validation
27. Equipment detail popup shell
28. Weapon equipment equip and unequip MVP
29. Equipped-state minimal UX polishing
30. Equipped weapon attack bonus combat connection validation
31. Android device testing for the completed 031 equipment MVP

The coordinate-based combat minimum MVP was completed through step 030F.

The 031 Hero Equipment and Growth System MVP is complete.

The current next major MVP is 032: UI Canvas Consolidation and Combat UI Migration MVP.

---

## 5. Current Unity Scene Standard

The current development scene is MainPrototype.

Do not modify SampleScene.

Codex must not open, edit, save, or intentionally modify SampleScene unless the user explicitly asks for it.

Any scene work must be based on MainPrototype.

After Unity work, Codex must check whether unintended scene changes occurred.

For the current 032 MVP only, MainPrototype scene changes may be allowed when the specific 032 substep explicitly permits scene hierarchy changes.

Codex must not treat general scene editing as allowed outside the assigned 032 substep.

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
8. Bottom global tab touch areas must not conflict with combat UI or popup UI.
9. Safe Area handling must be preserved.
10. Camera-hole, top system bar, and bottom system bar risks must be considered.

Codex must not change Android system UI behavior, Android build settings, or ProjectSettings unless the current user task explicitly requests that change.

---

## 7. MVP Numbering Standard

MVP numbers use three digits.

Substeps inside one MVP use letters A, B, C, and so on.

Example:

031: Hero Equipment and Growth System MVP

031A: Build the 8-slot hero equipment UI skeleton

032: UI Canvas Consolidation and Combat UI Migration MVP

032A: Map the UI Canvas hierarchy and migration design

The same numbering must be used in branch names, work instructions, completion reports, and commit messages.

Codex must not work beyond the specified MVP substep unless the current user task explicitly allows it.

---

## 8. Current Next MVP: 032 UI Canvas Consolidation and Combat UI Migration MVP

The next work package is 032 UI Canvas Consolidation and Combat UI Migration MVP.

The purpose of 032 is to remove the temporary dual-Canvas UI structure and consolidate the combat UI into the existing UI_OverlayCanvas structure without breaking combat, equipment, laboratory, synthesis, Save and Load, or Android portrait behavior.

Current scene UI problem:

1. UI_Canvas still contains the legacy combat UI.
2. UI_OverlayCanvas contains the newer UI shell.
3. Combat UI and non-combat UI are currently split across two Canvas objects.
4. MainTabController currently handles objects across both Canvas structures.
5. BottomMenuPanel and BottomGlobalTabArea represent an old/new bottom-area split.
6. This dual Canvas structure is a temporary structural debt from earlier MVPs.
7. New UI-heavy MVPs should not be built on top of this unstable split.

032 is not a new content MVP.

032 must not implement new magic scroll features, new equipment features, new combat effects, new tutorials, new store systems, new society systems, or new SaveData.

032 is a UI structure stabilization MVP.

---

## 9. 032 Step Plan

032 is divided into the following steps.

1. 032A: Map the UI Canvas hierarchy and migration design
2. 032B: Create an empty CombatArea skeleton under UI_OverlayCanvas
3. 032C: Migrate CombatPanel and InfoPanel into CombatArea and minimally adapt EnsureThreeAreaLayout
4. 032D: Unify the bottom tab standard around BottomGlobalTabArea
5. 032E: Minimally adjust MainTabController and disable UI_Canvas for validation
6. 032F: Remove UI_Canvas and perform final Android regression testing

Each 032 substep should be handled in a separate branch unless the user explicitly decides otherwise.

Each substep must remain independently testable.

Codex must not merge 032C into main if the combat screen is knowingly broken.

Codex must stop and report if a substep requires broader refactoring than the assigned scope.

---

## 10. 032A Working Standard

The purpose of 032A is investigation and mapping.

032A should not implement the migration.

032A included scope:

1. Inspect the current MainPrototype UI hierarchy.
2. Identify the actual children of UI_Canvas.
3. Identify the actual children of UI_OverlayCanvas.
4. Identify the actual children of SafeAreaUIRoot.
5. Identify whether SafeAreaRoot and SafeAreaUIRoot naming is mixed.
6. Identify MainTabController scene references and name-based lookup targets.
7. Identify objects created at runtime by MainTabController.
8. Identify UI objects that must be migrated before UI_Canvas can be removed.
9. Identify UI objects that can be deleted with UI_Canvas.
10. Identify HeaderPanel status and whether it is actually used.
11. Identify BottomMenuPanel status and whether it is still required.
12. Identify risks related to Raycast Target, Canvas sorting, sibling order, and Android Safe Area.

032A excluded scope:

1. Do not move UI objects.
2. Do not create CombatArea yet.
3. Do not modify MainPrototype unless explicitly instructed.
4. Do not modify MainTabController unless explicitly instructed.
5. Do not modify SaveData.
6. Do not modify combat logic.
7. Do not modify equipment logic.
8. Do not modify ProjectSettings.
9. Do not modify SampleScene.

032A completion report must clearly distinguish between scene objects and runtime-generated objects.

Runtime-generated objects such as TopCombatHud, StatusHudCard, and DebugResetProgressButton may not exist in the scene file before Play Mode. Codex must not assume that a missing scene object means the UI is unused.

The user should cross-check the 032A hierarchy map inside Unity Editor before proceeding to 032B.

---

## 11. 032B Working Standard

The purpose of 032B is to create an empty CombatArea under the UI_OverlayCanvas structure.

032B included scope:

1. Create CombatArea under UI_OverlayCanvas > SafeAreaUIRoot.
2. Set CombatArea as the intended parent for future combat UI migration.
3. Use a full-stretch RectTransform layout appropriate for the current SafeAreaUIRoot.
4. Place CombatArea in a sibling order that does not cover PopupOverlay or BottomGlobalTabArea.
5. Avoid unnecessary Image components, unnecessary Raycast Target, and unnecessary input blocking.
6. Do not move CombatPanel or InfoPanel yet.
7. Preserve all existing combat, equipment, laboratory, and synthesis flows.

032B may modify:

1. Assets/AnimaSpire/Scenes/MainPrototype.unity

032B should avoid modifying:

1. Assets/AnimaSpire/Scripts/UI/MainTabController.cs

032B must not modify:

1. SaveData-related files
2. Equipment-related logic
3. Combat-related logic
4. ProjectSettings
5. SampleScene

032B must pass Play Mode regression before commit.

---

## 12. 032C Working Standard

The purpose of 032C is to migrate CombatPanel and InfoPanel under CombatArea and minimally adapt runtime layout logic so the migrated combat UI still works.

032C included scope:

1. Move CombatPanel under CombatArea.
2. Move InfoPanel under CombatArea.
3. Keep TopCombatHud as a runtime child of CombatPanel.
4. Keep StatusHudCard and DebugResetProgressButton behavior intact.
5. Preserve Stage, Gold, Hero HP, Debug Reset, projectile flow, and combat progression.
6. Minimally adapt MainTabController.EnsureThreeAreaLayout so it works with the new CombatArea parent structure.
7. Ensure the combat screen does not become a known broken intermediate state.
8. Keep the bottom tab visible and usable.
9. Do not remove UI_Canvas yet.
10. Do not remove BottomMenuPanel yet unless the current task explicitly permits it.

032C may modify:

1. Assets/AnimaSpire/Scenes/MainPrototype.unity
2. Assets/AnimaSpire/Scripts/UI/MainTabController.cs

032C MainTabController modifications are limited to:

1. EnsureThreeAreaLayout compatibility with CombatArea.
2. Minimal reference lookup adjustments needed for CombatPanel and InfoPanel after migration.
3. Minimal anchor, offset, and parent-Rect assumptions needed to keep the existing visual layout stable.

032C must not do the following:

1. Do not refactor MainTabController broadly.
2. Do not create a new UIManager.
3. Do not create a new LayoutManager.
4. Do not create a new CanvasManager.
5. Do not split combat Logic and View.
6. Do not modify combat damage logic.
7. Do not modify projectile logic.
8. Do not modify equipment logic.
9. Do not modify SaveData.
10. Do not modify ProjectSettings.
11. Do not modify SampleScene.

If 032C requires modifications beyond EnsureThreeAreaLayout, reference lookup, and minimal layout compatibility, Codex must stop and report.

If 032C becomes too large, it may be split inside the 032 MVP as:

1. 032C-1: Prepare combat panel migration around CombatArea
2. 032C-2: Migrate CombatPanel and InfoPanel and adapt layout compatibility

A split inside 032 does not create a new MVP.

032C should be followed by an Android device check if possible, because Safe Area, screen cropping, and touch regions may behave differently on device.

---

## 13. 032D Working Standard

The purpose of 032D is to unify the bottom navigation standard around BottomGlobalTabArea.

032D included scope:

1. Treat BottomGlobalTabArea as the final bottom global tab standard.
2. Identify and remove or disable remaining BottomMenuPanel dependency.
3. Preserve ShowBattle, ShowWardrobe, ShowLaboratory, and ShowSynthesisRoom flows.
4. Preserve temporary fallback behavior for currently unimplemented tabs.
5. Ensure bottom tab touches work in portrait mode.
6. Ensure PopupOverlay correctly blocks or overlays bottom tab interactions when needed.

032D may modify:

1. Assets/AnimaSpire/Scenes/MainPrototype.unity
2. Assets/AnimaSpire/Scripts/UI/MainTabController.cs

032D must not modify:

1. Equipment logic
2. Combat logic
3. SaveData
4. Stage or Gold logic
5. ProjectSettings
6. SampleScene

032D should include an Android bottom-tab touch check if possible.

032D Android check recommendations:

1. Battle tab touch works.
2. Equipment tab touch works.
3. Laboratory tab touch works.
4. BottomGlobalTabArea does not overlap the Android bottom system bar.
5. Popup overlay and bottom tab raycast behavior is correct.
6. No bottom screen cropping occurs.

---

## 14. 032E Working Standard

The purpose of 032E is to validate the new UI structure with UI_Canvas disabled before deleting UI_Canvas.

032E is not the first layout adaptation step. EnsureThreeAreaLayout compatibility must already be handled in 032C.

032E included scope:

1. Disable UI_Canvas.
2. Validate that combat UI works without active UI_Canvas.
3. Check remaining UI_Canvas references.
4. Check remaining HeaderPanel references.
5. Check remaining BottomMenuPanel references.
6. Check runtime UI duplicate creation.
7. Perform minimal MainTabController cleanup if needed.
8. Prepare for safe UI_Canvas removal in 032F.

032E may modify:

1. Assets/AnimaSpire/Scenes/MainPrototype.unity
2. Assets/AnimaSpire/Scripts/UI/MainTabController.cs

032E must not do the following:

1. Do not refactor MainTabController broadly.
2. Do not create a new UIManager.
3. Do not create a new CanvasManager.
4. Do not create a new LayoutManager.
5. Do not split combat Logic and View.
6. Do not modify combat logic.
7. Do not modify equipment logic.
8. Do not modify SaveData.
9. Do not modify ProjectSettings.
10. Do not modify SampleScene.

If 032E becomes too large, it may be split inside the 032 MVP as:

1. 032E-1: Disable UI_Canvas and verify remaining references
2. 032E-2: Minimally clean MainTabController remaining legacy references

A split inside 032 does not create a new MVP.

---

## 15. 032F Working Standard

The purpose of 032F is to remove UI_Canvas and finalize UI_OverlayCanvas as the single UI Canvas standard.

032F included scope:

1. Remove UI_Canvas from MainPrototype.
2. Confirm CombatArea works as the combat UI parent.
3. Confirm CombatPanel and InfoPanel work under CombatArea.
4. Confirm BottomGlobalTabArea is the final bottom tab standard.
5. Confirm PopupOverlay appears above combat and content UI.
6. Confirm HeaderPanel is removed with UI_Canvas if it is unused.
7. Confirm BottomMenuPanel is removed or no longer required.
8. Perform full Unity Play Mode regression.
9. Perform Android device regression.
10. Report any Unity auto-modified files.

032F may modify:

1. Assets/AnimaSpire/Scenes/MainPrototype.unity
2. Assets/AnimaSpire/Scripts/UI/MainTabController.cs only if minimal remaining legacy-reference cleanup is needed

032F must not modify:

1. SaveData-related files
2. Equipment logic
3. Combat logic
4. Projectile logic
5. Stage or Gold logic
6. ProjectSettings
7. SampleScene

032F is complete only when UI_Canvas is removed and the game still works in Unity Play Mode and Android device testing.

---

## 16. UI Canvas Structure Standard

The final target UI structure after 032 should be:

1. UI_OverlayCanvas
   - SafeAreaUIRoot
      - CombatArea
         - CombatPanel
         - InfoPanel
      - MainContentArea
      - TopBarOverlay
      - BottomGlobalTabArea
      - PopupOverlay

Preferred sibling order under SafeAreaUIRoot:

1. CombatArea
2. MainContentArea
3. TopBarOverlay
4. BottomGlobalTabArea
5. PopupOverlay

Intent:

1. CombatArea is the base combat UI area.
2. MainContentArea is for non-combat main screens such as equipment, laboratory, and synthesis.
3. TopBarOverlay is for top overlay UI when needed.
4. BottomGlobalTabArea is the final global bottom tab standard.
5. PopupOverlay must appear above other UI areas.

Do not put combat UI inside MainContentArea.

Do not put equipment, laboratory, or synthesis main panels inside CombatArea.

Do not create another new global Canvas unless explicitly requested and reviewed.

---

## 17. HeaderPanel and BottomMenuPanel Standard

HeaderPanel and BottomMenuPanel are legacy UI_Canvas-era objects.

HeaderPanel standard:

1. HeaderPanel is not a default migration target.
2. HeaderPanel is a deletion candidate if it is inactive and unused.
3. Codex must check whether HeaderPanel is actively used before deletion.
4. If active use or required reference is found, Codex must report before deleting it.
5. Codex must not create new header systems during 032.

BottomMenuPanel standard:

1. BottomMenuPanel is not the final bottom navigation standard.
2. BottomGlobalTabArea is the final bottom global tab standard.
3. BottomMenuPanel should be removed, disabled, or made unused during the 032 migration.
4. Codex must not leave active duplicate bottom navigation structures after 032F.
5. Codex must not add new bottom tab systems during 032.

---

## 18. Raycast, Sorting, and Sibling Order Standard

During 032, Codex must consider Raycast Target, Canvas sorting, and sibling order.

Rules:

1. PopupOverlay must be able to appear above combat and content UI.
2. BottomGlobalTabArea must remain touchable when no popup is blocking it.
3. PopupOverlay should block touches behind it when a modal popup is open.
4. CombatArea must not block bottom tab touches.
5. Empty structural containers should not have unnecessary Raycast Target components.
6. Transparent Images used only for layout must not accidentally block input.
7. Sibling order must be checked after reparenting UI objects.
8. UI object movement must not create duplicate click targets.
9. ScrollRect and button interactions must not conflict.
10. Android device touch behavior is the final validation standard for touch-area issues.

---

## 19. Hero Equipment Standard

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

The 031 equipment MVP currently supports a Weapon-centered MVP equip and unequip flow.

Weapon equipment can be equipped and unequipped through the equipment detail popup.

The equipped Weapon state is displayed in the equipment catalog and the top Weapon Slot display.

Weapon Slot Upgrade affects the equipped weapon attack bonus through existing runtime calculation.

The current user-facing term is Weapon Slot.

Do not use the term MagicBook Slot in new UI or new rules.

---

## 20. Equipment Growth Standard

Equipment is managed by equipmentId.

Equipment with the same equipmentId is treated as the same equipment.

Do not create and store unlimited individual equipment instances.

The core growth unit of equipment is tier.

Tier is the growth stage of the equipment.

Tier is not equipment level.

Owned quantity is treated like promotion material.

The fixed stat of the highest owned tier is applied or used as the current MVP growth basis.

For MVP implementation, fixed stats should first use a simple absolute-value table.

Main options are not updated by promotion.

Selectable options are not updated by promotion.

Main options and selectable options may only be updated or acquired when obtaining or summoning equipment.

Do not implement main options or selectable options unless the user explicitly assigns that step.

The 031 MVP implemented equipment acquisition, owned state, synthesis, equip and unequip, Weapon Slot Upgrade, Save and Load validation, and combat attack bonus validation.

The 031 MVP did not implement full eight-slot equipment loadout, equipment options, tier promotion, batch synthesis, or equipment-level growth.

---

## 21. Equipment Forbidden Rules

The following are forbidden.

1. Do not add EquipmentSaveData.level.
2. Do not add equipment-level growth.
3. Do not add a magic scroll slot to the equipment UI.
4. Do not use the term MagicBook Slot.
5. Do not add spirit-related equipment slots.
6. Do not implement full eight-equipment-slot loadout unless the user explicitly assigns that step.
7. Do not implement equipment options unless the user explicitly assigns that step.
8. Do not implement tier promotion unless the user explicitly assigns that step.
9. Do not implement batch synthesis unless the user explicitly assigns that step.
10. Do not modify equipment combat stat calculation during 032.
11. Do not modify equipment equip or unequip logic during 032.
12. Do not modify Weapon Slot Upgrade logic during 032.

Existing code or data may still contain MagicBook-related implementation from older MVPs.

Codex must not delete or broadly refactor existing MagicBook-related code unless the user explicitly requests it.

For new user-facing UI and new project rules, do not use the term MagicBook Slot.

The current valid naming standard is Weapon Slot.

---

## 22. Magic Scroll Standard

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

The 032 UI Canvas Consolidation MVP must not implement the magic scroll system.

---

## 23. Combat System Standard

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

032 may move combat UI panels, but must not modify combat logic.

During 032, Codex must not modify damage calculation, projectile movement, projectile arrival checks, stage progression, Gold reward logic, Hero HP logic, Enemy HP logic, or combat end resolution.

---

## 24. Combat Stat Calculation Standard

The current hero basic attack damage calculation already includes the equipped Weapon attack bonus path.

The current MVP combat stat principle is simple additive calculation.

Current concept:

Hero basic attack damage = base hero attack + equipped Weapon attack bonus + Weapon Slot upgrade bonus

This is intentionally simple for the current MVP.

Do not add StatManager, StatResolver, ModifierSystem, or DamageCalculator during 032.

Do not modify bonusSkillDamage behavior during 032.

Do not apply equipment bonuses to enemy damage.

Do not save final combat damage values into SaveData.

Future advanced formulas may be introduced in a separate reviewed MVP, but 032 must not implement them.

---

## 25. Projectile and Physics Standard

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

032 must not change projectile logic.

---

## 26. Laboratory Standard

The former spirit headquarters is removed and replaced by the magic laboratory.

The laboratory unlocks after clearing Stage 1-10.

The laboratory is the hero's home and magic research base.

For First Release, the laboratory should not be a complete complex system.

The first laboratory scope should focus on opening the base, showing the next request, and connecting to equipment and magic menus.

The current implemented laboratory shell connects to wardrobe and synthesis room flows.

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

032 must preserve the laboratory, wardrobe, equipment catalog, equipment synthesis room, and popup flows.

032 must not redesign the laboratory system.

---

## 27. First Release Standard

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
13. Stable single-Canvas UI foundation for combat and non-combat screens

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

032 is treated as a First Release foundation-stabilization MVP because stable UI structure is required before adding more UI-heavy systems.

---

## 28. Early Scenario Standard

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

032 must not implement early scenario or tutorial content.

---

## 29. SaveData Principles

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
9. UI hierarchy migration state
10. Canvas migration state
11. Popup runtime state
12. Tab selection runtime state unless a future step explicitly requires it

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

032 must not change SaveData.

032 must not modify PlayerProgressData.cs.

032 must not modify EquipmentSaveData.cs.

032 must not modify EquipmentLoadoutState.cs.

032 must not change dataVersion.

---

## 30. Debug Reset Standard

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
8. Bottom global tabs display correctly
9. Combat UI displays correctly after reset

For public builds, Debug Reset may be hidden or limited to development builds.

032 must not remove or break Debug Reset.

---

## 31. Unity Auto-Modified File Warning

After Unity work, always check for unintended changes in the following files or folders.

1. Assets/DefaultVolumeProfile.asset
2. Assets/Settings/UniversalRP.asset
3. Assets/UniversalRenderPipelineGlobalSettings.asset
4. ProjectSettings/ProjectSettings.asset
5. ProjectSettings/SceneTemplateSettings.json
6. .utmp/
7. Assets/Resources/
8. Assets/Resources.meta

If these files are changed unintentionally, do not include them in the commit target without user confirmation.

Android builds, Play Mode, or simply opening Unity settings may cause automatic changes.

Codex must not include unintended Unity setting changes in a commit target.

032 may intentionally modify MainPrototype.unity only when the assigned 032 substep allows it.

032 must not modify ProjectSettings.

032 must not modify SampleScene.

---

## 32. Git Operation Standard

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

Branch name examples for 032:

1. feature/ui-canvas-map-032a
2. feature/ui-combat-area-032b
3. feature/ui-combat-panel-migration-032c
4. feature/ui-bottom-tab-unification-032d
5. feature/ui-main-tab-controller-cleanup-032e
6. feature/ui-canvas-removal-032f

When merge commits are needed, commands must include the -m option so that an editor such as Vim does not open.

---

## 33. Codex Working Procedure

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
11. Whether the task is part of 032 UI Canvas migration
12. Whether MainPrototype scene modification is explicitly allowed for the current 032 step
13. Whether MainTabController modification is explicitly allowed for the current 032 step

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
12. Scene hierarchy changes
13. MainTabController changes
14. UI_Canvas status
15. UI_OverlayCanvas status
16. CombatArea status
17. BottomGlobalTabArea status
18. Raycast, sorting, or sibling-order concerns
19. Android testing need or result

Codex must not run git add, commit, push, merge, or branch deletion.

Codex must not build Android APK or AAB.

---

## 34. Code Implementation Principles

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
11. Keep structures light and efficient.
12. Avoid overengineering.
13. Avoid adding manager classes that are not needed in the current MVP.
14. Avoid spreading temporary hardcoded values across multiple files.
15. Keep responsibilities clear between UI, manager, state, combat, and save layers.

During 032, Codex must prefer minimal compatibility adjustments over broad UI architecture redesign.

---

## 35. UI Implementation Principles

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
11. Prevent Raycast Target conflicts.
12. Keep PopupOverlay above main content and combat UI.
13. Keep BottomGlobalTabArea touchable when no modal popup blocks it.
14. Avoid creating duplicate bottom navigation systems.
15. Avoid hiding core combat UI behind non-combat content panels.

Equipment UI layout standard:

Left offensive equipment slots:

Weapon, Necklace, Earring, Ring

Center:

Hero character or placeholder

Right defensive equipment slots:

Hat, Outfit, Gloves, Shoes

032 UI migration standard:

1. UI_OverlayCanvas is the final global UI Canvas target.
2. SafeAreaUIRoot is the primary Safe Area root.
3. CombatArea should contain combat UI panels after migration.
4. MainContentArea should contain non-combat main panels.
5. BottomGlobalTabArea is the final bottom global tab standard.
6. PopupOverlay should be the top popup layer.
7. UI_Canvas is a legacy Canvas to be removed by 032F.

---

## 36. Forbidden APIs and Forbidden Structures

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
11. Large StatManager
12. Large StatResolver
13. ModifierSystem
14. DamageCalculator
15. New UIManager during 032
16. New CanvasManager during 032
17. New LayoutManager during 032
18. New scene loading framework during 032
19. New Addressables or Resources-based UI loading during 032

Use simple MVP-scoped implementation first.

032 must not introduce new broad managers for the canvas migration.

---

## 37. Removed Concepts

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

032 must not remove old spirit-related remnants unless the current task explicitly requires that cleanup.

---

## 38. Pre-Work Checklist

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
11. Whether the current step allows MainPrototype scene changes
12. Whether the current step allows MainTabController changes
13. Whether the current step is investigation-only
14. Whether UI_Canvas, UI_OverlayCanvas, SafeAreaUIRoot, CombatArea, BottomGlobalTabArea, or PopupOverlay are involved
15. Whether Android testing is needed after this step

---

## 39. Post-Work Checklist

After implementing, Codex must check and report:

1. Modified file list
2. Implemented content
3. Not implemented content
4. Console Error status
5. Console Warning status
6. SaveData change status
7. PlayerProgressData.cs change status
8. EquipmentSaveData.cs change status
9. EquipmentLoadoutState.cs change status
10. MainPrototype change status
11. SampleScene change status
12. ProjectSettings change status
13. Unity auto-modified file status
14. .utmp/ status
15. Assets/Resources/ status
16. Existing combat regression status
17. Equipment regression status
18. Laboratory regression status
19. Synthesis room regression status
20. Save and Load regression status
21. Debug Reset regression status
22. Bottom global tab regression status
23. Popup regression status
24. Raycast or touch conflict status
25. Android testing need or result
26. Git status result

Codex must not run git add, commit, push, merge, or branch deletion.

Codex must not build Android APK or AAB.

---

## 40. Current Next Task Summary

The next MVP is 032: UI Canvas Consolidation and Combat UI Migration MVP.

The first step is 032A: Map the UI Canvas hierarchy and migration design.

032A is an investigation and mapping step.

032A must not implement the migration.

032A must not modify SaveData, combat logic, equipment logic, ProjectSettings, or SampleScene.

032A must identify the current UI_Canvas and UI_OverlayCanvas hierarchy, MainTabController references, runtime-created UI, migration targets, deletion candidates, and risks.

032A success conditions:

1. UI_Canvas hierarchy is mapped.
2. UI_OverlayCanvas hierarchy is mapped.
3. SafeAreaUIRoot hierarchy is mapped.
4. MainTabController references are listed.
5. Runtime-generated UI objects are identified.
6. CombatPanel, InfoPanel, HeaderPanel, BottomMenuPanel, BottomGlobalTabArea, and PopupOverlay status is documented.
7. HeaderPanel use status is checked.
8. BottomMenuPanel use status is checked.
9. Raycast, sorting, and sibling-order risks are documented.
10. No unintended code or scene changes are made unless explicitly allowed.
11. SaveData is not changed.
12. Existing combat, equipment, laboratory, synthesis, Save and Load, and Debug Reset are not broken.
13. SampleScene is not modified.
14. ProjectSettings are not modified.
15. No unintended Unity setting file changes remain.

After 032A, the user should cross-check the hierarchy map in Unity Editor before 032B begins.

---

## 41. Final Summary

Anima Spire is now a magic research and magic-scroll customization automatic combat RPG.

The spirit concept is removed from this game.

Combat direction is hero versus enemies.

Magic scrolls provide skills and combat loadout depth.

Equipment provides pure stat growth.

The 031 Hero Equipment and Growth System MVP is complete.

The current next MVP is 032 UI Canvas Consolidation and Combat UI Migration MVP.

032 must consolidate the legacy dual-Canvas UI structure by moving combat UI into the UI_OverlayCanvas structure and eventually removing UI_Canvas.

032 must proceed carefully in small steps.

The first step is 032A: Map the UI Canvas hierarchy and migration design.

Codex must strictly follow this document, stay within the assigned scope, and must not perform unrequested planning changes, SaveData changes, combat logic changes, equipment logic changes, spirit reintroduction, magic-scroll equipment integration, broad UI architecture redesign, Android build operations, or Git write operations.