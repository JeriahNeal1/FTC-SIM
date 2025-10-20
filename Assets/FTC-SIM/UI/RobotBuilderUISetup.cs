using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FTCSIM.Core;

namespace FTCSIM.UI
{
    /// <summary>
    /// Helper script to automatically set up the Robot Builder UI canvas and panels.
    /// This script creates the three-panel layout: Parts Catalog (left), 3D Preview (center), Properties (right).
    /// Attach this to an empty GameObject and run it in the Unity Editor.
    /// </summary>
    [ExecuteInEditMode]
    public class RobotBuilderUISetup : MonoBehaviour
    {
        [Header("Setup Options")]
        public bool autoSetupOnStart = false;
        public bool createInGameCamera = true;
        
        [Header("Layout Settings")]
        public float catalogPanelWidth = 300f;
        public float propertiesPanelWidth = 350f;
        
        [Header("References (Auto-assigned)")]
        public Canvas mainCanvas;
        public Camera buildCamera;
        public RobotBuilderUI builderUI;
        
        [ContextMenu("Setup Robot Builder UI")]
        public void SetupUI()
        {
            Debug.Log("Setting up Robot Builder UI...");
            
            // Create or find canvas
            if (mainCanvas == null)
            {
                mainCanvas = CreateCanvas();
            }
            
            // Create camera if needed
            if (createInGameCamera && buildCamera == null)
            {
                buildCamera = CreateBuildCamera();
            }
            
            // Create the three-panel layout
            CreateThreePanelLayout();
            
            // Create RobotBuilderUI component
            SetupRobotBuilderUIComponent();
            
            Debug.Log("Robot Builder UI setup complete!");
        }
        
        private Canvas CreateCanvas()
        {
            GameObject canvasObj = new GameObject("RobotBuilderCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("Created main canvas: " + canvasObj.name);
            return canvas;
        }
        
        private Camera CreateBuildCamera()
        {
            GameObject cameraObj = new GameObject("BuildCamera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.2f, 0.2f, 0.25f, 1f);
            cam.cullingMask = LayerMask.GetMask("Default");
            cam.transform.position = new Vector3(0, 2, -5);
            cam.transform.LookAt(Vector3.zero);
            
            Debug.Log("Created build camera: " + cameraObj.name);
            return cam;
        }
        
        private void CreateThreePanelLayout()
        {
            if (mainCanvas == null)
            {
                Debug.LogError("Cannot create panels without a canvas!");
                return;
            }
            
            RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
            
            // Create Parts Catalog Panel (Left)
            GameObject catalogPanel = CreatePartsCatalogPanel(canvasRect);
            
            // Create Preview Panel (Center)
            GameObject previewPanel = CreatePreviewPanel(canvasRect);
            
            // Create Properties Panel (Right)
            GameObject propertiesPanel = CreatePropertiesPanel(canvasRect);
            
            Debug.Log("Three-panel layout created successfully!");
        }
        
        private GameObject CreatePartsCatalogPanel(RectTransform canvasRect)
        {
            GameObject panel = new GameObject("PartsCatalogPanel");
            panel.transform.SetParent(canvasRect, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(catalogPanelWidth, 0);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            
            // Add header
            CreatePanelHeader(panel, "Parts Catalog");
            
            // Add search bar
            CreateSearchBar(panel);
            
            // Add category dropdown
            CreateCategoryDropdown(panel);
            
            // Add scroll view for parts
            CreatePartsScrollView(panel);
            
            Debug.Log("Created Parts Catalog Panel");
            return panel;
        }
        
        private GameObject CreatePreviewPanel(RectTransform canvasRect)
        {
            GameObject panel = new GameObject("PreviewPanel");
            panel.transform.SetParent(canvasRect, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = new Vector2(catalogPanelWidth, 0);
            rect.offsetMax = new Vector2(-propertiesPanelWidth, 0);
            
            // Transparent background to see 3D view
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.1f);
            
            // Add instructions text
            CreateInstructionsText(panel);
            
            Debug.Log("Created Preview Panel");
            return panel;
        }
        
        private GameObject CreatePropertiesPanel(RectTransform canvasRect)
        {
            GameObject panel = new GameObject("PropertiesPanel");
            panel.transform.SetParent(canvasRect, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(propertiesPanelWidth, 0);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            
            // Add header
            CreatePanelHeader(panel, "Properties");
            
            // Add selected part name text
            CreateSelectedPartText(panel);
            
            // Add scroll view for properties
            CreatePropertiesScrollView(panel);
            
            Debug.Log("Created Properties Panel");
            return panel;
        }
        
        private void CreatePanelHeader(GameObject parent, string title)
        {
            GameObject header = new GameObject("Header");
            header.transform.SetParent(parent.transform, false);
            
            RectTransform rect = header.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 50);
            
            Image bg = header.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            // Title text
            GameObject textObj = new GameObject("Title");
            textObj.transform.SetParent(header.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = title;
            text.fontSize = 24;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
        }
        
        private void CreateSearchBar(GameObject parent)
        {
            GameObject searchObj = new GameObject("SearchBar");
            searchObj.transform.SetParent(parent.transform, false);
            
            RectTransform rect = searchObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -60);
            rect.sizeDelta = new Vector2(-20, 35);
            
            Image bg = searchObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            TMP_InputField inputField = searchObj.AddComponent<TMP_InputField>();
            
            // Placeholder
            GameObject placeholder = new GameObject("Placeholder");
            placeholder.transform.SetParent(searchObj.transform, false);
            TMP_Text placeholderText = placeholder.AddComponent<TMP_Text>();
            placeholderText.text = "Search parts...";
            placeholderText.fontSize = 14;
            placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            placeholderText.alignment = TextAlignmentOptions.Left;
            
            RectTransform placeholderRect = placeholder.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10, 0);
            placeholderRect.offsetMax = new Vector2(-10, 0);
            
            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(searchObj.transform, false);
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Left;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 0);
            textRect.offsetMax = new Vector2(-10, 0);
            
            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
            inputField.name = "SearchField";
        }
        
        private void CreateCategoryDropdown(GameObject parent)
        {
            GameObject dropdownObj = new GameObject("CategoryDropdown");
            dropdownObj.transform.SetParent(parent.transform, false);
            
            RectTransform rect = dropdownObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -105);
            rect.sizeDelta = new Vector2(-20, 35);
            
            Image bg = dropdownObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            TMP_Dropdown dropdown = dropdownObj.AddComponent<TMP_Dropdown>();
            dropdown.name = "CategoryDropdown";
            
            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(dropdownObj.transform, false);
            TMP_Text labelText = labelObj.AddComponent<TMP_Text>();
            labelText.text = "All Categories";
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;
            
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(10, 0);
            labelRect.offsetMax = new Vector2(-30, 0);
            
            dropdown.captionText = labelText;
        }
        
        private void CreatePartsScrollView(GameObject parent)
        {
            GameObject scrollView = new GameObject("PartsScrollView");
            scrollView.transform.SetParent(parent.transform, false);
            
            RectTransform rect = scrollView.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(10, 10);
            rect.offsetMax = new Vector2(-10, -150);
            
            Image bg = scrollView.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            
            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            
            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 500);
            
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = false;
            
            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            content.name = "PartsCatalogContent";
            
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
        }
        
        private void CreateInstructionsText(GameObject parent)
        {
            GameObject textObj = new GameObject("InstructionsText");
            textObj.transform.SetParent(parent.transform, false);
            
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(400, 200);
            
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = "3D Robot Preview\n\n" +
                       "Click parts in the catalog to begin building\n" +
                       "Drag to position parts\n" +
                       "Green glow = Valid snap point\n" +
                       "Click to place part\n" +
                       "ESC to cancel";
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
        }
        
        private void CreateSelectedPartText(GameObject parent)
        {
            GameObject textObj = new GameObject("SelectedPartName");
            textObj.transform.SetParent(parent.transform, false);
            
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -60);
            rect.sizeDelta = new Vector2(-20, 40);
            
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = "No Part Selected";
            text.fontSize = 16;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            
            text.name = "SelectedPartNameText";
        }
        
        private void CreatePropertiesScrollView(GameObject parent)
        {
            GameObject scrollView = new GameObject("PropertiesScrollView");
            scrollView.transform.SetParent(parent.transform, false);
            
            RectTransform rect = scrollView.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(10, 10);
            rect.offsetMax = new Vector2(-10, -110);
            
            Image bg = scrollView.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            
            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            
            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 500);
            
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = false;
            
            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            content.name = "PropertiesContent";
            
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
        }
        
        private void SetupRobotBuilderUIComponent()
        {
            if (mainCanvas == null) return;
            
            GameObject builderObj = mainCanvas.gameObject;
            builderUI = builderObj.GetComponent<RobotBuilderUI>();
            
            if (builderUI == null)
            {
                builderUI = builderObj.AddComponent<RobotBuilderUI>();
            }
            
            // Assign references
            builderUI.partsCatalogPanel = GameObject.Find("PartsCatalogPanel");
            builderUI.previewPanel = GameObject.Find("PreviewPanel");
            builderUI.propertiesPanel = GameObject.Find("PropertiesPanel");
            
            Transform catalogContent = builderObj.transform.Find("PartsCatalogPanel/PartsScrollView/Viewport/Content");
            if (catalogContent != null)
            {
                builderUI.partsCatalogContent = catalogContent;
            }
            
            Transform propertiesContent = builderObj.transform.Find("PropertiesPanel/PropertiesScrollView/Viewport/Content");
            if (propertiesContent != null)
            {
                builderUI.propertiesContent = propertiesContent;
            }
            
            TMP_InputField searchField = builderObj.GetComponentInChildren<TMP_InputField>();
            if (searchField != null && searchField.name == "SearchField")
            {
                builderUI.searchField = searchField;
            }
            
            TMP_Dropdown categoryDropdown = builderObj.GetComponentInChildren<TMP_Dropdown>();
            if (categoryDropdown != null && categoryDropdown.name == "CategoryDropdown")
            {
                builderUI.categoryDropdown = categoryDropdown;
            }
            
            TMP_Text selectedPartText = null;
            foreach (var text in builderObj.GetComponentsInChildren<TMP_Text>(true))
            {
                if (text.name == "SelectedPartNameText")
                {
                    selectedPartText = text;
                    break;
                }
            }
            if (selectedPartText != null)
            {
                builderUI.selectedPartNameText = selectedPartText;
            }
            
            builderUI.previewCamera = buildCamera;
            
            // Ensure RobotController exists
            RobotController controller = FindObjectOfType<RobotController>();
            if (controller == null)
            {
                GameObject controllerObj = new GameObject("RobotController");
                controller = controllerObj.AddComponent<RobotController>();
                Debug.Log("Created RobotController");
            }
            builderUI.robotController = controller;
            
            Debug.Log("RobotBuilderUI component configured");
        }
        
        private void Start()
        {
            if (autoSetupOnStart && Application.isPlaying)
            {
                SetupUI();
            }
        }
    }
}
