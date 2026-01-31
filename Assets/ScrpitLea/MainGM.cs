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
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == 3)
                {
                    switch (hit.collider.tag)
                    {
                        case "0":
                            Instantiate(MiniJuegos[0], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("0");
                            break;
                        case "1":
                            Instantiate(MiniJuegos[1], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("1");
                            break;
                        case "2":
                            Instantiate(MiniJuegos[2], new Vector3(0, 0, 0), Quaternion.identity);
                            Debug.Log("2");
                            break;
                    }
                }
                //Debug.Log("Click sobre: " + hit.collider.name);
            }
        }
    }
}
