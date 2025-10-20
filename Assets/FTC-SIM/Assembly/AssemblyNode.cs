using UnityEngine;
using System;
using System.Collections.Generic;
using FTCSIM.Core;

namespace FTCSIM.Assembly
{
    /// <summary>
    /// Represents a single node in the assembly hierarchy.
    /// Can be a part or a subassembly containing other nodes.
    /// </summary>
    [Serializable]
    public class AssemblyNode
    {
        public string id;
        public string displayName;
        public string partSKU; // null if subassembly
        public bool isSubassembly;
        
        public Vector3 localPosition;
        public Quaternion localRotation;
        
        public List<AssemblyNode> children = new List<AssemblyNode>();
        public List<AssemblyRelation> relations = new List<AssemblyRelation>();
        
        // Physics properties (aggregated for subassemblies)
        public float mass;
        public Vector3 centerOfMass;
        
        // Reference to actual GameObject in scene
        [NonSerialized]
        public GameObject gameObject;
        
        public AssemblyNode(string id, string displayName, bool isSubassembly = false)
        {
            this.id = id;
            this.displayName = displayName;
            this.isSubassembly = isSubassembly;
            this.localPosition = Vector3.zero;
            this.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Defines how two assembly nodes are connected.
    /// </summary>
    [Serializable]
    public class AssemblyRelation
    {
        public string parentNodeId;
        public string childNodeId;
        public string parentAttachmentId;
        public string childAttachmentId;
        public RelationType type;
        
        public enum RelationType
        {
            Fixed,      // Bolted connection
            Revolute,   // Rotational joint
            Prismatic,  // Sliding joint
            Shaft       // Shaft connection
        }
        
        public AssemblyRelation(string parentNodeId, string childNodeId, string parentAttachmentId, string childAttachmentId, RelationType type = RelationType.Fixed)
        {
            this.parentNodeId = parentNodeId;
            this.childNodeId = childNodeId;
            this.parentAttachmentId = parentAttachmentId;
            this.childAttachmentId = childAttachmentId;
            this.type = type;
        }
    }
}
