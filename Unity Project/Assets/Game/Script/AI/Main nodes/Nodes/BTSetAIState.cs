using Scripts.AI;
using System.Collections;

namespace Scripts.BT
{
    public class BTSetAIState : BTnode
    {
        private IHasAIState _aiState;
        private AIState _stateToSet;

        public BTSetAIState(IHasAIState aiState, AIState stateToSet)
        {
            _aiState = aiState;
            _stateToSet = stateToSet;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_aiState != null)
            {
                _aiState.SetAIState(_stateToSet);
                status = BTstatus.SUCCESS;
            }
            else
                status = BTstatus.FAILURE;

            yield break;
        }
    }
}