namespace Application;

public record CreateCommand(string Name, string Address, string Country, string BaseCode) : Command;