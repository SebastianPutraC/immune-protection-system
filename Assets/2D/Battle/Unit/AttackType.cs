using System;

[Flags]
public enum AttackType
{
    NONE = 0,
    MELEE = 1,
    RANGED = 2,
    ARTILERRY = 4,
    AOE = 8
}
