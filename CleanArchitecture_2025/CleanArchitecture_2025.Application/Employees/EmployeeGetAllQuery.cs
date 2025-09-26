using CleanArchitecture_2025.Domain.Abstractions;
using CleanArchitecture_2025.Domain.Employees;
using MediatR;

namespace CleanArchitecture_2025.Application.Employees;
public sealed record EmployeeGetAllQuery() : IRequest<IQueryable<EmployeeGetAllQueryResponse>>;

public sealed class EmployeeGetAllQueryResponse : EntityDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateOnly BirthOfDate { get; set; }
    public decimal Salary { get; set; }
    public string TCNo { get; set; } = default!;
}

internal sealed class EmployeeGetAllQueryHandler : IRequestHandler<EmployeeGetAllQuery, IQueryable<EmployeeGetAllQueryResponse>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeGetAllQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public Task<IQueryable<EmployeeGetAllQueryResponse>> Handle(EmployeeGetAllQuery request, CancellationToken cancellationToken)
    {
        var response = _employeeRepository.GetAll()
            .Select(s => new EmployeeGetAllQueryResponse
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                BirthOfDate = s.BirthOfDate,
                Salary = s.Salary,
                TCNo = s.PersonelInformation.TCNo,
                UpdateAt = s.UpdateAt,
                CreateAt = s.CreateAt,
                DeleteAt = s.DeleteAt,
                IsDeleted = s.IsDeleted,
            })
            .AsQueryable();

        return Task.FromResult(response);
    }
}
