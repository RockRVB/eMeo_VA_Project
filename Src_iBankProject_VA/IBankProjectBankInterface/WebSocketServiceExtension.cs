using eCATDistributedServiceProtocol;
using IBankProjectBusinessServiceProtocol;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebSocketServiceProtocol;

namespace IBankProjectBankInterface
{
    public class WebSocketServiceExtension
    {
        public IWebSocketServiceProtocol WebSocketService { get; internal set; }

        public IBankProjectBusinessServiceContext Context { get; set; }

        public ProxyCommandResult WebSocketServiceExtensionTest(string argAccessToken, string argAlias)
        {
            Task.Run(() =>
            {
                Thread.Sleep(100);
                //Trigger event to client
                WebSocketService.NotifyDistributedEvent("WebSocketServiceExtensionTestEvent", new EventJson("WebSocketServiceExtension", "WebSocketServiceExtensionTestEvent", new Dictionary<string, string>() {
                {"P1","P1" },
                {"P2", "P2" }
                }));
            });

            return ProxyCommandResult.Success;
        }
    }
}
