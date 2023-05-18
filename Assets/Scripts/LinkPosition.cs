using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPosition : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] public bool active = true;
    
    private List<Collider> deformableColliders = new List<Collider>();
    private Vector3 prevPos;
    private Vector3 prevScale;
    private Quaternion prevRot;
    // Start is called before the first frame update
    void Start()
    {
        /*GameObject[] deformables = GameObject.FindGameObjectsWithTag("Deformable");
        foreach (GameObject obj in deformables)
        {
            deformableColliders.Add(obj.GetComponent<Collider>());
        }*/
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(active){
            this.transform.position = targetTransform.position;
            this.transform.rotation = targetTransform.rotation;
            this.transform.localScale = targetTransform.localScale/*/5f*/;
        }
        /*foreach (Collider col in deformableColliders)
        {
            if(col.bounds.Contains(this.transform.position)){
                this.transform.position = prevPos;
                this.transform.rotation = prevRot;
                this.transform.localScale = prevScale;
            }
        }
        prevPos = this.transform.position;
        prevRot = this.transform.rotation;
        prevScale = this.transform.localScale;*/
    }

    public void SetActive(bool value){
        active = value;
    }
}
