using InterRapidisimoBack.Application.Dtos;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InterRapidisimoBack.API.Swagger;

public sealed class EstudianteCreateExampleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.MethodInfo.GetParameters()
            .Any(parameter => parameter.ParameterType == typeof(EstudianteCreateDto)))
        {
            return;
        }

        if (operation.RequestBody is null
            || !operation.RequestBody.Content.TryGetValue("application/json", out var requestBody))
        {
            return;
        }

        requestBody.Example = new OpenApiObject
        {
            ["identificationNumber"] = new OpenApiString("S1006"),
            ["firstName"] = new OpenApiString("Laura"),
            ["lastName"] = new OpenApiString("García"),
            ["email"] = new OpenApiString("laura.s1006@example.com"),
            ["phone"] = new OpenApiString("3001000006"),
            ["academicProgramId"] = new OpenApiInteger(1),
            ["subjectIds"] = new OpenApiArray
            {
                new OpenApiInteger(1),
                new OpenApiInteger(3),
                new OpenApiInteger(5)
            }
        };
    }
}