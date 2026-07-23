using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.DoAction
{
    public class DoAction_RandomAnimation : DoActionBase
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private string[] _animationsOptions;

        public override void Execute()
        {
            int index = Random.Range(0, _animationsOptions.Length);

            _anim.Play(_animationsOptions[index], 0, 0);
        }
    }
}