using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FTCSIM.Core;
using FTCSIM.Assembly;
using FTCSIM.Parts;

namespace FTCSIM.Snapping
{
    /// <summary>
    /// Handles snapping logic for goBILDA and other modular patterns.
    /// Detects compatible attachment points and creates assembly relations.
    /// </summary>
    public class SnappingService : MonoBehaviour
    {
        public float snapDistance = 0.05f; // 50mm
        public float snapAngleTolerance = 15f; // degrees
        
        private AssemblyGraph assemblyGraph;
        private PartsLibrary partsLibrary;
        
        public void Initialize(AssemblyGraph graph, PartsLibrary library)
        {
            assemblyGraph = graph;
            partsLibrary = library;
        }
        
        /// <summary>
        /// Find potential snap targets for a part being placed
        /// </summary>
        public List<SnapCandidate> FindSnapCandidates(GameObject partObject, AttachmentPoint partAttachment)
        {
            List<SnapCandidate> candidates = new List<SnapCandidate>();
            
            if (assemblyGraph == null || assemblyGraph.rootNode == null)
            {
                return candidates;
            }
            
            // Get world position of the attachment point on the part being placed
            Vector3 partAttachWorldPos = partObject.transform.TransformPoint(partAttachment.localPosition);
            Quaternion partAttachWorldRot = partObject.transform.rotation * partAttachment.localRotation;
            
            // Search for compatible attachment points in the assembly
            SearchForSnapTargetsRecursive(assemblyGraph.rootNode, partAttachWorldPos, partAttachWorldRot, 
                partAttachment, candidates);
            
            // Sort by distance
            candidates = candidates.OrderBy(c => c.distance).ToList();
            
            return candidates;
        }
        
        private void SearchForSnapTargetsRecursive(AssemblyNode node, Vector3 partAttachWorldPos, 
            Quaternion partAttachWorldRot, AttachmentPoint partAttachment, List<SnapCandidate> candidates)
        {
            if (node.gameObject == null)
            {
                return;
            }
            
            // Get part definition to check attachment points
            if (!string.IsNullOrEmpty(node.partSKU))
            {
                var partDef = partsLibrary.GetPart(node.partSKU);
                if (partDef != null)
                {
                    foreach (var targetAttachment in partDef.mounts)
                    {
                        // Check pattern compatibility
                        if (!ArePatternsCompatible(partAttachment.pattern, targetAttachment.pattern))
                        {
                            continue;
                        }
                        
                        // Calculate world position of target attachment
                        Vector3 targetWorldPos = node.gameObject.transform.TransformPoint(targetAttachment.localPosition);
                        Quaternion targetWorldRot = node.gameObject.transform.rotation * targetAttachment.localRotation;
                        
                        // Check distance
                        float distance = Vector3.Distance(partAttachWorldPos, targetWorldPos);
                        if (distance > snapDistance)
                        {
                            continue;
                        }
                        
                        // Check alignment
                        float angleDiff = Quaternion.Angle(partAttachWorldRot, targetWorldRot);
                        if (angleDiff > snapAngleTolerance)
                        {
                            continue;
                        }
                        
                        // Valid candidate
                        candidates.Add(new SnapCandidate
                        {
                            targetNode = node,
                            targetAttachment = targetAttachment,
                            distance = distance,
                            angleDifference = angleDiff,
                            snapPosition = targetWorldPos,
                            snapRotation = targetWorldRot
                        });
                    }
                }
            }
            
            // Search children
            foreach (var child in node.children)
            {
                SearchForSnapTargetsRecursive(child, partAttachWorldPos, partAttachWorldRot, 
                    partAttachment, candidates);
            }
        }
        
        private bool ArePatternsCompatible(string pattern1, string pattern2)
        {
            // Exact match
            if (pattern1 == pattern2)
            {
                return true;
            }
            
            // goBILDA patterns are compatible within the same system
            if (pattern1.StartsWith("Pattern:goBILDA") && pattern2.StartsWith("Pattern:goBILDA"))
            {
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Apply snap to connect two parts
        /// </summary>
        public bool ApplySnap(GameObject partObject, AttachmentPoint partAttachment, SnapCandidate candidate)
        {
            if (candidate == null || candidate.targetNode == null)
            {
                return false;
            }
            
            // Position the part at the snap location
            partObject.transform.position = candidate.snapPosition;
            partObject.transform.rotation = candidate.snapRotation;
            
            // Create assembly node for the new part
            // (This would typically be done by the caller, but we outline the logic here)
            
            return true;
        }
        
        /// <summary>
        /// Visualize snap preview
        /// </summary>
        public void DrawSnapPreview(SnapCandidate candidate)
        {
            if (candidate == null) return;
            
            // Draw gizmos for snap point
            Debug.DrawLine(candidate.snapPosition, candidate.snapPosition + Vector3.up * 0.1f, Color.green);
            Debug.DrawLine(candidate.snapPosition, candidate.snapPosition + Vector3.right * 0.1f, Color.red);
            Debug.DrawLine(candidate.snapPosition, candidate.snapPosition + Vector3.forward * 0.1f, Color.blue);
        }
    }
    
    /// <summary>
    /// Represents a potential snap target
    /// </summary>
    public class SnapCandidate
    {
        public AssemblyNode targetNode;
        public AttachmentPoint targetAttachment;
        public float distance;
        public float angleDifference;
        public Vector3 snapPosition;
        public Quaternion snapRotation;
    }
}
