using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Words : MonoBehaviour {
	public static event Action <String> OnWordSetChar;

	private String[] _words = {"BONUS", "HELLO", "DINOSAUR"};// 0 - Available, 1 - Active, 2 - Used
	private int[] _wordsAvailable = new int[3];
	private String originalWord;
	private String currentWord;
	private int wordId;
	private int charId;

	private const Char SecretSign = '*';

	// Use this for initialization
	void Start () {
		int state;
		for (var i = 0; i < _words.Length; i++) {
			state = PlayerPrefs.GetInt ("Words_Available" + i, 0);
			_wordsAvailable [i] = state;
			if (state == 1) {
				originalWord = _words [i];
			}
		}

		if (originalWord == null) {
			wordId = 0;
			SetNextWord ();
		} else {
			currentWord = PlayerPrefs.GetString ("Words_CurrentWord", "");
		}
			
		SetNextChar ();
	}

	private void OnEnable()
	{
		BonusChar.OnBonusGot += OnBonusGot;
	}

	//--------------------------------------------------------------
	//							CHARS
	//--------------------------------------------------------------

	private void SetNextChar() {
		charId = GetNextCharId();
		GameEvents.Send (OnWordSetChar, originalWord.Substring(charId,1));
	}

	private void OnBonusGot()
	{
		ReplaceCharInString(currentWord, charId, originalWord[charId]);
		if (CheckCurrWord ()) {
			SaveCurrentWord();
			wordId = GetNextWordId ();
			SetNextWord ();
		} else
			SetNextChar ();
	}

	private int GetNextCharId() {
		int id = Random.Range (0, originalWord.Length-1);
		if (currentWord [id] != SecretSign) {
			for (int i = 0; i < currentWord.Length; i++) {
				++id;
				if (id >= currentWord.Length - 1)
					id = 0;
				if (currentWord [id] != SecretSign) {
					return id;
				}
			}
		}
		return id;
	}

	//--------------------------------------------------------------
	//							WORDS
	//--------------------------------------------------------------

	private void PrepareNewWord (string originalWord)
	{
		for(int i = 0; i < originalWord.Length; i++) {
			currentWord.Insert(i, SecretSign.ToString());
		}
	}

	private void SetNextWord ()
	{
		originalWord = _words [wordId];
		_wordsAvailable [wordId] = 1;
		PlayerPrefs.GetInt ("Words_Available" + wordId, 1);
		PrepareNewWord (originalWord);
	}

	private void SaveCurrentWord ()
	{
		_wordsAvailable [wordId] = 2;
		PlayerPrefs.GetInt ("Words_Available" + wordId, 2);
	}

	private bool CheckCurrWord ()
	{
		for (int i = 0; i < currentWord.Length; i++) {
			if (currentWord [i] == SecretSign) {
				return false;
			}
		}
		return true;
	}

	private int GetNextWordId() {
		int id = Random.Range (0, _wordsAvailable.Length-1);
		if (_wordsAvailable [id] != 0) {
			for (int i = 0; i < _wordsAvailable.Length; i++) {
				++id;
				if (id >= _wordsAvailable.Length - 1)
					id = 0;
				if (_wordsAvailable [id] == 0) {
					return id;
				}
			}
		}
		return id;
	}
	
	public void ReplaceCharInString(ref String str, int index, Char newSymb)
	{
		str = str.Remove(index, 1).Insert(index, newSymb.ToString());
	}
 
// либо:
	public String ReplaceCharInString(String str, int index, Char newSymb)
	{
		return str.Remove(index, 1).Insert(index, newSymb.ToString());
	}
}
