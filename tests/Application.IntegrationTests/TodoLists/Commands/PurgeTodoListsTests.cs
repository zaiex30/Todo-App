using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Security;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoLists.Commands.PurgeTodoLists;
using Todo_App.Application.TodoLists.Commands.UpdateTodoList;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.IntegrationTests.TodoLists.Commands;

using static Testing;

public class PurgeTodoListsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var command = new PurgeTodoListsCommand();

        command.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(command);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldDenyNonAdministrator()
    {
        await RunAsDefaultUserAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => SendAsync(command);

        await action.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public async Task ShouldAllowAdministrator()
    {
        await RunAsAdministratorAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => SendAsync(command);

        await action.Should().NotThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public async Task ShouldDeleteAllLists()
    {
        await RunAsAdministratorAsync();

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #1",
            ForDeletion = Status.NA
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #2",
            ForDeletion = Status.NA
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #3",
            ForDeletion = Status.NA
        });

        await SendAsync(new PurgeTodoListsCommand());

        var count = await CountAsync<TodoList>();

        count.Should().Be(0);
    }

    [Test]
    public async Task ShouldSoftDeleteAllLists()
    {
        await RunAsAdministratorAsync();

        List<string> titles = new List<string>
        {
            "New List #1", "New List #2", "New List #3"
        };

        List<int> listIds = new List<int>
        {
            await SendAsync(new CreateTodoListCommand
            {
                Title = titles[0],
                ForDeletion = Status.No
            }),

            await SendAsync(new CreateTodoListCommand
            {
                Title = titles[1],
                ForDeletion = Status.No
            }),

            await SendAsync(new CreateTodoListCommand
            {
                Title = titles[2],
                ForDeletion = Status.No
            })
        };

        listIds.ForEach(async (i) =>
        {
            await SendAsync(new UpdateTodoListCommand
            {
                Id = listIds[i],
                Title = titles[i],
                ForDeletion = Status.Yes
            });
        });

        var count = await CountAsync<TodoList>();

        count.Should().Be(3);
    }
}
