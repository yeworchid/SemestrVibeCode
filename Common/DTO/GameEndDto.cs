namespace Common.DTO;

public class GameEndDto
{
    public List<PlayerResult> Results { get; set; } = new();
}

public class PlayerResult
{
    public int PlayerId { get; set; }
    public string Nickname { get; set; } = "";
    public int Points { get; set; }
}
