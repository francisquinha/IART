//#define DEBUG_ALPHA_BETA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class MinMax <GAME_BOARD, GAME_MOVE_DESCRIPTION>
{
    private Random random = new Random();

#if DEBUG_ALPHA_BETA
    public delegate void DebugBoard(GAME_BOARD gb);

    public DebugBoard debugBoard;
#endif
    float alpha_beta_minmax(float alpha, float beta, GAME_BOARD gb, int depth, bool node)
    {
        float? gover = rules.game_over(gb, depth);
        if (gover != null)
        {
#if DEBUG_ALPHA_BETA
            Console.WriteLine("depth " + depth + " wins " + gover);
            debugBoard(gb);
#endif
            return gover.Value;
        }
        if (depth >= max_depth) return rules.evaluate(gb);
        GAME_BOARD[] nstates = rules.next_states(gb, depth);
        bool nminmax = rules.selectMINMAX(gb, node);
        float next_value;
        foreach (int i in Enumerable.Range(0, nstates.Length).OrderBy(x => random.Next()))
        {
            GAME_BOARD ngb = nstates[i];
            next_value = alpha_beta_minmax(alpha, beta, ngb, depth + 1, nminmax);
            if (node == MIN_NODE && beta > next_value) beta = next_value;
            else if (node == MAX_NODE && alpha < next_value) alpha = next_value;
            if (alpha >= beta) break;
        }
        return node == MIN_NODE ? beta : alpha;
    }

    public GAME_MOVE_DESCRIPTION[] alpha_beta_minmax_init(GAME_BOARD gb)
    {
        float alpha = float.NegativeInfinity;
        float beta = float.PositiveInfinity;
        GAME_MOVE_DESCRIPTION[] result;
        alpha_beta_minmax_init_aux(alpha, beta, gb, 0, out result);
        return result;
    }

    float alpha_beta_minmax_init_aux(float alpha, float beta, GAME_BOARD gb, int depth, out GAME_MOVE_DESCRIPTION[] moves)
    {
        moves = new GAME_MOVE_DESCRIPTION[0];
        float? gover = rules.game_over(gb, depth);
        if (gover != null) return gover.Value;
        if (depth >= max_depth) return rules.evaluate(gb);

        GAME_MOVE_DESCRIPTION[] nplays = rules.possible_plays(gb, depth);
        bool nminmax = rules.selectMINMAX(gb, MAX_NODE);
        GAME_MOVE_DESCRIPTION[] temp_moves = new GAME_MOVE_DESCRIPTION[0];
        float next_value;
        foreach (int i in Enumerable.Range(0, nplays.Length).OrderBy(x => random.Next())) {
            GAME_MOVE_DESCRIPTION nplay = nplays[i];
            GAME_BOARD ngb = rules.board_after_play(gb, nplay);
            if (nminmax == MIN_NODE) next_value = alpha_beta_minmax(alpha, beta, ngb, depth + 1, MIN_NODE);
            else next_value = alpha_beta_minmax_init_aux(alpha, beta, ngb, depth + 1, out temp_moves);
#if DEBUG_ALPHA_BETA
            Console.WriteLine("alpha " + alpha + " depth " + depth);
#endif
            if (alpha < next_value)
            {
                alpha = next_value;
                moves = new GAME_MOVE_DESCRIPTION[temp_moves.Length + 1];
                moves[0] = nplay;
                Array.Copy(temp_moves, 0, moves, 1, temp_moves.Length);
#if DEBUG_ALPHA_BETA
                Console.WriteLine(depth);
                Console.WriteLine("temp");
                temp.All(o => { Console.WriteLine(o.ToString()); return true; });
                Console.WriteLine("result");
                result.All(o => { Console.WriteLine(o.ToString()); return true; });
#endif
            }
        }
        return alpha;
    }
}
