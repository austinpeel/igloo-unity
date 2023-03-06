using UnityEngine;
using TMPro;

public class ParametersDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textQ;
    [SerializeField] private TextMeshProUGUI textX;
    [SerializeField] private TextMeshProUGUI textY;


    public void SetQValueText(float q)
    {
        textQ.text = q.ToString("0.00");
    }

    public void SetPositionCenterText(Vector2 position)
    {
        textX.text = position.x.ToString("0.0");
        textY.text = position.y.ToString("0.0");
    }

}
