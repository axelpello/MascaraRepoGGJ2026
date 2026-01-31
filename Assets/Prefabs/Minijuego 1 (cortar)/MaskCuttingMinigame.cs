using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MaskCuttingMinigame : MonoBehaviour
{
  [Header("References")]
  public Camera targetCamera;
  public SpriteRenderer furnitureSpriteRenderer; // The furniture to cut from
  public LineRenderer lineRenderer; // To show the player's drawing
  public GameObject headOutline; // This needs to be active during drawing for reference, but deleted after cutting

  [Header("Drawing Settings")]
  public float minDistanceBetweenPoints = 0.1f;
  public Color drawingColor = Color.red;
  public float lineWidth = 0.05f;

  [Header("Capture Settings")]
  public int captureHeight = 500;

  [Header("Output")]
  public Sprite generatedMaskSprite; // Store the cut mask here
  public Texture2D generatedMaskTexture; // Store the texture here

  private List<Vector2> drawnPoints = new List<Vector2>();
  private bool isDrawing = false;
  private bool isProcessing = false; // Prevent multiple simultaneous captures
  private int currentStep = 1; // 1=Face outline, 2=Left eye, 3=Right eye, 4=Mouth
  private Texture2D workingTexture; // Texture that gets modified across steps

  private string[] stepNames = { "Face Outline", "Left Eye", "Right Eye", "Mouth" };

  void Start()
  {
    Debug.Log($"[MaskCutting] Start() called - GameObject: {gameObject.name}");

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

    Debug.Log($"[MaskCutting] Step {currentStep}: Draw {stepNames[currentStep - 1]}");
  }

  void Update()
  {
    // Add point on each mouse click
    if (Input.GetMouseButtonDown(0))
    {
      AddPoint();
    }

    // Press Enter to finish drawing and capture after 1 second
    if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !isProcessing)
    {
      StartCoroutine(FinishAndCapture());
    }

    // Optional: Press Escape to cancel drawing
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      CancelDrawing();
    }
  }

  IEnumerator FinishAndCapture()
  {
    isProcessing = true;

    // Close the shape
    FinishDrawing();

    // Wait 1 second only on the final step (step 4)
    if (currentStep == 4)
    {
      yield return new WaitForSeconds(1f);
    }

    // Process the current step
    ProcessCurrentStep();

    isProcessing = false;
  }

  void AddPoint()
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

      Debug.Log($"Point {drawnPoints.Count} added at {mousePos2D}");
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

  void CancelDrawing()
  {
    drawnPoints.Clear();
    if (lineRenderer != null)
    {
      lineRenderer.positionCount = 0;
    }
    Debug.Log("Drawing cancelled");
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

  void ProcessCurrentStep()
  {
    Debug.Log($"[MaskCutting] ProcessCurrentStep called - current step BEFORE processing: {currentStep}");

    if (currentStep == 1)
    {
      // First step: capture the initial face outline
      CutAndSaveMask(keepInside: true);
    }
    else if (currentStep >= 2 && currentStep <= 4)
    {
      // Steps 2-4: cut out eyes and mouth (remove inside)
      CutAndSaveMask(keepInside: false);
    }

    // Clear drawing for next step
    drawnPoints.Clear();
    if (lineRenderer != null)
    {
      lineRenderer.positionCount = 0;
    }

    // Move to next step
    currentStep++;
    Debug.Log($"[MaskCutting] Step incremented to: {currentStep}");

    // Check if we're done
    if (currentStep <= 4)
    {
      Debug.Log($"[MaskCutting] Step {currentStep}: Draw {stepNames[currentStep - 1]}");
    }
    else
    {
      Debug.Log("[MaskCutting] All steps complete! Final mask saved.");
      // Optionally disable the script or trigger next phase
    }
  }

  public Sprite CutAndSaveMask(bool keepInside = true)
  {
    // Only disable head outline on the final step
    if (currentStep == 4 && headOutline != null)
    {
      headOutline.SetActive(false);
    }

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

    // For first step, capture from camera. For subsequent steps, use workingTexture
    Texture2D maskedTexture;

    if (currentStep == 1)
    {
      // First step: capture from camera
      maskedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);

      // Store original camera settings
      CameraClearFlags originalClearFlags = targetCamera.clearFlags;
      Color originalBackgroundColor = targetCamera.backgroundColor;

      // Set camera to render with transparent background
      targetCamera.clearFlags = CameraClearFlags.SolidColor;
      targetCamera.backgroundColor = new Color(0, 0, 0, 0); // Transparent

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

      // Restore original camera settings
      targetCamera.clearFlags = originalClearFlags;
      targetCamera.backgroundColor = originalBackgroundColor;

      Destroy(renderTexture);
    }
    else
    {
      // Subsequent steps: work with existing texture
      if (workingTexture == null)
      {
        Debug.LogError("Working texture is null! Cannot continue.");
        return null;
      }
      maskedTexture = new Texture2D(workingTexture.width, workingTexture.height, TextureFormat.RGBA32, false);
      maskedTexture.SetPixels(workingTexture.GetPixels());
      maskedTexture.Apply();
      captureWidth = workingTexture.width;
      captureHeight = workingTexture.height;
    }

    // Apply mask based on drawn shape
    Color[] pixels = maskedTexture.GetPixels();

    int textureHeight = maskedTexture.height;
    int textureWidth = maskedTexture.width;

    for (int y = 0; y < textureHeight; y++)
    {
      for (int x = 0; x < textureWidth; x++)
      {
        int pixelIndex = y * textureWidth + x;

        // Convert pixel coordinates to world position
        Vector2 pixelWorldPos = PixelToWorldPosition(x, y, textureWidth, textureHeight);

        bool isInside = IsPointInsidePolygon(pixelWorldPos, drawnPoints);

        // Step 1: Keep inside, remove outside
        // Steps 2-4: Remove inside, keep outside
        bool shouldMakeTransparent = keepInside ? !isInside : isInside;

        if (shouldMakeTransparent)
        {
          // Make pixels transparent
          pixels[pixelIndex] = new Color(pixels[pixelIndex].r, pixels[pixelIndex].g,
                                         pixels[pixelIndex].b, 0f);
        }
      }
    }

    maskedTexture.SetPixels(pixels);
    maskedTexture.Apply();

    // Store the working texture for next steps
    workingTexture = maskedTexture;

    // Save as PNG only on final step
    if (currentStep == 4)
    {
      byte[] bytes = maskedTexture.EncodeToPNG();
      string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
      string filename = "CutMask_" + timestamp + ".png";
      string path = Path.Combine(Application.dataPath, filename);
      File.WriteAllBytes(path, bytes);
      Debug.Log("Mask saved to: " + path);
    }

    // Create and return sprite
    Sprite cutSprite = Sprite.Create(maskedTexture,
        new Rect(0, 0, maskedTexture.width, maskedTexture.height),
        new Vector2(0.5f, 0.5f));

    // Store for later use
    generatedMaskSprite = cutSprite;
    generatedMaskTexture = maskedTexture;

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
