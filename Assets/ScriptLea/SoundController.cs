using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    public AudioSource AS_Soundtrack; // arrastra un AudioSource en el inspector (o se creará en Start)
    //public AudioClip C_Soundtrack; // arrastra aquí el clip

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            AS_Soundtrack.playOnAwake = true;
            AS_Soundtrack.loop = true;
        }
    }
    public void PlayOnce(AudioSource AS)
    {
        AS.loop = false;
        AS.Play();
    }
    public void StopSound(AudioSource AS)
    {
        AS.Stop(); // detiene todo lo que reproduce esta AudioSource
    }
}
