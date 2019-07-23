
//GAME_BOARD deve contar 
//tabuleiro ; peças ; qual o jogador a jogar ; auxiliares ao display do jogo (ex: dizer qual a peça a mover-se se for preciso animar no display)

interface IGameRules<GAME_BOARD,GAME_MOVE_DESCRIPTION>{
	
	//return false p/ usar game_over
	//return true p/ usar game_over_evaluate
	//boolean evaluate_on_gameover();
	
	//retorna as jogadas possiveis
	GAME_MOVE_DESCRIPTION[] possible_plays(GAME_BOARD gb);
	
	//retorna o board resultante após a realização de 1 jogada
	GAME_BOARD board_after_play(GAME_BOARD gb, GAME_MOVE_DESCRIPTION gmd);
	
	//retorna os estados possiveis seguintes
	GAME_BOARD[] next_states(GAME_BOARD gb);
	
	//verifica se estamos num estado final do jogo
	bool game_over(GAME_BOARD gb);
	
	//verifica se estamos num estado final do jogo e avalia
	//boolean game_over_evaluate(GAME_BOARD gb,Integer evaluation);
	
	//a ser usado pelo minimax quando atingida a profundidade maxima
	//vitoria deve retornar Float.POSITIVE_INFINITY
	//derrota deve retornar Float.NEGATIVE_INFINITY
    float evaluate(GAME_BOARD gb);
}

public class MinMax <GAME_BOARD,GAME_MOVE_DESCRIPTION>{

	static bool DEBUG;
	/*
	
	GAME_BOARD initial_state;
	private GameRules<GAME_BOARD,GAME_MOVE_DESCRIPTION> game_rules;
    int max_depth;
    
    
    public MinMax(minmax_solver_type type, int maxDepth) { max_depth = maxDepth; max_depth = maxDepth;}
   
   //alpha = melhor para max (enc. no caminho ate raiz)
   //beta = melhor para min (enc. no caminho ate raiz)
     
     public Alpha_Beta_Prunning(){
     	Alpha_Beta_Prunning(Float.NEGATIVE_INFINITY,Float.POSITIVE_INFINITY,max_depth,true);
     }
     
	 private int Alpha_Beta_Prunning_Aux(int alpha, int beta, int depth, boolean ai_turn){
	        
	    }  
	*/
}
