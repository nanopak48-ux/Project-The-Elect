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

## Technology

* **C#**
* **.NET 9**
* **MonoGame 3.8**
* **MonoGame.Extended 6.1.1**
* **Visual Studio**
* **JSON**

## Project Structure

```text
Project The Elect/
├── Content/
│   └── Game assets, fonts, audio, textures and dialogue data
│
├── source code/
│   ├── 00_Program.cs
│   ├── 01_Main.cs
│   ├── 02_00_IGameState.cs
│   ├── 02_01_StateHome.cs
│   ├── 02_02_StateCutscene.cs
│   ├── 02_03_StateDialogue.cs
│   ├── 02_04_StatePlay.cs
│   ├── 02_05_StateMenu.cs
│   ├── 03_Audio.cs
│   ├── 04_01_DialogueManager.cs
│   ├── 04_02_DialogueData.cs
│   ├── 04_03_DialogueSprite.cs
│   ├── 05_Font.cs
│   └── 06_Button.cs
│
├── pipeline-references/
├── Project The Elect.csproj
└── app.manifest
```

The project separates **game states**, **dialogue systems**, **audio**, and **UI components** into different classes to make the code easier to manage and expand.

## Game States

The game currently uses several states to control the game flow:

* Home
* Cutscene
* Dialogue
* Gameplay
* Menu

Each state is handled independently through the game's state system.

## Dialogue System

Dialogue data is stored using **JSON**, allowing dialogue content to be separated from the main game code.

The dialogue system consists of:

* `DialogueManager`
* `DialogueData`
* `DialogueSprite`

This makes it easier to add or modify dialogue without changing the core game logic.

## Audio

The project includes a dedicated audio system for handling:

* Background music
* Sound effects
* Audio playback

Audio functionality is handled through `03_Audio.cs`.

## Requirements

To build and run the project, you will need:

* Windows
* Visual Studio
* .NET 9 SDK
* MonoGame 3.8
* NuGet packages used by the project

The project file currently targets **.NET 9** and references MonoGame Framework DesktopGL and MonoGame.Extended 6.1.1.

## How to Run

1. Clone the repository.

```bash
git clone https://github.com/nanopak48-ux/Project-The-Elect.git
```

2. Open:

```text
Game DeV I Proj/Game DeV I Proj.slnx
```

3. Open the project in Visual Studio.

4. Restore the NuGet packages.

5. Build and run the project.

## Project Status

**In Development**

This project is currently under development. Features, gameplay systems, assets, and story content may change during development.

## Repository

[Project The Elect – GitHub](https://github.com/nanopak48-ux/Project-The-Elect?utm_source=chatgpt.com)

## License

This project is created for educational and development purposes.


## Member

Looktaw | Pixel Artist, Character Design (Player)

Johnny | Project Manager, Programmer, Sound Engineer, 2D Artist

Kaowniew | Pixel Artist, Level Artist, Level Design

Plub | Pixel Artist, Character Design (Monster)
