﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
	const string INPUT_INVENTORY = "Inventory";
	[SerializeField] List<GameObject> _bookmarks = new List<GameObject>();
	[SerializeField] List<PageLoader> _flowerPages = new List<PageLoader>();
	[SerializeField] List<PageLoader> _lorePages = new List<PageLoader>();
	[SerializeField] int _flowerOrganizerId;
	[SerializeField] RectTransform _bookmarkTemplate;
	[SerializeField] GameObject _emptyPageTemplate;
	//[SerializeField] GameObject _nextPageTemplate;
	[SerializeField] Vector2[] _bookmarkPositions;
	int _currentBookmark = 0;
	int _currentPage = 0;
	[SerializeField] GameObject _book;
	[SerializeField] GameObject _prevPage;
	[SerializeField] GameObject _nextPage;

	void Awake()
	{
		SetupExtraBookmarks();
		SetupPage(_flowerOrganizerId, _flowerPages);
		foreach (PageLoader page in _flowerPages)
		{
			
		}

		/*
		if (_book == null)
		{
			_book = transform.Find("Book").gameObject; //slow
		}
		_book.SetActive(false); */
	}

	void Update()
	{
		if (Input.GetButtonDown(INPUT_INVENTORY))
		{
			Debug.Log("Inventory button");
			_book.SetActive(!_book.activeSelf);
			if (_book.activeSelf)
			{
				CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
			}
			else
			{
				CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
			}
		}
	}

	void SetupPage(int pageParentID, List<PageLoader> pages)
	{
		for(int i = 0; i<pages.Count; i++)
		{
			GameObject page = Instantiate<GameObject>(pages[i].gameObject, _bookmarks[pageParentID].transform);
			if(i == _currentPage || i == _currentPage+1)
			{
				page.SetActive(true);
			}
			else
			{
				page.SetActive(false);
			}
			pages[i] = page.GetComponent<PageLoader>();
			
			//Flower flower = pages[i].CreateThisFlower();
			//FlowerLibrary.AddFlower(flower.Name, 0);
		}
		if(pages.Count % 2 == 1)
		{
			GameObject emptyPage = Instantiate<GameObject>(_emptyPageTemplate, _bookmarks[pageParentID].transform);
			emptyPage.gameObject.SetActive(false);
			pages.Add(emptyPage.GetComponent<PageLoader>());
		}

	}
	void SetupExtraBookmarks()
	{
		for(int i = 0; i < _bookmarks.Count; i++)
		{
			GameObject bookmark = _bookmarks[i];
			_bookmarks[i] = Instantiate<GameObject>(bookmark, _book.transform);
			_bookmarks[i].transform.SetAsFirstSibling();
			GameObject bookmarkObject = Instantiate<GameObject>(_bookmarkTemplate.gameObject, _book.transform);
			RectTransform bookmarkTransform = bookmarkObject.GetComponent<RectTransform>();
			bookmarkTransform.localPosition += Vector3.right*40*i;
			//bookmarkObject.transform.position = new Vector3(_bookmarkPositions[i].x, _bookmarkPositions[i].y, 0.0f);
		}	
	}
	public void ChangePage(int change)
	{
		_currentPage = ChangeCurrentPage(_flowerPages.Count, change);
		ChangePage();
	}
	void ChangePage()
	{
		Debug.Log("Current page is: " + _currentPage.ToString());
		for (int i = 0; i < _flowerPages.Count; i++)
		{
			if (i == _currentPage || i == _currentPage + 1)
			{
				Debug.Log("Set page true");
				_flowerPages[i].gameObject.SetActive(true);
			}
			else
			{
				Debug.Log("Set page false");
				_flowerPages[i].gameObject.SetActive(false);
			}
		}

	}
	int ChangeCurrentPage(int pageCount, int change)
	{
		Debug.Log("Change page");
		if ((pageCount % 2) == 0)
		{
			int curPag = (_currentPage + 2 * change) % pageCount;
			curPag = curPag < 0 ? curPag + pageCount : curPag;

			return curPag;
		}
		else
		{
			Debug.Log("Is Odd PageCount");
			return ((_currentPage + 2 * change) % pageCount);
		}
	}

	void ToBookmark()
	{

	}
}
