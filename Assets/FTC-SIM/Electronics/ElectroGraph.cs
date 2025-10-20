using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FTCSIM.Electronics
{
    /// <summary>
    /// Manages the electronics graph and validates connections.
    /// Simulates power distribution, voltage sag, and current draw.
    /// </summary>
    public class ElectroGraph : MonoBehaviour
    {
        public List<ElectroNode> nodes = new List<ElectroNode>();
        public List<ElectroWire> wires = new List<ElectroWire>();
        
        private Dictionary<string, ElectroNode> nodeRegistry = new Dictionary<string, ElectroNode>();
        private Dictionary<string, ElectroWire> wireRegistry = new Dictionary<string, ElectroWire>();
        
        // Simulation state
        public float simulationTimeStep = 0.001f; // 1ms
        private float simulationTime = 0f;
        
        public void Initialize()
        {
            RebuildRegistries();
        }
        
        public void RebuildRegistries()
        {
            nodeRegistry.Clear();
            wireRegistry.Clear();
            
            foreach (var node in nodes)
            {
                nodeRegistry[node.id] = node;
            }
            
            foreach (var wire in wires)
            {
                wireRegistry[wire.id] = wire;
            }
        }
        
        public bool AddNode(ElectroNode node)
        {
            if (nodeRegistry.ContainsKey(node.id))
            {
                Debug.LogWarning($"Node with ID {node.id} already exists");
                return false;
            }
            
            nodes.Add(node);
            nodeRegistry[node.id] = node;
            return true;
        }
        
        public bool AddWire(ElectroWire wire)
        {
            // Validate connection
            if (!ValidateWire(wire))
            {
                return false;
            }
            
            wires.Add(wire);
            wireRegistry[wire.id] = wire;
            return true;
        }
        
        public bool RemoveNode(string nodeId)
        {
            var node = GetNode(nodeId);
            if (node == null) return false;
            
            // Remove all wires connected to this node
            var connectedWires = wires.Where(w => 
                w.a.nodeId == nodeId || w.b.nodeId == nodeId).ToList();
            
            foreach (var wire in connectedWires)
            {
                RemoveWire(wire.id);
            }
            
            nodes.Remove(node);
            nodeRegistry.Remove(nodeId);
            return true;
        }
        
        public bool RemoveWire(string wireId)
        {
            var wire = GetWire(wireId);
            if (wire == null) return false;
            
            wires.Remove(wire);
            wireRegistry.Remove(wireId);
            return true;
        }
        
        public ElectroNode GetNode(string nodeId)
        {
            return nodeRegistry.ContainsKey(nodeId) ? nodeRegistry[nodeId] : null;
        }
        
        public ElectroWire GetWire(string wireId)
        {
            return wireRegistry.ContainsKey(wireId) ? wireRegistry[wireId] : null;
        }
        
        /// <summary>
        /// Validate that a wire connection is valid (compatible pin types, no shorts)
        /// </summary>
        private bool ValidateWire(ElectroWire wire)
        {
            var nodeA = GetNode(wire.a.nodeId);
            var nodeB = GetNode(wire.b.nodeId);
            
            if (nodeA == null || nodeB == null)
            {
                Debug.LogWarning("Wire references non-existent node");
                return false;
            }
            
            var pinA = nodeA.GetPin(wire.a.pinId);
            var pinB = nodeB.GetPin(wire.b.pinId);
            
            if (pinA == null || pinB == null)
            {
                Debug.LogWarning("Wire references non-existent pin");
                return false;
            }
            
            // Check for pin role compatibility
            if (!ArePinsCompatible(pinA, pinB))
            {
                Debug.LogWarning($"Incompatible pin roles: {pinA.role} and {pinB.role}");
                return false;
            }
            
            // Check for short circuits (PWR to GND)
            if ((pinA.role == ElectroPin.PinRole.PWR && pinB.role == ElectroPin.PinRole.GND) ||
                (pinA.role == ElectroPin.PinRole.GND && pinB.role == ElectroPin.PinRole.PWR))
            {
                Debug.LogError("SHORT CIRCUIT DETECTED: Power connected directly to Ground!");
                return false;
            }
            
            return true;
        }
        
        private bool ArePinsCompatible(ElectroPin pinA, ElectroPin pinB)
        {
            // PWR connects to PWR, GND to GND, SIGNAL to SIGNAL, BUS to BUS
            return pinA.role == pinB.role;
        }
        
        /// <summary>
        /// Simulate one time step of the electronics system
        /// </summary>
        public void SimulateStep(float deltaTime)
        {
            simulationTime += deltaTime;
            
            // Reset currents
            foreach (var node in nodes)
            {
                foreach (var pin in node.pins)
                {
                    pin.current = 0f;
                }
            }
            
            // Simulate power distribution
            SimulatePowerDistribution();
            
            // Check for overcurrent conditions
            CheckOvercurrent();
        }
        
        private void SimulatePowerDistribution()
        {
            // Find battery nodes
            var batteries = nodes.Where(n => n.type == "Battery").ToList();
            
            foreach (var battery in batteries)
            {
                var pwrPin = battery.pins.Find(p => p.role == ElectroPin.PinRole.PWR);
                if (pwrPin != null)
                {
                    pwrPin.voltage = 12.0f; // Standard FTC battery voltage
                }
            }
            
            // Propagate voltage through wires
            foreach (var wire in wires)
            {
                if (wire.type == ElectroWire.WireType.Power)
                {
                    var nodeA = GetNode(wire.a.nodeId);
                    var nodeB = GetNode(wire.b.nodeId);
                    var pinA = nodeA?.GetPin(wire.a.pinId);
                    var pinB = nodeB?.GetPin(wire.b.pinId);
                    
                    if (pinA != null && pinB != null)
                    {
                        // Simplified voltage propagation (account for wire resistance later)
                        float avgVoltage = (pinA.voltage + pinB.voltage) / 2f;
                        pinA.voltage = avgVoltage;
                        pinB.voltage = avgVoltage;
                    }
                }
            }
        }
        
        private void CheckOvercurrent()
        {
            foreach (var node in nodes)
            {
                foreach (var pin in node.pins)
                {
                    if (Mathf.Abs(pin.current) > pin.maxCurrent)
                    {
                        Debug.LogWarning($"Overcurrent on {node.id}.{pin.id}: {pin.current}A > {pin.maxCurrent}A");
                    }
                }
            }
        }
        
        /// <summary>
        /// Export the electronics graph to JSON
        /// </summary>
        public string ExportToJson()
        {
            var data = new ElectroGraphData
            {
                nodes = nodes,
                wires = wires
            };
            return JsonUtility.ToJson(data, true);
        }
        
        /// <summary>
        /// Import electronics graph from JSON
        /// </summary>
        public void ImportFromJson(string json)
        {
            var data = JsonUtility.FromJson<ElectroGraphData>(json);
            nodes = data.nodes;
            wires = data.wires;
            RebuildRegistries();
        }
        
        [Serializable]
        private class ElectroGraphData
        {
            public List<ElectroNode> nodes;
            public List<ElectroWire> wires;
        }
    }
}
