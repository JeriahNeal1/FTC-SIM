using UnityEngine;
using System;
using System.Collections.Generic;
using FTCSIM.Core;

namespace FTCSIM.Parts
{
    /// <summary>
    /// Defines a modular robot part with its physical properties, attachment points, and constraints.
    /// Data-driven definition loaded from JSON.
    /// </summary>
    [Serializable]
    public class PartDefinition
    {
        public string sku;
        public string vendor;
        public string displayName;
        public string category; // Structural, Motion, Wheels, Actuators, etc.
        public float mass; // kg
        public string prefabPath; // Path to prefab in Resources
        
        public List<AttachmentPoint> mounts = new List<AttachmentPoint>();
        public List<RotationalMount> rotationalMounts = new List<RotationalMount>();
        public List<ColliderDefinition> colliders = new List<ColliderDefinition>();
        public PartRules rules = new PartRules();
        public ElectronicsProfile electronicsProfile;
        
        [Serializable]
        public class ColliderDefinition
        {
            public string source; // "convexHull", "box", "cylinder", "mesh"
            public float margin = 0.002f;
        }
        
        [Serializable]
        public class PartRules
        {
            public List<string> validMates = new List<string>();
            public float maxLoad = float.MaxValue; // Newtons
            public float maxTorque = float.MaxValue; // N⋅m
        }
        
        [Serializable]
        public class ElectronicsProfile
        {
            public string deviceType; // "Motor", "Servo", "Hub", "Battery", "Sensor"
            public List<PinDefinition> pins = new List<PinDefinition>();
            public float voltage = 12f; // Volts
            public float currentDraw = 0f; // Amps
        }
        
        [Serializable]
        public class PinDefinition
        {
            public string id;
            public string role; // PWR, GND, SIGNAL, BUS
            public float voltage;
            public float maxCurrent;
        }
    }
}
