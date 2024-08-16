using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool invertUIDirection = true;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (invertUIDirection)
        {
            Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position - directionToCamera);


        }
        else transform.LookAt(cameraTransform);


    }


}
