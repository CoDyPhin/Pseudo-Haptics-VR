using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
public class HandTrackingData : MonoBehaviour
{
    public LeapProvider leapProvider;
    
    private void Update()
    {
        List<Hand> _allHands = Hands.Provider.CurrentFrame.Hands;

        foreach (Hand _hand in _allHands)
        {
            //Use _hand to Explicitly get the specified fingers from it
            Finger _thumb = _hand.GetThumb();
            Finger _index = _hand.GetIndex();
            Finger _middle = _hand.GetMiddle();
            Finger _ring = _hand.GetRing();
            Finger _pinky = _hand.GetPinky();

            Debug.Log("Start logging, isLeft = " + _hand.IsLeft);
            Debug.Log(_thumb.TipPosition);
            Debug.Log(_index.TipPosition);
            Debug.Log(_middle.TipPosition);
            Debug.Log(_ring.TipPosition);
            Debug.Log(_pinky.TipPosition);

        }
        
    }

}
