# Magic Tiles Clone

## How to Run the Project

1. **Clone or Download the Repository**
   - Clone from this repo(privated):https://github.com/hypnoticHat/MatchTitle3Clone
   - or extract `.zip` file and open it with Unity
   - EX: how to open unity zip file with unity:https://www.youtube.com/watch?v=oCl3uzlbOOU

2. **Open in Unity**
   - Open the project in **Unity 2021.3 LTS** or later.
   - If you get any errors related to packages, let Unity auto-resolve or use the Package Manager.

3. **Play the Game**
   - Open the `MainScene` from `Assets/Scenes/`.
   - Press the Play button in Unity Editor to start.
   - Click Star button in middle of the UI and listen to the music while tapping the tiles that fall in rhythm.

## Design Choices
- **Beatmap-driven spawning**:  
Tiles are spawned based on a `.txt` beatmap file containing timestamps and lane data. This allows for easy beatmap creation or editing.

- **Object Pooling**:  
Used to optimize performance by reusing tile objects, especially important for long tiles which might otherwise create performance spikes.

- **Singleton Pattern**:  
Core systems like `GameManager`, `AudioManager`, `TileManager`, and `PoolManager` use the Singleton pattern for global access and centralized control.

- **State Machine**:  
`GameManager` manages game flow using a simple state machine (`Menu`, `Playing`, `Paused`, `GameOver`), making game logic easier to follow and extend.

- **Event System**:  
Loose coupling between components is achieved via events for example


## Beatmap Format
A beatmap file (`beatmap.txt` in `Resources/`) consists of rows like:
- The first number is **time (in seconds)**.
- The following numbers (0,1,2) represent each lane:
  - `0`: no tile
  - `1`: normal tap tile
  - `2`: part of a long tile (multiple `2`s vertically stacked = longer note)

## Attribution
 - 2D Casual UI HD
 - EXPORT PROJECT TO ZIP
---

