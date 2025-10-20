using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FTCSIM.Electronics;
using TMPro;
using System.Collections.Generic;

namespace FTCSIM.UI
{
    /// <summary>
    /// UI representation of an ElectroNode in the visual editor.
    /// </summary>
    public class ElectroNodeUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public TMP_Text nodeNameText;
        public TMP_Text nodeTypeText;
        public Transform pinsContainer;
        public GameObject pinUIPrefab;
        
        private ElectroNode electroNode;
        private ElectroGraphEditor graphEditor;
        private RectTransform rectTransform;
        private Canvas canvas;
        
        private List<GameObject> pinUIObjects = new List<GameObject>();
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
        }
        
        public void Initialize(ElectroNode node, ElectroGraphEditor editor)
        {
            electroNode = node;
            graphEditor = editor;
            
            if (nodeNameText != null)
            {
                nodeNameText.text = node.id;
            }
            
            if (nodeTypeText != null)
            {
                nodeTypeText.text = node.type;
            }
            
            CreatePinUIs();
        }
        
        private void CreatePinUIs()
        {
            if (pinsContainer == null || pinUIPrefab == null || electroNode == null) return;
            
            // Clear existing pins
            foreach (var pinUI in pinUIObjects)
            {
                if (pinUI != null) Destroy(pinUI);
            }
            pinUIObjects.Clear();
            
            // Create pin UIs
            foreach (var pin in electroNode.pins)
            {
                GameObject pinUI = Instantiate(pinUIPrefab, pinsContainer);
                
                var pinUIComponent = pinUI.GetComponent<ElectroPinUI>();
                if (pinUIComponent != null)
                {
                    pinUIComponent.Initialize(pin, electroNode, graphEditor);
                }
                
                pinUIObjects.Add(pinUI);
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Starting to drag node
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (rectTransform == null || canvas == null) return;
            
            // Move the node
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (graphEditor != null && electroNode != null)
            {
                graphEditor.OnNodeMoved(electroNode.id, rectTransform.anchoredPosition);
            }
        }
    }
}
