using Common;

namespace Server;

public class GameState
{
    public List<Player> Players { get; set; }
    public int CurrentCycle { get; set; }
    public int CurrentTurnIndex { get; set; }
    public List<int> TurnOrder { get; set; }
    public GamePhase Phase { get; set; }
    public bool IsStarted { get; set; }
    public bool IsFinished { get; set; }
    
    public GameState()
    {
        Players = new List<Player>();
        CurrentCycle = 0;
        CurrentTurnIndex = 0;
        TurnOrder = new List<int>();
        Phase = GamePhase.Waiting;
        IsStarted = false;
        IsFinished = false;
    }
    
    public Player? GetCurrentPlayer()
    {
        if (TurnOrder.Count == 0 || CurrentTurnIndex >= TurnOrder.Count)
            return null;
        
        int playerId = TurnOrder[CurrentTurnIndex];
        return Players.FirstOrDefault(p => p.Id == playerId);
    }
    
    public void ShuffleTurnOrder()
    {
        var random = new Random();
        TurnOrder = Players.Select(p => p.Id).OrderBy(_ => random.Next()).ToList();
        CurrentTurnIndex = 0;
    }
    
    public void NextTurn()
    {
        CurrentTurnIndex++;
        
        if (CurrentTurnIndex >= TurnOrder.Count)
        {
            // Новый цикл
            CurrentCycle++;
            CurrentTurnIndex = 0;
            
            if (CurrentCycle >= GameConfig.TotalCycles)
            {
                IsFinished = true;
                Phase = GamePhase.Finished;
            }
            else
            {
                ShuffleTurnOrder();
            }
        }
    }
}

public enum GamePhase
{
    Waiting,
    ArchetypeSelection,
    Playing,
    Finished
}
