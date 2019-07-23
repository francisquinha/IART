using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentago_Tests
{
    static class Pentago2P
    {
        public static void play()
        {
            Pentago_GameBoard gb = new Pentago_GameBoard();
            bool? winning_player = null;

            while(!gb.game_ended(out winning_player))
            {
                Console.Clear();
                gb.print_board();

                if(gb.get_player_turn() == Pentago_GameBoard.whites_turn)
                    Console.WriteLine("White's turn");
                else Console.WriteLine("Black's turn");

                if (gb.get_turn_state() == Pentago_GameBoard.turn_state_addpiece)
                {
                    Console.WriteLine("Place a piece: square,x,y     square E[0,3]      x,y E[0,2]");
                    int[] input;
                    try {
                        input = Console.ReadLine().Split(',').Select<string, int>(o => Convert.ToInt32(o)).ToArray();
                        if (input.Length < 3) continue;
                    } catch { continue; }
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
                    Pentago_Move pm = new Pentago_Move(input[0], input[1]==0? Pentago_Move.rotate_anticlockwise:Pentago_Move.rotate_clockwise  );
                    if (!pm.is_move_possible(gb)) continue;
                    pm.apply_move2board(gb);
                }

            }

            Console.Clear();
            gb.print_board();
            if (winning_player == null) Console.WriteLine("\nGAME ENDED IN DRAW");
            else if (winning_player == Pentago_GameBoard.whites_turn) Console.WriteLine("\nGAME ENDED - WHITE WON");
            else Console.WriteLine("\nGAME ENDED - BLACK WON");
        }

    }
}
