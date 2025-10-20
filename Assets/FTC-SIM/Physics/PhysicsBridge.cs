using UnityEngine;
using System.Collections.Generic;
using FTCSIM.Assembly;

namespace FTCSIM.Physics
{
    /// <summary>
    /// Bridges the assembly graph to Unity's physics system using ArticulationBodies.
    /// Creates and synchronizes physics representations of assemblies.
    /// </summary>
    public class PhysicsBridge : MonoBehaviour
    {
        private AssemblyGraph assemblyGraph;
        private Dictionary<string, ArticulationBody> articulationBodies = new Dictionary<string, ArticulationBody>();
        
        public void Initialize(AssemblyGraph graph)
        {
            assemblyGraph = graph;
        }
        
        /// <summary>
        /// Build ArticulationBody hierarchy from assembly graph
        /// </summary>
        public void BuildPhysicsTree()
        {
            if (assemblyGraph == null || assemblyGraph.rootNode == null)
            {
                Debug.LogError("Cannot build physics tree: AssemblyGraph not initialized");
                return;
            }
            
            ClearPhysicsTree();
            BuildPhysicsNodeRecursive(assemblyGraph.rootNode, null);
        }
        
        private void BuildPhysicsNodeRecursive(AssemblyNode node, ArticulationBody parentBody)
        {
            if (node.gameObject == null)
            {
                Debug.LogWarning($"Node {node.id} has no GameObject");
                return;
            }
            
            ArticulationBody body = node.gameObject.GetComponent<ArticulationBody>();
            if (body == null)
            {
                body = node.gameObject.AddComponent<ArticulationBody>();
            }
            
            // Configure articulation body
            body.mass = node.mass;
            body.useGravity = true;
            
            if (parentBody == null)
            {
                // Root body
                body.immovable = true;
                body.jointType = ArticulationJointType.FixedJoint;
            }
            else
            {
                // Determine joint type from relations
                var relation = FindRelationToParent(node);
                ConfigureJoint(body, relation);
            }
            
            articulationBodies[node.id] = body;
            
            // Process children
            foreach (var child in node.children)
            {
                BuildPhysicsNodeRecursive(child, body);
            }
        }
        
        private AssemblyRelation FindRelationToParent(AssemblyNode node)
        {
            // Find the relation that connects this node to its parent
            var parent = FindParentNode(assemblyGraph.rootNode, node.id);
            if (parent != null)
            {
                foreach (var relation in parent.relations)
                {
                    if (relation.childNodeId == node.id)
                    {
                        return relation;
                    }
                }
            }
            return null;
        }
        
        private AssemblyNode FindParentNode(AssemblyNode current, string childId)
        {
            foreach (var child in current.children)
            {
                if (child.id == childId)
                {
                    return current;
                }
                
                var found = FindParentNode(child, childId);
                if (found != null) return found;
            }
            return null;
        }
        
        private void ConfigureJoint(ArticulationBody body, AssemblyRelation relation)
        {
            if (relation == null)
            {
                body.jointType = ArticulationJointType.FixedJoint;
                return;
            }
            
            switch (relation.type)
            {
                case AssemblyRelation.RelationType.Fixed:
                    body.jointType = ArticulationJointType.FixedJoint;
                    break;
                    
                case AssemblyRelation.RelationType.Revolute:
                    body.jointType = ArticulationJointType.RevoluteJoint;
                    body.anchorRotation = Quaternion.identity;
                    
                    // Configure drive (motor/servo control)
                    var drive = body.xDrive;
                    drive.stiffness = 10000f;
                    drive.damping = 100f;
                    drive.forceLimit = 100f;
                    body.xDrive = drive;
                    break;
                    
                case AssemblyRelation.RelationType.Prismatic:
                    body.jointType = ArticulationJointType.PrismaticJoint;
                    
                    var pDrive = body.xDrive;
                    pDrive.stiffness = 10000f;
                    pDrive.damping = 100f;
                    pDrive.forceLimit = 100f;
                    body.xDrive = pDrive;
                    break;
                    
                default:
                    body.jointType = ArticulationJointType.FixedJoint;
                    break;
            }
        }
        
        public void ClearPhysicsTree()
        {
            foreach (var body in articulationBodies.Values)
            {
                if (body != null)
                {
                    Destroy(body);
                }
            }
            articulationBodies.Clear();
        }
        
        public ArticulationBody GetArticulationBody(string nodeId)
        {
            return articulationBodies.ContainsKey(nodeId) ? articulationBodies[nodeId] : null;
        }
        
        /// <summary>
        /// Apply motor torque to a revolute joint
        /// </summary>
        public void SetMotorTorque(string nodeId, float torque)
        {
            var body = GetArticulationBody(nodeId);
            if (body != null && body.jointType == ArticulationJointType.RevoluteJoint)
            {
                var drive = body.xDrive;
                drive.target = torque;
                body.xDrive = drive;
            }
        }
        
        /// <summary>
        /// Get current joint position/velocity
        /// </summary>
        public float GetJointPosition(string nodeId)
        {
            var body = GetArticulationBody(nodeId);
            if (body != null)
            {
                return body.jointPosition[0];
            }
            return 0f;
        }
        
        public float GetJointVelocity(string nodeId)
        {
            var body = GetArticulationBody(nodeId);
            if (body != null)
            {
                return body.jointVelocity[0];
            }
            return 0f;
        }
    }
}
