using System;
using UnityEngine;

public interface IPresenter
{
    void GetAntigen(UnitController antigen);
    void ReturnToBase();
    void ArrivedAtBase();

    bool isPresenting { get; set; }
    Action<int> AntigenPresented { get; set; }
}
