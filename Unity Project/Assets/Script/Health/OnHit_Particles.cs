using UnityEngine;

public class OnHit_Particles : MonoBehaviour
{
    [SerializeField] private GameObject _hittableObject;
    [SerializeField] private ParticleSystem _particleSystemComponent;

    private void Awake()
    {
        if (_hittableObject == null)
            _hittableObject = gameObject;

        _hittableObject.GetComponent<IHittable>().OnHitted += OnHit_Particles_OnHitted;
    }

    private void OnHit_Particles_OnHitted(int damage)
    {
        _particleSystemComponent.Play();
    }
}
