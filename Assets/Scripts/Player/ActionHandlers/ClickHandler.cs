using System;
using Camera;
using UnityEngine;
using Utils.Singleton;


namespace Player.ActionHandlers
{
    public class ClickHandler : DontDestroyMonoBehaviourSingleton<ClickHandler>
    {
        [SerializeField] private float clickToDragDuration;
        [SerializeField] private int _controlMouseButton = 0; // ����� ���������� �����. � ���� �� ������ ����� ����������� ������ ����������.

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
            // ��������� ������ - ��� ������ �������� ������� ����. ������ ��������� ������� ���� �����.
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
            _previousPointerPosition = _pointerDownPosition;
        }

        private void OnInputButton()
        {
            if (_isDrag)
            {
                var currentPointerPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                currentPointerPosition.z = 0f;
                var pointerDelta = currentPointerPosition - _previousPointerPosition;
                DragEvent?.Invoke(pointerDelta);

                _previousPointerPosition = currentPointerPosition;
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
        ��������� �� SetDragEventHandlers � ClearEvents �� ���� ��������:
        1) �.�. ������ ��������� ������� DragEvent, ������ ���������� � ������� ����� �� �� ���. ��-������, ��� �����, ��-������, ��
            ���� ������������� ClickHandler'� ��������� ��� ����� ����������� � ������� ��� �������. ���������� ������� � ������������� �����,
            �� ������� ����� ����������.
        2) ������� ������� SetDragEventHandlers � ClearEvents �������� ������������ ������������� ClickHandler'�. � ����� ������ ��
            ��� ������ ������������� �� ��� ������� � ������� +=, � � ������ � ������� SetDragEventHandlers. �����, ����� �����
            ������������� ���� ����������.
        */
    }
}