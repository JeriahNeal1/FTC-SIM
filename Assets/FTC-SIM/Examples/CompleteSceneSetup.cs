using UnityEngine;
using FTCSIM.Core;
using FTCSIM.UI;
using FTCSIM.Parts;

namespace FTCSIM.Examples
{
    /// <summary>
    /// Complete scene setup script that creates everything needed for the FTC-SIM robot builder.
    /// Attach this to an empty GameObject in your scene and run it to set up:
    /// - RobotController
    /// - Build Camera
    /// - Robot Builder UI (three-panel layout)
    /// - Telemetry Visualization
    /// - ElectroGraph Editor
    /// - Example parts loading
    /// </summary>
    public class CompleteSceneSetup : MonoBehaviour
    {
        [Header("Setup Options")]
        public bool setupOnStart = true;
        public bool loadExampleParts = true;
        public bool createFloor = true;
        
        [Header("Scene Setup")]
        public Material floorMaterial;
        public Vector3 floorSize = new Vector3(10f, 0.1f, 10f);
        
        [ContextMenu("Setup Complete Scene")]
        public void SetupScene()
        {
            Debug.Log("=== Starting Complete FTC-SIM Scene Setup ===");
            
            // Step 1: Create RobotController
            SetupRobotController();
            
            // Step 2: Create Build Camera
            SetupBuildCamera();
            
            // Step 3: Create Floor/Ground
            if (createFloor)
            {
                SetupFloor();
            }
            
            // Step 4: Setup Robot Builder UI
            SetupRobotBuilderUI();
            
            // Step 5: Load Parts Library
            if (loadExampleParts)
            {
                LoadPartsLibrary();
            }
            
            // Step 6: Setup Lighting
            SetupLighting();
            
            Debug.Log("=== Complete FTC-SIM Scene Setup Finished! ===");
            Debug.Log("You can now press Play to start building robots!");
        }
        
        private void SetupRobotController()
        {
            RobotController controller = FindObjectOfType<RobotController>();
            
            if (controller == null)
            {
                GameObject controllerObj = new GameObject("RobotController");
                controller = controllerObj.AddComponent<RobotController>();
                Debug.Log("✓ Created RobotController");
            }
            else
            {
                Debug.Log("✓ RobotController already exists");
            }
        }
        
        private void SetupBuildCamera()
        {
            Camera buildCam = Camera.main;
            
            if (buildCam == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                buildCam = cameraObj.AddComponent<Camera>();
                buildCam.tag = "MainCamera";
                
                // Setup camera for HDRP
                buildCam.clearFlags = CameraClearFlags.Skybox;
                buildCam.backgroundColor = new Color(0.2f, 0.25f, 0.3f, 1f);
                buildCam.farClipPlane = 100f;
                buildCam.nearClipPlane = 0.01f;
                
                Debug.Log("✓ Created Main Camera");
            }
            
            // Position camera for good robot building view
            buildCam.transform.position = new Vector3(0f, 2f, -5f);
            buildCam.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
            
            // Add audio listener if missing
            if (buildCam.GetComponent<AudioListener>() == null)
            {
                buildCam.gameObject.AddComponent<AudioListener>();
            }
            
            Debug.Log("✓ Build camera configured");
        }
        
        private void SetupFloor()
        {
            GameObject floor = GameObject.Find("Floor");
            
            if (floor == null)
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = "Floor";
                floor.transform.position = new Vector3(0f, -0.05f, 0f);
                floor.transform.localScale = floorSize;
                
                // Setup physics
                Rigidbody rb = floor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                
                // Apply material if provided
                if (floorMaterial != null)
                {
                    Renderer renderer = floor.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = floorMaterial;
                    }
                }
                else
                {
                    // Default gray floor
                    Renderer renderer = floor.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    }
                }
                
                Debug.Log("✓ Created floor");
            }
            else
            {
                Debug.Log("✓ Floor already exists");
            }
        }
        
        private void SetupRobotBuilderUI()
        {
            // Check if UI already exists
            Canvas existingCanvas = FindObjectOfType<Canvas>();
            RobotBuilderUI existingUI = FindObjectOfType<RobotBuilderUI>();
            
            if (existingUI != null)
            {
                Debug.Log("✓ Robot Builder UI already exists");
                return;
            }
            
            // Create UI Setup GameObject
            GameObject uiSetupObj = new GameObject("RobotBuilderUISetup");
            RobotBuilderUISetup uiSetup = uiSetupObj.AddComponent<RobotBuilderUISetup>();
            
            // Configure and run setup
            uiSetup.autoSetupOnStart = false;
            uiSetup.createInGameCamera = false; // We already have a camera
            uiSetup.catalogPanelWidth = 300f;
            uiSetup.propertiesPanelWidth = 350f;
            
            // Run the setup
            uiSetup.SetupUI();
            
            Debug.Log("✓ Robot Builder UI created");
            
            // Clean up the setup object (optional)
            // We'll keep it for now in case user wants to modify settings
            // DestroyImmediate(uiSetupObj);
        }
        
        private void LoadPartsLibrary()
        {
            Debug.Log("Loading parts library...");
            
            // Load parts from StreamingAssets
            string partsPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Parts");
            
            if (System.IO.Directory.Exists(partsPath))
            {
                PartsLibrary.Instance.LoadPartsFromDirectory(partsPath);
                int partCount = PartsLibrary.Instance.GetAllCategories().Count;
                Debug.Log($"✓ Loaded parts library ({partCount} categories)");
            }
            else
            {
                Debug.LogWarning("⚠ Parts directory not found at: " + partsPath);
                Debug.LogWarning("⚠ Creating example parts...");
                CreateExampleParts();
            }
        }
        
        private void CreateExampleParts()
        {
            // Create a few example parts programmatically
            // In a real scenario, these would be loaded from JSON files
            
            // Example C-Channel
            var cChannel = new PartDefinition
            {
                sku = "3203-0002-0002",
                vendor = "goBILDA",
                displayName = "2x2 C-Channel",
                category = "Structural",
                mass = 0.025f,
                prefabPath = ""
            };
            cChannel.mounts.Add(new AttachmentPoint("mount_1", "Pattern:goBILDA-16mm", Vector3.zero, Quaternion.identity));
            cChannel.mounts.Add(new AttachmentPoint("mount_2", "Pattern:goBILDA-16mm", new Vector3(0.032f, 0, 0), Quaternion.identity));
            
            PartsLibrary.Instance.AddPart(cChannel);
            
            // Example Motor
            var motor = new PartDefinition
            {
                sku = "5202-0002-0001",
                vendor = "goBILDA",
                displayName = "Yellow Jacket Motor",
                category = "Actuators",
                mass = 0.25f,
                prefabPath = ""
            };
            motor.mounts.Add(new AttachmentPoint("mount", "Pattern:goBILDA-16mm", Vector3.zero, Quaternion.identity));
            motor.electronicsProfile = new PartDefinition.ElectronicsProfile
            {
                deviceType = "DCMotor",
                voltage = 12f,
                currentDraw = 5f
            };
            
            PartsLibrary.Instance.AddPart(motor);
            
            Debug.Log("✓ Created example parts");
        }
        
        private void SetupLighting()
        {
            Light mainLight = FindObjectOfType<Light>();
            
            if (mainLight == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                mainLight = lightObj.AddComponent<Light>();
                mainLight.type = LightType.Directional;
                mainLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                
                Debug.Log("✓ Created directional light");
            }
            
            // Configure lighting for robot building
            mainLight.intensity = 1f;
            mainLight.color = Color.white;
            mainLight.shadows = LightShadows.Soft;
            
            Debug.Log("✓ Lighting configured");
        }
        
        private void Start()
        {
            if (setupOnStart && !Application.isEditor)
            {
                SetupScene();
            }
        }
        
        [ContextMenu("Print Scene Info")]
        public void PrintSceneInfo()
        {
            Debug.Log("=== FTC-SIM Scene Information ===");
            
            RobotController controller = FindObjectOfType<RobotController>();
            Debug.Log($"RobotController: {(controller != null ? "✓ Present" : "✗ Missing")}");
            
            Camera cam = Camera.main;
            Debug.Log($"Main Camera: {(cam != null ? "✓ Present" : "✗ Missing")}");
            
            Canvas canvas = FindObjectOfType<Canvas>();
            Debug.Log($"UI Canvas: {(canvas != null ? "✓ Present" : "✗ Missing")}");
            
            RobotBuilderUI builderUI = FindObjectOfType<RobotBuilderUI>();
            Debug.Log($"Robot Builder UI: {(builderUI != null ? "✓ Present" : "✗ Missing")}");
            
            int partCount = PartsLibrary.Instance.GetAllCategories().Count;
            Debug.Log($"Parts Library: {partCount} categories loaded");
            
            Debug.Log("====================================");
        }
    }
}
