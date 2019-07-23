//#define DEBUG_ALPHA_BETA

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class MT{
[DllImport("kernel32.dll")]
public static extern IntPtr GetCurrentThread();
[DllImport("kernel32.dll")]
public static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);
}

public partial class MinMax <GAME_BOARD, GAME_MOVE_DESCRIPTION>
{



    float alpha_beta_minmaxMT(float alpha, float beta, GAME_BOARD gb, int depth, bool node)
    {
        Random random = new Random();//probably faster than locking the same

        float? gover = rules.game_over(gb, depth);
        if (gover != null)
        {
            return gover.Value;
        }
        if (depth >= max_depth) return rules.evaluate(gb);
        GAME_BOARD[] nstates = rules.next_states(gb);
        bool nminmax = rules.selectMINMAX(gb, node);
        float next_value;
        foreach(int i in Enumerable.Range(0, nstates.Length).OrderBy(x => random.Next()))
        {
            GAME_BOARD ngb = nstates[i];
            next_value = alpha_beta_minmaxMT(alpha, beta, ngb, depth + 1, nminmax);
            if (node == MIN_NODE && beta > next_value) beta = next_value;
            else if (node == MAX_NODE && alpha < next_value) alpha = next_value;
            if (alpha >= beta) break;
        }
        return node == MIN_NODE ? beta : alpha;
    }


    public int num_of_thread = 2;

    GAME_MOVE_DESCRIPTION[] alpha_beta_minmax_initMTO(GAME_BOARD gb)
    {
        using (Process p = Process.GetCurrentProcess())
        {
            p.PriorityClass = ProcessPriorityClass.High;
        }

        //float alpha[] = float.NegativeInfinity;
       // float beta = float.PositiveInfinity;


        GAME_MOVE_DESCRIPTION[] nplays = rules.possible_plays(gb, 0);
        GAME_MOVE_DESCRIPTION[][] moves = new GAME_MOVE_DESCRIPTION[nplays.Length][];
        float[] alphas = new float[nplays.Length];
        for (int i = alphas.Length - 1; i >= 0; --i) alphas[i] = float.NegativeInfinity;
        bool nminmax = rules.selectMINMAX(gb, MAX_NODE);

        float next_value;
        /*   Parallel.ForEach(Enumerable.Range(0, nplays.Length).OrderBy(x => random.Next())
      , (int i) =>
      {
          Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
          GAME_MOVE_DESCRIPTION nplay = nplays[i];
          GAME_BOARD ngb = rules.board_after_play(gb, nplay);
          if (nminmax == MIN_NODE) next_value = alpha_beta_minmaxMT(alpha, beta, ngb, 1, MIN_NODE);
          else next_value = alpha_beta_minmax_init_auxMT(alpha, beta, ngb, 1, out temp_moves);

          if (alpha < next_value)
          {
              alpha = next_value;
              moves = new GAME_MOVE_DESCRIPTION[temp_moves.Length + 1];
              moves[0] = nplay;
              Array.Copy(temp_moves, 0, moves, 1, temp_moves.Length);
          }
      });*/
        float alpha = float.NegativeInfinity;
        Thread[] ts = new Thread[nplays.Length];
        foreach (int n in Enumerable.Range(0, nplays.Length).OrderBy(x => random.Next()))
        {
            ts[n] = new Thread((m) =>
           {
               int i = (int)m;
               GAME_MOVE_DESCRIPTION[] temp_moves = new GAME_MOVE_DESCRIPTION[0];
               GAME_MOVE_DESCRIPTION nplay = nplays[i];
               GAME_BOARD ngb = rules.board_after_play(gb, nplay);
               if (nminmax == MIN_NODE) next_value = alpha_beta_minmax(float.NegativeInfinity, float.PositiveInfinity, ngb, 1, MIN_NODE);
               else next_value = alpha_beta_minmax_init_aux(float.NegativeInfinity, float.PositiveInfinity, ngb, 1, out temp_moves);

               //using array instead of a lock (alpha can speed up the proccess but wont save best move accordingly, the alpha array will be used instead)
               if (alpha < next_value)
                {
                    alpha = next_value;
                   alphas[i] = next_value;
                   moves[i] = new GAME_MOVE_DESCRIPTION[temp_moves.Length + 1];
                    moves[i][0] = nplay;
                    Array.Copy(temp_moves, 0, moves[i], 1, temp_moves.Length);
                }
           });
            ts[n].Priority = ThreadPriority.AboveNormal;
            ts[n].SetApartmentState(ApartmentState.MTA);
           // ts[n].DisableComObjectEagerCleanup();
            ts[n].Start(n);
        }
        for (int i = ts.Length - 1; i >= 0; --i) ts[i].Join();

        int best = Array.IndexOf(alphas, alphas.Max());
        return moves[best];

    }

    GAME_MOVE_DESCRIPTION[] alpha_beta_minmax_initMT(GAME_BOARD gb)
    {
        Random rand = new Random();
        GAME_MOVE_DESCRIPTION[] nplays = rules.possible_plays(gb);
        nplays = nplays.OrderBy(x => rand.Next()).ToArray();

        if (nplays.Length < 4) return alpha_beta_minmax_init(gb);

        bool nminmax = rules.selectMINMAX(gb, MAX_NODE);


        float[] alpha = new float[num_of_thread];
        for (int a = alpha.Length - 1; a >= 0; --a) alpha[a] = float.NegativeInfinity;

        GAME_MOVE_DESCRIPTION[][] moves = new GAME_MOVE_DESCRIPTION[num_of_thread][];

        //ParallelOptions po = new ParallelOptions();
        //po.MaxDegreeOfParallelism = num_of_thread;

        /* Parallel.For(0, num_of_thread,
             //po,
              (n) =>
          {
              GAME_MOVE_DESCRIPTION[] temp_moves = new GAME_MOVE_DESCRIPTION[0];
              float next_value;
              int offset = nplays.Length / num_of_thread * n;
              int i = nplays.Length/ num_of_thread;
              if (n == num_of_thread-1) i += nplays.Length % num_of_thread;
              --i;
              for (; i >= 0; --i)
              {
                  GAME_MOVE_DESCRIPTION nplay = nplays[offset+i];
                  GAME_BOARD ngb = rules.board_after_play(gb, nplay);

                  if (nminmax == MIN_NODE) next_value = alpha_beta_minmaxMT(alpha[n], float.NegativeInfinity, ngb, 1, MIN_NODE);
                  else next_value = alpha_beta_minmax_init_auxMT(float.NegativeInfinity, float.PositiveInfinity, ngb, 1, out temp_moves);

                  if (alpha[n] < next_value)
                  {
                      alpha[n] = next_value;
                      moves[n] = new GAME_MOVE_DESCRIPTION[temp_moves.Length + 1];
                      moves[n][0] = nplay;
                      Array.Copy(temp_moves, 0, moves[n], 1, temp_moves.Length);
                  }

              }

          });*/

        using (Process p = Process.GetCurrentProcess())
            p.PriorityClass = ProcessPriorityClass.High;

            //Semaphore sem = new Semaphore(0,1);
            Thread[] threads = new Thread[num_of_thread];
        //Task[] task = new Task[num_of_thread];
        float alphaH = float.NegativeInfinity;
        for (int n = 0; n < num_of_thread; ++n)
        {
            threads[n] = new Thread(//abmt);
            
                   (no) =>
                   {
                       int nn=(int)no;

                  // MT.SetThreadAffinityMask(MT.GetCurrentThread(), new IntPtr(0xFFFE));// 1 << nn));
                    //   Thread.BeginThreadAffinity();
                       GAME_MOVE_DESCRIPTION[] temp_moves = new GAME_MOVE_DESCRIPTION[0];
                       float next_value;
                       int offset = nplays.Length / num_of_thread * nn;
                       int i = nplays.Length / num_of_thread;
                       if (nn == num_of_thread - 1) i += nplays.Length % num_of_thread;
                       --i;
                       for (; i >= 0; --i)
                       {
                           GAME_MOVE_DESCRIPTION nplay = nplays[offset + i];
                           GAME_BOARD ngb = rules.board_after_play(gb, nplay);

                           if (nminmax == MIN_NODE) next_value = alpha_beta_minmaxMT(alpha[nn], float.NegativeInfinity, ngb, 1, MIN_NODE);
                           else next_value = alpha_beta_minmax_init_auxMT(float.NegativeInfinity, float.PositiveInfinity, ngb, 1, out temp_moves);

                           if (alpha[nn] < next_value)//not loking, using an array of answer instead
                           {
                               alphaH = next_value;
                               alpha[nn] = next_value;
                               moves[nn] = new GAME_MOVE_DESCRIPTION[temp_moves.Length + 1];
                               moves[nn][0] = nplay;
                               Array.Copy(temp_moves, 0, moves[nn], 1, temp_moves.Length);
                           }

                       }
                       return;
                   }
                    );
            //threads[n].IsBackground = true;
            threads[n].TrySetApartmentState(ApartmentState.MTA);
            // threads[n].Priority = ThreadPriorityLevel.TimeCritical;
            //threads[n].DisableComObjectEagerCleanup();
            //threads[n].DisableComObjectEagerCleanup();
            threads[n].Priority = (ThreadPriority)4;// ThreadPriority.AboveNormal;
             threads[n].Start(n);

           // threads[n].Start(new object[] { gb, n, nplays, nminmax, alpha, moves });
           // task[n] = abmt(gb, n, nplays, nminmax, alpha, moves);
        }
        for (int i = 0; i < num_of_thread; ++i)
            threads[i].Join();
           // task[i].Wait();

        int best = Array.IndexOf(alpha, alpha.Max());

        return moves[best];
    }

    /*async*/
    private void abmt( GAME_BOARD gb,int nn, GAME_MOVE_DESCRIPTION[] nplays, bool nminmax, float[] alpha, GAME_MOVE_DESCRIPTION[][] moves)
    {

    /* GAME_BOARD gb = (GAME_BOARD) ((Object[]) o)[0];
    int nn = (int)((Object[])o)[1];
    GAME_MOVE_DESCRIPTION[] nplays = (GAME_MOVE_DESCRIPTION[])((Object[])o)[2];
    bool nminmax = (bool)((Object[])o)[3];
    float[] alpha = (float[])((Object[])o)[4];
    GAME_MOVE_DESCRIPTION[][] moves = (GAME_MOVE_DESCRIPTION[][])((Object[])o)[5];
     */

        GAME_MOVE_DESCRIPTION[] temp_moves = new GAME_MOVE_DESCRIPTION[0];
        float next_value;
        int offset = nplays.Length / num_of_thread * nn;
        int i = nplays.Length / num_of_thread;
        if (nn == num_of_thread - 1) i += nplays.Length % num_of_thread;
        --i;
        for (; i >= 0; --i)
        {
            GAME_MOVE_DESCRIPTION nplay = nplays[offset + i];
            GAME_BOARD ngb = rules.board_after_play(gb, nplay);

            if (nminmax == MIN_NODE) next_value = alpha_beta_minmaxMT(alpha[nn], float.NegativeInfinity, ngb, 1, MIN_NODE);
            else next_value = alpha_beta_minmax_init_auxMT(float.NegativeInfinity, float.PositiveInfinity, ngb, 1, out temp_moves);

            if (alpha[nn] < next_value)
            {
                alpha[nn] = next_value;
                moves[nn] = new GAME_MOVE_DESCRIPTION[temp_moves.Length + 1];
                moves[nn][0] = nplay;
                Array.Copy(temp_moves, 0, moves[nn], 1, temp_moves.Length);
            }

        }
    }

    float alpha_beta_minmax_init_auxMT(float alpha, float beta, GAME_BOARD gb, int depth, out GAME_MOVE_DESCRIPTION[] moves)
    {
        //Random random = new Random();//probably faster than locking the same

        moves = new GAME_MOVE_DESCRIPTION[0];
        float? gover = rules.game_over(gb, depth);
        if (gover != null) return gover.Value;
        if (depth >= max_depth) return rules.evaluate(gb);

        GAME_MOVE_DESCRIPTION[] nplays = rules.possible_plays(gb);
        bool nminmax = rules.selectMINMAX(gb, MAX_NODE);
        GAME_MOVE_DESCRIPTION[] temp_moves = new GAME_MOVE_DESCRIPTION[0];
        float next_value;
        foreach (int i in Enumerable.Range(0, nplays.Length).OrderBy(x => random.Next()))
        {
            GAME_MOVE_DESCRIPTION nplay = nplays[i];
            GAME_BOARD ngb = rules.board_after_play(gb, nplay);
            if (nminmax == MIN_NODE) next_value = alpha_beta_minmaxMT(alpha, beta, ngb, depth + 1, MIN_NODE);
            else next_value = alpha_beta_minmax_init_auxMT(alpha, beta, ngb, depth + 1, out temp_moves);

            if (alpha < next_value)
            {
                alpha = next_value;
                moves = new GAME_MOVE_DESCRIPTION[temp_moves.Length + 1];
                moves[0] = nplay;
                Array.Copy(temp_moves, 0, moves, 1, temp_moves.Length);
            }

        }
        return alpha;
    }




}
