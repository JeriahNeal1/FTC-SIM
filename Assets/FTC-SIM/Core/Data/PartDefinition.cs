using UnityEngine;
using System.Collections.Generic;

namespace FTCSim.Core.Data
{
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
        // TODO: Define and add Mounts, Colliders, Rules, etc.
    }
}
