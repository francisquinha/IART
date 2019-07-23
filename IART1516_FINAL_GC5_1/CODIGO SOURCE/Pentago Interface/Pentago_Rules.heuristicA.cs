//#define DEBUG_HEURISTIC_A

/*
 * This heuristic is inspired by the strategy guide: https://webdav.info.ucl.ac.be/webdav/ingi2261/ProblemSet3/PentagoRulesStrategy.pdf.
 * 
 * There are 4 ways to connect 5 pieces in a row and win at Pentago:
 * 1. monica's five - in a diagonal of the board - relative strength 3;
 * 2. middle five - vertically or horizontally through the middle of two adjacent squares - relative strength 5;
 * 3. straight five - vertically or horizontally using the borders of two adjacent squares - relative strength 7;
 * 4. triple power play - diagonally below or above the board diagonals - relative strength 9. 
 * The relative strengths are based on the versatility and the difficulty of defending against each of these strategies.
 * 
 * We start by computing the number of white and black pieces in each of these lines. But we are only interested in the
 * number of pieces when there is a possibility of making 5 in a row in that line. Thus, we want to know the number of
 * white and black pieces at the borders and in the interior of these lines, since pieces in the interior are blocking
 * and it takes to pieces in the borders to block. Except for the triple power plays, in those lines, any piece blocks
 * the opponent. At the end of this counting, we know the probability of both players making 5 in a row for each line.
 * 
 * In the next step, we use both the relative strengths and the counting described above. For each line of the board,
 * we add Math.Pow(relative_strength, white_count) - Math.Pow(relative_strength, black_count) to the board score. This
 * is assuming white is the color of the AI, but in the end if the contrary is true, we simply multiply by -1. We use
 * power so that several unblocked pieces in the same line are more important than the same pieces scattered across
 * different lines.
 * 
 * Finally, we compute these board values for every board obtained from the current board by a rotation move. After this,
 * we chose the maximum or minimum of these rotated board values, according to wether the current player's turn is the AI
 * or the opponent.
 * 
 * Although we want to favor triples over monicas, for example, since we are using power based on relative strengths,
 * we do not use the values 3, 5, 7 and 9, because they lead to huge discrepancies between these plays. Instead, after
 * some consideration of the intervals these values lead to, we decided to use the relative strengths 1.13, 1.15, 1.17
 * and 1.19. The idea is to favor some over others, but not too much and keeping the final board values within reasonable
 * limits.
 * 
 * The rest of the heuristic was obtained through observation of lost games and trying to avoid these losses. The first of
 * these observations is that if a player has 4 pieces in a row (in a monica, middle or straight line) with both ends open
 * at the end of their turn, then that player will win in their next turn, unless the opponent wins in this turn. Thus,
 * upon detection of this situation we immediatly return the player that will win and compute the board value accordingly
 * (100 if AI wins, -100 if opponent wins). 
 * 
 * Another sure way to win is to obtain two or more lines that are one piece away from winning. The heuristic also tries to
 * catch these situations, although there may be some false positives if the missing piece is in the same spot for both lines.
 * Apart from this limitation, whose occurrence we consider rare, we also return 100 or -100 for these cases.
 * 
 * Finally, we do some score tweaking in the following situations:
 * - if there is a 4 in a row with both ends open but it is the oponents turn, we score it as if there are 10 pieces in that
 * line, so that it is clearly valued more than other situations;
 * - if there is an almost win (one piece away from 5 in a row) that is not in the previous case, we increase the piece count
 * by 1;
 * - if there are 1, 2 or 3, unblocked pieces in the same line (monica, middle or straight) and both line borders are empty,
 * we increase the piece count by 1.
 */

using System;

using HOLESTATE = Pentago_GameBoard.hole_state;

public partial class Pentago_Rules
{

    public void setHeuristicAStrengths(float monica_strength, float middle_strength, float straight_strength, float triple_strength)
    {
        this.monica_strength = monica_strength;
        this.middle_strength = middle_strength;
        this.straight_strength = straight_strength;
        this.triple_strength = triple_strength;
    }

    public void setHeurARANDOM()
    {
        System.Random random = new System.Random();
        this.monica_strength =      .9f + ((float)random.Next(0, 400000)) / 1000000.0f; 
        this.middle_strength =      .9f + ((float)random.Next(0, 400000)) / 1000000.0f;
        this.straight_strength =    .9f + ((float)random.Next(0, 400000)) / 1000000.0f;
        this.triple_strength =      .9f + ((float)random.Next(0, 400000)) / 1000000.0f;
    }

    public void setA_setup1()
    {
        monica_strength = 1.13f;
        middle_strength = 1.15f;
        straight_strength = 1.17f;
        triple_strength = 1.19f;
    }

    public void setA_setup2()
    {
        monica_strength = 1.23308f; middle_strength = 1.26326f; straight_strength = 1.11807f; triple_strength = 0.99693f;
    }

    public void setA_setup3()
    {
        monica_strength = 0.946981f; middle_strength = 1.120938f; straight_strength = 1.126149f; triple_strength = 1.23813f;
    }

    public void setA_setup4()
    {
        monica_strength = 0.917492f; middle_strength = 1.138074f; straight_strength = 1.259564f; triple_strength = 1.168299f;
    }

    public float heuristicA(Pentago_GameBoard gb)
    {
        float value;
        bool? player;
        float result = gb.get_player_turn() == IA_PIECES ? float.NegativeInfinity : float.PositiveInfinity;
        if (gb.get_turn_state() == Pentago_GameBoard.turn_state_addpiece)
            gb.switch_turn_state();
        Pentago_Move[] nplays = possible_plays(gb);
        Pentago_GameBoard ngb;
        foreach (Pentago_Move move in nplays)
        {
            ngb = after_rotate(gb, move);
            player = boardValue(ngb, out value);
#if DEBUG_HEURISTIC_A
            ngb.print_board();
            Console.WriteLine(value + " " + player);
#endif
            if (player != null)
                return (player == IA_PIECES) ? 100 : -100;
            if ((gb.get_player_turn() == IA_PIECES && value > result)
                || (gb.get_player_turn() != IA_PIECES && value < result))
                result = value;
        }
        return result;
    }

    bool? boardValue(Pentago_GameBoard gb, out float value)
    {
        int[] monica1 = { 5, 10, 15, 20, 25, 30 };
        int[] monica2 = { 0, 7, 14, 21, 28, 35 };
        int[][] monicas = { monica1, monica2 };

        int[] middle1 = { 6, 7, 8, 9, 10, 11 };
        int[] middle2 = { 24, 25, 26, 27, 28, 29 };
        int[] middle3 = { 1, 7, 13, 19, 25, 31 };
        int[] middle4 = { 4, 10, 16, 22, 28, 34 };
        int[][] middles = { middle1, middle2, middle3, middle4 };

        int[] straight1 = { 0, 1, 2, 3, 4, 5 };
        int[] straight2 = { 12, 13, 14, 15, 16, 17 };
        int[] straight3 = { 18, 19, 20, 21, 22, 23 };
        int[] straight4 = { 30, 31, 32, 33, 34, 35 };
        int[] straight5 = { 0, 6, 12, 18, 24, 30 };
        int[] straight6 = { 2, 8, 14, 20, 26, 32 };
        int[] straight7 = { 3, 9, 15, 21, 27, 33 };
        int[] straight8 = { 5, 11, 17, 23, 29, 35 };
        int[][] straights = { straight1, straight2, straight3, straight4, straight5, straight6, straight7, straight8 };

        int[] triple1 = { 1, 8, 15, 22, 29 };
        int[] triple2 = { 6, 13, 20, 27, 34 };
        int[] triple3 = { 4, 9, 14, 19, 24 };
        int[] triple4 = { 11, 16, 21, 26, 31 };

        int[][] triples = { triple1, triple2, triple3, triple4 };

        value = 0;
#if DEBUG_HEURISTIC_A
        Console.WriteLine("monica");
#endif
        int whiteCount;
        int blackCount;
        bool? player;
        int whiteAlmost = 0;
        int blackAlmost = 0;
        foreach (int[] monica in monicas)
        {
            player = countLine(gb, monica, out whiteCount, out blackCount);
            if (player != null) return player;
            player = checkAlmost(gb, whiteCount, blackCount, ref whiteAlmost, ref blackAlmost);
            if (player != null) return player;
            value += (float)(Math.Pow(monica_strength, whiteCount) - Math.Pow(monica_strength, blackCount));
        }
#if DEBUG_HEURISTIC_A
        Console.WriteLine("middle");
#endif
        foreach (int[] middle in middles)
        {
            player = countLine(gb, middle, out whiteCount, out blackCount);
            if (player != null) return player;
            player = checkAlmost(gb, whiteCount, blackCount, ref whiteAlmost, ref blackAlmost);
            if (player != null) return player;
            value += (float)(Math.Pow(middle_strength, whiteCount) - Math.Pow(middle_strength, blackCount));
        }
#if DEBUG_HEURISTIC_A
        Console.WriteLine("straight");
#endif
        foreach (int[] straight in straights)
        {
            player = countLine(gb, straight, out whiteCount, out blackCount);
            if (player != null) return player;
            player = checkAlmost(gb, whiteCount, blackCount, ref whiteAlmost, ref blackAlmost);
            if (player != null) return player;
            value += (float)(Math.Pow(straight_strength, whiteCount) - Math.Pow(straight_strength, blackCount));
        }
#if DEBUG_HEURISTIC_A
        Console.WriteLine("triple");
#endif
        foreach (int[] triple in triples)
        {
            countShortLine(gb, triple, out whiteCount, out blackCount);
            player = checkAlmost(gb, whiteCount, blackCount, ref whiteAlmost, ref blackAlmost);
            if (player != null) return player;
            value += (float)(Math.Pow(triple_strength, whiteCount) - Math.Pow(triple_strength, blackCount));
        }
        if (IA_PIECES == IA_PIECES_BLACKS) value *= -1;
        return null;
    }

    bool? checkAlmost(Pentago_GameBoard gb, int whiteCount, int blackCount, ref int whiteAlmost, ref int blackAlmost)
    {
        if (whiteCount > 4)
        {
            whiteAlmost++;
#if DEBUG_HEURISTIC_A
            Console.WriteLine("white almost");
#endif
        }
        if (blackCount > 4)
        {
            blackAlmost++;
#if DEBUG_HEURISTIC_A
            Console.WriteLine("black almost");
#endif
        }
        if (whiteAlmost >= 2 && gb.get_player_turn() == IA_PIECES_WHITES) return IA_PIECES_WHITES;
        if (blackAlmost >= 2 && gb.get_player_turn() == IA_PIECES_BLACKS) return IA_PIECES_BLACKS;
        return null;
    }

    bool? countLine(Pentago_GameBoard gb, int[] line, out int whiteCount, out int blackCount)
    {
        int interiorWhites = 0;
        int interiorBlacks = 0;
        int borderWhites = 0;
        int borderBlacks = 0;
        if (gb.board[line[0]] == HOLESTATE.has_white) borderWhites++;
        else if (gb.board[line[0]] == HOLESTATE.has_black) borderBlacks++;
        if (gb.board[line[line.Length - 1]] == HOLESTATE.has_white) borderWhites++;
        else if (gb.board[line[line.Length - 1]] == HOLESTATE.has_black) borderBlacks++;
        for (int i = 1; i < line.Length - 1; i++)
        {
            if (gb.board[line[i]] == HOLESTATE.has_white) interiorWhites++;
            else if (gb.board[line[i]] == HOLESTATE.has_black) interiorBlacks++;
        }
        whiteCount = 0;
        blackCount = 0;
        if (interiorBlacks == 0 && borderBlacks < 2)
            whiteCount += borderWhites + interiorWhites;
        if (interiorWhites == 0 && borderWhites < 2)
            blackCount += borderBlacks + interiorBlacks;
#if DEBUG_HEURISTIC_A
        Console.WriteLine("W " + whiteCount + "  B " + blackCount);
#endif
        if (whiteCount == 4 && borderWhites == 0 || whiteCount > 4)
        {
            if (gb.get_player_turn() == IA_PIECES_WHITES) return IA_PIECES_WHITES;
            else whiteCount = 10;
        }
        else if (whiteCount == 4 && borderWhites < 2) whiteCount++;
        else if (whiteCount > 0 && whiteCount < 5 && borderWhites == 0) whiteCount += 1 - borderBlacks;
        if (blackCount == 4 && borderBlacks == 0 || blackCount > 4)
        {
            if (gb.get_player_turn() == IA_PIECES_BLACKS) return IA_PIECES_BLACKS;
            else blackCount = 10;
        }
        else if (blackCount == 4 && borderBlacks < 2) blackCount++;
        else if (blackCount > 0 && blackCount < 5 && borderBlacks == 0) blackCount += 1 - borderWhites;
        return null;
    }

    void countShortLine(Pentago_GameBoard gb, int[] line, out int whiteCount, out int blackCount)
    {
        int whites = 0;
        int blacks = 0;
        foreach (int i in line)
        {
            if (gb.board[i] == HOLESTATE.has_white) whites++;
            else if (gb.board[i] == HOLESTATE.has_black) blacks++;
        }
        whiteCount = 0;
        blackCount = 0;
        if (blacks == 0 && whites > 0)
            whiteCount = whites;
        else if (whites == 0 && blacks > 0)
            blackCount = blacks;
#if DEBUG_HEURISTIC_A
        Console.WriteLine("W " + whiteCount + "  B " + blackCount);
#endif
    }

    HOLESTATE[] rotate(HOLESTATE[] gb, Pentago_Move move)
    {
        HOLESTATE[] new_gb = (HOLESTATE[])gb.Clone();
        if (move.rotDir == Pentago_Move.rotate_clockwise)
            for (int x = 0; x < 3; ++x) for (int y = 0; y < 3; ++y)
                    new_gb[Pentago_GameBoard.board_position_to_index(Math.Abs(2 - y), x, move.square2rotate)]
                        = gb[Pentago_GameBoard.board_position_to_index(x, y, move.square2rotate)];
        else
            for (int x = 0; x < 3; ++x) for (int y = 0; y < 3; ++y)
                    new_gb[Pentago_GameBoard.board_position_to_index(y, Math.Abs(2 - x), move.square2rotate)]
                        = gb[Pentago_GameBoard.board_position_to_index(x, y, move.square2rotate)];
        return new_gb;
    }

    Pentago_GameBoard after_rotate(Pentago_GameBoard gb, Pentago_Move move)
    {
        return new Pentago_GameBoard(rotate(gb.board, move), gb.get_player_turn(), gb.get_turn_state());
    }

}