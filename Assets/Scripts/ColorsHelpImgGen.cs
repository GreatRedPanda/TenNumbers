using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorsHelpImgGen : MonoBehaviour
{

    public RectTransform Prefab;
    public GridLayoutGroup gridLayoutGroup;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        float paddingX = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;

        float paddingY = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;
        float cellSizeX = (rectTransform.rect.width - paddingX) / GameController.Instance.IterationsSprites.Count;
        float cellSizeY = (rectTransform.rect.height - paddingY) / 1;

        float cellSize = (cellSizeX < cellSizeY) ? cellSizeX : cellSizeY;
        gridLayoutGroup.cellSize = Vector2.one * cellSize;
        for (int i = 0; i < GameController.Instance.IterationsSprites.Count; i++)
        {


            RectTransform gemIcon = Instantiate(Prefab);




            gemIcon.SetParent(transform);
            gemIcon.localScale = Vector3.one;

            gemIcon.GetComponent<Image>().sprite = GameController.Instance.IterationsSprites[i];
        }
    }

   
}
