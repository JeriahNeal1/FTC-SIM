using UnityEngine;
using FTCSim.Core.Data;

namespace FTCSim.Core.Services
{
    /// <summary>
    /// Handles saving and loading of robot assemblies and ElectroGraph data to/from JSON files.
    /// </summary>
    public class SaveLoadService : MonoBehaviour
    {
        public void SaveRobot(string filePath, ElectroGraphData electroGraphData)
        {
            // TODO: Serialize assembly and electrograph data to a JSON file.
        }

        public void LoadRobot(string filePath)
        {
            // TODO: Deserialize robot data from JSON and reconstruct the robot.
        }
    }
}
