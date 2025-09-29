using CleanArchitecture_2025.Domain.Employees;
using MediatR;
using TS.Result;

namespace CleanArchitecture_2025.Application.Employees;
public sealed record EmployeeGetQuery(
    Guid Id) : IRequest<Result<Employee>>;

internal sealed class EmployeeGetQueryHandler : IRequestHandler<EmployeeGetQuery, Result<Employee>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeGetQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<Employee>> Handle(EmployeeGetQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (employee is null)
        {
            return Result<Employee>.Failure("Personel bulunamadı");
        }

        return employee;
    }
}