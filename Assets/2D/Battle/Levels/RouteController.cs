using System.Collections.Generic;
using UnityEngine;

public class RouteController : MonoBehaviour
{
    [SerializeField] private List<Route> routes;

    //Might make new Class for this
    [SerializeField] private float upLimit;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;
    [SerializeField] private float downLimit;

    [SerializeField] private Vector2 startingCameraPosition;

    public Route GetRouteByID(int id)
    {
        Route chosenRoute = routes.Find(route => route.routeId == id);
        return chosenRoute;
    }
}
