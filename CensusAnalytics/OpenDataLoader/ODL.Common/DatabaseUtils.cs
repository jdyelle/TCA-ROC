using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ODL.Common
{
    public class DatabaseUtils
    {
        public class Postgres
        {
            public static Npgsql.NpgsqlConnection ConnectToPostGRES(DBConnectionDetails dbConnectionInfo)
            {
                Npgsql.NpgsqlConnectionStringBuilder _connString = new Npgsql.NpgsqlConnectionStringBuilder();
                _connString.Username = dbConnectionInfo.DBUsername;
                _connString.Password = dbConnectionInfo.DBPassword;
                _connString.Database = dbConnectionInfo.DBCatalog;
                _connString.Host = dbConnectionInfo.DBServer;
                Npgsql.NpgsqlConnection _dbConnection = new Npgsql.NpgsqlConnection(_connString.ConnectionString);
                _dbConnection.Open();

                return _dbConnection;
            }
        }

        public static void Save(DBConnectionDetails DBConnection)
        {
            String _Serialized = Newtonsoft.Json.JsonConvert.SerializeObject(DBConnection);
            File.WriteAllText("./DBConnectionConfig.json", _Serialized);
        }

        public static DBConnectionDetails Load()
        {
            DBConnectionDetails _ReturnObject = null;
            if (File.Exists("./DBConnectionConfig.json"))
            {
                String _LoadedFile = Encoding.UTF8.GetString(File.ReadAllBytes("./DBConnectionConfig.json"));
                _ReturnObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DBConnectionDetails>(_LoadedFile);
            }
            else
            {
                _ReturnObject = new DBConnectionDetails();
            }
            return _ReturnObject;
        }

        private static String Encrypt(String InputString, String Key, String IV)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {

                //A better way to do this would be to use randomized keys but we need reversible storage for the json
                //myRijndael.GenerateKey();
                //myRijndael.GenerateIV();

                // Check arguments.
                if (InputString == null || InputString.Length <= 0) throw new ArgumentNullException("plainText");
                if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");
                byte[] encryptedBytes;

                //Temp variables that conform to key/IV lengths
                String _tempKey = (Key + "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ").Substring(0, 32);
                String _tempIV = (IV + "12345678901234567890").Substring(0, 16);

                // Create an RijndaelManaged object
                // with the specified key and IV.
                using (RijndaelManaged rijAlg = new RijndaelManaged())
                {
                    rijAlg.Key = Encoding.UTF8.GetBytes(_tempKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(_tempIV);

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {

                                //Write all data to the stream.
                                swEncrypt.Write(InputString);
                            }
                            encryptedBytes = msEncrypt.ToArray();
                        }
                    }
                }
                // Return the encrypted bytes from the memory stream.
                return WebUtility.UrlEncode(Convert.ToBase64String(encryptedBytes));
            }
        }

        private static String Decrypt(String InputString, String Key, String IV)
        {

            // Check arguments.
            if (InputString == null || InputString.Length <= 0) throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            String DecryptedText = String.Empty;

            //Temp variables that conform to key/IV lengths
            String _tempKey = (Key + "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ").Substring(0, 32);
            String _tempIV = (IV + "12345678901234567890").Substring(0, 16);

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Encoding.UTF8.GetBytes(_tempKey);
                rijAlg.IV = Encoding.UTF8.GetBytes(_tempIV);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(WebUtility.UrlDecode(InputString))))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            DecryptedText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return DecryptedText;
        }
    }
}
