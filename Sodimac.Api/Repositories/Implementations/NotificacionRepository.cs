using Sodimac.Api.Models;
using Sodimac.Api.Models.Contexts;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly SodimacContext _context;
        private readonly ILogger<NotificacionRepository> _logger;

        public NotificacionRepository(SodimacContext context, ILogger<NotificacionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<NotificacionCorreo> RegistrarAsync(NotificacionCorreo notificacion)
        {
            _context.NotificacionCorreos.Add(notificacion);
            await _context.SaveChangesAsync();
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
