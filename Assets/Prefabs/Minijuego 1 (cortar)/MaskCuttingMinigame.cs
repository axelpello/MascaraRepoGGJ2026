using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 4-Step Mask Cutting Minigame
/// Step 1: Draw around the face outline (keeps inside, removes outside)
/// Steps 2-4: Draw around eyes and mouth (removes inside, keeps outside)
/// Result: A mask sprite with face shape but with eye/mouth holes cut out
/// </summary>
public class MaskCuttingMinigame : MonoBehaviour
{
  [Header("References")]
  public Camera targetCamera; // Camera that captures the scene
  public SpriteRenderer furnitureSpriteRenderer; // The furniture sprite to cut the mask from
  public LineRenderer lineRenderer; // Visual feedback of player's current drawing
  public GameObject headOutline; // Dotted reference outline shown to player (hidden during capture)

  [Header("Reference Textures for Steps")]
  // Template images shown to guide player on what to draw for each body part
  public GameObject faceReference; // Face template - shown in step 1
  public GameObject leftEyeReference; // Left eye template - shown in step 2
  public GameObject rightEyeReference; // Right eye template - shown in step 3
  public GameObject mouthReference; // Mouth template - shown in step 4

  [Header("Drawing Settings")]
  public float minDistanceBetweenPoints = 0.1f; // Minimum distance between drawn points (prevents too many points)
  public Color drawingColor = Color.red; // Color of the line player draws
  public float lineWidth = 0.05f; // Width of the drawing line

  [Header("Capture Settings")]
  public int captureHeight = 500; // Height of captured image (width auto-calculated from camera aspect ratio)

  [Header("Output")]
  public Sprite generatedMaskSprite; // Final mask sprite result
  public Texture2D generatedMaskTexture; // Final mask texture result

  // Private variables
  private List<Vector2> drawnPoints = new List<Vector2>(); // Points player has clicked to draw current shape
  private bool isProcessing = false; // Prevents multiple simultaneous Enter key presses
  private int currentStep = 1; // Current step: 1=Face, 2=Left eye, 3=Right eye, 4=Mouth
  private Texture2D workingTexture; // Texture progressively modified across all steps
  private List<LineRenderer> completedLines = new List<LineRenderer>(); // Stores red lines from previous steps

  private string[] stepNames = { "Face Outline", "Left Eye", "Right Eye", "Mouth" }; // Names for debug logging

  /// <summary>
  /// Initialize the minigame: setup line renderer and show first template
  /// </summary>
  void Start()
  {
    // Configure the line renderer that shows player's drawing in real-time
    if (lineRenderer != null)
    {
      lineRenderer.startWidth = lineWidth;
      lineRenderer.endWidth = lineWidth;
      lineRenderer.startColor = drawingColor;
      lineRenderer.endColor = drawingColor;
      lineRenderer.positionCount = 0; // Start with no points
      lineRenderer.useWorldSpace = true; // Use world coordinates, not local
    }

    // Setup reference templates: only show face template for step 1, hide all others
    // Templates show player what shape to draw for each step
    if (faceReference != null) faceReference.SetActive(true); // Show face for step 1
    if (leftEyeReference != null) leftEyeReference.SetActive(false);
    if (rightEyeReference != null) rightEyeReference.SetActive(false);
    if (mouthReference != null) mouthReference.SetActive(false);
  }

  /// <summary>
  /// Handle player input each frame:
  /// - Left click: Add point to drawing
  /// - Enter: Complete current step and process mask
  /// - Escape: Cancel current drawing
  /// </summary>
  void Update()
  {
    // Left mouse button: Add a point to the current drawing
    if (Input.GetMouseButtonDown(0))
    {
      AddPoint();
    }

    // Enter key: Close the shape and process this step
    // isProcessing prevents multiple simultaneous Enter presses
    if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !isProcessing)
    {
      StartCoroutine(FinishAndCapture());
    }

    // Escape key: Clear current drawing without processing
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      CancelDrawing();
    }
  }

  /// <summary>
  /// Coroutine that handles finishing a drawing step:
  /// - Closes the drawn shape
  /// - On final step: waits 1 second, then hides all overlays for clean capture
  /// - Processes the step (capture and mask)
  /// </summary>
  IEnumerator FinishAndCapture()
  {
    isProcessing = true; // Prevent multiple Enter presses

    // Connect last point to first point to close the shape
    FinishDrawing();

    // Special handling for step 4 (final step): hide everything before capture
    if (currentStep == 4)
    {
      // Give player 1 second to see their completed drawing before final capture
      yield return new WaitForSeconds(1f);

      // Hide ALL visual overlays so final PNG only contains the clean mask
      // This includes: templates, red lines, dotted outline

      if (headOutline != null)
      {
        headOutline.SetActive(false);
      }

      // Hide reference templates
      if (faceReference != null)
      {
        faceReference.SetActive(false);
      }
      if (leftEyeReference != null)
      {
        leftEyeReference.SetActive(false);
      }
      if (rightEyeReference != null)
      {
        rightEyeReference.SetActive(false);
      }
      if (mouthReference != null)
      {
        mouthReference.SetActive(false);
      }

      // Hide all completed lines
      foreach (LineRenderer line in completedLines)
      {
        if (line != null)
        {
          line.gameObject.SetActive(false);
        }
      }

      // Hide current line renderer
      if (lineRenderer != null)
      {
        lineRenderer.positionCount = 0;
      }

      // Wait multiple frames to ensure Unity's rendering pipeline fully processes the hide operations
      // This is critical - without this, camera might capture before objects are actually hidden
      yield return null; // Wait 1 frame
      yield return null; // Wait another frame
      yield return new WaitForEndOfFrame(); // Wait until frame is fully rendered

      // Explicitly tell camera to render the now-clean scene
      if (targetCamera != null)
      {
        targetCamera.Render();
      }
    }

    // Process this step: capture image and apply mask
    ProcessCurrentStep();

    isProcessing = false;
  }

  /// <summary>
  /// Add a point to the current drawing when player clicks
  /// Only adds if point is far enough from last point (prevents too many clustered points)
  /// </summary>
  void AddPoint()
  {
    // Convert mouse screen position to world position
    Vector3 mouseWorldPos = GetMouseWorldPosition();
    Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

    // Only add point if:
    // 1) This is the first point, OR
    // 2) Point is far enough from previous point (controlled by minDistanceBetweenPoints)
    if (drawnPoints.Count == 0 ||
        Vector2.Distance(mousePos2D, drawnPoints[drawnPoints.Count - 1]) > minDistanceBetweenPoints)
    {
      drawnPoints.Add(mousePos2D);

      // Update line renderer to show the new point visually
      if (lineRenderer != null)
      {
        lineRenderer.positionCount = drawnPoints.Count;
        lineRenderer.SetPosition(drawnPoints.Count - 1, new Vector3(mousePos2D.x, mousePos2D.y, 0));
      }
    }
  }

  void StartDrawing()
  {
    drawnPoints.Clear();

    if (lineRenderer != null)
    {
      lineRenderer.positionCount = 0;
    }
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

  /// <summary>
  /// Cancel current drawing: clear all points and hide line
  /// </summary>
  void CancelDrawing()
  {
    drawnPoints.Clear();
    if (lineRenderer != null)
    {
      lineRenderer.positionCount = 0;
    }
  }

  /// <summary>
  /// Close the shape by connecting last point back to first point
  /// This creates a closed polygon needed for masking
  /// </summary>
  void FinishDrawing()
  {
    // Need at least 3 points to make a valid shape
    if (drawnPoints.Count > 2)
    {
      // Add first point at end to close the loop
      drawnPoints.Add(drawnPoints[0]);

      // Update line renderer to show the closing line
      if (lineRenderer != null)
      {
        lineRenderer.positionCount = drawnPoints.Count;
        lineRenderer.SetPosition(drawnPoints.Count - 1,
            new Vector3(drawnPoints[0].x, drawnPoints[0].y, 0));
      }
    }
  }

  /// <summary>
  /// Convert mouse screen position to world position
  /// </summary>
  Vector3 GetMouseWorldPosition()
  {
    Vector3 mousePos = Input.mousePosition;
    mousePos.z = -targetCamera.transform.position.z; // Set Z to camera distance
    return targetCamera.ScreenToWorldPoint(mousePos);
  }

  /// <summary>
  /// Process the current drawing step:
  /// 1. Apply mask based on drawn shape
  /// 2. Save line renderer for display in future steps
  /// 3. Move to next step
  /// 4. Show appropriate template for next step
  /// </summary>
  void ProcessCurrentStep()
  {
    // STEP 1: Keep inside drawn shape, remove everything outside
    // This captures the face area
    if (currentStep == 1)
    {
      CutAndSaveMask(keepInside: true);
    }
    // STEPS 2-4: Remove inside drawn shape, keep everything outside
    // This cuts holes for eyes and mouth
    else if (currentStep >= 2 && currentStep <= 4)
    {
      CutAndSaveMask(keepInside: false);
    }

    // Preserve the red line from this step by creating a permanent copy
    // This allows player to see all previous drawings as they progress
    if (lineRenderer != null && lineRenderer.positionCount > 0)
    {
      // Create a new GameObject to hold a permanent copy of this line
      GameObject lineObj = new GameObject($"CompletedLine_Step{currentStep}");
      lineObj.transform.parent = transform; // Parent to this object so it moves with us

      // Create new LineRenderer component and copy all properties
      LineRenderer newLine = lineObj.AddComponent<LineRenderer>();
      newLine.startWidth = lineRenderer.startWidth;
      newLine.endWidth = lineRenderer.endWidth;
      newLine.startColor = lineRenderer.startColor;
      newLine.endColor = lineRenderer.endColor;
      newLine.material = lineRenderer.material;
      newLine.useWorldSpace = lineRenderer.useWorldSpace;
      newLine.positionCount = lineRenderer.positionCount;

      // Copy all point positions from current line to new line
      Vector3[] positions = new Vector3[lineRenderer.positionCount];
      lineRenderer.GetPositions(positions);
      newLine.SetPositions(positions);

      // Store this completed line so we can hide it later on step 4
      completedLines.Add(newLine);
    }

    // Clear the current drawing data to prepare for next step
    drawnPoints.Clear();
    if (lineRenderer != null)
    {
      lineRenderer.positionCount = 0;
    }

    // Advance to next step
    currentStep++;

    // Update reference textures based on current step
    UpdateReferenceTextures();

    // Check if we're done
    if (currentStep > 4)
    {
      // Hide all the lines after final capture
      foreach (LineRenderer line in completedLines)
      {
        if (line != null)
        {
          line.gameObject.SetActive(false);
        }
      }
      if (lineRenderer != null)
      {
        lineRenderer.positionCount = 0;
      }
    }
  }

  /// <summary>
  /// Manage template visibility based on current step
  /// Templates accumulate: step 2 shows face+leftEye, step 3 shows face+leftEye+rightEye, etc.
  /// This helps player see context of what they've already drawn
  /// </summary>
  void UpdateReferenceTextures()
  {
    // Show templates cumulatively - each step adds a new template
    // but keeps previous ones visible for context

    // Step 1+: Always show face template
    if (currentStep >= 1 && faceReference != null)
    {
      faceReference.SetActive(true);
    }

    // Step 2+: Add left eye template
    if (currentStep >= 2 && leftEyeReference != null)
    {
      leftEyeReference.SetActive(true);
    }

    // Step 3+: Add right eye template
    if (currentStep >= 3 && rightEyeReference != null)
    {
      rightEyeReference.SetActive(true);
    }

    // Step 4: Add mouth template
    if (currentStep >= 4 && mouthReference != null)
    {
      mouthReference.SetActive(true);
    }

    // Step 5 (after completion): Hide all templates
    if (currentStep > 4)
    {
      if (faceReference != null) faceReference.SetActive(false);
      if (leftEyeReference != null) leftEyeReference.SetActive(false);
      if (rightEyeReference != null) rightEyeReference.SetActive(false);
      if (mouthReference != null) mouthReference.SetActive(false);
    }
  }

  /// <summary>
  /// Core masking function: Captures scene and applies polygon mask
  /// 
  /// Step 1 (keepInside=true): 
  /// - Captures camera view to texture
  /// - Makes everything OUTSIDE drawn polygon transparent
  /// - Result: Only face area remains
  /// 
  /// Steps 2-4 (keepInside=false):
  /// - Uses existing workingTexture from step 1
  /// - Makes everything INSIDE drawn polygon transparent
  /// - Result: Cuts holes for eyes/mouth
  /// 
  /// Step 4 also saves final PNG to disk
  /// </summary>
  /// <param name="keepInside">True = keep inside polygon (step 1), False = remove inside (steps 2-4)</param>
  public Sprite CutAndSaveMask(bool keepInside = true)
  {
    // Validate we have enough points to make a polygon
    if (drawnPoints.Count < 3)
    {
      return null;
    }

    // Validate furniture sprite exists
    if (furnitureSpriteRenderer == null || furnitureSpriteRenderer.sprite == null)
    {
      return null;
    }



    // Get reference to original sprite texture (not used directly, but kept for reference)
    Sprite originalSprite = furnitureSpriteRenderer.sprite;
    Texture2D originalTexture = originalSprite.texture;

    // Calculate texture width to match camera's aspect ratio
    // This prevents horizontal squishing in final image
    float aspectRatio = targetCamera.aspect;
    int captureWidth = Mathf.RoundToInt(captureHeight * aspectRatio);

    // Different approach for step 1 vs steps 2-4:
    // Step 1: Capture fresh image from camera
    // Steps 2-4: Work with existing texture from step 1
    Texture2D maskedTexture;

    if (currentStep == 1)
    {
      // STEP 1: Capture camera view to create initial texture
      maskedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);

      // CRITICAL: Hide dotted outline before capture
      // If outline is visible, it gets "baked" into the texture permanently
      // We save its state to restore it after capture (player still needs to see it)
      bool headOutlineWasActive = false;
      if (headOutline != null)
      {
        headOutlineWasActive = headOutline.activeSelf;
        headOutline.SetActive(false);
      }

      // Save camera's original settings so we can restore them after capture
      CameraClearFlags originalClearFlags = targetCamera.clearFlags;
      Color originalBackgroundColor = targetCamera.backgroundColor;

      // Configure camera for transparent background capture
      // This is crucial: makes areas outside furniture transparent instead of black/white
      targetCamera.clearFlags = CameraClearFlags.SolidColor;
      targetCamera.backgroundColor = new Color(0, 0, 0, 0); // Fully transparent

      // Create temporary RenderTexture to capture camera output
      RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
      RenderTexture currentRT = RenderTexture.active;

      // Render camera to our RenderTexture
      targetCamera.targetTexture = renderTexture;
      targetCamera.Render();

      // Copy RenderTexture pixels to our Texture2D
      RenderTexture.active = renderTexture;
      maskedTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
      maskedTexture.Apply();

      // Clean up: reset camera target and active RenderTexture
      targetCamera.targetTexture = null;
      RenderTexture.active = currentRT;

      // Restore headOutline visibility so player can still see it for future steps
      if (headOutline != null && headOutlineWasActive)
      {
        headOutline.SetActive(true);
      }

      // Restore camera to original settings
      targetCamera.clearFlags = originalClearFlags;
      targetCamera.backgroundColor = originalBackgroundColor;

      // Clean up temporary RenderTexture
      Destroy(renderTexture);
    }
    else
    {
      // STEPS 2-4: Don't capture camera again, reuse texture from step 1
      // We modify the existing texture to cut out eyes/mouth
      if (workingTexture == null)
      {
        return null;
      }
      // Create a copy of workingTexture to modify
      maskedTexture = new Texture2D(workingTexture.width, workingTexture.height, TextureFormat.RGBA32, false);
      maskedTexture.SetPixels(workingTexture.GetPixels());
      maskedTexture.Apply();
      captureWidth = workingTexture.width;
      captureHeight = workingTexture.height;
    }

    // MASKING ALGORITHM: Loop through every pixel and decide if it should be transparent
    // Get all pixels from texture as array for processing
    Color[] pixels = maskedTexture.GetPixels();

    int textureHeight = maskedTexture.height;
    int textureWidth = maskedTexture.width;

    // Loop through every pixel in the texture
    for (int y = 0; y < textureHeight; y++)
    {
      for (int x = 0; x < textureWidth; x++)
      {
        // Calculate 1D array index from 2D coordinates
        int pixelIndex = y * textureWidth + x;

        // Convert pixel texture coordinates to world coordinates
        // This allows us to check if pixel is inside player's drawn polygon
        Vector2 pixelWorldPos = PixelToWorldPosition(x, y, textureWidth, textureHeight);

        // Use ray casting algorithm to check if pixel is inside drawn polygon
        bool isInside = IsPointInsidePolygon(pixelWorldPos, drawnPoints);

        // Determine if pixel should be transparent based on step:
        // Step 1 (keepInside=true): Make pixels OUTSIDE polygon transparent
        // Steps 2-4 (keepInside=false): Make pixels INSIDE polygon transparent
        bool shouldMakeTransparent = keepInside ? !isInside : isInside;

        if (shouldMakeTransparent)
        {
          // Set alpha to 0 (fully transparent) while keeping RGB values
          pixels[pixelIndex] = new Color(pixels[pixelIndex].r, pixels[pixelIndex].g,
                                         pixels[pixelIndex].b, 0f);
        }
      }
    }

    // Apply all pixel changes back to texture
    maskedTexture.SetPixels(pixels);
    maskedTexture.Apply();

    // Save texture for next step to modify
    // Steps 2-4 will use this as their starting point instead of capturing camera again
    workingTexture = maskedTexture;

    // Only save PNG file on final step (step 4)
    if (currentStep == 4)
    {
      byte[] bytes = maskedTexture.EncodeToPNG();
      string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
      string filename = "CutMask_" + timestamp + ".png";
      string path = Path.Combine(Application.dataPath, filename);
      File.WriteAllBytes(path, bytes);
    }

    // Convert Texture2D to Sprite for use in Unity
    // Pivot at center (0.5, 0.5)
    Sprite cutSprite = Sprite.Create(maskedTexture,
        new Rect(0, 0, maskedTexture.width, maskedTexture.height),
        new Vector2(0.5f, 0.5f));

    // Store in public variables so other scripts can access the result
    generatedMaskSprite = cutSprite;
    generatedMaskTexture = maskedTexture;

    return cutSprite;
  }

  /// <summary>
  /// Convert pixel coordinates (texture space) to world coordinates
  /// This allows us to check if a texture pixel is inside the player's drawn polygon
  /// </summary>
  Vector2 PixelToWorldPosition(int pixelX, int pixelY, int textureWidth, int textureHeight)
  {
    // Calculate camera's visible area in world units
    float cameraHeight = targetCamera.orthographicSize * 2;
    float cameraWidth = cameraHeight * targetCamera.aspect;

    Vector3 cameraPos = targetCamera.transform.position;

    // Convert pixel to 0-1 normalized coordinates
    float normalizedX = pixelX / (float)textureWidth;
    float normalizedY = pixelY / (float)textureHeight;

    // Map normalized coordinates to world coordinates based on camera bounds
    float worldX = cameraPos.x - cameraWidth / 2 + normalizedX * cameraWidth;
    float worldY = cameraPos.y - cameraHeight / 2 + normalizedY * cameraHeight;

    return new Vector2(worldX, worldY);
  }

  /// <summary>
  /// Ray casting algorithm: Determines if a point is inside a polygon
  /// 
  /// How it works:
  /// 1. Cast a ray from the point to the right (positive X direction)
  /// 2. Count how many polygon edges the ray intersects
  /// 3. If odd number of intersections: point is INSIDE
  /// 4. If even number of intersections: point is OUTSIDE
  /// 
  /// This is a standard computational geometry algorithm
  /// </summary>
  bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
  {
    if (polygon.Count < 3) return false; // Need at least 3 points for a polygon

    int intersections = 0;

    // Check each edge of the polygon
    for (int i = 0; i < polygon.Count - 1; i++)
    {
      Vector2 p1 = polygon[i];
      Vector2 p2 = polygon[i + 1];

      // Check if ray from point (going right) could intersect this edge
      // Edge must cross the point's Y level
      if (point.y > Mathf.Min(p1.y, p2.y))
      {
        if (point.y <= Mathf.Max(p1.y, p2.y))
        {
          if (point.x <= Mathf.Max(p1.x, p2.x))
          {
            // Calculate X coordinate where ray intersects this edge
            float xIntersection = (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;

            // If intersection is to the right of point, count it
            if (p1.x == p2.x || point.x <= xIntersection)
            {
              intersections++;
            }
          }
        }
      }
    }

    // Odd intersections = inside, Even intersections = outside
    return (intersections % 2) != 0;
  }
}
