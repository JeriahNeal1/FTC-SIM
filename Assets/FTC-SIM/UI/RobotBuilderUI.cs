using UnityEngine;
using UnityEngine.UI;
using FTCSIM.Core;
using FTCSIM.Parts;
using FTCSIM.Assembly;
using TMPro;
using System.Collections.Generic;

namespace FTCSIM.UI
{
    /// <summary>
    /// Main in-game UI for the robot builder with drag-and-drop functionality.
    /// Features: Parts catalog (left), 3D preview (center), Properties panel (right).
    /// </summary>
    public class RobotBuilderUI : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject partsCatalogPanel;
        public GameObject previewPanel;
        public GameObject propertiesPanel;
        
        [Header("Parts Catalog")]
        public Transform partsCatalogContent;
        public GameObject partItemPrefab;
        public TMP_InputField searchField;
        public TMP_Dropdown categoryDropdown;
        
        [Header("Properties Panel")]
        public Transform propertiesContent;
        public TMP_Text selectedPartNameText;
        public GameObject motorSpeedControlPrefab;
        public GameObject servoConfigPrefab;
        
        [Header("3D Preview")]
        public Camera previewCamera;
        public Transform previewContainer;
        
        [Header("References")]
        public RobotController robotController;
        public DragDropBuilder dragDropBuilder;
        
        private List<PartDefinition> currentParts = new List<PartDefinition>();
        private PartDefinition selectedPart;
        private AssemblyNode selectedNode;
        
        private void Start()
        {
            InitializeUI();
            LoadPartsLibrary();
        }
        
        private void InitializeUI()
        {
            // Setup search field
            if (searchField != null)
            {
                searchField.onValueChanged.AddListener(OnSearchChanged);
            }
            
            // Setup category dropdown
            if (categoryDropdown != null)
            {
                categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);
                PopulateCategoryDropdown();
            }
            
            // Ensure robot controller exists
            if (robotController == null)
            {
                robotController = FindObjectOfType<RobotController>();
            }
            
            // Ensure drag-drop builder exists
            if (dragDropBuilder == null)
            {
                dragDropBuilder = FindObjectOfType<DragDropBuilder>();
                if (dragDropBuilder == null)
                {
                    GameObject builderObj = new GameObject("DragDropBuilder");
                    dragDropBuilder = builderObj.AddComponent<DragDropBuilder>();
                    dragDropBuilder.robotController = robotController;
                    dragDropBuilder.builderUI = this;
                }
            }
        }
        
        private void LoadPartsLibrary()
        {
            // Load parts from library
            var categories = PartsLibrary.Instance.GetAllCategories();
            currentParts.Clear();
            
            foreach (var category in categories)
            {
                currentParts.AddRange(PartsLibrary.Instance.GetPartsByCategory(category));
            }
            
            RefreshPartsCatalog();
        }
        
        private void PopulateCategoryDropdown()
        {
            if (categoryDropdown == null) return;
            
            categoryDropdown.ClearOptions();
            
            List<string> options = new List<string> { "All" };
            options.AddRange(PartsLibrary.Instance.GetAllCategories());
            
            categoryDropdown.AddOptions(options);
        }
        
        private void OnSearchChanged(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                LoadPartsLibrary();
            }
            else
            {
                currentParts = PartsLibrary.Instance.SearchParts(query);
                RefreshPartsCatalog();
            }
        }
        
        private void OnCategoryChanged(int index)
        {
            if (categoryDropdown == null) return;
            
            string category = categoryDropdown.options[index].text;
            
            if (category == "All")
            {
                LoadPartsLibrary();
            }
            else
            {
                currentParts = PartsLibrary.Instance.GetPartsByCategory(category);
                RefreshPartsCatalog();
            }
        }
        
        private void RefreshPartsCatalog()
        {
            if (partsCatalogContent == null) return;
            
            // Clear existing items
            foreach (Transform child in partsCatalogContent)
            {
                Destroy(child.gameObject);
            }
            
            // Create part items
            foreach (var part in currentParts)
            {
                CreatePartItem(part);
            }
        }
        
        private void CreatePartItem(PartDefinition part)
        {
            if (partItemPrefab == null || partsCatalogContent == null) return;
            
            GameObject itemObj = Instantiate(partItemPrefab, partsCatalogContent);
            PartCatalogItem item = itemObj.GetComponent<PartCatalogItem>();
            
            if (item != null)
            {
                item.Initialize(part, this);
            }
        }
        
        public void OnPartSelected(PartDefinition part)
        {
            selectedPart = part;
            
            if (dragDropBuilder != null)
            {
                dragDropBuilder.BeginDrag(part);
            }
        }
        
        public void OnNodeSelected(AssemblyNode node)
        {
            selectedNode = node;
            UpdatePropertiesPanel();
        }
        
        private void UpdatePropertiesPanel()
        {
            if (propertiesContent == null) return;
            
            // Clear existing properties
            foreach (Transform child in propertiesContent)
            {
                Destroy(child.gameObject);
            }
            
            if (selectedNode == null)
            {
                if (selectedPartNameText != null)
                {
                    selectedPartNameText.text = "No Part Selected";
                }
                return;
            }
            
            // Display part name
            if (selectedPartNameText != null)
            {
                selectedPartNameText.text = selectedNode.displayName;
            }
            
            // Get part definition
            var partDef = PartsLibrary.Instance.GetPart(selectedNode.partSKU);
            if (partDef == null) return;
            
            // If it has an electronics profile, show controls
            if (partDef.electronicsProfile != null)
            {
                CreateElectronicsControls(partDef.electronicsProfile);
            }
        }
        
        private void CreateElectronicsControls(PartDefinition.ElectronicsProfile profile)
        {
            if (profile.deviceType == "DCMotor" && motorSpeedControlPrefab != null)
            {
                GameObject controlObj = Instantiate(motorSpeedControlPrefab, propertiesContent);
                MotorSpeedControl control = controlObj.GetComponent<MotorSpeedControl>();
                
                if (control != null)
                {
                    control.Initialize(selectedNode, robotController);
                }
            }
            else if (profile.deviceType == "Servo" && servoConfigPrefab != null)
            {
                GameObject controlObj = Instantiate(servoConfigPrefab, propertiesContent);
                ServoConfiguration control = controlObj.GetComponent<ServoConfiguration>();
                
                if (control != null)
                {
                    control.Initialize(selectedNode, robotController);
                }
            }
        }
        
        public void TogglePanel(string panelName)
        {
            switch (panelName)
            {
                case "Parts":
                    if (partsCatalogPanel != null)
                    {
                        partsCatalogPanel.SetActive(!partsCatalogPanel.activeSelf);
                    }
                    break;
                case "Properties":
                    if (propertiesPanel != null)
                    {
                        propertiesPanel.SetActive(!propertiesPanel.activeSelf);
                    }
                    break;
            }
        }
    }
}
