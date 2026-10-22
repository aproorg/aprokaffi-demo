using DemoTaskApi.Services;

namespace DemoTaskApi.Tests.MockData;

/// <summary>
/// Centralized seeder for a connected owner/task graph.
///
/// Pattern (scaled down from a real relational seeder): build ONE coherent set of
/// related data, expose the ids you'll assert against as named properties, and
/// shape the data deliberately so behaviour is observable:
///
///   Alice → 2 tasks: 1 complete, 1 incomplete   (mixed — proves filtering works)
///   Bob   → 0 tasks                              (empty — the "owner with no tasks" case)
///
/// Tests reference <see cref="AliceId"/> / <see cref="BobId"/> instead of magic
/// numbers, so an assertion reads as intent. The human designs the SHAPE
/// (who owns what, which task is complete); that is the part AI should not guess.
/// </summary>
public class SeedDemoData
{
    private readonly ITaskOwnerService _owners;
    private readonly ITaskService _tasks;

    public int AliceId { get; private set; }
    public int BobId { get; private set; }

    public SeedDemoData(ITaskOwnerService owners, ITaskService tasks)
    {
        _owners = owners;
        _tasks = tasks;
    }

    public void Seed()
    {
        var alice = _owners.Create("Alice", "alice@example.com");
        var bob = _owners.Create("Bob", "bob@example.com");
        AliceId = alice.Id;
        BobId = bob.Id;

        // Alice: one completed task and one still open.
        var report = _tasks.Create("Write report", alice.Id);
        report.IsComplete = true;
        _tasks.Create("Review PR", alice.Id);

        // Bob intentionally owns no tasks.
    }
}
