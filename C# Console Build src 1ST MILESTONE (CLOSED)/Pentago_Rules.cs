using System;
using System.Collections.Generic;
using System.Linq;

public class Pentago_Rules : IGameRules<Pentago_GameBoard, Pentago_Move>
{

    public enum EvaluationFunction { func1, blabla2, blabla3 };
    EvaluationFunction ef;

    public bool remove_repeated_states_on_nextStates = false;
    public enum NextStatesFunction { all_states, removeSym_A_B, removeSym_A_B_C, someotherxpto };
    NextStatesFunction nsf;

    public static Pentago_Move[] all_possible_place_piece_moves = null;
    public static Pentago_Move[] all_possible_rotate_squares_moves = null;

    public Pentago_Rules(EvaluationFunction ef, NextStatesFunction nsf, bool remove_repeated_states_on_nextStates = false)
    {
        this.ef = ef;
        this.nsf = nsf;
        this.remove_repeated_states_on_nextStates = remove_repeated_states_on_nextStates;
        //create all possible plays.
        //since we are using a class, there is no need to initialize them again
        //when we are getting possible moves in a board

        if (all_possible_place_piece_moves == null)
        {
            all_possible_place_piece_moves = new Pentago_Move[36];
            int i = 0;
            for (int s2p = 0; s2p < 4; s2p++)
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 3; y++)
                    {
                        all_possible_place_piece_moves[i] = new Pentago_Move(s2p, x, y);
                        i++;
                    }
        }
        if (all_possible_rotate_squares_moves == null)
        {
            all_possible_rotate_squares_moves = new Pentago_Move[8];
            int i = 0;
            for (int s2p = 0; s2p < 4; s2p++)
            {
                all_possible_rotate_squares_moves[i] = new Pentago_Move(s2p, false);
                i++;
                all_possible_rotate_squares_moves[i] = new Pentago_Move(s2p, true);
                i++;
            }
        }
    }

    #region  Auxiliar Methods

    /// <summary>
    /// copies a square from one board to a square in another board
    /// </summary>
    void copy_square2(Pentago_GameBoard.hole_state[] scr, Pentago_GameBoard.hole_state[] dst, int src_square, int dst_square)
    {
        for (int i = 0; i < 3; i++)
            for (int a = 0; a < 3; a++)
                dst[Pentago_GameBoard.board_position_to_index(i, a, dst_square)] =
                       scr[Pentago_GameBoard.board_position_to_index(i, a, src_square)];
    }

    Pentago_GameBoard.hole_state[] get_rotated_board_90deg_anticlockwise(Pentago_GameBoard.hole_state[] board)
    {
        Pentago_GameBoard.hole_state[] newboard = new Pentago_GameBoard.hole_state[36];

        copy_square2(board, newboard, 0, 2);
        copy_square2(board, newboard, 1, 0);
        copy_square2(board, newboard, 2, 3);
        copy_square2(board, newboard, 3, 1);

        Pentago_Move rotsquares = new Pentago_Move(0, Pentago_Move.rotate_anticlockwise);
        rotsquares.move(newboard, true, Pentago_GameBoard.turn_state_rotate);
        rotsquares.square2rotate = 1; rotsquares.move(newboard, true, Pentago_GameBoard.turn_state_rotate);
        rotsquares.square2rotate = 2; rotsquares.move(newboard, true, Pentago_GameBoard.turn_state_rotate);
        rotsquares.square2rotate = 3; rotsquares.move(newboard, true, Pentago_GameBoard.turn_state_rotate);

        return newboard;
    }

    bool square0_has_maindiagonal_symmetry(Pentago_GameBoard.hole_state[] board)
    {
        return board[1] == board[6] && board[12] == board[2] && board[13] == board[8];
    }

    /// <summary>
    /// find board simmetries
    /// </summary>
    /// <param name="board"></param>
    /// <param name="typeA">true if the board remains always the same after been rotated (only need to check 1 square)</param>
    /// <param name="typeB">true if remains the same after rotaing 180 degrees (only need to check 2 adjacent squares)</param>
    /// <param name="typeC">if true, only need to process half square in some instances ( typeA ;  typeB ; 1square between 2 diagonal squares mirrored) </param> 
    void board_has_symmetry(Pentago_GameBoard.hole_state[] board, out bool typeA, out bool typeB, out bool[] typeC)
    {
        typeA = true; typeB = true;
        typeC = new bool[] { false, false, false, false };

        Pentago_GameBoard.hole_state[] aux = (Pentago_GameBoard.hole_state[])board.Clone();
        typeC[0] = square0_has_maindiagonal_symmetry(aux);

        aux = get_rotated_board_90deg_anticlockwise(aux);
        if (aux != board) typeA = false;
        typeC[1] = square0_has_maindiagonal_symmetry(aux);

        aux = get_rotated_board_90deg_anticlockwise(aux);
        if (aux != board)
        {
            typeA = false; typeB = false;
            return;//might be removed later to allways compute typeC[3]
        }
        typeC[2] = square0_has_maindiagonal_symmetry(aux);

        aux = get_rotated_board_90deg_anticlockwise(aux);
        if (aux != board) typeA = false;
        typeC[3] = square0_has_maindiagonal_symmetry(aux);
    }



    /// <summary>
    /// removes duplicates. could also remove using Distinc + override GetHashCode , would be faster but could (very rarely) make a wrong avaliation
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static Pentago_GameBoard[] removeDuplicates(Pentago_GameBoard[] list)
    {
        List<Pentago_GameBoard> newlist = new List<Pentago_GameBoard>();
        for (int i = list.Length - 1; i >= 0; i--)
        {
            bool save = true;
            foreach (Pentago_GameBoard elem in newlist)
            {
                if (elem == list[i])
                {
                    save = false;
                    break;
                }
            }
            if (save) newlist.Add(list[i]);
        }

        return newlist.ToArray();
    }


    #endregion

    //Interface Related --------------------------------------------------------------------

    public Pentago_Move[] possible_plays(Pentago_GameBoard gb)
    {
        Pentago_Move[] result = null;
        if (gb.get_turn_state() == Pentago_GameBoard.turn_state_addpiece)
        {
            return all_possible_place_piece_moves.Where(move => move.is_move_possible(gb)).ToArray();
        }
        else
        {
            //it is allways possible to rotate! no need to check
            return all_possible_rotate_squares_moves;//result = all_possible_rotate_squares_moves.Where(move => move.is_move_possible(gb)).ToArray();
        }

       // return result;
    }

    /// <summary>
    /// not really needed with current implementation, but use instead of "state_after_move()" on minimax (if needed) to keep the generic pattern
    /// </summary>
    /// <returns></returns>
    public Pentago_GameBoard board_after_play(Pentago_GameBoard gb, Pentago_Move gmd)
    {
        return gmd.state_after_move(gb);
    }

    public Pentago_GameBoard[] next_states(Pentago_GameBoard gb)
    {
        Pentago_Move[] moves = possible_plays(gb);

        if (remove_repeated_states_on_nextStates)
            return removeDuplicates(moves.Select(m => m.state_after_move(gb)).ToArray());//.Distinct().ToArray();
        else return moves.Select(m => m.state_after_move(gb)).ToArray();
    }

    public bool game_over(Pentago_GameBoard gb)
    {
        throw new NotImplementedException();
    }

    public float evaluate(Pentago_GameBoard gb)
    {
        throw new NotImplementedException();
        switch (ef)
        {
            case EvaluationFunction.func1:
                break;
            case EvaluationFunction.blabla2:
                break;
            case EvaluationFunction.blabla3:
                break;
            default:
                break;
        }
    }

}
