using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using MINMAX = MinMax<Pentago_GameBoard, Pentago_Move>;
using MMTYPE = MinMax<Pentago_GameBoard, Pentago_Move>.VERSION;

static class PerformanceTests
{
    static public void testPerformnace(int numOfBorads)
    {
        using (Process p = Process.GetCurrentProcess())
        {
            p.PriorityClass = ProcessPriorityClass.High;
        }

        Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

        Pentago_GameBoard[] testBoardsWhites = new Pentago_GameBoard[numOfBorads];
        Pentago_GameBoard[] testBoardsBlacks = new Pentago_GameBoard[numOfBorads];

        for (int i = 0; i < numOfBorads; i++)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(0, 16);
            GenerateRandomBoard rndBoard = new GenerateRandomBoard(numPieces, true);
            rndBoard.generateNewBoard();
            testBoardsWhites[i] = rndBoard.Pentago_gb;
        }

        for (int i = 0; i < numOfBorads; i++)
        {
            int numPieces = GenerateRandomBoard.GetRandomNumber(0, 16);
            GenerateRandomBoard rndBoard = new GenerateRandomBoard(numPieces, false);
            rndBoard.generateNewBoard();
            testBoardsBlacks[i] = rndBoard.Pentago_gb;
        }





        Pentago_Rules wrulesm = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
    Pentago_Rules.NextStatesFunction.all_states,
    Pentago_Rules.IA_PIECES_WHITES, false);
        Pentago_Rules brulesm = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
            Pentago_Rules.NextStatesFunction.all_states,
            Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_wm = new MINMAX(MINMAX.VERSION.minmax, wrulesm, 4);
        MINMAX test_bm = new MINMAX(MINMAX.VERSION.minmax, brulesm, 4);
        TimeSpan test1m = Performance.PerformanceTimes(test_wm, testBoardsWhites);
        TimeSpan test2m = Performance.PerformanceTimes(test_bm, testBoardsBlacks);

        TimeSpan tsm = test1m.Add(test2m);

        // Format and display the TimeSpan value.
        string elapsedTimem = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            tsm.Hours, tsm.Minutes, tsm.Seconds,
            tsm.Milliseconds / 10);

        Console.WriteLine("RunTime " + elapsedTimem);



        Pentago_Rules wrules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
    Pentago_Rules.NextStatesFunction.all_states,
    Pentago_Rules.IA_PIECES_WHITES, false);
        Pentago_Rules brules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
    Pentago_Rules.NextStatesFunction.all_states,
    Pentago_Rules.IA_PIECES_BLACKS, false);
        MINMAX test_w = new MINMAX(MINMAX.VERSION.alphabeta, wrules, 4);
        MINMAX test_b = new MINMAX(MINMAX.VERSION.alphabeta, brules, 4);

        TimeSpan test1 = Performance.PerformanceTimes(test_w, testBoardsWhites);
        TimeSpan test2 = Performance.PerformanceTimes(test_b, testBoardsBlacks);

        TimeSpan ts = test1.Add(test2);

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

        Console.WriteLine("RunTime " + elapsedTime);


    }





    static public void testPerformance(int numOfBorads, int numpieces, int depth, MINMAX[] toTest)
    {
        Pentago_GameBoard[] testBoards = new Pentago_GameBoard[numOfBorads];
        String filename_compareP = "CompareMinmaxsWithEqualPieces/comp_D" + depth.ToString();
        add2File(filename_compareP, numpieces.ToString(),false);

        if (numpieces % 2 == 0)
        {
            for (int i = 0; i < numOfBorads; i++)
            {
                GenerateRandomBoard rndBoard = new GenerateRandomBoard(numpieces / 2, true);
                rndBoard.generateNewBoard();
                testBoards[i] = rndBoard.Pentago_gb;
            }
        }
        else {
            for (int i = 0; i < numOfBorads; i++)
            {
                GenerateRandomBoard rndBoard = new GenerateRandomBoard((numpieces + 1) / 2, false);
                rndBoard.generateNewBoard();
                testBoards[i] = rndBoard.Pentago_gb;
            }
        }

        for (int i = 0; i < toTest.Length; ++i)
        {
            if (numpieces % 2 == 0) ((Pentago_Rules)toTest[i].rules).IA_PIECES = Pentago_Rules.IA_PIECES_WHITES;
            else ((Pentago_Rules)toTest[i].rules).IA_PIECES = Pentago_Rules.IA_PIECES_BLACKS;

            long test = Performance.PerformanceTimeMilisecs(toTest[i], testBoards);
            string timeStr = test.ToString();

            String vers = toTest[i].version.ToString() + "_" + ((Pentago_Rules)toTest[i].rules).nsf.ToString() + "_" + ((Pentago_Rules)toTest[i].rules).remove_repeated_states_on_nextStates.ToString();
            String filename_depth = "CheckComplexityPieces/" + vers + "_D" + depth.ToString();
            String filename_pieces = "CheckComplexityDepth/" + vers + "_P" + numpieces;

            add2File(filename_depth, numpieces.ToString() + ";" + timeStr);
            add2File(filename_pieces, depth.ToString() + ";"+timeStr);
            add2File(filename_compareP, ";" + timeStr,false);
        }
        add2File(filename_compareP,"");

    }

    static public void testPerfomanceSuite4Depth(int depth)
    {
        Pentago_Rules all = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
        Pentago_Rules.NextStatesFunction.all_states,
        Pentago_Rules.IA_PIECES_WHITES, false);
        Pentago_Rules sym = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
        Pentago_Rules.NextStatesFunction.check_symmetries,
        Pentago_Rules.IA_PIECES_WHITES, false);
        Pentago_Rules remdups = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
        Pentago_Rules.NextStatesFunction.all_states,
        Pentago_Rules.IA_PIECES_WHITES, true);
        Pentago_Rules dupsNsym = new Pentago_Rules(Pentago_Rules.EvaluationFunction.one,
        Pentago_Rules.NextStatesFunction.check_symmetries,
        Pentago_Rules.IA_PIECES_WHITES, true);

        MINMAX[] totest = {
            new MINMAX(MINMAX.VERSION.minmax, all, depth),
             new MINMAX(MINMAX.VERSION.minmax, sym, depth),
              new MINMAX(MINMAX.VERSION.minmax, remdups, depth),
               new MINMAX(MINMAX.VERSION.minmax, dupsNsym, depth),
            new MINMAX(MINMAX.VERSION.alphabeta, all, depth),
             new MINMAX(MINMAX.VERSION.alphabeta, sym, depth),
              new MINMAX(MINMAX.VERSION.alphabeta, remdups, depth),
               new MINMAX(MINMAX.VERSION.alphabeta, dupsNsym, depth),
        };

        int[] pieces2test = new int[24].Select((elem, ind) => ind).ToArray();

        add2File("CompareMinmaxsWithEqualPieces/comp_D" + depth.ToString(), totest.Aggregate<MINMAX,String>(""
            ,(sum,elem)=> 
            sum+=";"+ elem.version.ToString() + "_" + ((Pentago_Rules)elem.rules).nsf.ToString() + "_" + ((Pentago_Rules)elem.rules).remove_repeated_states_on_nextStates.ToString()
            ));
        foreach (int i in pieces2test)
        {
            testPerformance(numTests, i, depth, totest);
        }

    }

    static int numTests = 100;
    public static void allTests()
    {
        using (Process p = Process.GetCurrentProcess())
        {
            p.PriorityClass = ProcessPriorityClass.High;
        }

        Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

        if (!Directory.Exists("CompareMinmaxsWithEqualPieces"))     Directory.CreateDirectory("CompareMinmaxsWithEqualPieces");
        if (!Directory.Exists("CheckComplexityPieces")) Directory.CreateDirectory("CheckComplexityPieces");
        if (!Directory.Exists("CheckComplexityDepth")) Directory.CreateDirectory("CheckComplexityDepth");

        int[] depths = new int[4].Select((elem, ind) => (ind + 1)*2).ToArray();
        foreach(int i in depths)
        testPerfomanceSuite4Depth(i);

    }

    static void add2File(string filePath,string line2add,bool line=true)
    {
        if (!File.Exists(filePath))
        {
            // Create a file to write to.
            StreamWriter swNew = File.CreateText(filePath);
            if (line) swNew.WriteLine(line2add);
            else swNew.Write(line2add);
            swNew.Close();
        }
        else
        {

            StreamWriter swAppend = File.AppendText(filePath);
            if(line) swAppend.WriteLine(line2add);
            else swAppend.Write(line2add);
            swAppend.Close();
        }
    }


}