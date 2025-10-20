using UnityEngine;
using UnityEngine.UI;
using FTCSIM.Core;
using FTCSIM.Assembly;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// UI control for adjusting motor speed/power.
    /// </summary>
    public class MotorSpeedControl : MonoBehaviour
    {
        public Slider speedSlider;
        public TMP_Text speedValueText;
        public Toggle reverseToggle;
        
        private AssemblyNode motorNode;
        private RobotController robotController;
        private float currentSpeed = 0f;
        
        public void Initialize(AssemblyNode node, RobotController controller)
        {
            motorNode = node;
            robotController = controller;
            
            if (speedSlider != null)
            {
                speedSlider.value = 0f;
                speedSlider.onValueChanged.AddListener(OnSpeedChanged);
            }
            
            if (reverseToggle != null)
            {
                reverseToggle.onValueChanged.AddListener(OnReverseChanged);
            }
            
            UpdateDisplay();
        }
        
        private void OnSpeedChanged(float value)
        {
            currentSpeed = value;
            UpdateDisplay();
            ApplyMotorSpeed();
        }
        
        private void OnReverseChanged(bool isReversed)
        {
            ApplyMotorSpeed();
        }
        
        private void UpdateDisplay()
        {
            if (speedValueText != null)
            {
                float displaySpeed = currentSpeed;
                if (reverseToggle != null && reverseToggle.isOn)
                {
                    displaySpeed *= -1f;
                }
                speedValueText.text = $"{displaySpeed:F1}%";
            }
        }
        
        private void ApplyMotorSpeed()
        {
            if (motorNode == null || robotController == null) return;
            
            float speed = currentSpeed / 100f;
            if (reverseToggle != null && reverseToggle.isOn)
            {
                speed *= -1f;
            }
            
            // Apply to physics bridge
            if (robotController.physicsBridge != null)
            {
                // Convert speed to torque (simplified)
                float torque = speed * 10f; // Placeholder conversion
                robotController.physicsBridge.SetMotorTorque(motorNode.id, torque);
            }
        }
    }
}
