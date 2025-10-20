using UnityEngine;
using System.Collections.Generic;

namespace FTCSim.Core.Data
{
    [System.Serializable]
    public class ElectroGraphNode
    {
        public string id;
        public string type;
        // TODO: Add Pin definitions
    }

    [System.Serializable]
    public class ElectroGraphWire
    {
        public string type;
        public string[] a; // [nodeId, pinId]
        public string[] b; // [nodeId, pinId]
        public int awg;
    }
    
    /// <summary>
    /// Represents the serializable data for the entire electronics graph of a robot.
    /// </summary>
    [System.Serializable]
    public class ElectroGraphData
    {
        public List<ElectroGraphNode> nodes = new List<ElectroGraphNode>();
        public List<ElectroGraphWire> wires = new List<ElectroGraphWire>();
    }
}
