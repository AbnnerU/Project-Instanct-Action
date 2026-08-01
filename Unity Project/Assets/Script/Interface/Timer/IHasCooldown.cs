using System;

namespace Scripts.Timer
{
    public interface IHasCooldown 
    {
        public event Action<float> OnCooldownStarted;
        public event Action<float> OnCooldownCompleted;

        public float Cooldown { get; set; }

        public float CooldownProgression { get; }
    }
}