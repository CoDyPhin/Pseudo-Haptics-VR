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
    
    public LeapProvider leapProvider;
    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < 5; i++)
        {
            fingerTips[i].transform.position = attemptedPositions[i];
            prevTipsPosition[i] = fingerTips[i].transform.position;
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
                prevTipsPosition[i] = fingerTips[i].transform.position;
                Vector3 movementDirection = attemptedPositions[i] - prevTipsPosition[i];
                Collider bunnyCol = GameObject.FindWithTag("bunny").GetComponent<Collider>();
                if (bunnyCol.bounds.Contains(attemptedPositions[i]))
                {
                    Ray ray = new Ray(prevTipsPosition[i], movementDirection);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        // add an offset to the hit point to make the finger tip stop before getting inside the mesh
                        hit.point += 0.00001f * hit.normal;
                        fingerTips[i].transform.position = hit.point;
                        if (forcePerFinger[i] <= 5.0f) forcePerFinger[i] = Mathf.Max(forcePerFinger[i] + 0.3f * movementDirection.magnitude, 5f);
                        MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
                        if (deformer != null)
                        {
                            //Debug.Log("force: " + forcePerFinger[i]);
                            //Debug.Log("hit point: " + hit.point);
                            deformer.AddDeformingForce(hit.point, forcePerFinger[i]);
                        }
                    }
                }
                else
                {
                    fingerTips[i].transform.position = attemptedPositions[i];
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
