//<copyright file="Pentago_Rules_HEURISTIC_1.cs">
//Copyright (c) 12/04/2016 10:06:17 All Right Reserved
//</copyright>
//<author>Bruno Madeira</author>
//<date> 12/04/2016 10:06:17 </date>
//<summary>Implementation of a possible heuristic for the game Pentago</summary>

using System.Linq;
using System.Runtime.CompilerServices;
using HOLESTATE = Pentago_GameBoard.hole_state;


public partial class Pentago_Rules
{
    float heuristic1_bias = .5f;

    float h1_middle = 1.0f;
    float h1_inner = 1.0f;
    float h1_outer = 1.0f;
    float h1_main_Diag = 1.0f; 
    float h1_other_Diag = .7f;//can only make one (one possible five), should weight lees*/
    float[] weights;

    /// <summary>
    /// <para>set a bias to better define heuristic's 1 priorities (default is 0.5) </para>
    /// <para>set a value towards 0 to prioritize minimization of adversary possible lines</para>
    /// <para>set 1 to only prioritize maximizing own chances</para>
    /// </summary>
    /// <param name="bias">value from 0 to 1</param>
    public void setHeuristic1Bias(float bias)
    {
        heuristic1_bias = bias;
    }

    /// <summary>
    /// gets utility of a gameboard using heuristic1
    /// </summary>
    /// <param name="gb"></param>
    /// <returns></returns>
    public float heuristic1(HOLESTATE[] gb)
    {
        if(weights == null) weights = new float[] { h1_middle, h1_inner, h1_outer, h1_main_Diag, h1_other_Diag };
        float whites, blacks;
        calculate_available_classes(gb, out whites, out blacks, weights);

        float value;
       
        if (IA_PIECES == IA_PIECES_BLACKS) value = (blacks) * heuristic1_bias - (whites) * (1.0f - heuristic1_bias);
        else value = (whites) * heuristic1_bias - (blacks) * (1.0f - heuristic1_bias);

        return value;
    }



    public static void calculate_available_classes(Pentago_GameBoard gb, out float available4whites, out float available4blacks,float[] weights)
    {
        calculate_available_classes(gb.board, out available4whites, out available4blacks, weights);
    }

    static void calculate_available_classes(HOLESTATE[] gb, out float available4whites, out float available4blacks, float[] weights)
    {
        //int[] allsquares = new int[] { 0, 1, 2, 3 };
        available4whites = 0;
        available4blacks = 0;

        int[] L_P1 = new int[6]; //L1_1, L2_1, L3_1, L4_1, L5_1, L6_1; //lines 4 whites
        int[] L_P2 = new int[6]; //L1_2, L2_2, L3_2, L4_2, L5_2, L6_2; //lines 4 blacks
        int[] R_P1 = new int[6];//R1_1, R2_1, R3_1, R4_1, R5_1, R6_1;//rows...
        int[] R_P2 = new int[6];//R1_2, R2_2, R3_2, R4_2, R5_2, R6_2;
        int[] D_1 = new int[6];//DLC_1, DLD_1, DLU_1, DRC_1, DRD_1, DRU_1; //D diagonal | L left start, R right start | D down start, U up start
        int[] D_2 = new int[6]; //DLC_2, DLD_2, DLU_2, DRC_2, DRD_2, DRU_2; //D diagonal | L left start, R right start | D down start, U up start

        //calculate aux stuff first --------------------------------------

        //pluscross 1,2,3,4 P1 and P2
        int[] pluscross_1 = new int[4];
        int[] pluscross_2 = new int[4];
        for (int i = 0; i < 4; i++)
            available_pluscross(gb, i, out pluscross_1[i], out pluscross_2[i]);

        //mulcross 1,2,3,4 P1 and P2
        int[] mulcross_1 = new int[4];
        int[] mulcross_2 = new int[4];
        for (int i = 0; i < 4; i++)
            available_mulcross(gb, i, out mulcross_1[i], out mulcross_2[i]);

        //diamond 1,2,3,4 P1 and P2
        int[] diamondcross_1 = new int[4];
        int[] diamondcross_2 = new int[4];
        for (int i = 0; i < 4; i++)
            available_diamond(gb, i, out diamondcross_1[i], out diamondcross_2[i]);

        //box1 1,2,3,4 P1 and P2
        int[] box1_1 = new int[4];
        int[] box1_2 = new int[4];
        for (int i = 0; i < 4; i++)
            available_box1(gb, i, out box1_1[i], out box1_2[i]);

        //box2 1,2,3,4 P1 and P2
        int[] box2_1 = new int[4];
        int[] box2_2 = new int[4];
        for (int i = 0; i < 4; i++)
            available_box2(gb, i, out box2_1[i], out box2_2[i]);

        //corners 1,2,3,4 P1 and P2
        int[] corners_1 = new int[4];
        int[] corners_2 = new int[4];
        for (int i = 0; i < 4; i++)
            available_corners(gb, i, out corners_1[i], out corners_2[i]);

        //CALCULATE 4 LINES

        L_P1[0] = select_min(box1_1[0], box2_1[1]);
        L_P1[1] = select_min(pluscross_1[0], pluscross_1[1]);
        L_P1[2] = select_min(box2_1[0], box1_1[1]);
        L_P1[3] = select_min(box1_1[2], box2_1[3]);
        L_P1[4] = select_min(pluscross_1[2], pluscross_1[3]);
        L_P1[5] = select_min(box2_1[2], box1_1[3]);

        L_P2[0] = select_min(box1_2[0], box2_2[1]);
        L_P2[1] = select_min(pluscross_2[0], pluscross_2[1]);
        L_P2[2] = select_min(box2_2[0], box1_2[1]);
        L_P2[3] = select_min(box1_2[2], box2_2[3]);
        L_P2[4] = select_min(pluscross_2[2], pluscross_2[3]);
        L_P2[5] = select_min(box2_2[2], box1_2[3]);

        //CALCULATE 4 ROWS

        R_P1[0] = select_min(box2_1[0], box1_1[2]);
        R_P1[1] = select_min(pluscross_1[0], pluscross_1[2]);
        R_P1[2] = select_min(box1_1[0], box2_1[2]);
        R_P1[3] = select_min(box2_1[1], box1_1[3]);
        R_P1[4] = select_min(pluscross_1[1], pluscross_1[3]);
        R_P1[5] = select_min(box1_1[1], box2_1[3]);

        R_P2[0] = select_min(box2_2[0], box1_2[2]);
        R_P2[1] = select_min(pluscross_2[0], pluscross_2[2]);
        R_P2[2] = select_min(box1_2[0], box2_2[2]);
        R_P2[3] = select_min(box2_2[1], box1_2[3]);
        R_P2[4] = select_min(pluscross_2[1], pluscross_2[3]);
        R_P2[5] = select_min(box1_2[1], box2_2[3]);

        //CALCULATE 4 DIAGONALS
        D_1[0] = select_min(mulcross_1[0], mulcross_1[3]);
        D_1[1] = select_min(diamondcross_1[0], diamondcross_1[3], corners_1[1]);
        D_1[2] = select_min(diamondcross_1[0], diamondcross_1[3], corners_1[2]);
        D_1[3] = select_min(mulcross_1[1], mulcross_1[2]);
        D_1[4] = select_min(diamondcross_1[1], diamondcross_1[2], corners_1[3]);
        D_1[5] = select_min(diamondcross_1[1], diamondcross_1[2], corners_1[0]);

        D_2[0] = select_min(mulcross_2[0], mulcross_2[3]);
        D_2[1] = select_min(diamondcross_2[0], diamondcross_2[3], corners_2[1]);
        D_2[2] = select_min(diamondcross_2[0], diamondcross_2[3], corners_2[2]);
        D_2[3] = select_min(mulcross_2[1], mulcross_2[2]);
        D_2[4] = select_min(diamondcross_2[1], diamondcross_2[2], corners_2[3]);
        D_2[5] = select_min(diamondcross_2[1], diamondcross_2[2], corners_2[0]);

        available4whites =
        (L_P1[1] + L_P1[4] + R_P1[1] + R_P1[1]) * weights[0]//middle
                + (L_P1[2] + L_P1[3] + R_P1[2] + R_P1[3]) * weights[1]//inner 
        + (L_P1[0] + L_P1[5] + R_P1[0] + R_P1[5]) * weights[2]//outer 
           + (D_1[0] + D_1[3]) * weights[3]//main diags
        + (D_1[1] + D_1[2] + D_1[4] + D_1[5] ) * weights[4];//other diags
                                        //old L_P1.Sum() + R_P1.Sum() + D_1.Sum(); 

        available4blacks =
                    (L_P2[1] + L_P2[4] + R_P2[1] + R_P2[1]) * weights[0]//middle
                + (L_P2[2] + L_P2[3] + R_P2[2] + R_P2[3]) * weights[1]//inner 
        + (L_P2[0] + L_P2[5] + R_P2[0] + R_P2[5]) * weights[2]//outer 
           + (D_2[0] + D_2[3]) * weights[3]//main diags
        + (D_2[1] + D_2[2] + D_2[4] + D_2[5]) * weights[4];//other diags 
                                                           //old L_P2.Sum() + R_P2.Sum() + D_2.Sum(); ;

        //maybe max could be relevant 2 ?
    }


    static void available_pluscross(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks)
    {
        available4whites = 0;
        available4blacks = 0;

        HOLESTATE H11 = gb[Pentago_GameBoard.board_position_to_index(1, 1, square)];
        HOLESTATE H10 = gb[Pentago_GameBoard.board_position_to_index(1, 0, square)];
        HOLESTATE H12 = gb[Pentago_GameBoard.board_position_to_index(1, 2, square)];
        HOLESTATE H01 = gb[Pentago_GameBoard.board_position_to_index(0, 1, square)];
        HOLESTATE H21 = gb[Pentago_GameBoard.board_position_to_index(2, 1, square)];

        bool h11w = H11 != HOLESTATE.has_black;
        if (h11w && H10 != HOLESTATE.has_black) available4whites++;
        if (h11w && H12 != HOLESTATE.has_black) available4whites++;
        if (h11w && H01 != HOLESTATE.has_black) available4whites++;
        if (h11w && H21 != HOLESTATE.has_black) available4whites++;

        bool h11b = H11 != HOLESTATE.has_white;
        if (h11b && H10 != HOLESTATE.has_white) available4blacks++;
        if (h11b && H12 != HOLESTATE.has_white) available4blacks++;
        if (h11b && H01 != HOLESTATE.has_white) available4blacks++;
        if (h11b && H21 != HOLESTATE.has_white) available4blacks++;

    }

    static void available_mulcross(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks)
    {
        available4whites = 0;
        available4blacks = 0;

        HOLESTATE H11 = gb[Pentago_GameBoard.board_position_to_index(1, 1, square)];
        HOLESTATE H00 = gb[Pentago_GameBoard.board_position_to_index(0, 0, square)];
        HOLESTATE H02 = gb[Pentago_GameBoard.board_position_to_index(0, 2, square)];
        HOLESTATE H20 = gb[Pentago_GameBoard.board_position_to_index(2, 0, square)];
        HOLESTATE H22 = gb[Pentago_GameBoard.board_position_to_index(2, 2, square)];

        bool h11w = H11 != HOLESTATE.has_black;
        if (h11w && H00 != HOLESTATE.has_black) available4whites++;
        if (h11w && H02 != HOLESTATE.has_black) available4whites++;
        if (h11w && H20 != HOLESTATE.has_black) available4whites++;
        if (h11w && H22 != HOLESTATE.has_black) available4whites++;

        bool h11b = H11 != HOLESTATE.has_white;
        if (h11b && H00 != HOLESTATE.has_white) available4blacks++;
        if (h11b && H02 != HOLESTATE.has_white) available4blacks++;
        if (h11b && H20 != HOLESTATE.has_white) available4blacks++;
        if (h11b && H22 != HOLESTATE.has_white) available4blacks++;
    }

    static void available_diamond(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks)
    {
        available4whites = 0;
        available4blacks = 0;

        HOLESTATE H10 = gb[Pentago_GameBoard.board_position_to_index(1, 0, square)];
        HOLESTATE H12 = gb[Pentago_GameBoard.board_position_to_index(1, 2, square)];
        HOLESTATE H01 = gb[Pentago_GameBoard.board_position_to_index(0, 1, square)];
        HOLESTATE H21 = gb[Pentago_GameBoard.board_position_to_index(2, 1, square)];

        /* if (H10 != HOLESTATE.has_black) available4whites++;
         if (H12 != HOLESTATE.has_black) available4whites++;
         if (H01 != HOLESTATE.has_black) available4whites++;
         if (H21 != HOLESTATE.has_black) available4whites++;

         if (H10 != HOLESTATE.has_white) available4blacks++;
         if (H12 != HOLESTATE.has_white) available4blacks++;
         if (H01 != HOLESTATE.has_white) available4blacks++;
         if (H21 != HOLESTATE.has_white) available4blacks++;*/

        if (H10 != HOLESTATE.has_black && H21 != HOLESTATE.has_black) available4whites++;
        if (H12 != HOLESTATE.has_black && H01 != HOLESTATE.has_black) available4whites++;
        if (H01 != HOLESTATE.has_black && H10 != HOLESTATE.has_black) available4whites++;
        if (H21 != HOLESTATE.has_black && H12 != HOLESTATE.has_black) available4whites++;

        if (H10 != HOLESTATE.has_white && H21 != HOLESTATE.has_white) available4blacks++;
        if (H12 != HOLESTATE.has_white && H01 != HOLESTATE.has_white) available4blacks++;
        if (H01 != HOLESTATE.has_white && H10 != HOLESTATE.has_white) available4blacks++;
        if (H21 != HOLESTATE.has_white && H12 != HOLESTATE.has_white) available4blacks++;
    }

    static void available_corners(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks)
    {
        available4whites = 0;
        available4blacks = 0;

        HOLESTATE H00 = gb[Pentago_GameBoard.board_position_to_index(0, 0, square)];
        HOLESTATE H02 = gb[Pentago_GameBoard.board_position_to_index(0, 2, square)];
        HOLESTATE H20 = gb[Pentago_GameBoard.board_position_to_index(2, 0, square)];
        HOLESTATE H22 = gb[Pentago_GameBoard.board_position_to_index(2, 2, square)];

        if (H00 != HOLESTATE.has_black) available4whites++;
        if (H02 != HOLESTATE.has_black) available4whites++;
        if (H20 != HOLESTATE.has_black) available4whites++;
        if (H22 != HOLESTATE.has_black) available4whites++;

        if (H00 != HOLESTATE.has_white) available4blacks++;
        if (H02 != HOLESTATE.has_white) available4blacks++;
        if (H20 != HOLESTATE.has_white) available4blacks++;
        if (H22 != HOLESTATE.has_white) available4blacks++;
    }

    static void available_box1(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks)
    {
        available4whites = 0;
        available4blacks = 0;

        HOLESTATE H00 = gb[Pentago_GameBoard.board_position_to_index(0, 0, square)];
        HOLESTATE H02 = gb[Pentago_GameBoard.board_position_to_index(0, 2, square)];
        HOLESTATE H20 = gb[Pentago_GameBoard.board_position_to_index(2, 0, square)];
        HOLESTATE H22 = gb[Pentago_GameBoard.board_position_to_index(2, 2, square)];
        HOLESTATE H10 = gb[Pentago_GameBoard.board_position_to_index(1, 0, square)];
        HOLESTATE H12 = gb[Pentago_GameBoard.board_position_to_index(1, 2, square)];
        HOLESTATE H01 = gb[Pentago_GameBoard.board_position_to_index(0, 1, square)];
        HOLESTATE H21 = gb[Pentago_GameBoard.board_position_to_index(2, 1, square)];

        if (H00 != HOLESTATE.has_black && H01 != HOLESTATE.has_black) available4whites++;
        if (H10 != HOLESTATE.has_black && H20 != HOLESTATE.has_black) available4whites++;
        if (H21 != HOLESTATE.has_black && H22 != HOLESTATE.has_black) available4whites++;
        if (H02 != HOLESTATE.has_black && H12 != HOLESTATE.has_black) available4whites++;

        if (H00 != HOLESTATE.has_white && H01 != HOLESTATE.has_white) available4blacks++;
        if (H10 != HOLESTATE.has_white && H20 != HOLESTATE.has_white) available4blacks++;
        if (H21 != HOLESTATE.has_white && H22 != HOLESTATE.has_white) available4blacks++;
        if (H02 != HOLESTATE.has_white && H12 != HOLESTATE.has_white) available4blacks++;

    }

    static void available_box2(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks)
    {
        available4whites = 0;
        available4blacks = 0;

        HOLESTATE H00 = gb[Pentago_GameBoard.board_position_to_index(0, 0, square)];
        HOLESTATE H02 = gb[Pentago_GameBoard.board_position_to_index(0, 2, square)];
        HOLESTATE H20 = gb[Pentago_GameBoard.board_position_to_index(2, 0, square)];
        HOLESTATE H22 = gb[Pentago_GameBoard.board_position_to_index(2, 2, square)];
        HOLESTATE H10 = gb[Pentago_GameBoard.board_position_to_index(1, 0, square)];
        HOLESTATE H12 = gb[Pentago_GameBoard.board_position_to_index(1, 2, square)];
        HOLESTATE H01 = gb[Pentago_GameBoard.board_position_to_index(0, 1, square)];
        HOLESTATE H21 = gb[Pentago_GameBoard.board_position_to_index(2, 1, square)];

        if (H00 != HOLESTATE.has_black && H10 != HOLESTATE.has_black) available4whites++;
        if (H20 != HOLESTATE.has_black && H21 != HOLESTATE.has_black) available4whites++;
        if (H22 != HOLESTATE.has_black && H12 != HOLESTATE.has_black) available4whites++;
        if (H02 != HOLESTATE.has_black && H01 != HOLESTATE.has_black) available4whites++;

        if (H00 != HOLESTATE.has_white && H10 != HOLESTATE.has_white) available4blacks++;
        if (H20 != HOLESTATE.has_white && H21 != HOLESTATE.has_white) available4blacks++;
        if (H22 != HOLESTATE.has_white && H12 != HOLESTATE.has_white) available4blacks++;
        if (H02 != HOLESTATE.has_white && H01 != HOLESTATE.has_white) available4blacks++;
    }



}

