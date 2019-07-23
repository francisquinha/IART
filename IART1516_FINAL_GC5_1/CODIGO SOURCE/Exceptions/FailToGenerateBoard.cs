using System;



class FailToGenerateBoard: Exception
{
    public FailToGenerateBoard()
    {
    }

    public FailToGenerateBoard(string message)
        : base(message)
    {
    }

    public FailToGenerateBoard(string message, Exception inner)
        : base(message, inner)
    {
    }
}

