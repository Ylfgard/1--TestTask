using UnityEngine;
using Converters;

namespace Debuggers
{
    public class DetectionSectorDrawGizmos : MonoBehaviour
    {
        [SerializeField] private bool _deleteOnAwake;
        [SerializeField] private bool _showSector;
        [SerializeField] private Transform _transform;
        [SerializeField] private DetectionSectorComponentConverter _sectorParameters;
        [SerializeField] [Range (1, 45)] private int _drawStep;

        private void Awake()
        {
            if (_deleteOnAwake) Destroy(this);
        }

        private void OnDrawGizmosSelected()
        {
            if (_showSector == false) return;

            Vector2 dir = Vector2.right;
            Gizmos.color = Color.red;
            float angle = _sectorParameters.Component.SectorAngle / 2;
            float radius = _sectorParameters.Component.DetectRadius;
            
            Matrix4x4 translateMatrix;
            Vector3 prevDrawPoint = _transform.position;
            Vector3 newDrawPoint;

            for (float curAngle = angle; curAngle > -angle; curAngle -= _drawStep)
            {
                newDrawPoint = dir * radius;
                translateMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(curAngle, Vector3.forward));
                newDrawPoint = translateMatrix.MultiplyVector(newDrawPoint);
                newDrawPoint += _transform.position;
                Gizmos.DrawLine(prevDrawPoint, newDrawPoint);
                prevDrawPoint = newDrawPoint;
            }

            newDrawPoint = dir * radius;
            translateMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(-angle, Vector3.forward));
            newDrawPoint = translateMatrix.MultiplyVector(newDrawPoint);
            newDrawPoint += _transform.position;
            Gizmos.DrawLine(prevDrawPoint, newDrawPoint);
            Gizmos.DrawLine(newDrawPoint, _transform.position);
        }
    }
}
