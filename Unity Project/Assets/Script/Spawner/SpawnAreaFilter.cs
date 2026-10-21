using System;
using UnityEngine;

namespace Scripts.Spawner
{
    [Serializable]
    public class SpawnAreaFilter
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private LayerMask _blockingLayers;
        [SerializeField] private Vector3 _centerOffset = new Vector3(0f, 1f, 0f);
        [SerializeField] private Vector3 _halfExtents = new Vector3(0.5f, 0.9f, 0.5f);
        [SerializeField] private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Collide;

        public bool IsBlocked(Vector3 position, Quaternion rotation)
        {
            if (!_enabled) return false;

            // Include objects spawned or repositioned earlier in this frame.
            Physics.SyncTransforms();
            return Physics.CheckBox(position + rotation * _centerOffset,
                GetHalfExtents(), rotation, _blockingLayers, _triggerInteraction);
        }

        public void DrawGizmo(Vector3 position, Quaternion rotation)
        {
            if (!_enabled) return;

            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position + rotation * _centerOffset, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, GetHalfExtents() * 2f);
            Gizmos.matrix = previousMatrix;
        }

        private Vector3 GetHalfExtents()
        {
            return new Vector3(Mathf.Max(0.01f, Mathf.Abs(_halfExtents.x)),
                Mathf.Max(0.01f, Mathf.Abs(_halfExtents.y)),
                Mathf.Max(0.01f, Mathf.Abs(_halfExtents.z)));
        }
    }
}
