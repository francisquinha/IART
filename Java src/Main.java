package minmax;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;
import java.util.regex.Matcher;
import java.util.regex.Pattern;


public class Main {
	
	//[start] user interface related
	
	static final String move_input_format_msg = "<SQUARE> <X> <Y> <ROT SQUARE> <ROT DIR>";
	static final String invalid_move_msg = "Invalid move";

	 static String read_from_console() {
	        Scanner scanner = new Scanner(System.in);
	        scanner.useDelimiter("\n");
	        return  scanner.next();
	    }
	
	 static void get_user_move_input(int[] ref_input_var, Pattern user_input_pattern)
	 {
		 Matcher matcher;
		 boolean invalid_input;
		 
		 do{
			invalid_input=false;
			
				matcher = user_input_pattern.matcher(read_from_console());
				for (int i = 0; i < 5; ++i) 
					if ( matcher.find())
					{
						ref_input_var[i] = Integer.parseInt( matcher.group() );
					}
					else { invalid_input=true; break;}
				if(invalid_input) continue;
				
		 }while(invalid_input);
	 }
	 
	 static void  run_game_multiplayer()
	{
		List<Character> user_game_input = new ArrayList<>();
        Pattern user_input_pattern = Pattern.compile("\\d");
        //Matcher matcher;
        int[] player_move_input = new int[5];
        Pentago_Move move = new Pentago_Move(); 
        Pentago_GameBoard game_board = new Pentago_GameBoard();
        
		while (true /*!ended*/)
		{
			game_board.print_board();
			System.out.println(move_input_format_msg);
			get_user_move_input(player_move_input,user_input_pattern);
			
			try {
				move.set_move_from_user_input(player_move_input);
			} catch (Exception e1) {
				System.out.println(invalid_move_msg);
				continue;
			}
			if(!move.is_move_possible(game_board)){
				System.out.println(invalid_move_msg);
				continue;
			}
			
			try {
				move.apply_move2board(game_board);
			} catch (Exception e) {
				e.printStackTrace();
				System.out.println("!INVALID BOARD STATE! (GAME ABORTED)");
				return;
			}
		}
	}
	
	//[end] user interface related
	 
	public static void main(String[] args) throws IOException {

		run_game_multiplayer();

		
		/*Pentago_GameBoard game_board = new Pentago_GameBoard();
		
		while(true)
		{
			game_board.print_board();
			
			
			
		}*/
		
	}

}