using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TOGGLE_1ST_P : MonoBehaviour {

    void Start()
    {
        Toggle tg = GetComponent<Toggle>();
        tg.onValueChanged.AddListener(OnValueChanged);
        tg.isOn = IA_SETTINGS.PLAYER_1ST;
    }
    void OnValueChanged(bool value)
    {
        IA_SETTINGS.PLAYER_1ST = value;
    }
}
