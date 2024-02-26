using Player.ActionHandlers;
using UnityEngine;
using Utils.Singleton;

namespace Camera
{
    public class CameraMover : DontDestroyMonoBehaviourSingleton<CameraMover>
    {
        private ClickHandler _clickHandler;

        private void Awake()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.DragEvent += OnDrag;
        }

        private void OnDrag(Vector3 dragDelta)
        {
            Debug.Log(dragDelta);
        }
    }
}