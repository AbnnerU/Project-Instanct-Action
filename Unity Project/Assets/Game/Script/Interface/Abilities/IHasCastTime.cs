using System;

namespace Scripts.Abilities
{
    public interface IHasCastTime
    {
        public event Action<float> OnCastTimeStarted;
        public event Action<float> OnCastTimeFinished;

        float CastTime { get; set; }
    }
}
