﻿using System;
using System.Linq;
using NBitcoin.DataEncoders;
using Xunit;

namespace NBitcoin.Tests
{
    public class Bech32Test
    {
        private static readonly string[] VALID_CHECKSUM =
        {
            "A12UEL5L",
            "an83characterlonghumanreadablepartthatcontainsthenumber1andtheexcludedcharactersbio1tt5tgs",
            "abcdef1qpzry9x8gf2tvdw0s3jn54khce6mua7lmqqqxw",
            "11qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqc8247j",
            "split1checkupstagehandshakeupstreamerranterredcaperred2y9e3w"
        };

        private static readonly string[][] VALID_ADDRESS = {
            new [] { "BC1QW508D6QEJXTDG4Y5R3ZARVARY0C5XW7KV8F3T4", "0014751e76e8199196d454941c45d1b3a323f1433bd6"},
            new [] { "tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sl5k7","00201863143c14c5166804bd19203356da136c985678cd4d27a1b8c6329604903262"},
            new [] { "bc1pw508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7k7grplx", "5128751e76e8199196d454941c45d1b3a323f1433bd6751e76e8199196d454941c45d1b3a323f1433bd6"},
            new [] { "BC1SW50QA3JX3S", "6002751e"},
            new [] { "bc1zw508d6qejxtdg4y5r3zarvaryvg6kdaj", "5210751e76e8199196d454941c45d1b3a323"},
            new [] { "tb1qqqqqp399et2xygdj5xreqhjjvcmzhxw4aywxecjdzew6hylgvsesrxh6hy", "0020000000c4a5cad46221b2a187905e5266362b99d5e91c6ce24d165dab93e86433"},
        };

        private static readonly string[] INVALID_ADDRESS = {
            "tc1qw508d6qejxtdg4y5r3zarvary0c5xw7kg3g4ty",
            "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t5",
            "BC13W508D6QEJXTDG4Y5R3ZARVARY0C5XW7KN40WF2",
            "bc1rw5uspcuh",
            "bc10w508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7kw5rljs90",
            "BC1QR508D6QEJXTDG4Y5R3ZARVARYV98GJ9P",
            "tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sL5k7",
            "tb1pw508d6qejxtdg4y5r3zarqfsj6c3",
            "tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3pjxtptv",
        };

        [Fact]
        public void CanDetectError()
        {
            Bech32Encoder bech = Encoders.Bech32("bc");
            byte wit;
            var ex = Assert.Throws<Bech32FormatException>(() => bech.Decode("bc1zw508e6qejxtdg4y5r3zarvaryvg6kdaj", out wit));
            Assert.Single(ex.ErrorIndexes);
            Assert.Equal(8, ex.ErrorIndexes[0]);

            ex = Assert.Throws<Bech32FormatException>(() => bech.Decode("bc1zw508e6qeextdg4y5r3zarvaryvg6kdaj", out wit));
            Assert.Equal(2, ex.ErrorIndexes.Length);
            Assert.Equal(8, ex.ErrorIndexes[0]);
            Assert.Equal(12, ex.ErrorIndexes[1]);
        }

        [Fact]
        public void ValidateValidChecksum()
        {
            foreach (string test in VALID_CHECKSUM)
            {
                Bech32Encoder bech = Bech32Encoder.ExtractEncoderFromString(test);
                int pos = test.LastIndexOf('1');
                string test2 = test.Substring(0, pos + 1) + ((test[pos + 1]) ^ 1) + test.Substring(pos + 2);
                Assert.Throws<FormatException>(() => bech.DecodeData(test2));
            }
        }

        private readonly Bech32Encoder bech32 = Encoders.Bech32("bc");
        private readonly Bech32Encoder tbech32 = Encoders.Bech32("tb");

        [Fact]
        public void ValidAddress()
        {
            foreach (string[] address in VALID_ADDRESS)
            {
                byte witVer;
                byte[] witProg;
                Bech32Encoder encoder = this.bech32;
                try
                {
                    witProg = this.bech32.Decode(address[0], out witVer);
                    encoder = this.bech32;
                }
                catch
                {
                    witProg = this.tbech32.Decode(address[0], out witVer);
                    encoder = this.tbech32;
                }

                byte[] scriptPubkey = Scriptpubkey(witVer, witProg);
                string hex = string.Join("", scriptPubkey.Select(x => x.ToString("x2")));
                Assert.Equal(hex, address[1]);

                string addr = encoder.Encode(witVer, witProg);
                Assert.Equal(address[0].ToLowerInvariant(), addr);
            }
        }

        [Fact]
        public void InvalidAddress()
        {
            foreach (string test in INVALID_ADDRESS)
            {
                byte witver;
                try
                {
                    this.bech32.Decode(test, out witver);
                }
                catch (FormatException) { }
                try
                {
                    this.tbech32.Decode(test, out witver);
                }
                catch (FormatException) { }
            }
        }

        private static byte[] Scriptpubkey(byte witver, byte[] witprog)
        {
            int v = witver > 0 ? witver + 0x50 : 0;
            return (new[] { (byte)v, (byte)witprog.Length }).Concat(witprog);
        }
    }
}