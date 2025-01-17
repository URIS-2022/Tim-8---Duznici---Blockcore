﻿using Blockcore.Base;
using Blockcore.Configuration.Settings;
using Blockcore.Consensus.Checkpoints;
using Blockcore.Interfaces;
using Blockcore.Networks;

namespace Blockcore.IntegrationTests.Common.EnvironmentMockUpHelpers
{
    public class InitialBlockDownloadStateMock : IInitialBlockDownloadState
    {
        public InitialBlockDownloadStateMock(IChainState chainState, Network network, ConsensusSettings consensusSettings, ICheckpoints checkpoints)
        {
        }

        public bool IsInitialBlockDownload()
        {
            return false;
        }
    }
}
