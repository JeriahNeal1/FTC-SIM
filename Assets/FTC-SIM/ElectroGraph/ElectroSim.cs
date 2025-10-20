using UnityEngine;
using FTCSim.Core.Data;

namespace FTCSim.ElectroGraph
{
    /// <summary>
    /// Runtime simulator for the electronics graph.
    /// Evaluates power nets, simulates current/voltage, drives actuators, and publishes telemetry.
    /// </summary>
    public class ElectroSim : MonoBehaviour
    {
        public ElectroGraphData electroGraph;

        void FixedUpdate()
        {
            // TODO:
            // 1. Evaluate power rails (voltage sag, etc.)
            // 2. Update actuators (motors, servos) based on input signals and power.
            // 3. Calculate feedback (back-EMF, current draw).
            // 4. Publish data to Telemetry service.
        }
    }
}
