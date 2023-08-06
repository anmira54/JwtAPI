using System;
namespace Core.Models
{
    public class AppSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string JwtSecret { get; set; } = "lNlmmlFuop0FRHKeRJq/IwD4xVaqHPsTmEaBOOoc8LqWdBPmnZ9uVIWTmbP9iWcQqs6kXSuLUiOpq2V/DB/y9Q==";
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}

