using System.Security.Cryptography;

namespace JhpDataSystem
{
    public static class EncryptionHelper
    {
        public static PlainText Decrypt(this string textToEncrypt)
        {
            var encryptionKey = AppInstance.Instance.ApiAssets[Constants.ASSET_ADMIN_ENCRYPTIONKEY];
            var encryptedText = new EncryptedText(textToEncrypt);
            return encryptedText.Decrypt(encryptionKey);
        }

        public static EncryptedText Encrypt(this string textToEncrypt)
        {
            var encryptionKey = AppInstance.Instance.ApiAssets[Constants.ASSET_ADMIN_ENCRYPTIONKEY];
            var plainText = new PlainText(textToEncrypt);
            return plainText.Encrypt(encryptionKey);
        }

        public static EncryptedText Encrypt(this PlainText plainText, string encryptionKey)
        {
            var encryptedText = Crypto.Encrypt(plainText.Value, encryptionKey);
            return new EncryptedText(encryptedText);
        }

        public static PlainText Decrypt(this EncryptedText encryptedText, string encryptionKey)
        {
            var decryptedText = Crypto.Decrypt(encryptedText.Value, encryptionKey);
            return new PlainText(decryptedText);
        }
    }
}