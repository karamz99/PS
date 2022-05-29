using PS.Data;
using System.Net;

namespace PS
{
    public static class CustomersExtensions
    {
        public static async void Add(this List<Customer> Customers, IPAddress address, long received, long sent)
        {
            try
            {
                var customer = Customers.FirstOrDefault(x => x.IP == address.ToString());
                if (customer != null)
                {
                    customer.LastRequestTime = DateTime.Now;
                    customer.DataUsageRecieved += received;
                    customer.DataUsageSent += sent;
                }
            }
            catch { }
        }

        public static Customer? ByIP(this List<Customer> Customers, IPAddress address)
        {
            return Customers.FirstOrDefault(x => x.IP == address.ToString());
        }
    }
}
