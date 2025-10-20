# FTC-SIM In-Game UI Builder Setup Guide

## Overview

This guide will walk you through setting up the in-game drag-and-drop robot builder UI in Unity. The builder features a three-panel layout:
- **Left Panel**: Parts Catalog with search and filtering
- **Center Panel**: 3D Preview with drag-and-drop placement
- **Right Panel**: Properties panel for configuring motors and servos

## Prerequisites

- Unity 6.4 or later (tested with 6000.4.0a2)
- TextMeshPro package installed
- FTC-SIM core scripts already in place

## Quick Setup (Automated)

### Method 1: Using the Setup Script

1. **Create the UI Setup GameObject**
   - In your scene hierarchy, create an empty GameObject
   - Name it "UISetup"
   - Add the `RobotBuilderUISetup` component to it

2. **Configure the Setup**
   - In the Inspector, check `Auto Setup On Start` if you want it to run automatically
   - Set `Create In Game Camera` to true if you don't have a camera yet
   - Adjust `Catalog Panel Width` and `Properties Panel Width` as desired (default: 300px and 350px)

3. **Run the Setup**
   - Either:
     - Press Play and it will automatically set up (if Auto Setup On Start is checked)
     - Or right-click the component in Inspector → Context Menu → "Setup Robot Builder UI"

4. **Save the Scene**
   - The setup creates all necessary UI panels and components
   - Save your scene to preserve the setup

### Method 2: Using Prefab Creators

If you want to create reusable prefabs for UI components:

1. **Create Part Catalog Item Prefab**
   - Create an empty GameObject in your scene
   - Add `PartCatalogItemPrefabCreator` component
   - Right-click component → "Create Part Catalog Item Prefab"
   - Drag the created GameObject into your Project window to save as prefab
   - Delete the GameObject from scene after saving

2. **Create Motor Speed Control Prefab**
   - Create an empty GameObject
   - Add `MotorSpeedControlPrefabCreator` component
   - Right-click → "Create Motor Speed Control Prefab"
   - Save as prefab

3. **Create Servo Configuration Prefab**
   - Create an empty GameObject
   - Add `ServoConfigurationPrefabCreator` component
   - Right-click → "Create Servo Configuration Prefab"
   - Save as prefab

4. **Assign Prefabs to RobotBuilderUI**
   - Find the `RobotBuilderCanvas` GameObject
   - In the `RobotBuilderUI` component:
     - Assign `partItemPrefab` to your PartCatalogItem prefab
     - Assign `motorSpeedControlPrefab` to your MotorSpeedControl prefab
     - Assign `servoConfigPrefab` to your ServoConfiguration prefab

## Manual Setup (Advanced)

If you prefer to build the UI manually:

### Step 1: Create Canvas

1. Right-click in Hierarchy → UI → Canvas
2. Name it "RobotBuilderCanvas"
3. Set Canvas properties:
   - Render Mode: Screen Space - Overlay
   - Add Canvas Scaler component:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080
     - Match: 0.5 (width-height)

### Step 2: Create Parts Catalog Panel

1. Right-click Canvas → UI → Panel
2. Name it "PartsCatalogPanel"
3. Set RectTransform:
   - Anchor: Left stretch (0,0) to (0,1)
   - Pivot: (0, 0.5)
   - Width: 300
4. Add components:
   - Header with title "Parts Catalog"
   - Search input field
   - Category dropdown
   - Scroll view with vertical layout for parts list

### Step 3: Create Preview Panel

1. Right-click Canvas → UI → Panel
2. Name it "PreviewPanel"
3. Set RectTransform:
   - Anchor: Stretch all
   - Offset Min: (300, 0) - left margin for catalog
   - Offset Max: (-350, 0) - right margin for properties
4. Make background transparent or semi-transparent to see 3D view behind it

### Step 4: Create Properties Panel

1. Right-click Canvas → UI → Panel
2. Name it "PropertiesPanel"
3. Set RectTransform:
   - Anchor: Right stretch (1,0) to (1,1)
   - Pivot: (1, 0.5)
   - Width: 350
4. Add components:
   - Header with title "Properties"
   - Selected part name text
   - Scroll view for property controls

### Step 5: Setup RobotBuilderUI Component

1. Select the Canvas GameObject
2. Add Component → `RobotBuilderUI` script
3. Assign all references in Inspector:
   - Panels: Drag the three panel GameObjects
   - Content transforms: Find and assign scroll view content areas
   - UI elements: Assign search field, dropdown, texts
   - Prefabs: Assign the prefabs you created earlier

### Step 6: Setup Build Camera

1. Create a new Camera: GameObject → Camera
2. Name it "BuildCamera"
3. Position it to view your build area (e.g., position: 0, 2, -5)
4. Assign to RobotBuilderUI's `previewCamera` field

### Step 7: Setup RobotController

1. If you don't have a RobotController:
   - Create empty GameObject: "RobotController"
   - Add Component → `RobotController` script
2. Assign to RobotBuilderUI's `robotController` field

## Testing the Builder

### Test in Play Mode

1. Press Play in Unity Editor
2. The parts catalog should populate with available parts
3. Click on a part in the catalog
4. A "ghost" version of the part should appear and follow your mouse
5. Move the ghost near other parts to see snapping visualization
6. Click to place the part
7. Select a placed part to see its properties in the right panel

### Expected Behavior

**Parts Catalog (Left)**:
- Shows all available parts from the PartsLibrary
- Search bar filters parts by name
- Category dropdown filters by category
- Clicking a part starts drag mode

**3D Preview (Center)**:
- Ghost part follows mouse cursor
- Green glow indicates valid snap point
- Red glow indicates no snap point available
- Click to place part at current position
- ESC to cancel drag

**Properties Panel (Right)**:
- Shows "No Part Selected" when nothing is selected
- Displays part name when selected
- Shows motor controls for DC motors
- Shows servo configuration for servos
- Real-time updates when adjusting sliders

## Troubleshooting

### Parts Don't Appear in Catalog

**Problem**: The parts catalog is empty.

**Solutions**:
1. Check that parts are loaded:
   - Parts should be in `StreamingAssets/Parts/` directory
   - Parts should be valid JSON following the PartDefinition schema
2. In Play mode, check the console for loading errors
3. Verify PartsLibrary singleton is initialized:
   ```csharp
   PartsLibrary.Instance.LoadPartsFromDirectory("StreamingAssets/Parts");
   ```

### Ghost Part Doesn't Follow Mouse

**Problem**: Clicking a part doesn't create a ghost.

**Solutions**:
1. Check that DragDropBuilder component exists:
   - It should be created automatically by RobotBuilderUI
   - If not, manually create and assign it
2. Verify Build Camera is assigned
3. Check for console errors

### Snapping Doesn't Work

**Problem**: Parts don't snap together.

**Solutions**:
1. Verify SnappingService is initialized in RobotController
2. Check that parts have attachment points defined:
   - Parts must have `mounts` array in their JSON definition
   - Mounts must have compatible `patternType` values
3. Adjust snap distance tolerance if needed (in SnappingService)

### Properties Panel Empty

**Problem**: Selecting a part doesn't show properties.

**Solutions**:
1. Check that motor/servo prefabs are assigned to RobotBuilderUI
2. Verify the part has an `electronicsProfile` in its definition
3. Check that the properties content transform is correctly assigned

### UI Doesn't Scale Properly

**Problem**: UI looks wrong on different resolutions.

**Solutions**:
1. Check Canvas Scaler settings:
   - Should be "Scale With Screen Size"
   - Reference resolution: 1920 x 1080
2. Verify panels are using proper anchors:
   - Left panel: Left-stretch anchor
   - Right panel: Right-stretch anchor
   - Center panel: Stretch-all with offsets

## Customization

### Changing Panel Sizes

Edit `RobotBuilderUISetup.cs`:
```csharp
public float catalogPanelWidth = 300f;  // Change this
public float propertiesPanelWidth = 350f;  // And this
```

### Styling Colors

All UI components use these color schemes:
- Dark background: `(0.15, 0.15, 0.15, 0.95)`
- Darker elements: `(0.1, 0.1, 0.1, 1.0)`
- Light text: `Color.white`
- Secondary text: `(0.7, 0.7, 0.7, 1.0)`
- Motor color: Blue `(0.2, 0.6, 0.9, 1.0)`
- Servo color: Orange `(0.9, 0.5, 0.2, 1.0)`

Edit the prefab creator scripts to change these values.

### Adding Custom Controls

To add new property controls:

1. Create a new prefab creator script (similar to MotorSpeedControlPrefabCreator)
2. Design your UI layout in the CreatePrefab method
3. Add corresponding C# component script (similar to MotorSpeedControl)
4. Update RobotBuilderUI.CreateElectronicsControls() to instantiate your control

## Advanced Features

### Visual Node Editor for ElectroGraph

The ElectroGraph visual editor allows you to wire up electronics:

1. Access via button in the UI (when implemented) or:
   - Open window: `FTC-SIM > ElectroGraph Editor` (Unity Editor menu)
2. Add nodes by clicking "Add Node" button
3. Select device type from dropdown
4. Drag nodes to position them
5. Click pins to create wires between devices
6. Wire validation happens automatically

### 3D Snapping Gizmos

Snapping visualization shows:
- **Green spheres**: Valid snap points
- **Red spheres**: Invalid/occupied snap points
- **Coordinate axes**: RGB = XYZ orientation at snap point
- **Snap indicator**: Appears at exact snap location when valid

Configure in `SnapGizmo.cs`:
```csharp
public float gizmoSize = 0.05f;
public Color validSnapColor = Color.green;
public Color invalidSnapColor = Color.red;
```

### Keyboard Shortcuts

While building:
- **ESC**: Cancel current drag operation
- **Space**: Start/stop simulation (when RobotController is running)
- **Tab**: Toggle UI visibility (implement in RobotBuilderUI)

## Next Steps

1. **Create More Parts**: Add goBILDA parts to your library
   - Copy example JSON files in `StreamingAssets/Parts/`
   - Follow the PartDefinition schema
   - Include 3D models in Resources folder

2. **Add 3D Models**: Replace primitive cubes with actual CAD models
   - Import CAD files (FBX, OBJ) into Unity
   - Create prefabs for each part
   - Reference prefabs in PartDefinition.prefabPath

3. **Implement ElectroGraph Editor**: Wire up electronics visually
   - Already have base implementation
   - Create UI panel for it
   - Add to robot builder interface

4. **Add Telemetry**: Monitor robot performance
   - Use TelemetryWindow from Editor folder
   - Display voltage, current, RPM in real-time
   - Add to UI for in-game monitoring

5. **Block-Based Programming**: Control robot logic
   - Implement visual programming editor
   - Similar to Scratch or Blockly
   - Integrate with RobotController

## Support

- Check main documentation: `ARCHITECTURE.md`, `GETTING_STARTED.md`
- Review example code in `Examples/` folder
- Check GitHub issues for known problems
- See `IN_GAME_BUILDER_GUIDE.md` for usage instructions

---

**Version**: 1.0
**Last Updated**: January 2025
**For**: Unity 6.4+ HDRP
