﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
	static GameObject _log;
	static GameObject _logEntry;

	[SerializeField] GameObject inventory;
	[SerializeField] float distance = 100;
	[SerializeField] public KeyCode interactKey = KeyCode.E;
	[SerializeField] public int interactLayerMask = -1;
	void Awake()
	{
		//FlowerLibrary.InitiateLibrary(); // OBS! Do not comment, SUPER important.
	}

	void Start()
	{
		// === Flower Segment ===
		// ==========================================
		// This is debugging
		List<string> flowerNames = FlowerLibrary.GetAllFlowerNames();
		string debugFlowerNames = "All flower names: ";
		foreach (string name in flowerNames)
		{
			debugFlowerNames += name + " |";
		}
		Debug.Log(debugFlowerNames);
		// ===========================================

		// === Load Log Stuff ===
		// WARNING! No other objects can have the names "Log" and "LogEntry" at the start of a scene.
		
        //Har avaktiverat logging tills vidare då det inte är konfirmerat add det skall finnas ett logg fönster i spelet
        //Om vi använder det senare föreslåt jag att vi använder eventManager för kommunikationen

        //_log = GameObject.Find("Log");
		//_logEntry = GameObject.Find("LogEntry");
		//if (_log == null || _logEntry == null)
		//{
		//	Debug.LogError("Missing 'Log' or 'LogEntry' GameObject");
		//}

		//Debug.Log("Log Object = " + _log.name);
		//Debug.Log("LogEntry Object = " + _logEntry.name);
	}

	void Update()
	{
		if (Input.GetKeyDown(interactKey))
		{
			RaycastHit collision;
			bool hit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * distance, out collision, distance);
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distance, Color.green, 1f);

			if (hit)
			{
                foreach(Interactable interactable in collision.transform.GetComponents<Interactable>())
                {
                    collision.collider.gameObject.GetComponent<Interactable>().Interact();
                }

			}
		}

	}

	public static void AddLogEntry(string entry)
	{
		Debug.Log("Test");
		GameObject newEntry = Instantiate<GameObject>(_logEntry, _log.transform);
		//RectTransform entryRect = newEntry.GetComponent<RectTransform>();
		try { newEntry.GetComponent<Text>().text = entry; }
		catch (UnityException e) { Debug.LogError("Missing Text component on 'LogEntry' object, leading to an error: \n" + e.Message); }
		Destroy(newEntry, 3f);
	}
}