using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stellar;
using Stellar.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Examples
{
    using UrlContent = KeyValuePair<string, string>;
    class Program
    {
        static string horizon_url = "https://horizon-testnet.stellar.org/";
        static string network_passphrase = "Test SDF Network ; September 2015";

        static void Main(string[] args)
        {
            Stellar.Network.CurrentNetwork = network_passphrase;

            // seed from account on the testnetwork
            var myKeyPair = KeyPair.FromSeed("SDMJOANF6CDRHWVG3N6I34VHFEWD2KK5I5SPGFU5FDB6SY5FJNXTWN24");
            Account myAccount = new Account(myKeyPair, GetSequence(myKeyPair.Address));

            var randomAccountKeyPair = CreateRandomAccount(myAccount, 1000 * Stellar.One.Value);

            Payment(myKeyPair, randomAccountKeyPair, 10 * Stellar.One.Value);

            // Wait for input to prevent the cmd window from closing
            Console.Read();
        }

        static string GetResult(string msg)
        {
            using (var client = new HttpClient())
            {
                string response = client.GetStringAsync(horizon_url + WebUtility.UrlEncode(msg)).Result;
                return response;
            }
        }

        static HttpResponseMessage PostResult(string tx)
        {
            using (var client = new HttpClient())
            {
                var body = new List<UrlContent>();
                body.Add(new UrlContent("tx", tx));
                var formUrlEncodedContent = new FormUrlEncodedContent(body);
                return client.PostAsync(horizon_url + "transactions", formUrlEncodedContent).Result;
            }
        }

        private static long GetSequence(string address)
        {
            using (var client = new HttpClient())
            {
                string response = client.GetStringAsync(horizon_url + "accounts/" + address).Result;
                var json = JObject.Parse(response);
                return (long)json["sequence"];
            }
        }

        static KeyPair CreateRandomAccount(Account source, long nativeAmount)
        {
            var dest = KeyPair.Random();

            var operation =
                new CreateAccountOperation.Builder(dest, nativeAmount)
                .SetSourceAccount(source.KeyPair)
                .Build();

            source.IncrementSequenceNumber();

            Stellar.Transaction transaction =
                new Stellar.Transaction.Builder(source)
                .AddOperation(operation)
                .Build();

            transaction.Sign(source.KeyPair);

            var tx = transaction.ToEnvelopeXdrBase64();

            var response = PostResult(tx);

            Console.WriteLine("response:" + response.ReasonPhrase);
            Console.WriteLine(dest.Address);
            Console.WriteLine(dest.Seed);

            return dest;
        }

        private static void DecodeTransactionResult(string result)
        {
            var bytes = Convert.FromBase64String(result);
            var reader = new Stellar.Generated.ByteReader(bytes);
            var txResult = Stellar.Generated.TransactionResult.Decode(reader);

        }

        private static void DecodeTxFee(string result)
        {
            var bytes = Convert.FromBase64String(result);
            var reader = new Stellar.Generated.ByteReader(bytes);
            var txResult = Stellar.Generated.LedgerEntryChanges.Decode(reader);

        }

        static void Payment(KeyPair from, KeyPair to, long amount)
        {
            Account source = new Account(from, GetSequence(from.Address));

            // load asset
            Stellar.Generated.Asset asset = Stellar.Asset.Native();

            var operation =
                new PaymentOperation.Builder(to, asset, amount)
                .SetSourceAccount(from)
                .Build();

            source.IncrementSequenceNumber();

            Stellar.Transaction transaction =
                new Stellar.Transaction.Builder(source)
                .AddOperation(operation)
                .Build();

            transaction.Sign(source.KeyPair);

            var tx = transaction.ToEnvelopeXdrBase64();

            var response = PostResult(tx);

            Console.WriteLine(response.ReasonPhrase);
        }

        private static Stellar.Generated.Asset GetAsset(KeyPair master, string assetCode)
        {
            return new Stellar.Generated.Asset
            {
                Discriminant = AssetType.Create(AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4),
                AlphaNum4 = new Stellar.Generated.Asset.AssetAlphaNum4
                {
                    AssetCode = ASCIIEncoding.ASCII.GetBytes(assetCode),
                    Issuer = master.AccountId
                }
            };
        }
    }
}