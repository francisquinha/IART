using System;

public class Pentago_Move {

	public int index;//can replace above variables for memory optimization
    public int square2rotate;
    public bool rotDir; //clockwise = false; anticlockwise = true
	
	public const bool rotate_clockwise = false;
	public const bool rotate_anticlockwise = true;


    /*public Pentago_Move(){}

    public Pentago_Move(int s2p, int x, int y, int s2r, bool rot_dir)
	{
		index = Pentago_GameBoard.board_position_to_index(x,y, s2p);
		square2rotate=s2r; 
		rotDir=rot_dir;
	}*/

    public Pentago_Move(int s2p, int x, int y)
    {
        index = Pentago_GameBoard.board_position_to_index(x, y, s2p);
    }

    public Pentago_Move(int s2r, bool rot_dir)
    {
        square2rotate = s2r;
        rotDir = rot_dir;
    }

    public Pentago_Move(int ind, int s2r, bool rot_dir)
    {
        index = ind;
        square2rotate = s2r;
        rotDir = rot_dir;
    }

    void set_move_from_user_input(int[] move) 
	{
		index = Pentago_GameBoard.board_position_to_index(move[1], move[2], move[0]);
		square2rotate=move[3]; 
		rotDir=move[4]!=0;
	}
	
	public bool is_move_possible(Pentago_GameBoard gb)
	{
 
        if (gb.get_turn_state() == Pentago_GameBoard.turn_state_addpiece && gb.board[index] == Pentago_GameBoard.hole_state.is_empty)
            return true;

        if (gb.get_turn_state() == Pentago_GameBoard.turn_state_rotate && square2rotate >= 0 && square2rotate < 4)
            return true;

            return false;
	}

    //below methods should only be called after is_move_possible is verified
    /// <summary>
    /// DEPRECATED !!! USE move with 3 args!!! (can still be used fr debug and what not)
    /// </summary>
    /// <returns></returns>
   /* public Pentago_GameBoard.hole_state[] move(Pentago_GameBoard.hole_state[] gb,bool turn) 
	{
		gb[index]//Pentago_GameBoard.board_postion_to_index(hole2place_x, hole2place_y,square2place)] 
		= turn == Pentago_GameBoard.blacks_turn? Pentago_GameBoard.hole_state.has_black: Pentago_GameBoard.hole_state.has_white;
		
		Pentago_GameBoard.hole_state[] new_gb = (Pentago_GameBoard.hole_state[]) gb.Clone();
		
		if(rotDir == rotate_clockwise) 
			for(int x = 0 ; x <3 ; ++x) for(int y = 0 ; y <3 ; ++y)
			new_gb[Pentago_GameBoard.board_position_to_index(Math.Abs(2-y), x,square2rotate)]
				=gb[Pentago_GameBoard.board_position_to_index(x,y ,square2rotate)];
		else 
			for(int x = 0 ; x <3 ; ++x) for(int y = 0 ; y <3 ; ++y)
				new_gb[Pentago_GameBoard.board_position_to_index(y, Math.Abs(2-x),square2rotate)]
				=gb[Pentago_GameBoard.board_position_to_index(x,y ,square2rotate)];
		
		return new_gb;
	}*/

    /// <summary>
    /// Applies the play to the gameboard. Use apply_move2board instead unless you want to reutilize the Pentago_Move instance (usefull in aux methods to process stuff faster)
    /// </summary>
    /// <param name="gb"></param>
    /// <param name="turn">when turn state = placing a piece, tells what type piece will be placed</param>
    /// <param name="turnstate">whether player is placing a piece or rotating a square</param>
    /// <returns></returns>
    public Pentago_GameBoard.hole_state[] move(Pentago_GameBoard.hole_state[] gb, bool turn,bool turnstate)
    {
        Pentago_GameBoard.hole_state[] new_gb = (Pentago_GameBoard.hole_state[])gb.Clone();

        if (turnstate == Pentago_GameBoard.turn_state_addpiece)
        {
            new_gb[index]//Pentago_GameBoard.board_postion_to_index(hole2place_x, hole2place_y,square2place)] 
            = turn == Pentago_GameBoard.blacks_turn ? Pentago_GameBoard.hole_state.has_black : Pentago_GameBoard.hole_state.has_white;
        }
        else {

            if (rotDir == rotate_clockwise)
                for (int x = 0; x < 3; ++x) for (int y = 0; y < 3; ++y)
                        new_gb[Pentago_GameBoard.board_position_to_index(Math.Abs(2 - y), x, square2rotate)]
                            = gb[Pentago_GameBoard.board_position_to_index(x, y, square2rotate)];
            else
                for (int x = 0; x < 3; ++x) for (int y = 0; y < 3; ++y)
                        new_gb[Pentago_GameBoard.board_position_to_index(y, Math.Abs(2 - x), square2rotate)]
                        = gb[Pentago_GameBoard.board_position_to_index(x, y, square2rotate)];
        }

        return new_gb;
    }

    public Pentago_GameBoard state_after_move(Pentago_GameBoard gb) 
	{
		return new Pentago_GameBoard( this.move(gb.board,gb.get_player_turn(),gb.get_turn_state()) ,
            gb.get_turn_state()==Pentago_GameBoard.turn_state_rotate? !gb.get_player_turn()  : gb.get_player_turn()
            , !gb.get_turn_state());
	}
	
	public void apply_move2board(Pentago_GameBoard gb) 
	{
        gb.board = this.move(gb.board, gb.get_player_turn(), gb.get_turn_state());
        if(gb.get_turn_state()==Pentago_GameBoard.turn_state_rotate) gb.switch_player_turn();
        gb.switch_turn_state();
    }


    #region USEFULL HACKS

    /// <summary>
    /// applies 2 steps immiditaly, not made for standard minimax
    /// </summary>
    /// <param name="gb"></param>
    /// <returns></returns>
    public Pentago_GameBoard.hole_state[] move2steped(Pentago_GameBoard.hole_state[] gb, bool turn, bool turnstate)
    {
        Pentago_GameBoard.hole_state[] new_gb = (Pentago_GameBoard.hole_state[])gb.Clone();

            new_gb[index]//Pentago_GameBoard.board_postion_to_index(hole2place_x, hole2place_y,square2place)] 
            = turn == Pentago_GameBoard.blacks_turn ? Pentago_GameBoard.hole_state.has_black : Pentago_GameBoard.hole_state.has_white;

            if (rotDir == rotate_clockwise)
                for (int x = 0; x < 3; ++x) for (int y = 0; y < 3; ++y)
                        new_gb[Pentago_GameBoard.board_position_to_index(Math.Abs(2 - y), x, square2rotate)]
                            = gb[Pentago_GameBoard.board_position_to_index(x, y, square2rotate)];
            else
                for (int x = 0; x < 3; ++x) for (int y = 0; y < 3; ++y)
                        new_gb[Pentago_GameBoard.board_position_to_index(y, Math.Abs(2 - x), square2rotate)]
                        = gb[Pentago_GameBoard.board_position_to_index(x, y, square2rotate)];

        return new_gb;
    }

    /// <summary>
    /// applies 2 steps immiditaly, not made for standard minimax
    /// </summary>
    /// <param name="gb"></param>
    /// <returns></returns>
    public Pentago_GameBoard state_after_move_2steped(Pentago_GameBoard gb)
    {
        return new Pentago_GameBoard(this.move2steped(gb.board, gb.get_player_turn(), gb.get_turn_state()),
            !gb.get_player_turn() 
            , Pentago_GameBoard.turn_state_addpiece);
    }

    #endregion

    public override string ToString()
    {
        int x, y, sqr;
        Pentago_GameBoard.board_index_to_position(index,out x,out y,out sqr);

        return "place(" + x + "," + y + "," + sqr + ")" +
            "  rot(" + square2rotate + ","
            + (rotDir==rotate_anticlockwise? "anticlock":"clock")
            + "," + ")";
    }

}
