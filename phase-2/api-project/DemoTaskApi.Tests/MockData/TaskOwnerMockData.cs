using DemoTaskApi.Models;

namespace DemoTaskApi.Tests.MockData;

/// <summary>
/// Static factory for TaskOwner test data.
///
/// Pattern: every method returns a fully-valid object with realistic defaults,
/// and exposes optional parameters ONLY for the fields a test is likely to vary.
/// A test overrides what it cares about and ignores the rest — keeping the test
/// short and its intent obvious. This is the pattern AI generates well from a DTO.
/// </summary>
public static class TaskOwnerMockData
{
    public static TaskOwner CreateMockOwner(
        int id = 1,
        string name = "Demo Owner",
        string email = "owner@example.com")
    {
        return new TaskOwner
        {
            Id = id,
            Name = name,
            Email = email
        };
    }

    public static TaskOwnerDto CreateMockOwnerDto(
        int id = 1,
        string name = "Demo Owner",
        string email = "owner@example.com")
    {
        return new TaskOwnerDto(id, name, email);
    }

    /// <summary>Builds a collection of distinct, valid owners for list/paging tests.</summary>
    public static List<TaskOwner> CreateMockOwnerSet(int count = 2)
    {
        var owners = new List<TaskOwner>();
        for (int i = 1; i <= count; i++)
        {
            owners.Add(CreateMockOwner(i, $"Owner {i}", $"owner{i}@example.com"));
        }
        return owners;
    }
}
