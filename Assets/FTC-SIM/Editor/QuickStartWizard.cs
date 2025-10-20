using UnityEngine;
using UnityEditor;
using FTCSIM.Core;

namespace FTCSIM.Editor
{
    /// <summary>
    /// Quick Start Wizard to help new users set up FTC-SIM.
    /// </summary>
    public class QuickStartWizard : EditorWindow
    {
        private int currentStep = 0;
        private string[] stepTitles = new string[]
        {
            "Welcome to FTC-SIM",
            "Setup",
            "Create Robot Controller",
            "Test Example",
            "Next Steps"
        };
        
        private bool controllerExists = false;
        private bool streamingAssetsFolderExists = false;
        private bool examplePartsCopied = false;
        
        public static void ShowWindow()
        {
            QuickStartWizard window = GetWindow<QuickStartWizard>("Quick Start");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(500, 400);
            window.Show();
        }
        
        private void OnEnable()
        {
            CheckSetupStatus();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            // Header
            DrawHeader();
            
            // Progress bar
            DrawProgressBar();
            
            EditorGUILayout.Space(20);
            
            // Step content
            DrawStepContent();
            
            GUILayout.FlexibleSpace();
            
            // Navigation buttons
            DrawNavigation();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 16;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            
            EditorGUILayout.LabelField("FTC-SIM Quick Start Wizard", titleStyle, GUILayout.Height(30));
            
            GUIStyle stepStyle = new GUIStyle(EditorStyles.label);
            stepStyle.alignment = TextAnchor.MiddleCenter;
            
            EditorGUILayout.LabelField(stepTitles[currentStep], stepStyle);
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawProgressBar()
        {
            EditorGUILayout.BeginHorizontal();
            
            for (int i = 0; i < stepTitles.Length; i++)
            {
                Color color = i < currentStep ? Color.green : (i == currentStep ? Color.yellow : Color.gray);
                
                Rect rect = GUILayoutUtility.GetRect(60, 10);
                EditorGUI.DrawRect(rect, color);
                
                if (i < stepTitles.Length - 1)
                {
                    GUILayout.Space(5);
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawStepContent()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            switch (currentStep)
            {
                case 0:
                    DrawWelcomeStep();
                    break;
                case 1:
                    DrawSetupStep();
                    break;
                case 2:
                    DrawControllerStep();
                    break;
                case 3:
                    DrawTestStep();
                    break;
                case 4:
                    DrawNextStepsStep();
                    break;
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawWelcomeStep()
        {
            EditorGUILayout.LabelField("Welcome!", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "FTC-SIM is an interactive robot builder and simulator for Unity 6 HDRP.\n\n" +
                "This wizard will help you:\n" +
                "• Set up the required folders\n" +
                "• Create a RobotController\n" +
                "• Load example parts\n" +
                "• Test the simulation\n\n" +
                "Click 'Next' to begin!",
                MessageType.Info);
        }
        
        private void DrawSetupStep()
        {
            EditorGUILayout.LabelField("Project Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            // Check status
            CheckSetupStatus();
            
            // StreamingAssets folder
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("StreamingAssets/Parts folder:", GUILayout.Width(200));
            EditorGUILayout.LabelField(streamingAssetsFolderExists ? "✓ Exists" : "✗ Missing");
            
            if (!streamingAssetsFolderExists)
            {
                if (GUILayout.Button("Create", GUILayout.Width(80)))
                {
                    FTCSimMenu.CreateStreamingAssetsFolder();
                    CheckSetupStatus();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Example parts
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Example parts copied:", GUILayout.Width(200));
            EditorGUILayout.LabelField(examplePartsCopied ? "✓ Yes" : "✗ No");
            
            if (!examplePartsCopied)
            {
                if (GUILayout.Button("Copy", GUILayout.Width(80)))
                {
                    FTCSimMenu.CopyExampleParts();
                    CheckSetupStatus();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            
            if (streamingAssetsFolderExists && examplePartsCopied)
            {
                EditorGUILayout.HelpBox("Setup complete! Click 'Next' to continue.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Complete the setup steps above, then click 'Next'.", MessageType.Warning);
            }
        }
        
        private void DrawControllerStep()
        {
            EditorGUILayout.LabelField("Create Robot Controller", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            CheckSetupStatus();
            
            if (controllerExists)
            {
                EditorGUILayout.HelpBox("✓ RobotController found in scene!", MessageType.Info);
                
                var controller = FindObjectOfType<RobotController>();
                if (controller != null)
                {
                    EditorGUILayout.LabelField("GameObject: " + controller.gameObject.name);
                    
                    if (GUILayout.Button("Select in Hierarchy"))
                    {
                        Selection.activeGameObject = controller.gameObject;
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "The RobotController is the main coordinator for the simulation.\n\n" +
                    "Click the button below to create one in your scene.",
                    MessageType.Info);
                
                EditorGUILayout.Space(10);
                
                if (GUILayout.Button("Create RobotController", GUILayout.Height(40)))
                {
                    FTCSimMenu.CreateRobotController();
                    CheckSetupStatus();
                }
            }
        }
        
        private void DrawTestStep()
        {
            EditorGUILayout.LabelField("Test the Simulation", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "To test FTC-SIM:\n\n" +
                "1. Press the Play button (▶) at the top of Unity\n" +
                "2. Open FTC-SIM > Robot Builder Window from the menu\n" +
                "3. The example parts will be loaded automatically\n" +
                "4. Click 'Start Simulation' in the Robot Builder\n\n" +
                "You can also attach the SimpleRobotExample script to a GameObject to see a programmatic example.",
                MessageType.Info);
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("Open Robot Builder Window", GUILayout.Height(30)))
            {
                RobotBuilderWindow.ShowWindow();
            }
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Open Parts Library Browser", GUILayout.Height(30)))
            {
                PartsLibraryBrowser.ShowWindow();
            }
        }
        
        private void DrawNextStepsStep()
        {
            EditorGUILayout.LabelField("You're All Set!", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "Congratulations! FTC-SIM is ready to use.\n\n" +
                "Here's what you can do next:",
                MessageType.Info);
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("📖 Read Getting Started Guide", GUILayout.Height(30)))
            {
                FTCSimMenu.OpenGettingStarted();
            }
            
            if (GUILayout.Button("🏗️ Open Robot Builder", GUILayout.Height(30)))
            {
                RobotBuilderWindow.ShowWindow();
            }
            
            if (GUILayout.Button("📚 Browse Parts Library", GUILayout.Height(30)))
            {
                PartsLibraryBrowser.ShowWindow();
            }
            
            if (GUILayout.Button("📊 View Telemetry Monitor", GUILayout.Height(30)))
            {
                TelemetryWindow.ShowWindow();
            }
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("Close Wizard", GUILayout.Height(25)))
            {
                Close();
            }
        }
        
        private void DrawNavigation()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = currentStep > 0;
            if (GUILayout.Button("← Previous", GUILayout.Height(30), GUILayout.Width(100)))
            {
                currentStep--;
            }
            GUI.enabled = true;
            
            GUILayout.FlexibleSpace();
            
            GUI.enabled = currentStep < stepTitles.Length - 1;
            if (GUILayout.Button("Next →", GUILayout.Height(30), GUILayout.Width(100)))
            {
                currentStep++;
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void CheckSetupStatus()
        {
            controllerExists = FindObjectOfType<RobotController>() != null;
            
            string partsPath = Application.streamingAssetsPath + "/Parts";
            streamingAssetsFolderExists = System.IO.Directory.Exists(partsPath);
            
            if (streamingAssetsFolderExists)
            {
                string[] files = System.IO.Directory.GetFiles(partsPath, "*.json");
                examplePartsCopied = files.Length > 0;
            }
            else
            {
                examplePartsCopied = false;
            }
        }
    }
}
