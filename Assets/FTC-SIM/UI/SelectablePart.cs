using UnityEngine;
using FTCSIM.Assembly;

namespace FTCSIM.UI
{
    /// <summary>
    /// Component for parts in the 3D view that makes them selectable.
    /// </summary>
    public class SelectablePart : MonoBehaviour
    {
        public AssemblyNode assemblyNode;
        public RobotBuilderUI builderUI;
        
        private Renderer partRenderer;
        private Color originalColor;
        private bool isSelected = false;
        
        private void Start()
        {
            partRenderer = GetComponent<Renderer>();
            if (partRenderer != null)
            {
                originalColor = partRenderer.material.color;
            }
        }
        
        private void OnMouseDown()
        {
            if (builderUI != null && assemblyNode != null)
            {
                builderUI.OnNodeSelected(assemblyNode);
                SetSelected(true);
            }
        }
        
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            if (partRenderer != null)
            {
                if (selected)
                {
                    partRenderer.material.color = Color.yellow;
                }
                else
                {
                    partRenderer.material.color = originalColor;
                }
            }
        }
    }
}
