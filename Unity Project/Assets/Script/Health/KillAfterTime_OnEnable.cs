using Scripts.PoolManager;
using System.Collections;
using UnityEngine;

public class KillAfterTime_OnEnable : MonoBehaviour
{
    [SerializeField] private float _timer;  
    [SerializeField] private GameObject _hittableReference;
    [Header("Release")]
    [SerializeField] private bool _releaseObject;
    [SerializeField] private float _releaseTime;

    private IHittable _hittable;

    private float _elapsedTime = 0;

    private void OnEnable()
    {
        if (_hittable ==null)
            _hittable = _hittableReference.GetComponent<IHittable>();

        _elapsedTime = 0;

         StartCoroutine(TimerLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator TimerLoop()
    {
        do
        {
            _elapsedTime += Time.deltaTime;
            yield return null;
        }while(_elapsedTime < _timer);

        _hittable.Kill();

        if (_releaseObject)
        {
            _elapsedTime = 0;
            do
            {
                _elapsedTime += Time.deltaTime;
                yield return null;
            } while (_elapsedTime < _timer);
        }

        if (_releaseObject)
            Destroy(gameObject);

        yield break;
    }
}
