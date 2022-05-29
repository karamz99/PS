using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PS.Data;

namespace PS.Pages.Status
{
    public partial class Index
    {
        [Inject] public MainContext context { get; set; }
        bool loading = true;
        List<Group> groups;
        List<Customer> customers;

        protected override async Task OnInitializedAsync()
        {
            loading = true;
            groups = await context.Groups.Include(x => x.Customers).AsNoTracking().ToListAsync();
            customers = await context.Customers.AsNoTracking().ToListAsync();
            loading = false;
        }
    }
}
