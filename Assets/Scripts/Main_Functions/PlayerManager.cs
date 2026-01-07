using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class PlayerManager
{
    // playerID = 0 : Player1, 1 : Player2
    public static int playerID = 0;

    public static Transform playTransform;

    public static int deathNumber = 10;

    public static int onlyFadeOut = 0;

    public static bool isMessageSelect = false;

    public static bool isRButtonUsed = false;


}
