# 958225-project

# Project The Elect

**Project The Elect** is a 2D story-driven game developed with **C# and MonoGame**.

The game focuses on exploration, dialogue, cutscenes, and gameplay sequences while using a state-based game structure to manage different parts of the game.

## Features

* 2D Game
* Story-driven gameplay
* Dialogue system
* Cutscene system
* Multiple game states
* Background music and sound effects
* Character dialogue sprites
* JSON-based dialogue data
* Custom UI and buttons
* Asset management through MonoGame Content Pipeline
## Play the game

1. Download the Windows x64 game ZIP from the repository's **Releases** page.
2. Extract the ZIP file. Keep the extracted files together in the same folder.
3. Run `Project The Elect English.exe`.

The release build is self-contained and does not require a separate .NET installation.

## Controls

| Action | Control |
| --- | --- |
| Move | W, A, S, D |
| Interact with objects | E |
| Advance or reveal dialogue | Space Bar |
| Lens minigame | Hold Space Bar |
| Scan minigame | Press Space Bar when the pointer overlaps the target |
| Keypad minigame | Left/Right Arrow to choose; Space Bar to confirm |
| Pause menu | Esc |
| Navigate menus | Up/Down Arrow; Enter or Space Bar to select |
| Adjust settings | Left/Right Arrow or use the mouse |
| Final decision | Up/Down Arrow; Space Bar to confirm |

## Technology

* **C#**
* **.NET 9**
* **MonoGame 3.8**
* **MonoGame.Extended 6.1.1**
* **Visual Studio**
* **JSON**
### Visual Studio

1. Clone or download this repository.
2. Open `Project The Elect English.csproj` in Visual Studio.
3. Allow NuGet packages to restore.
4. Select the `Release` configuration and build the project.
5. Run the project from Visual Studio, or launch the generated executable in the build output folder.

### Command line

Run these commands from the folder containing the project file:

```powershell
dotnet restore "Project The Elect English.csproj"
dotnet run --project "Project The Elect English.csproj"
```

To create a self-contained Windows x64 release build:

```powershell
dotnet publish "Project The Elect English.csproj" -c Release -r win-x64 --self-contained true
```


## Member

Looktaw | Pixel Artist, Character Design (Player)

Johnny | Project Manager, Lead Game-Designer, Programmer, Sound Engineer, 2D Artist

Kaowniew | Pixel Artist, Level Artist, Level Design

Plub | Pixel Artist, Character Design (Monster)
