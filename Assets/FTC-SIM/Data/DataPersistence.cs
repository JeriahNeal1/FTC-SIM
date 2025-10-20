using UnityEngine;
using System;
using System.IO;
using FTCSIM.Assembly;
using FTCSIM.Electronics;

namespace FTCSIM.Data
{
    /// <summary>
    /// Handles saving and loading of robot assemblies and electronics to/from JSON
    /// </summary>
    public class DataPersistence : MonoBehaviour
    {
        public string saveDirectory = "SavedRobots";
        
        private void Awake()
        {
            // Ensure save directory exists
            string fullPath = Path.Combine(Application.persistentDataPath, saveDirectory);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }
        
        /// <summary>
        /// Save robot to JSON file
        /// </summary>
        public bool SaveRobot(string robotName, AssemblyGraph assemblyGraph, ElectroGraph electroGraph)
        {
            try
            {
                var robotData = new RobotData
                {
                    version = "1.0",
                    robotName = robotName,
                    timestamp = DateTime.Now.ToString("o"),
                    assemblyRoot = SerializeAssemblyNode(assemblyGraph.rootNode),
                    electronicsGraph = SerializeElectronicsGraph(electroGraph)
                };
                
                string json = JsonUtility.ToJson(robotData, true);
                string fileName = $"{robotName}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string fullPath = Path.Combine(Application.persistentDataPath, saveDirectory, fileName);
                
                File.WriteAllText(fullPath, json);
                
                Debug.Log($"Robot saved to: {fullPath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save robot: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Load robot from JSON file
        /// </summary>
        public bool LoadRobot(string filePath, out AssemblyGraph assemblyGraph, out ElectroGraph electroGraph)
        {
            assemblyGraph = null;
            electroGraph = null;
            
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"File not found: {filePath}");
                    return false;
                }
                
                string json = File.ReadAllText(filePath);
                var robotData = JsonUtility.FromJson<RobotData>(json);
                
                if (robotData == null)
                {
                    Debug.LogError("Failed to parse robot data");
                    return false;
                }
                
                // Create assembly graph
                GameObject graphObj = new GameObject("AssemblyGraph");
                assemblyGraph = graphObj.AddComponent<AssemblyGraph>();
                assemblyGraph.rootNode = DeserializeAssemblyNode(robotData.assemblyRoot);
                assemblyGraph.Initialize();
                
                // Create electronics graph
                GameObject electroObj = new GameObject("ElectroGraph");
                electroGraph = electroObj.AddComponent<ElectroGraph>();
                DeserializeElectronicsGraph(robotData.electronicsGraph, electroGraph);
                
                Debug.Log($"Robot loaded from: {filePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load robot: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Get list of saved robot files
        /// </summary>
        public string[] GetSavedRobots()
        {
            string fullPath = Path.Combine(Application.persistentDataPath, saveDirectory);
            if (!Directory.Exists(fullPath))
            {
                return new string[0];
            }
            
            return Directory.GetFiles(fullPath, "*.json");
        }
        
        private SerializableAssemblyNode SerializeAssemblyNode(AssemblyNode node)
        {
            if (node == null) return null;
            
            var serialized = new SerializableAssemblyNode
            {
                id = node.id,
                displayName = node.displayName,
                partSKU = node.partSKU,
                isSubassembly = node.isSubassembly,
                localPosition = node.localPosition,
                localRotation = node.localRotation,
                mass = node.mass
            };
            
            foreach (var child in node.children)
            {
                serialized.children.Add(SerializeAssemblyNode(child));
            }
            
            serialized.relations.AddRange(node.relations);
            
            return serialized;
        }
        
        private AssemblyNode DeserializeAssemblyNode(SerializableAssemblyNode serialized)
        {
            if (serialized == null) return null;
            
            var node = new AssemblyNode(serialized.id, serialized.displayName, serialized.isSubassembly)
            {
                partSKU = serialized.partSKU,
                localPosition = serialized.localPosition,
                localRotation = serialized.localRotation,
                mass = serialized.mass
            };
            
            node.relations.AddRange(serialized.relations);
            
            foreach (var childSerialized in serialized.children)
            {
                node.children.Add(DeserializeAssemblyNode(childSerialized));
            }
            
            return node;
        }
        
        private SerializableElectronicsGraph SerializeElectronicsGraph(ElectroGraph graph)
        {
            if (graph == null) return null;
            
            return new SerializableElectronicsGraph
            {
                nodes = graph.nodes,
                wires = graph.wires
            };
        }
        
        private void DeserializeElectronicsGraph(SerializableElectronicsGraph serialized, ElectroGraph graph)
        {
            if (serialized == null || graph == null) return;
            
            graph.nodes = serialized.nodes;
            graph.wires = serialized.wires;
            graph.Initialize();
        }
    }
    
    [Serializable]
    public class RobotData
    {
        public string version;
        public string robotName;
        public string timestamp;
        public SerializableAssemblyNode assemblyRoot;
        public SerializableElectronicsGraph electronicsGraph;
    }
    
    [Serializable]
    public class SerializableAssemblyNode
    {
        public string id;
        public string displayName;
        public string partSKU;
        public bool isSubassembly;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public float mass;
        public System.Collections.Generic.List<SerializableAssemblyNode> children = 
            new System.Collections.Generic.List<SerializableAssemblyNode>();
        public System.Collections.Generic.List<AssemblyRelation> relations = 
            new System.Collections.Generic.List<AssemblyRelation>();
    }
    
    [Serializable]
    public class SerializableElectronicsGraph
    {
        public System.Collections.Generic.List<ElectroNode> nodes = 
            new System.Collections.Generic.List<ElectroNode>();
        public System.Collections.Generic.List<ElectroWire> wires = 
            new System.Collections.Generic.List<ElectroWire>();
    }
}
