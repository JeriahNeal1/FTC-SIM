using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FTCSIM.Parts
{
    /// <summary>
    /// Manages the library of available robot parts.
    /// Loads part definitions from JSON and provides search/filter capabilities.
    /// </summary>
    public class PartsLibrary : MonoBehaviour
    {
        private static PartsLibrary instance;
        public static PartsLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PartsLibrary>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("PartsLibrary");
                        instance = go.AddComponent<PartsLibrary>();
                    }
                }
                return instance;
            }
        }
        
        private Dictionary<string, PartDefinition> parts = new Dictionary<string, PartDefinition>();
        private Dictionary<string, List<PartDefinition>> categorizedParts = new Dictionary<string, List<PartDefinition>>();
        
        public void LoadPartsFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Debug.LogWarning($"Parts directory not found: {directoryPath}");
                return;
            }
            
            string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
            
            foreach (string file in jsonFiles)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    PartDefinition part = JsonUtility.FromJson<PartDefinition>(json);
                    
                    if (part != null && !string.IsNullOrEmpty(part.sku))
                    {
                        RegisterPart(part);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load part from {file}: {e.Message}");
                }
            }
            
            Debug.Log($"Loaded {parts.Count} parts from library");
        }
        
        public void RegisterPart(PartDefinition part)
        {
            parts[part.sku] = part;
            
            if (!categorizedParts.ContainsKey(part.category))
            {
                categorizedParts[part.category] = new List<PartDefinition>();
            }
            categorizedParts[part.category].Add(part);
        }
        
        public PartDefinition GetPart(string sku)
        {
            return parts.ContainsKey(sku) ? parts[sku] : null;
        }
        
        public List<PartDefinition> GetPartsByCategory(string category)
        {
            return categorizedParts.ContainsKey(category) ? categorizedParts[category] : new List<PartDefinition>();
        }
        
        public List<PartDefinition> SearchParts(string query)
        {
            query = query.ToLower();
            return parts.Values
                .Where(p => p.displayName.ToLower().Contains(query) || 
                           p.sku.ToLower().Contains(query) ||
                           p.category.ToLower().Contains(query))
                .ToList();
        }
        
        public List<string> GetAllCategories()
        {
            return categorizedParts.Keys.ToList();
        }
        
        public void ClearLibrary()
        {
            parts.Clear();
            categorizedParts.Clear();
        }
    }
}
