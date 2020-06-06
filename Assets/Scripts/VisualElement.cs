using UnityEngine;
using System.Collections;
using TMPro;
using System;
using UnityEngine.UI;

public class VisualElement: MonoBehaviour
{
    public int PrevMatch { get { return mCurrentPlace; } }
    public RectTransform RectTransf;

    public Vector2 Size;

     int mCurrentPlace=-1;

    int lastChecked = -1;
    TextMeshProUGUI text;
    Image image;

    int _side = 1;

    private void Start()
    {
        GameController.Instance.MovementStart += StartMovement;
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponentInChildren<Image>();

    }

    public void StartMovement(bool start)
    {
        if (!start)
            MakeOverlayEffect(start, -1);
        lastChecked = -1;

        //Debug.Log("MOVEMENT END> NEED CLAMP");


        //NEED clamping angle and scale for numbers
        // SetRotation( angle);
    }


    public void SwitchSide(int side)
    {
        _side = side;
      
        image.transform.localScale = new Vector3(_side, 1, 1);
        text.transform.localScale = new Vector3(_side, 1, 1);


        //Debug.Log( this.GetInstanceID()+"   "+  _side+"   " + image.transform.localScale+ "   "+ text.transform.localScale);
    }
    public void MakeOverlayEffect(bool effect, int currentPlace=0, float sizeCoef=1.5f)
    {
        mCurrentPlace = currentPlace;
       // Debug.Log(lastChecked != mCurrentPlace+"CALLED☺ ☻♦" + lastChecked + "  " + mCurrentPlace);
        //Debug.Log((lastChecked != mCurrentPlace)+"CALLED☺ ☻♦" + lastChecked + "  " + mCurrentPlace);
        if (effect)
        {
           
            if (lastChecked != mCurrentPlace)
            {
                RectTransf.sizeDelta = sizeCoef * Size;
                lastChecked = currentPlace;
            }
        }
        else
        {
            RectTransf.sizeDelta = Size;
            lastChecked = -1;
            //mCurrentPlace = -1;
        }
    }
    public void SetText(ElementData element)
    {
    if(text==null)
            text = GetComponentInChildren<TextMeshProUGUI>();
    if(image==null)
        image = GetComponentInChildren<Image>();
        if (text != null)
        {

            if (element.CurrentNumber != -1)
                text.text = element.CurrentNumber.ToString();
            else
                text.text = "";




           // text.color = GameLogic.IterationColors[element.IterationsCount].Color;
        }
       // Debug.Log(element.IterationsCount +"   " + GameController.Instance.IterationsSprites[element.IterationsCount].name);

        if (image != null)
            image.sprite = GameController.Instance.IterationsSprites[element.IterationsCount];

        SwitchSide(element.Side);

        float angle = RectTransf.localRotation.eulerAngles.z;
        SetRotation(Mathf.Deg2Rad*(angle));
    }

    internal void PlayNewElementAnimation()
    {


        iTween.PunchScale(gameObject, iTween.Hash("y", 2, "x",2, "time", 1));
    }

    public void SetRotation(float angle)
    {

        float angleInDegree = Mathf.Rad2Deg*(angle);
        if (image == null)
            image = GetComponentInChildren<Image>();
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();


        RectTransf.localRotation = Quaternion.Euler(0, 0, angleInDegree);
        if (text != null)
            text.rectTransform.localRotation= Quaternion.Euler(0, 0, -1* _side*angleInDegree);


    }
}

