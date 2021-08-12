using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler,IDragHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle = null;
    public Vector2 input = Vector2.zero;
    private Canvas canvas;

    public Camera _camera;

    private float deadZone = 0;

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Camera.main.WorldToScreenPoint(background.position);//将ui坐标中的background映射到屏幕中的实际坐标
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position)/ (radius * canvas.scaleFactor);//将屏幕中的触点和background的距离映射到ui空间下实际的距离
        HandleInput(input.magnitude, input.normalized, radius, _camera);//对输入进行限制
        handle.anchoredPosition = input * radius; //实时计算handle的位置
    }

    public void HandleInput(float magnitude, Vector2 normalized, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
            {
                input = normalized;
            }
        }
        else
        {
            input = Vector2.zero;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
