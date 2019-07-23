using System;

public class Pentago_GameBoard 
{

    /* indexes in the game board
	 * 0  1  2 | 3  4  5
	 * 6  7  8 | 9  10 11
	 * 12 13 14| 15 16 17
	 * -------------------
	 * 18 19 20| 21 22 23
	 * 24 25 26| 27 28 29
	 * 30 31 32| 33 34 35 
	 * */
    public enum hole_state { is_empty = 0, has_white = 1, has_black = -1 }

    public hole_state[] board; /*faster 2 read if needed*/

    public int emptyholes = 36;

    public const bool whites_turn = false;
    public const bool blacks_turn = true;
    private bool player_turn;
    public bool get_player_turn() { return player_turn; }//avoids checking board
    public void switch_player_turn() { player_turn = !player_turn; }

    public const bool turn_state_addpiece = false;
    public const bool turn_state_rotate = true;
    private bool turn_state;
    public bool get_turn_state() { return turn_state; }
    public void switch_turn_state() { turn_state = !turn_state; }


    public Pentago_GameBoard()
    {
        board = new hole_state[36];
        for (int i = board.Length - 1; i >= 0; --i) board[i] = 0;
        player_turn = whites_turn;
        turn_state = turn_state_addpiece;

    }

    public Pentago_GameBoard(hole_state[] board, bool turn, bool turnState)
    {
        this.board = board;
        player_turn = turn;
        turn_state = turnState;
    }

    public Pentago_GameBoard(hole_state[] board, bool turn, bool turnState, int emptyholes)
    {
        this.board = board;
        player_turn = turn;
        turn_state = turnState;
        this.emptyholes = emptyholes;
    }

    public static int board_position_to_index(int x, int y, int square)
    {

        //Pre contitions -------------------------------------------------
        //remove once everything is working
        if (x < 0 | x > 2) { Console.WriteLine("board_position_to_index - Not a valid square"); return -1; }
        if (y < 0 | y > 2) { Console.WriteLine("board_position_to_index - Not a valid square"); return -1; }
        if (square < 0 | square > 3) { Console.WriteLine("board_position_to_index - Not a valid square"); return -1; }

        //Method body ----------------------------------------------------
        int index = 0;

        switch (square)
        {
            case 0:
                break;
            case 1:
                index = 3;
                break;
            case 2:
                index = 18;
                break;
            case 3:
                index = 21;
                break;
            default:
                break;
        }

        return index + x + y * 6;
    }

    public static void board_index_to_position(int index, out int x , out int y, out int outsquare)
    {
        outsquare = 0;
        int line = index / 6;
        int row = index % 6;
        if (line > 2) outsquare = 2;
        if (row > 2) outsquare++;

        x = row % 3;
        y = line % 3;
    }

    public static bool compare_boards(hole_state[] b1, hole_state[] b2)
    {
        for (int i = 0; i < 36; i++)
            if (b1[i] != b2[i]) return false;
        return true;
    }

    public static bool operator ==(Pentago_GameBoard b1, Pentago_GameBoard b2)
    {

        if (b1.turn_state != b2.turn_state || b1.player_turn != b2.player_turn) return false;

        for (int i = 0; i < 36; i++)
            if (b1.board[i] != b2.board[i]) return false;

        return true;
    }

    public static bool operator !=(Pentago_GameBoard b1, Pentago_GameBoard b2)
    {
        return !(b1 == b2);
    }

    public override bool Equals(object obj) {
        if (obj is Pentago_GameBoard)
            return (Pentago_GameBoard)obj == this;
        else return false;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public void print_board()
    {

        Console.WriteLine(" --------------- ");
        int index = 0;
        for (int i = 0; i < 12; ++i)
        {
            Console.Write("| ");
            while (index < i * 3 + 3)
            {
                switch (board[index])
                {
                    case hole_state.is_empty: Console.Write("  "); break;
                    case hole_state.has_white: Console.Write("W "); break;
                    case hole_state.has_black: Console.Write("B "); break;
                    default: Console.Write("X "); break;
                }
                index++;
            }
            if (i % 2 == 1) Console.WriteLine("| ");
            if (i == 5) Console.WriteLine("|---------------|");
        }
        Console.WriteLine(" --------------- ");
    }

    public string toString()
    {
        string result = " --------------- \n";
        int index = 0;
        for (int i = 0; i < 12; ++i)
        {
            result += "| ";
            while (index < i * 3 + 3)
            {
                switch (board[index])
                {
                    case hole_state.is_empty: result += "  "; break;
                    case hole_state.has_white: result += "W "; break;
                    case hole_state.has_black:result += "B "; break;
                    default: result += "X "; break;
                }
                index++;
            }
            if (i % 2 == 1) result += "| \n";
            if (i == 5) result += "|---------------|\n";
        }
        result += " --------------- \n";
        return result;
    }

    private void check_small_diagonal(int diagonal, ref bool white_made_a_line, ref bool black_made_a_line)
    {
        int sequence_size = 0;
        for (int i = 0; i < 5; i++)
        {
            hole_state hole= hole_state.is_empty;
            switch (diagonal)
            {
                case 0: hole = board[(i + 1) * 6 + i]; break;
                case 1: hole = board[i * 6 + i+1]; break;
                case 2: hole = board[(i + 1) * 6 + 5-i]; break;
                case 3: hole = board[i * 6 +4-i]; break;
                default: break;
            }

            if (hole == hole_state.is_empty)
                    break;//speed up, can't be any line there


            if (hole == hole_state.has_white && sequence_size >= 0)
            {
                if (sequence_size < 0) return; //speedup
                ++sequence_size;
                if (sequence_size == 5) { white_made_a_line = true; break; }
            }
            else if (hole == hole_state.has_black && sequence_size <= 0)
            {
                if (sequence_size > 0) return; //speedup
                --sequence_size;
                if (sequence_size == -5) { black_made_a_line = true; break; }
            }
            else sequence_size = 0;
        }
    }

    /// <summary>
    /// check if game has ended, and if there is a winner
    /// </summary>
    /// <param name="player"> player = null if draw or game did not end, false if whites win (= to turn value)</param>
    /// <returns>true if ended</returns>
    public bool game_ended(out bool? player)
    {
        bool board_full = true;
        bool white_made_a_line = false;
        bool black_made_a_line = false;
        int sequence_size;//counts positive 4 white and negative 4 black to avoid using another var

        //could also check horizontals and verticals in one iteration using more memory, not sure what is best

        //check horizontals and if board is full
        for (int line = 0; line < 6; ++line)
        {
            if (board_full) if (board[line * 6] == hole_state.is_empty) board_full = false;
            sequence_size = (int)board[line * 6];

            for (int row = 1; row < 6; ++row)
            {
                hole_state hole = board[line * 6 + row];

                if (hole == hole_state.is_empty)
                {
                    board_full = false;
                    if (row != 5) break;//speed up, can't be any line there
                }

                if (hole == hole_state.has_white)
                {
                    if (sequence_size < 0) sequence_size = 0;
                    ++sequence_size;
                    if (sequence_size == 5) { white_made_a_line = true; break; }
                }
                else if (hole == hole_state.has_black)
                {
                    if (sequence_size > 0) sequence_size = 0;
                    --sequence_size;
                    if (sequence_size == -5) { black_made_a_line = true; break; }
                }
                else sequence_size = 0;
            }
        }

        //check verticals
        for (int row = 0; row < 6; ++row)
        {
            sequence_size = (int)board[row];

            for (int line = 1; line < 6; ++line)
            {
                hole_state hole = board[line * 6 + row];

                if (hole == hole_state.is_empty)
                    if (line != 5) break;//speed up, can't be any line there


                if (hole == hole_state.has_white)
                {
                    if (sequence_size < 0) sequence_size = 0;
                    ++sequence_size;
                    if (sequence_size == 5) { white_made_a_line = true; break; }
                }
                else if (hole == hole_state.has_black)
                {
                    if (sequence_size > 0) sequence_size = 0;
                    --sequence_size;
                    if (sequence_size == -5) { black_made_a_line = true; break; }
                }
                else sequence_size = 0;
            }
        }

        //check 2 larger diagonals
        sequence_size = (int)board[0];
        for (int i = 1; i < 6; i++)
        {
            hole_state hole = board[i * 6 + i];
            if (hole == hole_state.is_empty)
                if(i!=5) break;//speed up, can't be any line there
                

            if (hole == hole_state.has_white)
            {
                if (sequence_size < 0) sequence_size = 0;
                ++sequence_size;
                if (sequence_size == 5) { white_made_a_line = true; break; }
            }
            else if (hole == hole_state.has_black)
            {
                if (sequence_size > 0) sequence_size = 0;
                --sequence_size;
                if (sequence_size == -5) { black_made_a_line = true; break; }
            }
            else sequence_size = 0;
        }
        sequence_size = (int)board[5];
        for (int i = 1; i < 6; i++)
        {
            hole_state hole = board[i * 6 + 5-i];
            if (hole == hole_state.is_empty)
                if (i!=5) break;//speed up, can't be any line there
                

            if (hole == hole_state.has_white)
            {
                if (sequence_size < 0) sequence_size = 0;
                ++sequence_size;
                if (sequence_size == 5) { white_made_a_line = true; break; }
            }
            else if (hole == hole_state.has_black)
            {
                if (sequence_size > 0) sequence_size = 0;
                --sequence_size;
                if (sequence_size == -5) { black_made_a_line = true; break; }
            }
            else sequence_size = 0;
        }

        //check 4 smaller diagonals
        for (int i = 0; i < 4; i++)
        check_small_diagonal(i,ref white_made_a_line,ref black_made_a_line);

        if (white_made_a_line && !black_made_a_line) player = whites_turn;
        else if (!white_made_a_line && black_made_a_line) player = blacks_turn;
        else player = null;

        if (turn_state == turn_state_rotate) board_full = false;
        return board_full || black_made_a_line || white_made_a_line;
    }

    public Pentago_GameBoard Clone()
    {
        return new Pentago_GameBoard((hole_state[]) board.Clone(), player_turn, turn_state,emptyholes);
    }
}