using Unity.VisualScripting;
using UnityEngine;

public class MainGM : MonoBehaviour
{
    [SerializeField]
    GameObject[] MiniJuegos = new GameObject[3];
    public GameObject selectorLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hola");
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Guarda la posicion del mouse en mousePos cuando hace click
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); //En hit se guarda lo que clickeó
            Debug.Log("lo detecto!");

            if (hit.collider != null) //Si toque algo
            {
                if (hit.collider.gameObject.layer == 3) //El layer 3 es "minijuegos".
                {
                    switch (hit.collider.tag)
                    {
                        case "0":
                            MiniJuegos[0].transform.gameObject.SetActive(true);
                            //Instantiate(MiniJuegos[0], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("Juego 0 activado");
                            break;
                        case "1":
                            MiniJuegos[1].transform.gameObject.SetActive(true);
                            Debug.Log(MiniJuegos[1].transform.gameObject.activeSelf);
                            //Instantiate(MiniJuegos[1], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("Juego 1 haktibado");
                            break;
                        case "2":
                            MiniJuegos[2].transform.gameObject.SetActive(true);
                            //Instantiate(MiniJuegos[2], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("Juego 2 activado");
                            break;
                    }
                    selectorLevel.transform.gameObject.SetActive(false);
                }
                //Debug.Log("Click sobre: " + hit.collider.name);
            }
        }
    }
}
