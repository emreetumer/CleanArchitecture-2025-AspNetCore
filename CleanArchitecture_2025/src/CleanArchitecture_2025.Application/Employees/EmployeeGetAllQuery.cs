using CleanArchitecture_2025.Domain.Abstractions;
using CleanArchitecture_2025.Domain.Employees;
using CleanArchitecture_2025.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

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
    private readonly UserManager<AppUser> _userManager;

    public EmployeeGetAllQueryHandler(IEmployeeRepository employeeRepository, UserManager<AppUser> userManager)
    {
        _employeeRepository = employeeRepository;
        _userManager = userManager;
    }

    public Task<IQueryable<EmployeeGetAllQueryResponse>> Handle(EmployeeGetAllQuery request, CancellationToken cancellationToken)
    {

        var response = (from employee in _employeeRepository.GetAll()
                        join create_user in _userManager.Users.AsQueryable() on employee.CreateUserId
                            equals create_user.Id
                        join update_user in _userManager.Users.AsQueryable() on employee.CreateUserId
                            equals update_user.Id into update_user
                        from update_users in update_user.DefaultIfEmpty()
                        select new EmployeeGetAllQueryResponse
                        {
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Salary = employee.Salary,
                            BirthOfDate = employee.BirthOfDate,
                            CreateAt = employee.CreateAt,
                            DeleteAt = employee.DeleteAt,
                            Id = employee.Id,
                            IsDeleted = employee.IsDeleted,
                            TCNo = employee.PersonelInformation.TCNo,
                            UpdateAt = employee.UpdateAt,
                            CreateUserId = employee.CreateUserId,
                            CreateUserName = create_user.FirstName + " " + create_user.LastName + " (" + create_user.Email + " )",
                            UpdateUserId = employee.UpdateUserId,
                            UpdateUserName = employee.UpdateUserId == null ? null : update_users.FirstName + " " + update_users.LastName + " (" + update_users.Email + " )",
                        });
        return Task.FromResult(response);

        //var response = _employeeRepository.GetAll()
        //    .Select(s => new EmployeeGetAllQueryResponse
        //    {
        //        Id = s.Id,
        //        FirstName = s.FirstName,
        //        LastName = s.LastName,
        //        BirthOfDate = s.BirthOfDate,
        //        Salary = s.Salary,
        //        TCNo = s.PersonelInformation.TCNo,
        //        UpdateAt = s.UpdateAt,
        //        CreateAt = s.CreateAt,
        //        DeleteAt = s.DeleteAt,
        //        IsDeleted = s.IsDeleted,
        //    })
        //    .AsQueryable();

        //return Task.FromResult(response);
    }
}
