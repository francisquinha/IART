using System;
using HOLESTATE = Pentago_GameBoard.hole_state;

static partial class UnitTesting
{
    #region CUSTOM GAME BOARDS 2 BE USED IN TESTS

    static Pentago_GameBoard board1, board1r, boardAlphaBeta, boardMinMax, boardHeuristicA, emptyBoard;

    static void initialize_test_gameboards()
    {
        const HOLESTATE B = HOLESTATE.has_black;
        const HOLESTATE W = HOLESTATE.has_white;
        const HOLESTATE E = HOLESTATE.is_empty;

        board1 = new Pentago_GameBoard(
            new HOLESTATE[]{
                B,B,B, E,E,E,
                W,W,W, B,B,B,
                B,B,B, W,W,W,

                W,W,W, B,B,B,
                B,B,B, W,W,W,
                W,W,W, E,E,E }
            , Pentago_GameBoard.whites_turn, Pentago_GameBoard.turn_state_addpiece);

        board1r = new Pentago_GameBoard(
          new HOLESTATE[]{
                B,B,B, E,E,E,
                W,W,W, B,B,B,
                B,B,B, W,W,W,

                W,W,W, B,B,B,
                B,B,B, W,W,W,
                W,W,W, E,E,E }
          , Pentago_GameBoard.whites_turn, Pentago_GameBoard.turn_state_rotate);

        boardAlphaBeta = new Pentago_GameBoard(
          new HOLESTATE[]{
                B,E,E, E,E,E,
                B,E,E, E,W,E,
                E,E,E, E,E,E,

                B,E,W, E,E,E,
                E,W,E, E,E,E,
                E,E,E, E,E,E }
          , Pentago_GameBoard.whites_turn, Pentago_GameBoard.turn_state_addpiece);

        boardMinMax = new Pentago_GameBoard(
          new HOLESTATE[]{
                B,E,E, E,E,E,
                B,E,E, E,W,E,
                E,E,E, E,E,E,

                B,E,W, E,E,E,
                E,W,E, E,E,E,
                E,E,E, E,E,E }
          , Pentago_GameBoard.whites_turn, Pentago_GameBoard.turn_state_addpiece);

        boardHeuristicA = new Pentago_GameBoard(
          new HOLESTATE[]{
                E,B,E,E,W,E,
                E,E,E,B,W,B,
                E,E,E,B,B,W,
                E,E,B,W,E,B,
                E,W,W,W,W,E,
                E,B,E,W,E,E }
          , Pentago_GameBoard.blacks_turn, Pentago_GameBoard.turn_state_addpiece);

        emptyBoard = new Pentago_GameBoard(
          new HOLESTATE[]{
                E,E,E,E,E,E,
                E,E,E,E,E,E,
                E,E,E,E,E,E,
                E,E,E,E,E,E,
                E,E,E,E,E,E,
                E,E,E,E,E,E }
          , Pentago_GameBoard.whites_turn, Pentago_GameBoard.turn_state_addpiece);

    }

    #endregion


    public static void test_auxiliar_methods()
    {
        Console.WriteLine("test_auxiliar_methods()");
        initialize_test_gameboards();

        Pentago_Rules pr_110 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.control, Pentago_Rules.NextStatesFunction.all_states, false);

        Pentago_Rules pr_111 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.control, Pentago_Rules.NextStatesFunction.all_states, true);

        Console.WriteLine("test1");
        foreach (Pentago_Move pm in pr_110.possible_plays(board1))
            Console.WriteLine(pm.ToString());

        Console.WriteLine("test2");
        Console.WriteLine(pr_110.next_states(board1r).Length);//expected 8
        Console.WriteLine(pr_111.next_states(board1r).Length);//expected 1+2+1+2 = 6

        /*Console.WriteLine("test2 a)");
          foreach (Pentago_GameBoard gb in pr_110.next_states(board1r))
              gb.print_board();
          Console.WriteLine("test2 b)");
          foreach (Pentago_GameBoard gb in pr_111.next_states(board1r))
              gb.print_board();*/

        //RAW test... time is not measured... just 2 have a practical perception
       /* int a, b;
        Console.WriteLine("test3-time 1e5");
        for (int i = 0; i < 100000; i++)
            Pentago_Rules.calculate_available_classes(board1, out a, out b);
        Console.WriteLine("test3-ended 1e5");*/

        /* Console.WriteLine("test3-time 1e6");
         for (int i = 0; i < 1000000; i++)
             Pentago_Rules.calculate_available_classes(board1, out a, out b);
         Console.WriteLine("test3-ended 1e6");

         Console.WriteLine("test3-time 1e7");
         for (int i = 0; i < 10000000; i++)
             Pentago_Rules.calculate_available_classes(board1, out a, out b);
         Console.WriteLine("test3-ended 1e7");

         Console.WriteLine("test3-time 1e8");
         for (int i = 0; i < 100000000; i++)
             Pentago_Rules.calculate_available_classes(board1, out a, out b);
         Console.WriteLine("test3-ended 1e8");*/

        Pentago_Rules rulestest = new Pentago_Rules();
        for (int i = 0; i < 100; i++)
            Console.WriteLine(rulestest.ControlHeuristic());

    }

    



}

