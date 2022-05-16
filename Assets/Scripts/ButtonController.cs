using System;
using DG.Tweening;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] 
    private bool isClicked = false;

    [SerializeField] 
    private Transform button;

    public bool isLeft = false;
    
    private float buttonDuration;
    private bool buttonMoveOnDown;
    private float buttonMoveHeight;

    private bool isActive = false;

    private void Awake()
    {
        buttonDuration = LevelEditor.Instance.BUTTON_PRESS_TIMER;
        buttonMoveOnDown = LevelEditor.Instance.BUTTON_MOVE_ON_PRESS;
        buttonMoveHeight = LevelEditor.Instance.BUTTON_MOVE_TARGET;
    }

    private void Start()
    {
        UIManager.Instance.startGame += () => isActive = true;

        UIManager.Instance.gameEnd += () => isActive = false;
    }

    private void OnMouseDown()
    {
        if(buttonMoveOnDown && isActive)
            MoveButton();
    }

    private void OnMouseUp()
    {
        if(!buttonMoveOnDown)
            MoveButton();
    }
    
    /*
     * When button is pressed the button gives inform to its lane.
     * Then it locks it self for a certain time
     * while button completes its pressed animation.
     */

    private void MoveButton()
    {
        if(isClicked) return;
        isClicked = true;
        
        if(isLeft)
            PlatformController.Instance.MoveLeft();
        else
            PlatformController.Instance.MoveRight();            

        button.DOLocalMoveY(buttonMoveHeight, buttonDuration).SetLoops(2,LoopType.Yoyo).OnComplete(()=>
            StartCoroutine(Helper.InvokeAction(()=>isClicked = false,.5f))
            );
    }

}
