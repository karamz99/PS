using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Data;
using PS.Services;
using System.Collections.Generic;

namespace PS.Pages.Settings
{
    public partial class RequestsPage
    {
        [CascadingParameter] public IModalService? Modal { get; set; }
        [Inject] public MainContext? context { get; set; }
        [Inject] public NavigationManager NavMngr { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        List<RequestInfo>? requests { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await FillRequests();
        }

        async Task FillRequests()
        {
            //requests = await context.RequestInfos.OrderBy(x => x.Date).Include(x => x.Customer).AsNoTracking().ToListAsync();
        }
    }
}
