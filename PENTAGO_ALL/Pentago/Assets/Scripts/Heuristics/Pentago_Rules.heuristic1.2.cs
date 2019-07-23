//use heuristicA(gb)*2 + heuristic1dot2(gb.board) 

using System.Linq;
using System.Runtime.CompilerServices;
using HOLESTATE = Pentago_GameBoard.hole_state;


public partial class Pentago_Rules
{
    //std
    float heuristic1dot2_own_possibilities_weigth = 1.0f;
    float heuristic1dot2_oponent_possibilities_weigth = 1.0f;
    float heuristic1dot2_own_strongChances_weigth = 1.0f;
    float heuristic1dot2_oponent_strongChances_weigth = 1.0f;

    //SETUPS:
    //active defensive and passive offense //will only focus on attacks when the chance appears
    public void setHeur12_AD_PO()
    {
        heuristic1dot2_own_possibilities_weigth = 4.0f;
        heuristic1dot2_oponent_possibilities_weigth = 3.0f;
        heuristic1dot2_own_strongChances_weigth = 2.0f;
        heuristic1dot2_oponent_strongChances_weigth = 8.0f;
    }
    //passive defense and active ofense (counter plays) //will focus on ofense unless the board looks pretty bad
    public void setHeur12_PD_AO()
    {
        heuristic1dot2_own_possibilities_weigth = 3.0f;
        heuristic1dot2_oponent_possibilities_weigth = 4.0f;
        heuristic1dot2_own_strongChances_weigth = 8.0f;
        heuristic1dot2_oponent_strongChances_weigth = 3.0f;
    }
    //ultra defensive //focus on defend
    public void setHeur12_UD()
    {
        heuristic1dot2_own_possibilities_weigth = 1.0f;
        heuristic1dot2_oponent_possibilities_weigth = 5.0f;
        heuristic1dot2_own_strongChances_weigth = 3.0f;
        heuristic1dot2_oponent_strongChances_weigth = 10.1f;
    }
    //ultra ofensive //focus on attack
    public void setHeur12_UO()
    {
        heuristic1dot2_own_possibilities_weigth = 5.0f;
        heuristic1dot2_oponent_possibilities_weigth = 1.0f;
        heuristic1dot2_own_strongChances_weigth = 8.1f;
        heuristic1dot2_oponent_strongChances_weigth = 3.0f;
    }
    //passive //priorityze possibilities
    public void setHeur12_P()
    {
        heuristic1dot2_own_possibilities_weigth = 10.0f;
        heuristic1dot2_oponent_possibilities_weigth = 10.0f;
        heuristic1dot2_own_strongChances_weigth = 6.0f;
        heuristic1dot2_oponent_strongChances_weigth = 4.0f;
    }
    public void setHeur12RANDOM()
    {
        System.Random random = new System.Random();        

        heuristic1dot2_own_possibilities_weigth = ( (float)random.Next(0, 1500)) / 100f;
        heuristic1dot2_oponent_possibilities_weigth = ( (float)random.Next(0, 1500)) / 100;
        heuristic1dot2_own_strongChances_weigth = ( (float)random.Next(0, 1500)) / 100;
        heuristic1dot2_oponent_strongChances_weigth = ( (float)random.Next(0, 1500)) / 100;
        h1_middle = ( (float)random.Next(0, 700)) / 100;
        h1_inner = ( (float)random.Next(0, 700)) / 100;
        h1_outer = ( (float)random.Next(0, 700)) / 100;
        h1_main_Diag = ( (float)random.Next(0, 700)) / 100;
        h1_other_Diag = ( (float)random.Next(0, 700)) / 100;
        System.Console.WriteLine(
            heuristic1dot2_own_possibilities_weigth
            + " , " + heuristic1dot2_oponent_possibilities_weigth
            + " , " + heuristic1dot2_own_strongChances_weigth
            + " , " + heuristic1dot2_oponent_strongChances_weigth
            + " , " + h1_middle
            + " , " + h1_inner
            + " , " + h1_outer
            + " , " + h1_main_Diag
            + " , " + h1_other_Diag
            );

    }

   /*     public void setcustom()
    {
        //-9,49 , 8,38 , 5,58 , -1,88 , 2,3 , 0,85 , -0,36 , 1,85 , -0,09

        heuristic1dot2_own_possibilities_weigth = -9.49f;
        heuristic1dot2_oponent_possibilities_weigth = 8.38f;
        heuristic1dot2_own_strongChances_weigth = 5.58f;
        heuristic1dot2_oponent_strongChances_weigth = -1.88f;
        h1_middle = 12.3f;
        h1_inner = 0.85f;
        h1_outer = -0.36f;
        h1_main_Diag = 1.85f;
        h1_other_Diag = -0.09f;
    }*/

    //USING DIAGONAL HACK(no longer the 1.2 heuristic if used
    //highly improves winning rate when playing 1st (combined with A)
    /* float heuristic1dot2_own_possibilities_weigth = 0.0f;
     float heuristic1dot2_oponent_possibilities_weigth = 0.0f;
     float heuristic1dot2_own_strongChances_weigth = 1.0f;
     float heuristic1dot2_oponent_strongChances_weigth = 0.0f;*/
    //highly improves winning rate when playing 2nd (combined with A)
    /* float heuristic1dot2_own_possibilities_weigth = 0.0f;
     float heuristic1dot2_oponent_possibilities_weigth = 0.0f;
     float heuristic1dot2_own_strongChances_weigth = 1.0f;
     float heuristic1dot2_oponent_strongChances_weigth = 2.0f;*/

    void setUpDiagonalHack()
    {
        HEUR12RELAXED = true;
        diagonal_hack = true;
        if (IA_PIECES == IA_PIECES_WHITES) //if plays first
        {
            heuristic1dot2_own_possibilities_weigth = 0.0f;
            heuristic1dot2_oponent_possibilities_weigth = 0.0f;
            heuristic1dot2_own_strongChances_weigth = 1.0f;
            heuristic1dot2_oponent_strongChances_weigth = 0.0f;
            return;
        }
        heuristic1dot2_own_possibilities_weigth = 0.0f;
        heuristic1dot2_oponent_possibilities_weigth = 0.0f;
        heuristic1dot2_own_strongChances_weigth = 1.0f;
        heuristic1dot2_oponent_strongChances_weigth = 2.0f;
    }


    public bool HEUR12RELAXED = true;
    bool diagonal_hack = false; //only get diagonals info


    /// <summary>
    /// set biases for the 1dot2 heuristic
    /// </summary>
    /// <param name="bias1">set heuristic1dot2_own_possibilities_weigth</param>
    /// <param name=""> set heuristic1dot2_oponent_possibilities_weigth</param>
    /// <param name="">set heuristic1dot2_own_strongChances_weigth</param>
    /// <param name="">set heuristic1dot2_oponent_strongChances_weigth</param>
    public void setHeuristic1dot2Biases(float bias1, float bias2, float bias3, float bias4)
    {
        heuristic1dot2_own_possibilities_weigth = bias1;
        heuristic1dot2_oponent_possibilities_weigth = bias2;
        heuristic1dot2_own_strongChances_weigth = bias3;
        heuristic1dot2_oponent_strongChances_weigth = bias4;
    }

    /// <summary>
    /// gets utility of a gameboard using heuristic1
    /// </summary>
    /// <param name="gb"></param>
    /// <returns></returns>
    public float heuristic1dot2(HOLESTATE[] gb)
    {
        if (weights == null) weights = new float[] { h1_middle, h1_inner, h1_outer, h1_main_Diag, h1_other_Diag };
        float whites, blacks, whitesS, blacksS;
        calculate_available_classes2(gb, out whites, out blacks, out whitesS, out blacksS, weights);

        float value;
        if (IA_PIECES == IA_PIECES_WHITES) value =
           + (whites) * heuristic1dot2_own_possibilities_weigth - (blacks) * heuristic1dot2_oponent_possibilities_weigth
            + (whitesS) * heuristic1dot2_own_strongChances_weigth - (blacksS) * heuristic1dot2_oponent_strongChances_weigth
            ;
        else value =
           -(whites) * heuristic1dot2_oponent_possibilities_weigth + (blacks) * heuristic1dot2_own_possibilities_weigth
            - (whitesS) * heuristic1dot2_oponent_strongChances_weigth + (blacksS) * heuristic1dot2_own_strongChances_weigth
            ;

        
        value = value / (
            heuristic1dot2_own_possibilities_weigth + heuristic1dot2_oponent_possibilities_weigth 
           + heuristic1dot2_own_strongChances_weigth + heuristic1dot2_oponent_strongChances_weigth
           );

        return value;
    }



    public void calculate_available_classes2(Pentago_GameBoard gb, out float available4whites, out float available4blacks, out float strongWhites, out float strongBlacks, float[] weights)
    {
        calculate_available_classes2(gb.board, out available4whites, out available4blacks, out strongWhites, out strongBlacks, weights);
    }

    void calculate_available_classes2(HOLESTATE[] gb, out float available4whites, out float available4blacks, out float strongWhites, out float strongBlacks, float[] weights)
    {
        //int[] allsquares = new int[] { 0, 1, 2, 3 };
        /* available4whites = 0;
         available4blacks = 0;
         strongWhites = 0;
         strongBlacks = 0;*/

        int[] L_P1 = new int[6]; //L1_1, L2_1, L3_1, L4_1, L5_1, L6_1; //lines 4 whites
        int[] L_P2 = new int[6]; //L1_2, L2_2, L3_2, L4_2, L5_2, L6_2; //lines 4 blacks
        int[] R_P1 = new int[6];//R1_1, R2_1, R3_1, R4_1, R5_1, R6_1;//rows...
        int[] R_P2 = new int[6];//R1_2, R2_2, R3_2, R4_2, R5_2, R6_2;
        int[] D_1 = new int[6];//DLC_1, DLD_1, DLU_1, DRC_1, DRD_1, DRU_1; //D diagonal | L left start, R right start | D down start, U up start
        int[] D_2 = new int[6]; //DLC_2, DLD_2, DLU_2, DRC_2, DRD_2, DRU_2; //D diagonal | L left start, R right start | D down start, U up start

        int[] L_P1M = new int[6]; //L1_1, L2_1, L3_1, L4_1, L5_1, L6_1; //lines 4 whites
        int[] L_P2M = new int[6]; //L1_2, L2_2, L3_2, L4_2, L5_2, L6_2; //lines 4 blacks
        int[] R_P1M = new int[6];//R1_1, R2_1, R3_1, R4_1, R5_1, R6_1;//rows...
        int[] R_P2M = new int[6];//R1_2, R2_2, R3_2, R4_2, R5_2, R6_2;
        int[] D_1M = new int[6];//DLC_1, DLD_1, DLU_1, DRC_1, DRD_1, DRU_1; //D diagonal | L left start, R right start | D down start, U up start
        int[] D_2M = new int[6]; //DLC_2, DLD_2, DLU_2, DRC_2, DRD_2, DRU_2; //D diagonal | L left start, R right start | D down start, U up start

        //calculate aux stuff first --------------------------------------

        //pluscross 1,2,3,4 P1 and P2
        int[] pluscross_1 = new int[4];
        int[] pluscross_2 = new int[4];
        int[] pluscross_1m = new int[4];
        int[] pluscross_2m = new int[4];
        int[] pluscross_1sm = new int[4];
        int[] pluscross_2sm = new int[4];
        if (!diagonal_hack)
        {
            for (int i = 0; i < 4; i++)
                available_pluscross_v2(gb, i, out pluscross_1[i], out pluscross_2[i],
                    out pluscross_1m[i], out pluscross_2m[i], out pluscross_1sm[i], out pluscross_2sm[i]);
        }

        //mulcross 1,2,3,4 P1 and P2
        int[] mulcross_1 = new int[4];
        int[] mulcross_2 = new int[4];
        int[] mulcross_1m = new int[4];
        int[] mulcross_2m = new int[4];
        int[] mulcross_1sm = new int[4];
        int[] mulcross_2sm = new int[4];
        for (int i = 0; i < 4; i++)
            available_mulcross_v2(gb, i, out mulcross_1[i], out mulcross_2[i],
               out mulcross_1m[i], out mulcross_2m[i], out mulcross_1sm[i], out mulcross_2sm[i]);


        //diamond 1,2,3,4 P1 and P2
        int[] diamondcross_1 = new int[4];
        int[] diamondcross_2 = new int[4];
        int[] diamondcross_1m = new int[4];
        int[] diamondcross_2m = new int[4];
        for (int i = 0; i < 4; i++)
            available_diamond_v2(gb, i, out diamondcross_1[i], out diamondcross_2[i],
              out diamondcross_1m[i], out diamondcross_2m[i]);

        //box1 1,2,3,4 P1 and P2
        int[] box1_1 = new int[4];
        int[] box1_2 = new int[4];
        int[] box1_1m = new int[4];
        int[] box1_2m = new int[4];
        int[] box1_1ms = new int[4];
        int[] box1_2ms = new int[4];
        if (!diagonal_hack)
        {

            for (int i = 0; i < 4; i++)
                available_box1_v2(gb, i, out box1_1[i], out box1_2[i],
                    out box1_1m[i], out box1_2m[i], out box1_1ms[i], out box1_2ms[i]);
        }

        //box2 1,2,3,4 P1 and P2
        int[] box2_1 = new int[4];
        int[] box2_2 = new int[4];
        int[] box2_1m = new int[4];
        int[] box2_2m = new int[4];
        int[] box2_1ms = new int[4];
        int[] box2_2ms = new int[4];
        if (!diagonal_hack)
        {
            for (int i = 0; i < 4; i++)
                available_box2_v2(gb, i, out box2_1[i], out box2_2[i],
                    out box2_1m[i], out box2_2m[i], out box2_1ms[i], out box2_2ms[i]);
        }

        //corners 1,2,3,4 P1 and P2
        int[] corners_1 = new int[4];
        int[] corners_2 = new int[4];
        int[] corners_1m = new int[4];
        int[] corners_2m = new int[4];
        if (!diagonal_hack)
        {
            for (int i = 0; i < 4; i++)
                available_corners_v2(gb, i, out corners_1[i], out corners_2[i], out corners_1m[i], out corners_2m[i]);
        }

        if (!diagonal_hack)
        {
            //CALCULATE 4 LINES
            L_P1[0] = select_min(box1_1[0], box2_1[1]);
            L_P1[1] = select_min(pluscross_1[0], pluscross_1[1]);
            L_P1[2] = select_min(box2_1[0], box1_1[1]);
            L_P1[3] = select_min(box1_1[2], box2_1[3]);
            L_P1[4] = select_min(pluscross_1[2], pluscross_1[3]);
            L_P1[5] = select_min(box2_1[2], box1_1[3]);

            L_P1M[0] = select_max(select_min(box1_1m[0], box2_1ms[1]), select_min(box1_1ms[0], box2_1m[1]));
            L_P1M[1] = select_max(select_min(pluscross_1m[0], pluscross_1sm[1]), select_min(pluscross_1sm[0], pluscross_1m[1]));
            L_P1M[2] = select_max(select_min(box2_1m[0], box1_1ms[1]), select_min(box2_1ms[0], box1_1[1]));
            L_P1M[3] = select_max(select_min(box1_1m[2], box2_1ms[3]), select_min(box1_1ms[2], box2_1m[3]));
            L_P1M[4] = select_max(select_min(pluscross_1m[2], pluscross_1sm[3]), select_min(pluscross_1sm[2], pluscross_1m[3]));
            L_P1M[5] = select_max(select_min(box2_1m[2], box1_1ms[3]), select_min(box2_1ms[2], box1_1m[3]));

            L_P2[0] = select_min(box1_2[0], box2_2[1]);
            L_P2[1] = select_min(pluscross_2[0], pluscross_2[1]);
            L_P2[2] = select_min(box2_2[0], box1_2[1]);
            L_P2[3] = select_min(box1_2[2], box2_2[3]);
            L_P2[4] = select_min(pluscross_2[2], pluscross_2[3]);
            L_P2[5] = select_min(box2_2[2], box1_2[3]);

            L_P2M[0] = select_max(select_min(box1_2m[0], box2_2ms[1]), select_min(box1_2ms[0], box2_2m[1]));
            L_P2M[1] = select_max(select_min(pluscross_2m[0], pluscross_2sm[1]), select_min(pluscross_2sm[0], pluscross_2m[1]));
            L_P2M[2] = select_max(select_min(box2_2m[0], box1_2ms[1]), select_min(box2_2ms[0], box1_2[1]));
            L_P2M[3] = select_max(select_min(box1_2m[2], box2_2ms[3]), select_min(box1_2ms[2], box2_2m[3]));
            L_P2M[4] = select_max(select_min(pluscross_2m[2], pluscross_2sm[3]), select_min(pluscross_2sm[2], pluscross_2m[3]));
            L_P2M[5] = select_max(select_min(box2_2m[2], box1_2ms[3]), select_min(box2_2ms[2], box1_2m[3]));

            //CALCULATE 4 ROWS

            R_P1[0] = select_min(box2_1[0], box1_1[2]);
            R_P1[1] = select_min(pluscross_1[0], pluscross_1[2]);
            R_P1[2] = select_min(box1_1[0], box2_1[2]);
            R_P1[3] = select_min(box2_1[1], box1_1[3]);
            R_P1[4] = select_min(pluscross_1[1], pluscross_1[3]);
            R_P1[5] = select_min(box1_1[1], box2_1[3]);

            R_P1M[0] = select_max(select_min(box2_1m[0], box1_1ms[2]), select_min(box2_1ms[0], box1_1m[2]));
            R_P1M[1] = select_max(select_min(pluscross_1m[0], pluscross_1sm[2]), select_min(pluscross_1sm[0], pluscross_1m[2]));
            R_P1M[2] = select_max(select_min(box1_1m[0], box2_1ms[2]), select_min(box1_1ms[0], box2_1m[2]));
            R_P1M[3] = select_max(select_min(box2_1m[1], box1_1ms[3]), select_min(box2_1ms[1], box1_1m[3]));
            R_P1M[4] = select_max(select_min(pluscross_1m[1], pluscross_1sm[3]), select_min(pluscross_1sm[1], pluscross_1m[3]));
            R_P1M[5] = select_max(select_min(box1_1m[1], box2_1ms[3]), select_min(box1_1ms[1], box2_1m[3]));

            R_P2[0] = select_min(box2_2[0], box1_2[2]);
            R_P2[1] = select_min(pluscross_2[0], pluscross_2[2]);
            R_P2[2] = select_min(box1_2[0], box2_2[2]);
            R_P2[3] = select_min(box2_2[1], box1_2[3]);
            R_P2[4] = select_min(pluscross_2[1], pluscross_2[3]);
            R_P2[5] = select_min(box1_2[1], box2_2[3]);

            R_P2M[0] = select_max(select_min(box2_2m[0], box1_2ms[2]), select_min(box2_2ms[0], box1_2m[2]));
            R_P2M[1] = select_max(select_min(pluscross_2m[0], pluscross_2sm[2]), select_min(pluscross_2sm[0], pluscross_2m[2]));
            R_P2M[2] = select_max(select_min(box1_2m[0], box2_2ms[2]), select_min(box1_2ms[0], box2_2m[2]));
            R_P2M[3] = select_max(select_min(box2_2m[1], box1_2ms[3]), select_min(box2_2ms[1], box1_2m[3]));
            R_P2M[4] = select_max(select_min(pluscross_2m[1], pluscross_2sm[3]), select_min(pluscross_2sm[1], pluscross_2m[3]));
            R_P2M[5] = select_max(select_min(box1_2m[1], box2_2ms[3]), select_min(box1_2ms[1], box2_2m[3]));
        }
        //CALCULATE 4 DIAGONALS
        D_1[0] = select_min(mulcross_1[0], mulcross_1[3]);
        D_1[1] = select_min(diamondcross_1[0], diamondcross_1[3], corners_1[1]);
        D_1[2] = select_min(diamondcross_1[0], diamondcross_1[3], corners_1[2]);
        D_1[3] = select_min(mulcross_1[1], mulcross_1[2]);
        D_1[4] = select_min(diamondcross_1[1], diamondcross_1[2], corners_1[3]);
        D_1[5] = select_min(diamondcross_1[1], diamondcross_1[2], corners_1[0]);

        D_1M[0] = select_max(select_min(mulcross_1m[0], mulcross_1sm[3]), select_min(mulcross_1sm[0], mulcross_1m[3]));
        D_1M[1] = select_min(diamondcross_1m[0], diamondcross_1m[3], corners_1m[1]);
        D_1M[2] = select_min(diamondcross_1m[0], diamondcross_1m[3], corners_1m[2]);
        D_1M[3] = select_max(select_min(mulcross_1m[1], mulcross_1sm[2]), select_min(mulcross_1sm[1], mulcross_1m[2]));
        D_1M[4] = select_min(diamondcross_1m[1], diamondcross_1m[2], corners_1m[3]);
        D_1M[5] = select_min(diamondcross_1m[1], diamondcross_1m[2], corners_1m[0]);

        D_2[0] = select_min(mulcross_2[0], mulcross_2[3]);
        D_2[1] = select_min(diamondcross_2[0], diamondcross_2[3], corners_2[1]);
        D_2[2] = select_min(diamondcross_2[0], diamondcross_2[3], corners_2[2]);
        D_2[3] = select_min(mulcross_2[1], mulcross_2[2]);
        D_2[4] = select_min(diamondcross_2[1], diamondcross_2[2], corners_2[3]);
        D_2[5] = select_min(diamondcross_2[1], diamondcross_2[2], corners_2[0]);

        D_2M[0] = select_max(select_min(mulcross_2m[0], mulcross_2sm[3]), select_min(mulcross_2sm[0], mulcross_2m[3]));
        D_2M[1] = select_min(diamondcross_2m[0], diamondcross_2m[3], corners_2m[1]);
        D_2M[2] = select_min(diamondcross_2m[0], diamondcross_2m[3], corners_2m[2]);
        D_2M[3] = select_max(select_min(mulcross_2m[1], mulcross_2sm[2]), select_min(mulcross_2sm[1], mulcross_2m[2]));
        D_2M[4] = select_min(diamondcross_2m[1], diamondcross_2m[2], corners_2m[3]);
        D_2M[5] = select_min(diamondcross_2m[1], diamondcross_2m[2], corners_2m[0]);

        if (!diagonal_hack)
        {
            //old
            //available4whites = L_P1.Sum() + R_P1.Sum() + D_1.Sum();
            //available4blacks = L_P2.Sum() + R_P2.Sum() + D_2.Sum();
            //strongWhites = L_P1M.Sum() + R_P1M.Sum() + D_1M.Sum();
           // strongBlacks = L_P2M.Sum() + R_P2M.Sum() + D_2M.Sum();

            available4whites =
                        (L_P1[1] + L_P1[4] + R_P1[1] + R_P1[1]) * weights[0]//middle
                + (L_P1[2] + L_P1[3] + R_P1[2] + R_P1[3]) * weights[1]//inner 
        + (L_P1[0] + L_P1[5] + R_P1[0] + R_P1[5]) * weights[2]//outer 
           + (D_1[0] + D_1[3]) * weights[3]//main diags
        + (D_1[1] + D_1[2] + D_1[4] + D_1[5]) * weights[4];//other diags

            available4blacks = (L_P2[1] + L_P2[4] + R_P2[1] + R_P2[1]) * weights[0]//middle
                + (L_P2[2] + L_P2[3] + R_P2[2] + R_P2[3]) * weights[1]//inner 
        + (L_P2[0] + L_P2[5] + R_P2[0] + R_P2[5]) * weights[2]//outer 
           + (D_2[0] + D_2[3]) * weights[3]//main diags
        + (D_2[1] + D_2[2] + D_2[4] + D_2[5]) * weights[4];

             strongWhites =
                 (L_P1M[1] + L_P1M[4] + R_P1M[1] + R_P1M[1]) * weights[0]//middle
                 + (L_P1M[2] + L_P1M[3] + R_P1M[2] + R_P1M[3]) * weights[1]//inner 
                 + (L_P1M[0] + L_P1M[5] + R_P1M[0] + R_P1M[5]) * weights[2]//outer 
                 + (D_1M[0] + D_1M[3]) * weights[3]//main diags
                 + (D_1M[1] + D_1M[2] + D_1M[4] + D_1M[5]) * weights[4];//other diags

             strongBlacks =
             (L_P2M[1] + L_P2M[4] + R_P2M[1] + R_P2M[1]) * weights[0]//middle
         + (L_P2M[2] + L_P2M[3] + R_P2M[2] + R_P2M[3]) * weights[1]//inner 
         + (L_P2M[0] + L_P2M[5] + R_P2M[0] + R_P2M[5]) * weights[2]//outer 
         + (D_2M[0] + D_2M[3]) * weights[3]//main diags
         + (D_2M[1] + D_2M[2] + D_2M[4] + D_2M[5]) * weights[4];//other diags*/

        }
        else
        {
            available4whites = D_1[0] + D_1[3];
            available4blacks = D_2[0] + D_2[3];
            strongWhites = D_1M[0] + D_1M[3];
            strongBlacks = D_2M[0] + D_2M[3];
        }
    }


    void available_pluscross_v2(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks, out int made4whites, out int made4blacks, out int stronglymade4whites, out int stronglymade4blacks)
    {
        available4whites = 0;
        available4blacks = 0;
        made4whites = 0;
        made4blacks = 0;
        stronglymade4whites = 0;
        stronglymade4blacks = 0;

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

        if (!HEUR12RELAXED)
        {
            bool h11hasB = H11 == HOLESTATE.has_black;
            if (h11hasB && H10 == HOLESTATE.has_black) { made4blacks++; if (H12 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H12 == HOLESTATE.has_black) { made4blacks++; if (H10 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H01 == HOLESTATE.has_black) { made4blacks++; if (H21 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H21 == HOLESTATE.has_black) { made4blacks++; if (H01 != HOLESTATE.has_white) stronglymade4blacks++; }

            bool h11hasW = H11 == HOLESTATE.has_white;
            if (h11hasW && H10 == HOLESTATE.has_white) { made4whites++; if (H12 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H12 == HOLESTATE.has_white) { made4whites++; if (H10 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H01 == HOLESTATE.has_white) { made4whites++; if (H21 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H21 == HOLESTATE.has_white) { made4whites++; if (H01 != HOLESTATE.has_black) stronglymade4whites++; }
        }
        else {
            bool h11hasB = H11 == HOLESTATE.has_black;
            if (h11hasB && H10 != HOLESTATE.has_white) { made4blacks++; if (H12 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H12 != HOLESTATE.has_white) { made4blacks++; if (H10 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H01 != HOLESTATE.has_white) { made4blacks++; if (H21 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H21 != HOLESTATE.has_white) { made4blacks++; if (H01 != HOLESTATE.has_white) stronglymade4blacks++; }

            bool h11hasW = H11 == HOLESTATE.has_white;
            if (h11hasW && H10 != HOLESTATE.has_black) { made4whites++; if (H12 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H12 != HOLESTATE.has_black) { made4whites++; if (H10 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H01 != HOLESTATE.has_black) { made4whites++; if (H21 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H21 != HOLESTATE.has_black) { made4whites++; if (H01 != HOLESTATE.has_black) stronglymade4whites++; }
        }
    }

    void available_mulcross_v2(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks, out int made4whites, out int made4blacks, out int stronglymade4whites, out int stronglymade4blacks)
    {
        available4whites = 0;
        available4blacks = 0;
        made4whites = 0;
        made4blacks = 0;
        stronglymade4whites = 0;
        stronglymade4blacks = 0;

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

        bool h11hasB = H11 == HOLESTATE.has_black;

        if (!HEUR12RELAXED)
        {
            if (h11hasB && H00 == HOLESTATE.has_black) { made4blacks++; if (H22 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H02 == HOLESTATE.has_black) { made4blacks++; if (H20 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H20 == HOLESTATE.has_black) { made4blacks++; if (H02 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H22 == HOLESTATE.has_black) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }

            bool h11hasW = H11 == HOLESTATE.has_white;
            if (h11hasW && H00 == HOLESTATE.has_white) { made4whites++; if (H22 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H02 == HOLESTATE.has_white) { made4whites++; if (H20 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H20 == HOLESTATE.has_white) { made4whites++; if (H02 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H22 == HOLESTATE.has_white) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
        }
        else {
            if (h11hasB && H00 != HOLESTATE.has_white) { made4blacks++; if (H22 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H02 != HOLESTATE.has_white) { made4blacks++; if (H20 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H20 != HOLESTATE.has_white) { made4blacks++; if (H02 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (h11hasB && H22 != HOLESTATE.has_white) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }

            bool h11hasW = H11 != HOLESTATE.has_white;
            if (h11hasW && H00 != HOLESTATE.has_black) { made4whites++; if (H22 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H02 != HOLESTATE.has_black) { made4whites++; if (H20 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H20 != HOLESTATE.has_black) { made4whites++; if (H02 != HOLESTATE.has_black) stronglymade4whites++; }
            if (h11hasW && H22 != HOLESTATE.has_black) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
        }

    }

    static void available_diamond_v2(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks, out int made4whites, out int made4blacks)
    {
        available4whites = 0;
        available4blacks = 0;
        made4whites = 0;
        made4blacks = 0;

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

        if (H10 == HOLESTATE.has_black) made4blacks++;
        if (H12 == HOLESTATE.has_black) made4blacks++;
        if (H01 == HOLESTATE.has_black) made4blacks++;
        if (H21 == HOLESTATE.has_black) made4blacks++;

        if (H10 == HOLESTATE.has_white) made4whites++;
        if (H12 == HOLESTATE.has_white) made4whites++;
        if (H01 == HOLESTATE.has_white) made4whites++;
        if (H21 == HOLESTATE.has_white) made4whites++;
    }

    static void available_corners_v2(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks, out int made4whites, out int made4blacks)
    {
        available4whites = 0;
        available4blacks = 0;
        made4whites = 0;
        made4blacks = 0;

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

        if (H00 == HOLESTATE.has_black) made4blacks++;
        if (H02 == HOLESTATE.has_black) made4blacks++;
        if (H20 == HOLESTATE.has_black) made4blacks++;
        if (H22 == HOLESTATE.has_black) made4blacks++;

        if (H00 == HOLESTATE.has_white) made4whites++;
        if (H02 == HOLESTATE.has_white) made4whites++;
        if (H20 == HOLESTATE.has_white) made4whites++;
        if (H22 == HOLESTATE.has_white) made4whites++;
    }

    void available_box1_v2(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks, out int made4whites, out int made4blacks, out int stronglymade4whites, out int stronglymade4blacks)
    {
        available4whites = 0;
        available4blacks = 0;
        made4whites = 0;
        made4blacks = 0;
        stronglymade4whites = 0;
        stronglymade4blacks = 0;

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

        if (!HEUR12RELAXED)
        {
            if (H00 == HOLESTATE.has_black && H01 == HOLESTATE.has_black) { made4blacks++; if (H02 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (H10 == HOLESTATE.has_black && H20 == HOLESTATE.has_black) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (H21 == HOLESTATE.has_black && H22 == HOLESTATE.has_black) { made4blacks++; if (H20 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (H02 == HOLESTATE.has_black && H12 == HOLESTATE.has_black) { made4blacks++; if (H22 != HOLESTATE.has_white) stronglymade4blacks++; }

            if (H00 == HOLESTATE.has_white && H01 == HOLESTATE.has_white) { made4whites++; if (H02 != HOLESTATE.has_black) stronglymade4whites++; }
            if (H10 == HOLESTATE.has_white && H20 == HOLESTATE.has_white) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
            if (H21 == HOLESTATE.has_white && H22 == HOLESTATE.has_white) { made4whites++; if (H20 != HOLESTATE.has_black) stronglymade4whites++; }
            if (H02 == HOLESTATE.has_white && H12 == HOLESTATE.has_white) { made4whites++; if (H22 != HOLESTATE.has_black) stronglymade4whites++; }

        }
        else {
            if ((H00 == HOLESTATE.has_black || H01 == HOLESTATE.has_black) && H00 != HOLESTATE.has_white && H01 != HOLESTATE.has_white) { made4blacks++; if (H02 != HOLESTATE.has_white) stronglymade4blacks++; }
            if ((H10 == HOLESTATE.has_black || H20 == HOLESTATE.has_black) && H10 != HOLESTATE.has_white && H20 != HOLESTATE.has_white) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }
            if ((H21 == HOLESTATE.has_black || H22 == HOLESTATE.has_black) && H21 != HOLESTATE.has_white && H22 != HOLESTATE.has_white) { made4blacks++; if (H20 != HOLESTATE.has_white) stronglymade4blacks++; }
            if ((H02 == HOLESTATE.has_black || H12 == HOLESTATE.has_black) && H02 != HOLESTATE.has_white && H12 != HOLESTATE.has_white) { made4blacks++; if (H22 != HOLESTATE.has_white) stronglymade4blacks++; }

            if ((H00 == HOLESTATE.has_white || H01 == HOLESTATE.has_white) && H00 != HOLESTATE.has_black && H01 != HOLESTATE.has_black) { made4whites++; if (H02 != HOLESTATE.has_black) stronglymade4whites++; }
            if ((H10 == HOLESTATE.has_white || H20 == HOLESTATE.has_white) && H10 != HOLESTATE.has_black && H20 != HOLESTATE.has_black) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
            if ((H21 == HOLESTATE.has_white || H22 == HOLESTATE.has_white) && H21 != HOLESTATE.has_black && H22 != HOLESTATE.has_black) { made4whites++; if (H20 != HOLESTATE.has_black) stronglymade4whites++; }
            if ((H02 == HOLESTATE.has_white || H12 == HOLESTATE.has_white) && H02 != HOLESTATE.has_black && H12 != HOLESTATE.has_black) { made4whites++; if (H22 != HOLESTATE.has_black) stronglymade4whites++; }
        }
    }

    void available_box2_v2(HOLESTATE[] gb, int square, out int available4whites, out int available4blacks, out int made4whites, out int made4blacks, out int stronglymade4whites, out int stronglymade4blacks)
    {
        available4whites = 0;
        available4blacks = 0;
        made4whites = 0;
        made4blacks = 0;
        stronglymade4whites = 0;
        stronglymade4blacks = 0;

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

        if (!HEUR12RELAXED)
        {
            if (H00 == HOLESTATE.has_black && H10 == HOLESTATE.has_black) { made4blacks++; if (H20 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (H20 == HOLESTATE.has_black && H21 == HOLESTATE.has_black) { made4blacks++; if (H22 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (H22 == HOLESTATE.has_black && H12 == HOLESTATE.has_black) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }
            if (H02 == HOLESTATE.has_black && H01 == HOLESTATE.has_black) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }

            if (H00 == HOLESTATE.has_white && H10 == HOLESTATE.has_white) { made4whites++; if (H20 != HOLESTATE.has_black) stronglymade4whites++; }
            if (H20 == HOLESTATE.has_white && H21 == HOLESTATE.has_white) { made4whites++; if (H22 != HOLESTATE.has_black) stronglymade4whites++; }
            if (H22 == HOLESTATE.has_white && H12 == HOLESTATE.has_white) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
            if (H02 == HOLESTATE.has_white && H01 == HOLESTATE.has_white) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
        }
        else {
            if ((H00 == HOLESTATE.has_black || H10 == HOLESTATE.has_black) && H00 != HOLESTATE.has_white && H10 != HOLESTATE.has_white) { made4blacks++; if (H20 != HOLESTATE.has_white) stronglymade4blacks++; }
            if ((H20 == HOLESTATE.has_black || H21 == HOLESTATE.has_black) && H20 != HOLESTATE.has_white && H21 != HOLESTATE.has_white) { made4blacks++; if (H22 != HOLESTATE.has_white) stronglymade4blacks++; }
            if ((H22 == HOLESTATE.has_black || H12 == HOLESTATE.has_black) && H22 != HOLESTATE.has_white && H12 != HOLESTATE.has_white) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }
            if ((H02 == HOLESTATE.has_black || H01 == HOLESTATE.has_black) && H02 != HOLESTATE.has_white && H01 != HOLESTATE.has_white) { made4blacks++; if (H00 != HOLESTATE.has_white) stronglymade4blacks++; }

            if ((H00 == HOLESTATE.has_white || H10 == HOLESTATE.has_white) && H00 != HOLESTATE.has_black && H10 != HOLESTATE.has_black) { made4whites++; if (H20 != HOLESTATE.has_black) stronglymade4whites++; }
            if ((H20 == HOLESTATE.has_white || H21 == HOLESTATE.has_white) && H20 != HOLESTATE.has_black && H21 != HOLESTATE.has_black) { made4whites++; if (H22 != HOLESTATE.has_black) stronglymade4whites++; }
            if ((H22 == HOLESTATE.has_white || H12 == HOLESTATE.has_white) && H22 != HOLESTATE.has_black && H12 != HOLESTATE.has_black) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
            if ((H02 == HOLESTATE.has_white || H01 == HOLESTATE.has_white) && H02 != HOLESTATE.has_black && H01 != HOLESTATE.has_black) { made4whites++; if (H00 != HOLESTATE.has_black) stronglymade4whites++; }
        }

    }



}

