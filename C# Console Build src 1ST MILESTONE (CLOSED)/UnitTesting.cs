using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentago_Tests
{
    static class UnitTesting
    {
        #region GAME BOARDS 2 BE USED IN TESTS

        static Pentago_GameBoard board1, board1r, board2, board3, board4, board5, board6 ;

        static void initialize_test_gameboards()
        {
            const Pentago_GameBoard.hole_state B = Pentago_GameBoard.hole_state.has_black;
            const Pentago_GameBoard.hole_state W = Pentago_GameBoard.hole_state.has_white;
            const Pentago_GameBoard.hole_state E = Pentago_GameBoard.hole_state.is_empty;

            board1 = new Pentago_GameBoard(
                new Pentago_GameBoard.hole_state[]{
                B,B,B, E,E,E,
                W,W,W, B,B,B,
                B,B,B, W,W,W,

                W,W,W, B,B,B,
                B,B,B, W,W,W,
                W,W,W, E,E,E }
                ,Pentago_GameBoard.whites_turn,Pentago_GameBoard.turn_state_addpiece);

            board1r = new Pentago_GameBoard(
              new Pentago_GameBoard.hole_state[]{
                B,B,B, E,E,E,
                W,W,W, B,B,B,
                B,B,B, W,W,W,

                W,W,W, B,B,B,
                B,B,B, W,W,W,
                W,W,W, E,E,E }
              , Pentago_GameBoard.whites_turn, Pentago_GameBoard.turn_state_rotate);


        }

         #endregion


    public static void test_auxiliar_methods()
        {
            Console.WriteLine("test_auxiliar_methods()");
            initialize_test_gameboards();

            Pentago_Rules pr_110 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.func1, Pentago_Rules.NextStatesFunction.all_states,false);

            Pentago_Rules pr_111 = new Pentago_Rules(Pentago_Rules.EvaluationFunction.func1, Pentago_Rules.NextStatesFunction.all_states,true);

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



        }
        


    }
}
