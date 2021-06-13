using UnityEngine;
using System.Collections;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _pullSpeed = 1f;
    [SerializeField] private float _reattachmentDistance = 1f;
    [SerializeField] private float _hookTimeout = 5f;
    [SerializeField] private float _hookAttachTimeout = 7f;
    [SerializeField] private GameObject _hookSprite;
    [SerializeField] private Hook _hookPrefab;

    private DistanceJoint2D _joint;
    private Hook _hook;
    private bool _hookDischarged = false;
    private bool _hookAttached = false;
    private float _timeSinceHookDischarged = 0f;
    private float _timeSinceHookAttached = 0f;
    private Spaceship _spaceship;
    private System.Action<Spaceship> _onPullSuccess;

    private void Start()
    {
        _hookSprite.SetActive(true);
        _hook = Instantiate(_hookPrefab);
        _hook.gameObject.SetActive(false);
        _joint = GetComponent<DistanceJoint2D>();
        _joint.enabled = false;
        _spaceship = GetComponent<Spaceship>();
    }

    private void Update()
    {
        if (_hookAttached)
        {
            _timeSinceHookDischarged = 0f;

            PullHook();
            _timeSinceHookAttached += Time.deltaTime;
            if (_timeSinceHookAttached > _hookAttachTimeout)
            {
                ResetHook();
            }
        }
        else if (_hookDischarged)
        {
            _timeSinceHookDischarged += Time.deltaTime;
            if (_timeSinceHookDischarged > _hookTimeout)
            {
                ResetHook();
            }
        }
    }

    public void ShootHook(System.Action<Spaceship> onPullSuccess)
    {
        if (_hookDischarged)
        {
            return;
        }

        _onPullSuccess = onPullSuccess;
        _hookDischarged = true;
        _hook.gameObject.SetActive(true);

        _hook.transform.position = _hookSprite.transform.position;
        _hook.transform.rotation = _hookSprite.transform.rotation;

        _hookSprite.SetActive(false);

        _hook.Fire(_speed, _hookSprite, _spaceship,
            (targetSpaceship) => //Hook succeeded.
            {
                _joint.enabled = true;
                _joint.connectedBody = targetSpaceship.GetComponent<Rigidbody2D>();
                _hookAttached = true;
            },
            () => //Hook failed.
            {
                StartCoroutine(DelayResetHook());
            });
    }

    private void PullHook()
    {
        if (_joint.connectedBody == null)
        {
            ResetHook();
            return;
        }

        _joint.distance -= _pullSpeed * Time.deltaTime;

        var distanceToConnectedBody = Vector2.Distance(_joint.connectedBody.transform.position, transform.position);
        if (_joint.distance > distanceToConnectedBody)
        {
            _joint.distance = distanceToConnectedBody;
        }

        if (Vector2.Distance(_joint.connectedBody.transform.position, transform.position) < _reattachmentDistance)
        {
            ResetHook();
            _onPullSuccess?.Invoke(_joint.connectedBody.GetComponent<Spaceship>());
        }
    }

    private IEnumerator DelayResetHook()
    {
        yield return new WaitForSeconds(1f);
        ResetHook();
    }

    private void ResetHook()
    {
        _hookDischarged = false;
        _joint.enabled = false;
        _hookAttached = false;
        _hookSprite.SetActive(true);
        _hook.gameObject.SetActive(false);
        _timeSinceHookDischarged = 0f;
        _timeSinceHookAttached = 0f;
    }
}
