using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Data;
using PS.Services;
using System.Collections.Generic;

namespace PS.Pages.Customers
{
    public partial class Customers
    {
        [CascadingParameter] public IModalService? Modal { get; set; }
        [Parameter] public int Id { get; set; }
        [Inject] public MainContext? context { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        List<Customer>? customers { get; set; }
        Group? group;

        protected override async Task OnInitializedAsync()
        {
            await FillCustomers();
        }

        async Task FillCustomers()
        {
            var quiry = context.Customers.OrderBy(x => x.Name).Include(x => x.Group).AsQueryable();
            if (Id > 0)
            {
                quiry = quiry.Where(x => x.GroupId == Id);
                group = await context.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            }

            customers = await quiry.AsNoTracking().ToListAsync();
            ProxyService.Customers = await context.Customers.Include(x => x.Group).ToListAsync();
        }

        async Task CreateCustomer()
        {
            var modal = Modal.Show<CreateUpdate>("Create new user");
            var modalResult = await modal.Result;
            if (!modalResult.Cancelled)
                await FillCustomers();
        }

        async Task UpdateCustomer(Customer? customer)
        {
            var mp = new ModalParameters();
            mp.Add(nameof(Customer.Id), customer.Id);
            var modal = Modal.Show<CreateUpdate>("Update user", mp);
            var modalResult = await modal.Result;
            if (!modalResult.Cancelled)
                await FillCustomers();
        }

        async Task DeleteCustomer(Customer? customer)
        {
            if (await JS.DisplayConfirm("Delete user", "Are you sure to delete selected user?"))
            {
                var customerInDb = await context.Customers.FirstOrDefaultAsync(x => x.Id == customer.Id);
                if (customerInDb != null)
                {
                    context.Customers.Remove(customerInDb);
                    await context.SaveChangesAsync();
                }
                customers.Remove(customer);
            }
        }
    }
}
