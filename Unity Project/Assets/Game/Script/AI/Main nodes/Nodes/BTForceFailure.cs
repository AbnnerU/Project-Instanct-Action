using System.Collections;

namespace Scripts.BT
{
    public class BTForceFailure : BTnode
    {
        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.FAILURE;

            yield break;    
        }
    }
}