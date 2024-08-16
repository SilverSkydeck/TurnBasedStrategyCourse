using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private static MouseWorld instance;


    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        // Debug.Log(Input.mousePosition);

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, mousePlaneLayerMask));
        //transform.position = raycastHit.point;
        transform.position = MouseWorld.GetPosition();

    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        //Debug.Log(raycastHit.point);
        return raycastHit.point;
    }


}
