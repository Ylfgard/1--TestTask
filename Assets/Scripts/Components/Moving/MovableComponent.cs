using System;
using UnityEngine;

namespace Components.Moving
{
    [Serializable]
    public struct MovableComponent
    {
        public Transform Transform;
        public float Speed;
    }
}