﻿using UnityEngine;

public class Testerino : MonoBehaviour
{
    public BossHealthBar ses;
    private float timeElapsed;

    private void Start()
    {
        timeElapsed = 0.0f;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 1)
        {
            ses.TakeDamage(10);
            timeElapsed = 0.0f;
        }
    }
}