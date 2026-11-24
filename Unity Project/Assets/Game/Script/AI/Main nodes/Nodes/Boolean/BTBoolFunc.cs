using System;
using System.Collections;

namespace Scripts.BT
{
    public class BTBoolFunc : BTnode
    {
        private Func<bool> _func;

        public BTBoolFunc(Func<bool> func)
        {
            _func = func;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_func != null && _func())
                status = BTstatus.SUCCESS;
            else
                status = BTstatus.FAILURE;

            yield break;
        }
    }
}
