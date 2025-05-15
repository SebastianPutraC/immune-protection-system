using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public int routeId;
    [SerializeField] private List<Transform> waypoints; //LAST WAYPOINT IS THE END
    [SerializeField] private int[] allyConnectingRouteIds;
    [SerializeField] private int[] enemyConnectingRouteIds;

    public Transform GetNextWaypoint(int index)
    {
        if (index >= waypoints.Count || index < 0)
        {
            return null; //Already reached last waypoint
        }
        return waypoints[index];
    }
    public int GetWaypointsLength()
    {
        return waypoints.Count - 1;
    }

    public int GetNextRoute(int direction, UnitSide unitSide)
    {
        int[] connectingRoutes;
        if (unitSide == UnitSide.ALLY)
        {
            connectingRoutes = allyConnectingRouteIds;
        }
        else if (unitSide == UnitSide.ENEMY)
        {
            connectingRoutes = enemyConnectingRouteIds;
        }
        else
        {
            return -1;
        }

        if (connectingRoutes.Length <= 0)
        {
            return -1;
        }
        switch(direction)
        {
            case 1: //MIDDLE or DEFAULT
                if (connectingRoutes.Length >= 0)
                {
                    return connectingRoutes[0];
                }
                break;
            case 0: //UP
                if (connectingRoutes.Length >= 1)
                {
                    return connectingRoutes[1];
                }
                break;
            case 2: //DOWN
                if (connectingRoutes.Length >= 2)
                {
                    return connectingRoutes[2];
                }
                break;
            default:
                return connectingRoutes[0];
        }

        return -1;
    }

    //SHOW WAYPOINT AT START
    //SHOW WAYPOINT WHEN SELECTING UNITS
}
