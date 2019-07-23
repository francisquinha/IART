//#define PLAY_AGAINST_HUMAN

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MINMAX = MinMax<Pentago_GameBoard, Pentago_Move>;
static partial class UnitTesting
{
    static public void testMinMax()
    {
        initialize_test_gameboards();
        Pentago_Rules wrules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.control,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX test_w = new MINMAX(MINMAX.VERSION.minmax, wrules, 6);
        Pentago_Rules brules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.control,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_b = new MINMAX(MINMAX.VERSION.minmax, brules, 6);
        bool? player;
        boardMinMax.print_board();
        while (!boardMinMax.game_ended(out player))
        {
#if DEBUG_MIN_MAX
        alpha_beta_test.debugBoard = (o) => { o.print_board(); };
#endif
            applyPrintMoves2(test_w.run(boardMinMax));
#if PLAY_AGAINST_MIN_MAX
        Console.WriteLine("Place a piece: square,x,y     square E[0,3]      x,y E[0,2]");
        int[] input = Console.ReadLine().Split(',').Select<string, int>(o => Convert.ToInt32(o)).ToArray();
        Pentago_Move pm = new Pentago_Move(input[0], input[1], input[2]);
        pm.apply_move2board(boardAlphaBeta);
        Console.WriteLine("Rotate a square: square,dir     square E[0,3]      dir E[0-anti,1-clock]");
        input = Console.ReadLine().Split(',').Select<string, int>(o => Convert.ToInt32(o)).ToArray();
        pm = new Pentago_Move(input[0], input[1] == 0 ? Pentago_Move.rotate_anticlockwise : Pentago_Move.rotate_clockwise);
        pm.apply_move2board(boardAlphaBeta);
        boardAlphaBeta.print_board();
#else
            applyPrintMoves2(test_b.run(boardMinMax));
#endif
        }
    }

    static void applyPrintMoves2(Pentago_Move[] moves)
    {
        foreach (Pentago_Move move in moves)
        {
            move.apply_move2board(boardMinMax);
            boardMinMax.print_board();
        }
    }

}
