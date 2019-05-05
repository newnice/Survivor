using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class CustomJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {
    [SerializeField] private string
        horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input

    [SerializeField]
    private string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

    [SerializeField] private RectTransform pointerRectTransform = null;
    [SerializeField] private float axisSpeed = 100f;
    private Rect _componentRect;
    private Rect _pointerRect;
    private Vector2 _pointerWorldPosition;
    private Vector2 _pointerLocalPosition;

    [SerializeField] [Range(0f, 0.5f)] private float pointerSize = 0.25f;
    [SerializeField] private bool sensitiveToTouch = false;

    [SerializeField]
    private string touchButtonName = "Fire1"; // The name given to the vertical axis for the cross platform input

    private CrossPlatformInputManager.VirtualAxis _horizontalAxis, _verticalAxis;
    private CrossPlatformInputManager.VirtualButton _touchButton;


    private void Start() {
        var componentRectTransform = GetComponent<RectTransform>();
        _pointerWorldPosition = pointerRectTransform.position;
        _pointerLocalPosition = pointerRectTransform.localPosition;

        _componentRect = componentRectTransform.rect;

        var defaultAspectRatio = _componentRect.width / _componentRect.height;
        componentRectTransform.transform.localScale = new Vector3(1f , defaultAspectRatio);

        pointerRectTransform.sizeDelta =
            new Vector2(_componentRect.width * pointerSize, _componentRect.height * pointerSize);
        _pointerRect = pointerRectTransform.rect;
    }

    protected virtual void OnEnable() {
        _horizontalAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
        _verticalAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);

        CrossPlatformInputManager.RegisterVirtualAxis(_horizontalAxis);
        CrossPlatformInputManager.RegisterVirtualAxis(_verticalAxis);


        if (!sensitiveToTouch) return;
        _touchButton = new CrossPlatformInputManager.VirtualButton(touchButtonName);
        CrossPlatformInputManager.RegisterVirtualButton(_touchButton);
    }

    protected virtual void OnDisable() {
        CrossPlatformInputManager.UnRegisterVirtualAxis(horizontalAxisName);
        CrossPlatformInputManager.UnRegisterVirtualAxis(verticalAxisName);
        if (sensitiveToTouch)
            CrossPlatformInputManager.UnRegisterVirtualButton(touchButtonName);
    }

    public virtual void OnDrag(PointerEventData data) {
        var direction = (data.position - _pointerWorldPosition).normalized;
        var newPointerPos = new Vector2(direction.x * (_componentRect.width - _pointerRect.width)/2,
            direction.y * (_componentRect.height - _pointerRect.height)/2);
        pointerRectTransform.localPosition = _pointerLocalPosition + new Vector2(newPointerPos.x, newPointerPos.y);

        _horizontalAxis.Update(direction.x * axisSpeed);
        _verticalAxis.Update(direction.y * axisSpeed);
    }

//OnPointerUp is not working without OnPointerDown(((
    public void OnPointerDown(PointerEventData data) {
        if (sensitiveToTouch) {
            _touchButton.Pressed();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        pointerRectTransform.localPosition = _pointerLocalPosition;
        _horizontalAxis.Update(0f);
        _verticalAxis.Update(0f);
        if (sensitiveToTouch) {
            _touchButton.Released();
        }
    }
}