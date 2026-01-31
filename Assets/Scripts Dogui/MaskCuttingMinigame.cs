using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MaskCuttingMinigame : MonoBehaviour
{
  [Header("References")]
  public Camera targetCamera;
  public SpriteRenderer furnitureSpriteRenderer; // The furniture to cut from
  public LineRenderer lineRenderer; // To show the player's drawing

  [Header("Drawing Settings")]
  public float minDistanceBetweenPoints = 0.1f;
  public Color drawingColor = Color.red;
  public float lineWidth = 0.05f;

  [Header("Capture Settings")]
  public int captureHeight = 500;

  private List<Vector2> drawnPoints = new List<Vector2>();
  private bool isDrawing = false;

  void Start()
  {
    // Setup line renderer
    if (lineRenderer != null)
    {
      lineRenderer.startWidth = lineWidth;
      lineRenderer.endWidth = lineWidth;
      lineRenderer.startColor = drawingColor;
      lineRenderer.endColor = drawingColor;
      lineRenderer.positionCount = 0;
      lineRenderer.useWorldSpace = true;
    }
  }

  void Update()
  {
    // Start drawing on mouse down
    if (Input.GetMouseButtonDown(0))
    {
      StartDrawing();
    }

    // Continue drawing while holding mouse
    if (Input.GetMouseButton(0) && isDrawing)
    {
      ContinueDrawing();
    }

    // Finish drawing on mouse up
    if (Input.GetMouseButtonUp(0) && isDrawing)
    {
      FinishDrawing();
    }

    // Press P to cut and save
    if (Input.GetKeyDown(KeyCode.P))
    {
      CutAndSaveMask();
    }
  }

  void StartDrawing()
  {
    isDrawing = true;
    drawnPoints.Clear();

    if (lineRenderer != null)
    {
      lineRenderer.positionCount = 0;
    }

    Debug.Log("Started drawing");
  }

  void ContinueDrawing()
  {
    Vector3 mouseWorldPos = GetMouseWorldPosition();
    Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

    // Only add point if it's far enough from the last point
    if (drawnPoints.Count == 0 ||
        Vector2.Distance(mousePos2D, drawnPoints[drawnPoints.Count - 1]) > minDistanceBetweenPoints)
    {
      drawnPoints.Add(mousePos2D);

      // Update line renderer
      if (lineRenderer != null)
      {
        lineRenderer.positionCount = drawnPoints.Count;
        lineRenderer.SetPosition(drawnPoints.Count - 1, new Vector3(mousePos2D.x, mousePos2D.y, 0));
      }
    }
  }

  void FinishDrawing()
  {
    isDrawing = false;

    // Close the shape by connecting to the first point
    if (drawnPoints.Count > 2)
    {
      drawnPoints.Add(drawnPoints[0]);

      if (lineRenderer != null)
      {
        lineRenderer.positionCount = drawnPoints.Count;
        lineRenderer.SetPosition(drawnPoints.Count - 1,
            new Vector3(drawnPoints[0].x, drawnPoints[0].y, 0));
      }
    }

    Debug.Log($"Finished drawing with {drawnPoints.Count} points");
  }

  Vector3 GetMouseWorldPosition()
  {
    Vector3 mousePos = Input.mousePosition;
    mousePos.z = -targetCamera.transform.position.z;
    return targetCamera.ScreenToWorldPoint(mousePos);
  }

  public Sprite CutAndSaveMask()
  {
    if (drawnPoints.Count < 3)
    {
      Debug.LogWarning("Need at least 3 points to create a mask!");
      return null;
    }

    if (furnitureSpriteRenderer == null || furnitureSpriteRenderer.sprite == null)
    {
      Debug.LogWarning("No furniture sprite assigned!");
      return null;
    }

    // Get the original sprite texture
    Sprite originalSprite = furnitureSpriteRenderer.sprite;
    Texture2D originalTexture = originalSprite.texture;

    // Calculate width based on camera's aspect ratio to prevent squishing
    float aspectRatio = targetCamera.aspect;
    int captureWidth = Mathf.RoundToInt(captureHeight * aspectRatio);

    // Create a new texture with the same dimensions
    Texture2D maskedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);

    // Render the sprite to a RenderTexture
    RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
    RenderTexture currentRT = RenderTexture.active;

    targetCamera.targetTexture = renderTexture;
    targetCamera.Render();

    RenderTexture.active = renderTexture;
    maskedTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
    maskedTexture.Apply();

    targetCamera.targetTexture = null;
    RenderTexture.active = currentRT;
    Destroy(renderTexture);

    // Apply mask based on drawn shape
    Color[] pixels = maskedTexture.GetPixels();

    for (int y = 0; y < captureHeight; y++)
    {
      for (int x = 0; x < captureWidth; x++)
      {
        int pixelIndex = y * captureWidth + x;

        // Convert pixel coordinates to world position
        Vector2 pixelWorldPos = PixelToWorldPosition(x, y, captureWidth, captureHeight);

        // Check if this pixel is inside the drawn shape
        if (!IsPointInsidePolygon(pixelWorldPos, drawnPoints))
        {
          // Make pixels outside the drawn shape transparent
          pixels[pixelIndex] = new Color(pixels[pixelIndex].r, pixels[pixelIndex].g,
                                         pixels[pixelIndex].b, 0f);
        }
      }
    }

    maskedTexture.SetPixels(pixels);
    maskedTexture.Apply();

    // Save as PNG
    byte[] bytes = maskedTexture.EncodeToPNG();
    string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
    string filename = "CutMask_" + timestamp + ".png";
    string path = Path.Combine(Application.dataPath, filename);
    File.WriteAllBytes(path, bytes);
    Debug.Log("Mask saved to: " + path);

    // Create and return sprite
    Sprite cutSprite = Sprite.Create(maskedTexture,
        new Rect(0, 0, captureWidth, captureHeight),
        new Vector2(0.5f, 0.5f));

    // Clear the drawing
    drawnPoints.Clear();
    if (lineRenderer != null)
    {
      lineRenderer.positionCount = 0;
    }

    return cutSprite;
  }

  Vector2 PixelToWorldPosition(int pixelX, int pixelY, int textureWidth, int textureHeight)
  {
    // Get the camera's orthographic bounds
    float cameraHeight = targetCamera.orthographicSize * 2;
    float cameraWidth = cameraHeight * targetCamera.aspect;

    Vector3 cameraPos = targetCamera.transform.position;

    // Convert pixel to normalized coordinates (0 to 1)
    float normalizedX = pixelX / (float)textureWidth;
    float normalizedY = pixelY / (float)textureHeight;

    // Convert to world position
    float worldX = cameraPos.x - cameraWidth / 2 + normalizedX * cameraWidth;
    float worldY = cameraPos.y - cameraHeight / 2 + normalizedY * cameraHeight;

    return new Vector2(worldX, worldY);
  }

  // Ray casting algorithm to check if a point is inside a polygon
  bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
  {
    if (polygon.Count < 3) return false;

    int intersections = 0;
    for (int i = 0; i < polygon.Count - 1; i++)
    {
      Vector2 p1 = polygon[i];
      Vector2 p2 = polygon[i + 1];

      if (point.y > Mathf.Min(p1.y, p2.y))
      {
        if (point.y <= Mathf.Max(p1.y, p2.y))
        {
          if (point.x <= Mathf.Max(p1.x, p2.x))
          {
            float xIntersection = (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;

            if (p1.x == p2.x || point.x <= xIntersection)
            {
              intersections++;
            }
          }
        }
      }
    }

    return (intersections % 2) != 0;
  }
}
