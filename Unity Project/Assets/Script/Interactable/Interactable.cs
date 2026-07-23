using System;
using UnityEngine;

namespace Scripts.Interactable
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public event Action<Interactable> OnInteract;

        public GameObject GetObject()
        {
            return gameObject;
        }

        public void Interact() 
        { 
            OnInteract?.Invoke(this);
            Debug.Log($"Interact with {gameObject.name}");
        }   
    }
}