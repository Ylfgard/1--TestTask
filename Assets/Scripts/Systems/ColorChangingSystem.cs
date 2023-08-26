using Leopotam.EcsLite;
using Components.Detecting;
using Components;

namespace GameManagers.Systems
{
    internal class ColorChangingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filter;
        private EcsPool<DetectionStatusComponent> _detectionStatus;
        private EcsPool<RenderComponent> _renders;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filter = world.Filter<DetectionStatusComponent>().Inc<RenderComponent>().End();

            _detectionStatus = world.GetPool<DetectionStatusComponent>();
            _renders = world.GetPool<RenderComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var status = ref _detectionStatus.Get(entity);
                ref var render = ref _renders.Get(entity);

                if (status.IsChanged == false) continue;
                render.SpriteRenderer.color = status.IsDetected ? render.DetectedColor : render.NormalColor;
                status.IsChanged = false;
            }
        }
    }
}