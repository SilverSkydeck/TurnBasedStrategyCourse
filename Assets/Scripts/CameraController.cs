using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    private const float MIN_FOLLOW_OFFSET_Y = 2.8f;
    private const float MAX_FOLLOW_OFFSET_Y = 14f;

    private const float MIN_FOLLOW_OFFSET_Z = -20f;
    private const float MAX_FOLLOW_OFFSET_Z = -4f;


    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineTransposer cinemachineTransposer;
    [SerializeField] private Vector3 targetFollowOffset;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }


    private void Update()
    {
        HandleCameraMovement();
        HandleCameraRotation();
        //Debug.Log(Input.mouseScrollDelta);
        HandleCameraZoom();
    }

    private void HandleCameraMovement()
    {
        Vector3 inputMoveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z = +1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }

        float cameraMoveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        transform.position += moveVector * cameraMoveSpeed * Time.deltaTime;

    }

    private void HandleCameraRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = -1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y = +1f;
        }

        float cameraRotationSpeed = 100f;

        transform.eulerAngles += rotationVector * cameraRotationSpeed * Time.deltaTime;


    }


    private void HandleCameraZoom()
    {
        float yAxisZoomSpeed = 0.7f;
        float zAxisZoomSpeed = 1f;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= yAxisZoomSpeed;
            targetFollowOffset.z += zAxisZoomSpeed;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += yAxisZoomSpeed;
            targetFollowOffset.z -= zAxisZoomSpeed;
        }

        float zoomSpeed = 5f;
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_OFFSET_Y, MAX_FOLLOW_OFFSET_Y);
        targetFollowOffset.z = Mathf.Clamp(targetFollowOffset.z, MIN_FOLLOW_OFFSET_Z, MAX_FOLLOW_OFFSET_Z);

        //cinemachineTransposer.m_FollowOffset = targetFollowOffset;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
        //followOffset = Vector3.Lerp(Vector3(0, 0, 0), followOffset, Time.deltaTime);
    }



}
