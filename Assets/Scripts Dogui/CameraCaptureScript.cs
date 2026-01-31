using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Camera targetCamera;
    public int captureHeight;
    public int captureWidth;
    public Color targetolor;
    int depth = 24;

    //Method to render from camera
    public Sprite CaptureScreen()
    {

        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, depth);
        Rect rect = new Rect(0, 0, captureWidth, captureHeight);
        Texture2D texture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);

        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        // Process pixels: make only white visible, rest transparent
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];
            // Check if pixel is a specific color (AB7120)
            Color targetColor = new Color(171f / 255f, 113f / 255f, 32f / 255f);
            float threshold = 0.1f; // Tolerance for color matching
            /* Debug.Log("Pixel color detected at index: " + i + " Color: " + pixel); */
            if (Mathf.Abs(pixel.r - targetColor.r) < threshold &&
                Mathf.Abs(pixel.g - targetColor.g) < threshold &&
                Mathf.Abs(pixel.b - targetColor.b) < threshold)
            {
                // Keep white pixels green

                pixels[i] = new Color(0f, 1f, 1f, 1f);
            }
            else
            {
                // Make non-white pixels transparent
                pixels[i] = new Color(pixel.r, pixel.g, pixel.b, 0f);
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();

        targetCamera.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        Destroy(renderTexture);

        // Save as PNG
        byte[] bytes = texture.EncodeToPNG();
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = "Screenshot_" + timestamp + ".png";
        string path = Path.Combine(Application.dataPath, filename);
        File.WriteAllBytes(path, bytes);
        Debug.Log("Screenshot saved to: " + path);

        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

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
            CaptureScreen();
        }
    }
}
