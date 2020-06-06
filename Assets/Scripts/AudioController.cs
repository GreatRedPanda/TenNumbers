using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AudioInfo
{

    public GameAudioEventType AudioEventType;
    public AudioSource Source;

}
public class AudioController : MonoBehaviour
{

    public List<AudioInfo> AudioSources = new List<AudioInfo>();


    Dictionary<GameAudioEventType, AudioSource> audios = new Dictionary<GameAudioEventType, AudioSource>();
        
        // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlaySound(GameAudioEventType eventType )
    {
        if (GameData.SoundLevel == 1)
        {
            AudioInfo audioInfo = AudioSources.Find(x => x.AudioEventType == eventType);
            if (audioInfo != null)
            {
                audioInfo.Source.Play();
            }
        }
    }
}
