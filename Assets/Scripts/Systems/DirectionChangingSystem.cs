using Leopotam.EcsLite;
using Components.Moving;
using Components.Borders;
using UnityEngine;
using Tags;

namespace GameManagers.Systems
{
    internal class DirectionChangingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filter, _newEntityFilter;
        private EcsPool<DirectionComponent> _directions;
        private EcsPool<BordersStatusComponent> _objectsBorderStatus;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filter = world.Filter<DirectionComponent>().Inc<BordersStatusComponent>().End();
            _newEntityFilter = world.Filter<NewEntityTag>().End();

            _directions = world.GetPool<DirectionComponent>();
            _objectsBorderStatus = world.GetPool<BordersStatusComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var newEntity in _newEntityFilter)
            {
                ref var direction = ref _directions.Get(newEntity);
                ref var borderStatus = ref _objectsBorderStatus.Get(newEntity);

                direction.Direction = GetRandomDirection(borderStatus.BorderCrossing);
                direction.IsChanged = true;
            }

            foreach (var entity in _filter)
            {
                ref var borderStatus = ref _objectsBorderStatus.Get(entity);

                if (borderStatus.BorderCrossing == Border.None) continue;

                ref var direction = ref _directions.Get(entity);
                direction.Direction = GetRandomDirection(borderStatus.BorderCrossing);
                direction.IsChanged = true;
            }
        }

        private Vector2 GetRandomDirection(Border border)
        {
            float x = Random.Range(border == Border.Left ? 0f : -1f, border == Border.Right ? 0f : 1f);
            float y = Random.Range(border == Border.Bottom ? 0f : -1f, border == Border.Top ? 0f : 1f);
            return new Vector2(x, y).normalized;
        }
    }
}