using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void BtnJugar()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void BtnSalir()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
