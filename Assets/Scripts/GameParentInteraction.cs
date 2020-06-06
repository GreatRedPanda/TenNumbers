using System.Collections;
//using System.Array;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameParentInteraction : MonoBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform ParentCanvas;

    public Vector2 coef;
    public Vector2 center;


    public event System.Action<Vector3> PointerMove;
    public event System.Action<Vector3> PointerEndMove;
    public event System.Action<Vector3, float> PointerStartMove;
    // Use this for initialization
    void Start()
    {

        PointerMove += GameController.Instance.PointerMove;
        PointerEndMove += GameController.Instance.PointerEndMove;
        PointerStartMove += GameController.Instance.PointerStartMove;

        Debug.Log(GetComponent<RectTransform>().rect.size);

        center= transform.position;
        coef = new Vector2(ParentCanvas.rect.size.x / Screen.width,
            ParentCanvas.rect.size.y / Screen.height);


        //Debug.Log(coef+"cCOEF");
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        
        //Debug.Log(eventData.position + "POINTER POSITIOB");
        Vector2 coords = eventData.position;
        float r = Mathf.Pow((coords.x - center.x) , 2) + Mathf.Pow((coords.y - center.y), 2);

        //Debug.Log("COORDS" + coords + "   " + center + " r " + r);


        float r2 = Mathf.Pow((coords.x - center.x)*coef.x, 2) + Mathf.Pow((coords.y - center.y)*coef.y, 2);

        //Debug.Log("COORDS" + coords + "   " + center + " r " + r2);

        PointerStartMove?.Invoke(coords, r2);

    }

    public void OnDrag(PointerEventData eventData)
    {
        PointerMove?.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PointerEndMove?.Invoke(eventData.position);
    }
}
