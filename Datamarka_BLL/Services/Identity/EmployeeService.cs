using Datamarka_BLL.Contracts.Identity;
using Datamarka_DAL;
using Datamarka_DomainModel.Models.Identity;
using System.Data;

namespace Datamarka_BLL.Services.Identity
{
	internal class EmployeeService : IEmployeeService
	{
		private readonly IUnitOfWork _unitOfWork;

		public EmployeeService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public List<Employee> GetEmployees()
		{

			var repo = _unitOfWork.GetRepository<Employee>();

			var query = repo.AsReadOnlyQueryable();

			query = from employee in query
					select employee;

			var projectedQuery = from employee in query
								 select new Employee
								 {
									 User = employee.User,
									 Id = employee.Id,
									 Post = employee.Post,
									 Skills = employee.Skills,
								 };

			return projectedQuery.ToList();
		}

		public Employee? GetEmployeeById(long? employeeId)
		{
			var repo = _unitOfWork.GetRepository<Employee>();
			
			var employee = repo.AsReadOnlyQueryable()
				.FirstOrDefault(em => em.Id == employeeId);
			
			return employee;
		}

		public async Task<Employee> CreateEmployee(Employee employee)
		{
			var repo = _unitOfWork.GetRepository<Employee>();

			var newDbEmployee = new Employee
			{
				User = employee.User,
				Post = employee.Post,
				Skills = employee.Skills,
			};

			Employee trackedEmployee = repo.Create(newDbEmployee);

			await _unitOfWork.SaveChangesAsync();

			return trackedEmployee;
		}

		public async Task WriteEmployee(Employee employeeToWrite)
		{
			var repo = _unitOfWork.GetRepository<Employee>();

			repo.InsertOrUpdate(
			employee => employee.Id == employeeToWrite.Id,
			employeeToWrite
			);

			await _unitOfWork.SaveChangesAsync();
		}


		public async Task DeleteEmployee(long employeeId)
		{
			var repo = _unitOfWork.GetRepository<Employee>();
			var trackedEmployee = repo
				.AsQueryable()
				.First(em => em.Id == employeeId);

			repo.Delete(trackedEmployee);
			await _unitOfWork.SaveChangesAsync();
		}

	}
}
