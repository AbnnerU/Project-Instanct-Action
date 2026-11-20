using UnityEngine;

namespace Parallax
{
    [DefaultExecutionOrder(1000)]
    public class ParallaxLayer : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private Transform _cameraTransform;
        [Tooltip("Camera movement followed by this layer. 0 = world-fixed; 100 = follows the camera.")]
        [SerializeField, Range(0f, 100f)] private float _horizontalFollowPercent = 80f;
        [SerializeField, Range(0f, 100f)] private float _verticalFollowPercent;

        [Header("Horizontal Loop")]
        [Tooltip("Use equal-width, unrotated SpriteRenderers directly parented to this layer.")]
        [SerializeField] private bool _loopHorizontal;
        [SerializeField] private SpriteRenderer[] _tiles;

        private Vector3 _previousCameraPosition;
        private float _tileWidth;
        private bool _isInitialized;

        private void OnEnable()
        {
            _isInitialized = false;
        }

        private void LateUpdate()
        {
            if (!_isInitialized)
            {
                if (!TryInitialize()) return;
            }

            if (_cameraTransform == null) return;

            Vector3 cameraPosition = _cameraTransform.position;
            Vector3 delta = cameraPosition - _previousCameraPosition;
            transform.position += new Vector3(delta.x * _horizontalFollowPercent / 100f,
                delta.y * _verticalFollowPercent / 100f, 0f);
            _previousCameraPosition = cameraPosition;

            if (_loopHorizontal) WrapTiles(cameraPosition.x);
        }

        private bool TryInitialize()
        {
            if (_cameraTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null) _cameraTransform = mainCamera.transform;
            }

            if (_cameraTransform == null)
                return DisableWithWarning("Assign a camera transform or tag a camera MainCamera.");

            if (_loopHorizontal && !TryInitializeTiles()) return false;

            _previousCameraPosition = _cameraTransform.position;
            _isInitialized = true;
            return true;
        }

        private bool TryInitializeTiles()
        {
            if (_tiles == null || _tiles.Length < 3)
                return DisableWithWarning("Horizontal looping requires at least three tiles.");

            for (int i = 0; i < _tiles.Length; i++)
            {
                var tile = _tiles[i];
                if (tile == null || tile.sprite == null || tile.transform.parent != transform ||
                    Quaternion.Angle(tile.transform.rotation, Quaternion.identity) > 0.01f)
                    return DisableWithWarning("Tiles must have sprites, be direct children and have no world rotation.");

                for (int j = 0; j < i; j++)
                    if (_tiles[j] == tile) return DisableWithWarning("Each tile must be a different renderer.");

                float width = tile.bounds.size.x;
                if (i == 0) _tileWidth = width;
                if (width <= 0f || Mathf.Abs(width - _tileWidth) > 0.01f)
                    return DisableWithWarning("All tiles must have the same positive world-space width.");
            }

            // Arrange copies around the first tile, preserving each tile's height and depth.
            float center = _tiles[0].bounds.center.x;
            for (int i = 0; i < _tiles.Length; i++)
            {
                float target = center + (i - (_tiles.Length - 1) * 0.5f) * _tileWidth;
                _tiles[i].transform.position += Vector3.right * (target - _tiles[i].bounds.center.x);
            }
            return true;
        }

        private void WrapTiles(float cameraX)
        {
            float span = _tileWidth * _tiles.Length;
            foreach (var tile in _tiles)
            {
                if (tile == null) continue;
                float offset = tile.bounds.center.x - cameraX;
                float cycles = Mathf.Floor((offset + span * 0.5f) / span);
                tile.transform.position -= Vector3.right * (cycles * span);
            }
        }

        private bool DisableWithWarning(string message)
        {
            Debug.LogWarning($"[ParallaxLayer] {message}", this);
            enabled = false;
            return false;
        }
    }
}
