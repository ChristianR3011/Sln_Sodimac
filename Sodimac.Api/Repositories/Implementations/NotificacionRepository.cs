using Sodimac.Api.Models;
using Sodimac.Api.Data;
using Sodimac.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace Sodimac.Api.Repositories.Implementations
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly MongoDataContext _mongo;
        private readonly ILogger<NotificacionRepository> _logger;

        public NotificacionRepository(MongoDataContext mongo, ILogger<NotificacionRepository> logger)
        {
            _mongo = mongo;
            _logger = logger;
        }

        public async Task<NotificacionCorreo> RegistrarAsync(NotificacionCorreo notificacion)
        {
            notificacion.IdNotificacion = await _mongo.NextIdAsync("notificaciones");
            await _mongo.Collection<NotificacionCorreo>("notificacionesCorreo").InsertOneAsync(notificacion);
            return notificacion;
        }

        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                // Simulación del envío de correo (aquí iría la integración con SendGrid, AWS SES, etc.)
                _logger.LogInformation("Simulando envío de correo a {Destinatario} | Asunto: {Asunto}", destinatario, asunto);

                await Task.Delay(100); // Simula latencia

                _logger.LogInformation("Correo enviado exitosamente a {Destinatario}", destinatario);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {Destinatario}", destinatario);
                return false;
            }
        }
    }
}
