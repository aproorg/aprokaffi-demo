namespace DemoTaskApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    // Optional link to the owner who is responsible for the task.
    public int? OwnerId { get; set; }
}

// OwnerId is optional so existing callers (POST /tasks with just a title) keep working.
public record CreateTaskRequest(string Title, int? OwnerId = null);
