using Leopotam.EcsLite;
using Components.Spawn;
using AB_Utility.FromSceneToEntityConverter;
using UnityEngine;
using Components.Borders;
using Tags;
using Components.Moving;

namespace GameManagers.Systems
{
    internal class SpawnSystem : IEcsInitSystem, IEcsRunSystem
    {
        private bool _needSpawn;
        private EcsWorld _world;
        private EcsFilter _filter, _newEntityFilter;
        private EcsPool<SpawnerComponent> _spawners;
        private EcsPool<MovableComponent> _movable;
        private EcsPool<NewEntityTag> _newEntities;
        private GameObject _prefab;
        private Vector3 _spawnPoint;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<SpawnerComponent>().End();
            _newEntityFilter = _world.Filter<NewEntityTag>().End();
            _spawners = _world.GetPool<SpawnerComponent>();
            _movable = _world.GetPool<MovableComponent>();
            _newEntities = _world.GetPool<NewEntityTag>();

            ref var spawner = ref _spawners.Get(_filter.GetRawEntities()[0]);

            spawner.SpawnButton.onClick.AddListener(Spawn);
            spawner.DespawnButton.onClick.AddListener(Despawn);
            _prefab = spawner.SpawnPrefab;

            var fieldPool = _world.GetPool<FieldBordersComponent>();
            _spawnPoint = fieldPool.GetRawDenseItems()[1].Center;

            _filter = _world.Filter<DespawnableTag>().End();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var newEntity in _newEntityFilter)
                _newEntities.Del(newEntity);

            if (_needSpawn)
            {
                EcsConverter.InstantiateAndCreateEntity(_prefab, _spawnPoint, Quaternion.identity, _world);
                _needSpawn = false;
            }
        }

        private void Spawn()
        {
            _needSpawn = true;
        }

        private void Despawn()
        {
            if (_filter.GetEntitiesCount() == 0) return;

            var entity = _filter.GetRawEntities()[0];
            Object.Destroy(_movable.Get(entity).Transform.gameObject);
            _world.DelEntity(entity);
        }
    }
}