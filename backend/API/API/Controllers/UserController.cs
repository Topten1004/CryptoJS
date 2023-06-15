using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using System.Security.Cryptography;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult UserLogin([FromBody] UserLogin login)
        {

            string password = DecryptStringAES(login.Password);

            if (login.UserId != null && password != null)
            {
                return Ok(password);
            }
            else
            {
                return BadRequest();
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string DecryptStringAES(string encryptedValue)
        {
            var keybytes = Encoding.UTF8.GetBytes("8056483646328763");
            var iv = Encoding.UTF8.GetBytes("8056483646328763");
            //DECRYPT FROM CRIPTOJS
            var encrypted = Convert.FromBase64String(encryptedValue);
            var decryptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return decryptedFromJavascript;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
