using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Data;
using PS.Services;
using System.Collections.Generic;

namespace PS.Pages.Groups
{
    public partial class Groups
    {
        [CascadingParameter] public IModalService? Modal { get; set; }
        [Inject] public MainContext? context { get; set; }
        [Inject] public NavigationManager NavMngr { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        List<Group>? groups { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await FillGroups();
        }

        async Task FillGroups()
        {
            groups = await context.Groups.OrderBy(x => x.Name).Include(x => x.Customers).AsNoTracking().ToListAsync();
            ProxyService.Customers = await context.Customers.Include(x => x.Group).ToListAsync();
        }

        async Task CreateGroup()
        {
            var modal = Modal.Show<CreateUpdate>("Create new group");
            var modalResult = await modal.Result;
            if (!modalResult.Cancelled)
                await FillGroups();
        }

        async Task UpdateGroup(Group? group)
        {
            var mp = new ModalParameters();
            mp.Add(nameof(Group.Id), group.Id);
            var modal = Modal.Show<CreateUpdate>("Update group", mp);
            var modalResult = await modal.Result;
            if (!modalResult.Cancelled)
                await FillGroups();
        }

        async Task DeleteGroup(Group? group)
        {
            if (await JS.DisplayConfirm("Delete group", "Are you sure to delete selected group?"))
            {
                var groupInDb = await context.Groups.FirstOrDefaultAsync(x => x.Id == group.Id);
                if (groupInDb != null)
                {
                    context.Groups.Remove(groupInDb);
                    await context.SaveChangesAsync();
                }
                groups.Remove(group);
            }
        }
    }
}
