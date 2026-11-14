using Sodimac.Api.Models;

namespace Sodimac.Api.Repositories.Interfaces
{
    public interface INotificacionRepository
    {
        Task<NotificacionCorreo> RegistrarAsync(NotificacionCorreo notificacion);
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
    }
}
