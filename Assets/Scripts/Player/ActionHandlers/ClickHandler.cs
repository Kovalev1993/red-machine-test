using System;
using Camera;
using UnityEngine;
using Utils.Singleton;


namespace Player.ActionHandlers
{
    public class ClickHandler : DontDestroyMonoBehaviourSingleton<ClickHandler>
    {
        [SerializeField] private float clickToDragDuration;
        [SerializeField] private int _controlMouseButton = 0; // Иначе магическое число. К тому же теперь имеем возможность менять управление.

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
            // Вынесение метода - это просто практика чистого кода. Теперь поддержка проекта чуть легче.
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
        Избавился от SetDragEventHandlers и ClearEvents по двум причинам:
        1) Т.к. теперь появилось событие DragEvent, список параметров у функций вырос бы до трёх. Во-первых, это много, во-вторых, не
            всем пользователям ClickHandler'а требуется или будут требоваться в будущем все события. Получаются сложные и разнообразные кейсы,
            от которых лучше избавиться.
        2) Наличие функций SetDragEventHandlers и ClearEvents нарушает единообразие использования ClickHandler'а. В одном случае мы
            вне класса подписываемся на его события с помощью +=, а в другом с помощью SetDragEventHandlers. Лучше, чтобы везде
            использование было одинаковым.
        */
    }
}