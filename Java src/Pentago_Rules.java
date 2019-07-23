package minmax;

public class Pentago_Rules implements GameRules<Pentago_GameBoard,int[]> {
	
/*see board on Pentago_GameBoard.java
 * 
 * */
	
	//Moves -------------------------------------------------------------------------------
	
	//there is only one type of move
	public int[] move(){
		return null;
	}
	
	//Interface Related -------------------------------------------------------------------
	
	public int[][] possible_plays(Pentago_GameBoard gb)
	{
		int[][] plays = null;
		return plays;
	}
		
	public Pentago_GameBoard board_after_play(Pentago_GameBoard gmd)
	{
		return gmd;
	}
		
	public boolean game_over(Pentago_GameBoard gb)
	{
		return false;
	}
	
	public float evaluate(Pentago_GameBoard gb)
	{
		
		return 1.0f;
	}

	@Override
	public Pentago_GameBoard board_after_play(int[] gmd) {
		// TODO Auto-generated method stub
		return null;
	}
	
}
