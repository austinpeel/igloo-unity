using UnityEngine;
using TMPro;

public class ParametersDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textQ;


    public void SetQValueText(float q)
    {
        textQ.text = q.ToString("0.00");
    }

}
