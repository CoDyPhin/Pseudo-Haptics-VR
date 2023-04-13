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
    private bool[] collidingFingers = {false, false, false, false, false};
    
    public LeapProvider leapProvider;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            fingerTips[i].transform.position = attemptedPositions[i];
            prevTipsPosition[i] = fingerTips[i].transform.position;
            fingerMovementDirection[i] = Vector3.zero;
        }
    }

    public void updateCollidingFingers(int fingerIndex, bool isColliding)
    {
        collidingFingers[fingerIndex] = isColliding;
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
                /*Vector3 movementDirection = (fingerTips[i].transform.position - prevTipsPosition[i]);
                if(movementDirection.normalized.Equals(Vector3.zero)) movementDirection = movementDirection.normalized;
                fingerMovementDirection[i] = Vector3.Lerp(fingerMovementDirection[i], movementDirection, Time.deltaTime * 10f);
                prevTipsPosition[i] = fingerTips[i].transform.position;*/
            }

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
                //Debug.Log("finger " + i + " is colliding: " + collidingFingers[i]);
                //prevTipsPosition[i] = fingerTips[i].transform.position;
                Collider bunnyCol = GameObject.FindWithTag("bunny").GetComponent<Collider>();
                if (bunnyCol.bounds.Contains(fingerTips[i].transform.position))
                //if(collidingFingers[i])
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
