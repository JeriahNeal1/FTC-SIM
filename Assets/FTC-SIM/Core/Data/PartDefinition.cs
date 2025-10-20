using UnityEngine;
using System.Collections.Generic;

namespace FTCSim.Core.Data
{
    [System.Serializable]
    public class MountData
    {
        // Placeholder for mount point data (e.g., position, rotation, pattern type)
        public Vector3 position;
        public Quaternion rotation;
        public string pattern;
    }

    [System.Serializable]
    public class RuleData
    {
        public List<string> validMates;
    }
    
    /// <summary>
    /// Represents the data-driven definition for a single robot part, based on the PartDefinition.json schema.
    /// </summary>
    [CreateAssetMenu(fileName = "PartDefinition", menuName = "FTC-SIM/Part Definition", order = 1)]
    public class PartDefinition : ScriptableObject
    {
        public string sku;
        public string vendor;
        public string displayName;
        public string category;
        public float mass;
        public List<MountData> mounts = new List<MountData>();
        public RuleData rules;
    }
}
