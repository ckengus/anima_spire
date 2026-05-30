# PROJECT_RULES.md_v09

## Authoring Notes

- This document must be written in English.
- This document is the minimal implementation guardrail file for Codex and ClaudeCode.
- This document is not a game design document.
- This document is not an MVP roadmap.
- This document is not a replacement for the current work instruction.
- Codex and ClaudeCode must use this file as a stable guardrail before implementation or code review.
- Detailed project background, current MVP details, implementation scope, allowed files, forbidden files, and step-specific requirements must be provided in each work instruction by GPT.
- This file should not be revised every time the design plan or MVP roadmap changes.
- Revise this file only when a core implementation guardrail changes, a repeated critical mistake must be prevented, or the user explicitly requests revision.
- Keep this file concise and operational.

---

## 1. Purpose

This document defines the minimum implementation rules for the Anima Spire Unity project.

Codex and ClaudeCode must treat this file as the top-level project-specific implementation guardrail.

This file exists to prevent unsafe implementation behavior, broad unrequested changes, SaveData mistakes, Unity scene mistakes, Android build mistakes, Git mistakes, and accidental expansion of legacy remnants.

This file does not define the full current MVP scope.

The current work instruction defines the exact task scope.

If the current work instruction is more specific than this file, follow the current work instruction as long as it does not violate this file.

If this file and the current work instruction appear to conflict, stop and report the conflict instead of guessing.

---

## 2. How to Use This File

Before implementation or review, Codex and ClaudeCode must check:

1. The current work instruction
2. The current branch
3. The current MVP substep
4. Allowed files
5. Forbidden files
6. Whether scene changes are allowed
7. Whether SaveData changes are allowed
8. Whether Android native file changes are allowed
9. Whether ProjectSettings changes are allowed
10. Whether Git operations are forbidden

This file provides stable guardrails.

The current work instruction provides step-specific details.

Do not implement outside the current work instruction.

Do not pre-implement future systems.

Do not broaden the task because it seems useful.

If judgment is required beyond the instruction, stop and report.

---

## 3. Project Identity Guardrails

Anima Spire is a Unity-based mobile portrait 2D automatic combat RPG.

The current direction is a magic research and magic-scroll customization RPG.

The player character is a mage.

Combat direction is hero versus enemies.

Magic scrolls are the future skill and loadout system.

Hero equipment provides stat growth.

Do not turn the project back into a spirit-collection RPG unless the user explicitly changes the project direction.

Do not introduce new spirit collection, spirit growth, spirit affection, spirit headquarters, or spirit-based skill systems.

Do not implement project direction changes unless the current work instruction explicitly assigns them.

---

## 4. Legacy Remnants Guardrails

Legacy names and code remnants may still exist in the project.

Known legacy or transitional names may include:

1. Spirit
2. SpiritUnit
3. MagicBook
4. MagicBook Slot
5. Belt
6. HeaderPanel
7. BottomMenuPanel
8. UI_Canvas

Treat these as legacy or transitional remnants unless the current work instruction explicitly says otherwise.

Do not expand legacy remnants into new systems.

Do not broadly rename or delete legacy remnants unless the current work instruction explicitly requests cleanup or migration.

Do not use deprecated names in new user-facing UI, new project rules, or new work instructions.

If legacy code must be touched to complete the current task, make the smallest safe change and report exactly what was touched.

If a legacy name is connected to SaveData or existing runtime behavior, do not rename or remove it without explicit migration instructions.

---

## 5. Scene and Unity Guardrails

The development scene standard is MainPrototype.

Do not modify SampleScene.

Do not open, edit, save, or intentionally modify SampleScene unless the user explicitly requests it.

Scene changes are allowed only when the current work instruction explicitly permits them.

Do not modify ProjectSettings unless the current work instruction explicitly permits it.

After Unity work, check whether unintended scene or settings changes occurred.

Pay special attention to the following files and folders:

1. Assets/DefaultVolumeProfile.asset
2. Assets/Settings/UniversalRP.asset
3. Assets/UniversalRenderPipelineGlobalSettings.asset
4. ProjectSettings/ProjectSettings.asset
5. ProjectSettings/SceneTemplateSettings.json
6. .utmp/
7. Assets/Resources/
8. Assets/Resources.meta

If these files or folders changed unintentionally, report them and do not include them as intended work.

---

## 6. UI Guardrails

The current UI Canvas standard is UI_OverlayCanvas.

UI_Canvas is legacy and must not be revived.

Do not create a new global Canvas for ordinary UI unless the current work instruction explicitly requests and justifies it.

SafeAreaUIRoot is the standard root for clickable UI.

Clickable UI must stay inside Safe Area.

Background Surface may extend behind the Android A-Area.

The A-Area means the upper system icon, status bar, camera-hole, notch, or cutout area.

Do not place buttons, cards, icons, or touch targets inside the A-Area.

Device Simulator is not the official validation standard for A-Area, Safe Area, camera-hole, system icon, or Android system bar behavior.

Unity Editor Play Mode may be used for functional checks, hierarchy checks, tab transition checks, popup checks, and Console Error checks.

Android real-device testing is required for final judgment on A-Area, system icon, status bar, camera-hole, notch, and bottom system gesture behavior.

Do not treat Android A-Area behavior as complete unless the user reports real-device test results.

---

## 7. UI Surface and Placeholder Guardrails

Temporary MVP UI Surface is not final visual design.

Temporary Surface may use simple opaque single-color backgrounds and clear borders.

Do not use transparent-only placeholder Surface when the purpose is structure verification.

Do not place floating skill icons without a background Surface.

Do not make unimplemented features look fully functional.

Use placeholder states clearly.

Do not implement final color palette, final font system, final icon images, or polished art unless explicitly assigned.

Icon-style UI should use fixed icon sizes.

Device adaptation should use spacing, margins, alignment, row or column layout, and wrapping rather than arbitrary per-device icon scaling.

---

## 8. SaveData Guardrails

Do not change SaveData unless the current work instruction explicitly allows it.

Do not change dataVersion unless explicitly allowed.

Do not add temporary UI state to SaveData.

Do not save runtime combat state.

Do not save:

1. Projectile position
2. Projectile target
3. Projectile timer
4. currentStageToken
5. Runtime position
6. Attack timers
7. Temporary combat state
8. Visual effect state
9. UI hierarchy state
10. Canvas migration state
11. Popup runtime state
12. Modal runtime state
13. Full Page runtime state
14. Android A-Area validation state
15. Placeholder UI state

If SaveData changes seem necessary, stop and report before implementing.

---

## 9. Equipment and Magic Guardrails

The current user-facing equipment slot term is Weapon Slot.

Do not use the term MagicBook Slot in new UI, new rules, or new work instructions.

Do not add EquipmentSaveData.level.

Do not add equipment-level growth.

Do not implement equipment self-leveling.

Equipment growth must not be confused with magic scroll growth.

Magic scrolls are not equipment.

Do not add magic scroll slots to the equipment UI.

Do not mix the equipment system and the magic scroll system unless the current work instruction explicitly requests a reviewed integration.

Do not implement full equipment redesign, full eight-slot loadout, equipment options, batch synthesis, or tier promotion unless explicitly assigned.

Existing MagicBook-related code may still be connected to current SaveData, equipment loadout, UI, or combat bonus paths.

Do not rename, delete, or broadly refactor MagicBook-related code unless the current work instruction explicitly requests a migration or cleanup.

If new user-facing UI or new documentation is needed, use the current naming standard rather than deprecated MagicBook Slot terminology.

Belt may still exist as a legacy enum or code remnant.

Do not expand Belt into a new active equipment slot unless explicitly assigned.

---

## 10. Combat Guardrails

Do not modify combat logic unless the current work instruction explicitly allows it.

Do not modify damage calculation unless explicitly allowed.

Do not modify projectile logic unless explicitly allowed.

Do not modify stage progression unless explicitly allowed.

Do not modify Gold reward logic unless explicitly allowed.

Do not modify Hero HP or Enemy HP logic unless explicitly allowed.

Do not modify combat end resolution unless explicitly allowed.

Do not implement CombatManager pause or resume unless explicitly allowed.

Do not introduce Physics2D-based combat unless explicitly requested and reviewed.

Forbidden unless explicitly approved:

1. Physics2D
2. Rigidbody2D
3. Collider2D
4. OnTriggerEnter2D
5. FixedUpdate-based combat hit detection
6. Large StatManager
7. Large StatResolver
8. ModifierSystem
9. DamageCalculator
10. Broad generic Skill Engine

Use the existing lightweight combat structure unless the work instruction says otherwise.

---

## 11. Android Guardrails

Codex and ClaudeCode must not build Android APK or AAB.

The user performs Android Build And Run.

Do not modify Android native files unless the current work instruction explicitly allows it.

Android native files may include:

1. Assets/Plugins/Android/AndroidManifest.xml
2. Assets/Plugins/Android/res/values/styles.xml
3. Assets/Plugins/Android/res/values/colors.xml
4. Assets/Plugins/Android/mainTemplate.gradle
5. Assets/Plugins/Android/launcherTemplate.gradle
6. Assets/Plugins/Android/baseProjectTemplate.gradle
7. ProjectSettings/ProjectSettings.asset
8. Generated Gradle project files

Do not hide Android system icons for general gameplay or ordinary UI.

Do not use windowIsTranslucent=true unless explicitly approved after review.

Do not modify HUD placement logic to compensate for Android A-Area unless explicitly assigned.

Do not re-add Screen.safeArea top inset to an already Safe Area-based HUD layout.

Do not add Android-specific hardcoded HUD inset values unless explicitly approved.

If Android Theme, Manifest, styles.xml, Gradle, or ProjectSettings changes seem necessary beyond the current instruction, stop and report.

---

## 12. Git Guardrails

Codex and ClaudeCode must not perform Git write operations.

Do not run:

1. git add
2. git commit
3. git push
4. git merge
5. git branch -d
6. git branch -D
7. git push origin --delete
8. local branch deletion
9. remote branch deletion

Codex and ClaudeCode may inspect Git status if the current work instruction asks for it.

The user performs commit, push, merge, and branch cleanup.

Completion reports must state that no Git write operation was performed.

---

## 13. Implementation Behavior Rules

Implement only the assigned scope.

Do not refactor unrelated code.

Do not create broad architecture unless explicitly assigned.

Do not create unnecessary generic managers.

Do not create unnecessary singleton systems.

Do not create new scene loading frameworks unless explicitly assigned.

Do not create Addressables or Resources-based UI loading unless explicitly assigned.

Do not mix UI display, SaveData, combat calculation, and equipment calculation responsibilities.

Do not solve future MVP problems inside the current task.

Do not assume that a placeholder should become a real system.

Do not make unimplemented features look complete.

Do not delete old code just because it looks unused unless the current work instruction explicitly requests cleanup.

If a requested change requires broader modification than expected, stop and report.

If an instruction is ambiguous, stop and report.

If there is a conflict between files, scene hierarchy, SaveData, Android behavior, or the work instruction, stop and report.

---

## 14. Required Completion Report

After work, Codex or ClaudeCode must provide a completion report.

The report must include:

1. Work summary
2. Modified files
3. File-by-file change summary
4. Files intentionally not modified
5. SaveData change status
6. Scene change status
7. ProjectSettings change status
8. Android native file change status
9. Unity auto-modified file status
10. Test results
11. Console Error status
12. Remaining issues
13. Confirmation that no Git write operation was performed
14. Confirmation that no Android APK or AAB build was performed

If the task was review-only, the report must state that no files were modified.

If unexpected files changed, list them clearly.

If testing was not possible, state what was not tested and why.

---

## 15. Final Forbidden List

The following are forbidden unless the current work instruction explicitly allows them.

1. Modifying SampleScene
2. Changing the standard development scene away from MainPrototype
3. Reviving UI_Canvas
4. Creating a new global Canvas for ordinary UI
5. Placing clickable UI in Android A-Area
6. Treating Device Simulator as official Android A-Area validation
7. Changing SaveData
8. Changing dataVersion
9. Adding EquipmentSaveData.level
10. Adding equipment self-leveling
11. Using MagicBook Slot as a new user-facing term
12. Changing Weapon Slot naming without approval
13. Adding magic scroll slots to the equipment UI
14. Mixing magic scrolls into the equipment system without explicit instruction
15. Reintroducing legacy concepts as new active systems
16. Broadly deleting legacy remnants without explicit cleanup instruction
17. Broadly renaming MagicBook-related SaveData or runtime code without explicit migration instruction
18. Expanding Belt into a new active equipment slot without explicit instruction
19. Changing combat damage logic without explicit instruction
20. Changing projectile logic without explicit instruction
21. Switching combat to Physics2D without explicit approval
22. Adding large manager systems without explicit instruction
23. Modifying Android native files without explicit instruction
24. Building Android APK or AAB
25. Running git add
26. Running git commit
27. Running git push
28. Running git merge
29. Deleting local or remote branches
30. Including unintended Unity auto-modified files as intended changes
31. Implementing future systems inside placeholder work
32. Expanding the task beyond the current work instruction

---

## 16. Final Summary

PROJECT_RULES.md_v09 is a stable implementation guardrail file.

It is intentionally not a full project design document.

It is intentionally not an MVP roadmap.

It is intentionally not a step-specific work instruction.

Codex and ClaudeCode must follow the current work instruction for detailed scope, but must always stay inside the guardrails in this file.

Legacy code and names may still exist in the project.

Legacy remnants must not be expanded, renamed, deleted, or migrated unless the current work instruction explicitly says so.

When in doubt, stop and report instead of guessing.