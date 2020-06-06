using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

 public static   class GameData
{
    public static int SoundLevel=1;

    public static int HighScore = 0;



    public static  void LoadGameData()
    {


        if (PlayerPrefs.HasKey(nameof(SoundLevel)))
        {
            SoundLevel = PlayerPrefs.GetInt(nameof(SoundLevel));

        }
        else
        {
            PlayerPrefs.SetInt(nameof(SoundLevel), SoundLevel);

        }

        if (PlayerPrefs.HasKey(nameof(HighScore)))
        {
            HighScore = PlayerPrefs.GetInt(nameof(HighScore));

        }
        else
            PlayerPrefs.SetInt(nameof(HighScore), HighScore);

    }

    public static void SetSound()
    {
        if(SoundLevel==0)
        SoundLevel = 1;
        else
            SoundLevel = 0;


        // if (!PlayerPrefs.HasKey(nameof(SoundLevel)))
        {
            PlayerPrefs.SetInt(nameof(SoundLevel), SoundLevel);
        }
    }
    public static void SetHighscore(int score)
    {

        HighScore = score;


        // if (!PlayerPrefs.HasKey(nameof(SoundLevel)))
        {
            PlayerPrefs.SetInt(nameof(HighScore), HighScore);
        }
    }

}

