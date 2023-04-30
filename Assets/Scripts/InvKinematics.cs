using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvKinematics : MonoBehaviour
{
    public Transform[] bones;
    public Transform[] joints;

    public Transform target;

    public float boneLength = 1.0f;
    public float tolerance = 0.01f;
    public int maxIterations = 10;

    private void Update()
    {
        // Calculate the distances between the joints and the target
        float d1 = Vector3.Distance(joints[0].position, target.position);
        float d2 = Vector3.Distance(joints[1].position, target.position);
        float d3 = Vector3.Distance(joints[2].position, target.position);

        // Check if the target is out of reach
        if (d1 + d2 + d3 > boneLength * 3)
        {
            // If the target is out of reach, move the finger as close as possible to the target
            MoveFinger(target.position);
            return;
        }

        // Use the iterative IK solver to move the finger towards the target
        for (int i = 0; i < maxIterations; i++)
        {
            // Calculate the distances between the joints and the target
            d1 = Vector3.Distance(joints[0].position, target.position);
            d2 = Vector3.Distance(joints[1].position, target.position);
            d3 = Vector3.Distance(joints[2].position, target.position);

            // Check if the distance to the target is within tolerance
            if (d3 < tolerance)
            {
                break;
            }

            // Calculate the angles of the joints
            float a1 = Mathf.Acos(Mathf.Clamp((d1 * d1 - d2 * d2 + boneLength * boneLength) / (2 * d1 * boneLength), -1f, 1f));
            float a2 = Mathf.Acos(Mathf.Clamp((d2 * d2 - d3 * d3 + boneLength * boneLength) / (2 * d2 * boneLength), -1f, 1f));

            // Calculate the directions from the joints to the target
            Vector3 dir1 = (target.position - joints[0].position).normalized;
            Vector3 dir2 = (target.position - joints[1].position).normalized;

            // Calculate the rotations of the joints
            Quaternion rot1 = Quaternion.LookRotation(dir1, bones[0].up) * Quaternion.AngleAxis(a1 * Mathf.Rad2Deg, Vector3.forward);
            Quaternion rot2 = Quaternion.LookRotation(dir2, bones[1].up) * Quaternion.AngleAxis(a2 * Mathf.Rad2Deg, Vector3.forward);

            // Apply the rotations to the bones
            bones[0].rotation = rot1;
            bones[1].rotation = rot2;
            bones[2].rotation = Quaternion.LookRotation((target.position - joints[2].position).normalized, bones[2].up);
        }
    }

    private void MoveFinger(Vector3 targetPosition)
    {
        // Move the finger as close as possible to the target position
        Vector3 dir = (targetPosition - joints[0].position).normalized;
        float length = boneLength * 3;
        joints[0].position = joints[0].position + dir * length;
        joints[1].position = joints[1].position + dir * length;
        joints[2].position = joints[2].position + dir * length;
    }
}