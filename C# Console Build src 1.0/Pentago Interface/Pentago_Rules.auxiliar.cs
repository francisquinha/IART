using System.Collections.Generic;
using System.Linq;
using HOLESTATE = Pentago_GameBoard.hole_state;

public partial class Pentago_Rules
{
    /// <summary>
    /// copies a square from one board to a square in another board
    /// </summary>
    void copy_square2(HOLESTATE[] scr, HOLESTATE[] dst, int src_square, int dst_square)
    {
        for (int i = 0; i < 3; i++)
            for (int a = 0; a < 3; a++)
                dst[Pentago_GameBoard.board_position_to_index(i, a, dst_square)] =
                       scr[Pentago_GameBoard.board_position_to_index(i, a, src_square)];
    }

    HOLESTATE[] get_rotated_board_90deg_anticlockwise(HOLESTATE[] board)
    {
        HOLESTATE[] newboard = new HOLESTATE[36];

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

    bool square0_has_maindiagonal_symmetry(HOLESTATE[] board)
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
    void board_has_symmetry(HOLESTATE[] board, out bool typeA, out bool typeB, out bool[] typeC)
    {
        typeA = true; typeB = true;
        typeC = new bool[] { false, false, false, false };

        HOLESTATE[] aux = (HOLESTATE[])board.Clone();
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

    /// <summary>
    /// select the minimum value of an arbitrary number of integers variables
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    static public int select_min(params int[] values)
    {
        return values.Min();
    }


}

