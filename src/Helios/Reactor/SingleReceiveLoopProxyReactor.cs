﻿using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Helios.Net;
using Helios.Reactor.Response;
using Helios.Serialization;

namespace Helios.Reactor
{
    /// <summary>
    /// <see cref="IReactor"/> implementation which spawns <see cref="ReactorProxyResponseChannel"/> instances for responding directly with connected clients,
    /// but maintains a single event loop for responding to incoming requests, rather than allowing each <see cref="ReactorProxyResponseChannel"/> to maintain
    /// its own independent event loop.
    /// 
    /// Great for scenarios where you want to be able to set a single event loop for a server and forget about it.
    /// </summary>
    public abstract class SingleReceiveLoopProxyReactor<TIdentifier> : ProxyReactorBase<TIdentifier>
    {
        protected SingleReceiveLoopProxyReactor(IPAddress localAddress, int localPort, NetworkEventLoop eventLoop, IMessageEncoder encoder, IMessageDecoder decoder, SocketType socketType = SocketType.Stream, ProtocolType protocol = ProtocolType.Tcp, int bufferSize = NetworkConstants.DEFAULT_BUFFER_SIZE) 
            : base(localAddress, localPort, eventLoop, encoder, decoder, socketType, protocol, bufferSize)
        {
        }

        protected override void ReceivedData(NetworkData availableData, ReactorResponseChannel responseChannel)
        {
            if (EventLoop.Receive != null)
            {
                List<NetworkData> decoded;
                Decoder.Decode(ConnectionAdapter, availableData, out decoded);
                foreach(var message in decoded)
                    EventLoop.Receive(message, responseChannel);
            }
        }
    }
}