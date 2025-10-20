using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FTCSIM.Assembly
{
    /// <summary>
    /// Manages the hierarchical assembly graph (DAG) of robot components.
    /// Provides methods to add, remove, and validate assemblies.
    /// </summary>
    public class AssemblyGraph : MonoBehaviour
    {
        public AssemblyNode rootNode;
        private Dictionary<string, AssemblyNode> nodeRegistry = new Dictionary<string, AssemblyNode>();
        
        public void Initialize()
        {
            if (rootNode == null)
            {
                rootNode = new AssemblyNode("root", "Robot Assembly", true);
            }
            RebuildRegistry();
        }
        
        public void RebuildRegistry()
        {
            nodeRegistry.Clear();
            if (rootNode != null)
            {
                RegisterNodeRecursive(rootNode);
            }
        }
        
        private void RegisterNodeRecursive(AssemblyNode node)
        {
            if (!nodeRegistry.ContainsKey(node.id))
            {
                nodeRegistry[node.id] = node;
            }
            
            foreach (var child in node.children)
            {
                RegisterNodeRecursive(child);
            }
        }
        
        public AssemblyNode GetNode(string nodeId)
        {
            return nodeRegistry.ContainsKey(nodeId) ? nodeRegistry[nodeId] : null;
        }
        
        public bool AddNode(AssemblyNode parent, AssemblyNode child)
        {
            if (parent == null || child == null) return false;
            
            // Check for circular dependencies
            if (WouldCreateCycle(parent, child)) return false;
            
            parent.children.Add(child);
            nodeRegistry[child.id] = child;
            return true;
        }
        
        public bool RemoveNode(string nodeId)
        {
            var node = GetNode(nodeId);
            if (node == null || node == rootNode) return false;
            
            // Find parent and remove
            var parent = FindParent(rootNode, nodeId);
            if (parent != null)
            {
                parent.children.Remove(node);
                UnregisterNodeRecursive(node);
                return true;
            }
            return false;
        }
        
        private AssemblyNode FindParent(AssemblyNode current, string childId)
        {
            if (current.children.Any(c => c.id == childId))
            {
                return current;
            }
            
            foreach (var child in current.children)
            {
                var parent = FindParent(child, childId);
                if (parent != null) return parent;
            }
            return null;
        }
        
        private bool WouldCreateCycle(AssemblyNode parent, AssemblyNode child)
        {
            // Check if parent is a descendant of child (which would create a cycle)
            return IsDescendant(child, parent.id);
        }
        
        private bool IsDescendant(AssemblyNode node, string ancestorId)
        {
            if (node.id == ancestorId) return true;
            
            foreach (var child in node.children)
            {
                if (IsDescendant(child, ancestorId)) return true;
            }
            return false;
        }
        
        private void UnregisterNodeRecursive(AssemblyNode node)
        {
            nodeRegistry.Remove(node.id);
            foreach (var child in node.children)
            {
                UnregisterNodeRecursive(child);
            }
        }
        
        /// <summary>
        /// Aggregate mass and inertia for the entire assembly.
        /// </summary>
        public void UpdatePhysicsProperties()
        {
            UpdatePhysicsPropertiesRecursive(rootNode);
        }
        
        private void UpdatePhysicsPropertiesRecursive(AssemblyNode node)
        {
            if (!node.isSubassembly)
            {
                // Leaf node - mass is from part definition
                return;
            }
            
            // Aggregate from children
            float totalMass = 0f;
            Vector3 weightedCOM = Vector3.zero;
            
            foreach (var child in node.children)
            {
                UpdatePhysicsPropertiesRecursive(child);
                totalMass += child.mass;
                weightedCOM += child.centerOfMass * child.mass;
            }
            
            node.mass = totalMass;
            node.centerOfMass = totalMass > 0 ? weightedCOM / totalMass : Vector3.zero;
        }
        
        /// <summary>
        /// Get all nodes in the assembly graph as a flat list.
        /// </summary>
        public List<AssemblyNode> GetAllNodes()
        {
            List<AssemblyNode> allNodes = new List<AssemblyNode>();
            if (rootNode != null)
            {
                CollectNodesRecursive(rootNode, allNodes);
            }
            return allNodes;
        }
        
        private void CollectNodesRecursive(AssemblyNode node, List<AssemblyNode> collection)
        {
            collection.Add(node);
            foreach (var child in node.children)
            {
                CollectNodesRecursive(child, collection);
            }
        }
    }
}
