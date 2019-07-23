using System;
using static Pentago_GameBoard;
using static Pentago_Move;
using HOLESTATE = Pentago_GameBoard.hole_state;

public class GenerateRandomBoard
{
    public Pentago_GameBoard Pentago_gb;
    private int number_of_plays;
    public bool player_turn_to_end;
    int MAX_COUNT = 100000;

    /// <summary>
    /// This is the constructor for the class that generates random board
    /// </summary>
    /// <param name="num_pieces">Number of pieces that the last player will have</param>
    /// <param name="last_player_to_play">true last player = black; false last player = white</param>
    public GenerateRandomBoard(int num_pieces, bool last_player_to_play)
    {
        Pentago_gb = new Pentago_GameBoard();

        if (last_player_to_play)
        {
            player_turn_to_end = blacks_turn;
            number_of_plays = num_pieces * 2;
        }
        else
        {
            player_turn_to_end = whites_turn;
            number_of_plays = (num_pieces * 2) - 1;
        }
    }

    public void generateNewBoard()
    {
        Pentago_gb.board = new hole_state[36];
        for (int i=0; i<number_of_plays; i++)
        {
            bool test_move = generateNewMove(ref Pentago_gb.board);

            if (!test_move)
            {
                throw new FailToGenerateBoard("Fail to generate board!");
            }

            Pentago_gb.switch_player_turn();
        }
    }

    private bool generateNewMove(ref HOLESTATE [] test_board)
    {
        bool valid_move = false;
        int count = 0; // avoid infinit loop

        while (!valid_move)
        {
            int square = GetRandomNumber(0,4); // creates a number between 0 and 4
            int x = GetRandomNumber(0, 3);
            int y = GetRandomNumber(0, 3);

            Pentago_Move new_move = new Pentago_Move(square, x, y);
            if (new_move.is_move_possible(Pentago_gb))
            {
                bool? winning_player; // not needed
                HOLESTATE[] temp = (HOLESTATE[]) test_board.Clone();
                HOLESTATE [] temp_1 = new_move.move(test_board, Pentago_gb.get_player_turn(), turn_state_addpiece);
                test_board = (HOLESTATE [])temp_1.Clone();

                if (!Pentago_gb.game_ended(out winning_player))
                {
                    valid_move = true;
                    break;
                } else
                {
                    test_board = (HOLESTATE[])temp.Clone();
                }
            }

            if (count == MAX_COUNT) break;
            count++;
        }
        return valid_move;
    }

    private static readonly Random getrandom = new Random();
    private static readonly object syncLock = new object();
    public static int GetRandomNumber(int min, int max)
    {
        lock (syncLock)
        { // synchronize
            return getrandom.Next(min, max);
        }
    }

    // debug stuff
    public void print_moves(int square, int x, int y)
    {
        Console.WriteLine("Square: " + square + ", X: " + x + ", Y: " + y);
    }

}