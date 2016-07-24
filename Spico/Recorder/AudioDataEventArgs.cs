﻿using System;

namespace Spico.Recorder
{
    public class AudioDataEventArgs
    {
        public Byte[] Data { get; private set; }

        public AudioDataEventArgs(Byte[] data)
        {
            Data = data;
        }
    }
}
