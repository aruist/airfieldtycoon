using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KauppaProgress : MonoBehaviour
{
    public float closeTime = 2;
    public Text text;
    public TextMeshProUGUI textPro;

    private float timer;
    private bool timerRunning;
    private void OnEnable()
    {
        timer = 0;
        timerRunning = false;
        if (text != null)
        {
            text.text = "Progressing...";
        }
        if (textPro != null)
        {
            textPro.SetText("Progressing...");
        }
    }

    public void CloseWithMessage(string message)
    {
        if (text != null)
        {
            text.text = message;
        }
        if (textPro != null)
        {
            textPro.SetText(message);
        }
        timerRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!timerRunning)
            return;
        timer += Time.deltaTime;
        if (timer >= closeTime)
        {
            timerRunning = false;
            gameObject.SetActive(false);
        }
    }
}
