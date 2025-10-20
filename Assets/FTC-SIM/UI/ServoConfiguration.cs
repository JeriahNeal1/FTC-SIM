using UnityEngine;
using UnityEngine.UI;
using FTCSIM.Core;
using FTCSIM.Assembly;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// UI control for configuring servo position and speed-torque ratio.
    /// </summary>
    public class ServoConfiguration : MonoBehaviour
    {
        public Slider positionSlider;
        public TMP_Text positionValueText;
        public Slider speedSlider;
        public TMP_Text speedValueText;
        public Slider torqueSlider;
        public TMP_Text torqueValueText;
        
        private AssemblyNode servoNode;
        private RobotController robotController;
        
        private float targetPosition = 90f; // degrees
        private float speed = 100f; // percentage
        private float torque = 50f; // percentage
        
        public void Initialize(AssemblyNode node, RobotController controller)
        {
            servoNode = node;
            robotController = controller;
            
            if (positionSlider != null)
            {
                positionSlider.minValue = 0f;
                positionSlider.maxValue = 180f;
                positionSlider.value = targetPosition;
                positionSlider.onValueChanged.AddListener(OnPositionChanged);
            }
            
            if (speedSlider != null)
            {
                speedSlider.minValue = 0f;
                speedSlider.maxValue = 100f;
                speedSlider.value = speed;
                speedSlider.onValueChanged.AddListener(OnSpeedChanged);
            }
            
            if (torqueSlider != null)
            {
                torqueSlider.minValue = 0f;
                torqueSlider.maxValue = 100f;
                torqueSlider.value = torque;
                torqueSlider.onValueChanged.AddListener(OnTorqueChanged);
            }
            
            UpdateDisplay();
        }
        
        private void OnPositionChanged(float value)
        {
            targetPosition = value;
            UpdateDisplay();
            ApplyServoConfiguration();
        }
        
        private void OnSpeedChanged(float value)
        {
            speed = value;
            UpdateDisplay();
            ApplyServoConfiguration();
        }
        
        private void OnTorqueChanged(float value)
        {
            torque = value;
            UpdateDisplay();
            ApplyServoConfiguration();
        }
        
        private void UpdateDisplay()
        {
            if (positionValueText != null)
            {
                positionValueText.text = $"{targetPosition:F0}°";
            }
            
            if (speedValueText != null)
            {
                speedValueText.text = $"{speed:F0}%";
            }
            
            if (torqueValueText != null)
            {
                torqueValueText.text = $"{torque:F0}%";
            }
        }
        
        private void ApplyServoConfiguration()
        {
            if (servoNode == null || robotController == null) return;
            
            // Apply to physics bridge
            if (robotController.physicsBridge != null)
            {
                // Convert position to torque command (simplified)
                float targetTorque = (speed / 100f) * (torque / 100f) * 5f;
                robotController.physicsBridge.SetMotorTorque(servoNode.id, targetTorque);
            }
        }
    }
}
