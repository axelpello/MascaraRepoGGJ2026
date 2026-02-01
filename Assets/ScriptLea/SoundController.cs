using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip[] sfxClips;
    public enum SFX
    {
        Aceptado,
        Angry,
        CajonAbriendose,
        CajonAbriendoseLapices,
        ClickButton,
        Denegado,
        Escaner,
        heHey,
        Hmm,
        Huh_01,
        Huh_02,
        Idea,
        Lapices,
        Limpio,
        Mopa,
        Sucio,
        TelaSillon,
        Texto_01,
        Texto_02,
        TijeraTelaSillon,
        Tijeras
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            musicSource.playOnAwake = true;
            musicSource.loop = true;
        }
    }
    public void PlayOnce(SFX sfx)
    {
        sfxSource.PlayOneShot(sfxClips[(int)sfx]);
        Debug.Log(sfxClips[(int)sfx].name);
    }
    public void StopSound(AudioSource AS)
    {
        AS.Stop(); // detiene todo lo que reproduce esta AudioSource
    }
}
