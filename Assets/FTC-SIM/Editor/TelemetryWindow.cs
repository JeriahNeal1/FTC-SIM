using UnityEngine;
using UnityEditor;
using FTCSIM.Core;
using System.Collections.Generic;

namespace FTCSIM.Editor
{
    /// <summary>
    /// Unity Editor window for monitoring telemetry data in real-time.
    /// Displays voltage, current, RPM, and other metrics.
    /// </summary>
    public class TelemetryWindow : EditorWindow
    {
        private RobotController controller;
        private TelemetrySystem telemetry;
        private Vector2 scrollPosition;
        
        private bool autoRefresh = true;
        private double lastRefreshTime = 0;
        private const double refreshInterval = 0.1; // 10Hz
        
        [MenuItem("FTC-SIM/Telemetry Window")]
        public static void ShowWindow()
        {
            TelemetryWindow window = GetWindow<TelemetryWindow>("Telemetry");
            window.minSize = new Vector2(400, 400);
            window.Show();
        }
        
        private void OnEnable()
        {
            FindController();
            EditorApplication.update += OnEditorUpdate;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }
        
        private void OnEditorUpdate()
        {
            if (autoRefresh && Application.isPlaying)
            {
                if (EditorApplication.timeSinceStartup - lastRefreshTime > refreshInterval)
                {
                    Repaint();
                    lastRefreshTime = EditorApplication.timeSinceStartup;
                }
            }
        }
        
        private void FindController()
        {
            controller = FindObjectOfType<RobotController>();
            if (controller != null)
            {
                telemetry = controller.GetComponent<TelemetrySystem>();
                if (telemetry == null)
                {
                    telemetry = controller.gameObject.AddComponent<TelemetrySystem>();
                }
            }
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            // Header
            DrawHeader();
            
            // Status
            DrawStatus();
            
            EditorGUILayout.Space(10);
            
            // Telemetry data
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            if (Application.isPlaying && telemetry != null)
            {
                DrawTelemetryData();
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play mode to view telemetry data.", MessageType.Info);
            }
            
            EditorGUILayout.EndScrollView();
            
            // Controls
            DrawControls();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("FTC-SIM Telemetry Monitor", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            autoRefresh = GUILayout.Toggle(autoRefresh, "Auto Refresh", EditorStyles.toolbarButton, GUILayout.Width(100));
            
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                FindController();
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawStatus()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (controller == null)
            {
                EditorGUILayout.HelpBox("No RobotController found in the scene.", MessageType.Warning);
                
                if (GUILayout.Button("Find Controller", GUILayout.Height(25)))
                {
                    FindController();
                }
            }
            else if (telemetry == null)
            {
                EditorGUILayout.HelpBox("No TelemetrySystem found. Adding component...", MessageType.Info);
                FindController();
            }
            else
            {
                EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Logging:", telemetry.isLogging ? "Active" : "Inactive");
                EditorGUILayout.LabelField("Interval:", telemetry.loggingInterval.ToString("F3") + "s");
                
                var data = telemetry.GetLatestData();
                if (data != null)
                {
                    EditorGUILayout.LabelField("Latest Sample:", data.timestamp.ToString("F2") + "s");
                }
                
                var log = telemetry.GetDataLog();
                EditorGUILayout.LabelField("Samples Logged:", log.Count.ToString());
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawTelemetryData()
        {
            var data = telemetry.GetLatestData();
            
            if (data == null)
            {
                EditorGUILayout.HelpBox("No telemetry data available yet. Start logging to collect data.", MessageType.Info);
                return;
            }
            
            // Battery metrics
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Battery", EditorStyles.boldLabel);
            
            DrawMetric("Voltage", data.voltage, "V", Color.green);
            DrawMetric("Current", data.current, "A", Color.yellow);
            DrawMetric("Power", data.power, "W", Color.red);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
            
            // Motor metrics
            if (data.motorRPMs.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Motors", EditorStyles.boldLabel);
                
                foreach (var kvp in data.motorRPMs)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(kvp.Key + " RPM:", GUILayout.Width(120));
                    EditorGUILayout.LabelField(kvp.Value.ToString("F1") + " RPM");
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(5);
            }
            
            // Motor currents
            if (data.motorCurrents.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Motor Currents", EditorStyles.boldLabel);
                
                foreach (var kvp in data.motorCurrents)
                {
                    DrawMetric(kvp.Key, kvp.Value, "A", Color.cyan);
                }
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(5);
            }
            
            // Temperatures
            if (data.temperatures.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Temperatures", EditorStyles.boldLabel);
                
                foreach (var kvp in data.temperatures)
                {
                    Color tempColor = kvp.Value > 60 ? Color.red : (kvp.Value > 40 ? Color.yellow : Color.green);
                    DrawMetric(kvp.Key, kvp.Value, "°C", tempColor);
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawMetric(string label, float value, string unit, Color barColor)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(label + ":", GUILayout.Width(100));
            EditorGUILayout.LabelField(value.ToString("F2") + " " + unit, GUILayout.Width(80));
            
            // Simple bar visualization
            Rect barRect = GUILayoutUtility.GetRect(100, 18);
            float normalizedValue = Mathf.Clamp01(value / 15f); // Normalize to 0-15 range
            
            EditorGUI.DrawRect(barRect, Color.gray);
            Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * normalizedValue, barRect.height);
            EditorGUI.DrawRect(fillRect, barColor);
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawControls()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Controls available in Play mode only.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }
            
            if (telemetry == null)
            {
                EditorGUILayout.HelpBox("TelemetrySystem not found.", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            
            if (!telemetry.isLogging)
            {
                if (GUILayout.Button("Start Logging", GUILayout.Height(30)))
                {
                    telemetry.StartLogging();
                }
            }
            else
            {
                if (GUILayout.Button("Stop Logging", GUILayout.Height(30)))
                {
                    telemetry.StopLogging();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Export controls
            if (GUILayout.Button("Export to CSV", GUILayout.Height(25)))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Export Telemetry Data",
                    Application.persistentDataPath,
                    "telemetry_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    "csv"
                );
                
                if (!string.IsNullOrEmpty(path))
                {
                    telemetry.ExportToCSV(path);
                    EditorUtility.DisplayDialog("Success", "Telemetry data exported successfully!", "OK");
                }
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}
