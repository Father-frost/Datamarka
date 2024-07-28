using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_BLL.Contracts.Identity
{
	public interface IEmployeeService : IService
	{
		public List<Employee> GetEmployees();

		public Employee GetEmployeeById(long? employeeId);
		public Task<Employee> CreateEmployee(Employee employee);

		public Task WriteEmployee(Employee employeeToWrite);

		public Task DeleteEmployee(long employeeId);
	}
}
