namespace Lottery.Contracts;

public record PlayerDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsCPU { get; set; }
    public decimal Balance { get; set; }
}
