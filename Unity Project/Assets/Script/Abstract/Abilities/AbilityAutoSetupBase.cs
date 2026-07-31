using UnityEngine;

namespace Scripts.Abilities
{
    public abstract class AbilityAutoSetupBase : MonoBehaviour
    {
        public abstract void Setup(AbilityContext context);
    }
}
