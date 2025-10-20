using UnityEngine;
using FTCSIM.Assembly;
using FTCSIM.Electronics;
using FTCSIM.Physics;
using FTCSIM.Snapping;
using FTCSIM.Parts;
using FTCSIM.Data;

namespace FTCSIM.Core
{
    /// <summary>
    /// Main controller for the FTC-SIM robot builder.
    /// Coordinates all subsystems and manages the simulation lifecycle.
    /// </summary>
    public class RobotController : MonoBehaviour
    {
        [Header("System References")]
        public AssemblyGraph assemblyGraph;
        public ElectroGraph electroGraph;
        public PhysicsBridge physicsBridge;
        public SnappingService snappingService;
        public PartsLibrary partsLibrary;
        public DataPersistence dataPersistence;
        public TelemetrySystem telemetrySystem;
        
        [Header("Simulation Settings")]
        public bool isSimulating = false;
        public float fixedTimeStep = 0.001f; // 1ms
        public float electroSimTimeStep = 0.0005f; // 0.5ms
        
        private float electroSimAccumulator = 0f;
        
        private void Awake()
        {
            InitializeSystems();
        }
        
        private void InitializeSystems()
        {
            // Create system components if they don't exist
            if (assemblyGraph == null)
            {
                GameObject graphObj = new GameObject("AssemblyGraph");
                graphObj.transform.SetParent(transform);
                assemblyGraph = graphObj.AddComponent<AssemblyGraph>();
            }
            
            if (electroGraph == null)
            {
                GameObject electroObj = new GameObject("ElectroGraph");
                electroObj.transform.SetParent(transform);
                electroGraph = electroObj.AddComponent<ElectroGraph>();
            }
            
            if (physicsBridge == null)
            {
                GameObject physicsObj = new GameObject("PhysicsBridge");
                physicsObj.transform.SetParent(transform);
                physicsBridge = physicsObj.AddComponent<PhysicsBridge>();
            }
            
            if (snappingService == null)
            {
                GameObject snapObj = new GameObject("SnappingService");
                snapObj.transform.SetParent(transform);
                snappingService = snapObj.AddComponent<SnappingService>();
            }
            
            if (dataPersistence == null)
            {
                GameObject dataObj = new GameObject("DataPersistence");
                dataObj.transform.SetParent(transform);
                dataPersistence = dataObj.AddComponent<DataPersistence>();
            }
            
            if (telemetrySystem == null)
            {
                GameObject telemetryObj = new GameObject("TelemetrySystem");
                telemetryObj.transform.SetParent(transform);
                telemetrySystem = telemetryObj.AddComponent<TelemetrySystem>();
            }
            
            // Initialize systems
            assemblyGraph.Initialize();
            electroGraph.Initialize();
            physicsBridge.Initialize(assemblyGraph);
            snappingService.Initialize(assemblyGraph, PartsLibrary.Instance);
            
            // Set physics timestep
            Time.fixedDeltaTime = fixedTimeStep;
            
            Debug.Log("FTC-SIM Robot Controller initialized");
        }
        
        private void Start()
        {
            // Load parts library
            string partsPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Parts");
            PartsLibrary.Instance.LoadPartsFromDirectory(partsPath);
        }
        
        private void FixedUpdate()
        {
            if (!isSimulating) return;
            
            // Simulate electronics at higher frequency
            electroSimAccumulator += Time.fixedDeltaTime;
            while (electroSimAccumulator >= electroSimTimeStep)
            {
                electroGraph.SimulateStep(electroSimTimeStep);
                electroSimAccumulator -= electroSimTimeStep;
            }
            
            // Update physics bridge (happens at Unity's fixed timestep)
            UpdateMotorControls();
        }
        
        private void UpdateMotorControls()
        {
            // Bridge electronics to physics
            // Find motor nodes and apply torque based on voltage/current
            foreach (var node in electroGraph.nodes)
            {
                if (node.type == "DCMotor" && !string.IsNullOrEmpty(node.assemblyNodeId))
                {
                    // Calculate motor torque from voltage and current
                    var plusPin = node.pins.Find(p => p.id == "plus" || p.id == "motor_plus");
                    var minusPin = node.pins.Find(p => p.id == "minus" || p.id == "motor_minus");
                    
                    if (plusPin != null && minusPin != null)
                    {
                        float voltage = plusPin.voltage - minusPin.voltage;
                        float torque = CalculateMotorTorque(voltage);
                        
                        physicsBridge.SetMotorTorque(node.assemblyNodeId, torque);
                        
                        // Back-EMF feedback
                        float velocity = physicsBridge.GetJointVelocity(node.assemblyNodeId);
                        float backEMF = CalculateBackEMF(velocity);
                        plusPin.voltage -= backEMF;
                    }
                }
            }
        }
        
        private float CalculateMotorTorque(float voltage)
        {
            // Simplified motor model: T = k * V
            // For typical FTC motor: ~1.4 N⋅m at 12V
            const float motorConstant = 0.117f; // N⋅m/V
            return motorConstant * voltage;
        }
        
        private float CalculateBackEMF(float velocity)
        {
            // Back-EMF: V = k * ω
            const float backEMFConstant = 0.01f; // V/(rad/s)
            return backEMFConstant * velocity;
        }
        
        /// <summary>
        /// Start simulation
        /// </summary>
        public void StartSimulation()
        {
            if (isSimulating)
            {
                Debug.LogWarning("Simulation already running");
                return;
            }
            
            // Build physics tree from assembly graph
            physicsBridge.BuildPhysicsTree();
            
            // Update physics properties
            assemblyGraph.UpdatePhysicsProperties();
            
            isSimulating = true;
            Debug.Log("Simulation started");
        }
        
        /// <summary>
        /// Stop simulation
        /// </summary>
        public void StopSimulation()
        {
            if (!isSimulating)
            {
                Debug.LogWarning("Simulation not running");
                return;
            }
            
            isSimulating = false;
            Debug.Log("Simulation stopped");
        }
        
        /// <summary>
        /// Save current robot
        /// </summary>
        public bool SaveRobot(string robotName)
        {
            if (dataPersistence == null)
            {
                Debug.LogError("DataPersistence system not initialized");
                return false;
            }
            
            return dataPersistence.SaveRobot(robotName, assemblyGraph, electroGraph);
        }
        
        /// <summary>
        /// Load robot from file
        /// </summary>
        public bool LoadRobot(string filePath)
        {
            if (dataPersistence == null)
            {
                Debug.LogError("DataPersistence system not initialized");
                return false;
            }
            
            AssemblyGraph loadedAssembly;
            ElectroGraph loadedElectro;
            
            if (dataPersistence.LoadRobot(filePath, out loadedAssembly, out loadedElectro))
            {
                // Replace current systems
                if (assemblyGraph != null && assemblyGraph.gameObject != null)
                {
                    Destroy(assemblyGraph.gameObject);
                }
                if (electroGraph != null && electroGraph.gameObject != null)
                {
                    Destroy(electroGraph.gameObject);
                }
                
                assemblyGraph = loadedAssembly;
                electroGraph = loadedElectro;
                
                assemblyGraph.transform.SetParent(transform);
                electroGraph.transform.SetParent(transform);
                
                physicsBridge.Initialize(assemblyGraph);
                snappingService.Initialize(assemblyGraph, PartsLibrary.Instance);
                
                return true;
            }
            
            return false;
        }
    }
}
