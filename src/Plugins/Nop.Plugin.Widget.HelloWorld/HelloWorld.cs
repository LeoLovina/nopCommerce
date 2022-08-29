using System.Threading.Tasks;
using Nop.Services.Plugins;

namespace Nop.Plugin.Widget.HelloWorld
{
    public class HelloWorld : BasePlugin
    {
        public override async Task InstallAsync()
        {
            // logic during installation goes here ..
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }
}