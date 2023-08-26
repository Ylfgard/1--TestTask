using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using GameManagers.Systems;
using UnityEngine;

namespace GameManagers 
{
    sealed class EcsStartup : MonoBehaviour 
    {
        private EcsWorld _world;        
        private IEcsSystems _systems;

        void Start () 
        {
            _world = new EcsWorld ();
            _systems = new EcsSystems (_world);
            _systems
                
                .Add(new FieldBordersInitializeSystem())
                
                .Add(new MovingSystem())
                .Add(new BordersCheckingSystem())
                .Add(new DirectionChangingSystem())
                .Add(new DetectionSectorChekingSystem())
                .Add(new DetectionSectorDisplaySystem())
                .Add(new ColorChangingSystem())
                .Add(new SpawnSystem())

#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif

                .ConvertScene()
                .Init();
        }

        void Update () 
        {
            _systems?.Run();
        }

        void OnDestroy () 
        {
            if (_systems != null) 
            {
                _systems.Destroy();
                _systems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}