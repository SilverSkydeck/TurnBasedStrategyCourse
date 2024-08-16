using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCamera();
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //        actionCameraPosition = actionCameraPositionLeft;
    //    if (Input.GetMouseButtonDown(1))
    //        actionCameraPosition = actionCameraPositionRight;
    //}


    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 aimDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
               // Vector3 shoulderOffsetLeft = Quaternion.Euler(0, -90, 0) * aimDirection * shoulderOffsetAmount;
                Vector3 shoulderOffsetRight = Quaternion.Euler(0, 90, 0) * aimDirection * shoulderOffsetAmount;
                Vector3 cameraUnitHeight = Vector3.up * 1.7f;

                //Vector3 actionCameraPositionLeft = (shooterUnit.GetWorldPosition() + cameraUnitHeight + shoulderOffsetLeft + aimDirection * -1f);
                Vector3 actionCameraPositionRight = (shooterUnit.GetWorldPosition() + cameraUnitHeight + shoulderOffsetRight + aimDirection * -1f);

                Vector3 actionCameraPosition = actionCameraPositionRight;




                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraUnitHeight);

                ShowActionCamera();
                break;
        }

    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

}
