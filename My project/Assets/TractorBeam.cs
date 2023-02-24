using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    // Initialize boolean control values
    private bool itemSelected = false;
    private bool tractorBeamActive = false;
    private bool summonActive = false;
    private bool rotateItem = false;
    private bool moveItem = false;

    // public GameObject ConeCast;
    // private Collider collider;

    public GameObject player;
    private GameObject selectedItem;
    public LayerMask mask;

    public float tractorBeamSpeed = 1;
    
    /** Outside to-do's
     * Make a class for movable objects that has a toggle for organic movement/summoning
     * 
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
     */
    
    
    // Start is called before the first frame update
    void Start()
    {
        this.selectedItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Initializes the tractor beam by sphere casting in the direction of the controller
        // the closest "summonable" item is set as this.itemSelected if one exists
        if ( true /* tractorBeamActive */ )
        {
            RaycastHit[] tractorBeamObjs =
                Physics.SphereCastAll(this.player.transform.position, 
                    1.0f, this.player.transform.forward);
            float nearest = Mathf.Infinity;
            
            // iterate through hits and determine nearest "summonable" object
            foreach (RaycastHit hit in tractorBeamObjs)
            {
                if (hit.distance < nearest && hit.collider.CompareTag("summonable"))
                {
                    nearest = hit.distance;
                    this.selectedItem = hit.collider.gameObject;
                    this.itemSelected = true;
                }
            }
        }
        
        // summons the item by transforming it in the direction of the player
        if ( itemSelected && tractorBeamActive && summonActive ) 
        {
            
            this.selectedItem.transform.position = Vector3.MoveTowards(this.selectedItem.transform.position, 
                this.player.transform.position, Time.deltaTime * tractorBeamSpeed);
            
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.selectedItem.GetComponent<MoveableItem>().itemIsSelected = true;
        }
        
    }

    public void ActivateTractorBeam()
    {
        tractorBeamActive = true;
    }

    public void DeactivateTractorBeam()
    {
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
