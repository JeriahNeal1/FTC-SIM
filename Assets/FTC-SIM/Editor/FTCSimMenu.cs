using UnityEngine;
using UnityEditor;
using FTCSIM.Core;

namespace FTCSIM.Editor
{
    /// <summary>
    /// Menu items for FTC-SIM tools and quick actions.
    /// </summary>
    public static class FTCSimMenu
    {
        [MenuItem("FTC-SIM/Quick Start Wizard", false, 0)]
        public static void ShowQuickStartWizard()
        {
            QuickStartWizard.ShowWindow();
        }
        
        [MenuItem("FTC-SIM/Robot Builder Window", false, 1)]
        public static void ShowRobotBuilder()
        {
            RobotBuilderWindow.ShowWindow();
        }
        
        [MenuItem("FTC-SIM/Parts Library Browser", false, 2)]
        public static void ShowPartsLibrary()
        {
            PartsLibraryBrowser.ShowWindow();
        }
        
        [MenuItem("FTC-SIM/Telemetry Window", false, 3)]
        public static void ShowTelemetry()
        {
            TelemetryWindow.ShowWindow();
        }
        
        [MenuItem("FTC-SIM/Create Robot Controller", false, 20)]
        public static void CreateRobotController()
        {
            // Check if one already exists
            var existing = Object.FindObjectOfType<RobotController>();
            if (existing != null)
            {
                bool replace = EditorUtility.DisplayDialog("RobotController Exists",
                    "A RobotController already exists in the scene. Select it instead?",
                    "Yes, Select It", "No, Create New");
                
                if (replace)
                {
                    Selection.activeGameObject = existing.gameObject;
                    return;
                }
            }
            
            // Create new controller
            GameObject go = new GameObject("RobotController");
            RobotController controller = go.AddComponent<RobotController>();
            Selection.activeGameObject = go;
            
            EditorUtility.DisplayDialog("Success",
                "RobotController created!\n\nThe controller will initialize when you enter Play mode.",
                "OK");
        }
        
        [MenuItem("FTC-SIM/Setup/Create StreamingAssets Folder", false, 40)]
        public static void CreateStreamingAssetsFolder()
        {
            string path = Application.streamingAssetsPath + "/Parts";
            
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success",
                    "StreamingAssets/Parts folder created!\n\nPlace your part definition JSON files here.",
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Info",
                    "StreamingAssets/Parts folder already exists.",
                    "OK");
            }
            
            EditorUtility.RevealInFinder(path);
        }
        
        [MenuItem("FTC-SIM/Setup/Copy Example Parts to StreamingAssets", false, 41)]
        public static void CopyExampleParts()
        {
            string sourcePath = Application.dataPath + "/FTC-SIM/Data/Schemas";
            string destPath = Application.streamingAssetsPath + "/Parts";
            
            if (!System.IO.Directory.Exists(destPath))
            {
                System.IO.Directory.CreateDirectory(destPath);
            }
            
            try
            {
                // Copy example part files
                string[] exampleFiles = new string[]
                {
                    "ExamplePart_CChannel.json",
                    "ExamplePart_Motor.json"
                };
                
                int copiedCount = 0;
                foreach (string fileName in exampleFiles)
                {
                    string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                    string destFile = System.IO.Path.Combine(destPath, fileName);
                    
                    if (System.IO.File.Exists(sourceFile))
                    {
                        System.IO.File.Copy(sourceFile, destFile, true);
                        copiedCount++;
                    }
                }
                
                AssetDatabase.Refresh();
                
                EditorUtility.DisplayDialog("Success",
                    $"Copied {copiedCount} example part files to StreamingAssets/Parts/\n\n" +
                    "These parts will be loaded when you enter Play mode.",
                    "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error",
                    "Failed to copy example parts:\n" + e.Message,
                    "OK");
            }
        }
        
        [MenuItem("FTC-SIM/Documentation/Getting Started", false, 60)]
        public static void OpenGettingStarted()
        {
            string path = Application.dataPath + "/FTC-SIM/Documentation/GETTING_STARTED.md";
            Application.OpenURL("file://" + path);
        }
        
        [MenuItem("FTC-SIM/Documentation/Architecture", false, 61)]
        public static void OpenArchitecture()
        {
            string path = Application.dataPath + "/FTC-SIM/Documentation/ARCHITECTURE.md";
            Application.OpenURL("file://" + path);
        }
        
        [MenuItem("FTC-SIM/Documentation/Implementation Summary", false, 62)]
        public static void OpenImplementationSummary()
        {
            string path = Application.dataPath + "/FTC-SIM/Documentation/IMPLEMENTATION_SUMMARY.md";
            Application.OpenURL("file://" + path);
        }
        
        [MenuItem("FTC-SIM/About FTC-SIM", false, 80)]
        public static void ShowAbout()
        {
            EditorUtility.DisplayDialog("About FTC-SIM",
                "FTC-SIM - Interactive Robot Builder & Simulator\n\n" +
                "Version: 1.0 (Core Architecture)\n" +
                "Unity Version: 6.4+ HDRP\n" +
                "Platform: Windows (Primary)\n\n" +
                "An educational robot builder with realistic physics and electronics simulation.\n\n" +
                "Repository: github.com/JeriahNeal1/FTC-SIM",
                "OK");
        }
    }
}
