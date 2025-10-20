using UnityEngine;
using UnityEditor;
using FTCSIM.Core;

namespace FTCSIM.Editor
{
    /// <summary>
    /// Custom Inspector for RobotController to provide enhanced editor controls.
    /// </summary>
    [CustomEditor(typeof(RobotController))]
    public class RobotControllerEditor : UnityEditor.Editor
    {
        private RobotController controller;
        
        private void OnEnable()
        {
            controller = (RobotController)target;
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("FTC-SIM Controls", EditorStyles.boldLabel);
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play mode to use simulation controls.", MessageType.Info);
                
                if (GUILayout.Button("Open Robot Builder Window", GUILayout.Height(30)))
                {
                    RobotBuilderWindow.ShowWindow();
                }
                
                return;
            }
            
            EditorGUILayout.Space(5);
            
            // Simulation controls
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Simulation", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("Status:", controller.isSimulating ? "Running" : "Stopped");
            
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = !controller.isSimulating;
            if (GUILayout.Button("Start Simulation", GUILayout.Height(30)))
            {
                controller.StartSimulation();
            }
            GUI.enabled = true;
            
            GUI.enabled = controller.isSimulating;
            if (GUILayout.Button("Stop Simulation", GUILayout.Height(30)))
            {
                controller.StopSimulation();
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // Save/Load controls
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Robot Management", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Save Robot", GUILayout.Height(30)))
            {
                string robotName = EditorUtility.SaveFilePanel(
                    "Save Robot", 
                    Application.persistentDataPath, 
                    "MyRobot", 
                    ""
                );
                
                if (!string.IsNullOrEmpty(robotName))
                {
                    if (controller.SaveRobot(System.IO.Path.GetFileNameWithoutExtension(robotName)))
                    {
                        EditorUtility.DisplayDialog("Success", "Robot saved successfully!", "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Failed to save robot.", "OK");
                    }
                }
            }
            
            if (GUILayout.Button("Load Robot", GUILayout.Height(30)))
            {
                string filePath = EditorUtility.OpenFilePanel(
                    "Load Robot", 
                    Application.persistentDataPath, 
                    "json"
                );
                
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (controller.LoadRobot(filePath))
                    {
                        EditorUtility.DisplayDialog("Success", "Robot loaded successfully!", "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Failed to load robot.", "OK");
                    }
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // Assembly info
            if (controller.assemblyGraph != null && controller.assemblyGraph.rootNode != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Assembly Statistics", EditorStyles.boldLabel);
                
                int nodeCount = CountNodes(controller.assemblyGraph.rootNode);
                EditorGUILayout.LabelField("Total Nodes:", nodeCount.ToString());
                EditorGUILayout.LabelField("Root Mass:", controller.assemblyGraph.rootNode.mass.ToString("F3") + " kg");
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(10);
            
            // Electronics info
            if (controller.electroGraph != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Electronics Statistics", EditorStyles.boldLabel);
                
                EditorGUILayout.LabelField("Nodes:", controller.electroGraph.nodes.Count.ToString());
                EditorGUILayout.LabelField("Wires:", controller.electroGraph.wires.Count.ToString());
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(10);
            
            // Quick actions
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Open Robot Builder Window", GUILayout.Height(25)))
            {
                RobotBuilderWindow.ShowWindow();
            }
            
            if (GUILayout.Button("Open Telemetry Window", GUILayout.Height(25)))
            {
                TelemetryWindow.ShowWindow();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private int CountNodes(FTCSIM.Assembly.AssemblyNode node)
        {
            int count = 1;
            foreach (var child in node.children)
            {
                count += CountNodes(child);
            }
            return count;
        }
    }
}
