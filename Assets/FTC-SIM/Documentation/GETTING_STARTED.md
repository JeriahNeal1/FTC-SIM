# Getting Started with FTC-SIM

This guide will help you get started with the FTC-SIM robot builder and simulator.

## Installation

1. **Prerequisites**
   - Unity 6.4 or later (Alpha release)
   - Windows 10/11 (primary platform)
   - Git for version control

2. **Clone the Repository**
   ```bash
   git clone https://github.com/JeriahNeal1/FTC-SIM.git
   cd FTC-SIM
   ```

3. **Open in Unity**
   - Launch Unity Hub
   - Click "Open" and select the FTC-SIM folder
   - Unity will import all assets (this may take a few minutes)

## Quick Start - Running the Example

1. **Load the Example Scene**
   - In the Unity Editor, navigate to `Assets/OutdoorsScene.unity`
   - Double-click to open the scene

2. **Add the Example Script**
   - Create an empty GameObject (Right-click in Hierarchy → Create Empty)
   - Name it "SimpleRobotExample"
   - Add the `SimpleRobotExample.cs` script from `Assets/FTC-SIM/Examples/`
   - Press Play

3. **Test the Simulation**
   - The robot will be built automatically when you press Play
   - Press **SPACE** to start the physics simulation
   - Press **S** to save the robot
   - Press **ESC** to stop the simulation

## Building Your First Robot

### Step 1: Create the Robot Controller

```csharp
using FTCSIM.Core;
using UnityEngine;

public class MyRobotBuilder : MonoBehaviour
{
    void Start()
    {
        // Get or create robot controller
        var controller = FindObjectOfType<RobotController>();
        if (controller == null)
        {
            GameObject obj = new GameObject("RobotController");
            controller = obj.AddComponent<RobotController>();
        }
    }
}
```

### Step 2: Create Assembly Structure

```csharp
using FTCSIM.Assembly;

void BuildAssembly()
{
    var graph = controller.assemblyGraph;
    var root = graph.rootNode;
    
    // Create a chassis
    var chassis = new AssemblyNode("chassis_1", "Main Chassis", false);
    chassis.partSKU = "GB-1120-288-xxx"; // goBILDA C-Channel
    chassis.mass = 0.432f;
    graph.AddNode(root, chassis);
    
    // Add a motor
    var motor = new AssemblyNode("motor_1", "Drive Motor", false);
    motor.partSKU = "GB-YJ-3530-5201-xxx"; // Yellow Jacket Motor
    motor.mass = 0.250f;
    motor.localPosition = new Vector3(0.1f, 0, 0);
    graph.AddNode(chassis, motor);
}
```

### Step 3: Wire Electronics

```csharp
using FTCSIM.Electronics;

void BuildElectronics()
{
    var electro = controller.electroGraph;
    
    // Create battery
    var battery = new ElectroNode("battery", "Battery");
    battery.pins.Add(new ElectroPin("pos", ElectroPin.PinRole.PWR) 
        { voltage = 12f, maxCurrent = 20f });
    battery.pins.Add(new ElectroPin("neg", ElectroPin.PinRole.GND) 
        { voltage = 0f, maxCurrent = 20f });
    electro.AddNode(battery);
    
    // Create motor
    var motor = new ElectroNode("motor_1", "DCMotor");
    motor.pins.Add(new ElectroPin("plus", ElectroPin.PinRole.PWR) 
        { maxCurrent = 10f });
    motor.pins.Add(new ElectroPin("minus", ElectroPin.PinRole.PWR) 
        { maxCurrent = 10f });
    motor.assemblyNodeId = "motor_1"; // Link to assembly
    electro.AddNode(motor);
    
    // Wire them together
    var wirePlus = new ElectroWire("wire_1", ElectroWire.WireType.Power,
        "battery", "pos", "motor_1", "plus");
    electro.AddWire(wirePlus);
}
```

### Step 4: Start Simulation

```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        controller.StartSimulation();
    }
}
```

## Understanding the Architecture

### Assembly System
- **AssemblyGraph**: Hierarchical structure of your robot
- **AssemblyNode**: Individual parts or subassemblies
- **AssemblyRelation**: How parts connect (Fixed, Revolute, Prismatic)

### Electronics System
- **ElectroGraph**: Circuit diagram of your robot
- **ElectroNode**: Devices (battery, motor, sensor, hub)
- **ElectroWire**: Connections between device pins
- **ElectroPin**: Terminals with types (PWR, GND, SIGNAL, BUS)

### Physics System
- **PhysicsBridge**: Converts assembly to Unity ArticulationBodies
- Supports realistic joint simulation
- Motor torque is calculated from voltage/current

### Parts Library
- JSON-based part definitions
- goBILDA standard supported
- Extensible for custom parts

## Working with Parts

### Loading the Parts Library

```csharp
using FTCSIM.Parts;

void Start()
{
    string partsPath = Path.Combine(
        Application.streamingAssetsPath, "Parts");
    PartsLibrary.Instance.LoadPartsFromDirectory(partsPath);
}
```

### Searching for Parts

```csharp
// Search by category
var motors = PartsLibrary.Instance.GetPartsByCategory("Actuators");

// Search by name/SKU
var results = PartsLibrary.Instance.SearchParts("yellow jacket");

// Get specific part
var motor = PartsLibrary.Instance.GetPart("GB-YJ-3530-5201-xxx");
```

### Creating Custom Parts

1. Create a JSON file in `StreamingAssets/Parts/`:

```json
{
  "sku": "CUSTOM-001",
  "vendor": "Custom",
  "displayName": "My Custom Part",
  "category": "Structural",
  "mass": 0.100,
  "mounts": [
    {
      "id": "mount_0",
      "pattern": "Pattern:goBILDA-16mm",
      "localPosition": { "x": 0.0, "y": 0.0, "z": 0.0 },
      "localRotation": { "x": 0.0, "y": 0.0, "z": 0.0, "w": 1.0 }
    }
  ],
  "colliders": [{ "source": "convexHull", "margin": 0.002 }],
  "rules": { "validMates": ["Pattern:goBILDA-16mm"] }
}
```

2. Reload the parts library

## Saving and Loading Robots

### Save

```csharp
controller.SaveRobot("MyRobot");
// Saves to: Application.persistentDataPath/SavedRobots/
```

### Load

```csharp
string path = Path.Combine(
    Application.persistentDataPath, 
    "SavedRobots", 
    "MyRobot_20250120_123456.json");
controller.LoadRobot(path);
```

## Telemetry and Data Logging

### Enable Telemetry

```csharp
var telemetry = controller.GetComponent<TelemetrySystem>();
if (telemetry == null)
{
    telemetry = controller.gameObject.AddComponent<TelemetrySystem>();
}
telemetry.StartLogging();
```

### Export Data

```csharp
telemetry.StopLogging();
string csvPath = Path.Combine(
    Application.persistentDataPath, 
    "telemetry.csv");
telemetry.ExportToCSV(csvPath);
```

### Access Real-Time Data

```csharp
var data = telemetry.GetLatestData();
Debug.Log($"Voltage: {data.voltage}V");
Debug.Log($"Current: {data.current}A");
Debug.Log($"Power: {data.power}W");
```

## Next Steps

- **Read the Architecture Documentation**: `Assets/FTC-SIM/Documentation/ARCHITECTURE.md`
- **Explore Example Schemas**: `Assets/FTC-SIM/Data/Schemas/`
- **Study the Example Robot**: `Assets/FTC-SIM/Examples/SimpleRobotExample.cs`
- **Create Custom Parts**: Add JSON definitions to `StreamingAssets/Parts/`
- **Build a Drivetrain**: Follow the example to create a 4-wheel drive robot

## Troubleshooting

### Issue: Parts not loading
- Ensure JSON files are in `StreamingAssets/Parts/`
- Check Unity Console for parsing errors
- Validate JSON syntax

### Issue: Simulation not starting
- Check that assembly has at least one node
- Verify electronics are properly wired
- Look for validation errors in Console

### Issue: Motors not spinning
- Ensure motor electronics node is linked to assembly node
- Check battery voltage is > 0
- Verify wires connect battery to motors

## Support

- **Documentation**: See `Assets/FTC-SIM/Documentation/`
- **Issues**: Report bugs on GitHub
- **Repository**: [github.com/JeriahNeal1/FTC-SIM](https://github.com/JeriahNeal1/FTC-SIM)

Happy building! 🤖
