#define DEBUG_MINIMAX

//GAME_BOARD deve contar 
//tabuleiro ; peças ; qual o jogador a jogar ; auxiliares ao display do jogo (ex: dizer qual a peça a mover-se se for preciso animar no display)

/// <summary>
/// Interface that defines all the methods that must be implemented in order to use Minimaz Algorithm
/// </summary>
/// <typeparam name="GAME_BOARD">Class with the representation of the game's board</typeparam>
/// <typeparam name="GAME_MOVE_DESCRIPTION">Class with that is used to represent the game's player allowed moves/plays</typeparam>
interface IGameRules<GAME_BOARD,GAME_MOVE_DESCRIPTION>{
	
	GAME_MOVE_DESCRIPTION[] possible_plays(GAME_BOARD gb);
	
	GAME_BOARD board_after_play(GAME_BOARD gb, GAME_MOVE_DESCRIPTION gmd);

	GAME_BOARD[] next_states(GAME_BOARD gb);

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
}

public partial class MinMax <GAME_BOARD,GAME_MOVE_DESCRIPTION>{

    enum VERSION { minimax , alfabeta };
    IGameRules<GAME_BOARD, GAME_MOVE_DESCRIPTION> rules;
    const bool MAX_NODE = true;
    const bool MIN_NODE = false;
    
    void a()
    {
    }

    //TODO

    //max(...
    //{...  rules.selectMINMAX}
    //min(...
    //{...  rules.selectMINMAX}
}
