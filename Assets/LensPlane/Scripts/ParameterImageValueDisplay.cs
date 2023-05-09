using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParameterImageValueDisplay : MonoBehaviour
{
    [SerializeField] private Image parameterImage;
    [SerializeField] private TextMeshProUGUI parameterValueText;
    [SerializeField] private Vector2 offsetValueFromImage = Vector2.right * 20f;
    [SerializeField] private Vector2 offsetPosition = Vector2.zero;
    private RectTransform parameterImageRect;
    private RectTransform parameterValueTextRect;
    private RectTransform mainRect;
    private Vector2 basePosition = Vector2.zero;
    private Vector2 updatedOffset = Vector2.zero;

    private void Awake() 
    {
        parameterImageRect = parameterImage.GetComponent<RectTransform>();
        parameterValueTextRect = parameterValueText.GetComponent<RectTransform>();
        mainRect = GetComponent<RectTransform>();
    }

    private void OnValidate() 
    {
        parameterImageRect = parameterImage.GetComponent<RectTransform>();
        parameterValueTextRect = parameterValueText.GetComponent<RectTransform>();
        mainRect = GetComponent<RectTransform>();

        UpdateOffsetValueText();
    }

    public void SetPosition(Vector2 newPosition)
    {
        if (!mainRect)
        {
            mainRect = GetComponent<RectTransform>();
        }
        basePosition = newPosition + offsetPosition;

        mainRect.anchoredPosition = basePosition + updatedOffset;
    }

    // Inverse the rotation of the ellipse so that the image and the value remains horizontal
    public void SetRotationToZero()
    {
        if (!mainRect)
        {
            mainRect = GetComponent<RectTransform>();
        }
        mainRect.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void SetOffsetValueText(Vector2 newOffset, bool update = true)
    {
        offsetValueFromImage = newOffset;

        if (update)
        {
            UpdateOffsetValueText();
        }
    }

    private void UpdateOffsetValueText()
    {
        float xMaxImage = parameterImageRect.rect.xMax;

        Vector2 lowerRightCornerImagePos = Vector2.right * xMaxImage;

        parameterValueTextRect.anchoredPosition = lowerRightCornerImagePos + offsetValueFromImage; 
    }

    public void SetValueText(string newText)
    {
        parameterValueText.text = newText;
    }

    public void UpdateOffsetQParameter(float ellipseAngle)
    {
        if (!mainRect)
        {
            mainRect = GetComponent<RectTransform>();
        }

        float offset = 0f;
        int quadrant = ((int)(ellipseAngle / 90f));
        float t = ellipseAngle - quadrant * 90f;

        if (quadrant % 2 == 0)
        {
            offset = Mathf.Lerp(0f, mainRect.rect.width * mainRect.localScale.x/2f, t/90f);
        }
        else
        {
            offset = Mathf.Lerp(mainRect.rect.width * mainRect.localScale.x/2f, 0f, t/90f);
        }

        updatedOffset = Vector2.up * offset;

        mainRect.anchoredPosition = basePosition + updatedOffset;
    }

    public void UpdateOffsetEinsteinParameter(float ellipseAngle)
    {
        if (!mainRect)
        {
            mainRect = GetComponent<RectTransform>();
        }
    
        float halfWidth = mainRect.rect.width * mainRect.localScale.x/2f;
        float halfHeight = mainRect.rect.height * mainRect.localScale.x/2f;

        float offset = 0f;
        int quadrant = ((int)(ellipseAngle / 90f));
        float t = ellipseAngle - quadrant * 90f;

        if (quadrant % 2 == 0)
        {
            offset = Mathf.Lerp(halfWidth, halfHeight, t/90f);
        }
        else
        {
            offset = Mathf.Lerp(halfHeight, halfWidth, t/90f);
        }

        updatedOffset = Vector2.right * offset;

        mainRect.anchoredPosition = basePosition + updatedOffset;
    }

    public void UpdateOffsetCenterParameter(float ellipseAngle)
    {
        if (!mainRect)
        {
            mainRect = GetComponent<RectTransform>();
        }

        Vector2 offsetY = new Vector2(Mathf.Sin(ellipseAngle * Mathf.Deg2Rad) * offsetPosition.y, Mathf.Cos(ellipseAngle * Mathf.Deg2Rad) * offsetPosition.y);
        Vector2 offsetX = new Vector2(Mathf.Cos(ellipseAngle * Mathf.Deg2Rad) * offsetPosition.x, -Mathf.Sin(ellipseAngle * Mathf.Deg2Rad) * offsetPosition.x);

        mainRect.anchoredPosition = offsetX + offsetY;
    }
}
