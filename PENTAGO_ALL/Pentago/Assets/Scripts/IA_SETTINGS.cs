using UnityEngine;
using System.Collections;

public static class IA_SETTINGS  {
    //public static bool VS_IA = false;
    public enum vsMode { PP, PIA, IAIA };
    public static vsMode VS_IA = vsMode.PP;

    public static int IA_DEPTH = 0;
    public static int IA_HEURISTIC = 0;

    public static int IA_DEPTH_2 = 0;
    public static int IA_HEURISTIC_2 = 0;

    public static bool PLAYER_1ST = true;
}
