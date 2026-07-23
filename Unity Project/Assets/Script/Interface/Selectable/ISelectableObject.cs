
using System;

namespace Scripts.Selectables
{
    public interface ISelectableObject
    {
        public event Action<bool> OnSelected;

        public void Select();

        public void Deselect();
    }
}
