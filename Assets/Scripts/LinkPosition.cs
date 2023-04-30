using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPosition : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] public bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(active){
            this.transform.position = targetTransform.position;
            this.transform.rotation = targetTransform.rotation;
            this.transform.localScale = targetTransform.localScale/*/5f*/;
        }
    }

    public void SetActive(bool value){
        active = value;
    }
}
