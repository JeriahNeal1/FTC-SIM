using UnityEngine;
using System.Collections.Generic;

namespace FTCSIM.Core
{
    /// <summary>
    /// Telemetry system for monitoring robot performance.
    /// Tracks voltage, current, RPM, temperature, and other metrics.
    /// </summary>
    public class TelemetrySystem : MonoBehaviour
    {
        [System.Serializable]
        public class TelemetryData
        {
            public float timestamp;
            public float voltage;
            public float current;
            public float power;
            public Dictionary<string, float> motorRPMs = new Dictionary<string, float>();
            public Dictionary<string, float> motorCurrents = new Dictionary<string, float>();
            public Dictionary<string, float> temperatures = new Dictionary<string, float>();
        }
        
        private List<TelemetryData> dataLog = new List<TelemetryData>();
        private const int maxLogSize = 10000; // Keep last 10,000 samples
        
        public bool isLogging = false;
        public float loggingInterval = 0.1f; // 10Hz
        private float logTimer = 0f;
        
        private RobotController robotController;
        
        private void Awake()
        {
            robotController = GetComponentInParent<RobotController>();
        }
        
        private void Update()
        {
            if (!isLogging) return;
            
            logTimer += Time.deltaTime;
            if (logTimer >= loggingInterval)
            {
                CaptureDataPoint();
                logTimer = 0f;
            }
        }
        
        private void CaptureDataPoint()
        {
            if (robotController == null || robotController.electroGraph == null)
            {
                return;
            }
            
            var data = new TelemetryData
            {
                timestamp = Time.time
            };
            
            // Capture battery voltage and current
            var batteryNode = robotController.electroGraph.nodes.Find(n => n.type == "Battery");
            if (batteryNode != null)
            {
                var pwrPin = batteryNode.pins.Find(p => p.role == Electronics.ElectroPin.PinRole.PWR);
                if (pwrPin != null)
                {
                    data.voltage = pwrPin.voltage;
                    data.current = Mathf.Abs(pwrPin.current);
                    data.power = data.voltage * data.current;
                }
            }
            
            // Capture motor data
            foreach (var node in robotController.electroGraph.nodes)
            {
                if (node.type == "DCMotor" && !string.IsNullOrEmpty(node.assemblyNodeId))
                {
                    // Get RPM from physics bridge
                    float velocity = robotController.physicsBridge.GetJointVelocity(node.assemblyNodeId);
                    float rpm = velocity * 60f / (2f * Mathf.PI); // Convert rad/s to RPM
                    data.motorRPMs[node.id] = rpm;
                    
                    // Get current draw
                    var plusPin = node.pins.Find(p => p.id == "plus" || p.id == "motor_plus");
                    if (plusPin != null)
                    {
                        data.motorCurrents[node.id] = Mathf.Abs(plusPin.current);
                    }
                    
                    // Simplified temperature model (placeholder)
                    data.temperatures[node.id] = 25f + (data.motorCurrents.ContainsKey(node.id) ? 
                        data.motorCurrents[node.id] * 2f : 0f);
                }
            }
            
            // Add to log
            dataLog.Add(data);
            
            // Trim log if too large
            if (dataLog.Count > maxLogSize)
            {
                dataLog.RemoveAt(0);
            }
        }
        
        public void StartLogging()
        {
            isLogging = true;
            dataLog.Clear();
            Debug.Log("Telemetry logging started");
        }
        
        public void StopLogging()
        {
            isLogging = false;
            Debug.Log($"Telemetry logging stopped. Captured {dataLog.Count} samples");
        }
        
        public List<TelemetryData> GetDataLog()
        {
            return dataLog;
        }
        
        public void ExportToCSV(string filePath)
        {
            if (dataLog.Count == 0)
            {
                Debug.LogWarning("No telemetry data to export");
                return;
            }
            
            System.Text.StringBuilder csv = new System.Text.StringBuilder();
            
            // Header
            csv.AppendLine("Timestamp,Voltage,Current,Power");
            
            // Data rows
            foreach (var data in dataLog)
            {
                csv.AppendLine($"{data.timestamp},{data.voltage},{data.current},{data.power}");
            }
            
            try
            {
                System.IO.File.WriteAllText(filePath, csv.ToString());
                Debug.Log($"Telemetry exported to: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to export telemetry: {e.Message}");
            }
        }
        
        public TelemetryData GetLatestData()
        {
            return dataLog.Count > 0 ? dataLog[dataLog.Count - 1] : null;
        }
    }
}
