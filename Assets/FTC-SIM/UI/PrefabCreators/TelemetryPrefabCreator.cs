using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// Helper script to create telemetry visualization UI prefabs.
    /// </summary>
    [ExecuteInEditMode]
    public class TelemetryPrefabCreator : MonoBehaviour
    {
        [ContextMenu("Create Telemetry Panel")]
        public void CreateTelemetryPanel()
        {
            GameObject panel = new GameObject("TelemetryPanel");
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(10, -10);
            rect.sizeDelta = new Vector2(300, 250);
            
            // Background
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Layout
            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 15, 15);
            layout.spacing = 10;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panel.transform, false);
            TMP_Text title = titleObj.AddComponent<TMP_Text>();
            title.text = "Telemetry";
            title.fontSize = 18;
            title.fontStyle = FontStyles.Bold;
            title.color = Color.white;
            title.alignment = TextAlignmentOptions.Center;
            
            LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 25;
            
            // Battery Section
            CreateBatterySection(panel.transform);
            
            // System Metrics
            CreateMetricsSection(panel.transform);
            
            // Motors Container
            GameObject motorsContainer = new GameObject("MotorsContainer");
            motorsContainer.transform.SetParent(panel.transform, false);
            
            VerticalLayoutGroup motorsLayout = motorsContainer.AddComponent<VerticalLayoutGroup>();
            motorsLayout.spacing = 3;
            motorsLayout.childForceExpandWidth = true;
            motorsLayout.childForceExpandHeight = false;
            
            LayoutElement motorsLayoutElement = motorsContainer.AddComponent<LayoutElement>();
            motorsLayoutElement.preferredHeight = 80;
            motorsLayoutElement.flexibleHeight = 1;
            
            // Add TelemetryVisualizationUI component
            TelemetryVisualizationUI telemetryUI = panel.AddComponent<TelemetryVisualizationUI>();
            telemetryUI.motorsContainer = motorsContainer.transform;
            
            // Find and assign UI elements
            TMP_Text[] texts = panel.GetComponentsInChildren<TMP_Text>();
            foreach (var text in texts)
            {
                if (text.name == "VoltageText") telemetryUI.voltageText = text;
                else if (text.name == "CurrentText") telemetryUI.currentText = text;
                else if (text.name == "PowerText") telemetryUI.powerText = text;
                else if (text.name == "TemperatureText") telemetryUI.temperatureText = text;
            }
            
            Slider slider = panel.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                telemetryUI.batteryLevelSlider = slider;
                telemetryUI.batteryFillImage = slider.fillRect?.GetComponent<Image>();
            }
            
            Debug.Log("Telemetry Panel created! Save as prefab or use in scene.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = panel;
            #endif
        }
        
        private void CreateBatterySection(Transform parent)
        {
            GameObject batterySection = new GameObject("BatterySection");
            batterySection.transform.SetParent(parent, false);
            
            VerticalLayoutGroup layout = batterySection.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.childForceExpandWidth = true;
            
            LayoutElement sectionLayout = batterySection.AddComponent<LayoutElement>();
            sectionLayout.preferredHeight = 60;
            
            // Battery label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(batterySection.transform, false);
            TMP_Text label = labelObj.AddComponent<TMP_Text>();
            label.text = "Battery";
            label.fontSize = 14;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            
            // Battery slider
            GameObject sliderObj = new GameObject("BatteryLevelSlider");
            sliderObj.transform.SetParent(batterySection.transform, false);
            
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(0, 20);
            
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
            slider.interactable = false;
            
            // Slider background
            GameObject sliderBg = new GameObject("Background");
            sliderBg.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRect = sliderBg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = sliderBg.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Slider fill
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;
            
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = Color.green;
            
            slider.fillRect = fillRect;
        }
        
        private void CreateMetricsSection(Transform parent)
        {
            GameObject metricsSection = new GameObject("MetricsSection");
            metricsSection.transform.SetParent(parent, false);
            
            VerticalLayoutGroup layout = metricsSection.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 3;
            layout.childForceExpandWidth = true;
            
            LayoutElement sectionLayout = metricsSection.AddComponent<LayoutElement>();
            sectionLayout.preferredHeight = 80;
            
            // Create metric rows
            CreateMetricRow(metricsSection.transform, "Voltage:", "VoltageText", "12.0V");
            CreateMetricRow(metricsSection.transform, "Current:", "CurrentText", "0.0A");
            CreateMetricRow(metricsSection.transform, "Power:", "PowerText", "0.0W");
            CreateMetricRow(metricsSection.transform, "Temp:", "TemperatureText", "25.0°C");
        }
        
        private void CreateMetricRow(Transform parent, string labelText, string valueName, string defaultValue)
        {
            GameObject row = new GameObject(valueName.Replace("Text", "Row"));
            row.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childForceExpandWidth = true;
            
            LayoutElement rowLayout = row.AddComponent<LayoutElement>();
            rowLayout.preferredHeight = 18;
            
            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(row.transform, false);
            TMP_Text label = labelObj.AddComponent<TMP_Text>();
            label.text = labelText;
            label.fontSize = 12;
            label.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            label.alignment = TextAlignmentOptions.Left;
            
            LayoutElement labelLayout = labelObj.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 80;
            
            // Value
            GameObject valueObj = new GameObject(valueName);
            valueObj.transform.SetParent(row.transform, false);
            TMP_Text value = valueObj.AddComponent<TMP_Text>();
            value.text = defaultValue;
            value.fontSize = 12;
            value.fontStyle = FontStyles.Bold;
            value.color = Color.white;
            value.alignment = TextAlignmentOptions.Right;
            
            LayoutElement valueLayout = valueObj.AddComponent<LayoutElement>();
            valueLayout.flexibleWidth = 1;
        }
        
        [ContextMenu("Create Motor Telemetry Item")]
        public void CreateMotorTelemetryItem()
        {
            GameObject item = new GameObject("MotorTelemetryItem");
            
            RectTransform rect = item.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 60);
            
            // Background
            Image bg = item.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // Layout
            VerticalLayoutGroup layout = item.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.spacing = 3;
            layout.childForceExpandWidth = true;
            
            // Motor name
            GameObject nameObj = new GameObject("MotorName");
            nameObj.transform.SetParent(item.transform, false);
            TMP_Text name = nameObj.AddComponent<TMP_Text>();
            name.text = "Motor Name";
            name.fontSize = 11;
            name.fontStyle = FontStyles.Bold;
            name.color = Color.white;
            
            LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
            nameLayout.preferredHeight = 15;
            
            // RPM
            GameObject rpmRow = new GameObject("RPMRow");
            rpmRow.transform.SetParent(item.transform, false);
            HorizontalLayoutGroup rpmLayout = rpmRow.AddComponent<HorizontalLayoutGroup>();
            rpmLayout.spacing = 5;
            
            GameObject rpmLabel = new GameObject("Label");
            rpmLabel.transform.SetParent(rpmRow.transform, false);
            TMP_Text rpmLabelText = rpmLabel.AddComponent<TMP_Text>();
            rpmLabelText.text = "RPM:";
            rpmLabelText.fontSize = 10;
            rpmLabelText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            
            GameObject rpmValue = new GameObject("RPMValue");
            rpmValue.transform.SetParent(rpmRow.transform, false);
            TMP_Text rpmValueText = rpmValue.AddComponent<TMP_Text>();
            rpmValueText.text = "0";
            rpmValueText.fontSize = 10;
            rpmValueText.color = new Color(0.3f, 0.7f, 1f, 1f);
            rpmValueText.alignment = TextAlignmentOptions.Right;
            
            // Torque
            GameObject torqueRow = new GameObject("TorqueRow");
            torqueRow.transform.SetParent(item.transform, false);
            HorizontalLayoutGroup torqueLayout = torqueRow.AddComponent<HorizontalLayoutGroup>();
            torqueLayout.spacing = 5;
            
            GameObject torqueLabel = new GameObject("Label");
            torqueLabel.transform.SetParent(torqueRow.transform, false);
            TMP_Text torqueLabelText = torqueLabel.AddComponent<TMP_Text>();
            torqueLabelText.text = "Torque:";
            torqueLabelText.fontSize = 10;
            torqueLabelText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            
            GameObject torqueValue = new GameObject("TorqueValue");
            torqueValue.transform.SetParent(torqueRow.transform, false);
            TMP_Text torqueValueText = torqueValue.AddComponent<TMP_Text>();
            torqueValueText.text = "0.00 N⋅m";
            torqueValueText.fontSize = 10;
            torqueValueText.color = new Color(1f, 0.7f, 0.3f, 1f);
            torqueValueText.alignment = TextAlignmentOptions.Right;
            
            Debug.Log("Motor Telemetry Item created! Save as prefab.");
            
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = item;
            #endif
        }
    }
}
