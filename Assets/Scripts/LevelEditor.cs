using UnityEngine;

public class LevelEditor : Singleton<LevelEditor>
{
    /*
     * Very basic level editor for developer and the game artist to change varibles easily. This
     * script can be extended with more variables which will comfort the level design.
     */
    
    [Header("GATE SETUP")]
    public float GATE_OPEN_DURATION = .5F;
    
    [Header("BUTTON SETUP")]
    public bool BUTTON_MOVE_ON_PRESS = true;
    public float BUTTON_PRESS_TIMER = 2.0f;
    public float BUTTON_MOVE_TARGET = -.35F;

    [Header("CAR SETUP")][SerializeField] 
    public int CAR_SPAWN_AMOUNT = 10;
    public float CAR_SPAWN_DISTANCE = 2f;
    public float CAR_MOVEMENT_DURATION = 2F;

    [Header("COLOR SETUP")] [SerializeField]
    public Color leftColor = Color.magenta;
    public Color rightColor = Color.yellow;


}
