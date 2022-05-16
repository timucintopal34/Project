using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public enum LineType
    {
        Left,
        Mid,
        Right
    }

    [SerializeField]
    public LineType _lineType;

    [Space]
    [SerializeField] 
    private bool isLeftcar = false;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public GameObject tick;

    private void Start()
    {
        Init();
        tick.transform.DOLookAt(Camera.main.transform.position, 0);
    }
    
    /*
     * Grid colors itself depending on its color.
     */

    private void Init()
    {
        if (isLeftcar)
            _spriteRenderer.color = Color.magenta;
        else
            _spriteRenderer.color = Color.yellow;

        ; 
    }


    // Update is called once per frame
    public void CheckStatus(bool isLeftCar)
    {
        
        if (isLeftCar == this.isLeftcar)
            tick.SetActive(true);
        else // If wrong grid
            UIManager.Instance.FailGame();

    }

}
