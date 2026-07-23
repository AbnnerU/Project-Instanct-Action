using UnityEngine;

namespace Scripts.DoAction
{
    public class DoAction_AnimSetBool : DoActionBase
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private string _name;
        [SerializeField] private bool _value;

        public override void Execute()
        {
            _anim.SetBool(_name, _value);
        }
    }
}