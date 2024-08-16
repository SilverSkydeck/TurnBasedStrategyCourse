using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    //[SerializeField] private GameObject gameObject;

    private void Start()
    {
        UnitActionSystem.Instance.OnActionButtonUIBusy += UnitActionSystem_OnActionButtonUIBusy;
        HideBusyUI();
    }


    private void ShowBusyUI()
    {
        gameObject.SetActive(true);
    }

    private void HideBusyUI()
    {
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnActionButtonUIBusy(object sender, bool isUnitBusy)
    {
        if (isUnitBusy) ShowBusyUI();
        else HideBusyUI();

    }


}
