# In-Game Robot Builder Guide

## Overview

The in-game robot builder provides a complete 3D drag-and-drop interface for building robots within the game camera view. This guide explains how to use the builder and all its features.

## UI Layout

The robot builder uses a three-panel layout:

```
┌──────────────────────────────────────────────────────┐
│                                                      │
│  [Parts Catalog]    [3D Preview]    [Properties]   │
│      (Left)          (Center)          (Right)      │
│                                                      │
└──────────────────────────────────────────────────────┘
```

### Left Panel: Parts Catalog
- **Search bar**: Filter parts by name or SKU
- **Category dropdown**: Filter by part category
- **Part list**: Scrollable list of available parts
- **Part info**: Name, category, and mass displayed for each part

### Center Panel: 3D Preview
- **Robot view**: Live 3D visualization of your robot
- **Drag-and-drop**: Click and drag parts from catalog to place them
- **Snapping**: Parts automatically snap to valid attachment points
- **Selection**: Click parts to select and configure them

### Right Panel: Properties
- **Part info**: Name and details of selected part
- **Motor controls**: Speed sliders for DC motors
- **Servo configuration**: Position, speed, and torque settings
- **Real-time updates**: Changes apply immediately

## Building Your Robot

### Step 1: Starting the Builder

1. **Enter Play Mode** in Unity (press ▶)
2. **Ensure RobotController exists** in your scene
3. **The builder UI** should appear automatically

### Step 2: Adding Parts

1. **Browse the parts catalog** on the left panel
2. **Click on a part** you want to add
3. **The part becomes a "ghost"** that follows your mouse
4. **Move your mouse** over the 3D preview area
5. **Watch for snap indicators**:
   - **Green glow** = Valid snap point found
   - **Red glow** = No valid snap point
6. **Click to place** the part when happy with position

### Step 3: Using Snapping

The snapping system helps you attach parts correctly:

**Automatic Snapping**:
- Parts automatically detect nearby attachment points
- Compatible patterns (goBILDA 16mm, etc.) snap together
- Distance and angle tolerances ensure proper alignment

**Visual Feedback**:
- **Ghost part color**: Green when near valid snap, red otherwise
- **Snap indicator**: Appears at the exact snap location
- **Coordinate gizmos**: Show orientation at attachment points

**Manual Placement**:
- If no snap point is found, parts can be placed freely
- They become children of the root assembly node

### Step 4: Configuring Parts

After placing a part:

1. **Click on the part** in the 3D view to select it
2. **The properties panel** updates with part-specific controls
3. **For motors**:
   - Adjust speed slider (0-100%)
   - Toggle reverse direction
   - Changes apply to simulation in real-time
4. **For servos**:
   - Set target position (0-180°)
   - Adjust speed (0-100%)
   - Configure torque ratio (0-100%)

### Step 5: Organizing Your Robot

**Hierarchy**:
- Parts snap to create a parent-child hierarchy
- This hierarchy is visible in the Robot Builder Window
- Root node contains all top-level assemblies

**Subassemblies**:
- Complex structures can be organized as subassemblies
- Helps manage large robots
- Improves performance

## ElectroGraph Visual Editor

### Opening the Editor

1. **From the game UI**: Click "Electronics" button
2. **Or use menu**: `FTC-SIM > ElectroGraph Editor`

### Adding Devices

1. **Click "Add Node"** button
2. **Select device type** from dropdown:
   - Battery (power source)
   - Hub (controller)
   - DCMotor (drive motor)
   - Servo (position control)
   - Sensor (input device)
3. **Device appears** in the canvas center

### Moving Nodes

- **Click and drag** nodes to reposition them
- **Organize logically**: Battery → Hub → Motors/Servos

### Connecting Devices

1. **Click on a pin** of the first device
2. **Click on a pin** of the second device
3. **A wire appears** connecting the two pins
4. **Wire color** indicates type:
   - Red = Power
   - Black = Ground
   - Blue = Signal
   - Cyan = Bus

### Pin Types

**PWR (Power)**: Red pins, carry voltage
**GND (Ground)**: Black pins, ground reference
**SIGNAL (Signal)**: Blue pins, control/data signals
**BUS (Bus)**: Cyan pins, communication buses

### Validation

The editor automatically validates connections:
- ✅ Compatible pin types connect successfully
- ❌ Incompatible types show error
- ⚠️ Short circuits are prevented
- ⚠️ Overcurrent warnings appear

## Motor and Servo Controls

### DC Motor Control

**Speed Slider** (0-100%):
- 0% = stopped
- 50% = half speed
- 100% = full speed

**Reverse Toggle**:
- Unchecked = forward direction
- Checked = reverse direction

**Real-Time Application**:
- Changes apply immediately to simulation
- Motor torque calculated from speed setting
- Back-EMF affects voltage in simulation

### Servo Configuration

**Position Slider** (0-180°):
- Sets target angle for servo
- 90° = center position
- Servo moves toward target

**Speed Slider** (0-100%):
- Controls how fast servo moves
- Higher = faster movement
- Affects torque available

**Torque Slider** (0-100%):
- Speed-torque tradeoff
- Higher torque = slower but stronger
- Lower torque = faster but weaker

**Combined Effect**:
- Speed × Torque = effective servo power
- Tune for your application needs

## Keyboard Shortcuts

While building:
- **ESC**: Cancel current drag operation
- **Delete**: Remove selected part (coming soon)
- **Ctrl+Z**: Undo last action (coming soon)
- **Ctrl+S**: Save robot

While simulating:
- **Space**: Start/stop simulation
- **R**: Reset robot position
- **Tab**: Toggle UI visibility

## Tips and Best Practices

### Building Tips

1. **Start with structure**: Place chassis and frame parts first
2. **Add motors next**: Attach motors to structural parts
3. **Connect electronics**: Wire battery → hub → motors
4. **Test incrementally**: Test after adding each major component
5. **Use snapping**: Let the system guide you to valid connections

### Performance Tips

1. **Minimize part count**: Use larger structural parts when possible
2. **Organize as subassemblies**: Group related parts together
3. **Efficient wiring**: Minimize wire count in ElectroGraph
4. **Test early**: Catch issues before robot becomes complex

### Design Tips

1. **Center of mass**: Balance weight distribution
2. **Motor placement**: Position for optimal torque
3. **Wire management**: Keep electrical runs short
4. **Accessibility**: Leave room for maintenance

## Troubleshooting

### Parts won't snap
- **Check compatibility**: Verify pattern types match
- **Adjust distance**: Move closer to target snap point
- **Check angle**: Rotate part to align with target
- **Look for gizmo**: Green indicator shows valid snap

### Motors not working
- **Check wiring**: Verify battery → hub → motor connections
- **Check simulation**: Ensure simulation is running
- **Check power**: Battery should show voltage
- **Check control**: Motor speed slider should be > 0

### UI not responding
- **Check Play mode**: UI only works in Play mode
- **Check RobotController**: Must exist in scene
- **Check camera**: Build camera must be active
- **Restart**: Exit and re-enter Play mode

### Performance issues
- **Reduce parts**: Simplify robot design
- **Close panels**: Hide unused UI panels
- **Lower quality**: Adjust graphics settings
- **Check telemetry**: High update rates may slow UI

## Advanced Features

### Custom Snap Points

When creating custom parts:
1. Define attachment points in JSON
2. Specify pattern type (e.g., "Pattern:goBILDA-16mm")
3. Set position and rotation in local space
4. Set tolerance for snap distance

### Electronics Validation

The ElectroGraph validates:
- Pin compatibility (PWR→PWR, GND→GND, etc.)
- Short circuit prevention (PWR→GND blocked)
- Current limits (overcurrent warnings)
- Voltage levels (power distribution)

### Physics Integration

The builder integrates with physics:
- Parts generate ArticulationBodies
- Joints configured from AssemblyRelations
- Mass and inertia calculated automatically
- Motor torque applied from speed settings

## Next Steps

- **Read GETTING_STARTED.md** for API details
- **Check ARCHITECTURE.md** for system design
- **Try UNITY_EDITOR_GUIDE.md** for editor tools
- **Experiment with examples** in Examples folder

---

**Version**: 1.0 - In-Game Builder  
**Last Updated**: January 2025  
**For**: Unity 6.4+ HDRP
