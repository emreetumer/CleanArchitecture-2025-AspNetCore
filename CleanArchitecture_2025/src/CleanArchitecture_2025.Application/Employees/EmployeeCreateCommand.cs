using CleanArchitecture_2025.Domain.Employees;
using FluentValidation;
using GenericRepository;
using Mapster;
using MediatR;
using TS.Result;

namespace CleanArchitecture_2025.Application.Employees;
public sealed record EmployeeCreateCommand(
    string FirstName,
    string LastName,
    DateOnly BirthOfDate,
    decimal Salary,
    PersonelInformation PersonelInformation,
    Address? Address) : IRequest<Result<string>>;

public sealed class EmployeeCreateCommandValidatior : AbstractValidator<EmployeeCreateCommand>
{
    public EmployeeCreateCommandValidatior()
    {
        RuleFor(x => x.FirstName).MinimumLength(3).WithMessage("Ad alanı en az 3 karakter olmalıdır.");
        RuleFor(x => x.LastName).MinimumLength(3).WithMessage("Soyad alanı ne az 3 karakter olamlıdır");
        RuleFor(x => x.PersonelInformation.TCNo)
            .MinimumLength(11).WithMessage("Geçerli bir TC numarası yazın")
            .MaximumLength(11).WithMessage("Geçerli bir TC numarası yazın");
    }
}

// Handler metodu başka bir katmandan erişilemesi lazım bu sebeple internal yaptık.
internal sealed class EmployeeCreateCommandHandler : IRequestHandler<EmployeeCreateCommand, Result<string>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    public EmployeeCreateCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(EmployeeCreateCommand request, CancellationToken cancellationToken)
    {
        var isEmployeeExists = await _employeeRepository.AnyAsync(p => p.PersonelInformation.TCNo == request.PersonelInformation.TCNo, cancellationToken);
        if (isEmployeeExists)
        {
            return Result<string>.Failure("Bu TC numarası daha önce kaydedilmiş");
        }

        Employee employee = request.Adapt<Employee>();

        _employeeRepository.Add(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return "Personel kaydı başarıyla tamamlandı.";
    }
}