using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpController : MonoBehaviour, IPointerClickHandler
{
    public string AnimatorParameterName = "";
    bool showHelpPanel=true;
    Animator animator;

    
    public RectTransform HelpDataPanel;

    RectTransform rectTransform;

    Vector2 originalPos;
    Vector2 hiddenPos;

    Vector2[] waypoints;

    int currentPoint = 0;

    bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

         rectTransform = GetComponent<RectTransform>();
         originalPos = HelpDataPanel.anchoredPosition;
      
        HelpDataPanel.anchoredPosition = new Vector2(-rectTransform.rect.size.x, originalPos.y);

        waypoints = new Vector2[3];
        waypoints[0] = originalPos;

       
        waypoints[1] = new Vector2(-rectTransform.rect.size.x*0.2f, originalPos.y);
        waypoints[2] = originalPos;

        hiddenPos = HelpDataPanel.anchoredPosition;
        HelpDataPanel.anchoredPosition = originalPos;
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int count = eventData.clickCount;

        if (count == 1 && canMove)
        {
            canMove = false;
            showHelpPanel = !showHelpPanel;
            GameController.Instance.AudioCtrl.PlaySound(GameAudioEventType.START_MOVE);
            if (showHelpPanel)
            {
                Debug.Log("Double click");
                currentPoint = 0;

                iTween.ValueTo(gameObject, iTween.Hash(
    "from", HelpDataPanel.anchoredPosition,
    "to", waypoints[currentPoint],
    "time", 0.5f,
    "onupdate", "itweenUpdate", "oncomplete", "itweenComplete"
    )
    );
            }
            else
            {
                iTween.ValueTo(gameObject, iTween.Hash(
    "from", HelpDataPanel.anchoredPosition,
    "to", hiddenPos,
    "time", 0.6f,
    "onupdate", "itweenUpdate", "oncomplete", "hideComplete"
    ));
            }
            //animator.SetBool(AnimatorParameterName, showHelpPanel);
        }
    }


    void itweenUpdate(Vector2 val)
    {
        Debug.Log(val);
        HelpDataPanel.anchoredPosition = val;
    }

    void hideComplete()
    {
        canMove = true;
    }
    void itweenComplete()
    {
        currentPoint++;
        if (currentPoint < waypoints.Length)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
   "from", HelpDataPanel.anchoredPosition,
   "to", waypoints[currentPoint],
   "time", 0.3f,
   "onupdate", "itweenUpdate", "oncomplete", "itweenComplete"
   ));
        }
        else
        {
            canMove = true;
        }
    }
}
