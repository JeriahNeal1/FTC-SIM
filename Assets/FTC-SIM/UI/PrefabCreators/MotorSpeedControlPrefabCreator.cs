using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// Helper script to create the MotorSpeedControl prefab programmatically.
    /// </summary>
    [ExecuteInEditMode]
    public class MotorSpeedControlPrefabCreator : MonoBehaviour
    {
        [ContextMenu("Create Motor Speed Control Prefab")]
        public void CreatePrefab()
        {
            GameObject controlObj = new GameObject("MotorSpeedControl");
            
            RectTransform rect = controlObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 120);
            
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
            titleText.text = "Motor Speed";
            titleText.fontSize = 14;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = Color.white;
            
            LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 20;
            
            // Speed row container
            GameObject speedRow = new GameObject("SpeedRow");
            speedRow.transform.SetParent(controlObj.transform, false);
            HorizontalLayoutGroup speedRowLayout = speedRow.AddComponent<HorizontalLayoutGroup>();
            speedRowLayout.spacing = 10;
            speedRowLayout.childForceExpandWidth = true;
            
            LayoutElement speedRowLayoutElement = speedRow.AddComponent<LayoutElement>();
            speedRowLayoutElement.preferredHeight = 25;
            
            // Speed Label
            GameObject speedLabelObj = new GameObject("SpeedLabel");
            speedLabelObj.transform.SetParent(speedRow.transform, false);
            TMP_Text speedLabel = speedLabelObj.AddComponent<TMP_Text>();
            speedLabel.text = "Speed:";
            speedLabel.fontSize = 12;
            speedLabel.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            
            LayoutElement speedLabelLayout = speedLabelObj.AddComponent<LayoutElement>();
            speedLabelLayout.preferredWidth = 60;
            
            // Speed Slider
            GameObject sliderObj = new GameObject("SpeedSlider");
            sliderObj.transform.SetParent(speedRow.transform, false);
            
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(0, 20);
            
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 0;
            
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
            fillImage.color = new Color(0.2f, 0.6f, 0.9f, 1f);
            
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
            
            // Speed Value Text
            GameObject valueObj = new GameObject("SpeedValueText");
            valueObj.transform.SetParent(speedRow.transform, false);
            TMP_Text valueText = valueObj.AddComponent<TMP_Text>();
            valueText.text = "0%";
            valueText.fontSize = 12;
            valueText.color = Color.white;
            valueText.alignment = TextAlignmentOptions.Right;
            
            LayoutElement valueLayout = valueObj.AddComponent<LayoutElement>();
            valueLayout.preferredWidth = 50;
            
            // Reverse Toggle
            GameObject toggleObj = new GameObject("ReverseToggle");
            toggleObj.transform.SetParent(controlObj.transform, false);
            
            HorizontalLayoutGroup toggleLayout = toggleObj.AddComponent<HorizontalLayoutGroup>();
            toggleLayout.spacing = 10;
            
            LayoutElement toggleLayoutElement = toggleObj.AddComponent<LayoutElement>();
            toggleLayoutElement.preferredHeight = 25;
            
            GameObject toggleBox = new GameObject("Toggle");
            toggleBox.transform.SetParent(toggleObj.transform, false);
            RectTransform toggleRect = toggleBox.AddComponent<RectTransform>();
            toggleRect.sizeDelta = new Vector2(20, 20);
            
            Toggle toggle = toggleBox.AddComponent<Toggle>();
            
            Image toggleBgImage = toggleBox.AddComponent<Image>();
            toggleBgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            GameObject toggleCheckmark = new GameObject("Checkmark");
            toggleCheckmark.transform.SetParent(toggleBox.transform, false);
            RectTransform checkmarkRect = toggleCheckmark.AddComponent<RectTransform>();
            checkmarkRect.anchorMin = Vector2.zero;
            checkmarkRect.anchorMax = Vector2.one;
            checkmarkRect.sizeDelta = Vector2.zero;
            Image checkmarkImage = toggleCheckmark.AddComponent<Image>();
            checkmarkImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            
            toggle.targetGraphic = checkmarkImage;
            toggle.graphic = checkmarkImage;
            toggle.isOn = false;
            
            GameObject toggleLabel = new GameObject("Label");
            toggleLabel.transform.SetParent(toggleObj.transform, false);
            TMP_Text toggleLabelText = toggleLabel.AddComponent<TMP_Text>();
            toggleLabelText.text = "Reverse Direction";
            toggleLabelText.fontSize = 12;
            toggleLabelText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            
            // Add MotorSpeedControl component
            MotorSpeedControl motorControl = controlObj.AddComponent<MotorSpeedControl>();
            motorControl.speedSlider = slider;
            motorControl.speedValueText = valueText;
            motorControl.reverseToggle = toggle;
            
            Debug.Log("MotorSpeedControl prefab created! Save this as a prefab in your project.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = controlObj;
            #endif
        }
    }
}
