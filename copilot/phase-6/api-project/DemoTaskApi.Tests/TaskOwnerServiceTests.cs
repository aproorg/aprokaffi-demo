using DemoTaskApi.Services;
using DemoTaskApi.Tests.MockData;

namespace DemoTaskApi.Tests;

public class TaskOwnerServiceTests
{
    private readonly TaskOwnerService _service = new();

    [Fact]
    public void Create_WithNameAndEmail_ReturnsOwnerWithId()
    {
        var owner = _service.Create("Alice", "alice@example.com");

        Assert.Equal(1, owner.Id);
        Assert.Equal("Alice", owner.Name);
        Assert.Equal("alice@example.com", owner.Email);
    }

    [Fact]
    public void GetById_NonExistentId_ReturnsNull()
    {
        var found = _service.GetById(999);

        Assert.Null(found);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsOwner()
    {
        var created = _service.Create("Bob", "bob@example.com");

        var found = _service.GetById(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Bob", found.Name);
    }
}

/// <summary>
/// Demonstrates the centralized seeder: one shaped graph, named ids, behaviour
/// that is observable because the data was designed to make it so.
/// </summary>
public class OwnerTaskQueryTests
{
    private readonly TaskService _tasks = new();
    private readonly TaskOwnerService _owners = new();
    private readonly SeedDemoData _seed;

    public OwnerTaskQueryTests()
    {
        _seed = new SeedDemoData(_owners, _tasks);
        _seed.Seed();
    }

    [Fact]
    public void GetByOwner_OwnerWithMixedTasks_ReturnsOnlyThatOwnersTasks()
    {
        var aliceTasks = _tasks.GetByOwner(_seed.AliceId);

        Assert.Equal(2, aliceTasks.Count);
        Assert.Contains(aliceTasks, t => t.Title == "Write report" && t.IsComplete);
        Assert.Contains(aliceTasks, t => t.Title == "Review PR" && !t.IsComplete);
    }

    [Fact]
    public void GetByOwner_OwnerWithNoTasks_ReturnsEmptyList()
    {
        var bobTasks = _tasks.GetByOwner(_seed.BobId);

        Assert.Empty(bobTasks);
    }
}
