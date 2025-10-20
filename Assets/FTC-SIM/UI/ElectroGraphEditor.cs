using UnityEngine;
using UnityEngine.UI;
using FTCSIM.Core;
using FTCSIM.Electronics;
using TMPro;
using System.Collections.Generic;

namespace FTCSIM.UI
{
    /// <summary>
    /// Visual node editor for creating and editing electrical connections.
    /// </summary>
    public class ElectroGraphEditor : MonoBehaviour
    {
        [Header("UI Elements")]
        public RectTransform canvasTransform;
        public GameObject nodeUIPrefab;
        public GameObject wireUIPrefab;
        public TMP_Dropdown deviceTypeDropdown;
        public Button addNodeButton;
        
        [Header("References")]
        public RobotController robotController;
        
        private Dictionary<string, GameObject> nodeUIObjects = new Dictionary<string, GameObject>();
        private List<GameObject> wireUIObjects = new List<GameObject>();
        private ElectroNode selectedNode;
        private ElectroPin selectedPin;
        private bool isDrawingWire = false;
        
        private void Start()
        {
            if (robotController == null)
            {
                robotController = FindObjectOfType<RobotController>();
            }
            
            if (addNodeButton != null)
            {
                addNodeButton.onClick.AddListener(OnAddNodeClicked);
            }
            
            PopulateDeviceTypeDropdown();
        }
        
        private void PopulateDeviceTypeDropdown()
        {
            if (deviceTypeDropdown == null) return;
            
            deviceTypeDropdown.ClearOptions();
            
            List<string> deviceTypes = new List<string>
            {
                "Battery",
                "Hub",
                "DCMotor",
                "Servo",
                "Sensor"
            };
            
            deviceTypeDropdown.AddOptions(deviceTypes);
        }
        
        public void RefreshGraph()
        {
            ClearUI();
            
            if (robotController == null || robotController.electroGraph == null) return;
            
            // Create UI for each node
            foreach (var node in robotController.electroGraph.nodes)
            {
                CreateNodeUI(node);
            }
            
            // Create UI for each wire
            foreach (var wire in robotController.electroGraph.wires)
            {
                CreateWireUI(wire);
            }
        }
        
        private void ClearUI()
        {
            foreach (var nodeUI in nodeUIObjects.Values)
            {
                if (nodeUI != null) Destroy(nodeUI);
            }
            nodeUIObjects.Clear();
            
            foreach (var wireUI in wireUIObjects)
            {
                if (wireUI != null) Destroy(wireUI);
            }
            wireUIObjects.Clear();
        }
        
        private void CreateNodeUI(ElectroNode node)
        {
            if (nodeUIPrefab == null || canvasTransform == null) return;
            
            GameObject nodeUI = Instantiate(nodeUIPrefab, canvasTransform);
            nodeUI.name = "Node_" + node.id;
            
            // Position based on node's graph position
            RectTransform rectTransform = nodeUI.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = node.graphPosition;
            }
            
            // Setup node UI component
            var nodeUIComponent = nodeUI.GetComponent<ElectroNodeUI>();
            if (nodeUIComponent != null)
            {
                nodeUIComponent.Initialize(node, this);
            }
            
            nodeUIObjects[node.id] = nodeUI;
        }
        
        private void CreateWireUI(ElectroWire wire)
        {
            if (wireUIPrefab == null || canvasTransform == null) return;
            
            GameObject wireUI = Instantiate(wireUIPrefab, canvasTransform);
            wireUI.name = "Wire_" + wire.id;
            
            var wireUIComponent = wireUI.GetComponent<ElectroWireUI>();
            if (wireUIComponent != null)
            {
                GameObject nodeA = nodeUIObjects.ContainsKey(wire.a.nodeId) ? nodeUIObjects[wire.a.nodeId] : null;
                GameObject nodeB = nodeUIObjects.ContainsKey(wire.b.nodeId) ? nodeUIObjects[wire.b.nodeId] : null;
                
                wireUIComponent.Initialize(wire, nodeA, nodeB);
            }
            
            wireUIObjects.Add(wireUI);
        }
        
        private void OnAddNodeClicked()
        {
            if (deviceTypeDropdown == null || robotController == null) return;
            
            string deviceType = deviceTypeDropdown.options[deviceTypeDropdown.value].text;
            string nodeId = deviceType.ToLower() + "_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
            
            ElectroNode newNode = new ElectroNode(nodeId, deviceType);
            
            // Add default pins based on device type
            AddDefaultPins(newNode, deviceType);
            
            // Set position in center of canvas
            newNode.graphPosition = Vector2.zero;
            
            // Add to graph
            if (robotController.electroGraph.AddNode(newNode))
            {
                CreateNodeUI(newNode);
            }
        }
        
        private void AddDefaultPins(ElectroNode node, string deviceType)
        {
            switch (deviceType)
            {
                case "Battery":
                    node.pins.Add(new ElectroPin("pos", ElectroPin.PinRole.PWR) { voltage = 12f, maxCurrent = 20f });
                    node.pins.Add(new ElectroPin("neg", ElectroPin.PinRole.GND) { maxCurrent = 20f });
                    break;
                    
                case "Hub":
                    node.pins.Add(new ElectroPin("pwr_in", ElectroPin.PinRole.PWR) { maxCurrent = 15f });
                    node.pins.Add(new ElectroPin("gnd_in", ElectroPin.PinRole.GND) { maxCurrent = 15f });
                    for (int i = 0; i < 4; i++)
                    {
                        node.pins.Add(new ElectroPin($"motor_{i}_plus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
                        node.pins.Add(new ElectroPin($"motor_{i}_minus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
                    }
                    break;
                    
                case "DCMotor":
                    node.pins.Add(new ElectroPin("plus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
                    node.pins.Add(new ElectroPin("minus", ElectroPin.PinRole.PWR) { maxCurrent = 10f });
                    break;
                    
                case "Servo":
                    node.pins.Add(new ElectroPin("power", ElectroPin.PinRole.PWR) { maxCurrent = 2f });
                    node.pins.Add(new ElectroPin("ground", ElectroPin.PinRole.GND) { maxCurrent = 2f });
                    node.pins.Add(new ElectroPin("signal", ElectroPin.PinRole.SIGNAL));
                    break;
                    
                case "Sensor":
                    node.pins.Add(new ElectroPin("vcc", ElectroPin.PinRole.PWR) { maxCurrent = 0.1f });
                    node.pins.Add(new ElectroPin("gnd", ElectroPin.PinRole.GND) { maxCurrent = 0.1f });
                    node.pins.Add(new ElectroPin("data", ElectroPin.PinRole.SIGNAL));
                    break;
            }
        }
        
        public void OnPinClicked(ElectroNode node, ElectroPin pin)
        {
            if (!isDrawingWire)
            {
                // Start drawing wire
                selectedNode = node;
                selectedPin = pin;
                isDrawingWire = true;
            }
            else
            {
                // Complete wire
                if (selectedNode != null && selectedPin != null)
                {
                    CreateWire(selectedNode, selectedPin, node, pin);
                }
                
                isDrawingWire = false;
                selectedNode = null;
                selectedPin = null;
            }
        }
        
        private void CreateWire(ElectroNode nodeA, ElectroPin pinA, ElectroNode nodeB, ElectroPin pinB)
        {
            if (robotController == null) return;
            
            string wireId = "wire_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
            
            ElectroWire.WireType wireType = pinA.role == ElectroPin.PinRole.GND 
                ? ElectroWire.WireType.Ground 
                : ElectroWire.WireType.Power;
            
            ElectroWire newWire = new ElectroWire(wireId, wireType, nodeA.id, pinA.id, nodeB.id, pinB.id);
            
            if (robotController.electroGraph.AddWire(newWire))
            {
                CreateWireUI(newWire);
            }
        }
        
        public void OnNodeMoved(string nodeId, Vector2 newPosition)
        {
            if (robotController == null) return;
            
            var node = robotController.electroGraph.GetNode(nodeId);
            if (node != null)
            {
                node.graphPosition = newPosition;
                
                // Update wire visuals
                RefreshWireUI();
            }
        }
        
        private void RefreshWireUI()
        {
            foreach (var wireUI in wireUIObjects)
            {
                if (wireUI != null)
                {
                    var wireUIComponent = wireUI.GetComponent<ElectroWireUI>();
                    if (wireUIComponent != null)
                    {
                        wireUIComponent.UpdatePosition();
                    }
                }
            }
        }
    }
}
