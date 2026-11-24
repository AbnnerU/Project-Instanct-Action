using UnityEngine;

namespace Scripts.Abilities.Extras
{
    public class AbilityParticleEffects : MonoBehaviour
    {
        [SerializeField] private AbilityBase _abilityComponent;
        [SerializeField] private ParticleSystem[] _particlesSystems;

        private void Awake()
        {
            _abilityComponent.OnAbilityStarted += Play_Particles;
        }

        private void Play_Particles(AbilityBase @base)
        {
            for (int i = 0; i < _particlesSystems.Length; i++)
                _particlesSystems[i].Play();
        }
    }
}
