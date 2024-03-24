using PathOfFilter.Application.Services.Models;

namespace PathOfFilter.Application.Contracts;

public interface IStorage
{
    Task Save(ItemFilter? existingFilter, ItemFilter newFilter);
    Task<List<FilterItem>?> Load(FilterCommand? command);
    Task<List<FilterItem>?> LoadBaseItems();
}