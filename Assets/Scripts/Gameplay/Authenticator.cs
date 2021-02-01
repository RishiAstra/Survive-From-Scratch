using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class Authenticator : MonoBehaviour
{
	public const string authFileName = "auth.txt";

	public static Authenticator main;

	public static Dictionary<string, byte[]> sessionIds;
	public static Dictionary<byte[], string> sessionIdsInverse;

	//private static System.Random random;//TODO: WARNING: NOT RANDOM
	private static RandomNumberGenerator random;
	
	// Start is called before the first frame update
	void Awake()
    {
		if (main != null) DestroyImmediate(gameObject);//there can only be 1, delete this one ASAP to avoid conflicts

		DontDestroyOnLoad(gameObject);

		random = new RNGCryptoServiceProvider();
		sessionIds = new Dictionary<string, byte[]>();
		sessionIdsInverse = new Dictionary<byte[], string>();
		//random = new System.Random(Random.Range(0, int.MaxValue));//i guess this is better than default seed?
    }

	public static string GetAccountPath(string username)
	{
		return Application.persistentDataPath + "\\Accounts\\" + username + "\\";
	}

	public static string GetAccountDataFile(string username, string datafile)
	{
		return GetAccountPath(username) + datafile;
	}

    public static bool MakeAccount(string username, string password)
	{
		Directory.CreateDirectory(GetAccountPath(username));
		string path = GetAccountDataFile(username, authFileName);
		//TODO: make separate create account page thingy
		FileStream fs = new FileStream(path, FileMode.Create);
		//byte[] usernameBytes = Encoding.ASCII.GetBytes(username);
		byte[] salt = new byte[16];
		random.GetBytes(salt);
		byte[] passHash = PBKDF2Hash(password, salt);

		//fs.WriteByte((byte)usernameBytes.Length);
		fs.WriteByte((byte)salt.Length);
		fs.WriteByte((byte)passHash.Length);
		//fs.Write(usernameBytes, 0, usernameBytes.Length);
		fs.Write(salt, 0, salt.Length);
		fs.Write(passHash, 0, passHash.Length);

		fs.Close();
		//OnLoginSuccess(username);
		print("New account with username: " + username);
		return true;
	}

    public static bool Login(string username, string password, out byte[] output)
	{
		string path = GetAccountDataFile(username, authFileName);
		if (File.Exists(path))
		{
			try
			{
				FileStream fs = new FileStream(path, FileMode.Open);
				byte[] salt = new byte[fs.ReadByte()];//16
				byte[] filePassHash = new byte[fs.ReadByte()];

				//fs.Read(usernameBytes, 0, usernameBytes.Length);
				fs.Read(salt, 0, salt.Length);
				fs.Read(filePassHash, 0, filePassHash.Length);
				byte[] passHash = PBKDF2Hash(password, salt);

				fs.Close();

				//bool same = true;
				if (passHash.Length != filePassHash.Length || passHash.Length < 8)//if it's smaller than 8, something's wrong
				{
					LoginFailed(username, "hash error", out output);
					return false;
					//same = false;
					//print("wront length");
				}
				else
				{
					for (int i = 0; i < passHash.Length; i++)
					{
						if (passHash[i] != filePassHash[i])
						{
							LoginFailed(username, "wrong username/password", out output);
							return false;
							//same = false;
							//print(i);
							//break;
						}
					}
				}

				///////////////////////Login Success////////////////////////////////////////////
				LoginSucceed(username, out output);
				return true;
				////////////////////////////////////////////////////////////////////////////////
				
			}
			catch (System.Exception e)
			{
				LoginFailed(username, "other error", out output);
				Debug.LogError("Can't log in! Error: \n" + e.Message + "\n" + e.StackTrace);
				return false;
			}
			//print(same + "||" + string.Join(",", salt) + "|" + string.Join(",", passHash) + "/" + string.Join(",", filePassHash));
		}
		else
		{
			LoginFailed(username, "account not found", out output);//can't login if this account doesn't exist
			return false;
		}
	}

	private static void LoginSucceed(string username, out byte[] output)
	{
		byte[] temp = new byte[16];
		random.GetBytes(temp);
		
		//if this temp is already used, get a new one
		while(sessionIdsInverse.TryGetValue(temp, out _))
		{
			random.GetBytes(temp);
			Debug.LogError("Random id already existed, Isn't this is statistically impossible?");
		}

		sessionIds.Add(username, temp);
		sessionIdsInverse.Add(temp, username);
		print("New session with username: " + username + ", id: " + string.Join(",", temp));
		output = null;//TODO: output a temporary code that will be used to verify this identity
	}

	private static void LoginFailed(string username, string message, out byte[] output)
	{
		output = null;
		print("Login failed");
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public static byte[] PBKDF2Hash(string input, byte[] salt)
	{
		// Generate the hash
		Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, 5000);
		return pbkdf2.GetBytes(20); //20 bytes length is 160 bits
	}
}
