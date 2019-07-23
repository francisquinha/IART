package minmax;

public class Pentago_Move {
	//there is only one type of play possible in pentago.
	//(player [0,1] ; square2place [0,3], hole2place x,y[0,2], square2rotate[0,3], rotDir [0,1]
	
	//boolean player; //whites = false; blacks = true
	//private int square2place;
	//int hole2place_x, hole2place_y;
	int index;//can replace above variables for memory optimization
	int square2rotate;
	boolean rotDir; //clockwise = false; anticlockwise = true
	
	public static final boolean rotate_clockwise = false;
	public static final boolean rotate_anticlockwise = true;
	//public static final boolean whites_turn = false;
	//public static final boolean blacks_turn = true;
	
	Pentago_Move(){};
	
	Pentago_Move(int s2p, int x, int y, int s2r, boolean rot_dir) throws Exception
	{
		//square2place=s2p; 
		//hole2place_x=x;		hole2place_y=y;
		index = Pentago_GameBoard.board_postion_to_index(x,y, s2p);
		square2rotate=s2r; 
		rotDir=rot_dir;
	}
	
	void set_move_from_user_input(int[] move) throws Exception
	{
		//square2place=move[0]; 
		//hole2place_x=move[1];		hole2place_y=move[2];
		index = Pentago_GameBoard.board_postion_to_index(move[1], move[2], move[0]);
		square2rotate=move[3]; 
		rotDir=move[4]!=0;
	}
	
	public boolean is_move_possible(Pentago_GameBoard gb)
	{
		/*int index;
		
		try {
			index = Pentago_GameBoard.board_postion_to_index(hole2place_x, hole2place_y,square2place);
		} catch (Exception e) {
			e.printStackTrace();
			return false;
		}*/
		
		if(gb.board[index] == Pentago_GameBoard.is_empty
				&& square2rotate>=0 && square2rotate<4) 
			return true;
		
		return false;
	}
	
	//below methods should only be called after is_move_possible is verified
	
	private int[] move(int[] gb,boolean turn) throws Exception
	{
		gb[index]//Pentago_GameBoard.board_postion_to_index(hole2place_x, hole2place_y,square2place)] 
		= turn == Pentago_GameBoard.blacks_turn? Pentago_GameBoard.has_black: Pentago_GameBoard.has_white;
		
		int[] new_gb = gb.clone();
		
		if(rotDir == rotate_clockwise) 
			for(int x = 0 ; x <3 ; ++x) for(int y = 0 ; y <3 ; ++y)
			new_gb[Pentago_GameBoard.board_postion_to_index(Math.abs(2-y), x,square2rotate)]
				=gb[Pentago_GameBoard.board_postion_to_index(x,y ,square2rotate)];
		else 
			for(int x = 0 ; x <3 ; ++x) for(int y = 0 ; y <3 ; ++y)
				new_gb[Pentago_GameBoard.board_postion_to_index(y, Math.abs(2-x),square2rotate)]
				=gb[Pentago_GameBoard.board_postion_to_index(x,y ,square2rotate)];
		
		return new_gb;
	}
	
	public Pentago_GameBoard state_after_move(Pentago_GameBoard gb) throws Exception
	{
		return new Pentago_GameBoard( this.move(gb.board,gb.get_player_turn()) , !gb.get_player_turn());
	}
	
	public void apply_move2board(Pentago_GameBoard gb) throws Exception
	{
		gb.board = this.move(gb.board,gb.get_player_turn());
		gb.switch_player_turn();
	}
}
