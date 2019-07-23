#define DEBUG_MINIMAX

//GAME_BOARD deve contar 
//tabuleiro ; peças ; qual o jogador a jogar ; auxiliares ao display do jogo (ex: dizer qual a peça a mover-se se for preciso animar no display)

/// <summary>
/// Interface that defines all the methods that must be implemented in order to use Minimaz Algorithm
/// </summary>
/// <typeparam name="GAME_BOARD">Class with the representation of the game's board</typeparam>
/// <typeparam name="GAME_MOVE_DESCRIPTION">Class with that is used to represent the game's player allowed moves/plays</typeparam>
public interface IGameRules<GAME_BOARD,GAME_MOVE_DESCRIPTION> { 
	
	GAME_MOVE_DESCRIPTION[] possible_plays(GAME_BOARD gb, int depth = 0);
	
	GAME_BOARD board_after_play(GAME_BOARD gb, GAME_MOVE_DESCRIPTION gmd);

	GAME_BOARD[] next_states(GAME_BOARD gb,int depth=0);

    /// <summary>
    /// checks if the game has ended.
    /// <para>returns null if game did not end or the value of utility if game ended</para>para>
    /// </summary>
    /// <param name="gb"></param>
    /// <param name="depth">optional usage, only usefull to calculate utility</param>
    /// <returns>null if game did not end or the value of utility if game ended</returns>
    float? game_over(GAME_BOARD gb,int depth);

    /// <summary>
    /// method to be used when minimax achieves max depth
    /// <para>may have a heuristics 'selector' in the implementation </para>
    /// <para>may return Float.POSITIVE_INFINITY or Float.NEGATIVE_INFINITY to force prioritization of certain plays</para>
    /// </summary>
    /// <param name="gb"></param>
    /// <returns></returns>
    float evaluate(GAME_BOARD gb);

    /// <summary>
    /// Should select the next node type to use in minimax. 
    /// <para>Default behaviour for most games should be return !currentIterationNode.</para>
    /// <para>Some Games, like pentago, need some additional consierations due to 'half step/play' endgames</para>
    /// </summary>
    /// <param name="gb"></param>
    /// <param name="currentIterationNode"></param>
    /// <returns></returns>
    bool selectMINMAX(GAME_BOARD thisnode_gb, bool currentIterationNode);

    /// <summary>
    /// decide what depth to use on minimax depending on the game board
    /// </summary>
    /// <param name="gb"></param>
    /// <returns></returns>
    int smart_depth(GAME_BOARD gb);

    /// <summary>
    /// get information about 'Rules' to print/display
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    string toDisplayString();
}

public partial class MinMax <GAME_BOARD,GAME_MOVE_DESCRIPTION>{

    public enum VERSION { minmax , alphabeta,cut_alphabeta,multithread_alphabeta };
    public VERSION version;
    public IGameRules<GAME_BOARD, GAME_MOVE_DESCRIPTION> rules;
    const bool MAX_NODE = true;
    const bool MIN_NODE = false;

    const int USE_SMART_DEPTH = 0;
    bool useSmartDepth;
    public int max_depth;
    
    public MinMax(VERSION version, IGameRules<GAME_BOARD, GAME_MOVE_DESCRIPTION> rules, int max_depth = 0)
    {
        this.version = version;
        this.rules = rules;
        this.max_depth = max_depth;
        useSmartDepth = max_depth == USE_SMART_DEPTH;
    }

    public GAME_MOVE_DESCRIPTION[] run(GAME_BOARD gb)
    {
        //System.Threading.Thread.CurrentThread.Priority = (System.Threading.ThreadPriority)4;
        if(useSmartDepth) max_depth = rules.smart_depth(gb);
        switch (version)
        {
            case VERSION.minmax:
                return minmax_init(gb);
            case VERSION.alphabeta:
                return alpha_beta_minmax_init(gb);
            case VERSION.cut_alphabeta:
                return CUT_alpha_beta_minmax_init(gb);
            case VERSION.multithread_alphabeta:
                return alpha_beta_minmax_initMT(gb);
            default:
                return null;
        }
    }

    public void printConfigs()
    {
        System.Console.WriteLine("Version: " + version.ToString());
        if (useSmartDepth) System.Console.WriteLine("Depth: Using Smart Depth");
        else System.Console.WriteLine("Depth: "+ max_depth);
        System.Console.WriteLine(rules.toDisplayString());
    }


 }
