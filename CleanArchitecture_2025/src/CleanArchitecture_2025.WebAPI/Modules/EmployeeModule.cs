using CleanArchitecture_2025.Application.Employees;
using CleanArchitecture_2025.Domain.Employees;
using MediatR;
using TS.Result;

namespace CleanArchitecture_2025.WebAPI.Modules;

public static class EmployeeModule
{
    public static void RegisterEmployeeRoutes(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/employees").WithTags("Employees").RequireAuthorization();

        group.MapPost(string.Empty,
            async (ISender sender, EmployeeCreateCommand request, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(request, cancellationToken);
                return response.IsSuccessful ? Results.Ok(response) : Results.InternalServerError(response);
            })
            .Produces<Result<string>>();

        group.MapGet(string.Empty,
            async (ISender sender, Guid id, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new EmployeeGetQuery(id), cancellationToken);
                return response.IsSuccessful ? Results.Ok(response) : Results.InternalServerError(response);
            })
            .Produces<Result<Employee>>();

    }
}
