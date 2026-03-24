# Lunar's Vehicle Framework

A powerful C# library for Subnautica that makes it incredibly easy to add new vehicles, fabricators, and items to the game.

**Current Version: 1.2.0**

## Features

✨ **Simple Fluent API** - Easy-to-read, chainable syntax for defining game content
🚗 **Vehicle Builder** - Add custom vehicles with just a few lines of code
🔧 **Fabricator System** - Create new fabricators with custom recipes
📦 **Item Management** - Add items and materials with flexible properties
🎨 **Model Support** - Use .fbx, .glTF, and other 3D formats directly (automatic conversion to bundles)
⚙️ **Minimal Dependencies** - Requires only Tobey's BepInEx Pack

## Installation

1. Place compiled `LunarsVehicleFramework.dll` in your BepInEx `plugins` folder
2. Restart your game

## Building

```batch
.\build.cmd Release
```

Output: `LunarsVehicleFramework\bin\Release\LunarsVehicleFramework.dll` OR:
Output: `L:\SteamLibrary\steamapps\common\Subnautica\BepInEx\Plugins\LunarsVehicleFramework\LunarsVehicleFramework.dll`

## Changelog

### v1.2.0
- **New:** Mobile Vehicle Bay custom tabs via `WithMVBTab()`
- **New:** Automatic logging for vehicle load events via `WithLogging()`
- **New:** Automatic spawn command registration via `WithSpawnCommand()`
- **New:** Console command `spawn <vehicle_id>` for testing
- **Logs:** `[VehicleLoad]`, `[VehicleCraftedMVB]`, `[VehicleSpawned]` prefixes for easy filtering

### v1.1.0
- **New:** Vehicle crafting recipes via `AddRecipe()`
- **New:** Mobile Vehicle Bay integration

### v1.0.0
- **Initial Release:** Basic vehicle, fabricator, and item builders
- Core fluent API with flexible properties
- 3D model file support (.fbx, .glTF)

### Prerequisites

- .NET SDK 6.0+ (download from https://dotnet.microsoft.com/download)
- Subnautica with BepInEx installed
- Text editor or Visual Studio

## Usage Examples

### Creating a Vehicle

```csharp
using LunarsVehicleFramework.Core;

// Simple vehicle
new VehicleBuilder("my_custom_vehicle", "My Custom Vehicle")
    .WithModel("Assets/Models/my_vehicle.fbx")
    .WithScale(1.5f, 1.5f, 1.5f)
    .WithProperty("Speed", 25f)
    .WithProperty("Health", 200)
    .Build();
```

### Creating a Vehicle with Mobile Vehicle Bay Recipe

**New in v1.1.0** - Add recipes to craft vehicles in the Mobile Vehicle Bay:

```csharp
using LunarsVehicleFramework.Core;

new VehicleBuilder("swiftfin", "Swiftfin")
    .WithModel("Assets/Swiftfin.fbx")
    .WithScale(1.0f, 1.0f, 1.0f)
    .WithProperty("Speed", 20f)
    .WithProperty("Health", 150)
    .WithProperty("MaxUpgradeModules", 4)
    .AddRecipe("MobileVehicleBay", 1, 
        ("titanium", 10),
        ("copper_wire", 4),
        ("advanced_wiring_kit", 1),
        ("glass", 2))
    .Build();
```

**What's happening:**
- `AddRecipe("MobileVehicleBay", ...)` - Adds a crafting recipe in the Mobile Vehicle Bay
- First parameter is the fabricator ID (use `"MobileVehicleBay"` for vehicle bay)
- Second parameter is how many of this vehicle you get per craft (usually 1)
- Following parameters are ingredient IDs and quantities needed

### Creating a Vehicle with Custom MVB Tab (v1.2.0+)

**New in v1.2.0** - Organize vehicles into custom tabs, automatic logging, and spawn commands:

```csharp
using LunarsVehicleFramework.Core;

new VehicleBuilder("ghost_leviathan", "Ghost Leviathan")
    .WithModel("Assets/GhostLeviathan.fbx")
    .WithScale(2.0f, 2.0f, 2.0f)
    .WithProperty("Speed", 30f)
    .WithProperty("Health", 500)
    .WithProperty("MaxUpgradeModules", 8)
    .AddRecipe("MobileVehicleBay", 1,
        ("titanium", 30),
        ("copper_wire", 10),
        ("advanced_wiring_kit", 5),
        ("glass", 8),
        ("diamond", 2))
    .WithMVBTab("Exotic Vehicles")    // Creates/uses "Exotic Vehicles" tab
    .WithLogging(true)                 // Log events (default: true)
    .WithSpawnCommand(true)            // Enable spawn command (default: true)
    .Build();
```

**What happens automatically:**
- Logs `[VehicleLoad]` when vehicle is registered
- Logs `[VehicleCraftedMVB]` when crafted in Mobile Vehicle Bay
- Logs `[VehicleSpawned]` when spawned via console
- Registers console command: `spawn ghost_leviathan`
- Creates "Exotic Vehicles" tab in the MVB UI if it doesn't exist
- Vehicle appears in the specified MVB tab for crafting

### Creating a Fabricator

```csharp
using LunarsVehicleFramework.Core;

new FabricatorBuilder("my_fabricator", "Advanced Workbench")
    .AddRecipe("titanium_ingot", 1, 
        ("titanium_ore", 2), 
        ("energy", 100))
    .AddRecipe("copper_wire", 5,
        ("copper_ore", 3))
    .WithProperty("PowerUsage", 50)
    .Build();
```

### Creating Items

```csharp
using LunarsVehicleFramework.Core;

new ItemBuilder("exotic_crystal", "Exotic Crystal")
    .WithDescription("A rare material found deep underwater")
    .WithIcon("Assets/Icons/crystal.png")
    .WithStackSize(64)
    .WithProperty("Rarity", "Legendary")
    .Build();
```

### Loading 3D Models

```csharp
using LunarsVehicleFramework.Models;

// Load model from file (automatic format detection)
GameObject model = ModelLoader.LoadModel("Assets/Models/my_vehicle.fbx");

// Load AssetBundle
AssetBundle bundle = BundleConverter.LoadBundle("Assets/bundles/my_bundle.bundle");
Mesh mesh = BundleConverter.LoadAsset<Mesh>(bundle, "my_mesh");
```

## Project Structure

```
LunarsVehicleFramework/
├── Plugin.cs                 # Main BepInEx plugin entry point
├── LunarsVehicleFramework.csproj
├── Properties/
│   └── AssemblyInfo.cs      # Assembly metadata
├── Core/
│   ├── VehicleBuilder.cs     # Vehicle creation API
│   ├── FabricatorBuilder.cs  # Fabricator creation API
│   └── ItemBuilder.cs        # Item creation API
└── Models/
    ├── ModelLoader.cs        # Load 3D model files
    └── BundleConverter.cs    # AssetBundle management
```

## API Reference

### VehicleBuilder
- `WithModel(string path)` - Set 3D model file path
- `WithScale(float x, float y, float z)` - Set model scale
- `WithProperty(string key, object value)` - Add custom properties
- `AddRecipe(string fabricatorId, int outputCount, params ingredients)` - Add recipe for crafting (v1.1.0+)
- `WithMVBTab(string tabName)` - Set the Mobile Vehicle Bay tab (v1.2.0+)
- `WithLogging(bool enabled)` - Enable/disable automatic logging (v1.2.0+, default: enabled)
- `WithSpawnCommand(bool enabled)` - Enable/disable automatic spawn command (v1.2.0+, default: enabled)
- `Build()` - Register the vehicle

### FabricatorBuilder
- `AddRecipe(string outputId, int outputCount, params ingredients)` - Add recipe
- `WithProperty(string key, object value)` - Add custom properties
- `Build()` - Register the fabricator

### ItemBuilder
- `WithDescription(string desc)` - Set item description
- `WithIcon(string path)` - Set item icon
- `WithStackSize(int size)` - Set inventory stack size
- `WithProperty(string key, object value)` - Add custom properties
- `Build()` - Register the item

### ModelLoader
- `LoadModel(string path)` - Load .fbx or .glTF models
- `ConvertToBundle(GameObject model, string outputPath)` - Convert to AssetBundle

### BundleConverter
- `LoadBundle(string path)` - Load AssetBundle from file
- `LoadAsset<T>(AssetBundle bundle, string name)` - Extract asset from bundle
- `UnloadBundle(string path, bool unloadObjects)` - Free bundle resources
- `GetAllAssetNames(AssetBundle bundle)` - List all assets in bundle

## Logging

All framework actions log to BepInEx logger. Vehicles have automatic structured logging with prefixes for easy filtering:

**Automatic Vehicle Logging (v1.2.0+):**
```
[VehicleLoad]       - When vehicle is registered/loaded
[VehicleCraftedMVB] - When vehicle is crafted in Mobile Vehicle Bay
[VehicleSpawned]    - When vehicle is spawned via console command
[SpawnCommand]      - When spawn command is registered or executed
```

**Manual Logging:**
```csharp
LunarsVehicleFrameworkPlugin.Logger.LogInfo("My message");
LunarsVehicleFrameworkPlugin.Logger.LogError("Error message");
```

## Spawn Commands (v1.2.0+)

Vehicles automatically register console commands for spawning during testing:

```
spawn <vehicle_id>
```

**Example:**
```
spawn ghost_leviathan
spawn swiftfin
spawn my_custom_vehicle
```

**Features:**
- Automatically registered when vehicle is built
- Vehicle ID is case-insensitive
- Logs spawn events with `[VehicleSpawned]` prefix
- Can be disabled per-vehicle with `.WithSpawnCommand(false)`

## Requirements

- **Tobey's BepInEx Pack** for Subnautica
- .NET Framework 4.7.2+
- UnityEngine (provided by Subnautica)

## Supported Model Formats

- `.fbx` (Autodesk FBX)
- `.gltf` / `.glb` (glTF/Binary glTF)

Other formats may be added in future updates.

## Mobile Vehicle Bay (MVB) Crafting

Use the `AddRecipe()` method to make your vehicles craftable in the Mobile Vehicle Bay:

```csharp
new VehicleBuilder("my_vehicle", "My Vehicle")
    .WithModel("Assets/my_vehicle.fbx")
    .AddRecipe("MobileVehicleBay", 1,    // 1 vehicle per craft
        ("titanium", 15),                 // 15 titanium
        ("advanced_wiring_kit", 2),       // 2 wiring kits
        ("glass", 4))                     // 4 glass
    .Build();
```

**Understanding Recipe Parameters:**
- **Fabricator ID**: `"MobileVehicleBay"` - The crafting station (always this for vehicles)
- **Output Count**: How many vehicles you get (typically 1)
- **Ingredients**: (`ingredientId`, `count`) pairs
  - `ingredientId` must match existing Subnautica item IDs
  - `count` is the quantity needed

**Common Subnautica Item IDs:**
- `titanium` - Raw titanium ore
- `copper_ore` - Copper ore
- `copper_wire` - Processed copper wire
- `advanced_wiring_kit` - Advanced electronics component
- `glass` - Processed glass
- `diamond` - Precious mineral
- `lithium` - Energy ore
- `quartz` - Silicon derivative

You can add multiple recipes to the same vehicle for different crafting stations by calling `AddRecipe()` multiple times.

## Notes

- All IDs must be unique across your mod ecosystem
- Model paths are relative to the mod folder
- Recipe ingredient counts are in raw units (crafting multipliers handled separately)
- Properties are flexible: use them for any custom data your vehicle/fabricator needs
- Fabricator IDs must match existing Subnautica fabricators (e.g., `"MobileVehicleBay"`, `"Fabricator"`, `"SeamothUpgradeConsole"`)
- Disable logging/spawn commands if you want silent operation: `.WithLogging(false).WithSpawnCommand(false)`

## Version History

### Upcoming Features (Planning)
- **v1.3.0:** Seamoth upgrade modules API
- **v1.4.0:** PDA/Encyclopedia entries for vehicles
- **v1.5.0:** Vehicle customization/paint jobs system

### How to Check Your Version

```csharp
// The version is in the BepInEx plugin attribute:
// [BepInPlugin("com.lunar.vehicleframework", "Lunar's Vehicle Framework", "X.Y.Z")]

// Check in BepInEx console or mod logs for version info
```

## License

See LICENSE file for details.

## Support

For issues or feature requests, please contact the mod author.
For things to me, open a issue on github.com/Lunar-Bytes.