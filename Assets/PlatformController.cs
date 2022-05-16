using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlatformController : Singleton<PlatformController>
{
    [SerializeField] 
    private Transform leftLine;
    [SerializeField] 
    private Transform rightLine;
    
    [SerializeField]
    private Transform leftDoor;
    [SerializeField]
    private Transform rightDoor;

    [Space] 
    [SerializeField] private GameObject leftCarPrefab;
    [SerializeField] private GameObject rightCarPrefab;

    [Space] 
    public List<Transform> leftCarsList = new List<Transform>();
    public List<Transform> rightCarsList = new List<Transform>();

    private bool leftDoorActive = false;
    private bool rightDoorActive = false;
    
    [Space]
    [SerializeField]
    private Transform leftDoorFirstMovement;
    [SerializeField]
    private Transform rightDoorFirstMovement;
    [SerializeField] 
    private Transform middleIntersactionPoint;
    [SerializeField] 
    private Transform twoLaneIntersaction;

    private float gateOpenDuration;
    
    // Start is called before the first frame update

    private void Awake()
    {
        gateOpenDuration = LevelEditor.Instance.GATE_OPEN_DURATION;
    }

    void Start()
    {
        Init();
        
    }

    public void Init()
    {
        for (int j = 0; j < LevelEditor.Instance.CAR_SPAWN_AMOUNT; j++)
        {
            var carLeft = Instantiate(leftCarPrefab, leftLine).transform;
            carLeft.DOLocalMoveZ(j * LevelEditor.Instance.CAR_SPAWN_DISTANCE, 0);
            
            leftCarsList.Add(carLeft);

            carLeft.GetComponent<CarController>().isleft = true;
            
            var carRight = Instantiate(rightCarPrefab, rightLine).transform;
            carRight.DOLocalMoveZ(j * LevelEditor.Instance.CAR_SPAWN_DISTANCE, 0);
            
            rightCarsList.Add(carRight);
            
            carRight.GetComponent<CarController>().isleft = false;
        }
        
    }
    
    /*
     * Algorthm of car to move to the grid. We take our grid and depending on the grid structure
     * add intersactions. For example if we need to go from right to left we can go directly or via
     * middle. We get our right starting point because we start from there than left starting point
     * than go to the opposite grid. If we move to opposite grid we need to take our starting point
     * than the middle intersaction. Before each step we need to check the desired lines we want to
     * move containts available grid or not.
     */

    public void MoveLeft()
    {
        if(leftDoorActive) return;

        leftDoorActive = true;
        
        if(leftCarsList.Count>0)
        {
            var car = leftCarsList[0];
            leftCarsList.RemoveAt(0);

            StartCoroutine(SortCars(leftCarsList, true));
            var target = GridManager.Instance.GetLeftList();
            
            if(target == null)
                return;
            
            var paths = new List<Vector3>();
            paths.Add(leftDoorFirstMovement.position);
            if(target._lineType == GridController.LineType.Mid )
            {
                if (GridManager.Instance.leftCanCrossMid)
                    paths.Add(middleIntersactionPoint.position);
                else
                {
                    paths.Add(rightDoorFirstMovement.position);
                    paths.Add(middleIntersactionPoint.position);
                }
            }
            else if (target._lineType == GridController.LineType.Right)
            {
                if (GridManager.Instance.leftCanCrossMid)
                {
                    paths.Add(middleIntersactionPoint.position);
                    paths.Add(twoLaneIntersaction.position);
                }
                else
                    paths.Add(rightDoorFirstMovement.position);
            }
            
            paths.Add(target.transform.position);
            
            
            car.GetComponent<CarController>().MoveTo(paths, target);
                
        }
        LeftDoorAnimation();
    }

    IEnumerator SortCars(List<Transform> carList, bool isLeft)
    {
        yield return new WaitForSeconds(.75f);

        var targetPos = 0;
        
        foreach (var car in carList)
        {
            car.DOLocalMoveZ(targetPos, .5f);
            targetPos += 2;
            yield return new WaitForSeconds(.1f);
        }

        if (isLeft)
            leftDoorActive = false;
        else
            rightDoorActive = false;
    }

    public void MoveRight()
    {
        if(rightDoorActive) return;

        rightDoorActive = true;
        
        if(rightCarsList.Count>0)
        {
            var car = rightCarsList[0];
            rightCarsList.RemoveAt(0);

            StartCoroutine(SortCars(rightCarsList, false));
            var target = GridManager.Instance.GetRightList();
            
            if(target == null)
                return;
            
            var paths = new List<Vector3>();
            paths.Add(rightDoorFirstMovement.position);
            if(target._lineType == GridController.LineType.Mid )
            {
                if (GridManager.Instance.rightCanCrossMid)
                    paths.Add(middleIntersactionPoint.position);
                else
                {
                    paths.Add(leftDoorFirstMovement.position);
                    paths.Add(middleIntersactionPoint.position);
                }
            }
            else if (target._lineType == GridController.LineType.Left)
            {
                if (GridManager.Instance.rightCanCrossMid)
                {
                    paths.Add(middleIntersactionPoint.position);
                    paths.Add(twoLaneIntersaction.position);
                }
                else
                    paths.Add(leftDoorFirstMovement.position);
            }
            
            paths.Add(target.transform.position);
            
            car.GetComponent<CarController>().MoveTo(paths, target);
                
        }
        RightDoorAnimation();
    }

    #region Animations

    private void LeftDoorAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(leftDoor.DOLocalRotate(Vector3.back * 85f, gateOpenDuration).SetEase(Ease.OutBack).SetDelay(.25f));
        mySequence.AppendInterval(.1f);
        mySequence.SetLoops(2,LoopType.Yoyo);
        // mySequence.OnComplete(() => leftDoorActive = false);
    }

    private void RightDoorAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        
        mySequence.Append(rightDoor.DOLocalRotate(Vector3.back * 85f, gateOpenDuration).SetEase(Ease.OutBack).SetDelay(.25f));
        mySequence.AppendInterval(.1f);
        mySequence.SetLoops(2,LoopType.Yoyo);
        // mySequence.OnComplete(() => rightDoorActive = false);
    }

    #endregion
    
}
