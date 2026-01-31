using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Camera targetCamera;
    int height = 1024;
    int width = 1024;
    int depth = 24;

    //method to render from camera
    public Sprite CaptureScreen() {
        RenderTexture renderTexture = new RenderTexture(width, height, depth);
        Rect rect = new Rect(0,0,width,height);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        targetCamera.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        Destroy(renderTexture);

        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);
        Debug.Log("Captura realizada");
        return sprite;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("CameraCapture script started");
    }

    // Update is called once per frame
    void Update()
    {
        // Press P to capture screen
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P key pressed!");
            CaptureScreen();
        }
    }
}
