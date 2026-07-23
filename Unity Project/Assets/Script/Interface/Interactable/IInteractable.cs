using System;
using UnityEngine;

namespace Scripts.Interactable
{
    public interface IInteractable
    {
        public event Action<Interactable> OnInteract;

        void Interact();

        GameObject GetObject();
    }
}