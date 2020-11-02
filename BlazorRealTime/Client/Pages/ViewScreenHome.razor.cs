using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorRealTime.Client.Pages
{
    public partial class ViewScreenHome
    {
        private string viewerName;
        private bool isStartView;

        private void StartView()
        {
            isStartView = !isStartView;
            StateHasChanged();
        }
    }
}
