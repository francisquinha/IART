using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IN_GAME_UI : MonoBehaviour {

    public Text endGameMSG;    
    public Image panel ;
    bool play_end_animation=false;
    public float animation_duration;
    float animation_frame=0.0f;
    public float maxPanelAlpha = 0.5f;

    public UnityBoard board;

    void Update()
    {
        if(play_end_animation)
        {
            animation_frame += Time.deltaTime;
            if (animation_frame>animation_duration){
                animation_frame = animation_duration;
                this.enabled = false;
            }
            endGameMSG.color = new Color(endGameMSG.color.r, endGameMSG.color.g, endGameMSG.color.b, 1 - (animation_duration - animation_frame) / animation_duration);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b,
                maxPanelAlpha*(1 - (animation_duration - animation_frame) / animation_duration));
            
        }
    }

    public void Exit()
    {
        board.ABORT();
        SceneManager.LoadScene("main_menu", LoadSceneMode.Single);
    }

    public void setEndGame(string text)
    {
        endGameMSG.text = text;
        endGameMSG.enabled = true;
        panel.enabled = true;
        play_end_animation = true;
    }

}
