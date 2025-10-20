using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FTCSIM.Electronics;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// UI representation of a pin on an ElectroNode.
    /// </summary>
    public class ElectroPinUI : MonoBehaviour, IPointerClickHandler
    {
        public TMP_Text pinNameText;
        public Image pinColorImage;
        
        private ElectroPin electroPin;
        private ElectroNode parentNode;
        private ElectroGraphEditor graphEditor;
        
        public void Initialize(ElectroPin pin, ElectroNode node, ElectroGraphEditor editor)
        {
            electroPin = pin;
            parentNode = node;
            graphEditor = editor;
            
            if (pinNameText != null)
            {
                pinNameText.text = pin.id;
            }
            
            if (pinColorImage != null)
            {
                pinColorImage.color = GetPinColor(pin.role);
            }
        }
        
        private Color GetPinColor(ElectroPin.PinRole role)
        {
            switch (role)
            {
                case ElectroPin.PinRole.PWR:
                    return Color.red;
                case ElectroPin.PinRole.GND:
                    return Color.black;
                case ElectroPin.PinRole.SIGNAL:
                    return Color.blue;
                case ElectroPin.PinRole.BUS:
                    return Color.cyan;
                default:
                    return Color.gray;
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (graphEditor != null && parentNode != null && electroPin != null)
            {
                graphEditor.OnPinClicked(parentNode, electroPin);
            }
        }
    }
}
