using System;
using System.Linq;


    static class Pentago1PH
    {
        public static void play()
        {
            Pentago_GameBoard gb = new Pentago_GameBoard();
            bool? winning_player = null;
            Pentago_Rules rules = new Pentago_Rules();
        rules.setHeuristic1Bias(0);

            while (!gb.game_ended(out winning_player))
            {
                Console.Clear();
                gb.print_board();

            if (gb.get_player_turn() == Pentago_GameBoard.whites_turn)
            {
                Pentago_GameBoard newbg = gb;
                float bestSoFar = float.NegativeInfinity;
                Pentago_Move[] moves = rules.possible_plays(gb);
                bool ended = false;
                //int count = 0;
                foreach(Pentago_Move move in moves)
                {
                   // Console.WriteLine(move.ToString());
                    Pentago_GameBoard gb_aux = move.state_after_move(gb);
                   // gb_aux.print_board();
                   // count++;
                  //  if (count > 5) break;
                    bool? winner;
                    if (gb_aux.game_ended(out winner))
                        if (winner != null && winner == Pentago_GameBoard.whites_turn)
                        { move.apply_move2board(gb); ended = true; }
                    if (ended) break;

                    foreach (Pentago_GameBoard gbs in rules.next_states(gb_aux))
                    {
                        if (gbs.game_ended(out winner))
                            if (winner != null && winner == Pentago_GameBoard.whites_turn)
                            { gb = gbs;  ended = true; }
                        if (ended) break;

                        float value = rules.heuristic1dot2(gbs.board);

                        if(value>bestSoFar)
                        {
                            bestSoFar = value;
                            newbg = gbs;
                        }

                    }
                    if (ended) break;

                }
                if (ended) break;

                gb = newbg;

                continue;
            }
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

