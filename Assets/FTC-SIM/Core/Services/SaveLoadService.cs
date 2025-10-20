using UnityEngine;
using FTCSim.Core.Data;
using System.IO;

namespace FTCSim.Core.Services
{
    /// <summary>
    /// Handles saving and loading of robot assemblies and ElectroGraph data to/from JSON files.
    /// </summary>
    public class SaveLoadService : MonoBehaviour
    {
        public void SaveRobot(string filePath, RobotData robotData)
        {
            string json = JsonUtility.ToJson(robotData, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Robot saved to {filePath}");
        }

        public RobotData LoadRobot(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                RobotData robotData = JsonUtility.FromJson<RobotData>(json);
                Debug.Log($"Robot loaded from {filePath}");
                return robotData;
            }
            else
            {
                Debug.LogError($"Save file not found at {filePath}");
                return null;
            }
        }
    }
}
