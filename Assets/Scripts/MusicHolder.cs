using UnityEngine;

public class MusicHolder : MonoBehaviour
{

    [SerializeField]
    AudioClip[] gameMusics;

    AudioSource audioSource;

    void Start()
    {        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = gameMusics[0];
        audioSource.volume = 0.4f;
        audioSource.Play();
        DontDestroyOnLoad(this);
    }
}
