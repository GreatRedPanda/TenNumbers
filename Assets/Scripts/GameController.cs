using System.Collections;
//using System.Array;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
[System.Serializable]
public class ColorPalette
{


    public int number;

    public Color Color;
}
public class GameController : MonoBehaviour
{

    public static GameController Instance;
    public List<ColorPalette> IterationsColors;
    public List<Sprite> IterationsSprites;

    public RectTransform Parent;
    public VisualElement ElementPrefab;
    public RectTransform CircleBasePrefab;
    public RectTransform ElementBasePrefab;
    public RectTransform LinePrefab;

    public RectTransform LostPanel;
    public RectTransform WaitGPSPanel;


    public event System.Action<bool> MovementStart;
    public string SaveKey = "saveGameData2423670";

   
    public TMPro.TextMeshProUGUI Score;
    public TMPro.TextMeshProUGUI HighScore;

    public SwitchingItem BackButton;
    int currentScore = 0;
    public int Colomns = 8;
    public int Rows = 5;
    public int Max=5;
    public int Min = 0;
    ElementData[,] field;
    VisualElement[,] fieldVis;

    float minDragDist;


    Vector2[] angles;
    Vector2 mousePrevPos;
    Vector2 startPos=Vector2.zero;
    int activeRow = -1;

    float prevAngle = 0;
    float prevAngleContinuos = 0;
    int shifting = 0;
    float diff = 0;


    bool lost = false;

   public AudioController AudioCtrl;

    
    int lastNumber = 0;

    int lastRow = -1;
    int lastShift = -1;

    int startMovePos = 0;
    int endMovePos = 0;

    int currentMovePos = 0;

    float angleStart = 0;
    float angleEnd = 0;


    Vector3 pointerPrevPosition;
    float mouseSpeed = 0;
    float time = 0;
    public float PointerSpeedThresh = 0.5f;
    int prevShift = -1;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        GameData.LoadGameData();
       
    }
    void Start()
    {
       
        GameLogic.IterationColors = IterationsColors;
        minDragDist = Screen.width / Colomns;
        field = new ElementData[Rows, Colomns];
        fieldVis = new VisualElement[Rows, Colomns];



    SaveData sd=    SaveController.Load(SaveKey);

        if (sd == null)
        {
            GameLogic.CleverMapGeneration(field, Min, Max);
            lastNumber = Random.Range(Min, Max);
        }
        else
        {

            field = sd.ArrayToMap();
            currentScore = sd.Score;
            lastNumber = sd.LastNumber;
            //lastNumber--;
            //if (lastNumber < Min)
            //    lastNumber = Max;
            
        }
        Score.text = currentScore+ "";
        HighScore.text = GameData.HighScore.ToString();
        GameLogic.GenerateCircle(fieldVis, field, Parent, ElementPrefab, new Vector2(5, 5), out angles);
        GameLogic.GenerateMapVisuals(1, Colomns, WaitGPSPanel.GetChild(0).GetComponent<RectTransform>(), ElementBasePrefab, CircleBasePrefab, new Vector2(5, 5));
        BackButton.Switch(false);

    }


    public void PointerStartMove(Vector3 pointerPos, float radius)
    {
        AudioCtrl.PlaySound(GameAudioEventType.START_MOVE);
        activeRow = GetCircleNumber(radius);
        int angleIndex = 0;
        float angle = 0;

        getCursorPos(pointerPos, out startMovePos, out angle, out prevAngleContinuos);
        prevAngle = angle;
       

        diff = 0;
        shifting = 0;

        pointerPrevPosition = pointerPos;
        time = Time.time;
        prevShift = -1;
    }
    public void PointerMove(Vector3 pointerPos)
    {
  
        if (activeRow != -1)
        {
            Move(activeRow, false, pointerPos);
            bool hasMove = true;

            float angle = 0;
            float angleNotClamped = 0;
            int aN = getCursorPos(pointerPos, out currentMovePos, out angle, out angleNotClamped);
            int s2 = currentMovePos - startMovePos;
           // Debug.Log("BEFORE " + currentMovePos + "    " + startMovePos + "s2 " + s2 + " prevs2 " + prevShift);

            GameLogic.CheckMoves(fieldVis, field, out hasMove, activeRow, Max, Min, -s2);
            Vector3 delta = pointerPos - pointerPrevPosition;
            delta.x /= Screen.width;
            delta.y /= Screen.height;
            float deltaTime = Time.time - time;
            mouseSpeed = delta.sqrMagnitude/deltaTime;

         

            pointerPrevPosition = pointerPos;
            time = Time.time;
            if (float.IsNaN(mouseSpeed))
                mouseSpeed = 0;
            Debug.Log("POINTER SPEED " + mouseSpeed);
            if (hasMove &&  s2!=prevShift && mouseSpeed < PointerSpeedThresh)
                
              AudioCtrl.PlaySound(GameAudioEventType.NUMBER_CHECKED);

            prevShift = s2;
        }

    }
    public void PointerEndMove(Vector3 pointerPos)
    {
        AudioCtrl.PlaySound(GameAudioEventType.END_MOVE);
        if (activeRow != -1)
        {

            int fcCount = 0;
            List<int> numbersSteps;
            float angle = 0;
            float angleNotClamped = 0;
            int aN=  getCursorPos(pointerPos, out endMovePos, out angle, out angleNotClamped);
            int s2 = endMovePos - startMovePos;

            string sd = SaveController.GetSaveDataAsJson(field, currentScore, lastNumber);
        
            GameLogic.MoveEnd(fieldVis, field, activeRow, -s2);

            GameLogic.ShiftMap(fieldVis, field, Direction.Up, Max, Min, activeRow, out fcCount, out numbersSteps, ref lastNumber);


            if(numbersSteps.Count>0)
                SaveController.AddStep(sd);
            ScoreCount( fcCount, numbersSteps);
            shifting = 0;

            bool hasMoves = GameLogic.CheckMoves(field, Max, Min);

            MovementStart?.Invoke(false);
            if (!hasMoves)
            {
                Lost();
            }
            if(SaveController.HasSteps)
            BackButton.Switch(true);
        }
    }



    public void Back(string buttonName)
    {
        if (lost)
        {

            LostPanel.gameObject.SetActive(false);
            lost = false;
        }
        int steps = 0;
        SaveData sd = SaveController.BackStep(ref steps);

        if (sd != null)
        {
            field = sd.ArrayToMap();
            for (int i = 0; i < fieldVis.GetLength(0); i++)
            {
                for (int j = 0; j < fieldVis.GetLength(1); j++)
                {
                    fieldVis[i, j].SetText(field[i, j]);
                }
            }
            currentScore = sd.Score;
            Score.text = currentScore.ToString();
            lastNumber = sd.LastNumber;
            //if (im != null)
            //{
            //    im.color = Color.blue;
            //}

        if(steps==0)
                BackButton.Switch(false);
        }
        //else
        //{

        //    //if (im != null)
        //    //{
        //    //    im.color = Color.red;
        //    //}
        //    BackButton.Switch(false);
        //}

    }
    void ScoreCount(int fcCount, List<int> steps)
    {
        
        currentScore += fcCount * 100;
        if (steps != null)
        {
            if (fcCount == 0 && steps.Count != 0)
                AudioCtrl.PlaySound(GameAudioEventType.NUMBER_SUMMED);
            else if(fcCount!=0)
                AudioCtrl.PlaySound(GameAudioEventType.TEN_ITER);
            foreach (var item in steps)
            {
                currentScore += item;

            }
        }
        Score.text = currentScore.ToString();
        SetHighScore(currentScore);
    }

    int GetCircleNumber(float radius)
    {
        int row = -1;
      
      
    
        for (int i = 0; i < GameLogic.Radiuses.Count; i++)
        {
            Debug.Log("RADIUSES" + GameLogic.Radiuses[i].y * GameLogic.Radiuses[i].y);
            if (radius <= GameLogic.Radiuses[i].y* GameLogic.Radiuses[i].y)
            {
               
                row = i;
                break;

            }

        }

        return row;

    }


    int getCursorPos(Vector3 pointerPos, out int curPos, out float angle, out float angleNotClamped)
    {
        curPos = 0;
        int anglaNum = 0;
        Vector2 vector = pointerPos - Parent.transform.position;
         angle = Mathf.Atan2(vector.y, vector.x);

        if (angle < 0)
        {
            angle = Mathf.Abs(angle);
            angle = 2 * Mathf.PI - angle;

        }
       angleNotClamped = angle;
       
        int i = 0;
        foreach (var item in angles)
        {
            //if (angle > item.x && angle < item.y)
            //{
            //    float min = Mathf.Abs(angle - item.x);
            //    float max = Mathf.Abs(angle - item.y);
            //    if (min <= max)
            //        angle = item.x;
            //    else
            //        angle = item.y;
            //    curPos = i;
            //    anglaNum = i;
            //    break;
            //}

            float angleDif = (item.y - item.x) / 2;
            float min = item.x - angleDif;
            float max = item.x + angleDif;
            if (item.x == 0)
            {
               
            }

            if (item.x == angles[angles.Length - 1].x)
            {

            }
            if (angle > min && angle < max)
            {
                angle = item.x;
                curPos = i;
               anglaNum = i;
                break;
            }
            i++;
        }
        if (angle == angles[angles.Length - 1].y)
            angle = 0;

        return anglaNum;
    }
    void Move(int row, bool clamping, Vector3 pointerPos)
    {
        int angleIndex = 0;
        float angle = 0;
        float angle1 = 0;
        getCursorPos(pointerPos, out angleIndex, out angle, out angle1);
  
        if (Mathf.Abs(angle - prevAngle) > 0)
        {
      

            if (activeRow != -1)
            {


                if (angle == 0 && prevAngle == angles[angles.Length - 1].x)
                {

                    shifting--;
                }
                else if (prevAngle == 0 && angle == angles[angles.Length - 1].x)
                {
                    
                    shifting++;

                }
                else
                 if (prevAngle > angle) // сюда добавить переходный пункт в 0 или 2пи
                {
                    
                    shifting++;
                }
                else
                    shifting--;

            }
            prevAngle = angle;
        }

       
        GameLogic.MoveRow(fieldVis, field, activeRow, diff, clamping, shifting);
        diff +=   angle1- prevAngleContinuos;
        prevAngleContinuos = angle1; 

    }


    void Lost()
    {
      
        lost = true;
       // SaveController.ClearSave(SaveKey);
        LostPanel.gameObject.SetActive(true);
        TMPro.TextMeshProUGUI lostTExt = LostPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (lostTExt != null)
            lostTExt.text = $"No more moves \n {currentScore}";
    }

    public void Restart()
    {

        GameLogic.Replay(fieldVis, field, Max, Min, ref lastNumber);

        Debug.Log("START NUMBER" + lastNumber);
        currentScore = 0;
        Score.text = "0";
        lost = false;
        SaveController.ClearSave(SaveKey);

        if (LostPanel.gameObject.activeSelf)
            LostPanel.gameObject.SetActive(false);
    }


    private void OnApplicationPause(bool pause)
    {
        //ДОБАВИТЬ ПРОВЕРКУ НА ПРОИГРЫШ!!!!
        if (pause)
            if(!lost)
                SaveController.Save(SaveKey, field, currentScore, lastNumber);
            else
                SaveController.ClearSave(SaveKey);

    }

    private void OnApplicationQuit()
    {
        //ДОБАВИТЬ ПРОВЕРКУ НА ПРОИГРЫШ!!!!
        if (!lost)
            SaveController.Save(SaveKey, field, currentScore, lastNumber);
        else
            SaveController.ClearSave(SaveKey);
    }






    public void ShowUI()
    {
        ShowLEaderboardWaitPanel();
        FindObjectOfType<GPlayServiceController>().ShowLeaderboardsUI(success => {


            WaitGPSPanel.gameObject.SetActive(false);
        }

        );
    }

    public void ShowLEaderboardWaitPanel()
    {
        WaitGPSPanel.gameObject.SetActive(true);

    }


    public void SetSound(Object sender)
    {
        GameData.SetSound();






        SwitchingItem si = sender as SwitchingItem;

            if (si != null)
            {
                si.Switch(GameData.SoundLevel == 1);
            }
      

    }

    public void SetHighScore(int score)
    {
        if (score > GameData.HighScore)
        {
            GameData.SetHighscore(score);

            HighScore.text = score.ToString();
        } 
    }



    public void ShowSettingsPanel(string panel)
    {
        Transform sp = transform.Find(panel);

        if (sp != null)
        {
            
                sp.gameObject.SetActive(!sp.gameObject.activeSelf);
            
        }

    }


    public void ShowURL(string url)
    {


        Application.OpenURL(url);
    }


    
}







//GraphicRaycaster gr = this.GetComponent<GraphicRaycaster>();
////Create the PointerEventData with null for the EventSystem
//PointerEventData ped = new PointerEventData(null);
////Set required parameters, in this case, mouse position
//ped.position = Input.mousePosition;
////Create list to receive all results
//List<RaycastResult> results = new List<RaycastResult>();
////Raycast it
//gr.Raycast(ped, results);


//foreach (var item in results)
//{
//    RectTransform itemTr = item.gameObject.GetComponent<RectTransform>();

//    if (itemTr != null)
//    {
//        for (int i = 0; i < Rows; i++)
//        {
//            for (int j = 0; j < Colomns; j++)
//            {
//                if (fieldVis[i, j] == itemTr)
//                {
//                    if (i != Rows - 1)
//                    {
//                        activeRow = i;

//                    }
//                    else
//                        activeRow = -1;
//                    break;
//                }
//            }
//        }
//    }
//}





//int getAngle(float angle)
//{
//    int angleNum = 0;
//    int i = 0;
//    foreach (var item in angles)
//    {
//        if (angle > item.x && angle < item.y)
//        {
//            float min = Mathf.Abs(angle - item.x);
//            float max = Mathf.Abs(angle - item.y);
//            if (min <= max)
//                angle = item.x;
//            else
//                angle = item.y;
//            endMovePos = i;
//            angleNum = i;
//            break;
//        }
//        i++;
//    }
//    if (angle == angles[angles.Length - 1].y)
//        angle = 0;
//    return angleNum;
//}