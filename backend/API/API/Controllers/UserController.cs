using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using System.Security.Cryptography;
using API.ViewModels;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDBContext _context;
        public int MaxDays = 5;
        public UserController(ILogger<UserController> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> UserLoginAsync([FromBody] UserLogin login)
        {
            if (!_context.Users.Any())
            {
                UserModel user = new UserModel();
                user.UserName = "David" + "Ray";
                user.Password = login.Password;
                user.UserId = login.UserId;
                user.Lastmodifieddate = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var result = _context.Users.Where( x => x.Password  == login.Password ).FirstOrDefault();

            if (result != null)
            {
                TimeSpan duration = DateTime.UtcNow- result.Lastmodifieddate;
                int numberOfDays = duration.Days;

                LoginResponse response = new LoginResponse();

                if (numberOfDays > (90 - MaxDays))
                {
                    response.message = "Password need update";
                    response.status = 1;
                }

                response.status = 0;
                response.message = "Login success";

                string password = DecryptStringAES(login.Password);

                return Ok(response);
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
