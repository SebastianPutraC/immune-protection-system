using UnityEngine;

public class BattleCameraController : MonoBehaviour //Will update this
{
    [SerializeField] private float dragSpeed = 2;
    [SerializeField] private Camera battleCamera;
    private Vector3 dragOrigin;

    private float upYLimit = 5;
    private float leftXLimit = -15;
    private float rightXLimit = 15;
    private float downYLimit = -5;

    private readonly float zPosition = -35;

    private float minZoom = 25f;
    private float maxZoom = 100f;
    private float sensZoom = 25f;

    public void InitializeBattleCamera(float up, float left, float right, float down, Vector2 position)
    {
        upYLimit = up;
        leftXLimit = left;
        rightXLimit = right;
        downYLimit = down;

        this.transform.position = new Vector3(position.x, position.y, zPosition);
    }

    private void Update()
	{
        float fov = battleCamera.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensZoom * -1;
        fov = Mathf.Clamp(fov, minZoom, maxZoom);
        battleCamera.fieldOfView = fov;

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
        if (battleCamera.transform.position.y > upYLimit)
        {
            battleCamera.transform.position = new Vector3(battleCamera.transform.position.x, upYLimit, battleCamera.transform.position.z);
        }
        if (battleCamera.transform.position.x < leftXLimit)
        {
            battleCamera.transform.position = new Vector3(leftXLimit, battleCamera.transform.position.y, battleCamera.transform.position.z);
        }
        if (battleCamera.transform.position.x > rightXLimit)
        {
            battleCamera.transform.position = new Vector3(rightXLimit, battleCamera.transform.position.y, battleCamera.transform.position.z);
        }
        if (battleCamera.transform.position.y < downYLimit)
        {
            battleCamera.transform.position = new Vector3(battleCamera.transform.position.x, downYLimit, battleCamera.transform.position.z);
        }
    }

    private void MoveCamera()
    {
        Vector3 pos = battleCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);
    }

    public void SetCameraPosition(Vector2 cameraPosition)
    {
        this.gameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, zPosition);
    }    
}
