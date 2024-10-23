using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JiwaCustomerPortal
{
    // Work-around for bug where you cannot have a blazor component with @rendermode InteractiveServer AND generic parameters - which is EXACTLY what I need in
    // JiwaAPIAutoQueryGrid.razor - so this is the work-around - define this attribute and instead of @rendermode InteractiveServer at the top
    // of the component, you put @attribute [type: RenderModeInteractiveServer] instead.
    // https://github.com/dotnet/razor/issues/9683
    public class RenderModeInteractiveServer : RenderModeAttribute
    {
        public override IComponentRenderMode Mode => RenderMode.InteractiveServer;        
    }
}
