
public class AI_thread : ThreadedJob
{
    MinMax<Pentago_GameBoard, Pentago_Move> ia; //ia must already been initialized
    public Pentago_Move[] moves;
    public Pentago_GameBoard gameboard;
    public bool concluded=false;

    public AI_thread(MinMax<Pentago_GameBoard, Pentago_Move> ia)
    { this.ia = ia; }

    protected override void ThreadFunction()
    {
        concluded = false;
        //UnityEngine.Debug.Log("STARTED");
       // UnityEngine.Debug.Log("!!" + gameboard.board[28]);
        moves = null;
        moves = ia.run(gameboard);

       // UnityEngine.Debug.Log("CONCLUDED");
    }
    protected override void OnFinished()
    {
        // This is executed by the Unity main thread when the job is finished
        concluded = true;
       // UnityEngine.Debug.Log("CONCLUDED2");
    }
}