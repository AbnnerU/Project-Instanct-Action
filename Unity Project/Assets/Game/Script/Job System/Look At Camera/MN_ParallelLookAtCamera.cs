using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Scripts.Job.LookAtCamera
{
    public class MN_ParallelLookAtCamera : SceneSingleton<MN_ParallelLookAtCamera>
    {
        [SerializeField] private Transform _cameraTransform;

        private List<Transform> _list_transformsToUpdate = new List<Transform>();

        private TransformAccessArray _transformAccessArray;

        private JobHandle _jobHandle;
        private bool _isDirty = false; 



        private void OnDestroy()
        {
            if (_transformAccessArray.isCreated)
                _transformAccessArray.Dispose();         
        }

        private void LateUpdate()
        {
            _jobHandle.Complete();

            if (_list_transformsToUpdate.Count == 0) return;
            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main?.transform;
                if (_cameraTransform == null) return;
            }

            if (_isDirty)
            {
                if (_transformAccessArray.isCreated)
                {
                    _transformAccessArray.Dispose();
                }
                _transformAccessArray = new TransformAccessArray(_list_transformsToUpdate.ToArray());
                _isDirty = false;
            }

            var job = new LookAtCameraJob
            {
                CameraPosition = _cameraTransform.position,
                WorldUp = Vector3.up
            };

            _jobHandle = job.Schedule(_transformAccessArray);
        }

        public void TryAddAgent(Transform agentTransform)
        {
            if (agentTransform != null && !_list_transformsToUpdate.Contains(agentTransform))
            {
                _list_transformsToUpdate.Add(agentTransform);
                _isDirty = true;
            }
        }

        public void TryRemoveAgent(Transform agentTransform)
        {
            if (agentTransform != null && _list_transformsToUpdate.Remove(agentTransform))
            {
                _isDirty = true;
            }
        }
    }


    [BurstCompile]
    public struct LookAtCameraJob : IJobParallelForTransform
    {
        [ReadOnly] public Vector3 CameraPosition;
        [ReadOnly] public Vector3 WorldUp;
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 direction = CameraPosition - transform.position;

            transform.rotation = Quaternion.LookRotation(direction, WorldUp);
        }
    }
}