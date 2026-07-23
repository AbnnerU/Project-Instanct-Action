using System;

namespace Scripts.TierManipulation.Visual
{
    public interface IReadOnlyEvolutionData
    {
        public event Action OnDataUpdated;

        public string PathID { get; }
        public string TextureId { get; }
        public string PathDisplayName { get; }
        public string TextureDisplayName { get; }
    }
}
