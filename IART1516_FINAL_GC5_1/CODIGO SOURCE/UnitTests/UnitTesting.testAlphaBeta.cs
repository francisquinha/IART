//#define PLAY_AGAINST_HUMAN

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MINMAX = MinMax<Pentago_GameBoard, Pentago_Move>;
static partial class UnitTesting
{
    static public void testAlphaBeta()
    {
        initialize_test_gameboards();
        Pentago_Rules wrules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.oneDotTwo,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_WHITES, false);
        MINMAX alpha_beta_test_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 0);
        Pentago_Rules brules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.A,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX alpha_beta_test_b = new MINMAX(MINMAX.VERSION.alphabeta, brules,0);
        bool? player;
/*        boardAlphaBeta.print_board();
        Console.WriteLine("|-|-|-|-|-|-|-|-|");*/
        while (!emptyBoard.game_ended(out player))
        {
#if DEBUG_ALPHA_BETA
        alpha_beta_test.debugBoard = (o) => { o.print_board(); };
#endif
            applyPrintMoves(alpha_beta_test_w.run(emptyBoard), emptyBoard);
            Console.WriteLine("|-|-|-|-|-|-|-|-|");
#if PLAY_AGAINST_HUMAN
        Console.WriteLine("Place a piece: square,x,y     square E[0,3]      x,y E[0,2]");
        int[] input = Console.ReadLine().Split(',').Select<string, int>(o => Convert.ToInt32(o)).ToArray();
        Pentago_Move pm = new Pentago_Move(input[0], input[1], input[2]);
        pm.apply_move2board(boardAlphaBeta);
        Console.WriteLine("Rotate a square: square,dir     square E[0,3]      dir E[0-anti,1-clock]");
        input = Console.ReadLine().Split
            (',').Select<string, int>(o => Convert.ToInt32(o)).ToArray();
        pm = new Pentago_Move(input[0], input[1] == 0 ? Pentago_Move.rotate_anticlockwise : Pentago_Move.rotate_clockwise);
        pm.apply_move2board(boardAlphaBeta);
        boardAlphaBeta.print_board();
#else
            applyPrintMoves(alpha_beta_test_b.run(emptyBoard), emptyBoard);
            Console.WriteLine("|-|-|-|-|-|-|-|-|");
#endif
        }
        if (player == null) Console.WriteLine("Tie");
        else if (player == Pentago_Rules.IA_PIECES_BLACKS) Console.WriteLine("Black wins");
        else Console.WriteLine("White wins");
    }

    static void applyPrintMoves(Pentago_Move[] moves, Pentago_GameBoard board)
    {
        foreach (Pentago_Move move in moves)
        {
            if (board.get_player_turn() == Pentago_GameBoard.whites_turn) Console.Write("White ");
            else Console.Write("Black ");
            if (board.get_turn_state() == Pentago_GameBoard.turn_state_rotate)
                Console.WriteLine("rotate");
            else Console.WriteLine("place");
            move.apply_move2board(board);
            board.print_board();
        }
    }
}
