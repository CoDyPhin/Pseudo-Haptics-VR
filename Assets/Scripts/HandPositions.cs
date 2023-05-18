using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leap;
using Leap.Unity;

public class HandPositions : MonoBehaviour
{
    public bool useLeapThumb = true;
    public bool useLeapIndex = true;
    public bool useLeapMiddle = true;
    public bool useLeapRing = true;
    public bool useLeapPinky = true;

    public bool baseThumb = true;
    public bool baseIndex = true;
    public bool baseMiddle = true;
    public bool baseRing = true;
    public bool basePinky = true;


    public LeapProvider leapProvider;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void toggleThumb(bool value){
        useLeapThumb = value;
    }

    public void toggleIndex(bool value){
        useLeapIndex = value;
    }

    public void toggleMiddle(bool value){
        useLeapMiddle = value;
    }

    public void toggleRing(bool value){
        useLeapRing = value;
    }

    public void togglePinky(bool value){
        useLeapPinky = value;
    }

    public void toggleBase(int i, bool value){
        switch(i){
            case 0:
                baseThumb = value;
                break;
            case 1:
                baseIndex = value;
                break;
            case 2:
                baseMiddle = value;
                break;
            case 3:
                baseRing = value;
                break;
            case 4:
                basePinky = value;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UseLeapInfo();
    }

    void UseLeapInfo(){
        List<Hand> _allHands = Hands.Provider.CurrentFrame.Hands;

        if (_allHands.Count > 0)
        {
            Hand _hand = _allHands[0];
            List<Finger> _fingers = _hand.Fingers;
            int finger_num = 0;
            foreach(Transform finger in this.transform){
                //int aux = 0;
                foreach(Transform child in finger){
                    /*if(aux == 0){
                        child.position = _fingers[finger_num].bones[0 + (finger_num != 0 ? 0 : 1)].PrevJoint;
                        child.rotation = _fingers[finger_num].bones[0 + (finger_num != 0 ? 0 : 1)].Rotation;
                        aux++;
                    }
                    else{*/
                        child.position = _fingers[finger_num].bones[1 + (finger_num != 0 ? 0 : 1)].PrevJoint;
                        child.rotation = _fingers[finger_num].bones[1 + (finger_num != 0 ? 0 : 1)].Rotation;
                        int obj_num2 = 0;
                        foreach (Transform bone2 in child){
                            if(obj_num2 == 2){
                                bone2.position = _fingers[finger_num].bones[2 + (finger_num != 0 ? 0 : 1)].PrevJoint;
                                bone2.rotation = _fingers[finger_num].bones[2 + (finger_num != 0 ? 0 : 1)].Rotation;
                                int obj_num3 = 0;
                                foreach (Transform bone3 in bone2){
                                    if(obj_num3 == 2){
                                        if(finger_num == 0){ bone3.position = _fingers[finger_num].TipPosition; }
                                        else{
                                            bone3.position = _fingers[finger_num].bones[3 + (finger_num != 0 ? 0 : 1)].PrevJoint;
                                            bone3.rotation = _fingers[finger_num].bones[3 + (finger_num != 0 ? 0 : 1)].Rotation;
                                            int obj_num4 = 0;
                                            foreach (Transform bone4 in bone3){
                                                
                                                if(obj_num4 == 2){
                                                    bone4.position = _fingers[finger_num].TipPosition;
                                                }
                                                obj_num4++;
                                            }
                                        }
                                    }
                                    obj_num3++;
                                }
                            }
                            obj_num2++;
                        }
                    //}
                }
                finger_num++;
            }
        }
    }
}
