//#define PLAY_AGAINST_HUMAN

using System;
static partial class UnitTesting
{
    static public void testGenerateNewBoard()
    {
        GenerateRandomBoard ranBoard = new GenerateRandomBoard(17, Pentago_GameBoard.whites_turn);
        ranBoard.generateNewBoard();
        ranBoard.Pentago_gb.print_board();
        ranBoard.generateNewBoard();
        ranBoard.Pentago_gb.print_board();
    }
}