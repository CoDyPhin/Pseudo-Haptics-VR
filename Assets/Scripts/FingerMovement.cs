using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leap;
using Leap.Unity;

public class FingerMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] fingerTips;

    private Vector3[] prevTipsPosition = new Vector3[5];
    private float[] forcePerFinger = {0f, 0f, 0f, 0f, 0f};
    [SerializeField] private Vector3[] attemptedPositions = new Vector3[5];
    private bool handDetected = false;
    private Vector3[] fingerMovementDirection = new Vector3[5];
    
    public LeapProvider leapProvider;

    private List<Transform> fingers = new List<Transform>();
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            fingerTips[i].transform.position = attemptedPositions[i];
            prevTipsPosition[i] = fingerTips[i].transform.position;
            fingerMovementDirection[i] = Vector3.zero;
        }
        
        foreach(Transform finger in this.transform){
            fingers.Add(finger);
        }
    }

    void CreateHandWithLeapPositions(){
        List<Hand> _allHands = Hands.Provider.CurrentFrame.Hands;
        if (_allHands.Count > 0)
        {
            Hand _hand = _allHands[0];
            List<Finger> _fingers = _hand.Fingers;
            bool aux = true;
            for(int n = 0; n < 5; n++){
                Transform currentFinger = fingers[n];
                aux = true;
                foreach(Transform child in currentFinger){
                    int counter = 0;
                    foreach(Transform element in child){
                        if(aux){ // joints
                            if(counter==3){
                                element.position = _fingers[n].TipPosition;
                            }
                            else {
                                if(n == 0){ // special case for thumb
                                    element.position = _fingers[n].bones[counter+1].NextJoint;
                                }
                                else{
                                    element.position = _fingers[n].bones[counter].NextJoint;
                                }
                                
                            }
                        }
                        else{ // bones
                            if(n == 0){ // special case for thumb
                                element.position = _fingers[n].bones[counter+1].Center;
                                element.rotation = _fingers[n].bones[counter+1].Rotation;
                                element.localScale = new Vector3(0.01f, _fingers[n].bones[counter+1].Length/2f, 0.01f);
                            }
                            else{
                                element.position = _fingers[n].bones[counter].Center;
                                element.rotation = _fingers[n].bones[counter].Rotation;
                                element.localScale = new Vector3(0.01f, _fingers[n].bones[counter].Length/2f, 0.01f);

                            }
                            element.Rotate(90f, 0f, 0f);

                        }
                        counter++;
                    }
                    aux = false;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        List<Hand> _allHands = Hands.Provider.CurrentFrame.Hands;

        if (_allHands.Count > 0)
        {
            if (!handDetected)
            {
                handDetected = true;
                toggleTips(handDetected);
            }
            Hand _hand = _allHands[0];
            Finger _thumb = _hand.GetThumb();
            Finger _index = _hand.GetIndex();
            Finger _middle = _hand.GetMiddle();
            Finger _ring = _hand.GetRing();
            Finger _pinky = _hand.GetPinky();

            attemptedPositions[0] = _thumb.TipPosition;
            attemptedPositions[1] = _index.TipPosition;
            attemptedPositions[2] = _middle.TipPosition;
            attemptedPositions[3] = _ring.TipPosition;
            attemptedPositions[4] = _pinky.TipPosition;

    
            for (int i = 0; i < 5; i++)
            {
                fingerTips[i].transform.position = attemptedPositions[i];
                fingerMovementDirection[i] = (fingerTips[i].transform.position - prevTipsPosition[i]);
            }
            CreateHandWithLeapPositions();

        }
        else
        {
            if (handDetected)
            {
                handDetected = false;
                toggleTips(handDetected);
            }
        }

    }

    void FixedUpdate()
    {
        if (handDetected)
        {
            for (int i = 0; i < 5; i++)
            {
                Collider bunnyCol = GameObject.FindWithTag("bunny").GetComponent<Collider>();
                if (bunnyCol.bounds.Contains(fingerTips[i].transform.position))
                {
                    Ray ray = new Ray(prevTipsPosition[i], fingerMovementDirection[i]);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Object")))
                    {
                        hit.point += 0.00001f * hit.normal;
                        //fingerTips[i].transform.position = hit.point;
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
                }
            }
        }
    }

    void toggleTips(bool state)
    {
        foreach (var tip in fingerTips)
        {
            tip.SetActive(state);
        }
    }
}
