﻿using System.Threading;
using System.Threading.Tasks;
using PS.Core.EventArguments;
using PS.Core.Helpers;
using PS.Core.Http;
using PS.Core.Network.Tcp;

namespace PS.Core
{
    public partial class ProxyServer
    {

        /// <summary>
        ///     Handle upgrade to websocket
        /// </summary>
        private async Task handleWebSocketUpgrade(SessionEventArgs args,
            HttpClientStream clientStream, TcpServerConnection serverConnection,
            CancellationTokenSource cancellationTokenSource, CancellationToken cancellationToken)
        {
            await serverConnection.Stream.WriteRequestAsync(args.HttpClient.Request, cancellationToken);

            var httpStatus = await serverConnection.Stream.ReadResponseStatus(cancellationToken);

            var response = args.HttpClient.Response;
            response.HttpVersion = httpStatus.Version;
            response.StatusCode = httpStatus.StatusCode;
            response.StatusDescription = httpStatus.Description;

            await HeaderParser.ReadHeaders(serverConnection.Stream, response.Headers,
                cancellationToken);

            await clientStream.WriteResponseAsync(response, cancellationToken);

            // If user requested call back then do it
            if (!args.HttpClient.Response.Locked)
            {
                await onBeforeResponse(args);
            }

            await TcpHelper.SendRaw(clientStream, serverConnection.Stream, BufferPool,
                args.OnDataSent, args.OnDataReceived, cancellationTokenSource, ExceptionFunc);
        }
    }
}
