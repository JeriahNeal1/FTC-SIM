# FTC-SIM Quick Start User Guide

## What You Need to Do in Unity Editor

This guide tells you **exactly** what to do in the Unity Editor to get the FTC-SIM robot builder working.

## Prerequisites

- Unity 6.4 or later installed
- FTC-SIM repository cloned and opened in Unity
- TextMeshPro package installed (Unity will prompt you if needed)

## Step-by-Step Setup

### Option 1: Automatic Setup (Recommended - 2 minutes)

This is the fastest way to get everything working.

1. **Open Unity Project**
   - Open Unity Hub
   - Click "Open" and select the FTC-SIM folder
   - Wait for Unity to import all assets (this may take a few minutes on first load)

2. **Open or Create a Scene**
   - If you have a scene: Open it (e.g., `Assets/OutdoorsScene.unity`)
   - If starting fresh: Create new scene (File → New Scene → Basic (Built-in))

3. **Run the Complete Scene Setup**
   - In the Hierarchy, right-click → Create Empty
   - Name it "SceneSetup"
   - In the Inspector, click "Add Component"
   - Search for "Complete Scene Setup"
   - Select `CompleteSceneSetup` script
   - In the Inspector, check these options:
     - ☑ Setup On Start
     - ☑ Load Example Parts
     - ☑ Create Floor
   - Right-click on the `CompleteSceneSetup` component
   - Click "Setup Complete Scene"

4. **Wait for Setup to Complete**
   - Watch the Console window for progress messages
   - You should see messages like:
     ```
     ✓ Created RobotController
     ✓ Build camera configured
     ✓ Created floor
     ✓ Robot Builder UI created
     ✓ Loaded parts library
     ```

5. **Save Your Scene**
   - File → Save Scene As...
   - Name it something like "RobotBuilderScene"

6. **Enter Play Mode**
   - Click the Play button (▶) at the top of the Unity Editor
   - The robot builder UI should appear!

**You're done! Skip to "Using the Robot Builder" section below.**

---

### Option 2: Manual Prefab Setup (5-10 minutes)

If you want more control or the automatic setup doesn't work:

#### A. Create Required Prefabs

1. **Create Part Catalog Item Prefab**
   - Create empty GameObject in Hierarchy
   - Add `PartCatalogItemPrefabCreator` component
   - Right-click component → "Create Part Catalog Item Prefab"
   - Drag the created GameObject from Hierarchy to Project window
   - Create a folder: `Assets/FTC-SIM/Prefabs` if it doesn't exist
   - Save it as "PartCatalogItem.prefab"
   - Delete the GameObject from Hierarchy

2. **Create Motor Speed Control Prefab**
   - Create empty GameObject
   - Add `MotorSpeedControlPrefabCreator` component  
   - Right-click → "Create Motor Speed Control Prefab"
   - Save as "MotorSpeedControl.prefab"
   - Delete from Hierarchy

3. **Create Servo Configuration Prefab**
   - Create empty GameObject
   - Add `ServoConfigurationPrefabCreator` component
   - Right-click → "Create Servo Configuration Prefab"
   - Save as "ServoConfiguration.prefab"
   - Delete from Hierarchy

4. **Create ElectroGraph UI Prefabs**
   - Create empty GameObject
   - Add `ElectroGraphUIPrefabCreator` component
   - Right-click → "Create ElectroGraph Node UI Prefab"
   - Save as "ElectroNodeUI.prefab", delete from Hierarchy
   - Repeat for Pin and Wire prefabs:
     - "Create ElectroGraph Pin UI Prefab" → "ElectroPinUI.prefab"
     - "Create ElectroGraph Wire UI Prefab" → "ElectroWireUI.prefab"

#### B. Setup the UI Canvas

1. **Create the UI Setup GameObject**
   - Hierarchy → Right-click → Create Empty
   - Name it "UISetup"
   - Add Component → `RobotBuilderUISetup`
   - In Inspector, configure:
     - Catalog Panel Width: 300
     - Properties Panel Width: 350
     - Create In Game Camera: ☑ (check)
   - Right-click component → "Setup Robot Builder UI"

2. **Assign the Prefabs**
   - Find "RobotBuilderCanvas" in Hierarchy
   - Select it
   - Find the `RobotBuilderUI` component in Inspector
   - Drag your prefabs to these fields:
     - Part Item Prefab → PartCatalogItem.prefab
     - Motor Speed Control Prefab → MotorSpeedControl.prefab
     - Servo Config Prefab → ServoConfiguration.prefab

#### C. Setup RobotController

1. **Create RobotController**
   - If not already present:
   - Hierarchy → Right-click → Create Empty
   - Name it "RobotController"
   - Add Component → `RobotController`

2. **Link to UI**
   - Select "RobotBuilderCanvas"
   - In `RobotBuilderUI` component
   - Drag "RobotController" to the Robot Controller field

#### D. Load Parts

1. **Ensure Parts Directory Exists**
   - In Project window, navigate to `Assets/StreamingAssets/Parts`
   - You should see example JSON files:
     - ExamplePart_CChannel.json
     - ExamplePart_Motor.json

2. **Parts Load Automatically in Play Mode**
   - Parts are loaded when RobotController initializes
   - You'll see them in the parts catalog

---

## Using the Robot Builder

Once setup is complete and you're in Play Mode:

### Building Your First Robot

1. **Look at the UI Layout**
   - **Left Panel**: Parts Catalog
   - **Center**: 3D viewport (where your robot appears)
   - **Right Panel**: Properties (for selected parts)

2. **Browse the Parts Catalog**
   - Left panel shows available parts
   - Use the search bar to filter by name
   - Use the dropdown to filter by category

3. **Add a Part**
   - Click on a part in the catalog (e.g., "2x2 C-Channel")
   - A "ghost" version appears and follows your mouse
   - Move it around in the 3D viewport

4. **Snap Parts Together**
   - Move the ghost part near another part
   - When close to a valid snap point:
     - Ghost turns **green**
     - A snap indicator appears
   - Click to place the part

5. **Configure Motors and Servos**
   - Click on a placed motor or servo in the 3D view
   - The Properties panel (right) updates
   - For motors:
     - Adjust speed slider (0-100%)
     - Toggle reverse direction
   - For servos:
     - Set target position (0-180°)
     - Adjust speed and torque

6. **Cancel Drag**
   - Press **ESC** to cancel placing a part

### Keyboard Shortcuts

- **ESC**: Cancel current drag operation
- **Space**: Start/stop simulation (if implemented)
- **Tab**: Toggle UI visibility (if implemented)

### Tips for First-Time Users

1. **Start Simple**: Begin with 2-3 parts to learn the system
2. **Use Snapping**: Let the green indicator guide you to valid connections
3. **Check the Console**: Unity console shows helpful messages and errors
4. **Save Often**: File → Save Scene to preserve your work
5. **Experiment**: Try different parts and configurations!

## Troubleshooting

### "Parts Catalog is Empty"

**Problem**: No parts appear in the left panel.

**Solutions**:
1. Check Console for error messages
2. Verify `Assets/StreamingAssets/Parts/` contains JSON files
3. Exit Play mode, check files exist, re-enter Play mode
4. Run `CompleteSceneSetup` again with "Load Example Parts" checked

### "Ghost Part Doesn't Appear"

**Problem**: Clicking a part doesn't start dragging.

**Solutions**:
1. Make sure you're in Play Mode (press ▶)
2. Check Console for errors
3. Verify RobotController exists in scene
4. Check that Build Camera is assigned in RobotBuilderUI

### "Parts Won't Snap"

**Problem**: Parts stay red and won't snap together.

**Solutions**:
1. Move closer to the target part
2. Make sure parts have compatible mount patterns
3. Try rotating your view to get a better angle
4. Check that parts have attachment points defined in JSON

### "Properties Panel is Blank"

**Problem**: Selecting a motor doesn't show controls.

**Solutions**:
1. Verify prefabs are assigned to RobotBuilderUI
2. Check that the part has an electronicsProfile in its JSON
3. Look for errors in Console
4. Try placing a new motor part

### "UI Looks Wrong / Doesn't Scale"

**Problem**: UI panels are misaligned or cut off.

**Solutions**:
1. Check Canvas Scaler settings:
   - Should be "Scale With Screen Size"
   - Reference: 1920 x 1080
2. Try different aspect ratios in Game view
3. Adjust panel widths in RobotBuilderUISetup
4. Re-run the setup script

### "Script Compilation Errors"

**Problem**: Red errors in Console about missing scripts.

**Solutions**:
1. Wait for Unity to finish importing all assets
2. Assets → Reimport All
3. Check that all .meta files are present
4. Verify Unity version is 6.4 or later

## What to Do Next

### 1. Create Custom Parts

- Duplicate an example JSON file in `StreamingAssets/Parts/`
- Modify properties (name, mass, category, etc.)
- Define attachment points and electronics profiles
- Reload parts (exit and re-enter Play mode)

### 2. Add 3D Models

- Import CAD files (FBX, OBJ) into Unity
- Create prefabs from them
- Reference prefabs in part JSON files
- Parts will use actual 3D models instead of cubes

### 3. Wire Electronics

- Open ElectroGraph Editor (Window → FTC-SIM → ElectroGraph Editor)
- Add battery, hub, motors
- Connect them with wires
- Validate electrical connections

### 4. Add Programming

- Implement block-based programming (future feature)
- Control robot behavior with code
- Test in simulation

### 5. Build Complex Robots

- Create subassemblies
- Add wheels and drivetrains
- Configure sensors
- Test different designs

## Getting Help

- **Documentation**: Check other .md files in `Assets/FTC-SIM/Documentation/`
- **Examples**: Look at `Assets/FTC-SIM/Examples/` for code samples
- **Console**: Unity Console shows helpful error messages
- **GitHub**: Check repository for updates and issues

## Summary Checklist

Before entering Play Mode, verify:

- ✓ RobotController GameObject exists in scene
- ✓ Main Camera exists and is positioned correctly
- ✓ RobotBuilderCanvas exists with UI panels
- ✓ RobotBuilderUI component has all references assigned
- ✓ Prefabs are created and assigned
- ✓ StreamingAssets/Parts/ directory contains JSON files
- ✓ No red errors in Console window
- ✓ Scene is saved

If all items are checked, click Play (▶) and start building!

---

**Need Help?** Check the full documentation in `UI_BUILDER_SETUP_GUIDE.md` and `IN_GAME_BUILDER_GUIDE.md`.

**Version**: 1.0  
**Last Updated**: January 2025  
**For**: Unity 6.4+ HDRP
