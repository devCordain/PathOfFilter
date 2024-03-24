using PathOfFilter.Application.Contracts;
using PathOfFilter.Application.Services.Models;

namespace PathOfFilter.Application.Handlers;
public class FilterService
{
    private readonly IStorage _storage;

    public FilterService(IStorage storage)
    {
        _storage = storage;
    }

    public async Task<ItemFilter?> Create(ItemFilter? existingFilter, string? input)
    {
        var command = FilterCommand.TryCreate(input);

        var loadedFilterItems = await _storage.Load(command);

        var baseItems = await _storage.LoadBaseItems();

        if (baseItems == null) return null;

        var newFilter = new ItemFilter(existingFilter, loadedFilterItems, command, baseItems);

        await _storage.Save(existingFilter, newFilter);

        return newFilter;
    }
}
