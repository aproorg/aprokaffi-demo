using DemoTaskApi.Services;

namespace DemoTaskApi.Tests;

public class TaskServiceTests
{
    private readonly TaskService _service = new();

    [Fact]
    public void Create_ReturnsTaskWithId()
    {
        var task = _service.Create("Test task");

        Assert.Equal(1, task.Id);
        Assert.Equal("Test task", task.Title);
        Assert.False(task.IsComplete);
    }

    [Fact]
    public void GetAll_ReturnsAllCreatedTasks()
    {
        _service.Create("Task 1");
        _service.Create("Task 2");

        var tasks = _service.GetAll();

        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsTask()
    {
        var created = _service.Create("Find me");

        var found = _service.GetById(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Find me", found.Title);
    }

    [Fact]
    public void GetById_NonExistentId_ReturnsNull()
    {
        var found = _service.GetById(999);

        Assert.Null(found);
    }
}
