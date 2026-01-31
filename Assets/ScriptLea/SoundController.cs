using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    public AudioSource AS_Soundtrack; // arrastra un AudioSource en el inspector (o se creará en Start)

    public enum SFX
    {
        Aceptado,
        CajonAbriendoseLapices,
        CajonAbriendose,
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
    public AudioClip[] sfxClips;

    public void PlaySFX(SFX sfx)
    {
        AS_Soundtrack.PlayOneShot(sfxClips[(int)sfx]);
    }
    void Start()
    {
        //PlaySFX(SFX.Aceptado);
    }

}
