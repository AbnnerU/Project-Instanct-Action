using UnityEngine;

namespace Scripts.DoAction
{
    public class DoAction_DefineActiveState : DoActionBase
    {
        [SerializeField] private bool _autoSearchByTag;
        [SerializeField] private string _targetTag;
        [SerializeField] private GameObject _objectComponent;
        [SerializeField] private bool _setActive;

        private void Awake()
        {
            if (_autoSearchByTag)
                _objectComponent = GameObject.FindGameObjectWithTag(_targetTag);
        }

        public override void Execute()
        {
            _objectComponent.SetActive(_setActive);
        }
    }
}