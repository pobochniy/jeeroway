using Atheneum.Dto.Robo;
using System.Threading.Tasks;

namespace Atheneum.Interface
{
    public interface IRoboHub
    {
        /// <summary>
        /// С управления на сервер
        /// </summary>
        Task PushControl(RoboControlDto dto);

        /// <summary>
        /// Отправка управления с сервера на робота
        /// </summary>
        Task BroadCastControl(RoboControlDto dto);
    }
}
