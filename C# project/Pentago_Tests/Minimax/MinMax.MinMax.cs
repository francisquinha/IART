//#define DEBUG_MIN_MAX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class MinMax <GAME_BOARD, GAME_MOVE_DESCRIPTION>
{
#if DEBUG_MIN_MAX
    public delegate void DebugBoard(GAME_BOARD gb);

    public DebugBoard debugBoard;
#endif
    float minmax(GAME_BOARD gb, int depth, bool node)
    {
        float? gover = rules.game_over(gb, depth);
        if (gover != null)
        {
#if DEBUG_ALPHA_MIN_MAX
            Console.WriteLine("depth " + depth + " wins " + gover);
            debugBoard(gb);
#endif
            return gover.Value;
        }
        if (depth >= max_depth) return rules.evaluate(gb);
        GAME_BOARD[] nstates = rules.next_states(gb,depth);
        bool nminmax = rules.selectMINMAX(gb, node);
        float next_value;
        float temp_value;
        if (node == MIN_NODE) temp_value = float.PositiveInfinity;
        else temp_value = float.NegativeInfinity;
        foreach (GAME_BOARD ngb in nstates)
        {
            next_value = minmax(ngb, depth + 1, nminmax);
            if (node == MIN_NODE && temp_value > next_value) temp_value = next_value;
            else if (node == MAX_NODE && temp_value < next_value) temp_value = next_value;
        }
        return temp_value;
    }

    public GAME_MOVE_DESCRIPTION[] minmax_init(GAME_BOARD gb)
    {
        GAME_MOVE_DESCRIPTION[] result;
        minimax_init_aux(gb, 0, out result);
        return result;
    }

    float minimax_init_aux(GAME_BOARD gb, int depth, out GAME_MOVE_DESCRIPTION[] moves)
    {
        moves = new GAME_MOVE_DESCRIPTION[0];
        float? gover = rules.game_over(gb, depth);
        if (gover != null) return gover.Value;
        if (depth >= max_depth) return rules.evaluate(gb);

        GAME_MOVE_DESCRIPTION[] nplays = rules.possible_plays(gb, depth);
        bool nminmax = rules.selectMINMAX(gb, MAX_NODE);
        GAME_MOVE_DESCRIPTION[] result = null;
        GAME_MOVE_DESCRIPTION[] temp;
        float next_value;
        float temp_value = float.NegativeInfinity;
        foreach (GAME_MOVE_DESCRIPTION nplay in nplays)
        {
            GAME_BOARD ngb = rules.board_after_play(gb, nplay);
            if (nminmax == MIN_NODE)
            {
                next_value = minmax(ngb, depth + 1, MIN_NODE);
                temp = new GAME_MOVE_DESCRIPTION[0];
            }
            else next_value = minimax_init_aux(ngb, depth + 1, out temp);
#if DEBUG_MIN_MAX
            Console.WriteLine("depth " + depth);
#endif
            if (temp_value < next_value)
            {
                temp_value = next_value;
                result = new GAME_MOVE_DESCRIPTION[temp.Length + 1];
                result[0] = nplay;
                Array.Copy(temp, 0, result, 1, temp.Length);
#if DEBUG_MIN_MAX
                Console.WriteLine(depth);
                Console.WriteLine("temp");
                temp.All(o => { Console.WriteLine(o.ToString()); return true; });
                Console.WriteLine("result");
                result.All(o => { Console.WriteLine(o.ToString()); return true; });
#endif
            }
        }
        moves = result;
        return temp_value;
    }
}
