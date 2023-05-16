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
    [SerializeField] private Transform[] targetPoles = new Transform[5];
    [SerializeField] private Transform[] baseBones = new Transform[5];
    [SerializeField] private Transform[] mainJoints = new Transform[5];

    [SerializeField] private GameObject[] fingerIKObjects = new GameObject[5];
    private FastIKFabric[] fingerIKs = new FastIKFabric[5];

    [SerializeField] private List<GameObject> thumbLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> indexLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> middleLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> ringLinkPos = new List<GameObject>();
    [SerializeField] private List<GameObject> pinkyLinkPos = new List<GameObject>();

    [SerializeField] private List<GameObject> deformables = new List<GameObject>();
    private List<Collider> deformableColliders = new List<Collider>();

    private bool[] alreadyColliding = {false, false, false, false, false};
    private HandPositions handPosScript;

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
        foreach (GameObject obj in deformables)
        {
            deformableColliders.Add(obj.GetComponent<Collider>());
        }
        handPosScript = this.GetComponent<HandPositions>();
    }

    void Update()
    {
        bool isColliding = false;
        for (int i = 0; i < 5; i++)
        {
            fingerMovementDirection[i] = (fingerTips[i].transform.position - prevTipsPosition[i]);
            //prevTipsPosition[i] = fingerTips[i].transform.position;
            //Collider bunnyCol = GameObject.FindWithTag("Deformable").GetComponent<Collider>();
            foreach (Collider bunnyCol in deformableColliders){
                if(bunnyCol.bounds.Contains(baseBones[i].position)){
                    handPosScript.toggleBase(i, false);
                }
                if (bunnyCol.bounds.Contains(fingerTips[i].transform.position))
                {
                    isColliding = true;
                    Ray ray = new Ray(prevTipsPosition[i], fingerMovementDirection[i]);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Object")))
                    {
                        hit.point += 0.00001f * hit.normal;
                        targetTransforms[i].position = hit.point;
                        if(!alreadyColliding[i]){
                            alreadyColliding[i] = true;
                            targetPoles[i].position = GetPerpendicularIntersection(hit.point, baseBones[i].position, mainJoints[i].position);
                        }
                        /*List<Hand> _allHands = Hands.Provider.CurrentFrame.Hands;
                        Hand _hand = _allHands[0];
                        targetPoles[i].position = fingerTips[i].transform.position + _hand.Fingers[i].Direction.normalized * 0.5f;*/
                        toggleReverseKinematics(i, true);
                        if (forcePerFinger[i] <= 3.5f) forcePerFinger[i] = Mathf.Max(forcePerFinger[i] + 0.1f * fingerMovementDirection[i].magnitude, 3.5f);
                        MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
                        if (deformer != null)
                        {
                            deformer.AddDeformingForce(hit.point, forcePerFinger[i]);
                        }
                    }
                }
            }

            if(!isColliding){
                alreadyColliding[i] = false;
                prevTipsPosition[i] = fingerTips[i].transform.position;
                forcePerFinger[i] = 0f;
                toggleReverseKinematics(i, false);
            }
        }
        
    }

    void toggleReverseKinematics(int i, bool state)
    {
        fingerIKs[i].enabled = state;
        //bool first = true;
        if(i == 0){
            handPosScript.toggleThumb(!state);
            foreach (GameObject obj in thumbLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 1){
            handPosScript.toggleIndex(!state);
            foreach (GameObject obj in indexLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 2){
            handPosScript.toggleMiddle(!state);
            foreach (GameObject obj in middleLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 3){
            handPosScript.toggleRing(!state);
            foreach (GameObject obj in ringLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        else if(i == 4){
            handPosScript.togglePinky(!state);
            foreach (GameObject obj in pinkyLinkPos){
                /*if(first){
                    first = false;
                    continue;
                }*/
                obj.GetComponent<LinkPosition>().SetActive(!state);
            }
        }
        
    }

    Vector3 GetPerpendicularIntersection(Vector3 position1, Vector3 position2, Vector3 intersectionPosition)
    {
        // Calculate the vector between the two given positions
        Vector3 direction = position2 - position1;
        
        // Calculate the intersection of the vector and the plane passing through the intersection position and perpendicular to the vector
        Vector3 intersection = intersectionPosition - Vector3.Dot(intersectionPosition - position1, direction.normalized) * direction.normalized;

        // Calculate the vector from the intersection position to the calculated intersection point
        Vector3 toIntersection = intersection - intersectionPosition;

        // Calculate the perpendicular vector to the original vector and the vector from the intersection position to the calculated intersection point
        Vector3 perpendicular = Vector3.Cross(direction.normalized, toIntersection.normalized);

        // Calculate the distance between the intersection of the first vector and the perpendicular line
        float distance = Vector3.Dot(intersectionPosition - position1, direction.normalized);

        // Calculate the position that is at least 2 distance units away from the intersection point towards the given position
        //Vector3 result = intersectionPosition + Mathf.Max(0.5f, 0.5f - distance) * direction.normalized + 0.5f * toIntersection.magnitude * perpendicular.normalized;

        // apply the perpendicular vector to the position1 with a distance of 0.5f
        Vector3 result = position1 + 0.5f * perpendicular.normalized;

        return result;
    }
}
