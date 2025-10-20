using UnityEngine;

namespace FTCSim.Core.Data
{
    /// <summary>
    /// Top-level container for all data that defines a robot, used for serialization.
    /// </summary>
    [System.Serializable]
    public class RobotData
    {
        // TODO: Add AssemblyGraphData
        public ElectroGraphData electroGraphData;

        public RobotData()
        {
            electroGraphData = new ElectroGraphData();
        }
    }
}
