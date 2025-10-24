using Microsoft.Extensions.Logging;
namespace PmtAdmin.Infrastructure.Logging
{
    public class LoggerAdapter<T> : IAppLogger<T> where T : class
    {
        private readonly ILogger<T> _logger;
        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }
        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }
    }

    public interface IAppLogger<T> where T : class
    {
        public void LogInformation(string message, params object[] args);
        public void LogWarning(string message, params object[] args);
        public void LogError(string message, params object[] args);
    }
}
