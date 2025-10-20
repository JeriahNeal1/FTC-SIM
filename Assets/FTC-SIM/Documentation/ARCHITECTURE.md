# FTC-SIM Architecture Documentation

## Overview

FTC-SIM is an interactive robot builder and simulator built in Unity 6 HDRP for Windows. It allows users to design, wire, program, and simulate complex robots with real-world physics and electronics.

## Core Systems

### 1. Assembly Architecture (`Assets/FTC-SIM/Assembly/`)

The assembly system manages the hierarchical structure of robot components using a Directed Acyclic Graph (DAG).

**Key Classes:**
- `AssemblyNode`: Represents a single component or subassembly
- `AssemblyGraph`: Manages the entire assembly hierarchy
- `AssemblyRelation`: Defines connections between nodes (Fixed, Revolute, Prismatic, Shaft)

**Features:**
- Infinite nesting of subassemblies
- Automatic mass/inertia aggregation
- Cycle detection for DAG validation
- Support for multiple connection types

### 2. Parts Library (`Assets/FTC-SIM/Parts/`)

Data-driven part definition system supporting the goBILDA modular standard.

**Key Classes:**
- `PartDefinition`: JSON-serializable part definition
- `PartsLibrary`: Manages part catalog with search/filter capabilities

**Part Definition Schema:**
```json
{
  "sku": "GB-XXXX-XXX",
  "vendor": "goBILDA",
  "displayName": "Part Name",
  "category": "Structural|Motion|Wheels|Actuators",
  "mass": 0.0,
  "mounts": [...],
  "rotationalMounts": [...],
  "colliders": [...],
  "rules": {...},
  "electronicsProfile": {...}
}
```

### 3. ElectroGraph System (`Assets/FTC-SIM/Electronics/`)

Node-based electronics simulation with validation and real-time co-simulation.

**Key Classes:**
- `ElectroNode`: Device in the electronics graph (battery, hub, motor, sensor)
- `ElectroPin`: Typed terminal (PWR, GND, SIGNAL, BUS)
- `ElectroWire`: Connection between pins with type and resistance
- `ElectroGraph`: Manages graph and runs simulation

**Features:**
- Pin type validation
- Short circuit detection
- Voltage/current propagation
- Wire resistance modeling
- JSON import/export

### 4. Physics Bridge (`Assets/FTC-SIM/Physics/`)

Connects assembly graph to Unity's ArticulationBody physics system.

**Key Classes:**
- `PhysicsBridge`: Creates and manages ArticulationBody hierarchy

**Features:**
- Automatic ArticulationBody tree generation
- Joint configuration (Fixed, Revolute, Prismatic)
- Motor torque control
- Joint position/velocity feedback
- Back-EMF simulation

### 5. Snapping Service (`Assets/FTC-SIM/Snapping/`)

Intelligent snapping for goBILDA pattern-based assembly.

**Key Classes:**
- `SnappingService`: Detects compatible attachment points
- `SnapCandidate`: Represents a potential snap target

**Features:**
- Pattern compatibility checking
- Distance and angle tolerance
- Ranked candidate list
- Visual snap preview

### 6. Data Persistence (`Assets/FTC-SIM/Data/`)

Save/load system for robot assemblies and electronics.

**Key Classes:**
- `DataPersistence`: Handles JSON serialization/deserialization
- `RobotData`: Complete robot save format

**Features:**
- Versioned JSON format
- Complete assembly + electronics export
- Timestamped saves
- File management utilities

### 7. Core Controllers (`Assets/FTC-SIM/Core/`)

Main coordination and simulation systems.

**Key Classes:**
- `RobotController`: Master controller coordinating all systems
- `TelemetrySystem`: Real-time monitoring and data logging
- `AttachmentPoint`: Defines mounting locations
- `RotationalMount`: Defines shaft/bearing connections

**Features:**
- Coordinated simulation lifecycle
- Electronics-to-physics bridging
- Configurable timesteps
- Telemetry capture and CSV export

## Simulation Architecture

### Time Steps

- **Physics**: 0.5-1ms fixed timestep (Unity FixedUpdate)
- **Electronics**: 0.5-1ms substeps (oversampled control loop)
- **Telemetry**: Configurable (default 10Hz)

### Co-Simulation Flow

1. **Electronics Update** (high frequency)
   - Simulate voltage/current distribution
   - Calculate motor voltages
   - Check overcurrent conditions

2. **Physics Bridge**
   - Convert motor voltages to torques
   - Apply to ArticulationBodies
   - Read back joint velocities

3. **Feedback**
   - Calculate back-EMF from velocities
   - Update motor currents
   - Propagate to power rails

4. **Telemetry**
   - Capture voltage, current, RPM
   - Log temperature estimates
   - Export to CSV

## Extension Points

### Custom Parts

1. Create JSON part definition
2. Place in `StreamingAssets/Parts/`
3. Define attachment points and mounts
4. Optional: Add electronics profile

### Custom Electronics Devices

1. Extend `ElectroNode` type
2. Define pin configuration
3. Implement simulation behavior
4. Register in ElectroGraph

### Custom Validators

- Assembly constraint validation
- Electronics validation rules
- Physics constraint checks

## Performance Considerations

### Targets
- **Windows**: 60 FPS @ 3000 parts on i9-14900HX + RTX 4080
- **Physics**: ArticulationBody with 0.5-1ms timestep
- **Rendering**: HDRP with DLSS support

### Optimizations
- LOD colliders for complex assemblies
- Dynamic sleeping for static components
- Async physics baking
- Efficient graph traversal

## File Structure

```
Assets/FTC-SIM/
├── Core/
│   ├── AttachmentPoint.cs
│   ├── RobotController.cs
│   └── TelemetrySystem.cs
├── Assembly/
│   ├── AssemblyNode.cs
│   └── AssemblyGraph.cs
├── Parts/
│   ├── PartDefinition.cs
│   └── PartsLibrary.cs
├── Electronics/
│   ├── ElectroNode.cs
│   └── ElectroGraph.cs
├── Physics/
│   └── PhysicsBridge.cs
├── Snapping/
│   └── SnappingService.cs
├── Data/
│   ├── DataPersistence.cs
│   └── Schemas/
│       ├── ExamplePart_CChannel.json
│       ├── ExamplePart_Motor.json
│       └── ExampleElectroGraph.json
└── Documentation/
    └── ARCHITECTURE.md (this file)
```

## Usage Example

```csharp
// Initialize robot controller
var controller = FindObjectOfType<RobotController>();

// Load parts library
PartsLibrary.Instance.LoadPartsFromDirectory("StreamingAssets/Parts");

// Create a simple assembly
var rootNode = controller.assemblyGraph.rootNode;
var channelNode = new AssemblyNode("channel_1", "C-Channel", false);
channelNode.partSKU = "GB-1120-288-xxx";
controller.assemblyGraph.AddNode(rootNode, channelNode);

// Start simulation
controller.StartSimulation();

// Save robot
controller.SaveRobot("MyRobot");
```

## Next Steps

### Vertical Slice Milestones
1. ✅ Core architecture implementation
2. ⬜ Unity Editor UI for assembly
3. ⬜ Visual node editor for ElectroGraph
4. ⬜ 4-wheel drivetrain example
5. ⬜ Telemetry visualization

### Beta Features
- Expanded goBILDA library
- Belt/chain simulation
- IMU and encoder sensors
- Thermal modeling
- Block-based logic editor

### v1.0 Features
- Complete goBILDA catalog
- Advanced wheel physics (mecanum, gecko)
- Full validation suite
- Tutorial system
- Robot sharing/import

## License

See LICENSE file in repository root.
