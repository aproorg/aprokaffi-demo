using DemoTaskApi.Models;

namespace DemoTaskApi.Services;

public class TaskOwnerService : ITaskOwnerService
{
    private readonly List<TaskOwner> _owners = [];
    private int _nextId = 1;

    public List<TaskOwner> GetAll()
    {
        return _owners.ToList();
    }

    public TaskOwner? GetById(int id)
    {
        return _owners.FirstOrDefault(o => o.Id == id);
    }

    public TaskOwner Create(string name, string email)
    {
        var owner = new TaskOwner
        {
            Id = _nextId++,
            Name = name,
            Email = email
        };
        _owners.Add(owner);
        return owner;
    }
}
