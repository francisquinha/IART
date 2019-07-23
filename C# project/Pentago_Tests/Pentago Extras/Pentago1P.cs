using System;
using System.Linq;
using System.Diagnostics;

using MINMAX = MinMax<Pentago_GameBoard, Pentago_Move>;

static class Pentago1P
{
    public static void play(int depth, Pentago_Rules.EvaluationFunction ef, Pentago_Rules.NextStatesFunction nsf, bool remove_duplicates, bool ia_pieces)
    {
        Pentago_GameBoard gb = new Pentago_GameBoard();
        bool? winning_player = null;
        Pentago_Rules rules = new Pentago_Rules(ef, nsf, ia_pieces, remove_duplicates);
        MINMAX alpha_beta_test = new MINMAX(MINMAX.VERSION.alphabeta, rules, depth);
        Stopwatch sw = new Stopwatch();

        while (!gb.game_ended(out winning_player))
        {
            gb.print_board();

            if (gb.get_player_turn() != ia_pieces)
            {
                Console.WriteLine("Your turn");
                if (gb.get_turn_state() == Pentago_GameBoard.turn_state_addpiece)
                {
                    Console.WriteLine("Place a piece: square,x,y     square E[0,3]      x,y E[0,2]");
                    int[] input;
                    try
                    {
                        input = Console.ReadLine().Split(',').Select<string, int>(o => Convert.ToInt32(o)).ToArray();
                        if (input.Length < 3) continue;
                    }
                    catch { continue; }
                    Pentago_Move pm = new Pentago_Move(input[0], input[1], input[2]);
                    if (!pm.is_move_possible(gb)) continue;
                    pm.apply_move2board(gb);
                }
                else
                {
                    Console.WriteLine("Rotate a square: square,dir     square E[0,3]      dir E[0-anti,1-clock]");
                    int[] input;
                    try
                    {
                        input = Console.ReadLine().Split(',').Select<string, int>(o => Convert.ToInt32(o)).ToArray();
                        if (input.Length < 2) continue;
                    }
                    catch { continue; }
                    Pentago_Move pm = new Pentago_Move(input[0], input[1] == 0 ? Pentago_Move.rotate_anticlockwise : Pentago_Move.rotate_clockwise);
                    if (!pm.is_move_possible(gb)) continue;
                    pm.apply_move2board(gb);
                }
            }
            else
            {
                Console.WriteLine("IA's turn");
                sw.Start();
                applyMoves(alpha_beta_test.run(gb), gb);
                sw.Stop();
                Console.WriteLine("Time={0}", sw.Elapsed);
            }

        }
        if (winning_player == null) Console.WriteLine("\nGAME ENDED IN DRAW");
        else if (winning_player != ia_pieces) Console.WriteLine("\nGAME ENDED - YOU WON");
        else Console.WriteLine("\nGAME ENDED - IA WON");
    }

    static void applyMoves(Pentago_Move[] moves, Pentago_GameBoard board)
    {
        foreach (Pentago_Move move in moves)
            move.apply_move2board(board);
    }

}

