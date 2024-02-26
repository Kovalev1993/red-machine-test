using Player;
using Player.ActionHandlers;
using TMPro;
using UnityEngine;
using Utils.Singleton;

namespace Camera
{
    public class CameraMover : DontDestroyMonoBehaviourSingleton<CameraMover>
    {
        [Header("Settings")]
        [SerializeField] private float _movingMultiplier;
        [SerializeField] private float _movingDuration;
        [SerializeField] private float _smoothTime;

        [Header("Components")]
        [SerializeField] private Transform _transform;

        private ClickHandler _clickHandler;

        private float _startTime;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.DragStartEvent += OnDragStart;
            _clickHandler.DragEvent += OnDrag;
            _clickHandler.DragEndEvent += OnDragEnd;

            _targetPosition = transform.position;
        }

        private void OnDestroy()
        {
            _clickHandler.DragStartEvent -= OnDragStart;
            _clickHandler.DragEvent -= OnDrag;
            _clickHandler.DragEndEvent -= OnDragEnd;
        }

        private void OnDragStart(Vector3 startPosition)
        {
            if (!CanBeDraggable())
                return;

            _startTime = Time.time;
        }

        private void OnDrag(Vector3 dragDelta)
        {
            if (!CanBeDraggable())
                return;

            if (dragDelta! != Vector3.zero)
                _startTime = Time.time;
            _targetPosition = _targetPosition + _movingMultiplier * dragDelta;
        }

        private void OnDragEnd(Vector3 endPosition)
        {
            if (!CanBeDraggable())
                return;

            //_targetPosition = transform.position;
        }

        private bool CanBeDraggable()
        {
            return PlayerController.PlayerState == PlayerState.None;
        }

        private void FixedUpdate()
        {
            if (_movingDuration == 0)
            {
                transform.position = _targetPosition;
            }
            else
            {
                //var delta = Mathf.Clamp01((Time.time - _startTime) / _movingDuration);
                //transform.position = Vector3.Lerp(transform.position, _targetPosition, delta);
                var velocity = Vector3.zero;
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref velocity, _smoothTime);
            }
        }
    }
}