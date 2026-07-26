using Scripts.AI;
using Scripts.Animation;
using Scripts.BT;
using Scripts.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.BehaviourTree.Agents
{
    [RequireComponent(typeof(BehaviorTree))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyBrain : MonoBehaviour, IHasBehaviourTree, IHasAIState, INavMeshAgentStateUpdate, IHasCurrentVelocity
    {
        public event Action<AIState> OnAINewState;
        public event Action<AgentMovementOrder> OnOrderChanged;
        public event Action<Transform> OnTargetChanged;

        [SerializeField] private bool _autoSetup = true;
        [SerializeField] private AgentMovementOrder _eCurrentOrder = AgentMovementOrder.STAND;
        [SerializeField] private AIState _eCurrentAiState = AIState.STANDING;

        [Header("References")]
        [SerializeField] private BehaviorTree _behaviorTree;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private string _targetTag;
        [SerializeField] private Transform _currentTarget;
        [SerializeField] private Vector3 _currentTargetPosition;

        [Header("Movement")]
        [SerializeField] private float _movementSpeed = 3.5f;
        //[SerializeField] private float _stoppingDistance = 0.25f;
        [SerializeField] private float _targetUpdateInterval = 0.1f;
        [SerializeField] private bool _standWhenTargetIsReached = true;

        [Header("Combat")]
        [SerializeField] private float _attackDistance = 1.5f;
        [SerializeField] private float _maxAttackDuration = 2f;
        [SerializeField] private bool _useXZDistanceForAttack = true;

        [Header("Behaviour Tree")]
        [SerializeField] private float _btExecutionInterval = 0.1f;

        private Func<Transform> _func_TargetProvider;
        private BTSelector _rootSelector;
        private bool _isSetup;
        private bool _isBehaviourTreeStarted;
        private int _currentAreaMask;
        private Vector3 _currentVelocity;
        private bool _isOnNavMeshLink;

        public enum AgentMovementOrder
        {
            STAND = 0,
            GO_TO_TARGET = 1,
            PATROL = 2,
            GO_TO_POSITION = 3
        }

        private void Reset()
        {
            GetReferences();
        }

        private void Awake()
        {
            if (_autoSetup)
            {
                Setup();
                StartBehaviourTree();
            }
        }

        private void OnDisable()
        {
            StopBehaviourTree();
        }

        private void OnEnable()
        {
            if (_autoSetup && _isSetup && !_isBehaviourTreeStarted)
                StartBehaviourTree();
        }

        public void Setup()
        {
            GetReferences();
            BuildBehaviourTree();
            _isSetup = true;
        }

        public void StartBehaviourTree()
        {
            if (!_isSetup)
                Setup();

            if (_behaviorTree == null || _rootSelector == null || _isBehaviourTreeStarted)
                return;

            _behaviorTree.SetActive(true);
            _behaviorTree.SetBehaviorRoot(_rootSelector);
            _behaviorTree.SetExecutionInterval(_btExecutionInterval);
            _behaviorTree.StartCoroutine(_behaviorTree.Begin());
            _isBehaviourTreeStarted = true;
        }

        public void StopBehaviourTree()
        {
            if (_behaviorTree == null || !_isBehaviourTreeStarted)
                return;

            _behaviorTree.Stop();
            _isBehaviourTreeStarted = false;
        }

        public void Stand()
        {
            //SetOrder(AgentMovementOrder.STAND);
            StopAgent();
        }

        public void GoToTarget()
        {
            SetOrder(AgentMovementOrder.GO_TO_TARGET);
        }

        public void GoToTarget(Transform target)
        {
            SetCurrentTarget(target);
            GoToTarget();
        }

        public void GoToPosition(Vector3 position)
        {
            SetCurrentTargetPosition(position);
            SetOrder(AgentMovementOrder.GO_TO_POSITION);
        }

        public void Patrol()
        {
            SetOrder(AgentMovementOrder.PATROL);
        }

        public void SetOrder(AgentMovementOrder newOrder)
        {
            if (_eCurrentOrder == newOrder)
            {
                if (newOrder == AgentMovementOrder.STAND)
                    StopAgent();

                return;
            }

            _eCurrentOrder = newOrder;

            if (newOrder == AgentMovementOrder.STAND)
                StopAgent();

            OnOrderChanged?.Invoke(newOrder);
        }

        public AgentMovementOrder GetCurrentOrder()
        {
            return _eCurrentOrder;
        }

        public void SetCurrentTarget(Transform target)
        {
            if (_currentTarget == target)
                return;

            _currentTarget = target;
            OnTargetChanged?.Invoke(target);
        }

        public Transform GetCurrentTarget()
        {
            return GetConfiguredTarget();
        }

        public void SetCurrentTargetPosition(Vector3 position)
        {
            _currentTargetPosition = position;
        }

        public Vector3 GetCurrentTargetPosition()
        {
            return _currentTargetPosition;
        }

        public void SetTargetProvider(Func<Transform> targetProvider)
        {
            _func_TargetProvider = targetProvider;
        }

        public void ClearTargetProvider()
        {
            _func_TargetProvider = null;
        }

        public void SetMovementSpeed(float movementSpeed)
        {
            _movementSpeed = movementSpeed;

            if (_navMeshAgent != null)
                _navMeshAgent.speed = movementSpeed;

            if (!_isSetup)
                return;

            BuildBehaviourTree();

            if (_behaviorTree != null)
                _behaviorTree.SetBehaviorRoot(_rootSelector);
        }

        public AIState GetAIState()
        {
            return _eCurrentAiState;
        }

        public bool IsOnNavMeshLink()
        {
            return _isOnNavMeshLink;
        }

        public int GetCurrentAreaMask()
        {
            return _currentAreaMask;
        }

        public Vector3 GetCurrentVelocity()
        {
            return _currentVelocity;
        }

        public void SetAIState(AIState newState)
        {
            if (_eCurrentAiState == newState)
                return;

            _eCurrentAiState = newState;
            OnAINewState?.Invoke(newState);
        }

        public void AgentUpdate(bool isOnNavMeshLink, int areaMask, Vector3 velocity)
        {
            _isOnNavMeshLink = isOnNavMeshLink;
            _currentAreaMask = areaMask;
            _currentVelocity = velocity;
        }

        private void GetReferences()
        {
            if (_behaviorTree == null)
                TryGetComponent(out _behaviorTree);

            if (_navMeshAgent == null)
                TryGetComponent(out _navMeshAgent);

            if (_comboController == null)
                TryGetComponent(out _comboController);

            if (!string.IsNullOrEmpty(_targetTag))
            {
                GameObject targetObject = GameObject.FindGameObjectWithTag(_targetTag);
                if (targetObject != null)
                    _currentTarget = targetObject.transform;
            }
        }

        private void BuildBehaviourTree()
        {
            BTBoolFunc btIsGoToTargetOrder = new BTBoolFunc(() => _eCurrentOrder == AgentMovementOrder.GO_TO_TARGET);
            BTBoolFunc btHasTarget = new BTBoolFunc(() => GetMovementTarget() != null);

            BTSetAIState btSetStandingState = new BTSetAIState(this, AIState.STANDING);
            BTSetAIState btSetMoveToTargetState = new BTSetAIState(this, AIState.MOVE_TO_TARGET);
            BTSetAIState btSetPatrolState = new BTSetAIState(this, AIState.PATROLLING);
            BTSetAIState btSetAttackState = new BTSetAIState(this, AIState.ATTACKING);

            BTDoAction btStopAgent = new BTDoAction(StopAgent);
            BTDoAction btHandleTargetReached = new BTDoAction(HandleTargetReached);
            BTLookAtTarget btLookAtTarget = new BTLookAtTarget(GetConfiguredTarget, transform, true);
            BTIsTargetInRange btIsTargetInAttackRange = new BTIsTargetInRange(GetConfiguredTarget, transform, _attackDistance, _useXZDistanceForAttack);
            BTComboAttack btComboAttack = new BTComboAttack(_comboController, _maxAttackDuration);

            BTGoToTarget btGoToTarget = new BTGoToTarget(
                GetMovementTarget,
                _navMeshAgent,
                this,
                _attackDistance,
                _targetUpdateInterval,
                _movementSpeed);

            BTSequence btGoToTargetSequence = new BTSequence(new List<BTnode>
            {
                btIsGoToTargetOrder,
                btHasTarget,
                btSetMoveToTargetState,
                btGoToTarget,
                btHandleTargetReached
            });

            BTSequence btAttackSequence = new BTSequence(new List<BTnode>
            {
                btIsGoToTargetOrder,
                btHasTarget,
                btIsTargetInAttackRange,
                btSetAttackState,
                btStopAgent,
                btLookAtTarget,
                btComboAttack
            });

            BTSequence btStandSequence = new BTSequence(new List<BTnode>
            {
                btSetStandingState,
                btStopAgent
            });

            _rootSelector = new BTSelector(new List<BTnode>
            {
                btAttackSequence,
                btGoToTargetSequence,
                btStandSequence
            });
        }

        private Transform GetConfiguredTarget()
        {
            return _func_TargetProvider?.Invoke() ?? _currentTarget;
        }

        private Transform GetMovementTarget()
        {
            if (_eCurrentOrder != AgentMovementOrder.GO_TO_TARGET)
                return null;

            return GetConfiguredTarget();
        }

        private void HandleTargetReached()
        {
            _currentVelocity = Vector3.zero;

            if (!_standWhenTargetIsReached)
                return;

            Stand();
        }

        private void StopAgent()
        {
            if (_navMeshAgent == null || !_navMeshAgent.enabled || !_navMeshAgent.isOnNavMesh)
                return;

            _currentVelocity = Vector3.zero;
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }
    }
}
