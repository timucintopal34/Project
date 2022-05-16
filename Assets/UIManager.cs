using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] 
    private Transform startGameText;
    [SerializeField] 
    private Transform failGameText;
    [SerializeField] 
    private Transform successGameText;

    public bool gameStarted = false;
    public bool gameEnded = false;
    public UnityAction startGame;
    public UnityAction gameEnd;
    
    /*
     * Ordinary UIManager to control game flow. Here we Invoke some actions to
     * inform some scripts to let them know if which game stage we are so they
     * can take some actions. For example the input be enable when game starts
     * and disabled when game ends.
     */

    private void Start()
    {
        startGameText.gameObject.SetActive(true);
        
    }

    private void Update()
    {
        if(!gameStarted)
            if(Input.GetMouseButtonDown(0))
            {
                gameStarted = true;
                startGameText.gameObject.SetActive(false);
                startGame?.Invoke();
            }
    }

    [ContextMenu("Success")]
    public void SuccessGame()
    {
        if(!gameEnded)
        {
            successGameText.gameObject.SetActive(true);
            gameEnd?.Invoke();
            gameEnded = true;
        }
    }

    [ContextMenu("Fail")]
    public void FailGame()
    {
        if(!gameEnded)
        {
            failGameText.gameObject.SetActive(true);
            gameEnd?.Invoke();
            gameEnded = true;
        }
    }
}
