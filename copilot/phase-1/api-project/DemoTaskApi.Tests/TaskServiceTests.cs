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

    [Theory]
    [InlineData(null)]
    [InlineData(7)]
    public void Complete_ExistingTask_ReturnsCompletedTaskAndPreservesFields(int? ownerId)
    {
        const string title = "  Finish report  ";
        var created = _service.Create(title, ownerId);
        var idBeforeCompletion = created.Id;
        var titleBeforeCompletion = created.Title;
        var ownerIdBeforeCompletion = created.OwnerId;
        ITaskService service = _service;

        var completed = service.Complete(idBeforeCompletion);

        Assert.NotNull(completed);
        Assert.True(completed.IsComplete);
        Assert.Equal(idBeforeCompletion, completed.Id);
        Assert.Equal(titleBeforeCompletion, completed.Title);
        Assert.Equal(ownerIdBeforeCompletion, completed.OwnerId);

        var found = service.GetById(idBeforeCompletion);
        Assert.NotNull(found);
        Assert.True(found.IsComplete);
        Assert.Equal(idBeforeCompletion, found.Id);
        Assert.Equal(titleBeforeCompletion, found.Title);
        Assert.Equal(ownerIdBeforeCompletion, found.OwnerId);
    }

    [Fact]
    public void Complete_AlreadyCompletedTask_ReturnsCompletedTaskWithoutDuplicatingOrConsumingIds()
    {
        var created = _service.Create("  Finish report  ", 7);
        var firstCompletion = _service.Complete(created.Id);
        Assert.NotNull(firstCompletion);

        var idBeforeSecondCompletion = firstCompletion.Id;
        var titleBeforeSecondCompletion = firstCompletion.Title;
        var ownerIdBeforeSecondCompletion = firstCompletion.OwnerId;
        var isCompleteBeforeSecondCompletion = firstCompletion.IsComplete;

        var secondCompletion = _service.Complete(created.Id);

        Assert.NotNull(secondCompletion);
        Assert.True(firstCompletion.IsComplete);
        Assert.Equal(idBeforeSecondCompletion, firstCompletion.Id);
        Assert.Equal(titleBeforeSecondCompletion, firstCompletion.Title);
        Assert.Equal(ownerIdBeforeSecondCompletion, firstCompletion.OwnerId);
        Assert.True(secondCompletion.IsComplete);
        Assert.Equal(idBeforeSecondCompletion, secondCompletion.Id);
        Assert.Equal(titleBeforeSecondCompletion, secondCompletion.Title);
        Assert.Equal(ownerIdBeforeSecondCompletion, secondCompletion.OwnerId);
        Assert.Equal(isCompleteBeforeSecondCompletion, secondCompletion.IsComplete);

        var tasksBeforeNextCreate = _service.GetAll();
        Assert.Single(tasksBeforeNextCreate);
        Assert.Equal(idBeforeSecondCompletion, tasksBeforeNextCreate[0].Id);
        Assert.Equal(titleBeforeSecondCompletion, tasksBeforeNextCreate[0].Title);
        Assert.Equal(ownerIdBeforeSecondCompletion, tasksBeforeNextCreate[0].OwnerId);
        Assert.True(tasksBeforeNextCreate[0].IsComplete);

        var nextTask = _service.Create("Next task");
        Assert.Equal(idBeforeSecondCompletion + 1, nextTask.Id);
        Assert.False(nextTask.IsComplete);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Complete_MissingOrNonpositiveId_ReturnsNullWithoutChangingTasks(int id)
    {
        _service.Create("First task", 7);
        _service.Create("Second task", 8);
        var beforeCompletion = _service.GetAll()
            .Select(task => (task.Id, task.Title, task.OwnerId, task.IsComplete))
            .ToArray();

        var completed = _service.Complete(id);

        Assert.Null(completed);
        Assert.Null(_service.GetById(id));
        var afterCompletion = _service.GetAll()
            .Select(task => (task.Id, task.Title, task.OwnerId, task.IsComplete))
            .ToArray();
        Assert.Equal(beforeCompletion, afterCompletion);

        var nextTask = _service.Create("Next task");
        Assert.Equal(beforeCompletion[^1].Id + 1, nextTask.Id);
    }

    [Fact]
    public void Complete_EmptyStore_ReturnsNullWithoutConsumingId()
    {
        Assert.Null(_service.Complete(999));
        Assert.Empty(_service.GetAll());

        var task = _service.Create("First task");

        Assert.Equal(1, task.Id);
        Assert.False(task.IsComplete);
    }

    [Fact]
    public void Complete_ExistingTask_IsVisibleInAllQueriesWithoutChangingOtherTasks()
    {
        const string targetTitle = "  Finish report  ";
        var target = _service.Create(targetTitle, 7);
        _service.Create("Owner 7 sibling", 7);
        _service.Create("Owner 8 task", 8);
        _service.Create("Unowned task");
        var beforeCompletion = _service.GetAll()
            .Select(task => (task.Id, task.Title, task.OwnerId, task.IsComplete))
            .ToArray();

        var completed = _service.Complete(target.Id);

        Assert.NotNull(completed);
        Assert.True(completed.IsComplete);
        Assert.Equal(target.Id, completed.Id);
        Assert.Equal(targetTitle, completed.Title);
        Assert.Equal(7, completed.OwnerId);

        var byId = _service.GetById(target.Id);
        Assert.NotNull(byId);
        Assert.True(byId.IsComplete);
        Assert.Equal(beforeCompletion[0].Id, byId.Id);
        Assert.Equal(beforeCompletion[0].Title, byId.Title);
        Assert.Equal(beforeCompletion[0].OwnerId, byId.OwnerId);

        var expectedAll = beforeCompletion
            .Select(task => (task.Id, task.Title, task.OwnerId, IsComplete: task.Id == target.Id || task.IsComplete))
            .ToArray();
        var allTasks = _service.GetAll()
            .Select(task => (task.Id, task.Title, task.OwnerId, task.IsComplete))
            .ToArray();
        Assert.Equal(expectedAll, allTasks);

        var ownerSevenTasks = _service.GetByOwner(7)
            .Select(task => (task.Id, task.Title, task.OwnerId, task.IsComplete))
            .ToArray();
        Assert.Equal(expectedAll.Where(task => task.OwnerId == 7).ToArray(), ownerSevenTasks);
        Assert.Equal(2, ownerSevenTasks.Length);
        Assert.Equal(target.Id, ownerSevenTasks[0].Id);
        Assert.True(ownerSevenTasks[0].IsComplete);
        Assert.False(ownerSevenTasks[1].IsComplete);

        var ownerEightTasks = _service.GetByOwner(8)
            .Select(task => (task.Id, task.Title, task.OwnerId, task.IsComplete))
            .ToArray();
        Assert.Equal(expectedAll.Where(task => task.OwnerId == 8).ToArray(), ownerEightTasks);
        Assert.Single(ownerEightTasks);
        Assert.NotEqual(target.Id, ownerEightTasks[0].Id);
        Assert.False(ownerEightTasks[0].IsComplete);
    }
}
