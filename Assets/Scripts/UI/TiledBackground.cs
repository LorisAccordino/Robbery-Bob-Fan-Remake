using UnityEngine;
using UnityEngine.UI;

public class TiledBackground : MonoBehaviour
{
    public RawImage rawImage;    // Assign the RawImage
    public Vector2 scrollSpeed = new Vector2(0, -0.5f); // Movement speed in UV units

    private Vector2 uvOffset;

    void Update()
    {
        if(rawImage == null) return;

        uvOffset += scrollSpeed * Time.deltaTime;
        rawImage.uvRect = new Rect(uvOffset, rawImage.uvRect.size);
    }
}