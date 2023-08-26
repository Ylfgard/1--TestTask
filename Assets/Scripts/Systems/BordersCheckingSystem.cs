using Leopotam.EcsLite;
using Components.Moving;
using Components.Borders;
using Components;
using Tags;

namespace GameManagers.Systems
{
    internal class BordersCheckingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filter, _newEntityFilter;
        private EcsPool<MovableComponent> _movables;
        private EcsPool<RenderComponent> _renders;
        private EcsPool<BordersStatusComponent> _objectsBorderStatus;
        private FieldBordersComponent _fieldBorders;
        
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filter = world.Filter<MovableComponent>().Inc<RenderComponent>().End();
            _newEntityFilter = world.Filter<NewEntityTag>().End();

            _movables = world.GetPool<MovableComponent>();
            _renders = world.GetPool<RenderComponent>();
            _objectsBorderStatus = world.GetPool<BordersStatusComponent>();

            _fieldBorders = world.GetPool<FieldBordersComponent>().GetRawDenseItems()[1];
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var newEntity in _newEntityFilter)
            {
                ref var render = ref _renders.Get(newEntity).SpriteRenderer;
                ref var borderStatus = ref _objectsBorderStatus.Add(newEntity);
                borderStatus.DetectDistance = render.bounds.extents.x;
                borderStatus.BorderCrossing = Border.None;
            }

            foreach (var entity in _filter)
            {
                ref var objectTransform = ref _movables.Get(entity).Transform;
                ref var borderStatus = ref _objectsBorderStatus.Get(entity);

                borderStatus.BorderCrossing = 
                    objectTransform.position.y + borderStatus.DetectDistance >= _fieldBorders.TopBorder ? Border.Top :
                    objectTransform.position.y - borderStatus.DetectDistance <= _fieldBorders.BottomBorder ? Border.Bottom :
                    objectTransform.position.x + borderStatus.DetectDistance >= _fieldBorders.RightBorder ? Border.Right :
                    objectTransform.position.x - borderStatus.DetectDistance <= _fieldBorders.LeftBorder ? Border.Left :
                    Border.None;
            }
        }
    }
}