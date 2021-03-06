﻿using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TrainingMenu : MonoBehaviour
{
    [SerializeField] private Button MenuButton = null;
    [SerializeField] private GameObject MenuPanel = null;
    [SerializeField] private Button PauseButton = null;
    [SerializeField] private GameObject PausePanel = null;
    [SerializeField] private Button ResumeButton = null;
    [SerializeField] private Button ExitButton = null;
    [SerializeField] private GameObject ExitPanel = null;
    [SerializeField] private Button YesButton = null;
    [SerializeField] private Button NoButton = null;
    [SerializeField] private MainCamera mainCamera = null;
    [SerializeField] private GameObject LoadPanel = null;
    private bool ActivePanel = false;

    private void Start()
    {
        LoadPanel.SetActive(true);
        MenuPanel.SetActive(false);
        PausePanel.SetActive(false);
        ExitPanel.SetActive(false);
        MenuButton.onClick.AddListener(MenuScreen);
        PauseButton.onClick.AddListener(Pause);
        ResumeButton.onClick.AddListener(Resume);
        ExitButton.onClick.AddListener(Exit);
        YesButton.onClick.AddListener(Yes);
        NoButton.onClick.AddListener(No);
    }

    public void LoadOk()
    {
        LoadPanel.SetActive(false);
    }

    private void MenuScreen()
    {
        if(!ActivePanel)
        {
            Audio2d.Instance.Play("Ok");
            MenuPanel.SetActive(true);
            ActivePanel = true;
        }
        else
        {
            Audio2d.Instance.Play("Cancel");
            if(MenuPanel.activeSelf)
            {
                MenuPanel.SetActive(false);
            }
            if(PausePanel.activeSelf)
            {
                PausePanel.SetActive(false);
            }
            if(ExitPanel.activeSelf)
            {
                ExitPanel.SetActive(false);
            }
            Time.timeScale = 1.0f;
            ActivePanel = false;
        } 
    }

    private void Pause()
    {
        Audio2d.Instance.Play("Ok");
        Time.timeScale = 0.0f;
        PausePanel.SetActive(true);
        MenuPanel.SetActive(false);
    }

    private void Resume()
    {
        Audio2d.Instance.Play("Cancel");
        Time.timeScale = 1.0f;
        PausePanel.SetActive(false);
        ActivePanel = false;
    }

    private void Exit()
    {
        Audio2d.Instance.Play("Ok");
        ExitPanel.SetActive(true);
        MenuPanel.SetActive(false);
    }

    private void Yes()
    {
        Audio2d.Instance.Play("Ok");
        mainCamera.Target = null;
        ActivePanel = false;
        PhotonNetwork.LeaveRoom();
    }

    private void No()
    {
        Audio2d.Instance.Play("Cancel");
        MenuPanel.SetActive(true);
        ExitPanel.SetActive(false);
    }
}

