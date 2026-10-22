namespace DemoTaskApi.Models;

// A person responsible for tasks. A task may belong to one owner (TaskItem.OwnerId).
public class TaskOwner
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Incoming payload for POST /owners.
public record CreateTaskOwnerRequest(string Name, string Email);

// Outgoing shape returned by the owner endpoints — the API's public contract,
// kept separate from the TaskOwner storage model.
public record TaskOwnerDto(int Id, string Name, string Email);
