using Leopotam.EcsLite;
using Components.Moving;
using Components.Detecting;
using UnityEngine;
using Tags;

namespace GameManagers.Systems
{
    internal class DetectionSectorDisplaySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filter, _newEntityFilter;
        private EcsPool<DirectionComponent> _directions;
        private EcsPool<DetectionSectorComponent> _detetors;
        private EcsPool<SectorDrawComponent> _drawers;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filter = world.Filter<DetectionSectorComponent>().
                Inc<SectorDrawComponent>().Inc<DirectionComponent>().End();
            _newEntityFilter = world.Filter<NewEntityTag>().End();

            _directions = world.GetPool<DirectionComponent>();
            _drawers = world.GetPool<SectorDrawComponent>();
            _detetors = world.GetPool<DetectionSectorComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var newEntity in _newEntityFilter)
            {
                ref var drawer = ref _drawers.Get(newEntity);
                ref var detector = ref _detetors.Get(newEntity);

                drawer.Image.fillAmount = detector.SectorAngle / 360f;
                drawer.Transform.localScale = Vector3.one * detector.DetectRadius;
            }

            foreach (var entity in _filter)
            {
                ref var direction = ref _directions.Get(entity);

                if (direction.IsChanged == false) continue;

                ref var drawerTransf = ref _drawers.Get(entity).Transform;
                ref var detector = ref _detetors.Get(entity);

                float angle = Vector2.SignedAngle(Vector2.right, direction.Direction) + detector.SectorAngle / 2;
                Vector3 rotation = new Vector3(0, 0, angle);
                drawerTransf.rotation = Quaternion.Euler(rotation);
                direction.IsChanged = false;
            }
        }
    }
}