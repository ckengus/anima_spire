# Anima Spire Project Rules

## Project Overview
Anima Spire is a 2D idle RPG prototype made in Unity.

The current goal is not to build the full game.
The current goal is to build the smallest playable prototype.

## Developer Context
The project owner is a non-programmer using AI coding tools.
All changes must be small, safe, readable, and easy to test.

## Current Prototype Goal
Build the smallest playable loop:

- A hero and one spirit stand on the left side.
- Enemies appear on the right side.
- The hero automatically casts a spellbook skill.
- The spirit automatically uses its base skill.
- Enemies take damage and die.
- Gold increases when enemies die.
- The stage advances from 1-1 to 1-10.
- Stage 1-10 is a boss stage.
- If the hero dies, the game moves back to the previous stage.
- If the hero dies on X-1, the game stays on X-1.
- Gold can be used for a very simple power upgrade.

## Strict Rules
- Do not build the full game at once.
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

## Coding Rules
- Keep scripts small and readable.
- Prefer simple MonoBehaviour scripts for the first prototype.
- Use placeholder sprites or simple Unity objects first.
- Do not optimize prematurely.
- Do not introduce external packages unless explicitly requested.
- After each change, explain:
  1. what files were created,
  2. what files were modified,
  3. how to test the feature,
  4. what remains incomplete.

## Folder Rules
Use this folder structure under Assets/AnimaSpire when possible:

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

## First Development Priority
The first implementation priority is:

1. Create the basic folder structure.
2. Create a MainPrototype scene.
3. Create minimal empty managers:
   - GameManager
   - StageManager
   - CombatManager
4. Confirm the project compiles without errors.

Do not implement combat before the project structure is stable.
