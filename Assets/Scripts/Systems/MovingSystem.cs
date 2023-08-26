using Leopotam.EcsLite;
using UnityEngine;
using Components.Moving;
using Tags;

namespace GameManagers.Systems
{
    internal class MovingSystem : IEcsInitSystem, IEcsRunSystem 
    {
        private EcsFilter _filter, _newEntityFilter;
        private EcsPool<MovableComponent> _movables;
        private EcsPool<DirectionComponent> _directions;

        public void Init (IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _filter = world.Filter<MovableComponent>().Inc<DirectionComponent>().End();
            _newEntityFilter = world.Filter<NewEntityTag>().End();

            _movables = world.GetPool<MovableComponent>();
            _directions = world.GetPool<DirectionComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var newEntity in _newEntityFilter)
            {
                ref var movable = ref _movables.Get(newEntity);
                movable.Speed = Random.Range(1f, 10f);
            }

            foreach (var entity in _filter)
            {
                ref var movable = ref _movables.Get(entity);
                ref var direction = ref _directions.Get(entity);
                Vector3 step = direction.Direction * movable.Speed * Time.deltaTime;
                movable.Transform.position += step;
            }
        }
    }
}