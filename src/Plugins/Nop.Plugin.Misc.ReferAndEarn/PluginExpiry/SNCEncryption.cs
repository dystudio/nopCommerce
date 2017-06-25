using System;
using System.Security.Cryptography;
using System.Text;

namespace Nop.Plugin.Misc.ReferAndEarn.PluginExpiry
{
	public static class SNCEncryption
	{
		private const string SECURITY_KEY = "6D7SuPSoEA7ec66U-referandearn";

		public static string Encrypt(string toEncrypt, bool useHashing)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
			string s = "6D7SuPSoEA7ec66U-referandearn";
			byte[] key;
			if (useHashing)
			{
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				key = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(s));
				mD5CryptoServiceProvider.Clear();
			}
			else
			{
				key = Encoding.UTF8.GetBytes(s);
			}
			TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			tripleDESCryptoServiceProvider.Key = key;
			tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
			tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			tripleDESCryptoServiceProvider.Clear();
			return Convert.ToBase64String(array, 0, array.Length);
		}

		public static string Decrypt(string cipherString, bool useHashing)
		{
			byte[] array = Convert.FromBase64String(cipherString);
			string s = "6D7SuPSoEA7ec66U-referandearn";
			byte[] key;
			if (useHashing)
			{
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				key = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(s));
				mD5CryptoServiceProvider.Clear();
			}
			else
			{
				key = Encoding.UTF8.GetBytes(s);
			}
			TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			tripleDESCryptoServiceProvider.Key = key;
			tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
			tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor();
			byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			tripleDESCryptoServiceProvider.Clear();
			return Encoding.UTF8.GetString(bytes);
		}

		public static bool IsBase64(this string base64String)
		{
			bool flag = base64String == null || base64String.Length == 0 || base64String.Length % 4 != 0 || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				try
				{
					SNCEncryption.Decrypt(base64String, true);
					result = true;
					return result;
				}
				catch (Exception var_2_61)
				{
				}
				result = false;
			}
			return result;
		}
	}
}
