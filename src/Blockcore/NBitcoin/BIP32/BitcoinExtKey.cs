﻿using Blockcore.Consensus.ScriptInfo;
using Blockcore.Networks;

namespace NBitcoin
{
    public abstract class BitcoinExtKeyBase : Base58Data, IDestination
    {
        protected BitcoinExtKeyBase(IBitcoinSerializable key, Network network)
            : base(key.ToBytes(), network)
        {
        }

        protected BitcoinExtKeyBase(string base58, Network network)
            : base(base58, network)
        {
        }


        #region IDestination Members

        public abstract Script ScriptPubKey
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// Base58 representation of an ExtKey, within a particular network.
    /// </summary>
    public class BitcoinExtKey : BitcoinExtKeyBase, ISecret
    {
        /// <summary>
        /// Constructor. Creates an extended key from the Base58 representation, checking the expected network.
        /// </summary>
        public BitcoinExtKey(string base58, Network expectedNetwork = null)
            : base(base58, expectedNetwork)
        {
        }

        /// <summary>
        /// Constructor. Creates a representation of an extended key, within the specified network.
        /// </summary>
        public BitcoinExtKey(ExtKey key, Network network)
            : base(key, network)
        {
        }

        /// <summary>
        /// Gets whether the data is the correct expected length.
        /// </summary>
        protected override bool IsValid
        {
            get
            {
                return this.vchData.Length == 74;
            }
        }

        private ExtKey _Key;

        /// <summary>
        /// Gets the extended key, converting from the Base58 representation.
        /// </summary>
        public ExtKey ExtKey
        {
            get
            {
                if (this._Key == null)
                {
                    this._Key = new ExtKey();
                    this._Key.ReadWrite(this.vchData);
                }
                return this._Key;
            }
        }

        /// <summary>
        /// Gets the type of item represented by this Base58 data.
        /// </summary>
        public override Base58Type Type
        {
            get
            {
                return Base58Type.EXT_SECRET_KEY;
            }
        }

        /// <summary>
        /// Gets the script of the hash of the public key corresponing to the private key 
        /// of the extended key of this Base58 item.
        /// </summary>
        public override Script ScriptPubKey
        {
            get
            {
                return this.ExtKey.ScriptPubKey;
            }
        }

        /// <summary>
        /// Gets the Base58 representation, in the same network, of the neutered extended key.
        /// </summary>
        public BitcoinExtPubKey Neuter()
        {
            return this.ExtKey.Neuter().GetWif(this.Network);
        }

        #region ISecret Members

        /// <summary>
        /// Gets the private key of the extended key of this Base58 item.
        /// </summary>
        public Key PrivateKey
        {
            get
            {
                return this.ExtKey.PrivateKey;
            }
        }

        #endregion

        /// <summary>
        /// Implicit cast from BitcoinExtKey to ExtKey.
        /// </summary>
        public static implicit operator ExtKey(BitcoinExtKey key)
        {
            if (key == null)
                return null;
            return key.ExtKey;
        }
    }

    /// <summary>
    /// Base58 representation of an ExtPubKey, within a particular network.
    /// </summary>
    public class BitcoinExtPubKey : BitcoinExtKeyBase
    {
        /// <summary>
        /// Constructor. Creates an extended public key from the Base58 representation, checking the expected network.
        /// </summary>
        public BitcoinExtPubKey(string base58, Network expectedNetwork = null)
            : base(base58, expectedNetwork)
        {
        }

        /// <summary>
        /// Constructor. Creates a representation of an extended public key, within the specified network.
        /// </summary>
        public BitcoinExtPubKey(ExtPubKey key, Network network)
            : base(key, network)
        {
        }

        private ExtPubKey _PubKey;

        /// <summary>
        /// Gets the extended public key, converting from the Base58 representation.
        /// </summary>
        public ExtPubKey ExtPubKey
        {
            get
            {
                if (this._PubKey == null)
                {
                    this._PubKey = new ExtPubKey();
                    this._PubKey.ReadWrite(this.vchData);
                }
                return this._PubKey;
            }
        }

        /// <summary>
        /// Gets the type of item represented by this Base58 data.
        /// </summary>
        public override Base58Type Type
        {
            get
            {
                return Base58Type.EXT_PUBLIC_KEY;
            }
        }

        /// <summary>
        /// Gets the script of the hash of the public key of the extended key of this Base58 item.
        /// </summary>
        public override Script ScriptPubKey
        {
            get
            {
                return this.ExtPubKey.ScriptPubKey;
            }
        }

        /// <summary>
        /// Implicit cast from BitcoinExtPubKey to ExtPubKey.
        /// </summary>
        public static implicit operator ExtPubKey(BitcoinExtPubKey key)
        {
            if (key == null)
                return null;
            return key.ExtPubKey;
        }
    }
}
