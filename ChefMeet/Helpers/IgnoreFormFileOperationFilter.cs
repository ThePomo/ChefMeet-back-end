using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ChefMeet.Helpers
{
    public class IgnoreFormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Se l'azione contiene parametri IFormFile, evita di generare dettagli in Swagger
            var hasFormFile = context.ApiDescription.ParameterDescriptions
                .Any(p => p.Type == typeof(IFormFile) || p.Type == typeof(IFormFileCollection));

            if (hasFormFile)
            {
                operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = "🔒 Endpoint con upload file" } };
                operation.RequestBody = null;
                operation.Responses.Clear();
                operation.Summary = "⛔ Endpoint non visibile su Swagger";
                operation.Description = "Questo endpoint utilizza IFormFile (upload immagine) e non è gestibile da Swagger.";
            }
        }
    }
}
