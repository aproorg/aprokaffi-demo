using DemoTaskApi.Models;

namespace DemoTaskApi.Services;

public class TaskService : ITaskService
{
    private readonly List<TaskItem> _tasks = [];
    private int _nextId = 1;

    public List<TaskItem> GetAll()
    {
        return _tasks.ToList();
    }

    public TaskItem? GetById(int id)
    {
        return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public TaskItem Create(string title, int? ownerId = null)
    {
        var task = new TaskItem
        {
            Id = _nextId++,
            Title = title,
            Status = TaskItemStatus.Todo,
            OwnerId = ownerId
        };
        _tasks.Add(task);
        return task;
    }

    public TaskItem? UpdateStatus(int id, TaskItemStatus status)
    {
        var task = GetById(id);
        if (task is not null)
        {
            task.Status = status;
        }
        return task;
    }

    public List<TaskItem> GetByOwner(int ownerId)
    {
        return _tasks.Where(t => t.OwnerId == ownerId).ToList();
    }
}
