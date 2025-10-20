using UnityEngine;
using FTCSIM.Core;
using FTCSIM.Parts;
using FTCSIM.Assembly;
using FTCSIM.Snapping;
using System.Collections.Generic;

namespace FTCSIM.UI
{
    /// <summary>
    /// Handles drag-and-drop placement of parts with 3D snapping in the game view.
    /// </summary>
    public class DragDropBuilder : MonoBehaviour
    {
        [Header("References")]
        public RobotController robotController;
        public RobotBuilderUI builderUI;
        public Camera buildCamera;
        
        [Header("Drag Settings")]
        public float dragDistance = 5f;
        public LayerMask snapLayerMask = -1;
        
        [Header("Visual Feedback")]
        public Material validSnapMaterial;
        public Material invalidSnapMaterial;
        public GameObject snapIndicatorPrefab;
        
        private PartDefinition currentPartDef;
        private GameObject ghostObject;
        private bool isDragging = false;
        private SnapCandidate bestSnapCandidate;
        private GameObject snapIndicator;
        
        private void Start()
        {
            if (buildCamera == null)
            {
                buildCamera = Camera.main;
            }
            
            if (robotController == null)
            {
                robotController = FindObjectOfType<RobotController>();
            }
        }
        
        private void Update()
        {
            if (isDragging)
            {
                UpdateDragPosition();
                UpdateSnapping();
                
                if (Input.GetMouseButtonDown(0))
                {
                    TryPlacePart();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CancelDrag();
                }
            }
        }
        
        public void BeginDrag(PartDefinition part)
        {
            if (part == null) return;
            
            currentPartDef = part;
            isDragging = true;
            
            CreateGhostObject();
        }
        
        private void CreateGhostObject()
        {
            if (ghostObject != null)
            {
                Destroy(ghostObject);
            }
            
            // Create a simple representation of the part
            ghostObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ghostObject.name = "Ghost_" + currentPartDef.displayName;
            
            // Make it semi-transparent
            var renderer = ghostObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = invalidSnapMaterial != null ? invalidSnapMaterial : new Material(Shader.Find("Standard"));
                Color col = renderer.material.color;
                col.a = 0.5f;
                renderer.material.color = col;
            }
            
            // Remove collider so it doesn't interfere
            var collider = ghostObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            
            // Scale based on part mass (rough approximation)
            float scale = Mathf.Pow(currentPartDef.mass, 0.33f) * 0.5f;
            ghostObject.transform.localScale = Vector3.one * Mathf.Max(scale, 0.1f);
        }
        
        private void UpdateDragPosition()
        {
            if (ghostObject == null || buildCamera == null) return;
            
            Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPos = ray.origin + ray.direction * dragDistance;
            
            ghostObject.transform.position = targetPos;
        }
        
        private void UpdateSnapping()
        {
            if (ghostObject == null || robotController == null) return;
            
            // Find snap candidates
            var snappingService = robotController.snappingService;
            if (snappingService == null || currentPartDef.mounts.Count == 0) return;
            
            // Use the first mount for now
            var partMount = currentPartDef.mounts[0];
            
            List<SnapCandidate> candidates = snappingService.FindSnapCandidates(
                ghostObject, 
                partMount
            );
            
            if (candidates.Count > 0)
            {
                bestSnapCandidate = candidates[0];
                
                // Snap ghost to target
                ghostObject.transform.position = bestSnapCandidate.snapPosition;
                ghostObject.transform.rotation = bestSnapCandidate.snapRotation;
                
                // Update visual feedback
                var renderer = ghostObject.GetComponent<Renderer>();
                if (renderer != null && validSnapMaterial != null)
                {
                    renderer.material = validSnapMaterial;
                    Color col = renderer.material.color;
                    col.a = 0.5f;
                    renderer.material.color = col;
                }
                
                // Show snap indicator
                ShowSnapIndicator(bestSnapCandidate);
            }
            else
            {
                bestSnapCandidate = null;
                
                // Update visual feedback
                var renderer = ghostObject.GetComponent<Renderer>();
                if (renderer != null && invalidSnapMaterial != null)
                {
                    renderer.material = invalidSnapMaterial;
                    Color col = renderer.material.color;
                    col.a = 0.5f;
                    renderer.material.color = col;
                }
                
                HideSnapIndicator();
            }
        }
        
        private void ShowSnapIndicator(SnapCandidate candidate)
        {
            if (snapIndicatorPrefab == null) return;
            
            if (snapIndicator == null)
            {
                snapIndicator = Instantiate(snapIndicatorPrefab);
            }
            
            snapIndicator.transform.position = candidate.snapPosition;
            snapIndicator.transform.rotation = candidate.snapRotation;
            snapIndicator.SetActive(true);
        }
        
        private void HideSnapIndicator()
        {
            if (snapIndicator != null)
            {
                snapIndicator.SetActive(false);
            }
        }
        
        private void TryPlacePart()
        {
            if (currentPartDef == null || robotController == null) return;
            
            // Create new assembly node
            string nodeId = "node_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
            AssemblyNode newNode = new AssemblyNode(nodeId, currentPartDef.displayName, false);
            newNode.partSKU = currentPartDef.sku;
            newNode.mass = currentPartDef.mass;
            
            if (bestSnapCandidate != null)
            {
                // Snap to existing part
                newNode.localPosition = ghostObject.transform.localPosition;
                newNode.localRotation = ghostObject.transform.localRotation;
                
                robotController.assemblyGraph.AddNode(bestSnapCandidate.targetNode, newNode);
                
                // Create relation
                var relation = new AssemblyRelation(
                    bestSnapCandidate.targetNode.id,
                    newNode.id,
                    bestSnapCandidate.targetAttachment.id,
                    currentPartDef.mounts[0].id,
                    AssemblyRelation.RelationType.Fixed
                );
                bestSnapCandidate.targetNode.relations.Add(relation);
            }
            else
            {
                // Place as new root child
                newNode.localPosition = ghostObject.transform.position;
                newNode.localRotation = ghostObject.transform.rotation;
                
                robotController.assemblyGraph.AddNode(robotController.assemblyGraph.rootNode, newNode);
            }
            
            // Create visual representation
            CreatePartGameObject(newNode);
            
            // Notify UI
            if (builderUI != null)
            {
                builderUI.OnNodeSelected(newNode);
            }
            
            // Continue dragging same part or end?
            // For now, end the drag
            EndDrag();
        }
        
        private void CreatePartGameObject(AssemblyNode node)
        {
            if (node.gameObject != null) return;
            
            // Create GameObject for the part
            GameObject partObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            partObj.name = node.displayName;
            partObj.transform.position = node.localPosition;
            partObj.transform.rotation = node.localRotation;
            
            // Scale based on mass
            float scale = Mathf.Pow(node.mass, 0.33f) * 0.5f;
            partObj.transform.localScale = Vector3.one * Mathf.Max(scale, 0.1f);
            
            // Store reference
            node.gameObject = partObj;
            
            // Add selection component
            var selectable = partObj.AddComponent<SelectablePart>();
            selectable.assemblyNode = node;
            selectable.builderUI = builderUI;
        }
        
        private void CancelDrag()
        {
            EndDrag();
        }
        
        private void EndDrag()
        {
            isDragging = false;
            currentPartDef = null;
            bestSnapCandidate = null;
            
            if (ghostObject != null)
            {
                Destroy(ghostObject);
                ghostObject = null;
            }
            
            HideSnapIndicator();
        }
    }
}
