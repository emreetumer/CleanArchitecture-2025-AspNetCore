using CleanArchitecture_2025.Application.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace CleanArchitecture_2025.WebAPI.Controllers;

[Route("odata")]
[ApiController]
[EnableQuery]
public class AppODataController : ODataController
{
    private readonly ISender _sender;

    public AppODataController(ISender sender)
    {
        _sender = sender;
    }

    public static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new();
        builder.EnableLowerCamelCase();
        builder.EntitySet<EmployeeGetAllQueryResponse>("employees");
        return builder.GetEdmModel();
    }

    [HttpGet("employees")]
    //public async Task<IActionResult> GetAllEmployees(CancellationToken cancellationToken)
    public async Task<IQueryable<EmployeeGetAllQueryResponse>> GetAllEmployees(CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new EmployeeGetAllQuery(), cancellationToken);
        //return Ok(response);
        return response;
    }
}
