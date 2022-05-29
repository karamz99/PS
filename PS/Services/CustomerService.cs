using PS.Data;

namespace PS.Services
{
    public interface ICustomerService
    {
        static Customer? Customer { get; set; }
    }

    public class CustomerService : ICustomerService
    {
        public static Customer? Customer { get; set; }
    }
}
