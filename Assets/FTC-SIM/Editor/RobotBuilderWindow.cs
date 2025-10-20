using UnityEngine;
using UnityEditor;
using FTCSIM.Core;
using FTCSIM.Assembly;
using FTCSIM.Parts;

namespace FTCSIM.Editor
{
    /// <summary>
    /// Main Unity Editor window for the FTC-SIM Robot Builder.
    /// Provides UI for assembly, parts library, and robot management.
    /// </summary>
    public class RobotBuilderWindow : EditorWindow
    {
        private RobotController controller;
        private Vector2 scrollPosition;
        private int selectedTab = 0;
        private string[] tabNames = { "Assembly", "Parts Library", "Robot Info" };
        
        // Assembly tab
        private Vector2 assemblyScrollPosition;
        private AssemblyNode selectedNode;
        
        // Parts library tab
        private Vector2 partsScrollPosition;
        private string searchQuery = "";
        private string selectedCategory = "All";
        
        [MenuItem("FTC-SIM/Robot Builder Window")]
        public static void ShowWindow()
        {
            RobotBuilderWindow window = GetWindow<RobotBuilderWindow>("Robot Builder");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnEnable()
        {
            FindOrCreateController();
        }
        
        private void FindOrCreateController()
        {
            controller = FindObjectOfType<RobotController>();
            if (controller == null && !Application.isPlaying)
            {
                EditorGUILayout.HelpBox("No RobotController found in scene. Create one to start building.", MessageType.Info);
            }
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            // Header
            DrawHeader();
            
            // Controller status
            DrawControllerStatus();
            
            // Tabs
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            
            EditorGUILayout.Space(10);
            
            // Tab content
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            switch (selectedTab)
            {
                case 0:
                    DrawAssemblyTab();
                    break;
                case 1:
                    DrawPartsLibraryTab();
                    break;
                case 2:
                    DrawRobotInfoTab();
                    break;
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("FTC-SIM Robot Builder", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                FindOrCreateController();
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawControllerStatus()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (controller == null)
            {
                EditorGUILayout.HelpBox("No RobotController found in the scene.", MessageType.Warning);
                
                if (GUILayout.Button("Create RobotController", GUILayout.Height(30)))
                {
                    GameObject go = new GameObject("RobotController");
                    controller = go.AddComponent<RobotController>();
                    Selection.activeGameObject = go;
                    EditorGUILayout.HelpBox("RobotController created! It will initialize when you enter Play mode.", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Controller Status", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("GameObject:", controller.gameObject.name);
                EditorGUILayout.LabelField("Simulating:", controller.isSimulating ? "Yes" : "No");
                
                if (Application.isPlaying)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    if (!controller.isSimulating)
                    {
                        if (GUILayout.Button("Start Simulation", GUILayout.Height(25)))
                        {
                            controller.StartSimulation();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Stop Simulation", GUILayout.Height(25)))
                        {
                            controller.StopSimulation();
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("Enter Play mode to start simulation.", MessageType.Info);
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawAssemblyTab()
        {
            EditorGUILayout.LabelField("Assembly Hierarchy", EditorStyles.boldLabel);
            
            if (controller == null || controller.assemblyGraph == null)
            {
                EditorGUILayout.HelpBox("No assembly graph available. RobotController must be initialized.", MessageType.Info);
                return;
            }
            
            if (controller.assemblyGraph.rootNode == null)
            {
                EditorGUILayout.HelpBox("Assembly graph has no root node. Initialize in Play mode.", MessageType.Info);
                return;
            }
            
            EditorGUILayout.Space(5);
            assemblyScrollPosition = EditorGUILayout.BeginScrollView(assemblyScrollPosition);
            
            DrawAssemblyNodeRecursive(controller.assemblyGraph.rootNode, 0);
            
            EditorGUILayout.EndScrollView();
            
            // Selected node details
            if (selectedNode != null)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Selected Node Details", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("ID:", selectedNode.id);
                EditorGUILayout.LabelField("Name:", selectedNode.displayName);
                EditorGUILayout.LabelField("Part SKU:", selectedNode.partSKU ?? "N/A");
                EditorGUILayout.LabelField("Is Subassembly:", selectedNode.isSubassembly.ToString());
                EditorGUILayout.LabelField("Mass:", selectedNode.mass.ToString("F3") + " kg");
                EditorGUILayout.LabelField("Children:", selectedNode.children.Count.ToString());
                EditorGUILayout.LabelField("Relations:", selectedNode.relations.Count.ToString());
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawAssemblyNodeRecursive(AssemblyNode node, int indentLevel)
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Space(indentLevel * 20);
            
            bool isSelected = selectedNode == node;
            Color originalColor = GUI.backgroundColor;
            if (isSelected)
            {
                GUI.backgroundColor = Color.cyan;
            }
            
            string icon = node.isSubassembly ? "▼" : "●";
            string label = $"{icon} {node.displayName}";
            
            if (GUILayout.Button(label, EditorStyles.label, GUILayout.ExpandWidth(true)))
            {
                selectedNode = node;
            }
            
            GUI.backgroundColor = originalColor;
            
            EditorGUILayout.EndHorizontal();
            
            if (node.children != null && node.children.Count > 0)
            {
                foreach (var child in node.children)
                {
                    DrawAssemblyNodeRecursive(child, indentLevel + 1);
                }
            }
        }
        
        private void DrawPartsLibraryTab()
        {
            EditorGUILayout.LabelField("Parts Library", EditorStyles.boldLabel);
            
            // Search bar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search:", GUILayout.Width(60));
            searchQuery = EditorGUILayout.TextField(searchQuery);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Category filter
            var categories = PartsLibrary.Instance.GetAllCategories();
            if (categories.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Category:", GUILayout.Width(60));
                
                string[] categoryOptions = new string[categories.Count + 1];
                categoryOptions[0] = "All";
                for (int i = 0; i < categories.Count; i++)
                {
                    categoryOptions[i + 1] = categories[i];
                }
                
                int selectedIndex = System.Array.IndexOf(categoryOptions, selectedCategory);
                if (selectedIndex < 0) selectedIndex = 0;
                
                selectedIndex = EditorGUILayout.Popup(selectedIndex, categoryOptions);
                selectedCategory = categoryOptions[selectedIndex];
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(10);
            
            // Parts list
            partsScrollPosition = EditorGUILayout.BeginScrollView(partsScrollPosition);
            
            var parts = string.IsNullOrEmpty(searchQuery) 
                ? (selectedCategory == "All" 
                    ? PartsLibrary.Instance.GetAllCategories().Count > 0 
                        ? PartsLibrary.Instance.GetPartsByCategory(PartsLibrary.Instance.GetAllCategories()[0])
                        : new System.Collections.Generic.List<PartDefinition>()
                    : PartsLibrary.Instance.GetPartsByCategory(selectedCategory))
                : PartsLibrary.Instance.SearchParts(searchQuery);
            
            if (parts.Count == 0)
            {
                EditorGUILayout.HelpBox("No parts found. Load parts library in Play mode or check StreamingAssets/Parts/", MessageType.Info);
            }
            else
            {
                foreach (var part in parts)
                {
                    DrawPartItem(part);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawPartItem(PartDefinition part)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField(part.displayName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("SKU: " + part.sku);
            EditorGUILayout.LabelField("Category: " + part.category);
            EditorGUILayout.LabelField("Mass: " + part.mass.ToString("F3") + " kg");
            EditorGUILayout.LabelField("Mounts: " + part.mounts.Count);
            
            if (part.electronicsProfile != null)
            {
                EditorGUILayout.LabelField("Electronics: " + part.electronicsProfile.deviceType);
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        private void DrawRobotInfoTab()
        {
            EditorGUILayout.LabelField("Robot Information", EditorStyles.boldLabel);
            
            if (controller == null)
            {
                EditorGUILayout.HelpBox("No RobotController available.", MessageType.Info);
                return;
            }
            
            EditorGUILayout.Space(5);
            
            // Save/Load section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Save / Load", EditorStyles.boldLabel);
            
            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Save Robot", GUILayout.Height(30)))
                {
                    string robotName = EditorUtility.SaveFilePanel("Save Robot", 
                        Application.persistentDataPath, 
                        "MyRobot", 
                        "");
                    
                    if (!string.IsNullOrEmpty(robotName))
                    {
                        controller.SaveRobot(System.IO.Path.GetFileNameWithoutExtension(robotName));
                        EditorUtility.DisplayDialog("Success", "Robot saved successfully!", "OK");
                    }
                }
                
                if (GUILayout.Button("Load Robot", GUILayout.Height(30)))
                {
                    string filePath = EditorUtility.OpenFilePanel("Load Robot", 
                        Application.persistentDataPath, 
                        "json");
                    
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
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play mode to save/load robots.", MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // Statistics
            if (controller.assemblyGraph != null && controller.assemblyGraph.rootNode != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Assembly Statistics", EditorStyles.boldLabel);
                
                int nodeCount = CountNodes(controller.assemblyGraph.rootNode);
                EditorGUILayout.LabelField("Total Nodes:", nodeCount.ToString());
                EditorGUILayout.LabelField("Root Children:", controller.assemblyGraph.rootNode.children.Count.ToString());
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(10);
            
            // Documentation links
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Documentation", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Open Getting Started Guide"))
            {
                Application.OpenURL("file://" + Application.dataPath + "/FTC-SIM/Documentation/GETTING_STARTED.md");
            }
            
            if (GUILayout.Button("Open Architecture Documentation"))
            {
                Application.OpenURL("file://" + Application.dataPath + "/FTC-SIM/Documentation/ARCHITECTURE.md");
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private int CountNodes(AssemblyNode node)
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
