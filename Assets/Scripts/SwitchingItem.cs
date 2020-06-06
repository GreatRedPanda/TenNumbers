using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwitchingItem : MonoBehaviour
{
    public Image ImageToSwitch;
    public Sprite[] SpritesToSwitch;
    public Color[] ColorToSwitch;
    int currentColorIndex = 0;
    int currentSpriteIndex = 0;
    bool useColorSwitch = false;
    bool useImageSwitch = false;
    // Use this for initialization
    void Start()
    {
        if (SpritesToSwitch != null && SpritesToSwitch.Length > 0)
            useImageSwitch = true;

        if (ColorToSwitch != null && ColorToSwitch.Length > 0)
            useColorSwitch = true;
    }





    public void Switch()
    {

        if (useColorSwitch)
        {
            currentColorIndex++;
            if (currentColorIndex == ColorToSwitch.Length)
                currentColorIndex = 0;
            ImageToSwitch.color = ColorToSwitch[currentColorIndex];
        }

        if (useImageSwitch)
        {
            currentSpriteIndex++;
            if (currentSpriteIndex == SpritesToSwitch.Length)
                currentSpriteIndex = 0;
            ImageToSwitch.sprite = SpritesToSwitch[currentSpriteIndex];
        }
    }


    public void Switch(bool enable)
    {

        if (useColorSwitch)
        {
            if(enable)
            currentColorIndex=1;
            else
                currentColorIndex = 0;
            ImageToSwitch.color = ColorToSwitch[currentColorIndex];
        }

        if (useImageSwitch)
        {
            if (enable)
                currentSpriteIndex = 1;
            else
                currentSpriteIndex = 0;
            ImageToSwitch.sprite = SpritesToSwitch[currentSpriteIndex];
        }
    }

}
