using System;

namespace Scripts.TierManipulation
{
    public interface IHasTier
    {
        public event Action<Tier, Tier> OnTierUpdate;
        public event Action OnReadyForNextTier; 

        public bool Maximized { get; }
        public bool IsReadyForNextTier { get; } 
        public float TierProgressPercentage { get; }     

        void SetTierValue(Tier newTier, bool triggerEvent);

        Tier GetTier();

        void TryUpgradeTier(); 
    }
}
