using Leopotam.EcsLite;
using Components;
using Components.Borders;
using Tags;
using UnityEngine;

namespace GameManagers.Systems
{
    public class FieldBordersInitializeSystem : IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsPool<RenderComponent> _renders;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _filter = world.Filter<RenderComponent>().Inc<FieldTag>().End();
            _renders = world.GetPool<RenderComponent>();

            if (_filter.GetEntitiesCount() > 1) Debug.LogError("Wrong field count");

            ref var render = ref _renders.Get(_filter.GetRawEntities()[0]).SpriteRenderer;
            Vector2 topRightCorner = render.bounds.center + render.bounds.extents;
            Vector2 bottomLeftCorner = render.bounds.center - render.bounds.extents;

            var newEntity = world.NewEntity();
            ref var fieldBorders = ref world.GetPool<FieldBordersComponent>().Add(newEntity);
            fieldBorders.TopBorder = topRightCorner.y;
            fieldBorders.RightBorder = topRightCorner.x;
            fieldBorders.BottomBorder = bottomLeftCorner.y;
            fieldBorders.LeftBorder = bottomLeftCorner.x;
            fieldBorders.Center = render.bounds.center;
        }
    }
}
