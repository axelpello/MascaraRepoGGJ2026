using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class CinematicaScript : MonoBehaviour
{
    //public List<List<Sprite>> cinematicas = new List<List<Sprite>>();

    [System.Serializable]
    public class SpriteList
    {
        public List<Sprite> sprites;
    }
    public List<SpriteList> spriteGroups;
    public List<Button> botones = new List<Button>();
    public Canvas MainBg;
    private int indxCin = 0;
    private int indxImg = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Image>().sprite = spriteGroups[0].sprites[0];
    }

    public void NextImage()
    {
        indxImg++;
        if (indxImg >= spriteGroups[indxCin].sprites.Count)
        {
            indxImg = 0;
            indxCin++;
            MainBg.gameObject.SetActive(true);
            transform.parent.gameObject.SetActive(false);
            return;
        }
        gameObject.GetComponent<Image>().sprite = spriteGroups[indxCin].sprites[indxImg];
    }
    public void PrevImage()
    {
        indxImg--;
        if (indxImg < 0)
        {
            indxImg = 0;
            return;
        }
        gameObject.GetComponent<Image>().sprite = spriteGroups[indxCin].sprites[indxImg];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
