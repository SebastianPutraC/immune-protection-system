using System;
using UnityEngine;

public interface IPresenter
{
    void GetAntigen(UnitController antigen);
    void ReturnToBase();
    void ArrivedAtBase();
    Action<int> AntigenPresented { get; set; }
}
