namespace Common.DTO;

public class StartGameDto
{
    public int PlayerId { get; set; }
    public int Players { get; set; }
    public int Cycles { get; set; }
    public ArchetypeType ArchetypeType { get; set; }
}
