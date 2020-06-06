using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public  ElementData[] elementDatas;

    public int Rows;
    public int Colomns;
    public int Score;
    public int LastNumber;
    public void MapToArray(ElementData[,] elements)
    {

        Rows = elements.GetLength(0);
        Colomns = elements.GetLength(1);
        elementDatas = new ElementData[Rows * Colomns];


        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Colomns; j++)
            {
                elementDatas[i * Colomns + j] = elements[i, j];
            }
        }
    }

    public ElementData[,] ArrayToMap()
    {

        ElementData[,] map = new ElementData[Rows, Colomns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Colomns; j++)
            {
               map[i, j] = elementDatas[i * Colomns + j];
            }
        }


        return map;

    }

}
public static class SaveController
 {
    public static List<string> PrevStates = new List<string>();
    public static int BackStepsLimit = 4;
    public static int QueueAmount = 50;
    public static    List<int> BPrevGenNumbers = new List<int>();
    
    public static bool HasSteps { get { return PrevStates.Count > 0; } }
    public static void AddGeneratedNumberToQueue(int number )
    {

        BPrevGenNumbers.Add(number);
        if (BPrevGenNumbers.Count == QueueAmount)
            BPrevGenNumbers.RemoveAt(0);
    }

    public static int GetPrevGeneratedNumberToQueue(int step)
    {
        int lastgenNum = -1;
        if (BPrevGenNumbers.Count != 0)
        {
            lastgenNum = BPrevGenNumbers[BPrevGenNumbers.Count - 1];

        }
        return  lastgenNum;
    }


    public static void AddStep(string data)
    {

        
           

            PrevStates.Add(data);

        if (PrevStates.Count == BackStepsLimit)
            PrevStates.RemoveAt(0);

    

    }

    public static void AddStep(SaveData save)
    {


        string data = GetSaveDataAsJson(save);


        PrevStates.Add(data);

        if (PrevStates.Count == BackStepsLimit)
            PrevStates.RemoveAt(0);



    }
    public static SaveData GetSave(ElementData[,] elements, int score, int lastNumber)
    {

        SaveData sd = new SaveData();

        sd.MapToArray(elements);
        sd.Score = score;
        sd.LastNumber = lastNumber;
        return sd;
    }
    public static SaveData BackStep(ref int steps)
    {

        SaveData sd = null;

        if (PrevStates.Count != 0)
        {
           string data = PrevStates[PrevStates.Count - 1];
            try
            {
                 sd = JsonUtility.FromJson<SaveData>(data);            
                PrevStates.RemoveAt(PrevStates.Count - 1);
            }
            catch
            { }
            steps = PrevStates.Count;
        }
        return sd;

    }


    public  static void Save(string key, ElementData[,] elements, int score, int lastNumber)
    {
        string data = GetSaveDataAsJson(elements, score,  lastNumber);
        PlayerPrefs.SetString(key, data);
    }

 public   static string GetSaveDataAsJson(ElementData[,] elements, int score, int lastNumber)
    {
        SaveData sd = GetSave(elements,  score,  lastNumber);
        string data = JsonUtility.ToJson(sd);

        return data;

    }
    public static string GetSaveDataAsJson(SaveData sd)
    {
       

      
        string data = JsonUtility.ToJson(sd);

        return data;

    }
    public static SaveData Load(string key)
    {

        SaveData save = null;
        if (PlayerPrefs.HasKey(key))
        {

            string data = PlayerPrefs.GetString(key);

            try
            {
                SaveData sd = JsonUtility.FromJson<SaveData>(data);
                save = sd;
            }
            catch
            { }

        }
        return save;
    }


    public static void ClearSave(string key)
    {
        PrevStates = new List<string>();
        PlayerPrefs.DeleteKey(key);

    }




 }

