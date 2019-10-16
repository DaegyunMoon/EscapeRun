using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoblinHealth : MonoBehaviour
{
    public Image currentHealth;
    public GameObject healthCanvas;

    private void Start()
    {
        HideCanvas();
    }

    public void UpdateHealth(double healthPoint, double maxPoint)
    {
        healthCanvas.SetActive(true);
        float ratio = (float)(healthPoint / maxPoint);
        currentHealth.rectTransform.localScale = new Vector3(ratio, 1, 1);
        Invoke("HideCanvas", 5f);
    }

    public void HideCanvas()
    {
        healthCanvas.SetActive(false);
    }
}
