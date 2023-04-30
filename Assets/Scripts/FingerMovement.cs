using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leap;
using Leap.Unity;
using DitzelGames.FastIK;

public class FingerMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] fingerTips = new GameObject[5];

    private Vector3[] prevTipsPosition = new Vector3[5];
    private float[] forcePerFinger = {0f, 0f, 0f, 0f, 0f};
    //private bool handDetected = false;
    private Vector3[] fingerMovementDirection = new Vector3[5];

    [SerializeField] private Transform[] targetTransforms = new Transform[5];
    [SerializeField] private GameObject[] fingerIKObjects = new GameObject[5];
    private FastIKFabric[] fingerIKs = new FastIKFabric[5];

    [SerializeField] private List<GameObject> thumbLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> indexLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> middleLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> ringLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> pinkyLinkPos = new List<GameObject>();

    //private List<Transform> fingers = new List<Transform>();
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            prevTipsPosition[i] = fingerTips[i].transform.position;
            fingerMovementDirection[i] = Vector3.zero;
            fingerIKs[i] = fingerIKObjects[i].GetComponent<FastIKFabric>();
        }

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        

    }

    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            fingerMovementDirection[i] = (fingerTips[i].transform.position - prevTipsPosition[i]);
            //prevTipsPosition[i] = fingerTips[i].transform.position;
            Collider bunnyCol = GameObject.FindWithTag("Deformable").GetComponent<Collider>();
            if (bunnyCol.bounds.Contains(fingerTips[i].transform.position))
            {
                Ray ray = new Ray(prevTipsPosition[i], fingerMovementDirection[i]);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Object")))
                {
                    hit.point += 0.00001f * hit.normal;
                    targetTransforms[i].position = hit.point;
                    //targetTransforms[i].rotation = Quaternion.LookRotation(hit.normal);
                    //targetTransforms[i].position = fingerTips[i].transform.position;
                    //fingerIKs[i].activateRK(true);
                    toggleReverseKinematics(i, true);
                    if (forcePerFinger[i] <= 3.5f) forcePerFinger[i] = Mathf.Max(forcePerFinger[i] + 0.1f * fingerMovementDirection[i].magnitude, 3.5f);
                    MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
                    if (deformer != null)
                    {
                        deformer.AddDeformingForce(hit.point, forcePerFinger[i]);
                    }
                }
            }
            else
            {
                prevTipsPosition[i] = fingerTips[i].transform.position;
                forcePerFinger[i] = 0f;
                toggleReverseKinematics(i, false);
                //fingerIKs[i].activateRK(false);
            }
        }
        
    }

    void toggleReverseKinematics(int i, bool state)
    {
        if(state){
            fingerIKs[i].enabled = true;
        }
        else{
            fingerIKs[i].enabled = false;
        }
        //bool first = true;
        if(i == 0){
            this.GetComponent<HandPositions>().toggleThumb(!state);
            foreach (GameObject obj in thumbLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 1){
            this.GetComponent<HandPositions>().toggleIndex(!state);
            foreach (GameObject obj in indexLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 2){
            this.GetComponent<HandPositions>().toggleMiddle(!state);
            foreach (GameObject obj in middleLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 3){
            this.GetComponent<HandPositions>().toggleRing(!state);
            foreach (GameObject obj in ringLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 4){
            this.GetComponent<HandPositions>().togglePinky(!state);
            foreach (GameObject obj in pinkyLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        
    }
}
