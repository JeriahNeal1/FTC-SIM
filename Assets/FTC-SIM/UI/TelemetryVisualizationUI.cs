using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FTCSIM.Core;
using System.Collections.Generic;

namespace FTCSIM.UI
{
    /// <summary>
    /// Real-time telemetry visualization UI for monitoring robot performance.
    /// Displays voltage, current, RPM, and other metrics in real-time.
    /// </summary>
    public class TelemetryVisualizationUI : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_Text voltageText;
        public TMP_Text currentText;
        public TMP_Text powerText;
        public TMP_Text temperatureText;
        public Slider batteryLevelSlider;
        public Image batteryFillImage;
        
        [Header("Motor Telemetry")]
        public Transform motorsContainer;
        public GameObject motorTelemetryItemPrefab;
        
        [Header("Update Settings")]
        public float updateInterval = 0.1f; // Update 10 times per second
        
        [Header("References")]
        public RobotController robotController;
        
        private float updateTimer = 0f;
        private Dictionary<string, GameObject> motorTelemetryItems = new Dictionary<string, GameObject>();
        
        private void Start()
        {
            if (robotController == null)
            {
                robotController = FindObjectOfType<RobotController>();
            }
            
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            if (batteryFillImage != null)
            {
                batteryFillImage.color = Color.green;
            }
        }
        
        private void Update()
        {
            updateTimer += Time.deltaTime;
            
            if (updateTimer >= updateInterval)
            {
                updateTimer = 0f;
                UpdateTelemetryDisplay();
            }
        }
        
        private void UpdateTelemetryDisplay()
        {
            if (robotController == null || robotController.telemetrySystem == null) return;
            
            var telemetry = robotController.telemetrySystem;
            
            // Update main telemetry values
            UpdateMainTelemetry(telemetry);
            
            // Update motor telemetry
            UpdateMotorTelemetry(telemetry);
        }
        
        private void UpdateMainTelemetry(TelemetrySystem telemetry)
        {
            // Get latest telemetry data
            float voltage = 12f; // Default battery voltage
            float current = 0f;
            float power = 0f;
            float temperature = 25f; // Default temperature
            
            // Try to get actual values from telemetry
            var latestData = telemetry.GetLatestData();
            if (latestData != null && latestData.ContainsKey("voltage"))
            {
                voltage = (float)latestData["voltage"];
            }
            if (latestData != null && latestData.ContainsKey("current"))
            {
                current = (float)latestData["current"];
            }
            
            power = voltage * current;
            
            // Update voltage text
            if (voltageText != null)
            {
                voltageText.text = $"{voltage:F2}V";
                
                // Color code based on voltage level
                if (voltage < 10f)
                {
                    voltageText.color = Color.red;
                }
                else if (voltage < 11f)
                {
                    voltageText.color = Color.yellow;
                }
                else
                {
                    voltageText.color = Color.green;
                }
            }
            
            // Update current text
            if (currentText != null)
            {
                currentText.text = $"{current:F2}A";
                
                // Color code based on current draw
                if (current > 20f)
                {
                    currentText.color = Color.red;
                }
                else if (current > 15f)
                {
                    currentText.color = Color.yellow;
                }
                else
                {
                    currentText.color = Color.white;
                }
            }
            
            // Update power text
            if (powerText != null)
            {
                powerText.text = $"{power:F1}W";
            }
            
            // Update temperature text
            if (temperatureText != null)
            {
                temperatureText.text = $"{temperature:F1}°C";
                
                // Color code based on temperature
                if (temperature > 80f)
                {
                    temperatureText.color = Color.red;
                }
                else if (temperature > 60f)
                {
                    temperatureText.color = Color.yellow;
                }
                else
                {
                    temperatureText.color = Color.white;
                }
            }
            
            // Update battery level slider
            if (batteryLevelSlider != null)
            {
                float batteryPercent = Mathf.InverseLerp(9f, 12.6f, voltage);
                batteryLevelSlider.value = batteryPercent;
                
                // Update battery fill color
                if (batteryFillImage != null)
                {
                    if (batteryPercent < 0.2f)
                    {
                        batteryFillImage.color = Color.red;
                    }
                    else if (batteryPercent < 0.5f)
                    {
                        batteryFillImage.color = Color.yellow;
                    }
                    else
                    {
                        batteryFillImage.color = Color.green;
                    }
                }
            }
        }
        
        private void UpdateMotorTelemetry(TelemetrySystem telemetry)
        {
            if (motorsContainer == null || motorTelemetryItemPrefab == null) return;
            
            // This would iterate through all motors and update their telemetry displays
            // For now, we'll create a placeholder structure
            
            // Get motor data from physics bridge
            if (robotController.physicsBridge == null) return;
            
            // Example: Update existing motor items or create new ones
            // This is a simplified version - in a real implementation, you'd query actual motor data
            
            foreach (var node in robotController.assemblyGraph.GetAllNodes())
            {
                var partDef = PartsLibrary.Instance.GetPart(node.partSKU);
                if (partDef != null && partDef.electronicsProfile != null && 
                    partDef.electronicsProfile.deviceType == "DCMotor")
                {
                    UpdateMotorTelemetryItem(node.id, node.displayName, 0f, 0f);
                }
            }
        }
        
        private void UpdateMotorTelemetryItem(string motorId, string motorName, float rpm, float torque)
        {
            if (motorsContainer == null) return;
            
            GameObject item;
            if (!motorTelemetryItems.ContainsKey(motorId))
            {
                // Create new telemetry item
                if (motorTelemetryItemPrefab != null)
                {
                    item = Instantiate(motorTelemetryItemPrefab, motorsContainer);
                    motorTelemetryItems[motorId] = item;
                }
                else
                {
                    return;
                }
            }
            else
            {
                item = motorTelemetryItems[motorId];
            }
            
            // Update the item's display
            var texts = item.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = motorName;
                texts[1].text = $"{rpm:F0} RPM";
                texts[2].text = $"{torque:F2} N⋅m";
            }
        }
        
        public void ToggleVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
