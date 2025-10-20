using UnityEngine;
using System;

namespace FTCSIM.Core
{
    /// <summary>
    /// Defines a point where parts can be attached using goBILDA or other modular patterns.
    /// </summary>
    [Serializable]
    public class AttachmentPoint
    {
        public string id;
        public string pattern; // e.g., "Pattern:goBILDA-16mm"
        public Vector3 localPosition;
        public Quaternion localRotation;
        public float tolerance = 0.001f; // meters
        public string[] compatiblePatterns;
        
        public AttachmentPoint(string id, string pattern, Vector3 localPosition, Quaternion localRotation)
        {
            this.id = id;
            this.pattern = pattern;
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            this.compatiblePatterns = new string[] { pattern };
        }
    }

    /// <summary>
    /// Defines a rotational mount for shafts, bearings, etc.
    /// </summary>
    [Serializable]
    public class RotationalMount
    {
        public string id;
        public string mountType; // e.g., "Shaft-5mm", "Bearing-8mm"
        public Vector3 localPosition;
        public Vector3 axis; // Rotation axis in local space
        public float diameter; // meters
        public float length; // meters
        public bool canDrive = false;
        
        public RotationalMount(string id, string mountType, Vector3 localPosition, Vector3 axis, float diameter)
        {
            this.id = id;
            this.mountType = mountType;
            this.localPosition = localPosition;
            this.axis = axis;
            this.diameter = diameter;
        }
    }
}
