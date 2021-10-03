﻿namespace BBP.FasterKVMiner
{
    public class Tracker : IDisposable
    {
        private FasterKVBBPiMiner fasterKV;

        private const int MaxThreadCount = 10;
        private const int BufferMaxCapacity = 4194304 * MaxThreadCount * 5;
        private long piBytesOffset = 0;
        private readonly PiByteBuffer piBytes;
        private readonly Thread[] Threads;

        private readonly int[] BlockSizes = new int[]
        {
            128,
            256,
            512,
            1024,
            4096,
            1048576,
            4194304,
        };

        public Tracker()
        {
            this.fasterKV = new FasterKVBBPiMiner();
            this.piBytes = new PiByteBuffer(
                startingOffset: piBytesOffset,
                maxCapacity: BufferMaxCapacity);
        }

        private async Task<WorkBlock> StartWork(long startingOffset)
        {
            var workBlock = new WorkBlock(
                startingOffset: startingOffset,
                blockSizes: this.BlockSizes);

            return await Task<WorkBlock>.Run(() =>
            {
                return workBlock
                    .AsWorkable()
                    .Work(workingMemory: this.piBytes);
            }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.fasterKV.Dispose();
        }
    }
}