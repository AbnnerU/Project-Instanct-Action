using System;
using System.Collections;

namespace Scripts.BT
{
    public class BTIsValueGreater : BTnode
    {
        private Func<float> _valueProvider;
        private float _valueToCompare;

        public BTIsValueGreater(Func<float> valueProvider, float valueToCompare)
        {
            _valueProvider = valueProvider;
            _valueToCompare = valueToCompare;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_valueProvider != null && _valueProvider != null && _valueProvider() > _valueToCompare)
                status = BTstatus.SUCCESS;
            else
                status = BTstatus.FAILURE;

            yield break;
        }
    }
}