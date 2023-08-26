using UnityEngine;
using System;

namespace Components.Detecting
{
    [Serializable]
    public struct DetectionSectorComponent
    {
        public bool RandomValue;
        [Range (0, 15)] public float DetectRadius;
        [Range (0, 360)] public int SectorAngle;
    }
}