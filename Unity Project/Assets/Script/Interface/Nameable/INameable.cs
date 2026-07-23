using System;

namespace Scripts.Nameable
{
    public interface INameable
    {
        public event Action<string> OnNameChanged;

        string Name { get; }

        public void SetName(string name);
    }
}