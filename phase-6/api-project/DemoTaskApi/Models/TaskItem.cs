using System.Text.Json.Serialization;

namespace DemoTaskApi.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskItemStatus
{
    Todo,
    InProgress,
    Done
}

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    // Derived from Status so existing clients reading isComplete keep working.
    public bool IsComplete => Status == TaskItemStatus.Done;

    // Optional link to the owner who is responsible for the task.
    public int? OwnerId { get; set; }
}

// OwnerId is optional so existing callers (POST /tasks with just a title) keep working.
public record CreateTaskRequest(string Title, int? OwnerId = null);

public record UpdateTaskStatusRequest(TaskItemStatus Status);