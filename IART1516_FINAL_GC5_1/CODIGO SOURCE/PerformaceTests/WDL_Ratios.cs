using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MINMAX = MinMax<Pentago_GameBoard, Pentago_Move>;
using EVFC = Pentago_Rules.EvaluationFunction;

public static class WDL_Ratios
{

    const bool test_with_control_auto = false;
    const bool test_with_others_auto_suite1 = false;
    const bool test_with_others_auto_suite2 = true;
    const bool test_with_control_d6 = false;
    const bool test_with_others_d6 = false;

    const int number_of_games_div4_pertest = 100;
    //assuming we use prev var with 2500;
    //total of 10.000 games per test... 
    //lets supose we have 40 tests of 1 second each -> 40.000 secs = 11.11... hours
    //lets supose we have 40 tests of 10 second each -> 400.000 secs = 111.11... hours = ~5 days
    //lets assume we have 40 tests of 1 minute each (=prev*60) ->  24.000.00 sec = 666,66... hours = ~28 days
    //lets assume we have 40 tests of 10 minutes each (=prev*600) -> 24.000.000 sec = 6666,66... hours = ~278 days

    static void printLatexTableHeader()
    {
        Console.WriteLine("dv4 tests:" + number_of_games_div4_pertest);
        Console.WriteLine();
        Console.WriteLine("\\begin{table}[]");
        Console.WriteLine("\\centering");
        Console.WriteLine("\\resizebox{\\columnwidth}{!} {");
        Console.WriteLine("\\setlength\\tabcolsep{ 1.5pt}");
        Console.WriteLine("\\begin{tabular}{|c|c|c|c|c|c|c|c|c|c|c|c|c|c|}");
        Console.WriteLine("\\hline");
        Console.WriteLine(" &  & \\multicolumn{4}{c|}{Tabuleio Inicial Vazio} & \\multicolumn{4}{c|}{Tabuleiro Inicial Aleatorio} & \\multicolumn{4}{c|}{Total} \\\\ \\cline{3-14}");
        //Console.WriteLine("\\multirow{-2}{*}{Joga com Brancas} & \\multirow{-2}{*}{Joga com Pretas} & {\\color[HTML]{00009B} Vit} & {\\color[HTML]{9A0000} Der} & {\\color[HTML]{009901} Emp} & Tur & {\\color[HTML]{00009B} Vit} & {\\color[HTML]{9A0000} Der} & {\\color[HTML]{009901} Emp} & Tur & {\\color[HTML]{00009B} Vit} & {\\color[HTML]{9A0000} Der} & {\\color[HTML]{009901} Emp} & Tur \\\\ \\hline");
        Console.WriteLine("\\multirow{-2}{*}{Joga com Brancas} & \\multirow{-2}{*}{Joga com Pretas} & {\\color[HTML]{00009B} Vit\\perthousand} & {\\color[HTML]{9A0000} Der\\perthousand} & {\\color[HTML]{009901} Emp\\perthousand} & Tur & {\\color[HTML]{00009B} Vit\\perthousand} & {\\color[HTML]{9A0000} Der\\perthousand} & {\\color[HTML]{009901} Emp\\perthousand} & Tur & {\\color[HTML]{00009B} Vit\\perthousand} & {\\color[HTML]{9A0000} Der\\perthousand} & {\\color[HTML]{009901} Emp\\perthousand} & Tur \\\\ \\hline");
    }

    static void printLatexTableFooter()
    {
        Console.WriteLine(
"\\end{tabular}" + "}"
+ " \\caption{ My caption}"
+ " \\label{ my - label}"
+ " \\end{table}"
            );
    }

    static void printseparator()
    {
        Console.WriteLine();
        //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
        //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
    }

    static Pentago_GameBoard[] testBoards;
    public static void RUN()
    {
        testBoards = new Pentago_GameBoard[number_of_games_div4_pertest * 2];

        int i = 0;
        for (; i < number_of_games_div4_pertest; ++i)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(1, 12);
            GenerateRandomBoard rndBoard = new GenerateRandomBoard(numPieces, true);
            rndBoard.generateNewBoard();
            testBoards[i] = rndBoard.Pentago_gb;
        }

        int aux = number_of_games_div4_pertest * 2;
        for (; i < aux; ++i)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(1, 12);
            GenerateRandomBoard rndBoard = new GenerateRandomBoard(numPieces, false);
            rndBoard.generateNewBoard();
            testBoards[i] = rndBoard.Pentago_gb;
        }

        //using AUTO depth  - - - - - - - - - - - 
        //minimum is 4, gets bigger when there are more pieces on the board

        if (test_with_control_auto)
        {
            printLatexTableHeader();
            //control heuristic
            test00();
            printseparator();
            System.Threading.Thread.Sleep(1000);
            //heuristic1 vs control
            test01_0();
            test01_1();
            test02_0();
            test02_1();
            test03_0();
            test03_1();

            System.Threading.Thread.Sleep(1000);
            printseparator();
            //heuristic1.2 relaxed vs control
            test1dot2vsC_0(0);
            test1dot2vsC_0(1);
            test1dot2vsC_0(2);
            test1dot2vsC_0(3);
            test1dot2vsC_0(4);
            test1dot2vsC_0(5);
            test1dot2vsC_1(0);
            test1dot2vsC_1(1);
            test1dot2vsC_1(2);
            test1dot2vsC_1(3);
            test1dot2vsC_1(4);
            test1dot2vsC_1(5);

            printLatexTableFooter();
            Console.WriteLine();
            Console.WriteLine("%===========================================");
            Console.WriteLine();
            printLatexTableHeader();

            System.Threading.Thread.Sleep(1000);
            printseparator();
            //heuristicA vs control
            testAvsC_0(EVFC.A, 1);
            testAvsC_0(EVFC.A, 2);
            testAvsC_0(EVFC.A, 3);
            testAvsC_0(EVFC.A, 4);
            testAvsC_1(EVFC.A, 1);
            testAvsC_1(EVFC.A, 2);
            testAvsC_1(EVFC.A, 3);
            testAvsC_1(EVFC.A, 4);

            System.Threading.Thread.Sleep(1000);
            printseparator();
            //heuristicAstar vs control
            testAvsC_0(EVFC.Astar, 1);
            testAvsC_0(EVFC.Astar, 2);
            testAvsC_0(EVFC.Astar, 3);
            testAvsC_0(EVFC.Astar, 4);
            testAvsC_1(EVFC.Astar, 1);
            testAvsC_1(EVFC.Astar, 2);
            testAvsC_1(EVFC.Astar, 3);
            testAvsC_1(EVFC.Astar, 4);

            System.Threading.Thread.Sleep(1000);
            printseparator();
            //heuristicAhacked relaxed vs control
            testAvsC_0(EVFC.AplusDiagHack, 1);
            testAvsC_0(EVFC.AplusDiagHack, 2);
            testAvsC_0(EVFC.AplusDiagHack, 3);
            testAvsC_0(EVFC.AplusDiagHack, 4);
            testAvsC_1(EVFC.AplusDiagHack, 1);
            testAvsC_1(EVFC.AplusDiagHack, 2);
            testAvsC_1(EVFC.AplusDiagHack, 3);
            testAvsC_1(EVFC.AplusDiagHack, 4);

            printLatexTableFooter();
        }


        if (test_with_others_auto_suite1)
        {
            printLatexTableHeader();

            test_versus();

            printLatexTableFooter();
        }

        if (test_with_others_auto_suite2)
        {
            printLatexTableHeader();

            test_h12_notrelaxed1();
            test_h12_notrelaxed2();
            test_AstarNoRot1();
            test_AstarNoRot2();

            printLatexTableFooter();
        }

        //using depth 6 - - - - - - - - - - - 
        //NOT SO SURE ABOUT USING 6

        if (test_with_control_d6)
        {
            //heuristic1 vs control

            System.Threading.Thread.Sleep(1000);
            //heuristic1.2 relaxed vs control

            System.Threading.Thread.Sleep(1000);
            //heuristicA relaxed vs control

            System.Threading.Thread.Sleep(1000);
            //heuristicAhacked relaxed vs control

        }

        if (test_with_others_d6)
        {

            System.Threading.Thread.Sleep(1000);
            //1.2 relaxed vs A

            System.Threading.Thread.Sleep(1000);
            //1.2 vs 1.2

            System.Threading.Thread.Sleep(1000);
            //A vs A

            System.Threading.Thread.Sleep(1000);
            //A vs Ahacked
        }


    }

    static void test_aux(MINMAX M_W, MINMAX M_B, bool testFirst)
    {
        /* System.Console.WriteLine();
         System.Console.WriteLine("=======================================================");
         System.Console.WriteLine();
         M_W.printConfigs();
         System.Console.WriteLine("       --- --- ---VS --- --- ---");
         M_B.printConfigs();
         System.Console.WriteLine(" ---  --- --- --- --- --- --- ---");
         System.Console.WriteLine("from start");
         int[] test_start = UnitTesting.testHeuristic(number_of_games_div4_pertest * 2, M_W, M_B, testFirst, true, false, false);
         System.Console.WriteLine("from mid game");
         int[] test_mid = UnitTesting.testHeuristic(testBoards, M_W, M_B, testFirst, true, false, false);

         Console.WriteLine("TOTAL "
             + "W = " + (test_start[0] + test_mid[0])
             + "   L = " + (test_start[1] + test_mid[1])
             + "   D = " + (test_start[2] + test_mid[2])
             )
             ;*/
        System.Console.WriteLine();

        int[] test_start = UnitTesting.testHeuristic(number_of_games_div4_pertest * 2, M_W, M_B, testFirst, true, false, false);
        int[] test_mid = UnitTesting.testHeuristic(testBoards, M_W, M_B, testFirst, true, false, false);

        String names;
        if (testFirst) names = "\\cellcolor{blue!15}\\textbf{" + ((Pentago_Rules)M_W.rules).getHeurName() + "} & " + ((Pentago_Rules)M_B.rules).getHeurName();
        else names = names = ((Pentago_Rules)M_W.rules).getHeurName() + " & \\cellcolor{blue!15}\\textbf{" + ((Pentago_Rules)M_B.rules).getHeurName() + "}";
        Console.WriteLine(names + "& {\\color[HTML]{00009B} } & {\\color[HTML]{9A0000} } & {\\color[HTML]{009901} } &  & {\\color[HTML]{00009B} } & {\\color[HTML]{9A0000} } & {\\color[HTML]{009901} } &  & {\\color[HTML]{00009B} } & {\\color[HTML]{9A0000} } & {\\color[HTML]{009901} } &  \\\\ ");//\\cline{1-2}");
        Console.WriteLine((testFirst ? "\\cellcolor{ blue!15}" : "") + ((Pentago_Rules)M_W.rules).getHeurConfigs() + " & " + (!testFirst ? "\\cellcolor{ blue!15}" : "") + ((Pentago_Rules)M_B.rules).getHeurConfigs()
            + " & \\multirow{-2}{*}{{\\color[HTML]{00009B} " + test_start[0] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{9A0000} " + test_start[1] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{009901} " + test_start[2] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{" + test_start[3] + "} & \\multirow{-2}{*}{{\\color[HTML]{00009B} " + test_mid[0] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{9A0000} " + test_mid[1] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{009901} " + test_mid[2] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{" + test_mid[3] + "} & \\multirow{-2}{*}{{\\color[HTML]{00009B} " + (test_start[0] + test_mid[0]) * 1000 / (number_of_games_div4_pertest * 4) + "}} & \\multirow{-2}{*}{{\\color[HTML]{9A0000} " + (test_start[1] + test_mid[1]) * 1000 / (number_of_games_div4_pertest * 4) + "}} & \\multirow{-2}{*}{{\\color[HTML]{009901} " + (test_start[2] + test_mid[2]) * 1000 / (number_of_games_div4_pertest * 4) + "}} & \\multirow{-2}{*}{" + (test_start[3] + test_mid[3]) / 2 + "} \\\\ \\hline");
        //Console.WriteLine(12 + " & " + 12 + " & \\multirow{-2}{*}{{\\color[HTML]{00009B} " + test_start[0] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{9A0000} " + test_start[1] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{009901} " + test_start[2] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{" + test_start[3] + "} & \\multirow{-2}{*}{{\\color[HTML]{00009B} " + test_mid[0] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{9A0000} " + test_mid[1] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{{\\color[HTML]{009901} " + test_mid[2] * 1000 / (number_of_games_div4_pertest * 2) + "}} & \\multirow{-2}{*}{" + test_mid[3] + "} & \\multirow{-2}{*}{{\\color[HTML]{00009B} " + (test_start[0] + test_mid[0]) * 1000 / (number_of_games_div4_pertest * 4) + "}} & \\multirow{-2}{*}{{\\color[HTML]{9A0000} " + (test_start[1] + test_mid[1]) * 1000 / (number_of_games_div4_pertest * 4) + "}} & \\multirow{-2}{*}{{\\color[HTML]{009901} " + (test_start[2] + test_mid[2]) * 1000 / (number_of_games_div4_pertest * 4) + "}} & \\multirow{-2}{*}{" + (test_start[3] + test_mid[3]) / 2 + "} \\\\ \\hline");
    }

    static void test00()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.check_symmetries,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.check_symmetries,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, true);
    }

    #region with auto depth



    #region tests heuristic 1 vs control

    static void test01_0()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.one,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, true);
    }
    static void test01_1()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.one,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, false);
    }

    static void test02_0()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.one,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        wrules.setHeuristic1Bias(0.0f);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, true);
    }
    static void test02_1()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.one,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        brules.setHeuristic1Bias(0.0f);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, false);
    }

    static void test03_0()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.one,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        wrules.setHeuristic1Bias(1.0f);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, true);
    }
    static void test03_1()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.one,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        brules.setHeuristic1Bias(1.0f);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, false);
    }

    #endregion

    #region tests heuristic 1.2 vs control

    static void test1dot2vsC_0(int config)
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.oneDotTwo,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        if (config == 1) wrules.setHeur12_AD_PO();
        if (config == 2) wrules.setHeur12_PD_AO();
        if (config == 3) wrules.setHeur12_UD();
        if (config == 4) wrules.setHeur12_UO();
        if (config == 5) wrules.setHeur12_P();
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, true);
    }

    static void test1dot2vsC_1(int config)
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.oneDotTwo,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        if (config == 1) brules.setHeur12_AD_PO();
        if (config == 2) brules.setHeur12_PD_AO();
        if (config == 3) brules.setHeur12_UD();
        if (config == 4) brules.setHeur12_UO();
        if (config == 5) brules.setHeur12_P();
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, false);
    }

    #endregion

    #region heuristic A vs control

    static void testAvsC_0(EVFC heur, int config)
    {
        Pentago_Rules wrules = new Pentago_Rules(heur,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);

        if (config == 1) wrules.setA_setup1();
        if (config == 2) wrules.setA_setup2();
        if (config == 3) wrules.setA_setup3();
        if (config == 4) wrules.setA_setup4();

        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, true);
    }

    static void testAvsC_1(EVFC heur, int config)
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.control,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(heur,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_BLACKS, false);

        if (config == 1) brules.setA_setup1();
        if (config == 2) brules.setA_setup2();
        if (config == 3) brules.setA_setup3();
        if (config == 4) brules.setA_setup4();

        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        test_aux(minmax_w, minmax_b, false);
    }

    #endregion

    #region heuristic A star vs Control

    #endregion

    #region A hacked vs control

    #endregion

    #endregion with auto depth


    static void test_versus()
    {
        foreach (EVFC evfc1 in Enum.GetValues(typeof(EVFC)))
            foreach (EVFC evfc2 in Enum.GetValues(typeof(EVFC)))
            {
                if (evfc1 == EVFC.control
                || evfc2 == EVFC.control
                    ) continue;

                Pentago_Rules wrules = new Pentago_Rules(evfc1,
                Pentago_Rules.NextStatesFunction.all_states,
                Pentago_Rules.IA_PIECES_WHITES, false);
                MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
                Pentago_Rules brules = new Pentago_Rules(evfc2,
                        Pentago_Rules.NextStatesFunction.all_states,
                        Pentago_Rules.IA_PIECES_BLACKS, false);
                MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

                test_aux(minmax_w, minmax_b, true);
            }
    }

    static void test_h12_notrelaxed1()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.oneDotTwo,
        Pentago_Rules.NextStatesFunction.all_states,
        Pentago_Rules.IA_PIECES_WHITES, false);
        wrules.HEUR12RELAXED = false;
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);

        foreach (EVFC evfc2 in Enum.GetValues(typeof(EVFC)))
        {
            Pentago_Rules brules = new Pentago_Rules(evfc2,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_BLACKS, false);
            MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

            test_aux(minmax_w, minmax_b, true);
        }
    }

    static void test_AstarNoRot1()
    {
        Pentago_Rules wrules = new Pentago_Rules(EVFC.Astar,
        Pentago_Rules.NextStatesFunction.all_states,
        Pentago_Rules.IA_PIECES_WHITES, false);
        ;
        wrules.setAstarSettings(1.13f, 1.15f, 1.17f, 1.19f, false);
        MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);

        foreach (EVFC evfc2 in Enum.GetValues(typeof(EVFC)))
        {
            Pentago_Rules brules = new Pentago_Rules(evfc2,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_BLACKS, false);
            MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

            test_aux(minmax_w, minmax_b, true);
        }
    }

    static void test_h12_notrelaxed2()
    {
        Pentago_Rules brules = new Pentago_Rules(EVFC.oneDotTwo,
        Pentago_Rules.NextStatesFunction.all_states,
        Pentago_Rules.IA_PIECES_BLACKS, false);
        brules.HEUR12RELAXED = false;
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        foreach (EVFC evfc2 in Enum.GetValues(typeof(EVFC)))
        {
            Pentago_Rules wrules = new Pentago_Rules(evfc2,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_WHITES, false);

            MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);

            test_aux(minmax_w, minmax_b, false);
        }
    }

    static void test_AstarNoRot2()
    {
        Pentago_Rules brules = new Pentago_Rules(EVFC.Astar ,
        Pentago_Rules.NextStatesFunction.all_states,
        Pentago_Rules.IA_PIECES_BLACKS, false);
        brules.setAstarSettings(1.13f, 1.15f, 1.17f, 1.19f, false);
        MINMAX minmax_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 0);

        foreach (EVFC evfc2 in Enum.GetValues(typeof(EVFC)))
        {
            Pentago_Rules wrules = new Pentago_Rules(evfc2,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_WHITES, false);
            ;
            MINMAX minmax_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);

            test_aux(minmax_w, minmax_b, false);
        }
    }


}

