using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    private float _speed = 0f;
    private bool _hookFired = false;

    private Rigidbody2D _rigidbody;
    private System.Action<Spaceship> _onHook;
    private System.Action _onHookFail;
    private LineRenderer _lineRenderer;
    private GameObject _origin;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, Vector3.zero);
    }

    private void Update()
    {
        if (_lineRenderer == null || _origin == null)
        {
            return;
        }

        _lineRenderer.SetPosition(0, _origin.transform.position);
        _lineRenderer.SetPosition(1, transform.position + (transform.up * 0.2f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var targetSpaceship = collision.gameObject.GetComponent<Spaceship>();
        if (targetSpaceship != null)
        {
            _onHook?.Invoke(targetSpaceship);
            _rigidbody.simulated = false;
            transform.parent = targetSpaceship.transform;
        }
        else
        {
            _onHookFail?.Invoke();
        }
    }

    public void Fire(
        float speed,
        GameObject origin,
        Spaceship spaceshipOrigin,
        System.Action<Spaceship> onHook,
        System.Action onHookFail)
    {
        _onHook = onHook;
        _onHookFail = onHookFail;
        _origin = origin;
        transform.parent = null;

        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, Vector3.zero);

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
        _rigidbody.simulated = true;
        _rigidbody.angularVelocity = 0f;

        var originRigidbody = spaceshipOrigin.GetComponent<Rigidbody2D>();
        _rigidbody.velocity = originRigidbody.velocity + (Vector2)transform.up * speed;
    }
}
