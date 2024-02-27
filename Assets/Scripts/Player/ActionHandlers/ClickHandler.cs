using System;
using Camera;
using UnityEngine;
using Utils.Singleton;


namespace Player.ActionHandlers
{
    public class ClickHandler : DontDestroyMonoBehaviourSingleton<ClickHandler>
    {
        [SerializeField] private float clickToDragDuration;
        [SerializeField] private int _controlMouseButton; // Otherwise the magic number. Besides, we can change controls now.

        public event Action<Vector3> PointerDownEvent;
        public event Action<Vector3> ClickEvent;
        public event Action<Vector3> PointerUpEvent;
        public event Action<Vector3> DragStartEvent;
        public event Action<Vector3> DragEvent;
        public event Action<Vector3> DragEndEvent;

        private Vector3 _pointerDownPosition;
        private Vector3 _previousPointerPosition;

        private bool _isClick;
        private bool _isDrag;
        private float _clickHoldDuration;


        private void Update()
        {
            // Taking out a method is just part of clean code practice. Now supporting the project is a little easier.
            if (Input.GetMouseButtonDown(_controlMouseButton))
                OnInputButtonDown();
            else if (Input.GetMouseButton(_controlMouseButton))
                OnInputButton();
            else if (Input.GetMouseButtonUp(_controlMouseButton))
                OnInputButtonUp();
        }

        private void OnInputButtonDown()
        {
            _isClick = true;
            _clickHoldDuration = .0f;

            _pointerDownPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

            PointerDownEvent?.Invoke(_pointerDownPosition);

            _pointerDownPosition = new Vector3(_pointerDownPosition.x, _pointerDownPosition.y, .0f);

            _previousPointerPosition = Input.mousePosition;
        }

        private void OnInputButton()
        {
            if (_isDrag)
            {
                var pointerDelta = Input.mousePosition - _previousPointerPosition;
                DragEvent?.Invoke(pointerDelta);

                _previousPointerPosition = Input.mousePosition;
            }
        }

        private void OnInputButtonUp()
        {
            var pointerUpPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (_isDrag)
            {
                DragEndEvent?.Invoke(pointerUpPosition);

                _isDrag = false;
            }
            else
            {
                ClickEvent?.Invoke(pointerUpPosition);
            }

            PointerUpEvent?.Invoke(pointerUpPosition);

            _isClick = false;
        }

        private void LateUpdate()
        {
            if (!_isClick)
                return;

            _clickHoldDuration += Time.deltaTime;
            if (_clickHoldDuration >= clickToDragDuration)
            {
                DragStartEvent?.Invoke(_pointerDownPosition);

                _isClick = false;
                _isDrag = true;
            }
        }

        /*
        I deleted SetDragEventHandlers and ClearEvents for two reasons:
        1) Now we have the DragEvent event, and the list of parameters for functions would grow to three.
            First of all, this is a lot, and secondly, not all ClickHandler users need or will need all events in the future.
            As a result, we get complex and various cases, which we should avoid.
        2) The existence of the SetDragEventHandlers and ClearEvents functions breaks the consistency of ClickHandler usage.
            In one case we outside the class subscribe to its events with the help of +=, and in the other case with the help of
            SetDragEventHandlers. The same usage everywhere would be better.
        */
    }
}