using System.Security.Claims;

namespace CadastroDePessoas.API.Middlewares
{
    /// <summary>
    /// Middleware para adicionar camadas extras de segurança
    /// </summary>
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityMiddleware> _logger;

        public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            if (context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            AdicionarHeadersSeguranca(context);
            LogarTentativaAcesso(context);

            if (!ValidarOrigemRequisicao(context))
            {
                _logger.LogWarning("Origem não autorizada: {Origin}", context.Request.Headers["Origin"].FirstOrDefault());
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Origem não autorizada");
                return;
            }

            if (!ValidarRateLimit(context))
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Muitas requisições. Tente novamente em alguns minutos.");
                return;
            }

            await _next(context);
        }

        private void AdicionarHeadersSeguranca(HttpContext context)
        {
            var response = context.Response;

            response.Headers.Add("X-Content-Type-Options", "nosniff");
            response.Headers.Add("X-Frame-Options", "DENY");
            response.Headers.Add("X-XSS-Protection", "1; mode=block");
            response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            
           
            response.Headers.Add("Content-Security-Policy", "default-src 'self'; connect-src 'self' https://cadastro-de-pessoas-web.onrender.com");
            
            response.Headers.Remove("Server");
            response.Headers.Remove("X-Powered-By");
            response.Headers.Remove("X-AspNet-Version");
        }

        private void LogarTentativaAcesso(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
            var method = context.Request.Method;
            var path = context.Request.Path;
            var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(
                "API Access: {Method} {Path} | IP: {IP} | UserAgent: {UserAgent} | Auth: {IsAuth} | User: {UserId}",
                method, path, ip, userAgent, isAuthenticated, userId ?? "Anonymous");

            if (!isAuthenticated && !IsEndpointPublico(path))
            {
                _logger.LogWarning(
                    "Tentativa de acesso não autorizado: {Method} {Path} | IP: {IP}",
                    method, path, ip);
            }
        }

        private static bool ValidarOrigemRequisicao(HttpContext context)
        {
            var origin = context.Request.Headers["Origin"].FirstOrDefault();
            var host = context.Request.Host.Host;

            // Allow localhost and render.com domains
            if (host.Contains("localhost") || host.Contains("render.com"))
                return true;

            // Allow requests without Origin header (direct API calls)
            if (string.IsNullOrEmpty(origin))
                return true;

            var origensPermitidas = new[]
            {
                "https://cadastro-de-pessoas-web.onrender.com",
                "http://localhost:3001",
                "https://localhost:3001",
                "http://localhost:3000",
                "https://localhost:3000"
            };

            return origensPermitidas.Contains(origin);
        }

        private bool ValidarRateLimit(HttpContext context)
        { 
            return true; // Implementar lógica de rate limiting se necessário
        }

        private bool IsEndpointPublico(string path)
        {
            var endpointsPublicos = new[]
            {
                "/api/v1/auth/login",
                "/api/v2/auth/login",
                "/api/v1/auth/logout",
                "/api/v2/auth/logout",
                "/api/v1/auth/register",
                "/api/v1/auth/reset-admin",
                "/api/v1/health",
                "/api/v2/health",
                "/swagger",
                "/health"
            };

            return endpointsPublicos.Any(endpoint => path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase));
        }
    }
}