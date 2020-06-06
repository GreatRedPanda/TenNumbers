using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
public static class GameLogic 
{

    public static List<ColorPalette> IterationColors = new List<ColorPalette>();
    public static List<Vector2> Radiuses = new List<Vector2>();
    static Vector2[] radiuses;
    static Vector2[] sizes;
    static Vector2 parentSize;
    static Vector2[] anglesOfRows;



    public static void CleverMapGeneration(ElementData[,] map, int min, int max, int requiredCount=2)
    {
        List<int> baseGen = new List<int>();

        for (int j = 0; j < map.GetLength(1); j++)
        {
            baseGen.Add(j);
            map[0, j] = new ElementData() { CurrentNumber = Random.Range(min, max) };
        }

        for (int i = 1; i < map.GetLength(0); i++)
        {
            List<int> places = new List<int>(baseGen);
            List<int> placesTaken = new List<int>();
            int checkingNumber = Random.Range(1, map.GetLength(1));
            for (int k = 0; k < requiredCount; k++)
            {
                int randomIndex = Random.Range(0, places.Count);
                int place = places[randomIndex];
                places.RemoveAt(randomIndex);
                placesTaken.Add(place);


                int prevRowIndex = place + checkingNumber;
                if (prevRowIndex >= map.GetLength(1))
                {
                    prevRowIndex = place + checkingNumber - map.GetLength(1);
                }
                map[i, place] = new ElementData() { CurrentNumber = map[i - 1, prevRowIndex].CurrentNumber - 1 };
                if (map[i - 1, prevRowIndex].CurrentNumber == min)
                {
                    map[i, place].CurrentNumber = max - 1;
                }
               
            }

            if (i == map.GetLength(0) - 1)
            {
                List<int> places2 = new List<int>();
                for (int k = 0; k < baseGen.Count; k++)
                {
                    if (!placesTaken.Contains(k))
                        places2.Add(k);
                }
                for (int k = 0; k < requiredCount; k++)
                {
                    int randomIndex = Random.Range(0, places.Count);
                    int place = places2[randomIndex];
                    places2.RemoveAt(randomIndex);
                    placesTaken.Add(place);


                    int prevRowIndex = place + checkingNumber;
                    if (prevRowIndex >= map.GetLength(1))
                    {
                        prevRowIndex = place + checkingNumber - map.GetLength(1);
                    }
                    map[i, place] = new ElementData() { CurrentNumber = map[0, prevRowIndex].CurrentNumber + 1 };
                    map[i, place].CurrentNumber %=10;
                    

                }


            }

            for (int j = 0; j < map.GetLength(1); j++)
            {
                if(!placesTaken.Contains(j))
                    map[i, j] = new ElementData() { CurrentNumber = Random.Range(min, max) };
            }
        }


    }





    //int stepGen= Random.Range(1,4);
    //int checkingNumber = Random.Range(1, map.GetLength(1));
    //int border = requiredCount;
    //for (int j = 0; j < map.GetLength(1); j++)
    //{
    //    int stepGen2= Random.Range(1, 4);
    //    if ( stepGen==stepGen2 && border > 0)
    //    {

    //        int prevRowIndex = j + checkingNumber;
    //        if (prevRowIndex >= map.GetLength(1))
    //        {
    //            prevRowIndex=  j + checkingNumber- map.GetLength(1);
    //        }
    //        map[i, j] = new ElementData() { CurrentNumber = map[i - 1, prevRowIndex].CurrentNumber - 1 };
    //        if (map[i - 1, prevRowIndex].CurrentNumber == min)
    //        {
    //            map[i, j].CurrentNumber = max - 1;
    //        }
    //        Debug.Log("GENERATED NUMBER"+i+"   "+ map[i, j].CurrentNumber+"     "+ map[i - 1, prevRowIndex].CurrentNumber);
    //        border--;
    //    }
    //    else
    //    map[i, j] = new ElementData() { CurrentNumber = Random.Range(min, max) };


    //}
    public static void GenerateCircle(VisualElement[,] mapVis, ElementData[,] map, RectTransform parent, VisualElement prefab, Vector2 gapPercent, out Vector2[] angles)
    {
   
        angles = new Vector2[map.GetLength(1) ];
        Vector2 size = parent.rect.size;
        parentSize = size;
        //Debug.Log("SIZES" + size+"  "+parent.sizeDelta+"   "+Camera.main.WorldToScreenPoint(size));
        Vector2 gap = new Vector2(gapPercent.x * size.x, gapPercent.y * size.y) / 100;
        size.x -= gap.x * 2;
        size.y -= gap.y * 2;

       
        radiuses = new Vector2[map.GetLength(0)];
         sizes = new Vector2[map.GetLength(0)];
        float deltaAngle = (2*Mathf.PI / ((map.GetLength(1))));

        float radiusX = size.x / 2;
        float radiusY = size.y / 2;

        float minRadius= (radiusX < radiusY) ? radiusX : radiusY;
        float radiusRow = minRadius / (map.GetLength(0));

        for (int i = 0; i < radiuses.Length; i++)
        {

            float c = Mathf.PI * 2 * radiusRow * (i + 1);

            float cz = c / (2*Mathf.PI * (map.GetLength(1)+2));
            cz = radiusRow*2  / map.GetLength(1);
            sizes[i] = new Vector2(cz/size.x, cz/size.y);

            float gip = Mathf.Sqrt((cz*cz) + (cz*cz));
            radiuses[i] = new Vector2(radiusRow * (i + 1), radiusRow * (i + 1));
            Radiuses.Add(new Vector2(radiusRow * (i), radiusRow * (i + 1)+gip));
           // Debug.Log(Radiuses[i]);
        }


        Vector2 startPos = Vector2.one*0.5f;
      
        float t = 0;
        for (int j = 0; j < map.GetLength(1); j++)
        {

            angles[j] = new Vector2(t, t + deltaAngle);
            //Debug.Log(" ANGLES" + angles[j]);
            t += deltaAngle;

        }

        for (int i = 0; i < map.GetLength(0); i++)
        {
            t = 0;
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int i1 = i; int j1 = j;
                mapVis[i1, j1] = (VisualElement)GameObject.Instantiate(prefab);

            
                //SetText(mapVis[i, j], map[i, j]);

                mapVis[i1, j1].transform.SetParent(parent);

                mapVis[i1, j1].transform.localScale = Vector3.one;
              
                Vector2 posMin = new Vector2(Mathf.Cos(t) * radiuses[i].x/size.x, Mathf.Sin(t ) * radiuses[i].y/size.y) ;
                posMin = new Vector2(Mathf.Cos(t) * radiuses[i].x , Mathf.Sin(t) * radiuses[i].y );

                mapVis[i1, j1].transform.localPosition = posMin;
                mapVis[i1, j1].RectTransf.sizeDelta = new Vector2(sizes[i].x*size.x, sizes[i].y * size.y )*2;
                mapVis[i1, j1].Size = new Vector2(sizes[i].x * size.x, sizes[i].y * size.y) * 2;
                mapVis[i1, j1].SetText(map[i1, j1]);
                mapVis[i1, j1].SetRotation(t);
               t += deltaAngle;

            }
        }

        anglesOfRows = angles;

    }

    public static void GenerateMapVisuals(int rows, int colomns, RectTransform parent, RectTransform elementBaseprefab, RectTransform circleBasePref, Vector2 gapPercent)
    {
        Vector2 size = parent.rect.size;
        //Debug.Log("SIZES" + size + "  " + parent.sizeDelta + "   " + Camera.main.WorldToScreenPoint(size));
        Vector2 gap = new Vector2(gapPercent.x * size.x, gapPercent.y * size.y) / 100;
        size.x -= gap.x * 2;
        size.y -= gap.y * 2;
       // sizes = new Vector2[rows];
        float deltaAngle = (2 * Mathf.PI / (colomns));

        float radiusX = size.x / 2;
        float radiusY = size.y / 2;

        float minRadius = (radiusX < radiusY) ? radiusX : radiusY;
        float radiusRow = minRadius /rows;

        //for (int i = 0; i < radiuses.Length; i++)
        //{

        //    float c = Mathf.PI * 2 * radiusRow * (i + 1);

        //    float cz = c / (2 * Mathf.PI * (colomns + 2));
        //    cz = radiusRow * 2 / colomns;
        //  //  sizes[i] = new Vector2(cz / size.x, cz / size.y);
        //    radiuses[i] = new Vector2(radiusRow * (i + 1), radiusRow * (i + 1));


        //    //Debug.Log(Radiuses[i]);
        //}

        //for (int i = 0; i <rows; i++)
        //{
        //    RectTransform circle = GameObject.Instantiate(circleBasePref);

        //    circle.SetParent(parent);

        //    circle.localScale = Vector3.one;



        //    circle.transform.localPosition = Vector3.zero;
        //    circle.sizeDelta = new Vector2(radiuses[i].x, radiuses[i].y) * 2;
        //    circle.SetAsFirstSibling();
        //}

        float t = 0;

        for (int i = 0; i < rows; i++)
        {
            t = 0;
            for (int j = 0; j < colomns; j++)
            {


                RectTransform eb = GameObject.Instantiate(elementBaseprefab);

                eb.SetParent(parent);
                eb.SetAsFirstSibling();
                eb.localScale = Vector3.one;

                Vector2 posMin = new Vector2(Mathf.Cos(t) * radiuses[i].x / size.x, Mathf.Sin(t) * radiuses[i].y / size.y);
                posMin = new Vector2(Mathf.Cos(t) * radiuses[i].x, Mathf.Sin(t) * radiuses[i].y);
                eb.transform.localPosition = posMin;
                eb.sizeDelta = new Vector2(sizes[i].x * size.x, sizes[i].y * size.y) * 2;
                float angleInDegree = Mathf.Rad2Deg * (t);
                eb.localRotation = Quaternion.Euler(0, 0, angleInDegree);
                t += deltaAngle;

            }
        }


    }




    public static void ShiftMap(VisualElement[,] mapVis, ElementData[,] map, Direction dir, int max, int min, int activeRow, out int fullCycleCounts, out List<int> stepNumbers, ref int lastNumber)
    {
        stepNumbers = new List<int>();
        fullCycleCounts = 0;
        int nextRow = activeRow + 1;
        if (activeRow == -1)
            return;

        for (int j = 0; j < map.GetLength(1); j++)
        {
            int newSide = 1;

            if (map[activeRow, j].Side == 1)
            {

                nextRow = activeRow + 1;
                if (nextRow == map.GetLength(0))
                    nextRow = 0;
                newSide = -1;
            }
            else
            {

                nextRow = activeRow - 1;
                if (nextRow == -1)
                    nextRow = map.GetLength(0) - 1;
                newSide = 1;
            }

            if (((map[activeRow, j].CurrentNumber - 1 == map[nextRow, j].CurrentNumber)
              || (map[activeRow, j].CurrentNumber == min && map[nextRow, j].CurrentNumber == max - 1))
              && map[activeRow, j].Side == map[nextRow, j].Side

              )
            {

                int cAcNum = map[activeRow, j].CurrentNumber;
                int cNxNum = map[nextRow, j].CurrentNumber;
                int newAcNum = (cAcNum + cNxNum) % 10;

                int newNxNum = cAcNum;
                map[activeRow, j].CurrentNumber+=1;

                map[nextRow, j].CurrentNumber--;
              //  map[activeRow, j].CurrentNumber %=10;

               // map[nextRow, j].CurrentNumber %=10;

                if (map[activeRow, j].CurrentNumber >= max)
                    map[activeRow, j].CurrentNumber = min;
                if (map[nextRow, j].CurrentNumber < min)
                    map[nextRow, j].CurrentNumber = max - 1;
                map[activeRow, j].IterationsCount++;

                if (map[activeRow, j].IterationsCount == max)
                {
                    fullCycleCounts++;
                    map[activeRow, j].IterationsCount = 0;
                }
                //else
                {
                    stepNumbers.Add(map[activeRow, j].CurrentNumber);
                }
                map[activeRow, j].Side = newSide;

                map[nextRow, j].Side = newSide;

                mapVis[activeRow, j].SetText(map[activeRow, j]);
                mapVis[nextRow, j].SetText(map[nextRow, j]);
                mapVis[activeRow, j].PlayNewElementAnimation();

                mapVis[nextRow, j].PlayNewElementAnimation();
            }
        }


    }



    public static int GetNextNumber(int min, int max, int lasNumber)

    {
        int number = lasNumber;
        number+=3;

        if (number >= max)
            number = number - max;
        return number;
    }
    public static int GenerateNumber(int min, int max, ElementData[,] map)

    {
        int number = 0;
        if (map == null)
            number = Random.Range(min, max);
        else
        {

            List<int> countOfNumbersInMap = new List<int>();
            List<int> generationBase = new List<int>();

            for (int i = min; i < max; i++)
            {
                countOfNumbersInMap.Add(0);
            }
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                  
                    countOfNumbersInMap[map[i,j].CurrentNumber]++;
                }
            }
            for (int i = 0; i < countOfNumbersInMap.Count; i++)
            {

                int countOfGenNumber =max- countOfNumbersInMap[i];
                generationBase.Add(i);
                Debug.Log(i +"    "+ countOfNumbersInMap[i]);
                if (countOfNumbersInMap[i]<1)
                for (int j = 0; j < 4; j++)
                {
                    Debug.Log(i);
                    generationBase.Add(i);
                }
            }


            int rIndex = Random.Range(0, generationBase.Count);
            number = generationBase[rIndex];

        }
        Debug.Log("NEW NUMBER "+number);
        return number;
    }
    public static void MoveRow(VisualElement[,] mapVis, ElementData[,] map, int row,  float angleDelta, bool clamping=true, int shift=0)
    {


       
            for (int j = 0; j < map.GetLength(1); j++)
            {
            int newIndex = j;
               Vector3 posMin = new Vector3(Mathf.Cos(anglesOfRows[j].x+angleDelta) * radiuses[row].x, Mathf.Sin(anglesOfRows[j].x + angleDelta) * radiuses[row].y,0);
                if (clamping)
                {

                    float angle = anglesOfRows[j].x + angleDelta;
                    int rounds = (int)(angle) / (int)(2 * Mathf.PI);


                    float angleAcos = angle - rounds * 2 * Mathf.PI;

                    //ы Debug.Log(angleAcos + "  angle  " + (anglesOfRows[j].x + angleDelta));
                    for (int i = 0; i < anglesOfRows.Length; i++)
                    {


                        if (angleAcos > anglesOfRows[i].x && angleAcos < anglesOfRows[i].y)
                        {
                            float min = Mathf.Abs(angleAcos - anglesOfRows[i].x);
                            float max = Mathf.Abs(angleAcos - anglesOfRows[i].y);
                        float res = min;
                            if (min <= max)
                            {
                                posMin = new Vector3(Mathf.Cos(anglesOfRows[i].x) * radiuses[row].x, Mathf.Sin(anglesOfRows[i].x) * radiuses[row].y, 0);
                            res = anglesOfRows[i].x;
                            }
                            else
                            {
                                posMin = new Vector3(Mathf.Cos(anglesOfRows[i].y) * radiuses[row].x, Mathf.Sin(anglesOfRows[i].y) * radiuses[row].y, 0);
                            res = anglesOfRows[i].y;

                        }
                            mapVis[row, j].transform.localPosition = posMin;
                            mapVis[row, j].SetRotation(res);
                        break;
                        }
                    }

                }

                else
                {
                    mapVis[row, j].transform.localPosition = posMin;
                mapVis[row,j].SetRotation(anglesOfRows[j].x + angleDelta);
                }
            }



    }



    public static void MoveEnd(VisualElement[,] mapVis, ElementData[,] map, int row,  int shift = 0)
    {
        int dir = (int)Mathf.Sign(shift);
        shift = Mathf.Abs(shift);
        int shiftAmount = shift - (shift / mapVis.GetLength(1))* mapVis.GetLength(1);
        //Debug.Log("SHIFT AMOUNT"+shiftAmount);

        VisualElement[] mapVis1Row = new VisualElement[mapVis.GetLength(1)];
        ElementData[] mapRow = new ElementData[map.GetLength(1)];


        for (int i = 0; i < mapVis.GetLength(1); i++)
        {
            mapVis1Row[i] = mapVis[row, i];
            mapRow[i] = map[row, i];
        }

        VisualElement[] mapVisNew;
        ElementData[] mapNew;


        if (dir == 1)
        {
            VisualElement[] step1 = mapVis1Row.Skip( shiftAmount).Take(mapVis1Row.Length - shiftAmount).ToArray();
            VisualElement[] step2 = mapVis1Row.Take(shiftAmount).ToArray();
            mapVisNew = step1.Concat(step2).ToArray();
            ElementData[] stepData1 = mapRow.Skip(shiftAmount).Take(mapRow.Length - shiftAmount).ToArray();
            ElementData[] stepData2 = mapRow.Take(shiftAmount).ToArray();
            mapNew = stepData1.Concat(stepData2).ToArray();

        }
        else
        {
            VisualElement[] step1 = mapVis1Row.Skip(mapVis1Row.Length - shiftAmount).Take(shiftAmount).ToArray();
            VisualElement[] step2 = mapVis1Row.Take(mapVis1Row.Length - shiftAmount).ToArray();
            mapVisNew = step1.Concat(step2).ToArray();


            ElementData[] stepData1 = mapRow.Skip(mapRow.Length - shiftAmount).Take(shiftAmount).ToArray();
            ElementData[] stepData2 = mapRow.Take(mapRow.Length - shiftAmount).ToArray();
            mapNew = stepData1.Concat(stepData2).ToArray();
        }
        for (int j = 0; j < mapVis.GetLength(1); j++)
        {
             mapVis[row, j]= mapVisNew[j];
            map[row, j] = mapNew[j];
            Vector3 posMin = new Vector3(Mathf.Cos(anglesOfRows[j].x ) * radiuses[row].x, Mathf.Sin(anglesOfRows[j].x) * radiuses[row].y, 0);
            mapVis[row, j].transform.localPosition = posMin;
            mapVis[row, j].SetRotation(anglesOfRows[j].x);
        }
    }

   

    public static   bool CheckMoves(ElementData[,] map, int max, int min)
    {
        bool moves = false;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                for (int k = 0; k < map.GetLength(1); k++)
                {
                    int nextRow = i + 1;
                    if (map[i, j].Side == -1)
                    {
                        nextRow = i - 1;
                        if (nextRow == -1)
                            nextRow = map.GetLength(0) - 1;
                    }
                    else
                    {
                        nextRow = i + 1;
                        if (nextRow == map.GetLength(0))
                            nextRow = 0;

                    }


                    if (
                        ((map[i, j].CurrentNumber - 1 == map[nextRow, k].CurrentNumber)
                   || (map[i, j].CurrentNumber == min && map[nextRow, k].CurrentNumber == max - 1))
                             && map[i, j].Side == map[nextRow, k].Side

                   )
                    {

                        moves = true;
                        break;

                    } 
                 } 
            }
        }

        return moves;
    }


    public static void CheckMoves(VisualElement[,] mapVis, ElementData[,] map, out bool hasMoves, int row, int max, int min, int shift)
    {
        hasMoves = false;
        //if (map.GetLength(0) - 1 == row)
        //    return;
        int dir = (int)Mathf.Sign(shift);
        shift = Mathf.Abs(shift);
        int shiftAmount = shift - (shift / mapVis.GetLength(1)) * mapVis.GetLength(1);
       


        
            for (int j = 0; j < map.GetLength(1); j++)
            {

            int nextRow = row + 1;
            if (map[row, j].Side == -1)
            {
                nextRow = row - 1;
                if (nextRow == -1)
                    nextRow = map.GetLength(0) - 1;
            }
            else
            {
                if (nextRow == map.GetLength(0))
                    nextRow = 0;

            }
            int index = j + shiftAmount;
            if (dir == -1)
            {
                if (j+ shiftAmount < map.GetLength(1))
                {
                    index = j + shiftAmount;
                }
                else
                {
                    index = j + shiftAmount - map.GetLength(1);
                }
            }
            else
            {
                if (j - shiftAmount >=0)
                    index = j - shiftAmount;
                else
                {
                    index = map.GetLength(1)-shiftAmount+j;
                }

            }
            //Debug.Log(index);
            if (mapVis[row, j].PrevMatch != -1)
                mapVis[nextRow, mapVis[row, j].PrevMatch].MakeOverlayEffect(false, j);
            if (
            ((map[row, j].CurrentNumber - 1 == map[nextRow, index].CurrentNumber)
           || (map[row, j].CurrentNumber == min && map[nextRow, index].CurrentNumber == max - 1))
           && map[row, j].Side == map[nextRow, index].Side

           )
            {
                hasMoves = true;
              
                mapVis[row, j].MakeOverlayEffect(true, index);
                mapVis[nextRow, index].MakeOverlayEffect(true, j, sizeCoef:0.7f);
            }
            else
            {
                mapVis[row, j].MakeOverlayEffect(false, index);
                mapVis[nextRow, index].MakeOverlayEffect(false, j);
            }
        }
        

   
    }


   



    public static void Replay(VisualElement[,] mapVis, ElementData[,] map,  int max, int min, ref int lastNumber)
    {
        CleverMapGeneration(map, min, max);
        lastNumber = Random.Range(min, max);
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                //map[i, j] = new ElementData() { CurrentNumber = Random.Range(min, max) };

                //SetText(mapVis[i, j], map[i, j]);
                mapVis[i, j].SetText(map[i, j]);
            }
        }

    }
}
