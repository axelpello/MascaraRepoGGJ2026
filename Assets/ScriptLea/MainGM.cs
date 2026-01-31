using UnityEngine;

public class MainGM : MonoBehaviour
{
    [SerializeField]
    GameObject[] MiniJuegos = new GameObject[3];
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
                            Debug.Log("0");
                            break;
                        case "1":
                            MiniJuegos[0].transform.gameObject.SetActive(true);
                            //Instantiate(MiniJuegos[1], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("1");
                            break;
                        case "2":
                            MiniJuegos[0].transform.gameObject.SetActive(true);
                            //Instantiate(MiniJuegos[2], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("2");
                            break;

                    }
                }
                //Debug.Log("Click sobre: " + hit.collider.name);
            }
        }
    }
}
