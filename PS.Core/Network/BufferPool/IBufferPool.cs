﻿using System;

namespace PS.Core.StreamExtended.BufferPool
{
    /// <summary>
    ///     Use this interface to implement custom buffer pool.
    ///     To use the default buffer pool implementation use DefaultBufferPool class.
    /// </summary>
    public interface IBufferPool : IDisposable
    {
        int BufferSize { get; }

        byte[] GetBuffer();

        byte[] GetBuffer(int bufferSize);

        void ReturnBuffer(byte[] buffer);
    }
}
