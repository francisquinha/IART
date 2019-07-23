using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IA_SLIDER : MonoBehaviour {

    public bool IA_2 = false;
    public enum IA_SETT_2SET { HEUR , DEPTH}
    public IA_SETT_2SET setWhat;

    void Start()
    {
        Slider sl = GetComponent<Slider>();
        sl.onValueChanged.AddListener(OnValueChanged);
        if (setWhat == IA_SETT_2SET.DEPTH) sl.value = IA_2? IA_SETTINGS.IA_DEPTH_2 : IA_SETTINGS.IA_DEPTH;
        else sl.value = IA_2? IA_SETTINGS.IA_HEURISTIC_2 : IA_SETTINGS.IA_HEURISTIC;
    }
    void OnValueChanged(float value)
    {
        if (setWhat == IA_SETT_2SET.DEPTH)
        {
            if (IA_2) IA_SETTINGS.IA_DEPTH_2 = (int)value;
            else IA_SETTINGS.IA_DEPTH = (int)value;
        }
        else
        {
            if (IA_2) IA_SETTINGS.IA_HEURISTIC_2 = (int)value;
            else IA_SETTINGS.IA_HEURISTIC = (int)value;
        }
    }
}
