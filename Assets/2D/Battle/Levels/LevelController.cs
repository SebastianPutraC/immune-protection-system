using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<Route> routes;

    public List<Spawner> allySpawners;
    public List<Spawner> enemySpawners;
    public List<BodyCellController> cellSpawner;

    public List<BaseController> allyBaseControllers;
    public List<BaseController> enemyBaseControllers;

    //Might make new Class for this
    public float upLimit;
    public float leftLimit;
    public float rightLimit;
    public float downLimit;

    public Vector2 startingCameraPosition;

    public Route GetRouteByID(int id)
    {
        Route chosenRoute = routes.Where(route => route.routeId == id).FirstOrDefault();
        return chosenRoute;
    }
}
