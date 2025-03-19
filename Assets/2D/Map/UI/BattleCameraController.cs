using UnityEngine;

public class BattleCameraController : MonoBehaviour //Will update this
{
    [SerializeField] private float dragSpeed = 2;
    [SerializeField] private Camera battleCamera;
    private Vector3 dragOrigin;

    readonly private float leftXLimit = -15;
    readonly private float rightXLimit = 15;

    private void Update()
	{
        CheckCameraLimit();

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        MoveCamera();
    }

    private void CheckCameraLimit()
    {
        if (battleCamera.transform.position.x < leftXLimit)
        {
            battleCamera.transform.position = new Vector3(leftXLimit, battleCamera.transform.position.y, battleCamera.transform.position.z);
        }

        if (battleCamera.transform.position.x > rightXLimit)
        {
            battleCamera.transform.position = new Vector3(rightXLimit, battleCamera.transform.position.y, battleCamera.transform.position.z);
        }
    }

    private void MoveCamera()
    {
        Vector3 pos = battleCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, 0, 0);

        transform.Translate(move, Space.World);
    }
}
