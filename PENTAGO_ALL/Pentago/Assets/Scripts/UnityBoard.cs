using UnityEngine;
using System.Collections;
using System;
using System.Linq;

using MINMAX = MinMax<Pentago_GameBoard, Pentago_Move>;
using HEURISTIC = Pentago_Rules.EvaluationFunction;

public class UnityBoard : MonoBehaviour
{
    public bool hackedStart = false;

    public enum GAME_MODE { PlayerVSPlayer, PlayerVsIA, IAvsPlayer, IAvsIA };
    public GAME_MODE game_mode;

    public GameObject board;
    public GameObject whitePiecesPrefab;
    public GameObject blackPiecesPrefab;
    public GameObject[] squares;
    Canvas rotationsGUI;
    Transform new_piece_hole;

    const bool whitePieces = Pentago_GameBoard.whites_turn;
    const bool blackPieces = Pentago_GameBoard.blacks_turn;
    //bool player_pieces;
    int square2placePiece;

    enum GAME_STATE
    {
        w82start,
        startIA, IAw8, IAplacepiece, IArotatequare,
        startIA2, IAw82, IAplacepiece2, IArotatequare2,
        playerselecthole, playerselectrotation,
        ended
    }
    GAME_STATE gameState = GAME_STATE.w82start;

    private Pentago_GameBoard gameboard;
    bool? winning_player;
    public IN_GAME_UI ui;

    #region IA RELATED VARS
    AI_thread iaThread;
    Pentago_Rules rules;
    MINMAX ia;

    AI_thread iaThread2;
    Pentago_Rules rules2;
    MINMAX ia2;

    [Range(0, 5)]
    public int minmax_depth = 1;
    public HEURISTIC heuristic = HEURISTIC.control;
    [Range(0, 5)]
    public int minmax_depth2 = 1;
    public HEURISTIC heuristic2 = HEURISTIC.control;
    #endregion

    #region ROTATION RELATED VARS
    int[] squares_dest_rot;
    Quaternion[] default_rotations;
    Vector3 default_y_position;
    [Range(1f, 5f)]
    public float maxHigh = 2.0f;
    [Range(0.1f, 1.5f)]
    public float rotDuration = 1.0f;
    float halfRotDuration;
    [Range(0.333f, 3f)]
    public float animRotPow = 1.0f;
    float rotatation_animation_frame = 0.0f;
    int? squareRotating = null;
    bool rotation_direction;
    Quaternion previousRotation;
    #endregion

    #region PLACING PIECE RELTED VARS
    //bool isBeingPointed = false;
    static readonly float maxtime2ShutLight = 1.0f / 50.0f;
    float time2ShutLight = 0.0f;
    Light visual_cue;
    GameObject previousCuedHole;
    static readonly float maxLightIntensity = 2.0f;
    static readonly float lightupspeed = 14.0f;
    bool lightOwned = false;
    #endregion

    public void startGame()
    {
        if (!hackedStart)
        {
            if (IA_SETTINGS.VS_IA == IA_SETTINGS.vsMode.PIA)
            {
                if (IA_SETTINGS.PLAYER_1ST) game_mode = GAME_MODE.PlayerVsIA;
                else game_mode = GAME_MODE.IAvsPlayer;
            }
            else if (IA_SETTINGS.VS_IA == IA_SETTINGS.vsMode.IAIA) game_mode = GAME_MODE.IAvsIA;
            else game_mode = GAME_MODE.PlayerVSPlayer;
        }

        if (game_mode == GAME_MODE.PlayerVSPlayer || game_mode == GAME_MODE.PlayerVsIA)
            gameState = GAME_STATE.playerselecthole;
        else gameState = GAME_STATE.startIA;

        if (game_mode != GAME_MODE.PlayerVSPlayer)
        {
            if (!hackedStart)
            {
                minmax_depth = IA_SETTINGS.IA_DEPTH;
                switch (IA_SETTINGS.IA_HEURISTIC)
                {
                    case 0: heuristic = HEURISTIC.control; break;
                    case 1: heuristic = HEURISTIC.one; break;
                    case 2: heuristic = HEURISTIC.oneDotTwo; break;
                    case 3: heuristic = HEURISTIC.A; break;
                    case 4: heuristic = HEURISTIC.AplusDiagHack; break;
                    case 5: heuristic = HEURISTIC.Astar; break;
                    default:
                        break;
                }
            }

            rules = new Pentago_Rules(heuristic,
                Pentago_Rules.NextStatesFunction.check_symmetries,
                game_mode == GAME_MODE.IAvsIA ? Pentago_Rules.IA_PIECES_WHITES : (
                game_mode == GAME_MODE.PlayerVsIA ? Pentago_Rules.IA_PIECES_BLACKS : Pentago_Rules.IA_PIECES_WHITES),
                true);

            ia = new MINMAX(MINMAX.VERSION.cut_alphabeta, rules, minmax_depth * 2);
            ia.setCUT(Pentago_Rules.MAX_HEURISTIC_VALUE,Pentago_Rules.MIN_HEURISTIC_VALUE);
            //iaThread = new AI_thread(ia);
        }

        if (game_mode == GAME_MODE.IAvsIA)
        {
            minmax_depth2 = IA_SETTINGS.IA_DEPTH_2;
            switch (IA_SETTINGS.IA_HEURISTIC_2)
            {
                case 0: heuristic2 = HEURISTIC.control; break;
                case 1: heuristic2 = HEURISTIC.one; break;
                case 2: heuristic2 = HEURISTIC.oneDotTwo; break;
                case 3: heuristic2 = HEURISTIC.A; break;
                case 4: heuristic2 = HEURISTIC.AplusDiagHack; break;
                case 5: heuristic2 = HEURISTIC.Astar; break;
                default:
                    break;
            }

            rules2 = new Pentago_Rules(heuristic2,
                Pentago_Rules.NextStatesFunction.check_symmetries,
                 Pentago_Rules.IA_PIECES_BLACKS,
                true);

            ia2 = new MINMAX(MINMAX.VERSION.cut_alphabeta, rules2, minmax_depth2 * 2);
            ia2.setCUT(Pentago_Rules.MAX_HEURISTIC_VALUE, Pentago_Rules.MIN_HEURISTIC_VALUE);
        }

    }

    public void setRotationSquare(int square) { if (squareRotating.HasValue) return; squareRotating = square; }

    public void rotateSquare(bool clockwise)
    {
        if (rotatation_animation_frame != 0.0f) return;
        rotation_direction = clockwise ? Pentago_Move.rotate_clockwise : Pentago_Move.rotate_anticlockwise;
        halfRotDuration = rotDuration / 2.0f;
        previousRotation = squares[squareRotating.Value].transform.localRotation;
        squares_dest_rot[squareRotating.Value] = (squares_dest_rot[squareRotating.Value] + (clockwise ? 1 : 3)) % 4;
        // Debug.Log("new rotation set - " + squares_dest_rot[squareRotating.Value]);
    }



    bool GAMESUBSTATE_IS_PLACING_PIECE = false;//fast 'hacked solution', indicates when the placing of the piece is happening
    void PlayerPlacingPiece()
    {
        if (GAMESUBSTATE_IS_PLACING_PIECE) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Hole objectHit = hit.transform.GetComponent<Hole>();
            if (objectHit != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Hole hole = objectHit.GetComponent<Hole>();
                    new_piece_hole = hole.transform;
                    square2placePiece = Convert.ToInt32(hole.transform.parent.name.Substring(0, 1));

                    Pentago_Move mov = new Pentago_Move(square2placePiece, hole.x, hole.y);

                    if (mov.is_move_possible(gameboard))
                    {
                        mov.apply_move2board(gameboard);
                        visual_cue.intensity = 0;
                        time2ShutLight = 0;
                        lightOwned = false;
                        GAMESUBSTATE_IS_PLACING_PIECE = true;
                        StartCoroutine("PLACE_PIECE_COUROUTINE");
                        return;
                    }
                }

                if (previousCuedHole != null && previousCuedHole == objectHit.transform.gameObject || !lightOwned)
                {
                    time2ShutLight = maxtime2ShutLight;
                    visual_cue.transform.position = objectHit.transform.position;
                    lightOwned = true;
                    previousCuedHole = objectHit.transform.gameObject;
                }
            }
        }


        if (time2ShutLight > 0)
        {
            if (visual_cue.intensity < maxLightIntensity)
                visual_cue.intensity += lightupspeed * Time.deltaTime;
            time2ShutLight -= Time.deltaTime;
        }
        else
        {
            if (visual_cue.intensity > 0)
                visual_cue.intensity -= lightupspeed * Time.deltaTime;
            else
            {
                lightOwned = false;
                visual_cue.intensity = 0;
            }
        }
    }

    void ShowRotationGUI()
    {
        rotationsGUI.enabled = true;
    }

    void HideRotationGUI()
    {
        rotationsGUI.enabled = false;
    }

    bool ApplyRotation()
    {
        if (squareRotating.HasValue)
        {
            rotatation_animation_frame += Time.deltaTime;

            float rotatation_animation;
            if (rotatation_animation_frame >= rotDuration)
            {
                rotatation_animation = 1.0f;
            }
            else rotatation_animation =
                rotatation_animation_frame > halfRotDuration ?
                1.0f - .5f * Mathf.Pow((rotDuration - rotatation_animation_frame) / halfRotDuration, animRotPow)
                : 0.5f * Mathf.Pow(rotatation_animation_frame / halfRotDuration, animRotPow)
                ;

            squares[squareRotating.Value].transform.localRotation = Quaternion.Slerp(
               previousRotation,
                default_rotations[squares_dest_rot[squareRotating.Value]],
                rotatation_animation);

            //float angle = Quaternion.Angle(squares[squareRotating.Value].transform.localRotation,
            //    default_rotations[squares_dest_rot[squareRotating.Value]]);

            squares[squareRotating.Value].transform.localPosition = new Vector3(squares[squareRotating.Value].transform.localPosition.x,
                default_y_position.y + maxHigh * Mathf.Pow((.5f - Mathf.Abs(.5f - rotatation_animation)) * 2, animRotPow)
                , squares[squareRotating.Value].transform.localPosition.z);

            if (rotatation_animation_frame >= rotDuration)
            {

                rotatation_animation_frame = 0.0f;
                Pentago_Move move = new Pentago_Move(squareRotating.Value, rotation_direction);
                move.apply_move2board(gameboard);
                squareRotating = null;
                return true;//rotation completed
            }
        }
        return false;
    }

    void displayEndGame()
    {
        if (winning_player == null) ui.setEndGame("DRAW");
        else
        {
            if (winning_player.Value == Pentago_GameBoard.blacks_turn) ui.setEndGame("Black\nWINS");
            else ui.setEndGame("White\nWINS");
        }
    }


    void Start()
    {
        squares_dest_rot = new int[4];
        for (int i = 0; i < 4; i++)
            squares_dest_rot[i] = 0;

        default_rotations = new Quaternion[4];
        default_rotations[0] = Quaternion.Euler(-90, 0, 0);
        default_rotations[1] = Quaternion.Euler(-90, 90, 0);
        default_rotations[2] = Quaternion.Euler(-90, 180, 0);
        default_rotations[3] = Quaternion.Euler(-90, 270, 0);

        default_y_position = squares[0].transform.localPosition;

        squareRotating = null;

        visual_cue = GetComponentInChildren<Light>();
        rotationsGUI = GetComponentInChildren<Canvas>();
        HideRotationGUI();
        gameboard = new Pentago_GameBoard();

        startGame();
    }

    void Update()
    {

        switch (game_mode)
        {
            case GAME_MODE.PlayerVSPlayer: update_PvsP(); break;
            case GAME_MODE.IAvsPlayer: update_vsIA(); break;
            case GAME_MODE.PlayerVsIA: update_vsIA(); break;
            case GAME_MODE.IAvsIA: update_IAvsIA(); break;
            default: break;
        }


    }



    void update_PvsP()
    {
        switch (gameState)
        {
            case GAME_STATE.w82start:
                break;
            case GAME_STATE.startIA:
                gameState = GAME_STATE.playerselecthole;
                break;
            case GAME_STATE.IAplacepiece:
                gameState = GAME_STATE.playerselecthole;
                break;
            case GAME_STATE.playerselecthole:
                PlayerPlacingPiece();
                break;
            case GAME_STATE.playerselectrotation:
                if (ApplyRotation())
                {
                    HideRotationGUI();
                    if (gameboard.game_ended(out winning_player)) gameState = GAME_STATE.ended;
                    else gameState = GAME_STATE.startIA;
                }
                break;
            case GAME_STATE.ended:
                displayEndGame();
                this.enabled = false;
                break;
            default:
                break;
        }
    }

    void update_vsIA()
    {
        //http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
        //Debug.Log("!!" + gameboard.board[28]);
        switch (gameState)
        {
            case GAME_STATE.w82start:
                break;
            case GAME_STATE.startIA:
                //start IA couroutine
                iaThread = new AI_thread(ia);
                iaThread.gameboard = gameboard.Clone();
                iaThread.Start();
                StartCoroutine("IA_COUROUTINE");
                break;
            case GAME_STATE.IAw8:
                break;
            case GAME_STATE.IAplacepiece:
                IA_place_piece();
                break;
            case GAME_STATE.IArotatequare:
                if (ApplyRotation())
                {
                    if (gameboard.game_ended(out winning_player)) gameState = GAME_STATE.ended;
                    else gameState = GAME_STATE.playerselecthole; ;
                }
                break;
            case GAME_STATE.playerselecthole:
                PlayerPlacingPiece();
                break;
            case GAME_STATE.playerselectrotation:
                if (ApplyRotation())
                {
                    HideRotationGUI();
                    if (gameboard.game_ended(out winning_player)) gameState = GAME_STATE.ended;
                    else gameState = GAME_STATE.startIA;
                }
                break;
            case GAME_STATE.ended:
                displayEndGame();
                this.enabled = false;
                break;
            default:
                break;
        }
    }

    void update_IAvsIA()
    {
        //http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
        //Debug.Log("!!" + gameboard.board[28]);
        switch (gameState)
        {
            case GAME_STATE.w82start:
                break;
            case GAME_STATE.startIA:
                //start IA couroutine
                iaThread = new AI_thread(ia);
                iaThread.gameboard = gameboard.Clone();
                iaThread.Start();
                StartCoroutine("IA_COUROUTINE");
                break;
            case GAME_STATE.IAw8:
                break;
            case GAME_STATE.IAplacepiece:
                IA_place_piece(false);
                break;
            case GAME_STATE.IArotatequare:
                if (ApplyRotation())
                {
                    if (gameboard.game_ended(out winning_player)) gameState = GAME_STATE.ended;
                    else gameState = GAME_STATE.startIA2; ;
                }
                break;

            case GAME_STATE.startIA2:
                //start IA couroutine
                iaThread2 = new AI_thread(ia2);
                iaThread2.gameboard = gameboard.Clone();
                iaThread2.Start();
                StartCoroutine("IA_COUROUTINE2");
                break;
            case GAME_STATE.IAw82:
                break;
            case GAME_STATE.IAplacepiece2:
                IA_place_piece(true);
                break;
            case GAME_STATE.IArotatequare2:
                if (ApplyRotation())
                {
                    if (gameboard.game_ended(out winning_player)) gameState = GAME_STATE.ended;
                    else gameState = GAME_STATE.startIA; ;
                }
                break;

            case GAME_STATE.ended:
                displayEndGame();
                this.enabled = false;
                break;
            default:
                break;
        }
    }


    void IA_place_piece(bool IA2 = false)
    {
        if (!IA2 && iaThread != null && !iaThread.concluded) return;
        if (IA2 && iaThread2 != null && !iaThread2.concluded) return;
        if (GAMESUBSTATE_IS_PLACING_PIECE) return;
        Pentago_Move place_move = IA2 ? iaThread2.moves[0] : iaThread.moves[0];
        //Debug.Log((gameboard.get_player_turn() == Pentago_GameBoard.blacks_turn) + "__" + (gameboard.get_turn_state() == Pentago_GameBoard.turn_state_addpiece));
        place_move.apply_move2board(gameboard);
        // Debug.Log(gameboard.get_player_turn() + "__" + gameboard.get_turn_state());


        int x, y;
        Pentago_GameBoard.board_index_to_position(place_move.index, out x, out y, out square2placePiece);
        new_piece_hole = squares[square2placePiece].transform.parent.gameObject.GetComponentsInChildren<Hole>().First(
            o => o.x == x && o.y == y).gameObject.transform;

        GAMESUBSTATE_IS_PLACING_PIECE = true;
        StartCoroutine("PLACE_PIECE_COUROUTINE");
    }

    IEnumerator IA_COUROUTINE()
    {
        //Debug.Log("BEGIN");
        gameState = GAME_STATE.IAw8;
        yield return StartCoroutine(iaThread.WaitFor());
        gameState = GAME_STATE.IAplacepiece;
        //Debug.Log("END");
    }

    IEnumerator IA_COUROUTINE2()
    {
        //Debug.Log("BEGIN");
        gameState = GAME_STATE.IAw82;
        yield return StartCoroutine(iaThread2.WaitFor());
        gameState = GAME_STATE.IAplacepiece2;
        //Debug.Log("END");
    }


    IEnumerator PLACE_PIECE_COUROUTINE()
    {
        yield return null;

        GameObject newObj;
        if (gameboard.get_player_turn() == Pentago_GameBoard.whites_turn) newObj = GameObject.Instantiate(whitePiecesPrefab);
        else newObj = GameObject.Instantiate(blackPiecesPrefab);

        newObj.transform.position = new_piece_hole.transform.position;
        newObj.transform.parent = squares[square2placePiece].transform;

        if (gameboard.game_ended(out winning_player)) gameState = GAME_STATE.ended;
        else //if game did not end
        {
            if (gameState == GAME_STATE.playerselecthole)
            {
                gameState = GAME_STATE.playerselectrotation;
                ShowRotationGUI();
            }
            if (gameState == GAME_STATE.IAplacepiece)
            {
                if (iaThread.moves.Length < 2)
                {
                    Debug.LogError("IA did not decided on rotation!!!");
                }
                Pentago_Move rotate_move = iaThread.moves[1];
                setRotationSquare(rotate_move.square2rotate);
                rotateSquare(rotate_move.rotDir == Pentago_Move.rotate_clockwise);
                //done on apply rotation -> rotate_move.apply_move2board(gameboard);
                gameState = GAME_STATE.IArotatequare;
            }
            if (gameState == GAME_STATE.IAplacepiece2)
            {
                if (iaThread2.moves.Length < 2)
                {
                    Debug.LogError("2nd IA did not decided on rotation!!!");
                }
                Pentago_Move rotate_move = iaThread2.moves[1];
                setRotationSquare(rotate_move.square2rotate);
                rotateSquare(rotate_move.rotDir == Pentago_Move.rotate_clockwise);
                //done on apply rotation -> rotate_move.apply_move2board(gameboard);
                gameState = GAME_STATE.IArotatequare2;
            }
        }

        GAMESUBSTATE_IS_PLACING_PIECE = false;
    }

    public void ABORT()
    {
        if (iaThread != null && !iaThread.concluded) iaThread.Abort();
        if (iaThread2 != null && !iaThread2.concluded) iaThread2.Abort();
    }
}
