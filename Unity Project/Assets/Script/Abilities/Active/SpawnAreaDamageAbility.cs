using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.ObjectPooling;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class SpawnAreaDamageAbility : AbilityBase, IDamageDataAbility, IHasCastTime
    {
        public event Action<float> OnCastTimeStarted;
        public event Action<float> OnCastTimeFinished;
        [SerializeField] private AreaDamageRequestBase _areaPrefab;
        [SerializeField] private Transform _positionReference;
        [SerializeField] private Animator _animator;
        [SerializeField] private DamageData _damageData = new DamageData();
        [Header("Placement")]
        [SerializeField, Min(0)] private float _distance = 3f;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private LayerMask _groundLayers;
        [SerializeField, Min(0)] private float _groundProbeHeight = 2f;
        [SerializeField, Min(0.01f)] private float _groundProbeDistance = 5f;
        [Header("Timing")]
        [SerializeField, Min(0)] private float _castTime = 0.3f;
        [SerializeField, Min(0)] private float _explosionDelay = 0.8f;
        [SerializeField, Min(0)] private float _releaseDelay = 1f;
        [Header("Animation")]
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _animationTransitionDuration = 0.05f;
        [SerializeField] private int _animationLayerIndex;
        [SerializeField] private bool _cancelAttack = true;
        [SerializeField] private bool _blockAttacksDuringCast = true;

        private StatusInfoSourceBase _statusSource;
        private ComboControllerBase _comboController;
        private GameObject _owner;
        private ICriticalDamagePermissionsProvider _criticalPermissions;
        private IDamageResultReporter _reporter;
        private readonly List<AreaExecution> _list_Executions = new List<AreaExecution>();
        private AreaExecution _casting;

        private class AreaExecution
        {
            public AreaDamageRequestBase Area;
            public Coroutine Routine;
        }

        public float CastTime { get => _castTime; set => _castTime = Mathf.Max(0, value); }

        public void Setup(StatusInfoSourceBase source, ComboControllerBase combo, GameObject owner)
        {
            _statusSource = source;
            _comboController = combo;
            _owner = owner;
            _criticalPermissions = owner != null ? owner.GetComponent<ICriticalDamagePermissionsProvider>() : null;
            _reporter = owner != null ? owner.GetComponent<IDamageResultReporter>() : null;

            if (_positionReference == null && owner != null)
                _positionReference = owner.transform;

            if (_animator == null && owner != null) 
                _animator = owner.GetComponentInChildren<Animator>();
        }

        public void SetDamageData(IReadOnlyList<DamageData> list_Damage)
        {
            if (list_Damage != null && list_Damage.Count > 0 && list_Damage[0] != null) _damageData = list_Damage[0];
        }

        protected override bool CanExecute() => base.CanExecute() && _casting == null && isActiveAndEnabled;

        public override void Execute()
        {
            if (!CanExecute() || _areaPrefab == null || _positionReference == null) return;

            Vector3 forward = Vector3.ProjectOnPlane(_positionReference.forward, Vector3.up).normalized;

            if (forward.sqrMagnitude < 0.001f) 
                return;

            Vector3 point = _positionReference.position + forward * _distance + _offset;

            if (!Physics.Raycast(point + Vector3.up * _groundProbeHeight, Vector3.down, out RaycastHit ground,
                _groundProbeDistance, _groundLayers, QueryTriggerInteraction.Ignore))
            {
                Debug.LogWarning($"[SpawnAreaDamageAbility] No ground found at {point}. Check Ground Layers, offset and probe distance.", this);
                return;
            }

            var execution = new AreaExecution();
            _list_Executions.Add(execution);
            _casting = execution;

            if (_comboController != null)
            {
                if (_cancelAttack) 
                    _comboController.CancelAttack(false, true);

                if (_blockAttacksDuringCast) 
                    _comboController.BlockAttackInput(this);
            }

            if (_animator != null && !string.IsNullOrEmpty(_animationStateName))
                _animator.CrossFade(_animationStateName, _animationTransitionDuration, _animationLayerIndex, 0f);

            OnAbilityStarted?.Invoke(this);

            if (_casting != execution) return;

            execution.Routine = StartCoroutine(ExecuteArea(execution, ground.point, Quaternion.LookRotation(forward),
                Mathf.Max(0, _castTime), Mathf.Max(0, _explosionDelay), Mathf.Max(0, _releaseDelay)));
        }

        private IEnumerator ExecuteArea(AreaExecution execution, Vector3 position, Quaternion rotation, float cast, float delay, float release)
        {
            OnCastTimeStarted?.Invoke(cast);

            yield return new WaitForSeconds(cast);

            if (_casting != execution) 
                yield break;

            GameObject instance = PoolManager.SpawnObject(_areaPrefab.gameObject, position, rotation);

            instance.transform.SetParent(null, true);
            execution.Area = instance.GetComponent<AreaDamageRequestBase>();
            execution.Area.Setup(new DamageRequest(_damageData, _statusSource != null ? _statusSource.GetStatusInfo() : null,
                _owner != null ? _owner : gameObject, null, null, position,
                allowedCriticalTypes: _criticalPermissions != null ? _criticalPermissions.GetAllowedCriticalDamageTypes() : DamageRequest.DefaultCriticalTypes,
                attackerDamageResultReporter: _reporter));

            if (_casting != execution) 
                yield break;

            FinishCast();

            OnCastTimeFinished?.Invoke(cast);

            yield return new WaitForSeconds(delay);

            if (execution.Area == null || !execution.Area.gameObject.activeSelf) 
            { 
                _list_Executions.Remove(execution); 
                yield break; 
            }

            execution.Area.Execute();

            yield return new WaitForSeconds(release);

            Release(execution);
        }

        private void FinishCast()
        {
            if (_casting == null) return;
            _casting = null;
            if (_comboController != null) _comboController.UnblockAttackInput(this);
            OnAbilityFinished?.Invoke(this);
        }

        public override void Cancel()
        {
            if (_casting == null) return;
            AreaExecution execution = _casting;
            if (execution.Routine != null) StopCoroutine(execution.Routine);
            Release(execution);
            FinishCast();
        }

        private void Release(AreaExecution execution)
        {
            _list_Executions.Remove(execution);
            if (execution.Area != null && execution.Area.gameObject.activeSelf)
                PoolManager.ReleaseObject(execution.Area.gameObject);
            execution.Area = null;
        }

        protected override void OnDeactivate() => ClearAreas();
        private void OnDisable() 
        { 
            Cancel(); 
            ClearAreas(); 
        }

        private void ClearAreas()
        {
            StopAllCoroutines();
            while (_list_Executions.Count > 0) Release(_list_Executions[_list_Executions.Count - 1]);
        }
    }
}
