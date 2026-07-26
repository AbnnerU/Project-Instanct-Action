
using System;

namespace Scripts.AI
{
    public interface IHasAIState
    {
        public event Action<AIState> OnAINewState;

        AIState GetAIState();

        void SetAIState(AIState newState);
    }
}
