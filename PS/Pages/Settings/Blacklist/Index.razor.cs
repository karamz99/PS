using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Data;
using PS.Services;
using System.Collections.Generic;

namespace PS.Pages.Settings.Blacklist
{
    public partial class Index
    {
        [CascadingParameter] public IModalService? Modal { get; set; }
        [Inject] public MainContext? context { get; set; }
        [Inject] public NavigationManager NavMngr { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        List<BlackList>? blacklist { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await FillBlacklist();
        }

        async Task FillBlacklist()
        {
            blacklist = await context.BlackLists.OrderBy(x => x.Url).AsNoTracking().ToListAsync();
            ProxyService.BlackList = blacklist.Select(x => x.Url).ToArray();
        }

        async Task CreateUrl()
        {
            var modal = Modal.Show<CreateUpdate>("Add url to blacklist ");
            var modalResult = await modal.Result;
            if (!modalResult.Cancelled)
                await FillBlacklist();
        }

        async Task UpdateUrl(BlackList? blacklist)
        {
            var mp = new ModalParameters();
            mp.Add(nameof(BlackList.Id), blacklist.Id);
            var modal = Modal.Show<CreateUpdate>("Update url", mp);
            var modalResult = await modal.Result;
            if (!modalResult.Cancelled)
                await FillBlacklist();
        }

        async Task DeleteUrl(BlackList? blacklist)
        {
            if (await JS.DisplayConfirm("Delete url", "Are you sure to delete selected url?"))
            {
                var urlInDb = await context.BlackLists.FirstOrDefaultAsync(x => x.Id == blacklist.Id);
                if (urlInDb != null)
                {
                    context.BlackLists.Remove(urlInDb);
                    await context.SaveChangesAsync();
                }
                this.blacklist.Remove(blacklist);
                await FillBlacklist();
            }
        }
    }
}
