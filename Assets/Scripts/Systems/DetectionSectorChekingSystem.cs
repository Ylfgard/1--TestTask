using Leopotam.EcsLite;
using Components.Moving;
using Components.Detecting;
using Components.Borders;
using UnityEngine;
using Tags;

namespace GameManagers
{
    internal class DetectionSectorChekingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filterDetectors, _filterDetectable, _newEntityFilter;
        private EcsPool<DetectionSectorComponent> _detetors;
        private EcsPool<MovableComponent> _movables;
        private EcsPool<DirectionComponent> _directions;
        private EcsPool<BordersStatusComponent> _borders;
        private EcsPool<DetectionStatusComponent> _detectionsStatus;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterDetectors = world.Filter<DetectionSectorComponent>().
                Inc<MovableComponent>().Inc<DirectionComponent>().End();
            _filterDetectable = world.Filter<MovableComponent>().Inc<BordersStatusComponent>().End();
            _newEntityFilter = world.Filter<NewEntityTag>().End();
            
            _detetors = world.GetPool<DetectionSectorComponent>();
            _movables = world.GetPool<MovableComponent>();
            _directions = world.GetPool<DirectionComponent>();
            _borders = world.GetPool<BordersStatusComponent>();
            _detectionsStatus = world.GetPool<DetectionStatusComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var newEntity in _newEntityFilter)
            {
                ref var detectionStatus = ref _detectionsStatus.Add(newEntity);
                detectionStatus.IsChanged = true;
                detectionStatus.IsDetected = false;

                ref var detector = ref _detetors.Get(newEntity);
                if (detector.RandomValue == false) continue;
                detector.DetectRadius = Random.Range(1f, 5f);
                detector.SectorAngle = Random.Range(1, 360);
            }

            foreach (var entity in _filterDetectors)
            {
                ref var detector = ref _detetors.Get(entity);
                ref var lookDirection = ref _directions.Get(entity);
                ref var transf = ref _movables.Get(entity).Transform;
                ref var status = ref _detectionsStatus.Get(entity);

                bool isDetected = false;
                foreach (var checkedEntity in _filterDetectable)
                { 
                    ref var detectable = ref _movables.Get(checkedEntity).Transform;
                    if (transf == detectable) continue;

                    ref var border = ref _borders.Get(checkedEntity);
                    Vector2 checkDir = detectable.position - transf.position;

                    if (Vector2.SqrMagnitude(checkDir) > Mathf.Pow(detector.DetectRadius + border.DetectDistance, 2))
                        continue;

                    Vector3 perpend = Vector2.Perpendicular(checkDir).normalized * border.DetectDistance;
                    var checkDir1 = detectable.position + perpend - transf.position;
                    var checkDir2 = detectable.position - perpend - transf.position;
                    var angle1 = Vector2.SignedAngle(lookDirection.Direction, checkDir1);
                    var angle2 = Vector2.SignedAngle(lookDirection.Direction, checkDir2);
                    if (Mathf.Abs(angle1) > detector.SectorAngle / 2 && Mathf.Abs(angle2) > detector.SectorAngle / 2
                        && (Mathf.Sign(angle1) == Mathf.Sign(angle2) || Mathf.Abs(angle1) > 90 || Mathf.Abs(angle2) > 90))
                        continue;

                    isDetected = true;
                    break;
                }

                if (isDetected == status.IsDetected) continue;
                status.IsChanged = true;
                status.IsDetected = isDetected;
            }
        }
    }
}