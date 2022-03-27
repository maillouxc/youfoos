using System.Threading.Tasks;

namespace YouFoos.GameEventsService.Services
{
    public interface IGameplayMessageHandler
    {
        Task HandleMessageAsync(string messageText);
    }
}
