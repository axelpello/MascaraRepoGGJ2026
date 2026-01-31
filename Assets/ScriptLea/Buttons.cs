using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void BtnJugar()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
