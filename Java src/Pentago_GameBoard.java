package minmax;

public class Pentago_GameBoard {

	/* indexes in the game board
	 * 0  1  2 | 3  4  5
	 * 6  7  8 | 9  10 11
	 * 12 13 14| 15 16 17
	 * -------------------
	 * 18 19 20| 21 22 23
	 * 24 25 26| 27 28 29
	 * 30 31 32| 33 34 35 
	 * */
	
	public int[] board; /*faster 2 read if needed*/
	private boolean player_turn;
	boolean get_player_turn(){return player_turn;}/*avoids checking board*/
	void switch_player_turn(){player_turn = !player_turn;}
	
	public static final int is_empty = 0;
	public static final int has_white = 1;
	public static final int has_black = -1;
	public static final boolean whites_turn = false;
	public static final boolean blacks_turn = true;
	
	public enum game_state{ongoig,draw,whites_won,blacks_won };
	
	Pentago_GameBoard()
	{
		board = new int[36];
		for(int i = board.length-1; i>=0; --i) board[i] = 0;
		player_turn = whites_turn;
	}
	
	Pentago_GameBoard(int[] board, boolean turn)
	{
		this.board = board;//may need 2 clone here ???
		player_turn = turn;
	}
	
static int board_postion_to_index(int x, int y , int square) throws Exception{
	
	//Pre contitions -------------------------------------------------
	if (x<0 | x>2) throw new Exception("Invalid x");
	if (y<0 | y>2) throw new Exception("Invalid y");
	if (square<0 | square>3) throw new Exception("Invalid Square");
	
	//Method body ----------------------------------------------------
	int index=0;
	
	switch (square) {
	case 0:
		break;
	case 1:		index=3;	
		break;
	case 2:		index=18;	
		break;
	case 3:		index=21;	
		break;
	default:
		break;
	}
	
	return index + x + y*6;
}

void print_board(){
	
	System.out.println(" --------------- "); 
	int index=0;
	for(int i = 0; i<12;++i )
	{
		System.out.print("| "); 
		while(index<i*3+3) 
		{
			switch(board[index]){
			case is_empty:System.out.print("0 ") ;break;
			case has_white:System.out.print("W ") ;break;
			case has_black:System.out.print("B ") ;break;
			default: System.out.print("X ") ;break;
			}
			index++;
		}
		if(i % 2 == 1) System.out.println("| "); 
		if(i==5) System.out.println("|---------------|"); 
	}
	System.out.println(" --------------- "); 
}

//check if game has ended, and if there is a winner
//returns true if ended
//player = null if draw or game did not end
boolean game_ended(Boolean player)
{
	boolean board_full=true;
	boolean white_made_a_line=false;
	boolean black_made_a_line=false;
	int sequence_size;//counts positive 4 white and negative 4 black to avoid using another var
	
	//could also check horizontals and verticals in one iteration using more memory, not sure what is best
	
	//check horizontals and if board is full
	for(int line = 0; line<6 ; ++line)
	{
		if(board_full) if(board[line*6]==0) board_full = false;
		sequence_size = board[line*6];

		for(int row = 1; row<6 ; ++row)
		{
			int hole = board[line*6+row];
					
			if(board[line*6+row]==is_empty) 
				{
					board_full=false;
					if(row!=5) break;/*speed up, can't be any line there*/
				}

			if(hole == has_white && sequence_size<=0) {
				++sequence_size;
				if(sequence_size == 5) { white_made_a_line=true; break;}
			}
			else if(hole == has_black && sequence_size>=0) {
				--sequence_size;
				if(sequence_size == -5) { black_made_a_line=true; break;}
			}
			else sequence_size=0;
		}
	}

	//check verticals
	for(int row = 0; row<6 ; ++row)
	{
		if(board_full) if(board[row]==0) board_full = false;
		sequence_size = board[row];

		for(int line = 1; line<6 ; ++line)
		{
			int hole = board[line*6+row];
					
			if(board[line*6+row]==is_empty) 
				{
					board_full=false;
					if(line!=5) break;/*speed up, can't be any line there*/
				}

			if(hole == has_white && sequence_size<=0) {
				++sequence_size;
				if(sequence_size == 5) { white_made_a_line=true; break;}
			}
			else if(hole == has_black && sequence_size>=0) {
				--sequence_size;
				if(sequence_size == -5) { black_made_a_line=true; break;}
			}
			else sequence_size=0;
		}
	}
	
	//check diagonals
	//todo
	
	
	return board_full;
}

}