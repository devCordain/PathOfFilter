using PathOfFilter.Application.Handlers;

namespace PathOfFilter.Test;

public class FilterHandlerTests
{
    private readonly FilterService _sut;
    public FilterHandlerTests()
    {
        _sut = new FilterService(new LocalJsonStorageService());
    }


    [Fact]
    public async void Starting_New_Filter()
    {
        var filter = await _sut.Create(null, null);

        Assert.NotNull(filter);
        Assert.Equal(1, filter.Version);
        Assert.Equal("New Filter", filter.Name);
        Assert.NotEqual(Guid.Empty, filter.Id);
        Assert.NotEmpty(filter.Items);
    }

    [Fact]
    public async void Loading_Pure_Filter()
    {
        var newFilter = await _sut.Create(null, null);
        Assert.NotNull(newFilter);

        var loadedFilter = await _sut.Create(null, "load src pure");
        
        Assert.NotNull(loadedFilter);
        Assert.Equal(1, loadedFilter.Version);
        Assert.Equal("pure", loadedFilter.Name);
        Assert.NotEqual(newFilter.Id, loadedFilter.Id);
    }

    [Fact] // Idk how i want this to work
    public async void Loading_Existing_Filter()
    {
        var newFilter = await _sut.Create(null, null);
        Assert.NotNull(newFilter);

        var loadedFilter = await _sut.Create(newFilter, "load src pure");

        Assert.NotNull(loadedFilter);
        Assert.Equal(1, loadedFilter.Version);
        Assert.Equal($"load", loadedFilter.Action);
        Assert.NotEqual(newFilter.Name, loadedFilter.Name);
        Assert.NotEqual(newFilter.Id, loadedFilter.Id);
    }

    [Fact]
    public void Loading_Existing_Filter_With_Unavailable_Items()
    {

    }

    [Fact]
    public void Loading_Existing_Filter_Without_New_Items()
    {

    }

    [Fact]
    public void Adjusting_Existing_Filter_With_Invalid_Command()
    {

    }

    [Fact]
    public void Adjusting_Existing_Filter_With_Valid_Command()
    {

    }

    [Fact]
    public void Querying_Existing_Filter_With_Invalid_Command()
    {

    }

    [Fact]
    public void Querying_Existing_Filter_With_Valid_Command()
    {

    }
}