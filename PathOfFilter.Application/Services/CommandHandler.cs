
using PathOfFilter.Application.Models;

namespace PathOfFilter.Application.Services
{
    public static class CommandHandler
    {
        public static async Task<ItemFilter> CreateFilter(ItemFilter? currentFilter, string? command)
        {
            if(currentFilter == null) return new ItemFilter(command);

            await FilterSaver(currentFilter);

            return new ItemFilter(currentFilter, command);
        }

        private static async Task FilterSaver(ItemFilter currentFilter)
        {
            Console.WriteLine("Old filter saved");
            return;
        }
    }
}
