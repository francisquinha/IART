using System;
using MINMAX = MinMax<Pentago_GameBoard, Pentago_Move>;


static class PerformanceTests
{
    static public void testPerformnace(int numOfBorads)
    {
        Pentago_GameBoard[] testBoardsWhites = new Pentago_GameBoard[numOfBorads];
        Pentago_GameBoard[] testBoardsBlacks = new Pentago_GameBoard[numOfBorads];

        for (int i = 0; i < numOfBorads; i++)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(0, 17);
            Console.WriteLine("Num of pieces " + numPieces);
            GenerateRandomBoard rndBoard;

            Console.WriteLine("white");

            rndBoard = new GenerateRandomBoard(numPieces -1, true); // -1 prencipio de iguldade de peças do tabuleiro
            rndBoard.generateNewBoard();
            testBoardsWhites[i] = rndBoard.Pentago_gb;
            testBoardsWhites[i].print_board();

            Console.WriteLine("black");

            rndBoard = new GenerateRandomBoard(numPieces, false);
            rndBoard.generateNewBoard();
            testBoardsBlacks[i] = rndBoard.Pentago_gb;
            testBoardsBlacks[i].print_board();
        }

        Pentago_Rules wrules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.controlHeuristic,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX test_w = new MINMAX(MINMAX.VERSION.minmax, wrules, 6);
        Pentago_Rules brules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.controlHeuristic,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_b = new MINMAX(MINMAX.VERSION.minmax, brules, 6);

        TimeSpan test1 = Performance.PerformanceTimes(test_w, testBoardsWhites);
        TimeSpan test2 = Performance.PerformanceTimes(test_b, testBoardsBlacks);

        TimeSpan ts = test1.Add(test2);

        string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            test1.Hours, test1.Minutes, test1.Seconds,
            test1.Milliseconds / 10);

        string elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            test2.Hours, test2.Minutes, test2.Seconds,
            test2.Milliseconds / 10);

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

        Console.WriteLine("RunTime " + elapsedTime);
        Console.WriteLine("RunTime 1 " + elapsedTime1);
        Console.WriteLine("RunTime 2 " + elapsedTime2);
    }

    // testa qual a euristica mais rápida entre heuristics1 e heuristicsA
    static public void testPerformnace1(int numOfBorads)
    {
        Pentago_GameBoard[] testBoardsWhites = new Pentago_GameBoard[numOfBorads];
        Pentago_GameBoard[] testBoardsBlacks = new Pentago_GameBoard[numOfBorads];

        for (int i = 0; i < numOfBorads; i++)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(0, 17);
            Console.WriteLine("Num of pieces " + numPieces);
            GenerateRandomBoard rndBoard;

            Console.WriteLine("white");

            rndBoard = new GenerateRandomBoard(numPieces == 0? 0: numPieces-1, true); // -1 prencipio de iguldade de peças do tabuleiro
            rndBoard.generateNewBoard();
            testBoardsWhites[i] = rndBoard.Pentago_gb;
            testBoardsWhites[i].print_board();

            Console.WriteLine("black");

            rndBoard = new GenerateRandomBoard(numPieces, false);
            rndBoard.generateNewBoard();
            testBoardsBlacks[i] = rndBoard.Pentago_gb;
            testBoardsBlacks[i].print_board();
        }

        // heuristics1
        Pentago_Rules wrules1 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX test_w1 = new MINMAX(MINMAX.VERSION.minmax, wrules1, 4);
        Pentago_Rules brules1 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_b1 = new MINMAX(MINMAX.VERSION.minmax, brules1, 4);

        // heuristicsA
        Pentago_Rules wrulesA = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristicA,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX test_wA = new MINMAX(MINMAX.VERSION.minmax, wrulesA, 4);
        Pentago_Rules brulesA = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristicA,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_bA = new MINMAX(MINMAX.VERSION.minmax, brulesA, 4);



        TimeSpan test1_1 = Performance.PerformanceTimes(test_w1, testBoardsWhites);
        TimeSpan test2_1 = Performance.PerformanceTimes(test_b1, testBoardsBlacks);

        TimeSpan ts_1 = test1_1.Add(test2_1);

        // Format and display the TimeSpan value.
        string elapsedTime_1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts_1.Hours, ts_1.Minutes, ts_1.Seconds,
            ts_1.Milliseconds / 10);

        Console.WriteLine("RunTime heuristic1 " + elapsedTime_1);
        //////////////////////////////////////////////////////////////////////////////
        TimeSpan test1_A = Performance.PerformanceTimes(test_wA, testBoardsWhites);
        TimeSpan test2_A = Performance.PerformanceTimes(test_bA, testBoardsBlacks);

        TimeSpan ts_A = test1_A.Add(test2_A);

        // Format and display the TimeSpan value.
        string elapsedTime_A = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts_A.Hours, ts_A.Minutes, ts_A.Seconds,
            ts_A.Milliseconds / 10);

        Console.WriteLine("RunTime heuristicA " + elapsedTime_A);

    }


    // testa heuristica1 com minmax e diferentes draw values
    static public void testPerformnace2(int numOfBorads)
    {
        Pentago_GameBoard[] testBoardsWhites = new Pentago_GameBoard[numOfBorads];
        Pentago_GameBoard[] testBoardsBlacks = new Pentago_GameBoard[numOfBorads];

        for (int i = 0; i < numOfBorads; i++)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(0, 17);
            Console.WriteLine("Num of pieces " + numPieces);
            GenerateRandomBoard rndBoard;

            Console.WriteLine("white");

            rndBoard = new GenerateRandomBoard(numPieces == 0 ? 0 : numPieces - 1, true); // -1 prencipio de iguldade de peças do tabuleiro
            rndBoard.generateNewBoard();
            testBoardsWhites[i] = rndBoard.Pentago_gb;
            testBoardsWhites[i].print_board();

            Console.WriteLine("black");

            rndBoard = new GenerateRandomBoard(numPieces, false);
            rndBoard.generateNewBoard();
            testBoardsBlacks[i] = rndBoard.Pentago_gb;
            testBoardsBlacks[i].print_board();
        }

        // true
        Pentago_Rules wrules1 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_WHITES, true);
        MINMAX test_w1 = new MINMAX(MINMAX.VERSION.minmax, wrules1, 5);
        Pentago_Rules brules1 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, true);
        MINMAX test_b1 = new MINMAX(MINMAX.VERSION.minmax, brules1, 5);

        // false
        Pentago_Rules wrules2 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX test_w2 = new MINMAX(MINMAX.VERSION.minmax, wrules2, 5);
        Pentago_Rules brules2 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_b2 = new MINMAX(MINMAX.VERSION.minmax, brules2, 5);



        TimeSpan test1_1 = Performance.PerformanceTimes(test_w1, testBoardsWhites);
        TimeSpan test2_1 = Performance.PerformanceTimes(test_b1, testBoardsBlacks);

        TimeSpan ts_1 = test1_1.Add(test2_1);

        // Format and display the TimeSpan value.
        string elapsedTime_1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts_1.Hours, ts_1.Minutes, ts_1.Seconds,
            ts_1.Milliseconds / 10);

        Console.WriteLine("RunTime true " + elapsedTime_1);
        //////////////////////////////////////////////////////////////////////////////
        TimeSpan test1_2 = Performance.PerformanceTimes(test_w2, testBoardsWhites);
        TimeSpan test2_2 = Performance.PerformanceTimes(test_b2, testBoardsBlacks);

        TimeSpan ts_2 = test1_2.Add(test2_2);

        // Format and display the TimeSpan value.
        string elapsedTime_2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts_2.Hours, ts_2.Minutes, ts_2.Seconds,
            ts_2.Milliseconds / 10);

        Console.WriteLine("RunTime false " + elapsedTime_2);

    }

    // testa heuristica1 com minmax e diferentes states
    static public void testPerformnace3(int numOfBorads)
    {
        Pentago_GameBoard[] testBoardsWhites = new Pentago_GameBoard[numOfBorads];
        Pentago_GameBoard[] testBoardsBlacks = new Pentago_GameBoard[numOfBorads];

        for (int i = 0; i < numOfBorads; i++)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(0, 17);
            Console.WriteLine("Num of pieces " + numPieces);
            GenerateRandomBoard rndBoard;

            Console.WriteLine("white");

            rndBoard = new GenerateRandomBoard(numPieces == 0 ? 0 : numPieces - 1, true); // -1 prencipio de iguldade de peças do tabuleiro
            rndBoard.generateNewBoard();
            testBoardsWhites[i] = rndBoard.Pentago_gb;
            testBoardsWhites[i].print_board();

            Console.WriteLine("black");

            rndBoard = new GenerateRandomBoard(numPieces, false);
            rndBoard.generateNewBoard();
            testBoardsBlacks[i] = rndBoard.Pentago_gb;
            testBoardsBlacks[i].print_board();
        }

        // check_simetrie
        Pentago_Rules wrules1 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
                    Pentago_Rules.NextStatesFunction.check_symmetries,
                    Pentago_Rules.IA_PIECES_WHITES, true);
        MINMAX test_w1 = new MINMAX(MINMAX.VERSION.minmax, wrules1, 5);
        Pentago_Rules brules1 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
            Pentago_Rules.NextStatesFunction.check_symmetries,
            Pentago_Rules.IA_PIECES_BLACKS, true);
        MINMAX test_b1 = new MINMAX(MINMAX.VERSION.minmax, brules1, 5);

        // all_states
        Pentago_Rules wrules2 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
                    Pentago_Rules.NextStatesFunction.all_states,
                    Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX test_w2 = new MINMAX(MINMAX.VERSION.minmax, wrules2, 5);
        Pentago_Rules brules2 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristic1,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_b2 = new MINMAX(MINMAX.VERSION.minmax, brules2, 5);



        TimeSpan test1_1 = Performance.PerformanceTimes(test_w1, testBoardsWhites);
        TimeSpan test2_1 = Performance.PerformanceTimes(test_b1, testBoardsBlacks);

        TimeSpan ts_1 = test1_1.Add(test2_1);

        // Format and display the TimeSpan value.
        string elapsedTime_1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts_1.Hours, ts_1.Minutes, ts_1.Seconds,
            ts_1.Milliseconds / 10);

        Console.WriteLine("RunTime check_sim " + elapsedTime_1);
        //////////////////////////////////////////////////////////////////////////////
        TimeSpan test1_2 = Performance.PerformanceTimes(test_w2, testBoardsWhites);
        TimeSpan test2_2 = Performance.PerformanceTimes(test_b2, testBoardsBlacks);

        TimeSpan ts_2 = test1_2.Add(test2_2);

        // Format and display the TimeSpan value.
        string elapsedTime_2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts_2.Hours, ts_2.Minutes, ts_2.Seconds,
            ts_2.Milliseconds / 10);

        Console.WriteLine("RunTime allstates " + elapsedTime_2);

    }


}