using System;
using System.Collections.Generic;


enum Protocal : int
{
    MESSAGE_CREATEOBJ = 1000,
    MESSAGE_UPDATEDATA = 1001,
    MESSAGE_REFLECTDATA = 1002,
    MESSAGE_DAMAGE = 1003
}



enum SpecialEffects : int
{
    WEAPONTOWEAPON = 1,
    //WEAPONTOBODY = 2,
    BADYTOWEAPON = 2,
    BADYTOBADY = 3
}