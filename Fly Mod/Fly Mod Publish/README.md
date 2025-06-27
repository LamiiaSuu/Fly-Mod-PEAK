# FlyMod

**FlyMod v1.2.1** by lamia  
A flight mod for PEAK, using BepInEx. Supports toggleable or hold-to-fly controls, creative-style noclip, or a more jetpack-like flight.

---

## Features

- Freely fly through the game world
- Toggle or hold-to-fly (configurable)
- “Creative” noclip-style mode or directional jetpack/hero mode
- Sprint boost with Shift
- Customizable fly key (default **V**)

---

## Controls (Default)

- **Toggle Fly:** `V`
- **Sprint:** `LeftShift` (increases speed)
- **Ascend:** `Space`
- **Descend:** `LeftControl` (in creative mode)
- **Directional Movement:** WASD

---

## Installation

1. Install BepInEx for PEAK.
2. Place `Fly Mod.dll` into your `BepInEx/plugins` folder.
3. Launch the game.
4. The configuration file `BepInEx/config/com.lamia.flymod.cfg` will be created automatically.
5. Adjust settings in that file if desired.

---

## Configuration Highlights

Available in `com.lamia.flymod.cfg`:

- **FlyKey**: set the key to activate flying (default `V`)
- **ToggleFly**: toggle vs. hold-to-fly
- **CreativeFlyMode**: choose between noclip-style or boost-style flight
- **BaseForce, VerticalForce, SprintMultiplier, Maximum Velocity**: fine-tune the feeling of flight

---

