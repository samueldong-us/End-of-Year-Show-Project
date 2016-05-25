﻿using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    public UIOpenCloseAnimator HUD;
    public GameObject hudCanvas;
    public ParameterizedAnimator strengthBar;
    public float StrengthChangeTime;
    private Dictionary<string, MinigameState> minigames;
    private string currentMinigame;
    private State currentState;
    private float currentStrength;

    private enum MinigameState { New, Active, Finished }

    public void EnteredTarget(string target)
    {
        if (target == "Target 4")
        {
            if (minigames["Target 1"] != MinigameState.Finished || minigames["Target 2"] != MinigameState.Finished || minigames["Target 3"] != MinigameState.Finished)
            {
                return;
            }
        }
        currentMinigame = target;
        switch (minigames[target])
        {
            case MinigameState.Active:
                currentState = State.GainingStrength;
                GameObject.Find(currentMinigame).GetComponentInChildren<MinigameOpenCloseAnimator>().Open();
                GameObject.Find(currentMinigame).GetComponentInChildren<MinigameManager>().GameResume();
                break;

            case MinigameState.New:
            case MinigameState.Finished:
                GameObject.Find(target).GetComponentInChildren<UIOpenCloseAnimator>().Open();
                break;
        }
    }

    public void EnteredDimension()
    {
        GameObject.Find(currentMinigame).GetComponentInChildren<UIOpenCloseAnimator>().Close();
        GameObject.Find(currentMinigame).GetComponentInChildren<MinigameOpenCloseAnimator>().Open();
        GameObject.Find(currentMinigame).GetComponentInChildren<MinigameManager>().GameStart();
        minigames[currentMinigame] = MinigameState.Active;
        currentState = State.GainingStrength;
        hudCanvas.SetActive(true);
        HUD.Open();
    }

    public void LeftTarget(string target)
    {
        switch (minigames[target])
        {
            case MinigameState.Active:
                currentState = State.LosingStrength;
                GameObject.Find(currentMinigame).GetComponentInChildren<MinigameOpenCloseAnimator>().Close();
                GameObject.Find(currentMinigame).GetComponentInChildren<MinigameManager>().GamePause();
                break;

            case MinigameState.New:
            case MinigameState.Finished:
                GameObject.Find(target).GetComponentInChildren<UIOpenCloseAnimator>().Close();
                break;
        }
    }

    private void Start()
    {
        minigames = new Dictionary<string, MinigameState>();
        minigames["Target 1"] = MinigameState.New;
        minigames["Target 2"] = MinigameState.New;
        minigames["Target 3"] = MinigameState.New;
        minigames["Target 4"] = MinigameState.New;
        HUD.OnClose = () => hudCanvas.SetActive(false);
    }

    private void Update()
    {
        if (currentMinigame != null && minigames[currentMinigame] == MinigameState.Active)
        {
            switch (currentState)
            {
                case State.GainingStrength:
                    currentStrength += Time.deltaTime / StrengthChangeTime;
                    if (currentStrength > 1.0f)
                    {
                        currentStrength = 1.0f;
                        currentState = State.Neutral;
                    }
                    break;

                case State.LosingStrength:
                    currentStrength -= Time.deltaTime / StrengthChangeTime;
                    if (currentStrength < 0.0f)
                    {
                        currentStrength = 0.0f;
                        currentState = State.Neutral;
                        GameObject.Find(currentMinigame).GetComponentInChildren<MinigameOpenCloseAnimator>().Close();
                        GameObject.Find(currentMinigame).GetComponentInChildren<MinigameManager>().GameReset();
                        minigames[currentMinigame] = MinigameState.New;
                        HUD.Close();
                        currentMinigame = null;
                    }
                    break;
            }
            strengthBar.SetParameter(currentStrength);
        }
    }

    private enum State { Neutral, LosingStrength, GainingStrength }
}