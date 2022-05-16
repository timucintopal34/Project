using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] 
    private List<GridController> gridsForLeft = new List<GridController>();
    [SerializeField] 
    private List<GridController> gridsForRight = new List<GridController>();

    private List<GridController> _leftGrids = new List<GridController>();
    private List<GridController> _midGrids = new List<GridController>();
    private List<GridController> _rightGrids = new List<GridController>();

    [Space]
    [SerializeField] 
    private List<GridController> leftQueue = new List<GridController>();

    public bool leftCanCrossMid = false;
    
    [Space]
    [SerializeField] 
    private List<GridController> rightQueue = new List<GridController>();
    
    public bool rightCanCrossMid = false;

    private void Start()
    {
        LeftInit();
        LeftQueue();
        
        RightInit();
        RightQueue();
    }
    
    /*
     * This is the part which mostly took my time. There could be a better algorithm here.
     * I just listed conditions for the right queue. Is to know the grids left or right car can move.
     * The rest of the sorting can be done by the algorithm below.
     *
     * I init left, mid and right grids first seperately for left and right cars. Due to grids positions
     * their order is different.
     *
     * To queue the left, mid and right grids I filter them with conditions such as their positions in z x and
     * their middle or cross opposite line capabilities. I manually enter the left or right cancrossmid but I
     * could make it if the mid lane faces right or left. If faces right like in our level this means it can be
     * crossed from left. Because the entrance of the middle grids Z is bigger than its exit. 
     */

    private void LeftInit()
    {
        gridsForLeft = gridsForLeft.OrderBy(x => x.transform.position.x).ToList();

        var lowest = gridsForLeft[0];
        
        foreach (var grid in gridsForLeft)
            if(Mathf.Abs(grid.transform.position.x - lowest.transform.position.x) < .25f)
            {
                _leftGrids.Add(grid);
                grid._lineType = GridController.LineType.Left;
            }
        
        gridsForLeft = gridsForLeft.OrderByDescending(x => x.transform.position.x).ToList();

        var highest = gridsForLeft[0];
        
        foreach (var grid in gridsForLeft)
            if(Mathf.Abs(grid.transform.position.x - highest.transform.position.x) < .25f)
            {
                _rightGrids.Add(grid);
                grid._lineType = GridController.LineType.Right;
            }

        foreach (var grid in gridsForLeft)
            if(!_leftGrids.Contains(grid) && !_rightGrids.Contains(grid))
            {
                _midGrids.Add(grid);
                grid._lineType = GridController.LineType.Mid;
            }
    }
    
    private void RightInit()
    {
        gridsForRight = gridsForRight.OrderByDescending(x => x.transform.position.x).OrderBy(x => x.transform.position.z).ToList();

        var lowest = gridsForRight[0];
        
        foreach (var grid in gridsForRight)
            if(Mathf.Abs(grid.transform.position.x - lowest.transform.position.x) < .25f)
                _rightGrids.Add(grid);
        
        gridsForRight = gridsForRight.OrderBy(x => x.transform.position.x).OrderBy(x => x.transform.position.z).ToList();

        var highest = gridsForRight[0];
        
        foreach (var grid in gridsForRight)
            if(Mathf.Abs(grid.transform.position.x - highest.transform.position.x) < .25f)
                _leftGrids.Add(grid);

        foreach (var grid in gridsForRight)
            if(!_leftGrids.Contains(grid) && !_rightGrids.Contains(grid))
                _midGrids.Add(grid);

        _midGrids = _midGrids.OrderBy(x => x.transform.position.z).ToList();
    }

    private void LeftQueue()
    {

        for (int i = 0; i <= gridsForLeft.Count-1; i++)
        {
            if (LeftAmountStatus())
            {
                if(leftCanCrossMid)
                {
                    if (RightAmountStatus() && MidAmountStatus())
                    {
                        if (CompareListZ(_leftGrids, _midGrids))
                            AddLeftQueue(_leftGrids);
                        else if (CompareListZ(_rightGrids, _midGrids))
                            AddLeftQueue(_rightGrids);
                        else
                            AddLeftQueue(_midGrids);
                    }
                    else if (RightAmountStatus() && !MidAmountStatus())
                    {
                        if (CompareListZ(_leftGrids, _rightGrids))
                            AddLeftQueue(_leftGrids);
                        else
                            AddLeftQueue(_rightGrids);
                    }
                    else if (!RightAmountStatus() && MidAmountStatus())
                    {
                        if (CompareListZ(_leftGrids, _midGrids))
                            AddLeftQueue(_leftGrids);
                        else
                            AddLeftQueue(_midGrids);
                    }
                    else
                        AddLeftQueue(_leftGrids);
                }
                else
                    AddLeftQueue(_leftGrids);
            }
            else if(RightAmountStatus())
            {
                if(leftCanCrossMid)
                {
                    if (MidAmountStatus())
                    {
                        if (CompareListZ(_rightGrids, _midGrids))
                            AddLeftQueue(_rightGrids);
                        else
                            AddLeftQueue(_midGrids);
                    }
                    else
                        AddLeftQueue(_rightGrids);
                }
                else if(MidAmountStatus())
                {
                    if (CompareListZ(_rightGrids, _midGrids))
                        AddRightQueue(_rightGrids);
                    else
                        AddRightQueue(_midGrids);
                }
                else
                    AddRightQueue(_rightGrids);
            }
            else if(MidAmountStatus())
                AddLeftQueue(_midGrids);
        }
    }
    
    private void RightQueue()
    {
        for (int i = 0; i <= gridsForRight.Count-1; i++)
        {
            if (RightAmountStatus())
            {
                if(rightCanCrossMid)
                {
                    if (LeftAmountStatus() && MidAmountStatus())
                    {
                        if (CompareListZ(_rightGrids, _midGrids))
                            AddRightQueue(_rightGrids);
                        else if (CompareListZ(_leftGrids, _midGrids))
                            AddRightQueue(_leftGrids);
                        else
                            AddRightQueue(_midGrids);
                    }
                    else if (LeftAmountStatus() && !MidAmountStatus())
                    {
                        if (CompareListZ(_rightGrids, _leftGrids))
                            AddRightQueue(_rightGrids);
                        else
                            AddRightQueue(_leftGrids);
                    }
                    else if (!LeftAmountStatus() && MidAmountStatus())
                    {
                        if (CompareListZ(_rightGrids, _midGrids))
                            AddRightQueue(_rightGrids);
                        else
                            AddRightQueue(_midGrids);
                    }
                    else
                        AddRightQueue(_rightGrids);
                }
                else
                    AddRightQueue(_rightGrids);
            }
            else if(LeftAmountStatus())
            {
                if(rightCanCrossMid)
                {
                    if (MidAmountStatus())
                    {
                        if (CompareListZ(_leftGrids, _midGrids))
                            AddRightQueue(_leftGrids);
                        else
                            AddRightQueue(_midGrids);
                    }
                    else
                        AddRightQueue(_leftGrids);
                }
                else if(MidAmountStatus())
                {
                    if (CompareListZ(_leftGrids, _midGrids))
                        AddRightQueue(_leftGrids);
                    else
                        AddRightQueue(_midGrids);
                }
                else
                    AddRightQueue(_leftGrids);
            }
            else if(MidAmountStatus())
                AddRightQueue(_midGrids);
        }
    }

    private bool CompareListZ(List<GridController> list1, List<GridController> list2)
    {
        return list1[0].transform.position.z <= list2[0].transform.position.z;
    }

    private void AddLeftQueue(List<GridController> addFrom)
    {
        leftQueue.Add(addFrom[0]);
        addFrom.RemoveAt(0);
    }
    
    private void AddRightQueue(List<GridController> addFrom)
    {
        rightQueue.Add(addFrom[0]);
        addFrom.RemoveAt(0);
    }
    
    private bool LeftAmountStatus()
    {
        return _leftGrids.Count>0;
    }
    private bool RightAmountStatus()
    {
        return _rightGrids.Count>0;
    }
    private bool MidAmountStatus()
    {
        return _midGrids.Count>0;
    }

    public GridController GetLeftList()
    {
        if (leftQueue.Count == 0)
            return null;
        var temp = leftQueue[0];
        leftQueue.Remove(temp);
        rightQueue.Remove(temp);
        return temp;
    }
    
    public GridController GetRightList()
    {
        if (rightQueue.Count == 0)
            return null;
        var temp = rightQueue[0];
        rightQueue.Remove(temp);
        leftQueue.Remove(temp);
        return temp;
    }
}
