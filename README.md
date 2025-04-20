## Description
tool for Squad that provides tracking of players/vehicles/deployables on a 2D map based on existing in game map textures / images.
Familiar maps and intuitive to gain intel.

## Mod Support
- Global Escalation
- Steel Division

## Features
### Radar Features
- POI placements for mortar calculations
- Displays all friendly and enemy players 
- Displays all vehicles / emplacements / deployables
- Auto selects the layer 
- Displays a distance for any vehicle

### Gameplay Features

- Extended Interactions: Interact with objects from greater distances
- No Recoil, No Spread, No Sway ALL IN ONE feature (credits Lilith2) Gun model will still look lively and correct but all shots are pin point perfect to original Point of Aim.

Features from the original Butters222 source no listed here means I haven't verified how bugged or jank it is. Butters coded this haphazzardly and introduced many chances to crash the game client. In most cases, his features don't even work such as his own no recoil / no spread / no sway.


## Usage
1. Clone the repository.
2. Ensure all necessary dependencies are in place.
3. Compile the project in release mode unless you know what you're doing.
4. Run the application.

## Dependencies are included with this project
- FTD3XX.dll
- leechcore.dll, vmm.dll, dbghelp.dll, symsrv.dll and vcruntime140.dll - https://github.com/ufrisk/MemProcFS/releases
- libSkiaSharp.dll - SkiaSharp library

## Note
Ensure all necessary files are properly included and referenced for the application to function correctly.

## Acknowledgments
This project builds upon the original work created by [UC Forum Thread](https://www.unknowncheats.me/forum/escape-from-tarkov/482418-2d-map-dma-radar-wip.html) and its continuation [EFT-DMA-Radar-v2](https://www.unknowncheats.me/forum/escape-from-tarkov/639021-dma-radar-v2.html) by x0m, Keegi and MasterKeef. It is basically a fork of a EFT-DMA-Radar-v2 adapted to UE4 Squad base. It has a lot of perfomance issues, as well as some bugs.

## Preview
![image](https://github.com/Lilith2/Lone-Squad-Source/blob/main/preview/radar-preview.png)
