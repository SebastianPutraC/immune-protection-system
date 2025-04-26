using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RouteController : MonoBehaviour
{
    [SerializeField] private List<Route> routes;

    public Route GetRouteByID(int id)
    {
        Route chosenRoute = routes.Find(route => route.routeId == id);
        return chosenRoute;
    }
}
