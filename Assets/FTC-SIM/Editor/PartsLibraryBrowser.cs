using UnityEngine;
using UnityEditor;
using FTCSIM.Parts;
using System.Collections.Generic;

namespace FTCSIM.Editor
{
    /// <summary>
    /// Unity Editor window for browsing and managing the parts library.
    /// </summary>
    public class PartsLibraryBrowser : EditorWindow
    {
        private Vector2 scrollPosition;
        private Vector2 detailsScrollPosition;
        
        private string searchQuery = "";
        private string selectedCategory = "All";
        private PartDefinition selectedPart;
        
        private List<PartDefinition> currentPartsList = new List<PartDefinition>();
        
        [MenuItem("FTC-SIM/Parts Library Browser")]
        public static void ShowWindow()
        {
            PartsLibraryBrowser window = GetWindow<PartsLibraryBrowser>("Parts Library");
            window.minSize = new Vector2(600, 500);
            window.Show();
        }
        
        private void OnEnable()
        {
            RefreshPartsList();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            // Header
            DrawHeader();
            
            // Search and filter
            DrawSearchAndFilter();
            
            EditorGUILayout.Space(10);
            
            // Split view: parts list | part details
            EditorGUILayout.BeginHorizontal();
            
            // Left panel - parts list
            DrawPartsList();
            
            // Right panel - part details
            DrawPartDetails();
            
            EditorGUILayout.EndHorizontal();
            
            // Bottom controls
            DrawControls();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("FTC-SIM Parts Library", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                RefreshPartsList();
            }
            
            if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                EditorUtility.DisplayDialog("Parts Library Help", 
                    "The Parts Library loads JSON part definitions from StreamingAssets/Parts/.\n\n" +
                    "To add custom parts:\n" +
                    "1. Create a JSON file following the PartDefinition schema\n" +
                    "2. Place it in StreamingAssets/Parts/\n" +
                    "3. Click Refresh or enter Play mode\n\n" +
                    "See example schemas in Assets/FTC-SIM/Data/Schemas/", 
                    "OK");
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawSearchAndFilter()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Search bar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search:", GUILayout.Width(60));
            
            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
            {
                searchQuery = newSearchQuery;
                RefreshPartsList();
            }
            
            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                searchQuery = "";
                RefreshPartsList();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Category filter
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Category:", GUILayout.Width(60));
            
            var categories = PartsLibrary.Instance.GetAllCategories();
            string[] categoryOptions = new string[categories.Count + 1];
            categoryOptions[0] = "All";
            for (int i = 0; i < categories.Count; i++)
            {
                categoryOptions[i + 1] = categories[i];
            }
            
            int selectedIndex = System.Array.IndexOf(categoryOptions, selectedCategory);
            if (selectedIndex < 0) selectedIndex = 0;
            
            int newIndex = EditorGUILayout.Popup(selectedIndex, categoryOptions);
            if (newIndex != selectedIndex)
            {
                selectedCategory = categoryOptions[newIndex];
                RefreshPartsList();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Results count
            EditorGUILayout.LabelField("Results:", currentPartsList.Count.ToString());
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPartsList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            
            EditorGUILayout.LabelField("Parts List", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox);
            
            if (currentPartsList.Count == 0)
            {
                EditorGUILayout.HelpBox("No parts found.\n\nLoad parts in Play mode or add JSON files to StreamingAssets/Parts/", MessageType.Info);
            }
            else
            {
                foreach (var part in currentPartsList)
                {
                    bool isSelected = selectedPart == part;
                    Color originalColor = GUI.backgroundColor;
                    
                    if (isSelected)
                    {
                        GUI.backgroundColor = Color.cyan;
                    }
                    
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    if (GUILayout.Button(part.displayName, EditorStyles.label))
                    {
                        selectedPart = part;
                    }
                    
                    EditorGUILayout.LabelField("SKU: " + part.sku, EditorStyles.miniLabel);
                    EditorGUILayout.LabelField(part.category, EditorStyles.miniLabel);
                    
                    EditorGUILayout.EndVertical();
                    
                    GUI.backgroundColor = originalColor;
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPartDetails()
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField("Part Details", EditorStyles.boldLabel);
            
            if (selectedPart == null)
            {
                EditorGUILayout.HelpBox("Select a part from the list to view details.", MessageType.Info);
            }
            else
            {
                detailsScrollPosition = EditorGUILayout.BeginScrollView(detailsScrollPosition, EditorStyles.helpBox);
                
                // Basic info
                EditorGUILayout.LabelField("Basic Information", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("Display Name:", selectedPart.displayName);
                EditorGUILayout.LabelField("SKU:", selectedPart.sku);
                EditorGUILayout.LabelField("Vendor:", selectedPart.vendor);
                EditorGUILayout.LabelField("Category:", selectedPart.category);
                EditorGUILayout.LabelField("Mass:", selectedPart.mass.ToString("F3") + " kg");
                
                if (!string.IsNullOrEmpty(selectedPart.prefabPath))
                {
                    EditorGUILayout.LabelField("Prefab Path:", selectedPart.prefabPath);
                }
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(10);
                
                // Attachment points
                if (selectedPart.mounts != null && selectedPart.mounts.Count > 0)
                {
                    EditorGUILayout.LabelField("Attachment Points (" + selectedPart.mounts.Count + ")", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    foreach (var mount in selectedPart.mounts)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("ID: " + mount.id);
                        EditorGUILayout.LabelField("Pattern: " + mount.pattern);
                        EditorGUILayout.LabelField("Position: " + mount.localPosition.ToString());
                        EditorGUILayout.EndVertical();
                    }
                    
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.Space(5);
                }
                
                // Rotational mounts
                if (selectedPart.rotationalMounts != null && selectedPart.rotationalMounts.Count > 0)
                {
                    EditorGUILayout.LabelField("Rotational Mounts (" + selectedPart.rotationalMounts.Count + ")", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    foreach (var mount in selectedPart.rotationalMounts)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("ID: " + mount.id);
                        EditorGUILayout.LabelField("Type: " + mount.mountType);
                        EditorGUILayout.LabelField("Diameter: " + (mount.diameter * 1000f).ToString("F1") + " mm");
                        EditorGUILayout.LabelField("Can Drive: " + mount.canDrive);
                        EditorGUILayout.EndVertical();
                    }
                    
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.Space(5);
                }
                
                // Electronics profile
                if (selectedPart.electronicsProfile != null)
                {
                    EditorGUILayout.LabelField("Electronics Profile", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    EditorGUILayout.LabelField("Device Type:", selectedPart.electronicsProfile.deviceType);
                    EditorGUILayout.LabelField("Voltage:", selectedPart.electronicsProfile.voltage.ToString("F1") + " V");
                    EditorGUILayout.LabelField("Current Draw:", selectedPart.electronicsProfile.currentDraw.ToString("F2") + " A");
                    EditorGUILayout.LabelField("Pins:", selectedPart.electronicsProfile.pins.Count.ToString());
                    
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.Space(5);
                }
                
                // Colliders
                if (selectedPart.colliders != null && selectedPart.colliders.Count > 0)
                {
                    EditorGUILayout.LabelField("Colliders (" + selectedPart.colliders.Count + ")", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    foreach (var collider in selectedPart.colliders)
                    {
                        EditorGUILayout.LabelField("Source: " + collider.source);
                        EditorGUILayout.LabelField("Margin: " + (collider.margin * 1000f).ToString("F1") + " mm");
                    }
                    
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.Space(5);
                }
                
                // Rules
                if (selectedPart.rules != null)
                {
                    EditorGUILayout.LabelField("Part Rules", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    EditorGUILayout.LabelField("Valid Mates:", selectedPart.rules.validMates.Count.ToString());
                    foreach (var mate in selectedPart.rules.validMates)
                    {
                        EditorGUILayout.LabelField("  - " + mate);
                    }
                    
                    EditorGUILayout.LabelField("Max Load:", selectedPart.rules.maxLoad.ToString("F0") + " N");
                    EditorGUILayout.LabelField("Max Torque:", selectedPart.rules.maxTorque.ToString("F1") + " N⋅m");
                    
                    EditorGUILayout.EndVertical();
                }
                
                EditorGUILayout.EndScrollView();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawControls()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Open StreamingAssets/Parts Folder", GUILayout.Height(25)))
            {
                string partsPath = Application.streamingAssetsPath + "/Parts";
                if (!System.IO.Directory.Exists(partsPath))
                {
                    System.IO.Directory.CreateDirectory(partsPath);
                }
                EditorUtility.RevealInFinder(partsPath);
            }
            
            if (GUILayout.Button("View Example Schemas", GUILayout.Height(25)))
            {
                string schemasPath = Application.dataPath + "/FTC-SIM/Data/Schemas";
                EditorUtility.RevealInFinder(schemasPath);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        private void RefreshPartsList()
        {
            currentPartsList.Clear();
            
            if (!string.IsNullOrEmpty(searchQuery))
            {
                currentPartsList = PartsLibrary.Instance.SearchParts(searchQuery);
            }
            else if (selectedCategory == "All")
            {
                var categories = PartsLibrary.Instance.GetAllCategories();
                foreach (var category in categories)
                {
                    currentPartsList.AddRange(PartsLibrary.Instance.GetPartsByCategory(category));
                }
            }
            else
            {
                currentPartsList = PartsLibrary.Instance.GetPartsByCategory(selectedCategory);
            }
        }
    }
}
