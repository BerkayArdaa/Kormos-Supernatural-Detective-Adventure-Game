using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public Image crosshairImage;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click pressed
        {
            ShowCrosshair();
        }
        if (Input.GetMouseButtonUp(1)) // Right-click released
        {
            HideCrosshair();
        }
    }

    void ShowCrosshair()
    {
        crosshairImage.enabled = true;  // Show the crosshair
    }

    void HideCrosshair()
    {
        crosshairImage.enabled = false; // Hide the crosshair
    }
}
