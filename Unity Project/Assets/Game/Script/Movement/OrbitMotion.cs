using UnityEngine;

namespace Scripts.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class OrbitMotion : MonoBehaviour
    {
        private Rigidbody _body;
        private Transform _target;
        private Vector3 _offset;
        private Vector3 _axis;
        private Vector3 _radialDirection;
        private float _radius;
        private float _speed;
        private float _angle;

        private void Awake() => _body = GetComponent<Rigidbody>();

        public void Setup(Transform target, Vector3 offset, Vector3 axis, float radius, float speed, float angle)
        {
            _target = target;
            _offset = offset;
            _axis = axis.sqrMagnitude > 0.0001f ? axis.normalized : Vector3.up;
            _radialDirection = Vector3.Cross(_axis, Mathf.Abs(Vector3.Dot(_axis, Vector3.forward)) < 0.99f ? Vector3.forward : Vector3.right).normalized;
            _radius = Mathf.Max(0, radius);
            _speed = speed;
            _angle = angle;
            if (_body == null) _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;
            _body.useGravity = false;
            if (_target != null) _body.position = GetPosition();
        }

        private Vector3 GetPosition() => _target.position + _offset +
            Quaternion.AngleAxis(_angle, _axis) * _radialDirection * _radius;

        private void FixedUpdate()
        {
            if (_target == null) return;
            _angle = Mathf.Repeat(_angle + _speed * Time.fixedDeltaTime, 360f);
            _body.MovePosition(GetPosition());
        }

        private void OnDisable() => _target = null;
    }
}
