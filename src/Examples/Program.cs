using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasar;
using Quasar.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    class Program
    {
        static string url = "{INSERT_NODE_URL(not the horizon url)}/tx?blob=";

        static string GetResult(string msg)
        {
            using (var client = new HttpClient())
            {
                string response = client.GetStringAsync(url + WebUtility.UrlEncode(msg)).Result;
                return response;
            }
        }

        static KeyPair CreateRandomAccount(Account source, long nativeAmount)
        {
            var master = KeyPair.Master();
            var dest = KeyPair.Random();

            var operation =
                new CreateAccountOperation.Builder(dest, nativeAmount)
                .SetSourceAccount(source.KeyPair)
                .Build();

            source.IncrementSequenceNumber();

            Quasar.Transaction transaction =
                new Quasar.Transaction.Builder(source)
                .AddOperation(operation)
                .Build();

            transaction.Sign(source.KeyPair);

            string message = transaction.ToEnvelopeXdrBase64();

            string response = GetResult(message);

            Console.WriteLine(response);
            Console.WriteLine(dest.Address);
            Console.WriteLine(dest.Seed);

            return dest;
        }

        private static void DecodeTransactionResult(string result)
        {
            var bytes = Convert.FromBase64String(result);
            var reader = new Quasar.Generated.ByteReader(bytes);
            var txResult = Quasar.Generated.TransactionResult.Decode(reader);

        }

        private static void DecodeTxFee(string result)
        {
            var bytes = Convert.FromBase64String(result);
            var reader = new Quasar.Generated.ByteReader(bytes);
            var txResult = Quasar.Generated.LedgerEntryChanges.Decode(reader);

        }

        private static void CreateAccountSample()
        {
            // get master
            var master = KeyPair.Master();
            Account masterSource = new Account(master, 1);

            // create account
            var green = CreateRandomAccount(masterSource, 1000);

            Console.Read();
        }

        static void PaymentFromMaster(KeyPair kp, long amount)
        {
            // get master
            var master = KeyPair.Master();
            Account masterSource = new Account(master, 5);

            // load asset
            Quasar.Generated.Asset asset = Quasar.Asset.Native();

            var operation =
                new PaymentOperation.Builder(kp, asset, amount)
                .SetSourceAccount(masterSource.KeyPair)
                .Build();

            masterSource.IncrementSequenceNumber();

            Quasar.Transaction transaction =
                new Quasar.Transaction.Builder(masterSource)
                .AddOperation(operation)
                .Build();

            transaction.Sign(masterSource.KeyPair);

            string message = transaction.ToEnvelopeXdrBase64();

            string response = GetResult(message);

            Console.WriteLine(response);
        }

        static void Main(string[] args)
        {
            Quasar.Network.CurrentNetwork = "...";

            var accountKP = KeyPair.FromAddress("GCP2KAN7Y5DKFRYRS5RIMUGEYI246VAB2PLGJRFFJBY3JSCMA6YOOL5P");
            PaymentFromMaster(accountKP, 10 * Quasar.One.Value);

            Console.Read();
        }

        private static Quasar.Generated.Asset GetAsset(KeyPair master, string assetCode)
        {
            return new Quasar.Generated.Asset
            {
                Discriminant = AssetType.Create(AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4),
                AlphaNum4 = new Quasar.Generated.Asset.AssetAlphaNum4
                {
                    AssetCode = ASCIIEncoding.ASCII.GetBytes(assetCode),
                    Issuer = master.AccountId
                }
            };
        }
    }
}