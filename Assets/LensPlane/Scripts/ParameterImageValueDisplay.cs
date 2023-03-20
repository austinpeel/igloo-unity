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
        mainRect.anchoredPosition = newPosition + offsetPosition;
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
}
