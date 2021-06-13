using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject _particleEffect;
    [SerializeField] private int _damage = 1;

    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = SoundManager.GetInstance();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var health = collision.gameObject.GetComponent<Health>();
        health?.TakeDamage(_damage);

        if (_particleEffect != null)
        {
            var effectInstance = Instantiate(_particleEffect);
            effectInstance.transform.position = collision.contacts[0].point;
            Destroy(effectInstance, 5f);
        }

        _soundManager.Play(Constants.IMPACT_SOUND);

        Destroy(gameObject);
    }
}
