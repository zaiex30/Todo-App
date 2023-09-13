using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.TodoLists.Queries.GetTodos;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.IntegrationTests.TodoLists.Queries;

using static Testing;

public class GetTodosTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnPriorityLevels()
    {
        await RunAsDefaultUserAsync();

        var query = new GetTodosQuery();

        var result = await SendAsync(query);

        result.PriorityLevels.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldReturnAllListsAndItems()
    {
        await RunAsDefaultUserAsync();

        await AddAsync(new TodoList
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            ForDeletion = Status.No,
            Items =
                    {
                        new TodoItem { Title = "Apples", Done = true, ForDeletion = Status.No },
                        new TodoItem { Title = "Milk", Done = true, ForDeletion = Status.No },
                        new TodoItem { Title = "Bread", Done = true, ForDeletion = Status.No },
                        new TodoItem { Title = "Toilet paper", ForDeletion = Status.No },
                        new TodoItem { Title = "Pasta", ForDeletion = Status.No },
                        new TodoItem { Title = "Tissues" , ForDeletion = Status.No},
                        new TodoItem { Title = "Tuna" , ForDeletion = Status.No}
                    }
        });

        var query = new GetTodosQuery();

        var result = await SendAsync(query);

        result.Lists.Should().HaveCount(1);
        result.Lists.First().Items.Should().HaveCount(7);
    }

    [Test]
    public async Task ShouldReturnAllListsAndItemsNotForDeletion()
    {
        await RunAsDefaultUserAsync();

        await AddAsync(new TodoList
        {
            Title = "Buy Cars",
            Colour = Colour.Blue,
            ForDeletion = Status.No,
            Items =
                    {
                        new TodoItem { Title = "Ford", Done = true, ForDeletion = Status.No },
                        new TodoItem { Title = "Toyota", Done = true, ForDeletion = Status.NA },
                        new TodoItem { Title = "Ferrari", Done = true, ForDeletion = Status.Yes },
                        new TodoItem { Title = "BMW", ForDeletion = Status.No },
                        new TodoItem { Title = "Suzuki", ForDeletion = Status.No },
                        new TodoItem { Title = "Nissan" , ForDeletion = Status.Yes},
                        new TodoItem { Title = "KIA" , ForDeletion = Status.Yes}
                    }
        });

        var query = new GetTodosQuery();

        var result = await SendAsync(query);

        result.Lists.Should().HaveCount(1);
        result.Lists.First().Items.Should().HaveCount(3);
    }

    [Test]
    public async Task ShouldAcceptAnonymousUser()
    {
        var query = new GetTodosQuery();

        var action = () => SendAsync(query);

        await action.Should().NotThrowAsync<UnauthorizedAccessException>();
    }
}
