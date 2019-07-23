using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static partial class UnitTesting
{
    static public void testHeuristicA()
    {
        initialize_test_gameboards();
        Pentago_Rules rules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.heuristicAstar,
            Pentago_Rules.NextStatesFunction.check_symmetries,
            Pentago_Rules.IA_PIECES_WHITES, true);
        Console.WriteLine(rules.heuristicAstar(boardHeuristicA));
    }
}
