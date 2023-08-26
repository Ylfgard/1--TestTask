using UnityEngine;
using System;

namespace Components
{
    [Serializable]
    public struct RenderComponent
    {
        public SpriteRenderer SpriteRenderer;
        public Color NormalColor;
        public Color DetectedColor;
    }
}