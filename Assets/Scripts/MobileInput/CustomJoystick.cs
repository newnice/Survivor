using UnityEngine;
using UnityEngine.EventSystems;

public class CustomJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {
    public enum AxisOption {
        // Options for which axes to use
        Both, // Use both
        OnlyHorizontal, // Only horizontal
        OnlyVertical // Only vertical
    }

    [SerializeField] private AxisOption _axesToUse = AxisOption.Both;

    [SerializeField] private string
        horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input

    [SerializeField]
    private string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

    [SerializeField] private RectTransform _pointerRectTransform;
    private Rect _componentRect;
    private Rect _pointerRect;
    private Vector2 _pointerWorldPosition;
    private Vector2 _pointerLocalPosition;


    private void Awake() {
        var componentRect = GetComponent<RectTransform>();
        _pointerWorldPosition = _pointerRectTransform.position;
        _pointerLocalPosition = _pointerRectTransform.localPosition;
        
        _componentRect = componentRect.rect;
        _pointerRectTransform.sizeDelta = new Vector2(_componentRect.width / 4, _componentRect.height / 4);
        _pointerRect = _pointerRectTransform.rect;
    }

    public virtual void OnDrag(PointerEventData data) {
        var direction = (data.position - _pointerWorldPosition).normalized;
      
        var newPos = new Vector2(direction.x * (_componentRect.width-_pointerRect.width )/ 2, direction.y * (_componentRect.height-_pointerRect.height) / 2);

        _pointerRectTransform.localPosition = _pointerLocalPosition + new Vector2(newPos.x, newPos.y);
    }


    public void OnPointerDown(PointerEventData data) {
        //OnPointerUp is not working without OnPointerDown(((
    }

    public void OnPointerUp(PointerEventData eventData) {
        _pointerRectTransform.localPosition = _pointerLocalPosition;
    }
}