using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationShow : MonoBehaviour
{
    public float showTime = 2;
    float showtimer;
    private void OnEnable()
    {
        showtimer = 0;
    }

    private void Update()
    {
        showtimer += Time.deltaTime;
        if (showtimer >= showTime)
        {
            gameObject.SetActive(false);
        }
    }
}
