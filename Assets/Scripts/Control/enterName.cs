﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Threading;
using System.IO;
using System.Text;

public class enterName : MonoBehaviour {
	public float showLoginFailTime = 1;

	public InputField nameHere;
	public InputField passField;
	public Text failText;

	private static bool finished = true;

	private System.Diagnostics.Stopwatch sw;
	private Coroutine failCoroutine;
	//private System.Random random;//TODO: WARNING: NOT RANDOM

	// Use this for initialization
	void Start () {
		//sw = new System.Diagnostics.Stopwatch();
		//random = new System.Random(Random.Range(0, int.MaxValue));
	}
	
	// Update is called once per frame
	void Update () {
		if (finished && Input.GetKeyDown(KeyCode.Return)) {
			tryLogin ();			
        }
		//if(hashThread != null)
  //      {
		//	if (hashThread.IsAlive)
		//	{

		//	}
		//	else
		//	{
		//		if (!finished)
		//		{
		//			print(sw.ElapsedMilliseconds + "|" + string.Join(",", output));
		//			finished = true;
		//		}
		//	}
		//}        
	}

	//private void OnLoginSuccess(string username)
 //   {
	//	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
 //   }

	public void TryRegister()
	{
		string username = nameHere.text;
		string password = passField.text;
		bool succeed = Authenticator.MakeAccount(username, password);
		if (!succeed)
		{
			if (failCoroutine != null) StopCoroutine(failCoroutine);
			failCoroutine = StartCoroutine(showLoginError());
		}
	}

	IEnumerator showLoginError()
	{
		failText.text = Authenticator.lastLoginFailMessage;
		yield return new WaitForSeconds(showLoginFailTime);
		failText.text = "";
	}

	public void tryLogin(){

		//sw.Restart();
		string username = nameHere.text;
		string password = passField.text;
		bool succeed = Authenticator.Login(username, password, out _);
		if (!succeed)
		{
			if(failCoroutine != null) StopCoroutine(failCoroutine);
			failCoroutine = StartCoroutine(showLoginError());
		}
		else
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
		//print(password);
		//string path = Application.persistentDataPath + "\\" + username + ".txt";
		//if (File.Exists(path))
		//{
		//	Authenticator.Login(username, password, out _);
		//	////string[] lines = File.ReadAllLines(path);
		//	////if (lines.Length != 3) Debug.LogError("wrong number of lines");
		//	////if (lines[0] != username) print("wrong name..?");
		//	////byte[] salt = Encoding.UTF8.GetBytes(lines[1]);
		//	////byte[] passHash = PBKDF2Hash(password, salt);
		//	////byte[] filePassHash = Encoding.UTF8.GetBytes(lines[2]);

		//	//FileStream fs = new FileStream(path, FileMode.Open);
		//	////byte[] usernameBytes = new byte[fs.ReadByte()]; //Encoding.ASCII.GetBytes(username);
		//	//byte[] salt = new byte[fs.ReadByte()];//16
		//	//byte[] filePassHash = new byte[fs.ReadByte()];

		//	////fs.Read(usernameBytes, 0, usernameBytes.Length);
		//	//fs.Read(salt, 0, salt.Length);
		//	//fs.Read(filePassHash, 0, filePassHash.Length);
		//	//byte[] passHash = PBKDF2Hash(password, salt);

		//	//fs.Close();

		//	//bool same = true;
		//	//if (passHash.Length != filePassHash.Length)
		//	//{
		//	//	same = false;
		//	//	print("wront length");
		//	//}
		//	//else
		//	//{
		//	//	for (int i = 0; i < passHash.Length; i++)
		//	//	{
		//	//		if (passHash[i] != filePassHash[i])
		//	//		{
		//	//			same = false;
		//	//			print(i);
		//	//			break;
		//	//		}
		//	//	}
		//	//}
		//	//if (same) OnLoginSuccess(username);
		//	//else print("Login failed");
		//	//print(same + "||" + string.Join(",", salt) + "|" + string.Join(",", passHash) + "/" + string.Join(",", filePassHash));
		//}
		//else
		//{
		//	Authenticator.MakeAccount(username, password);
		//	////TODO: make separate create account page thingy
		//	//FileStream fs = new FileStream(path, FileMode.Create);
		//	////byte[] usernameBytes = Encoding.ASCII.GetBytes(username);
		//	//byte[] salt = new byte[16];
		//	//random.NextBytes(salt);
		//	//byte[] passHash = PBKDF2Hash(password, salt);

		//	////fs.WriteByte((byte)usernameBytes.Length);
		//	//fs.WriteByte((byte)salt.Length);
		//	//fs.WriteByte((byte)passHash.Length);
		//	////fs.Write(usernameBytes, 0, usernameBytes.Length);
		//	//fs.Write(salt, 0, salt.Length);
		//	//fs.Write(passHash, 0, passHash.Length);

		//	//fs.Close();
		//	//OnLoginSuccess(username);
		//	////string[] lines = new string[3];
		//	////lines[0] = username;
		//	////lines[1] = Encoding.UTF8.GetString(salt);
		//	////lines[2] = Encoding.GetString(passHash);

		//	////File.WriteAllLines(path, lines);

		//	////print(string.Join(",", PBKDF2Hash(password, salt)));
		//	////print("made new account " + string.Join(",", salt) + "|" + string.Join(",", passHash));
		//}
		////input = nameHere.text;
		////salt = System.Text.Encoding.ASCII.GetBytes("kjnfleajlfa");
		////finished = false;
		////hashThread = new Thread(new ThreadStart(Hash));
		////hashThread.Start();
		////int times = 0;
		////print(string.Join(",", PBKDF2Hash(nameHere.text, System.Text.Encoding.ASCII.GetBytes("kjnfleajlfa"))));
		////while (sw.ElapsedMilliseconds < 500)
		////         {
		////	PBKDF2Hash(nameHere.text, System.Text.Encoding.ASCII.GetBytes("kjnfleajlfa"));
		////	times++;
		////}
		////print(sw.ElapsedMilliseconds);
		////PhotonNetwork.playerName = nameHere.text;
		////SceneManager.LoadScene (1);
	}

	//public static void Hash()
	//{
	//	// Generate the hash
	//	Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, 10000);
	//	output = pbkdf2.GetBytes(20);
	//}

	//public static byte[] PBKDF2Hash(string input, byte[] salt)
	//{
	//	// Generate the hash
	//	Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, 5000);
	//	return pbkdf2.GetBytes(20); //20 bytes length is 160 bits
	//}
}