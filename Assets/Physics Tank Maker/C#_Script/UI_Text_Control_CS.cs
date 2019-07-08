﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{

	class TextProp
	{
		public string textString;
		public Color textColor;
		public float displayTime;

		public TextProp (string newString, Color newColor, float newTime)
		{
			textString = newString;
			textColor = newColor;
			displayTime = newTime;
		}
	}

	public class UI_Text_Control_CS : MonoBehaviour
	{

		public float Fade_In_Time = 1.0f;
		public float Fade_Out_Time = 1.0f;

		Text thisText;
		List < TextProp > textList = new List < TextProp > ();
		bool isDisplaying = false;
		bool isWaiting = false;

		void Start ()
		{
			thisText = GetComponent < Text > ();
		}

		public void Receive_Text (string tempString, Color tempColor, float tempTime)
		{ // Called from "Event_Controller_CS".
			TextProp newTextProp = new TextProp (tempString, tempColor, tempTime);
			textList.Add (newTextProp);
			if (isDisplaying) {
				isWaiting = true;
			}
		}

		void Update ()
		{
			if (textList.Count > 0 && isDisplaying == false) {
				isDisplaying = true;
				thisText.text = textList [0].textString;
				thisText.color = textList [0].textColor;
				StartCoroutine ("Fade_In");
			}
		}

		IEnumerator Fade_In ()
		{
			float count = 0.0f;
			Color tempColor = thisText.color;
			while (count < Fade_In_Time) {
				tempColor.a = Mathf.Lerp (0.0f, 1.0f, count / Fade_In_Time);
				thisText.color = tempColor;
				count += Time.deltaTime;
				yield return 0;
			}
			tempColor.a = 1.0f;
			thisText.color = tempColor;
			StartCoroutine ("Keep_Displaying");
		}

		IEnumerator Keep_Displaying ()
		{
			float count = 0.0f;
			while (count < textList [0].displayTime) {
				if (isWaiting) {
					isWaiting = false;
					break;
				} else {
					count += Time.deltaTime;
					yield return 0;
				}
			}
			StartCoroutine ("Fade_Out");
		}

		IEnumerator Fade_Out ()
		{
			float count = 0.0f;
			Color tempColor = thisText.color;
			while (count < Fade_Out_Time) {
				tempColor.a = Mathf.Lerp (1.0f, 0.0f, count / Fade_Out_Time);
				thisText.color = tempColor;
				count += Time.deltaTime;
				yield return 0;
			}
			tempColor.a = 0.0f;
			thisText.color = tempColor;
			textList.RemoveAt (0);
			isDisplaying = false;
		}

	}

}