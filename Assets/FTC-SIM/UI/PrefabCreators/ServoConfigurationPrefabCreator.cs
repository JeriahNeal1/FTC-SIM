using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// Helper script to create the ServoConfiguration prefab programmatically.
    /// </summary>
    [ExecuteInEditMode]
    public class ServoConfigurationPrefabCreator : MonoBehaviour
    {
        [ContextMenu("Create Servo Configuration Prefab")]
        public void CreatePrefab()
        {
            GameObject controlObj = new GameObject("ServoConfiguration");
            
            RectTransform rect = controlObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 200);
            
            // Background
            Image bg = controlObj.AddComponent<Image>();
            bg.color = new Color(0.18f, 0.18f, 0.18f, 1f);
            
            // Layout
            VerticalLayoutGroup layout = controlObj.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 15, 15);
            layout.spacing = 10;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(controlObj.transform, false);
            TMP_Text titleText = titleObj.AddComponent<TMP_Text>();
            titleText.text = "Servo Configuration";
            titleText.fontSize = 14;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = Color.white;
            
            LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 20;
            
            // Position Slider
            GameObject posSlider = CreateSliderRow(controlObj.transform, "Position", "PositionSlider", "PositionValueText", "90°", 0, 180, 90);
            // Speed Slider
            GameObject speedSlider = CreateSliderRow(controlObj.transform, "Speed", "SpeedSlider", "SpeedValueText", "100%", 0, 100, 100);
            // Torque Slider
            GameObject torqueSlider = CreateSliderRow(controlObj.transform, "Torque", "TorqueSlider", "TorqueValueText", "50%", 0, 100, 50);
            
            // Add ServoConfiguration component
            ServoConfiguration servoConfig = controlObj.AddComponent<ServoConfiguration>();
            servoConfig.positionSlider = posSlider.GetComponentInChildren<Slider>();
            servoConfig.speedSlider = speedSlider.GetComponentInChildren<Slider>();
            servoConfig.torqueSlider = torqueSlider.GetComponentInChildren<Slider>();
            
            TMP_Text[] texts = controlObj.GetComponentsInChildren<TMP_Text>();
            foreach (var text in texts)
            {
                if (text.name == "PositionValueText") servoConfig.positionValueText = text;
                else if (text.name == "SpeedValueText") servoConfig.speedValueText = text;
                else if (text.name == "TorqueValueText") servoConfig.torqueValueText = text;
            }
            
            Debug.Log("ServoConfiguration prefab created! Save this as a prefab in your project.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = controlObj;
            #endif
        }
        
        private GameObject CreateSliderRow(Transform parent, string label, string sliderName, string valueName, string defaultValue, float min, float max, float defaultSliderValue)
        {
            GameObject row = new GameObject(label + "Row");
            row.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup rowLayout = row.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 10;
            rowLayout.childForceExpandWidth = true;
            
            LayoutElement rowLayoutElement = row.AddComponent<LayoutElement>();
            rowLayoutElement.preferredHeight = 25;
            
            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(row.transform, false);
            TMP_Text labelText = labelObj.AddComponent<TMP_Text>();
            labelText.text = label + ":";
            labelText.fontSize = 12;
            labelText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            
            LayoutElement labelLayout = labelObj.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 70;
            
            // Slider
            GameObject sliderObj = new GameObject(sliderName);
            sliderObj.transform.SetParent(row.transform, false);
            
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(0, 20);
            
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = defaultSliderValue;
            
            // Slider Background
            GameObject sliderBg = new GameObject("Background");
            sliderBg.transform.SetParent(sliderObj.transform, false);
            RectTransform sliderBgRect = sliderBg.AddComponent<RectTransform>();
            sliderBgRect.anchorMin = Vector2.zero;
            sliderBgRect.anchorMax = Vector2.one;
            sliderBgRect.sizeDelta = Vector2.zero;
            Image sliderBgImage = sliderBg.AddComponent<Image>();
            sliderBgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            // Slider Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(10, 0);
            fillAreaRect.offsetMax = new Vector2(-10, 0);
            
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.9f, 0.5f, 0.2f, 1f);
            
            // Slider Handle Area
            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObj.transform, false);
            RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);
            
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            RectTransform handleRect = handle.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;
            
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            
            LayoutElement sliderLayout = sliderObj.AddComponent<LayoutElement>();
            sliderLayout.flexibleWidth = 1;
            
            // Value Text
            GameObject valueObj = new GameObject(valueName);
            valueObj.transform.SetParent(row.transform, false);
            TMP_Text valueText = valueObj.AddComponent<TMP_Text>();
            valueText.text = defaultValue;
            valueText.fontSize = 12;
            valueText.color = Color.white;
            valueText.alignment = TextAlignmentOptions.Right;
            
            LayoutElement valueLayout = valueObj.AddComponent<LayoutElement>();
            valueLayout.preferredWidth = 50;
            
            return row;
        }
    }
}
