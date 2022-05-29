using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Data;

namespace PS.Pages.Groups
{
    public partial class CreateUpdate
    {
        [CascadingParameter] BlazoredModalInstance ModalInstance { get; set; }
        [Inject] public MainContext context { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Parameter] public int Id { get; set; }
        CsValidation? CsValidation;
        Group group = new Group();

        protected override async void OnInitialized()
        {
            if (Id > 0)
            {
                group = await context.Groups.FirstOrDefaultAsync(x => x.Id == Id);
                if (group == null)
                {
                    await JS.DisplayMessage("Group not found.");
                    await ModalInstance.CloseAsync(ModalResult.Ok<Group>(group));
                }
                else
                    StateHasChanged();
            }
        }

        async Task Save()
        {
            CsValidation.ClearErrors();

            var errors = new Dictionary<string, List<string>>();

            group.Saving = true;

            if (await context.Groups.AnyAsync(x => x.Name == group.Name && x.Id != group.Id))
                errors.Add(nameof(Group.Name), new() { "This group name added." });

            if (errors.Any())
                CsValidation.DisplayErrors(errors);
            else
            {
                if (group.Id == 0) await context.Groups.AddAsync(group);
                await context.SaveChangesAsync();

                await ModalInstance.CloseAsync(ModalResult.Ok<Group>(group));
            }

            group.Saving = false;
        }
    }
}
