
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarController : MonoBehaviour
{
    [SerializeField] 
    private List<GameObject> carModels = new List<GameObject>();

    public bool isleft = false;

    private void Awake()
    {
        foreach (var car in carModels)
            car.SetActive(false);
            
        carModels[Random.Range(0, carModels.Count)].SetActive(true);
    }



    public void MoveTo(List<Vector3> paths, GridController gridController)
    {
        var pathArray = paths.ToArray();
        
        /*
         * Here we get the paths we will move to the grid. We execute the path with InOutSine as file requested.
         * Than we inform the grid controller if we match with it or not after we arrive.
         *
         */

        transform.DOPath(pathArray, 2, PathType.CatmullRom, PathMode.Full3D, 10, Color.red).SetLookAt(0.01f).SetEase(Ease.InOutSine).SetDelay(.5f).OnComplete(()=>gridController.CheckStatus(isleft));

    }

    private void OnTriggerEnter(Collider collider)
    {
        
        var parent = collider.attachedRigidbody;
        if(parent)
        {
            
            if (parent.TryGetComponent(out CarController carController))
            {
                if(carController.isleft != isleft)
                {
                    
                    /*
                     * If we crash with opposite color the game fails.
                     * We cannot crush with same color because the is
                     * a delay between buttons
                     */
                    UIManager.Instance.FailGame();
                    Crash();
                    carController.Crash();
                }
            }
        }
    }

    public void Crash()
    {
        transform.DOPause();
    }
}
