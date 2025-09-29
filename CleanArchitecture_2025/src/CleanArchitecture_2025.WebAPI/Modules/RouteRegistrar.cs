namespace CleanArchitecture_2025.WebAPI.Modules;

public static class RouteRegistrar
{
    public static void RegisterRoute(this IEndpointRouteBuilder app)
    {
        app.RegisterEmployeeRoutes();
        app.RegisterAuthRoutes();
    }
}
