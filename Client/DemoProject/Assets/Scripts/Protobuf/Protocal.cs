﻿using System;
using System.Collections.Generic;


enum Protocal : int
{
    MESSAGE_CREATEOBJ = 1000,
    MESSAGE_UPDATEDATA = 1001,
    MESSAGE_REFLECTDATA = 1002,
    MESSAGE_DAMAGE = 1003,
    MESSAGE_INITENERGYSPHERE = 1004,
    MESSAGE_COLLECT = 1005, 
    MESSAGE_GENERATORENERGY = 1006,
    MESSAGE_RELEASESKILL = 1007
}



enum SpecialEffects : int
{
    WEAPONTOWEAPON = 1,
    BADYTOWEAPON = 2,
    BADYTOBADY = 3
}



enum SphereType : int
{
    SPHERE_RED = 1,
    SPHERE_BLUE = 2,
    SPHERE_YELLOW = 3
}
