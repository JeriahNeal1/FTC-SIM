using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// Helper script to create ElectroGraph visual editor UI prefabs.
    /// Creates node, pin, and wire UI elements for the visual electronics editor.
    /// </summary>
    [ExecuteInEditMode]
    public class ElectroGraphUIPrefabCreator : MonoBehaviour
    {
        [ContextMenu("Create ElectroGraph Node UI Prefab")]
        public void CreateNodePrefab()
        {
            GameObject nodeObj = new GameObject("ElectroNodeUI");
            
            RectTransform rect = nodeObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 150);
            
            // Background
            Image bg = nodeObj.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.2f, 0.25f, 1f);
            
            // Add CanvasGroup for dragging
            CanvasGroup canvasGroup = nodeObj.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            
            // Header
            GameObject header = new GameObject("Header");
            header.transform.SetParent(nodeObj.transform, false);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 30);
            
            Image headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0.1f, 0.15f, 0.2f, 1f);
            
            // Node Name Text
            GameObject nameObj = new GameObject("NodeNameText");
            nameObj.transform.SetParent(header.transform, false);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = Vector2.zero;
            nameRect.anchorMax = Vector2.one;
            nameRect.sizeDelta = Vector2.zero;
            
            TMP_Text nameText = nameObj.AddComponent<TMP_Text>();
            nameText.text = "Node Name";
            nameText.fontSize = 14;
            nameText.fontStyle = FontStyles.Bold;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.white;
            
            // Node Type Text
            GameObject typeObj = new GameObject("NodeTypeText");
            typeObj.transform.SetParent(nodeObj.transform, false);
            RectTransform typeRect = typeObj.AddComponent<RectTransform>();
            typeRect.anchorMin = new Vector2(0, 1);
            typeRect.anchorMax = new Vector2(1, 1);
            typeRect.pivot = new Vector2(0.5f, 1);
            typeRect.anchoredPosition = new Vector2(0, -35);
            typeRect.sizeDelta = new Vector2(-10, 20);
            
            TMP_Text typeText = typeObj.AddComponent<TMP_Text>();
            typeText.text = "Type";
            typeText.fontSize = 11;
            typeText.alignment = TextAlignmentOptions.Center;
            typeText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            
            // Pins Container
            GameObject pinsContainer = new GameObject("PinsContainer");
            pinsContainer.transform.SetParent(nodeObj.transform, false);
            RectTransform pinsRect = pinsContainer.AddComponent<RectTransform>();
            pinsRect.anchorMin = new Vector2(0, 0);
            pinsRect.anchorMax = new Vector2(1, 1);
            pinsRect.offsetMin = new Vector2(5, 5);
            pinsRect.offsetMax = new Vector2(-5, -60);
            
            VerticalLayoutGroup pinsLayout = pinsContainer.AddComponent<VerticalLayoutGroup>();
            pinsLayout.spacing = 3;
            pinsLayout.padding = new RectOffset(5, 5, 5, 5);
            pinsLayout.childForceExpandWidth = true;
            pinsLayout.childForceExpandHeight = false;
            pinsLayout.childControlHeight = true;
            
            // Add ElectroNodeUI component
            ElectroNodeUI nodeUI = nodeObj.AddComponent<ElectroNodeUI>();
            nodeUI.nodeNameText = nameText;
            nodeUI.nodeTypeText = typeText;
            nodeUI.pinsContainer = pinsContainer.transform;
            
            Debug.Log("ElectroNodeUI prefab created! Save this as a prefab.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = nodeObj;
            #endif
        }
        
        [ContextMenu("Create ElectroGraph Pin UI Prefab")]
        public void CreatePinPrefab()
        {
            GameObject pinObj = new GameObject("ElectroPinUI");
            
            RectTransform rect = pinObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 25);
            
            // Background
            Image bg = pinObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            
            HorizontalLayoutGroup layout = pinObj.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5;
            layout.padding = new RectOffset(5, 5, 2, 2);
            layout.childForceExpandWidth = true;
            layout.childAlignment = TextAnchor.MiddleLeft;
            
            // Pin indicator (colored dot)
            GameObject indicator = new GameObject("PinIndicator");
            indicator.transform.SetParent(pinObj.transform, false);
            RectTransform indicatorRect = indicator.AddComponent<RectTransform>();
            indicatorRect.sizeDelta = new Vector2(15, 15);
            
            Image indicatorImage = indicator.AddComponent<Image>();
            indicatorImage.color = Color.red; // Will be set based on pin role
            
            LayoutElement indicatorLayout = indicator.AddComponent<LayoutElement>();
            indicatorLayout.preferredWidth = 15;
            indicatorLayout.preferredHeight = 15;
            indicatorLayout.flexibleWidth = 0;
            
            // Pin Name Text
            GameObject nameObj = new GameObject("PinNameText");
            nameObj.transform.SetParent(pinObj.transform, false);
            TMP_Text nameText = nameObj.AddComponent<TMP_Text>();
            nameText.text = "pin_name";
            nameText.fontSize = 11;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.Left;
            
            // Button for clicking
            Button button = pinObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            colors.pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            button.colors = colors;
            
            // Add ElectroPinUI component
            ElectroPinUI pinUI = pinObj.AddComponent<ElectroPinUI>();
            pinUI.pinIndicator = indicatorImage;
            pinUI.pinNameText = nameText;
            
            Debug.Log("ElectroPinUI prefab created! Save this as a prefab.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = pinObj;
            #endif
        }
        
        [ContextMenu("Create ElectroGraph Wire UI Prefab")]
        public void CreateWirePrefab()
        {
            GameObject wireObj = new GameObject("ElectroWireUI");
            
            RectTransform rect = wireObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 5);
            
            // Wire line image
            Image lineImage = wireObj.AddComponent<Image>();
            lineImage.color = Color.red; // Will be set based on wire type
            lineImage.raycastTarget = false;
            
            // Add ElectroWireUI component
            ElectroWireUI wireUI = wireObj.AddComponent<ElectroWireUI>();
            wireUI.wireLineImage = lineImage;
            
            Debug.Log("ElectroWireUI prefab created! Save this as a prefab.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = wireObj;
            #endif
        }
        
        [ContextMenu("Create Complete ElectroGraph Editor Panel")]
        public void CreateElectroGraphPanel()
        {
            GameObject panel = new GameObject("ElectroGraphEditorPanel");
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            // Background
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            
            // Header
            GameObject header = new GameObject("Header");
            header.transform.SetParent(panel.transform, false);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 50);
            
            Image headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0.08f, 0.08f, 0.08f, 1f);
            
            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(header.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0, 1);
            titleRect.pivot = new Vector2(0, 0.5f);
            titleRect.anchoredPosition = new Vector2(20, 0);
            titleRect.sizeDelta = new Vector2(200, 0);
            
            TMP_Text titleText = titleObj.AddComponent<TMP_Text>();
            titleText.text = "ElectroGraph Editor";
            titleText.fontSize = 20;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Left;
            
            // Toolbar
            GameObject toolbar = new GameObject("Toolbar");
            toolbar.transform.SetParent(header.transform, false);
            RectTransform toolbarRect = toolbar.AddComponent<RectTransform>();
            toolbarRect.anchorMin = new Vector2(1, 0);
            toolbarRect.anchorMax = new Vector2(1, 1);
            toolbarRect.pivot = new Vector2(1, 0.5f);
            toolbarRect.anchoredPosition = new Vector2(-20, 0);
            toolbarRect.sizeDelta = new Vector2(400, 0);
            
            HorizontalLayoutGroup toolbarLayout = toolbar.AddComponent<HorizontalLayoutGroup>();
            toolbarLayout.spacing = 10;
            toolbarLayout.childAlignment = TextAnchor.MiddleRight;
            toolbarLayout.childForceExpandWidth = false;
            toolbarLayout.childForceExpandHeight = false;
            
            // Device Type Dropdown
            GameObject dropdownObj = new GameObject("DeviceTypeDropdown");
            dropdownObj.transform.SetParent(toolbar.transform, false);
            RectTransform dropdownRect = dropdownObj.AddComponent<RectTransform>();
            dropdownRect.sizeDelta = new Vector2(150, 30);
            
            Image dropdownBg = dropdownObj.AddComponent<Image>();
            dropdownBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            TMP_Dropdown dropdown = dropdownObj.AddComponent<TMP_Dropdown>();
            dropdown.options.Add(new TMP_Dropdown.OptionData("Battery"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Hub"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("DCMotor"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Servo"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Sensor"));
            
            // Add Node Button
            GameObject buttonObj = new GameObject("AddNodeButton");
            buttonObj.transform.SetParent(toolbar.transform, false);
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(120, 30);
            
            Image buttonBg = buttonObj.AddComponent<Image>();
            buttonBg.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            
            Button button = buttonObj.AddComponent<Button>();
            
            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.sizeDelta = Vector2.zero;
            
            TMP_Text buttonText = buttonTextObj.AddComponent<TMP_Text>();
            buttonText.text = "Add Node";
            buttonText.fontSize = 14;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            
            // Canvas Transform (where nodes are placed)
            GameObject canvas = new GameObject("NodesCanvas");
            canvas.transform.SetParent(panel.transform, false);
            RectTransform canvasRect = canvas.AddComponent<RectTransform>();
            canvasRect.anchorMin = Vector2.zero;
            canvasRect.anchorMax = Vector2.one;
            canvasRect.offsetMin = new Vector2(10, 10);
            canvasRect.offsetMax = new Vector2(-10, -60);
            
            Image canvasBg = canvas.AddComponent<Image>();
            canvasBg.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            
            // Add ElectroGraphEditor component
            ElectroGraphEditor editor = panel.AddComponent<ElectroGraphEditor>();
            editor.canvasTransform = canvasRect;
            editor.deviceTypeDropdown = dropdown;
            editor.addNodeButton = button;
            
            Debug.Log("Complete ElectroGraph Editor Panel created! Save and use this in your scene.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = panel;
            #endif
        }
    }
}
