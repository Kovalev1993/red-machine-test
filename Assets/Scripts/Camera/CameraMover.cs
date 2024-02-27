using Player;
using Player.ActionHandlers;
using UnityEngine;
using Utils.Singleton;

namespace Camera
{
    public class CameraMover : DontDestroyMonoBehaviourSingleton<CameraMover>
    {
        [Header("Settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _smoothness;

        [Header("Components")]
        [SerializeField] private Transform _transform;

        private ClickHandler _clickHandler;
        private Vector3 _targetPosition;
        private float _screenAverageDimension;

        private void Awake()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.DragEvent += OnDrag;

            _targetPosition = transform.position;
            _screenAverageDimension = 0.5f * (Screen.width + Screen.height);
        }

        private void OnDestroy()
        {
            _clickHandler.DragEvent -= OnDrag;
        }

        private void OnDrag(Vector3 dragDelta)
        {
            if (!CanBeDraggable())
                return;

            _targetPosition = _targetPosition + _speed * dragDelta / _screenAverageDimension;
        }

        private bool CanBeDraggable()
        {
            return PlayerController.PlayerState == PlayerState.None;
        }

        private void LateUpdate()
        {
            if (transform.position == _targetPosition)
                return;

            if (_smoothness == 0)
                MoveInstantly();
            else
                MoveSmoothly();
        }

        private void MoveInstantly()
        {
            transform.position = _targetPosition;
        }

        private void MoveSmoothly()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _smoothness);
        }
    }
}