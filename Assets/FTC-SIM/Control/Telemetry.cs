using UnityEngine;

namespace FTCSim.Control
{
    /// <summary>
    /// Collects and displays real-time data from the simulation,
    /// such as voltage, current, RPM, temperature, etc.
    /// </summary>
    public class Telemetry : MonoBehaviour
    {
        void OnGUI()
        {
            // TODO: Display telemetry data on screen for debugging/gameplay.
            // Example: GUILayout.Label($"Battery Voltage: {0.0f} V");
        }

        // TODO: Add public methods for other systems (like ElectroSim) to push data to.
    }
}
