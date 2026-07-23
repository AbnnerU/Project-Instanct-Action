using System.Collections.Generic;
using UnityEngine;

namespace Scripts.DoAction
{
    public class DoAction_DefineActiveStateOfCanvas : DoActionBase
    {
        [SerializeField] private bool _autoSearchByTag;
        [SerializeField] private string _targetTag;
        [SerializeField] private List<Canvas> _list_CanvasToAffect = new List<Canvas>();
        [SerializeField] private bool _setActive;

        private void Awake()
        {
            if (_autoSearchByTag)
            {
                GameObject[] obj = GameObject.FindGameObjectsWithTag(_targetTag);

                foreach (var o in obj)
                    _list_CanvasToAffect.Add(o.GetComponent<Canvas>());
            }
        }

        public override void Execute()
        {
            for (int i = 0; i < _list_CanvasToAffect.Count; i++)
                _list_CanvasToAffect[i].enabled = _setActive;
        }
    }
}