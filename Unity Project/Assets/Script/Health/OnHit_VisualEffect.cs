using UnityEngine;
using UnityEngine.VFX;

public class OnHit_VisualEffect : MonoBehaviour
{
    [SerializeField] private GameObject _hittableObject;
    [SerializeField] private VisualEffect _visualEffectComponent;

    private void Awake()
    {
        if (_hittableObject == null)
            _hittableObject = gameObject;

        _hittableObject.GetComponent<IHittable>().OnHitted += OnHit_Particles_OnHitted;
    }

    private void OnHit_Particles_OnHitted(int damage)
    {
        _visualEffectComponent.Play();
    }
}
