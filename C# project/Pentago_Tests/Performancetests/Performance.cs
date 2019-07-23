using System;
using System.Diagnostics;

public static class Performance
{
    public static TimeSpan PerformanceTimes(MinMax<Pentago_GameBoard, Pentago_Move> ia, Pentago_GameBoard[] boards)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < boards.Length; i++)
        {
            ia.run(boards[i]);
        }

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
        
        return ts;
    }
}

