#define V2
//#define FORCE_GC
#define ONLY_SAVE_BELOW_MAXDEPTH
//#define OPEN_ON_START_CLOSE_ON_END_ONLY

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    static class PentagoPandora
    {
#if ONLY_SAVE_BELOW_MAXDEPTH
        const int MAX_FILE_NUM_USED = 15;
        const int MAX_DEPTH_2_SAVE = MAX_FILE_NUM_USED * 2;
# else
        const int MAX_FILE_NUM_USED = 18;
#endif

#if OPEN_ON_START_CLOSE_ON_END_ONLY

        static FileStream[] filestreams;
        static BinaryWriter[] writers;

        static void OpenFiles()
        {
            filestreams = new FileStream[MAX_FILE_NUM_USED];
            writers = new BinaryWriter[MAX_FILE_NUM_USED];

            for (int i = 0; i < MAX_FILE_NUM_USED; i++)
            {
                filestreams[i] = new FileStream("pent_" + i, FileMode.Append, FileAccess.Write, FileShare.None);
                writers[i] = new BinaryWriter(filestreams[i]);
            }
        }

        static void CloseFiles()
        {
            for (int i = 0; i < MAX_FILE_NUM_USED; i++)
            {
                writers[i].Close();
                filestreams[i].Close();
            }
        }

        static int get_board_file_index(Pentago_GameBoard gb)
        {
            Pentago_GameBoard.hole_state[] board = gb.board;
            int numWhites = 0;
            // numBlacks = 0;
            for (int i = 0; i < 36; i++)
            {
                if (board[i] == Pentago_GameBoard.hole_state.is_empty) continue;
                if (board[i] == Pentago_GameBoard.hole_state.has_white) numWhites++;
                //else numBlacks++;
            }

            return numWhites;
        }

        static void appendData2(int file_index, byte id1, ulong id2, short index, byte square2rotate, bool rotDir)
        {
            writers[file_index].Write(id1);
            writers[file_index].Write(id2);
            writers[file_index].Write(index);
            writers[file_index].Write(square2rotate);
            writers[file_index].Write(rotDir);
        }

#endif

        //only possible because we already know there is a winning strategy
        //the ultimate Pentago AI is about to be born !!!

        static Pentago_Rules rules;

        static void get_board_identifier(Pentago_GameBoard gb, out byte id1, out ulong id2)
        {
            Pentago_GameBoard.hole_state[] board = gb.board;
            id1 = 0; id2 = 0;
            for (int i = 0; i < 32; i++)
            {
                if (board[i] == Pentago_GameBoard.hole_state.is_empty) continue;
                id2 += ((ulong)(board[i] == Pentago_GameBoard.hole_state.has_black ? 1 : 2)) << (i * 2);
            }

            for (int i = 0; i < 4; i++)
            {
                if (board[i + 30] == Pentago_GameBoard.hole_state.is_empty) continue;
                id1 += Convert.ToByte((board[i] == Pentago_GameBoard.hole_state.has_black ? 1 : 2) << (i * 2));
            }

        }

        static string get_board_file(Pentago_GameBoard gb)
        {
            Pentago_GameBoard.hole_state[] board = gb.board;
            int numWhites = 0;
            // numBlacks = 0;
            for (int i = 0; i < 36; i++)
            {
                if (board[i] == Pentago_GameBoard.hole_state.is_empty) continue;
                if (board[i] == Pentago_GameBoard.hole_state.has_white) numWhites++;
                //else numBlacks++;
            }

            return "pent_" + numWhites;
        }

        static Pentago_Move get_play(Pentago_GameBoard gb)
        {
            //check if victory is possible within this play


            //check if victory is possible within this play
            byte id1; ulong id2;
            get_board_identifier(gb, out id1, out id2);

            //read stuff from binary file
            using (var fileStream = new FileStream(get_board_file(gb), FileMode.Open, FileAccess.Read, FileShare.None))
            using (var bw = new BinaryReader(fileStream))
            {
                while (bw.BaseStream.Position != bw.BaseStream.Length)
                {
                    byte readID1 = bw.ReadByte();
                    ulong readID2 = bw.ReadUInt64();
                    int readIndex = bw.ReadInt16();
                    int readSqr2Rot = bw.ReadByte();
                    bool readRotD = bw.ReadBoolean();
                    if (readID1 == id1 && readID2 == id2) return new Pentago_Move(readIndex, readSqr2Rot, readRotD);
                }
            }
            return null;
        }

        static void createDataFiles()
        {
            for (int i = 0; i < 18; i++)
            {
                if (File.Exists("pent_" + i))
                {
                    File.Delete("pent_" + i);
                }

                // Create the file.
                using (FileStream fs = File.Create("pent_" + i)) ;
            }
        }

        static void appendData(string filename, byte id1, ulong id2, short index, byte square2rotate, bool rotDir)
        {
            using (var fileStream = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fileStream))
            {
                bw.Write(id1);
                bw.Write(id2);
                bw.Write(index);
                bw.Write(square2rotate);
                bw.Write(rotDir);
            }
        }


        public static void BUILD_PANDORA()
        {
            createDataFiles();
            rules = new Pentago_Rules(Pentago_Rules.EvaluationFunction.func1, Pentago_Rules.NextStatesFunction.all_states);

#if OPEN_ON_START_CLOSE_ON_END_ONLY
            OpenFiles();
#endif

            BUILD_PANDORA_MINIMAX_MAX(new Pentago_GameBoard());

#if OPEN_ON_START_CLOSE_ON_END_ONLY
            CloseFiles();
#endif

        }

#if ONLY_SAVE_BELOW_MAXDEPTH
        static bool BUILD_PANDORA_MINIMAX_MAX(Pentago_GameBoard gb,int depth=0)
#else
       static bool BUILD_PANDORA_MINIMAX_MAX(Pentago_GameBoard gb)
#endif
        {
            bool? winner;
            if (gb.game_ended(out winner))
            {
                if (winner != null && winner == Pentago_GameBoard.whites_turn) return true;
                else return false;
            }

            Pentago_GameBoard auxOriginal = gb.Clone();

            Pentago_Move[] plays1 = rules.possible_plays(gb);
            //check for victory on 1st step
            foreach (Pentago_Move pm in plays1)
            {
                if (pm.state_after_move(auxOriginal).game_ended(out winner))
                {
                    if (winner != null && winner == Pentago_GameBoard.whites_turn) return true;
                }
            }

            //no victory found
            //apply second step for each possible
            Pentago_Move[] plays2 = Pentago_Rules.all_possible_rotate_squares_moves;
            foreach (Pentago_Move pm1 in plays1)
            {
                foreach (Pentago_Move pm2 in plays2)
                    if (
#if V2
#if ONLY_SAVE_BELOW_MAXDEPTH
                        BUILD_PANDORA_MINIMAX_MIN_V2(pm2.state_after_move(pm1.state_after_move(auxOriginal)),depth+1)
#else
        BUILD_PANDORA_MINIMAX_MIN_V2(pm2.state_after_move(pm1.state_after_move(auxOriginal)))
#endif

#else
#if ONLY_SAVE_BELOW_MAXDEPTH
        BUILD_PANDORA_MINIMAX_MIN(pm2.state_after_move(pm1.state_after_move(auxOriginal)),depth+1)
#else
        BUILD_PANDORA_MINIMAX_MIN(pm2.state_after_move(pm1.state_after_move(auxOriginal)))
#endif
                        
#endif

                        )
                    {
#if ONLY_SAVE_BELOW_MAXDEPTH
                        if (depth < MAX_DEPTH_2_SAVE)
                        {
                            byte id1; ulong id2;
                            get_board_identifier(auxOriginal, out id1, out id2);

#if OPEN_ON_START_CLOSE_ON_END_ONLY
                            appendData2(get_board_file_index(auxOriginal), id1, id2, (short)pm1.index, (byte)pm2.square2rotate, pm2.rotDir);
#else
                            appendData(get_board_file(auxOriginal), id1, id2, (short)pm1.index, (byte)pm2.square2rotate, pm2.rotDir);
#endif
                        }
#else
                            byte id1; ulong id2;
                        get_board_identifier(auxOriginal, out id1, out id2);

#if OPEN_ON_START_CLOSE_ON_END_ONLY
                            appendData2(get_board_file_index(auxOriginal), id1, id2, (short)pm1.index, (byte)pm2.square2rotate, pm2.rotDir);
#else
                            appendData(get_board_file(auxOriginal), id1, id2, (short)pm1.index, (byte)pm2.square2rotate, pm2.rotDir);
#endif

#endif

                        return true;
                    }
            }

            return false;
        }

#if ONLY_SAVE_BELOW_MAXDEPTH
        static bool BUILD_PANDORA_MINIMAX_MIN(Pentago_GameBoard gb,int depth)
#else
        static bool BUILD_PANDORA_MINIMAX_MIN(Pentago_GameBoard gb)
#endif
        {
            bool? winner;
            if (gb.game_ended(out winner))
            {
                if (winner != null && winner == Pentago_GameBoard.whites_turn) return true;
                else return false;
            }

            Pentago_GameBoard auxOriginal = gb.Clone();

            Pentago_Move[] plays1 = rules.possible_plays(gb);
            //check for victory on 1st step
            foreach (Pentago_Move pm in plays1)
            {
                if (pm.state_after_move(auxOriginal).game_ended(out winner))
                { 
                    if (winner!=null && winner == Pentago_GameBoard.blacks_turn) return false;
                }
            }

            //no victory found
            //apply second step for each possible
            Pentago_Move[] plays2 = Pentago_Rules.all_possible_rotate_squares_moves;
            foreach (Pentago_Move pm1 in plays1)
            {
                foreach (Pentago_Move pm2 in plays2)
                    if (
#if ONLY_SAVE_BELOW_MAXDEPTH
         !BUILD_PANDORA_MINIMAX_MAX(pm2.state_after_move(pm1.state_after_move(auxOriginal)),depth+1)
#else
        !BUILD_PANDORA_MINIMAX_MAX(pm2.state_after_move(pm1.state_after_move(auxOriginal)))
#endif


                        )
                    {
                        return false;
                    }
            }

            return true;
        }

#if ONLY_SAVE_BELOW_MAXDEPTH
       static bool BUILD_PANDORA_MINIMAX_MIN_V2(Pentago_GameBoard gb,int depth)
#else
       static bool BUILD_PANDORA_MINIMAX_MIN_V2(Pentago_GameBoard gb)
#endif
        {
            bool? winner;
            if (gb.game_ended(out winner))
            {
                if (winner != null && winner == Pentago_GameBoard.whites_turn) return true;
                else return false;
            }

            Pentago_GameBoard auxOriginal = gb.Clone();

            Pentago_Move[] plays1 = rules.possible_plays(gb);
            Pentago_GameBoard[] gbs = new Pentago_GameBoard[plays1.Length * 8];
            //check for victory on 1st step
            for (int i = plays1.Length-1;i>=0;i--)
            {
                gbs[i] = plays1[i].state_after_move(auxOriginal);
                if (gbs[i].game_ended(out winner))
                {
                    if (winner != null && winner == Pentago_GameBoard.blacks_turn) return false;
                }
            }

            //no victory found
            //apply second step for each possible
            for (int i = plays1.Length - 1; i >= 0; i--)
            {
                Pentago_GameBoard[] aux = Pentago_Rules.all_possible_rotate_squares_moves.Select(o=>o.state_after_move(gbs[i])).ToArray();
                for(int a = 0; a<8; a++)
                gbs[i+a* plays1.Length] = aux[a];
            }

            gbs = Pentago_Rules.removeDuplicates(gbs);

            foreach (Pentago_GameBoard brd in gbs)
                if (

#if ONLY_SAVE_BELOW_MAXDEPTH
        !BUILD_PANDORA_MINIMAX_MAX(brd,depth+1)
#else
        !BUILD_PANDORA_MINIMAX_MAX(brd)
#endif


                    )
                {
#if FORCE_GC
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
#endif
                    return false;
                }

            return true;
        }




        public static void test_simple_save_read()
        {
            createDataFiles();

            Pentago_GameBoard gb = new Pentago_GameBoard();

            byte id1; ulong id2;
            get_board_identifier(gb,out id1,out id2);

            appendData(get_board_file(gb), id1, id2, 2,1,false);
            appendData(get_board_file(gb), id1, id2, 2, 1, false);

            Console.WriteLine( get_play(gb).ToString() );

        }

    }

