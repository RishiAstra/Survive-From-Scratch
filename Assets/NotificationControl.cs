using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationControl : MonoBehaviour
{
	public static NotificationControl main;

	public List<Notification> notifications;
	public TMP_Text messageText;
	public Animator notificationAnimator;
	// Start is called before the first frame update
	void Start()
	{
		if (main != null) Debug.LogError("Two NotificationControl");
		main = this;
	}

	public void AddNotification(Notification n)
	{
		notifications.Add(n);
	}

	// Update is called once per frame
	void Update()
	{
		//if not triggering notification and not showing notification, remove the previous notification and show the next one
		if (!notificationAnimator.GetBool("Notify") && notificationAnimator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
		{

			if (notifications.Count > 0)
			{
				Notification n = notifications[0];
				messageText.text = n.message;
				notificationAnimator.SetTrigger("Notify");
			}
		}
	}

	public void DoneWithNotification()
	{
		if (notifications.Count > 0) notifications.RemoveAt(0);
	}
}

[System.Serializable]
public class Notification
{
	public string message;
}
