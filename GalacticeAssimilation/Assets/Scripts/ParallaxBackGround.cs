using UnityEngine;
using System.Collections;

/// <summary>
/// Source: https://www.youtube.com/watch?v=wBol2xzxCOU
/// </summary>
public class ParallaxBackGround : MonoBehaviour
{
    [SerializeField] private float _effectMultiplier = 0.5f;

    private Camera _camera;
    private Vector3 _lastCameraPosition;
    private float _textureUnitSizeX;
    private float _textureUnitSizeY;

    private void Start()
    {
        _camera = Camera.main;
        _lastCameraPosition = _camera.transform.position;
        var sprite = GetComponent<SpriteRenderer>().sprite;
        var texture = sprite.texture;
        _textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        _textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    private void Update()
    {
        var deltaMovement = _camera.transform.position - _lastCameraPosition;
        transform.position += deltaMovement * _effectMultiplier;
        _lastCameraPosition = _camera.transform.position;

        if (Mathf.Abs(_camera.transform.position.x - transform.position.x) >= _textureUnitSizeX)
        {
            var offsetPositionX = (_camera.transform.position.x - transform.position.x) % _textureUnitSizeX;
            transform.position = new Vector3(_camera.transform.position.x + offsetPositionX, transform.position.y);
        }

        if (Mathf.Abs(_camera.transform.position.y - transform.position.y) >= _textureUnitSizeY)
        {
            var offsetPositionY = (_camera.transform.position.y - transform.position.y) % _textureUnitSizeY;
            transform.position = new Vector3(transform.position.x, _camera.transform.position.y + offsetPositionY);
        }
    }
}
