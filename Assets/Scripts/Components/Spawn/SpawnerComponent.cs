using UnityEngine.UI;
using UnityEngine;
using System;

namespace Components.Spawn
{
    [Serializable]
    public struct SpawnerComponent
    {
        public GameObject SpawnPrefab;
        public Button SpawnButton;
        public Button DespawnButton;
    }
}
