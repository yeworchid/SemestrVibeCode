namespace Common.DTO;

public class AttackTargetDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int Losses { get; set; }
    public int Survivors { get; set; }
    public Dictionary<string, int> StolenResources { get; set; } = new();
}
