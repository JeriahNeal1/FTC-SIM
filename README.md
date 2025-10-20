# FTC-SIM - Interactive Robot Builder & Simulator

**Unity 6 HDRP | Windows-First | Educational Physics & Electronics**

FTC-SIM is an interactive robot-building game where players can design, wire, program, and simulate complex robots with real-world physics and electronics. Built for educational realism with approachable gameplay.

## 🎯 Features

### 🔩 Assembly System
- **goBILDA Native**: Accurate hole-pattern snapping and geometry
- **Infinite Nesting**: No hardcoded mechanisms - everything is modular
- **Smart Snapping**: Pattern-based attachment with validation
- **DAG Architecture**: Hierarchical assembly graph with cycle detection

### ⚡ ElectroGraph Electronics
- **Node-Based Wiring**: Visual editor for electronics connections
- **Real-Time Validation**: Short circuit detection, overcurrent protection
- **Co-Simulation**: Physics and electronics simulate together
- **Power Modeling**: Voltage sag, wire resistance, back-EMF

### 🧠 Physics Simulation
- **ArticulationBodies**: Unity's high-fidelity joint system
- **Accurate Kinematics**: Real motor curves and gearbox efficiency
- **Wheel Physics**: Mecanum, gecko, and traction tire models
- **Fixed Timestep**: 0.5-1ms for stable contact and control

### 📊 Telemetry & Data
- **Real-Time Charts**: Voltage, current, RPM, temperature
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

### Quick Start
```csharp
// Get robot controller
var controller = FindObjectOfType<RobotController>();

// Load parts library
PartsLibrary.Instance.LoadPartsFromDirectory("StreamingAssets/Parts");

// Start simulation
controller.StartSimulation();
```

## 📦 Project Structure

```
Assets/FTC-SIM/
├── Core/              # Main controllers and telemetry
├── Assembly/          # Assembly graph and nodes
├── Parts/             # Parts library and definitions
├── Electronics/       # ElectroGraph system
├── Physics/           # Physics bridge to Unity
├── Snapping/          # goBILDA snapping service
├── Data/              # Save/load and JSON schemas
└── Documentation/     # Architecture and API docs
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

- [Architecture Overview](Assets/FTC-SIM/Documentation/ARCHITECTURE.md)
- [Part Definition Schema](Assets/FTC-SIM/Data/Schemas/)
- [ElectroGraph Examples](Assets/FTC-SIM/Data/Schemas/ExampleElectroGraph.json)

## 🎮 Roadmap

### ✅ Vertical Slice (Current)
- Core architecture implemented
- Assembly graph with snapping
- ElectroGraph MVP
- Physics bridge with ArticulationBodies
- Save/load system

### 🔜 Beta (v0.8)
- [ ] Expanded goBILDA library
- [ ] Belt/chain simulation
- [ ] IMU and encoder sensors
- [ ] Thermal modeling
- [ ] Block-based logic editor
- [ ] Unity Editor UI tools

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

