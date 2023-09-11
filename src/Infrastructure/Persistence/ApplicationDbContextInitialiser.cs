using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;
using Todo_App.Infrastructure.Identity;

namespace Todo_App.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole("Administrator");

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        }

        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                ForDeletion = Status.No,
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃", ForDeletion = Status.No },
                    new TodoItem { Title = "Check off the first item ✅", ForDeletion = Status.No },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯", ForDeletion = Status.Yes },
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆", ForDeletion = Status.No },
                }
            });

            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List Two",
                ForDeletion = Status.Yes,
                Items =
                {
                    new TodoItem { Title = "Get groceries", ForDeletion = Status.Yes },
                    new TodoItem { Title = "Buy new furnitures", ForDeletion = Status.No },
                    new TodoItem { Title = "Feed the baby", ForDeletion = Status.Yes },
                    new TodoItem { Title = "Go to sleep", ForDeletion = Status.NA },
                }
            });

            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List Three",
                ForDeletion = Status.No,
                Items =
                {
                    new TodoItem { Title = "Make a new hit single", ForDeletion = Status.No },
                    new TodoItem { Title = "Interview Taylor Swift", ForDeletion = Status.No },
                    new TodoItem { Title = "Eat some grilled cheese sandwich", ForDeletion = Status.Yes },
                    new TodoItem { Title = "Sing Let it Go", ForDeletion = Status.NA },
                }
            });

            await _context.SaveChangesAsync();
        }
    }
}
