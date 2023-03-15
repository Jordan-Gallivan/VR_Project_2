using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableItem : MonoBehaviour
{
    public bool itemIsSelected = false;
    public bool moveItem = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!itemIsSelected && moveItem)
        {
            this.gameObject.transform.Translate(new Vector3(0.0f, 0f, 0.01f));
        }
    }
}
