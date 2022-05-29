using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Data;

namespace PS.Pages.Customers
{
    public partial class CreateUpdate
    {

        [CascadingParameter] BlazoredModalInstance ModalInstance { get; set; }
        [Inject] public MainContext context { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Parameter] public int Id { get; set; }
        CsValidation? CsValidation;
        List<Group> groups;
        Customer customer = new Customer();

        protected override async void OnInitialized()
        {
            groups = await context.Groups.OrderBy(x => x.Name).AsNoTracking().ToListAsync();

            if (Id == 0 && groups.Any())
            {
                customer.GroupId = groups.First().Id;
                customer.Group = groups.First();
            }
            else
            {
                customer = await context.Customers.FirstOrDefaultAsync(x => x.Id == Id);
                if (customer == null)
                {
                    await JS.DisplayMessage("Customer not found.");
                    await ModalInstance.CloseAsync(ModalResult.Ok<Customer>(customer));
                }
                else
                {
                    customer.PasswordConfirmation = customer.Password;
                    StateHasChanged();
                }
            }
        }

        async Task Save()
        {
            try
            {
                CsValidation.ClearErrors();

                var errors = new Dictionary<string, List<string>>();

                customer.Saving = true;

                if (await context.Customers.AnyAsync(x => x.Name == customer.Name && x.Id != customer.Id))
                    errors.Add(nameof(Customer.Name), new() { "This customer name added." });

                if (errors.Any())
                    CsValidation.DisplayErrors(errors);
                else
                {
                    if (customer.Id == 0)
                    {
                        customer.Group = null;
                        customer.HashPassword();
                        await context.Customers.AddAsync(customer);
                    }

                    await context.SaveChangesAsync();

                    await ModalInstance.CloseAsync(ModalResult.Ok<Customer>(customer));
                }

                customer.Saving = false;
            }
            catch (Exception ex)
            {
                customer.Saving = false;
                await JS.DisplayMessage(ex.Message);
            }
        }
    }
}
