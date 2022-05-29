using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Data;

namespace PS.Pages.Settings.Blacklist
{
    public partial class CreateUpdate
    {
        [CascadingParameter] BlazoredModalInstance ModalInstance { get; set; }
        [Inject] public MainContext context { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Parameter] public int Id { get; set; }
        CsValidation? CsValidation;
        BlackList blackList = new BlackList();

        protected override async void OnInitialized()
        {
            if (Id > 0)
            {
                blackList = await context.BlackLists.FirstOrDefaultAsync(x => x.Id == Id);
                if (blackList == null)
                {
                    await JS.DisplayMessage("Url not found.");
                    await ModalInstance.CloseAsync(ModalResult.Ok<BlackList>(blackList));
                }
                else
                    StateHasChanged();
            }
        }

        async Task Save()
        {
            CsValidation.ClearErrors();

            var errors = new Dictionary<string, List<string>>();

            blackList.Saving = true;

            if (await context.Groups.AnyAsync(x => x.Name == blackList.Url && x.Id != blackList.Id))
                errors.Add(nameof(Group.Name), new() { "This url added." });

            if (errors.Any())
                CsValidation.DisplayErrors(errors);
            else
            {
                blackList.Url = blackList.Url.ToLower();
                if (blackList.Id == 0) await context.BlackLists.AddAsync(blackList);
                await context.SaveChangesAsync();

                await ModalInstance.CloseAsync(ModalResult.Ok<BlackList>(blackList));
            }

            blackList.Saving = false;
        }
    }
}
