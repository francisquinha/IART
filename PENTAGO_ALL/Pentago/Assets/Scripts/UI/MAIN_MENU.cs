using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MAIN_MENU : MonoBehaviour {

    public Canvas[] Canvas;
    Vector2 cameraPrevPos;//float previous_camera_Xpos;
    Vector2 cameraTargetPos;// float camera_target_Xpos;
    public float transitionDuration;
    public float animTransitionPow;
    float transition_animation_frame;
    float halfTransitionDuration;
    bool updatecam = false;

	// Use this for initialization
	void Start () {
        halfTransitionDuration = transitionDuration / 2;
	}
	
	// Update is called once per frame
	void Update () {

        if (updatecam) place_camera();

	}


    void place_camera()
    {
        transition_animation_frame += Time.deltaTime;
        float transition_animation;
        if (transition_animation_frame >= transitionDuration)
        {
            transition_animation = 1.0f;
            updatecam = false;
        }
        else transition_animation =
            transition_animation_frame > halfTransitionDuration ?
            1.0f - .5f * Mathf.Pow((transitionDuration - transition_animation_frame) / halfTransitionDuration, animTransitionPow)
            : 0.5f * Mathf.Pow(transition_animation_frame / halfTransitionDuration, animTransitionPow)
            ;
      
        Camera.main.transform.position = new Vector3(
    /*previous_camera_Xpos*/cameraPrevPos.x * (1.0f - transition_animation) + cameraTargetPos.x/*camera_target_Xpos*/ * transition_animation
    , cameraPrevPos.y * (1.0f - transition_animation) + cameraTargetPos.y/*camera_target_Xpos*/ * transition_animation
    , Camera.main.transform.position.z);
    }

    public void change_canvas(int target_canvas)
    {
        //previous_camera_Xpos = Camera.main.transform.position.x;
        //camera_target_Xpos = Canvas[target_canvas].transform.position.x;
        cameraPrevPos = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
        cameraTargetPos = new Vector2(Canvas[target_canvas].transform.position.x, Canvas[target_canvas].transform.position.y);
        transition_animation_frame = 0;
        updatecam = true;
    }

    public void start_game(int mode)
    {
        if (mode == 1) IA_SETTINGS.VS_IA = IA_SETTINGS.vsMode.PP;
        if (mode == 2) IA_SETTINGS.VS_IA = IA_SETTINGS.vsMode.PIA;
        if (mode == 3) IA_SETTINGS.VS_IA = IA_SETTINGS.vsMode.IAIA;
		SceneManager.LoadScene("game_scene",LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
