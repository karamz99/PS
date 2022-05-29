using PS.Core.EventArguments;
using System.Text;

namespace PS.Services
{
    public static class ProxyEventArgsBaseExtensions
    {
        public static SampleClientState GetState(this ProxyEventArgsBase args)
        {
            if (args.ClientUserData == null)
            {
                args.ClientUserData = new SampleClientState();
            }

            return (SampleClientState)args.ClientUserData;
        }

    }
    public class SampleClientState
    {
        public StringBuilder PipelineInfo { get; } = new StringBuilder();
    }
}
