using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Pages
{
    public partial class ModalDialog : ComponentBase
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public EventCallback<bool> OnClose { get; set; }

        private Task ModalCancel()
        {
            return OnClose.InvokeAsync(false);
        }

        private Task ModalOk()
        {
            return OnClose.InvokeAsync(true);
        }
    }
}
