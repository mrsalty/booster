using System.Threading.Tasks;

namespace Booster
{
    public interface IHandlerAsync<T> where T : IEvent
    {
        Task HandleAsync(T @event);
    }
}