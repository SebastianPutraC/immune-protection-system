using System;
using UnityEngine;

public interface IInflammator
{
    Action<int> InflammatoryActivated { get; set; }
}
