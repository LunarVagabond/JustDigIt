# 🗂 Folder-by-Folder Philosophy (Godot C# Project)

## Autoloads/
Contains global systems and autoloaded scripts (e.g., `GameManager`, `InputHandler`, `AudioManager`). Useful for shared services and singleton logic.

## Levels/
Each level gets its own subfolder. Keeps `.tscn` and `.cs` files grouped together. Great for modularity and jam-focused scope control.

## Entities/
Each character, enemy, or NPC lives in its own folder. Allows you to organize their scenes, scripts, animations, and assets together.

## UI/
UI scenes like menus, HUDs, dialog boxes. Can be expanded to include subfolders like `HUD/`, `Menus/`, `Windows/` as needed.

## Shared/
Reusable components and helpers: generic scenes, utility scripts, shared materials, tween scripts, animations, etc.

## Assets/
Raw imported media assets such as textures, audio files, fonts, sprite sheets, etc. Avoid logic or scene files here.

## Tests/
Optional. Used for test scenes or unit testing frameworks like GUT. Helps isolate experimental or throwaway features.

(Added as reference not concrete information)


