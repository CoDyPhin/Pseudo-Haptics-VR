using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leap;
using Leap.Unity;

public class LeapPositions : MonoBehaviour
{
    public LeapProvider leapProvider;
    private List<Transform> fingers = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform finger in this.transform){
            fingers.Add(finger);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CreateHandWithLeapPositions();
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
}
