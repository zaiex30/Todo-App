using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoLists.Commands.UpdateTodoList;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.IntegrationTests.TodoLists.Commands;

using static Testing;

public class UpdateTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new UpdateTodoListCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List",
            ForDeletion = Status.No
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "Other List",
            ForDeletion = Status.No
        });

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = "Other List",
            ForDeletion = Status.No
        };

        (await FluentActions.Invoking(() =>
            SendAsync(command))
                .Should().ThrowAsync<ValidationException>().Where(ex => ex.Errors.ContainsKey("Title")))
                .And.Errors["Title"].Should().Contain("The specified title already exists.");
    }

    [Test]
    public async Task ShouldUpdateTodoList()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List",
            ForDeletion = Status.No
        });

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = "Updated List Title",
            ForDeletion = Status.No
        };

        await SendAsync(command);

        var list = await FindAsync<TodoList>(listId);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list!.ForDeletion.Should().Be(Status.No);
        list.LastModifiedBy.Should().NotBeNull();
        list.LastModifiedBy.Should().Be(userId);
        list.LastModified.Should().NotBeNull();
        list.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
