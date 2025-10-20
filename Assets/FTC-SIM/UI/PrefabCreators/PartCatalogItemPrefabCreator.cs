using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FTCSIM.UI
{
    /// <summary>
    /// Helper script to create the PartCatalogItem prefab programmatically.
    /// This creates the UI template for displaying parts in the catalog.
    /// </summary>
    [ExecuteInEditMode]
    public class PartCatalogItemPrefabCreator : MonoBehaviour
    {
        [ContextMenu("Create Part Catalog Item Prefab")]
        public void CreatePrefab()
        {
            GameObject itemObj = new GameObject("PartCatalogItem");
            
            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 100);
            
            // Background
            Image bg = itemObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Add layout group
            VerticalLayoutGroup layout = itemObj.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 5;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = false;
            
            // Part Name Text
            GameObject nameObj = new GameObject("PartNameText");
            nameObj.transform.SetParent(itemObj.transform, false);
            TMP_Text nameText = nameObj.AddComponent<TMP_Text>();
            nameText.text = "Part Name";
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = Color.white;
            
            LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
            nameLayout.preferredHeight = 20;
            
            // Info Container
            GameObject infoContainer = new GameObject("InfoContainer");
            infoContainer.transform.SetParent(itemObj.transform, false);
            HorizontalLayoutGroup infoLayout = infoContainer.AddComponent<HorizontalLayoutGroup>();
            infoLayout.spacing = 10;
            infoLayout.childForceExpandWidth = true;
            infoLayout.childForceExpandHeight = false;
            
            LayoutElement infoContainerLayout = infoContainer.AddComponent<LayoutElement>();
            infoContainerLayout.preferredHeight = 20;
            
            // Category Text
            GameObject categoryObj = new GameObject("PartCategoryText");
            categoryObj.transform.SetParent(infoContainer.transform, false);
            TMP_Text categoryText = categoryObj.AddComponent<TMP_Text>();
            categoryText.text = "Category";
            categoryText.fontSize = 12;
            categoryText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            
            // Mass Text
            GameObject massObj = new GameObject("PartMassText");
            massObj.transform.SetParent(infoContainer.transform, false);
            TMP_Text massText = massObj.AddComponent<TMP_Text>();
            massText.text = "0.000 kg";
            massText.fontSize = 12;
            massText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            massText.alignment = TextAlignmentOptions.Right;
            
            // Icon placeholder
            GameObject iconObj = new GameObject("PartIcon");
            iconObj.transform.SetParent(itemObj.transform, false);
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(40, 40);
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            LayoutElement iconLayout = iconObj.AddComponent<LayoutElement>();
            iconLayout.preferredHeight = 40;
            iconLayout.preferredWidth = 40;
            
            // Add PartCatalogItem component
            PartCatalogItem catalogItem = itemObj.AddComponent<PartCatalogItem>();
            catalogItem.partNameText = nameText;
            catalogItem.partCategoryText = categoryText;
            catalogItem.partMassText = massText;
            catalogItem.partIcon = iconImage;
            
            // Add button component for clicking
            Button button = itemObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            button.colors = colors;
            
            Debug.Log("PartCatalogItem prefab created! Save this as a prefab in your project.");
            
            // Try to select it in editor
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = itemObj;
            #endif
        }
    }
}
