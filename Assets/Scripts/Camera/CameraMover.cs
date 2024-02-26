using Player;
using Player.ActionHandlers;
using UnityEngine;
using Utils.Singleton;

namespace Camera
{
    public class CameraMover : DontDestroyMonoBehaviourSingleton<CameraMover>
    {
        [Header("Settings")]
        [SerializeField] private float _movingMultiplier;

        [Header("Components")]
        [SerializeField] private Transform _transform;

        private ClickHandler _clickHandler;

        private void Awake()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.DragEvent += OnDrag;
        }

        private void OnDestroy()
        {
            _clickHandler.DragEvent -= OnDrag;
        }

        private void OnDrag(Vector3 dragDelta)
        {
            if (PlayerController.PlayerState != PlayerState.None)
                return;

            _transform.Translate(_movingMultiplier * dragDelta);
        }
    }
}