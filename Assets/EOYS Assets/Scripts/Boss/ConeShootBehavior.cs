﻿using System.Collections.Generic;
using UnityEngine;

public class ConeShootBehavior : MonoBehaviour
{
    public VelocityMove bullet;
    public Transform bulletParent;
    public Transform player;
    public float radius;
    public float setupTime;
    public float spread;
    public float timeBetweenMoves;
    private State currentState;
    private int index;
    private bool running;
    private List<Vector3> starPoints;
    private Vector3 startingPoint;
    private float time;

    private enum State { Idle, SettingUp, Moving }

    public void Activate()
    {
        currentState = State.SettingUp;
        startingPoint = transform.localPosition;
    }

    public void Pause()
    {
        running = false;
        foreach (VelocityMove velocityMove in bulletParent.GetComponentsInChildren<VelocityMove>())
        {
            velocityMove.Pause();
        }
    }

    public void Reset()
    {
        currentState = State.Idle;
        time = 0.0f;
        running = true;
        List<GameObject> bullets = new List<GameObject>();
        foreach (Transform child in bulletParent)
        {
            bullets.Add(child.gameObject);
        }
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
    }

    public void Resume()
    {
        running = true;
        foreach (VelocityMove velocityMove in bulletParent.GetComponentsInChildren<VelocityMove>())
        {
            velocityMove.Resume();
        }
    }

    private void Fire()
    {
        Vector3 toPlayer = player.transform.localPosition - transform.localPosition;
        toPlayer.Normalize();
        Vector3 uAxis = Vector3.Cross(toPlayer, Vector3.forward).normalized;
        Vector3 vAxis = Vector3.Cross(toPlayer, uAxis);
        for (int wave = 0; wave < 10; wave++)
        {
            float speed = 0.025f * (wave + 1.0f / 11.0f);
            for (int i = 0; i < 72; i++)
            {
                float angle = (float)i / 72 * 2 * Mathf.PI;
                Vector3 target = toPlayer + spread * Mathf.Cos(angle) * uAxis + spread * Mathf.Sin(angle) * vAxis;
                VelocityMove spawnedBullet = Instantiate(bullet);
                spawnedBullet.transform.parent = bulletParent;
                spawnedBullet.transform.localPosition = transform.localPosition;
                spawnedBullet.velocity = speed * target.normalized;
            }
        }
    }

    private void Start()
    {
        currentState = State.Idle;
        time = 0.0f;
        running = true;
        starPoints = new List<Vector3>();
        for (int i = 0; i < 8; i++)
        {
            float angle = Mathf.PI / 2 + i * 2 * Mathf.PI / 8;
            starPoints.Add(new Vector3(radius * Mathf.Cos(angle), transform.localPosition.y, radius * Mathf.Sin(angle)));
        }
        index = 0;
    }

    private void Update()
    {
        if (running)
        {
            switch (currentState)
            {
                case State.SettingUp:
                    time += Time.deltaTime;
                    if (time <= setupTime)
                    {
                        transform.localPosition = Vector3.Lerp(startingPoint, starPoints[index], time / setupTime);
                    }
                    else
                    {
                        time = 0.0f;
                        currentState = State.Moving;
                        Fire();
                    }
                    break;

                case State.Moving:
                    time += Time.deltaTime;
                    if (time <= timeBetweenMoves)
                    {
                        transform.localPosition = Vector3.Lerp(starPoints[index], starPoints[(index + 1) % 8], time / timeBetweenMoves);
                    }
                    else
                    {
                        time = 0.0f;
                        index = (index + 1) % 8;
                        Fire();
                    }
                    break;
            }
        }
    }
}