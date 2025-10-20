using UnityEngine;
using UnityEngine.UI;
using FTCSIM.Electronics;

namespace FTCSIM.UI
{
    /// <summary>
    /// Visual representation of a wire connection between two pins.
    /// </summary>
    public class ElectroWireUI : MonoBehaviour
    {
        public Image lineImage;
        
        private ElectroWire electroWire;
        private GameObject nodeAUI;
        private GameObject nodeBUI;
        private RectTransform rectTransform;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        public void Initialize(ElectroWire wire, GameObject nodeA, GameObject nodeB)
        {
            electroWire = wire;
            nodeAUI = nodeA;
            nodeBUI = nodeB;
            
            if (lineImage != null)
            {
                lineImage.color = GetWireColor(wire.type);
            }
            
            UpdatePosition();
        }
        
        private Color GetWireColor(ElectroWire.WireType type)
        {
            switch (type)
            {
                case ElectroWire.WireType.Power:
                    return Color.red;
                case ElectroWire.WireType.Ground:
                    return Color.black;
                case ElectroWire.WireType.Signal:
                    return Color.blue;
                case ElectroWire.WireType.Bus:
                    return Color.cyan;
                default:
                    return Color.gray;
            }
        }
        
        public void UpdatePosition()
        {
            if (nodeAUI == null || nodeBUI == null || rectTransform == null) return;
            
            RectTransform rectA = nodeAUI.GetComponent<RectTransform>();
            RectTransform rectB = nodeBUI.GetComponent<RectTransform>();
            
            if (rectA == null || rectB == null) return;
            
            Vector2 posA = rectA.anchoredPosition;
            Vector2 posB = rectB.anchoredPosition;
            
            // Calculate line position and rotation
            Vector2 midPoint = (posA + posB) / 2f;
            Vector2 direction = posB - posA;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            rectTransform.anchoredPosition = midPoint;
            rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        private void Update()
        {
            UpdatePosition();
        }
    }
}
