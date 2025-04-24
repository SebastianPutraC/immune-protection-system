using System;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform battlefieldZPosition;

    private LayerMask layerMask;

    public Action<AllyUnitController> UnitSelected;
    private float zDistance;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Ally");
        zDistance = Mathf.Abs(cam.transform.position.z - battlefieldZPosition.position.z);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //One selected unit
        {
            var mousePos = Input.mousePosition;
            mousePos.z = zDistance;
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(mousePos), transform.up, Mathf.Infinity, layerMask);
            if (hit.collider != null && hit.collider.tag == "AllyUnit")
            {
                AllyUnitController unitSelect = hit.collider.GetComponent<AllyUnitController>();
                UnitSelected?.Invoke(unitSelect);
            }
        }
    }
}
