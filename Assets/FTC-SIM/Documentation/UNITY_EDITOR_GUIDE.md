# Unity Editor Integration Guide

## Overview

The Unity Editor integration for FTC-SIM provides a complete visual interface for building and managing robots without writing code. This guide explains how to use each editor window and tool.

## Quick Start

### First Time Setup

1. **Open Unity 6.4+** and load your FTC-SIM project
2. **Go to menu**: `FTC-SIM > Quick Start Wizard`
3. **Follow the wizard** steps to:
   - Create the StreamingAssets/Parts folder
   - Copy example parts
   - Create a RobotController in your scene
   - Test the simulation

### Accessing FTC-SIM Tools

All FTC-SIM tools are accessible from the top menu bar:
```
FTC-SIM/
├── Quick Start Wizard          (Setup guide for new users)
├── Robot Builder Window        (Main assembly interface)
├── Parts Library Browser       (Browse available parts)
├── Telemetry Window           (Monitor simulation data)
├── Create Robot Controller    (Add controller to scene)
├── Setup/                     (Setup utilities)
├── Documentation/             (Open guides)
└── About FTC-SIM              (Version info)
```

## Editor Windows

### 1. Robot Builder Window

**Access**: `FTC-SIM > Robot Builder Window`

The main window for managing your robot assembly.

#### Features

**Assembly Tab**
- View hierarchical assembly structure
- Select nodes to see details
- Navigate parent/child relationships
- View node properties (SKU, mass, children count)

**Parts Library Tab**
- Search parts by name or SKU
- Filter by category
- View part details (mass, mounts, electronics)
- Browse available components

**Robot Info Tab**
- Save robot to JSON file
- Load robot from JSON file
- View assembly statistics
- Access documentation links

#### What YOU Need to Do

1. **Enter Play Mode** (Press ▶ at top of Unity)
   - This initializes the RobotController
   - Parts library loads from StreamingAssets/Parts/
   
2. **Use the Window** to:
   - Monitor your assembly hierarchy
   - Browse available parts
   - Save/load robot configurations

**Note**: Robot building is currently programmatic (via scripts). Visual drag-and-drop assembly is planned for future updates.

---

### 2. Parts Library Browser

**Access**: `FTC-SIM > Parts Library Browser`

Browse and explore all available robot parts.

#### Features

- **Search bar**: Find parts by name, SKU, or description
- **Category filter**: Filter by Structural, Actuators, Wheels, etc.
- **Split view**: Parts list on left, details on right
- **Detailed information**:
  - Basic info (SKU, vendor, mass)
  - Attachment points with patterns
  - Rotational mounts (shafts, bearings)
  - Electronics profiles (motors, sensors)
  - Collider definitions
  - Part rules and constraints

#### What YOU Need to Do

1. **View the browser anytime** - works in Edit mode and Play mode
2. **Click "Open StreamingAssets/Parts Folder"** to add custom parts
3. **Click "View Example Schemas"** to see part definition format
4. **Select a part** to view complete specifications

**To Add Custom Parts**:
1. Create a JSON file following the schema (see examples)
2. Place it in `StreamingAssets/Parts/`
3. Click "Refresh" in the browser
4. Enter Play mode to load the parts

---

### 3. Telemetry Window

**Access**: `FTC-SIM > Telemetry Window`

Real-time monitoring of robot performance metrics.

#### Features

- **Auto-refresh**: Updates at 10Hz during simulation
- **Battery metrics**: Voltage, current, power with visual bars
- **Motor data**: RPM for each motor
- **Motor currents**: Individual motor current draw
- **Temperatures**: Estimated component temperatures
- **Data logging**: Start/stop telemetry logging
- **Export**: Save telemetry data to CSV

#### What YOU Need to Do

1. **Enter Play Mode**
2. **Start simulation** (in Robot Builder or via RobotController inspector)
3. **Open Telemetry Window**
4. **Click "Start Logging"** to begin collecting data
5. **Monitor real-time metrics** as your robot runs
6. **Click "Stop Logging"** when done
7. **Click "Export to CSV"** to save the data

**Use Cases**:
- Monitor battery drain during operation
- Check motor RPM and current
- Detect thermal issues
- Analyze robot performance over time

---

### 4. Quick Start Wizard

**Access**: `FTC-SIM > Quick Start Wizard`

Step-by-step setup guide for first-time users.

#### Steps

1. **Welcome**: Introduction to FTC-SIM
2. **Setup**: Create folders and copy example parts
3. **Create Controller**: Add RobotController to scene
4. **Test**: Instructions for testing the simulation
5. **Next Steps**: Links to windows and documentation

#### What YOU Need to Do

1. **Run the wizard once** when first setting up
2. **Follow each step** - the wizard automates setup tasks
3. **Click the action buttons** to:
   - Create StreamingAssets folder
   - Copy example parts
   - Create RobotController
4. **Use "Next" button** to progress through steps

---

## Custom Inspector for RobotController

When you select a GameObject with a RobotController component, you'll see an enhanced inspector.

### Features

- **Simulation controls**: Start/Stop buttons
- **Robot management**: Save/Load buttons with file dialogs
- **Assembly statistics**: Node count and total mass
- **Electronics statistics**: Number of nodes and wires
- **Quick actions**: Open Robot Builder and Telemetry windows

### What YOU Need to Do

1. **Select the RobotController GameObject** in the Hierarchy
2. **View the Inspector panel** on the right
3. **In Play Mode**:
   - Click "Start Simulation" to begin physics
   - Click "Stop Simulation" to pause
   - Click "Save Robot" to export to JSON
   - Click "Load Robot" to import from JSON
4. **Use Quick Actions** to open related windows

---

## Menu System

### Main Menu Items

#### FTC-SIM > Quick Start Wizard
Opens the setup wizard for new users.

#### FTC-SIM > Robot Builder Window
Opens the main robot builder interface.

#### FTC-SIM > Parts Library Browser
Opens the parts catalog browser.

#### FTC-SIM > Telemetry Window
Opens the telemetry monitoring window.

#### FTC-SIM > Create Robot Controller
Creates a new RobotController GameObject in the scene.
- Checks if one already exists
- Offers to select existing or create new
- Automatically selects the created object

#### FTC-SIM > Setup > Create StreamingAssets Folder
Creates the `StreamingAssets/Parts/` folder structure.
- Opens the folder in file explorer when done

#### FTC-SIM > Setup > Copy Example Parts to StreamingAssets
Copies example part definitions from the project to StreamingAssets.
- Includes: ExamplePart_CChannel.json, ExamplePart_Motor.json

#### FTC-SIM > Documentation > Getting Started
Opens the Getting Started guide in your default markdown viewer.

#### FTC-SIM > Documentation > Architecture
Opens the Architecture documentation.

#### FTC-SIM > Documentation > Implementation Summary
Opens the implementation summary document.

#### FTC-SIM > About FTC-SIM
Shows version and project information.

---

## Workflow Guide

### Creating Your First Robot

1. **Setup** (one-time):
   ```
   FTC-SIM > Quick Start Wizard
   - Follow all steps
   - Ensure RobotController is created
   ```

2. **Enter Play Mode**:
   ```
   Click ▶ button at top of Unity
   - RobotController initializes
   - Parts library loads
   - Systems become active
   ```

3. **Open Robot Builder**:
   ```
   FTC-SIM > Robot Builder Window
   - View assembly hierarchy
   - Browse parts library
   - Monitor robot status
   ```

4. **Build Robot** (currently via script):
   ```
   - Create a MonoBehaviour script
   - Use the API to build assembly
   - See SimpleRobotExample.cs for reference
   ```

5. **Start Simulation**:
   ```
   In Robot Builder or RobotController inspector:
   - Click "Start Simulation"
   - Robot begins physics simulation
   ```

6. **Monitor Performance**:
   ```
   FTC-SIM > Telemetry Window
   - Start logging
   - Watch real-time metrics
   - Export data when done
   ```

7. **Save Your Work**:
   ```
   In Robot Builder or RobotController:
   - Click "Save Robot"
   - Choose location and name
   - Robot saved as JSON
   ```

### Loading an Existing Robot

1. **Enter Play Mode**
2. **Open Robot Builder** or select RobotController
3. **Click "Load Robot"**
4. **Navigate to your JSON file**
5. **Click Open** - robot loads into scene

### Browsing Parts

1. **Open Parts Library Browser** (works anytime)
2. **Use search bar** or category filter
3. **Click on a part** to see full details
4. **View mounts, electronics, and specifications**

### Monitoring Telemetry

1. **Robot must be running** (Play mode + simulation started)
2. **Open Telemetry Window**
3. **Click "Start Logging"**
4. **Watch real-time data** update automatically
5. **Click "Export to CSV"** to save data

---

## Tips and Best Practices

### Performance

- **Auto-refresh in Telemetry**: Disable if UI updates slow down editor
- **Large assemblies**: Monitor node count in Robot Builder
- **Play mode only**: Most features require Play mode active

### Data Management

- **Frequent saves**: Save robot configurations regularly
- **Naming convention**: Use descriptive names for robot saves
- **Telemetry exports**: Export before stopping Play mode (data clears)

### Development

- **Inspector debugging**: Select RobotController to see system stats
- **Console monitoring**: Check Unity Console for system messages
- **Parts validation**: Use Parts Library Browser to verify JSON loaded correctly

### Troubleshooting

**Issue**: Parts not showing in browser
- **Fix**: Ensure JSON files are in StreamingAssets/Parts/
- **Fix**: Enter Play mode to trigger parts library loading
- **Fix**: Check Unity Console for JSON parsing errors

**Issue**: Can't start simulation
- **Fix**: Ensure you're in Play mode
- **Fix**: Check RobotController exists in scene
- **Fix**: Verify assembly graph has nodes

**Issue**: Telemetry shows no data
- **Fix**: Start simulation first
- **Fix**: Click "Start Logging" in Telemetry window
- **Fix**: Ensure motors are connected in ElectroGraph

---

## Keyboard Shortcuts

Currently, FTC-SIM uses Unity's standard keyboard shortcuts:
- **Ctrl+P** (Windows) / **Cmd+P** (Mac): Enter/Exit Play mode
- **F12**: Screenshot (Unity default)

Custom shortcuts for FTC-SIM tools may be added in future updates.

---

## Next Steps

Now that you understand the editor integration:

1. **Read the API documentation** in GETTING_STARTED.md
2. **Experiment with SimpleRobotExample.cs**
3. **Create custom parts** using the JSON schema
4. **Build your own robot** using the programmatic API
5. **Monitor performance** with the Telemetry window

For detailed coding examples and API reference, see:
- `Assets/FTC-SIM/Documentation/GETTING_STARTED.md`
- `Assets/FTC-SIM/Documentation/ARCHITECTURE.md`
- `Assets/FTC-SIM/Examples/SimpleRobotExample.cs`

---

**Version**: 1.0 - Unity Editor Integration  
**Last Updated**: January 2025  
**For**: Unity 6.4+ HDRP
