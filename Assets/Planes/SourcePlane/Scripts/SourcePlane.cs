using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SourcePlane : Plane
{
    [SerializeField] private InteractableEllipseUI interactEllipseUI;

    // Brightness Map
    [Header("Brightness Map")]
    [SerializeField] private Color colorBrightnessMap = Color.red;
    [SerializeField] private Image brightnessMap;
    [SerializeField] private bool displayBrightnessMap = true;
    [SerializeField] private Image brightnessColorScale;
    [SerializeField] private GameObject colorScaleOutline;
    [SerializeField] private bool displayBrightnessColorScale = true;
}
