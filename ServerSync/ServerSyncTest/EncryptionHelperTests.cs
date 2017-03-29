//using JhpDataSystem;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using ServerSync;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ServerSync.Tests
//{
//    [TestClass]
//    public class EncryptionHelperTests
//    {
//        [TestMethod]
//        public void EncryptTest()
//        {
//            var encryptionKey = "build.capacity.strengthen.health.systems.advocacy.policy.development.perf_improvement";
//            var textToEncrypt = "Jhpiego is an international, non-profit health organization affiliated with The Johns Hopkins University. For 40 years and in over 155 countries, Jhpiego has worked to prevent the needless deaths of women and their families";
//            var plainText = new PlainText(textToEncrypt);
//            var encryptedText = plainText.Encrypt(encryptionKey);
//            Assert.IsNotNull(encryptedText);
//            Assert.IsTrue(!string.IsNullOrWhiteSpace(encryptedText.Value));
//        }

//        [TestMethod()]
//        public void DecryptTest()
//        {
//            var encryptionKey = "plans to death star";
//            var textToEncrypt = "Jhpiego is an international, non-profit health organization affiliated with The Johns Hopkins University. For 40 years and in over 155 countries, Jhpiego has worked to prevent the needless deaths of women and their families";
//            var plainText = new PlainText(textToEncrypt);
//            var encryptedText = plainText.Encrypt(encryptionKey);
//            var decryptedText = encryptedText.Decrypt(encryptionKey);
//            Assert.IsNotNull(decryptedText);
//            Assert.AreEqual(textToEncrypt, decryptedText.Value);
//        }
//    }
//}