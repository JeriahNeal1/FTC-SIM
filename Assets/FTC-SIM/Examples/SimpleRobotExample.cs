using UnityEngine;
using FTCSIM.Core;
using FTCSIM.Assembly;
using FTCSIM.Electronics;
using FTCSIM.Parts;

namespace FTCSIM.Examples
{
    /// <summary>
    /// Example demonstrating how to programmatically build a simple robot.
    /// This creates a basic drivetrain with 4 motors connected to a battery.
    /// </summary>
    public class SimpleRobotExample : MonoBehaviour
    {
        private RobotController controller;
        
        void Start()
        {
            // Find or create the robot controller
            controller = FindObjectOfType<RobotController>();
            if (controller == null)
            {
                GameObject controllerObj = new GameObject("RobotController");
                controller = controllerObj.AddComponent<RobotController>();
            }
            
            // Wait a frame for initialization
            Invoke("BuildRobot", 0.1f);
        }
        
        void BuildRobot()
        {
            Debug.Log("=== Building Simple Robot Example ===");
            
            // 1. Create Assembly Structure
            BuildAssembly();
            
            // 2. Create Electronics
            BuildElectronics();
            
            // 3. Link electronics to physical parts
            LinkElectronicsToAssembly();
            
            Debug.Log("=== Robot Built Successfully ===");
            Debug.Log("Press SPACE to start simulation");
        }
        
        void BuildAssembly()
        {
            Debug.Log("Building assembly structure...");
            
            var graph = controller.assemblyGraph;
            var root = graph.rootNode;
            
            // Create chassis (C-Channel base)
            var chassis = new AssemblyNode("chassis_1", "Main Chassis", false);
            chassis.partSKU = "GB-1120-288-xxx";
            chassis.mass = 0.432f;
            chassis.localPosition = Vector3.zero;
            graph.AddNode(root, chassis);
            
            // Create motor nodes
            string[] motorPositions = { "FrontLeft", "FrontRight", "BackLeft", "BackRight" };
            Vector3[] motorOffsets = {
                new Vector3(-0.15f, 0, 0.15f),   // Front Left
                new Vector3(0.15f, 0, 0.15f),    // Front Right
                new Vector3(-0.15f, 0, -0.15f),  // Back Left
                new Vector3(0.15f, 0, -0.15f)    // Back Right
            };
            
            for (int i = 0; i < 4; i++)
            {
                var motor = new AssemblyNode($"motor_{i}", $"Motor {motorPositions[i]}", false);
                motor.partSKU = "GB-YJ-3530-5201-xxx";
                motor.mass = 0.250f;
                motor.localPosition = motorOffsets[i];
                graph.AddNode(chassis, motor);
                
                // Create relation (motor is mounted to chassis with revolute joint)
                var relation = new AssemblyRelation(
                    chassis.id, 
                    motor.id, 
                    "mount_body", 
                    "mount_body", 
                    AssemblyRelation.RelationType.Revolute
                );
                chassis.relations.Add(relation);
            }
            
            Debug.Log($"Created assembly with {graph.rootNode.children.Count} subsystems");
        }
        
        void BuildElectronics()
        {
            Debug.Log("Building electronics graph...");
            
            var electro = controller.electroGraph;
            
            // Create battery
            var battery = new ElectroNode("battery_main", "Battery");
            battery.pins.Add(new ElectroPin("pos", ElectroPin.PinRole.PWR) { voltage = 12f, maxCurrent = 20f });
            battery.pins.Add(new ElectroPin("neg", ElectroPin.PinRole.GND) { voltage = 0f, maxCurrent = 20f });
            electro.AddNode(battery);
            
            // Create expansion hub
            var hub = new ElectroNode("hub_expansion", "Hub");
            hub.pins.Add(new ElectroPin("pwr_in", ElectroPin.PinRole.PWR) { maxCurrent = 15f });
            hub.pins.Add(new ElectroPin("gnd_in", ElectroPin.PinRole.GND) { maxCurrent = 15f });
            
            // Add motor ports
            for (int i = 0; i < 4; i++)
            {
                hub.pins.Add(new ElectroPin($"motor_{i}_plus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
                hub.pins.Add(new ElectroPin($"motor_{i}_minus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
            }
            electro.AddNode(hub);
            
            // Create motors
            for (int i = 0; i < 4; i++)
            {
                var motor = new ElectroNode($"motor_{i}", "DCMotor");
                motor.pins.Add(new ElectroPin("plus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
                motor.pins.Add(new ElectroPin("minus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
                electro.AddNode(motor);
            }
            
            // Wire battery to hub
            var wireBattHub = new ElectroWire(
                "wire_batt_hub_pwr",
                ElectroWire.WireType.Power,
                "battery_main", "pos",
                "hub_expansion", "pwr_in"
            );
            electro.AddWire(wireBattHub);
            
            var wireBattHubGnd = new ElectroWire(
                "wire_batt_hub_gnd",
                ElectroWire.WireType.Ground,
                "battery_main", "neg",
                "hub_expansion", "gnd_in"
            );
            electro.AddWire(wireBattHubGnd);
            
            // Wire hub to motors
            for (int i = 0; i < 4; i++)
            {
                var wirePlus = new ElectroWire(
                    $"wire_hub_motor{i}_plus",
                    ElectroWire.WireType.Power,
                    "hub_expansion", $"motor_{i}_plus",
                    $"motor_{i}", "plus"
                );
                electro.AddWire(wirePlus);
                
                var wireMinus = new ElectroWire(
                    $"wire_hub_motor{i}_minus",
                    ElectroWire.WireType.Power,
                    "hub_expansion", $"motor_{i}_minus",
                    $"motor_{i}", "minus"
                );
                electro.AddWire(wireMinus);
            }
            
            Debug.Log($"Created electronics with {electro.nodes.Count} nodes and {electro.wires.Count} wires");
        }
        
        void LinkElectronicsToAssembly()
        {
            Debug.Log("Linking electronics to assembly...");
            
            // Link motor electronics nodes to their physical assembly nodes
            for (int i = 0; i < 4; i++)
            {
                var electroNode = controller.electroGraph.GetNode($"motor_{i}");
                if (electroNode != null)
                {
                    electroNode.assemblyNodeId = $"motor_{i}";
                }
            }
            
            Debug.Log("Electronics linked to assembly");
        }
        
        void Update()
        {
            // Press Space to start simulation
            if (Input.GetKeyDown(KeyCode.Space) && !controller.isSimulating)
            {
                Debug.Log("Starting simulation...");
                controller.StartSimulation();
            }
            
            // Press S to save robot
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Saving robot...");
                controller.SaveRobot("SimpleRobotExample");
            }
            
            // Press Escape to stop simulation
            if (Input.GetKeyDown(KeyCode.Escape) && controller.isSimulating)
            {
                Debug.Log("Stopping simulation...");
                controller.StopSimulation();
            }
        }
    }
}
