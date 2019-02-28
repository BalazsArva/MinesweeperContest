using System.Threading.Tasks;

namespace Minesweeper.WebAPI.Hubs.ClientContracts
{
    public interface IGameClient
    {
        Task GameTableUpdated();
    }
}