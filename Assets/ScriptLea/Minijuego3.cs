using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Minijuego3 : MonoBehaviour
{
    private MainGM mouseScript;
    /* private MainGM mouseScript;
    private bool isMopaSelected = false;
    private GameObject mopa;
   void Start()
    {
        mouseScript = GameObject.Find("Mouse").GetComponent<MainGM>();
        mouseScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.transform.name);
                if (hit.collider.gameObject.name == "Mopa")
                {
                    Debug.Log("SADDASADSDASDS");
                    mopa = hit.collider.gameObject;
                    isMopaSelected = true;
                }
                //Debug.Log("Click sobre: " + hit.collider.name);
            }
        }

        if (isMopaSelected)
        {
            mopa.transform.localPosition = mouseScript.transform.localPosition;
        }
    }*/
    private GameObject mopa;
    private bool isMopaSelected = false;
    public List<GameObject> basura;
    public List<GameObject> positions;
    public bool isSpawnin = true;
    Vector2 mousePos;
    IEnumerator BasuraDrop()
    {
        while (isSpawnin)
        {
            Instantiate(basura[Random.Range(0, basura.Count())], positions[Random.Range(0, positions.Count())].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(.5f);
        }
    }


    void Start()
    {
        mouseScript = GameObject.Find("Mouse").GetComponent<MainGM>();
        mouseScript.enabled = false;
        StartCoroutine(BasuraDrop());
        //drop();
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && !isMopaSelected)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider.name == "Mopa")
            {
                mopa = hit.collider.gameObject;
                isMopaSelected = true;
            }
        }
        if (isMopaSelected)
        {
            Vector3 pos = mopa.transform.position;
            pos.x = mousePos.x;
            mopa.transform.position = pos;

        }
    }


}
