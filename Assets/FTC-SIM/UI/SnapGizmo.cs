using UnityEngine;
using FTCSIM.Core;

namespace FTCSIM.UI
{
    /// <summary>
    /// 3D gizmo visualization for snap points and attachment indicators.
    /// </summary>
    public class SnapGizmo : MonoBehaviour
    {
        [Header("Gizmo Settings")]
        public float gizmoSize = 0.05f;
        public Color validSnapColor = Color.green;
        public Color invalidSnapColor = Color.red;
        public Color highlightColor = Color.yellow;
        
        [Header("Visual Elements")]
        public GameObject arrowPrefab;
        public GameObject spherePrefab;
        
        private AttachmentPoint attachmentPoint;
        private bool isValid = false;
        private bool isHighlighted = false;
        
        private GameObject visualIndicator;
        
        public void Initialize(AttachmentPoint mount, bool valid = false)
        {
            attachmentPoint = mount;
            isValid = valid;
            
            CreateVisualIndicator();
            UpdateVisualization();
        }
        
        private void CreateVisualIndicator()
        {
            // Create a simple sphere indicator
            if (spherePrefab != null)
            {
                visualIndicator = Instantiate(spherePrefab, transform);
            }
            else
            {
                visualIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                visualIndicator.transform.SetParent(transform);
            }
            
            visualIndicator.transform.localPosition = Vector3.zero;
            visualIndicator.transform.localScale = Vector3.one * gizmoSize;
            
            // Remove collider
            var collider = visualIndicator.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }
        
        public void SetValid(bool valid)
        {
            isValid = valid;
            UpdateVisualization();
        }
        
        public void SetHighlighted(bool highlighted)
        {
            isHighlighted = highlighted;
            UpdateVisualization();
        }
        
        private void UpdateVisualization()
        {
            if (visualIndicator == null) return;
            
            var renderer = visualIndicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color targetColor = isHighlighted ? highlightColor : (isValid ? validSnapColor : invalidSnapColor);
                renderer.material.color = targetColor;
            }
        }
        
        private void OnDrawGizmos()
        {
            if (attachmentPoint == null) return;
            
            Gizmos.color = isValid ? validSnapColor : invalidSnapColor;
            
            // Draw sphere at attachment point
            Gizmos.DrawWireSphere(transform.position, gizmoSize);
            
            // Draw coordinate axes
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * gizmoSize * 2);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * gizmoSize * 2);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * gizmoSize * 2);
        }
    }
}
