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
    private bool itemBeingMoved;
    private bool itemBeingRotated;
    
    // Initialize player characteristics
    public GameObject player;
    private Vector3 playerPos;
    
    // Initialize Selected Item Characteristics
    private GameObject selectedItem;
    private MovableItem movableItem;    // MovableItem Object to toggle organic movement
    private Vector3 itemPos;            // current position of selected Item
    private Quaternion itemOrientation; // current orientation of the selected Item
    private Quaternion itemRotatedOrientation;  // orientation user wants to rotate the item to
    private Vector3 itemDestPos;    // position user wants to move object to
    private float distToItem;   // magnitude of vector between player and item

    public GameObject beamRender;
    public GameObject glowSphere;
    private Color normalGlow = new Color(1.0f, 1.0f, 1.0f, 1.1568f);
    private Color SelectedGlow = new Color(1.0f, 0.514f, 0.514f, 1.1568f);
    
    public float tractorBeamSpeed = 100f;
    
    public LayerMask mask;
    

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
        itemSelected = false;
        tractorBeamActive = false;
        summonActive = false;
        itemBeingMoved = false;
        itemBeingRotated = false;
        
        playerPos = this.player.transform.position;
        
        selectedItem = null;
        movableItem = null;
        itemDestPos = Vector3.zero;
        distToItem = 0f;
        
        beamRender.GetComponent<MeshRenderer>().enabled = false;
        glowSphere.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        playerPos = this.player.transform.position;
        
        // Initializes the tractor beam by sphere casting in the direction of the controller
        // the closest "summonable" item is set as this.itemSelected if one exists
        if ( tractorBeamActive && !itemSelected)
        {
            RaycastHit[] tractorBeamObjs =
                Physics.SphereCastAll(this.player.transform.position, 
                    1.0f, this.player.transform.forward);
            float nearest = Mathf.Infinity;
// need to deselect item if tractor beam comes off it
// OR need a way to "secure" selected item
            // iterate through hits and determine nearest "summonable" object
            foreach ( RaycastHit hit in tractorBeamObjs )
            {
                if ( (hit.distance < nearest) && (hit.collider.gameObject != this.selectedItem) && 
                    hit.collider.CompareTag("summonable") )
                {
                    // "release" previously selected item
                    if ( movableItem ) movableItem.itemIsSelected = false;
                    
                    nearest = hit.distance; // reset comparrison distance
                    
                    selectedItem = hit.collider.gameObject; // pull hit item into selectedItem
                    movableItem = this.selectedItem.GetComponent<MovableItem>();    // pull MoveableItem Object from hit item
                    
                    // update nearest Item characteristics
                    itemPos = selectedItem.transform.position;
                    itemOrientation = this.selectedItem.transform.rotation;
                    distToItem = Vector3.Distance(playerPos, itemPos);
                    
                    // add glow sphere to nearest item
                    glowSphere.SetActive(false);
                    glowSphere.transform.position = itemPos;
                }
            }
        } // end tractorbeam update
        
        // summons the item by transforming it in the direction of the player
        if ( itemSelected && summonActive )
        {
            itemPos = selectedItem.transform.position;
            distToItem = Vector3.Distance(playerPos, itemPos);

            itemPos = Vector3.MoveTowards(itemPos, playerPos, 
                Time.deltaTime * tractorBeamSpeed * Mathf.Log(distToItem));
            itemOrientation = this.selectedItem.transform.rotation;
            // time.deltaTime how much time elapses between frames
        }
        
        // move item towards intended location
        if (itemBeingMoved)
        {
            itemPos = Vector3.MoveTowards(itemPos, itemDestPos, Time.deltaTime * 100);
            itemOrientation = this.selectedItem.transform.rotation;
            if (itemPos == itemDestPos)
            {
                itemBeingMoved = false;
            }
        }

        // if (itemBeingRotated)
        // {
        //     itemOrientation = itemRotatedOrientation;
        // }
            

        if (Input.GetKeyDown(KeyCode.Space)) this.ActivateTractorBeam();
        if (Input.GetKeyUp(KeyCode.Space)) this.DeactivateTractorBeam();
        if (Input.GetKeyDown(KeyCode.O)) this.SummonObject();
        if (Input.GetKeyUp(KeyCode.O)) this.DeactivateSummon();
        
    } // end update()

    public void ActivateTractorBeam()
    {
        tractorBeamActive = true;
        beamRender.GetComponent<MeshRenderer>().enabled = true;
    }

    public void DeactivateTractorBeam()
    {
        tractorBeamActive = false;
        beamRender.GetComponent<MeshRenderer>().enabled = false;
    }

    public void SelectNearestItem()
    {
        itemSelected = true;
        movableItem.itemIsSelected = true;
        var glowRenderer = glowSphere.GetComponent<Renderer>();
        glowRenderer.material.SetColor("_Color", SelectedGlow);
    }

    public void DeSelectNearestItem()
    {
        itemSelected = false;
        movableItem.itemIsSelected = false;
        var glowRenderer = glowSphere.GetComponent<Renderer>();
        glowRenderer.material.SetColor("_Color", normalGlow);
    }

    public void SummonObject()
    {
        this.summonActive = true;
    }

    public void DeactivateSummon()
    {
        this.summonActive = false;
    }

    public void WaiveOff()
    {
        selectedItem.transform.Translate(Vector3.right * 100f, player.transform);
        movableItem.itemIsSelected = false;
        itemSelected = false;
        selectedItem = null;
        
        var glowRenderer = glowSphere.GetComponent<Renderer>();
        glowRenderer.material.SetColor("_Color", normalGlow);
        glowSphere.SetActive(false);
    }

    public void MoveItem(Vector3 controllerPos)
    {
        if (tractorBeamActive && itemSelected)
        {
            // normalize vector from player to hand
            // get magnitude of vector from player to item curr pos
            // multiple vector from player to hand by magnitude of item
            // move item
            var playerToController = (controllerPos - playerPos).normalized;
            var playerToItemMag = (itemPos - playerPos).magnitude;
            itemDestPos = playerToController * playerToItemMag;
            itemBeingMoved = true;
        }
        else
        {
            itemBeingMoved = false;
        }
    }

    public void EndMovement()
    {
        itemBeingMoved = false;
    }

    public void RotateItem(Quaternion rotation)
    {
        if (tractorBeamActive && itemSelected)
        {
            itemBeingRotated = true;
            itemRotatedOrientation = rotation;
/////////////////////////// is this okay??
/// need to calc difference and rotate appropriately 
            itemOrientation = itemRotatedOrientation;
        }
        else
        {
            this.itemSelected = true;
            itemBeingRotated = false;
        }
    }

    public void EndRotation()
    {
        itemBeingRotated = false;
    }
    
    
}
