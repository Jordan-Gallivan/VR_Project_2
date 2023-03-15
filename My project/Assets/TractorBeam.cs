using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    // Initialize boolean control values
    private bool itemSelected;
    private bool tractorBeamActive;
    private bool summonActive;
    private bool rotateItem;
    private bool moveItem;
    
    public GameObject player;
    public LayerMask mask;
    
    private GameObject selectedItem;
    
    private MovableItem movableItem;

    public float tractorBeamSpeed = 100;

    /** Outside to-do's
     * Make a class for movable objects that has a toggle for organic movement/summoning
     * assign sphere cast to originate from cylinder
     */

    /** Overall Logic
     * left hand -> item selection
     *      grasp = tractor beam on/off  
     *      trigger = summon nearest object
     * 
     * right hand -> item manipulation
     *      hand waive = move item from cone cast and no longer summon
     *          re-initiate summon method
     *          ??? how to return it to it's original movement
     *      grasp = select item for manipulation -> stop all movemenet
     *          -> hand controller moves object left/right
     *              ??? how to determine movement of hand controller in C#
     *          ??? how to move it in depth (forward/backward)
     *      trigger = rotate object
     *          -> tied to rotation of hand controller
     *              ??? how to determine rotation of hand controller in C#
     * https://docs.unity3d.com/Manual/xr_input.html
     */

    void Start()
    {
        // initialize boolean control values
        this.itemSelected = false;
        this.tractorBeamActive = false;
        this.summonActive = false;
        this.rotateItem = false;
        this.moveItem = false;
        
        this.selectedItem = null;
        this.movableItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Initializes the tractor beam by sphere casting in the direction of the controller
        // the closest "summonable" item is set as this.itemSelected if one exists
        if ( tractorBeamActive )
        {
            RaycastHit[] tractorBeamObjs =
                Physics.SphereCastAll(this.player.transform.position, 
                    1.0f, this.player.transform.forward);
            float nearest = Mathf.Infinity;
            
            // iterate through hits and determine nearest "summonable" object
            foreach ( RaycastHit hit in tractorBeamObjs )
            {
                if ( (hit.distance < nearest) && (hit.collider.gameObject != this.selectedItem) && 
                    hit.collider.CompareTag("summonable") )
                {
                    if ( movableItem ) movableItem.itemIsSelected = false;
                    nearest = hit.distance;
                    this.selectedItem = hit.collider.gameObject;
                    this.itemSelected = true;
                    movableItem = this.selectedItem.GetComponent<MovableItem>();
                    movableItem.itemIsSelected = true;
                }
            }
        } // end tractorbeam update
        
        // summons the item by transforming it in the direction of the player
        if ( itemSelected && tractorBeamActive && summonActive )
        {
            Transform itemPos = this.selectedItem.transform;
            Transform playerPos = this.player.transform;
            float scaledDistance =
                Mathf.Exp(Vector3.Distance(playerPos.position, itemPos.position));
            
            itemPos.position = Vector3.MoveTowards(itemPos.position, 
                playerPos.position, Time.deltaTime * tractorBeamSpeed /** scaledDistance*/);
            // time.deltaTime how much time elapses between frames
        }

        if (Input.GetKeyDown(KeyCode.Space)) this.ActivateTractorBeam();
        if (Input.GetKeyUp(KeyCode.Space)) this.DeactivateTractorBeam();
        if (Input.GetKeyDown(KeyCode.O)) this.SummonObject();
        if (Input.GetKeyUp(KeyCode.O)) this.DeactivateSummon();
        
    } // end update()

    public void ActivateTractorBeam()
    {
        tractorBeamActive = true;
    }

    public void DeactivateTractorBeam()
    {
        movableItem.itemIsSelected = false;
        tractorBeamActive = false;
        itemSelected = false;
        this.selectedItem = null;
    }

    public void SummonObject()
    {
        this.summonActive = true;
    }

    public void DeactivateSummon()
    {
        this.summonActive = false;
    }
    
    
}
