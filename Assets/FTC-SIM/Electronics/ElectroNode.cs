using UnityEngine;
using System;
using System.Collections.Generic;

namespace FTCSIM.Electronics
{
    /// <summary>
    /// Represents a device node in the electronics graph (motor, sensor, hub, etc.)
    /// </summary>
    [Serializable]
    public class ElectroNode
    {
        public string id;
        public string type; // Battery, Hub, DCMotor, Servo, Sensor, etc.
        public List<ElectroPin> pins = new List<ElectroPin>();
        public Vector2 graphPosition; // Position in node editor UI
        
        // Reference to physical part
        [NonSerialized]
        public string assemblyNodeId;
        
        public ElectroNode(string id, string type)
        {
            this.id = id;
            this.type = type;
        }
        
        public ElectroPin GetPin(string pinId)
        {
            return pins.Find(p => p.id == pinId);
        }
    }
    
    /// <summary>
    /// Represents a pin/terminal on an electronic device
    /// </summary>
    [Serializable]
    public class ElectroPin
    {
        public string id;
        public PinRole role;
        public float voltage = 0f; // Volts
        public float current = 0f; // Amps
        public float maxCurrent = 10f; // Amps
        
        public enum PinRole
        {
            PWR,      // Power supply
            GND,      // Ground
            SIGNAL,   // Digital/Analog signal
            BUS       // Communication bus (I2C, SPI, etc.)
        }
        
        public ElectroPin(string id, PinRole role)
        {
            this.id = id;
            this.role = role;
        }
    }
    
    /// <summary>
    /// Represents a wire connection between two pins
    /// </summary>
    [Serializable]
    public class ElectroWire
    {
        public string id;
        public WireType type;
        public WireEndpoint a;
        public WireEndpoint b;
        public int awg = 18; // Wire gauge
        public float resistance; // Ohms
        
        public enum WireType
        {
            Power,
            Ground,
            Signal,
            Bus
        }
        
        [Serializable]
        public class WireEndpoint
        {
            public string nodeId;
            public string pinId;
            
            public WireEndpoint(string nodeId, string pinId)
            {
                this.nodeId = nodeId;
                this.pinId = pinId;
            }
        }
        
        public ElectroWire(string id, WireType type, string nodeIdA, string pinIdA, string nodeIdB, string pinIdB)
        {
            this.id = id;
            this.type = type;
            this.a = new WireEndpoint(nodeIdA, pinIdA);
            this.b = new WireEndpoint(nodeIdB, pinIdB);
            CalculateResistance();
        }
        
        private void CalculateResistance()
        {
            // Simplified wire resistance calculation based on AWG
            // Resistance per meter for copper wire at 20°C
            float resistancePerMeter = awg switch
            {
                14 => 0.00826f,
                16 => 0.01314f,
                18 => 0.02089f,
                20 => 0.03323f,
                _ => 0.02089f
            };
            
            // Assume 1 meter wire length
            resistance = resistancePerMeter;
        }
    }
}
