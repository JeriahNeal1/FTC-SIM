using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FTCSIM.Parts;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// UI item representing a part in the catalog that can be dragged.
    /// </summary>
    public class PartCatalogItem : MonoBehaviour, IPointerDownHandler
    {
        public TMP_Text partNameText;
        public TMP_Text partCategoryText;
        public TMP_Text partMassText;
        public Image partIcon;
        
        private PartDefinition partDefinition;
        private RobotBuilderUI builderUI;
        
        public void Initialize(PartDefinition part, RobotBuilderUI ui)
        {
            partDefinition = part;
            builderUI = ui;
            
            if (partNameText != null)
            {
                partNameText.text = part.displayName;
            }
            
            if (partCategoryText != null)
            {
                partCategoryText.text = part.category;
            }
            
            if (partMassText != null)
            {
                partMassText.text = $"{part.mass:F3} kg";
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (builderUI != null && partDefinition != null)
            {
                builderUI.OnPartSelected(partDefinition);
            }
        }
    }
}
