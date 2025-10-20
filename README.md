# FTC-SIM - Interactive Robot Builder & Simulator

**Unity 6 HDRP | Windows-First | Educational Physics & Electronics**

FTC-SIM is an interactive robot-building game where players can design, wire, program, and simulate complex robots with real-world physics and electronics. Built for educational realism with approachable gameplay.

## 🎯 Features

### 🎨 In-Game Drag-and-Drop Builder (NEW!)
- **Three-Panel Layout**: Parts catalog (left), 3D preview (center), properties (right)
- **Visual Snapping**: Color-coded indicators show valid attachment points
- **Real-Time Configuration**: Adjust motor speeds and servo settings on the fly
- **Search & Filter**: Find parts quickly with search and category filtering
- **One-Click Setup**: Automated scene setup with `CompleteSceneSetup` script

### 🔩 Assembly System
- **goBILDA Native**: Accurate hole-pattern snapping and geometry
- **Infinite Nesting**: No hardcoded mechanisms - everything is modular
- **Smart Snapping**: Pattern-based attachment with validation
- **DAG Architecture**: Hierarchical assembly graph with cycle detection

### ⚡ ElectroGraph Electronics
- **Visual Node Editor**: Drag-and-drop electronics wiring interface
- **Color-Coded Pins**: PWR (red), GND (black), SIGNAL (blue), BUS (cyan)
- **Real-Time Validation**: Short circuit detection, overcurrent protection
- **Co-Simulation**: Physics and electronics simulate together
- **Power Modeling**: Voltage sag, wire resistance, back-EMF

### 🧠 Physics Simulation
- **ArticulationBodies**: Unity's high-fidelity joint system
- **Accurate Kinematics**: Real motor curves and gearbox efficiency
- **Wheel Physics**: Mecanum, gecko, and traction tire models
- **Fixed Timestep**: 0.5-1ms for stable contact and control

### 📊 Telemetry & Data
- **Real-Time Monitoring**: Voltage, current, power, temperature displays
- **Battery Visualization**: Color-coded battery level indicator
- **Per-Motor Telemetry**: RPM and torque for each motor
- **CSV Export**: Replayable runs and data analysis
- **Save/Load**: Complete robot assemblies with versioning

## 🚀 Getting Started

### Requirements
- Unity 6.4 or later (Alpha)
- Windows 10/11 (primary platform)
- Recommended: i9-14900HX + RTX 4080 for optimal performance

### Installation
1. Clone this repository
2. Open in Unity 6.4+
3. Load the main scene: `Assets/OutdoorsScene.unity`
4. Press Play to start the simulation

### Quick Start (Automated UI Setup)

**Option 1: Automatic Setup (2 minutes)**
1. Create empty GameObject in your scene
2. Add `CompleteSceneSetup` component
3. Right-click component → "Setup Complete Scene"
4. Press Play to start building!

**Option 2: Code Setup**
```csharp
// Get robot controller
var controller = FindObjectOfType<RobotController>();

// Load parts library
PartsLibrary.Instance.LoadPartsFromDirectory("StreamingAssets/Parts");

// Start simulation
controller.StartSimulation();
```

**See detailed setup instructions in**: `Assets/FTC-SIM/Documentation/QUICK_START_USER_GUIDE.md`

## 📦 Project Structure

```
Assets/FTC-SIM/
├── Core/              # Main controllers and telemetry
├── Assembly/          # Assembly graph and nodes
├── Parts/             # Parts library and definitions
├── Electronics/       # ElectroGraph system
├── Physics/           # Physics bridge to Unity
├── Snapping/          # goBILDA snapping service
├── UI/                # In-game builder UI components
│   └── PrefabCreators/  # UI prefab generation scripts
├── Editor/            # Unity Editor custom windows
├── Examples/          # Working examples and setup scripts
├── Data/              # Save/load and JSON schemas
└── Documentation/     # Architecture, setup guides, and API docs
```

## 🧩 Core Systems

### Assembly Architecture
Hierarchical DAG of robot components with infinite nesting support. No hardcoded mechanisms - all behavior emerges from parts and constraints.

### Parts Library
Data-driven part definitions in JSON. Ships with goBILDA catalog:
- Structural: C-Channels, plates, brackets
- Motion: Shafts, bearings, gearboxes
- Wheels: Mecanum, gecko, traction
- Actuators: DC motors, servos

### ElectroGraph
Visual node editor for electronics wiring:
- Typed pins (PWR, GND, SIGNAL, BUS)
- Wire validation and short detection
- Real-time voltage/current simulation
- Motor-to-physics bridging

### Physics Bridge
Connects assembly graph to Unity ArticulationBodies:
- Automatic joint configuration
- Motor torque control
- Back-EMF feedback
- Joint position/velocity telemetry

## 📖 Documentation

### Getting Started
- [Quick Start User Guide](Assets/FTC-SIM/Documentation/QUICK_START_USER_GUIDE.md) - **Start Here!**
- [UI Builder Setup Guide](Assets/FTC-SIM/Documentation/UI_BUILDER_SETUP_GUIDE.md) - Detailed setup instructions
- [In-Game Builder Guide](Assets/FTC-SIM/Documentation/IN_GAME_BUILDER_GUIDE.md) - How to use the builder

### Technical Documentation
- [Architecture Overview](Assets/FTC-SIM/Documentation/ARCHITECTURE.md)
- [Getting Started (API)](Assets/FTC-SIM/Documentation/GETTING_STARTED.md)
- [Unity Editor Guide](Assets/FTC-SIM/Documentation/UNITY_EDITOR_GUIDE.md)
- [UI/UX Implementation Summary](Assets/FTC-SIM/Documentation/UI_UX_IMPLEMENTATION_SUMMARY.md)

### Examples & Schemas
- [Part Definition Schema](Assets/FTC-SIM/Data/Schemas/)
- [ElectroGraph Examples](Assets/FTC-SIM/Data/Schemas/ExampleElectroGraph.json)
- [Complete Scene Setup Script](Assets/FTC-SIM/Examples/CompleteSceneSetup.cs)

## 🎮 Roadmap

### ✅ Vertical Slice + UI/UX (Current - v0.6)
- Core architecture implemented
- Assembly graph with snapping
- ElectroGraph visual node editor
- Physics bridge with ArticulationBodies
- Save/load system
- **In-game drag-and-drop builder UI**
- **Three-panel layout (catalog, preview, properties)**
- **Motor and servo configuration controls**
- **Real-time telemetry visualization**
- **Automated scene setup system**

### 🔜 Beta (v0.8)
- [ ] Expanded goBILDA library (50+ parts)
- [ ] 3D CAD model integration
- [ ] Belt/chain simulation
- [ ] IMU and encoder sensors
- [ ] Thermal modeling
- [ ] Block-based logic editor
- [ ] Undo/redo system

### 🎯 v1.0
- [ ] Complete goBILDA catalog
- [ ] Advanced wheel physics
- [ ] Full validation suite
- [ ] Tutorial system
- [ ] Robot sharing/import
- [ ] FTC field environments

## 🛠️ Development

### Adding Custom Parts
1. Create JSON part definition (see schema examples)
2. Place in `StreamingAssets/Parts/`
3. Define attachment points and physics properties
4. Optional: Add electronics profile for motors/sensors

### Extending Electronics
1. Add new `ElectroNode` type
2. Define pin configuration
3. Implement simulation behavior in `ElectroGraph`

## 🎨 Performance Targets

- **Windows**: 60 FPS @ ~3000 parts on i9-14900HX + RTX 4080
- **Physics**: 0.5-1ms fixed timestep
- **Electronics**: 200-1000 Hz control loop
- **Rendering**: HDRP with DLSS support

## 📝 License

See [LICENSE](LICENSE) file for details.

## 🤝 Contributing

This is an educational project. Contributions are welcome!

## 📧 Contact

Project maintained by JeriahNeal1
Repository: [github.com/JeriahNeal1/FTC-SIM](https://github.com/JeriahNeal1/FTC-SIM)

---

**Built with Unity 6 HDRP | Windows-First | Educational Realism**

