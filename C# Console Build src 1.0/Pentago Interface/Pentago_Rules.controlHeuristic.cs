using HOLESTATE = Pentago_GameBoard.hole_state;

partial class Pentago_Rules
{

    /// <summary>
    /// Dummy heuristic, used to compare other heuristics performance
    /// </summary>
    /// <param name="gb"></param>
    /// <returns></returns>
    public float ControlHeuristic(HOLESTATE[] gb)
    {
        System.Random r = new System.Random();
        return (float) r.NextDouble();
    }

}

