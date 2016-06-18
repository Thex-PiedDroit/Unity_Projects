
using UnityEngine;
using System;
using System.Collections;



public static class ToolKit
{
	public static Vector3 TopLeftWorldToScreenPoint(Vector3 tWorldPoint)
	{
		return DownToTopCoordinate(Camera.main.WorldToScreenPoint(tWorldPoint));
	}

	public static Vector3 DownToTopCoordinate(Vector3 tBottomLeftSpacePoint)
	{
		tBottomLeftSpacePoint.y -= Screen.height;
		tBottomLeftSpacePoint.y *= -1.0f;

		return tBottomLeftSpacePoint;
	}

	static public T ToEnum<T>(string pToParse, T eDefaultValue) where T : struct, IConvertible
	{
		if (!typeof(T).IsEnum)
			throw new ArgumentException("T must be an enumerated type");

		if (string.IsNullOrEmpty(pToParse))
			return eDefaultValue;

		foreach (T eValue in Enum.GetValues(typeof(T)))
		{
			if (eValue.ToString().ToLower().Equals(pToParse.Trim().ToLower()))
				return eValue;
		}

		Debug.LogWarning("Could not find matching enum for \"" + pToParse + " in enum \"" + typeof(T).ToString() + "\"!");
		return eDefaultValue;
	}

	static public Color SetAlpha(Color tColor, float fAlpha)
	{
		tColor.a = Mathf.Clamp(fAlpha, 0.0f, 1.0f);
		return tColor;
	}

	static public Sprite GetSpriteFromAtlas(string pSpriteName, string pAtlasName)
	{
		Sprite[] pAllSprites = Resources.LoadAll<Sprite>("Sprites/");

		for (int i = 0; i < pAllSprites.Length; i++)
		{
			if (pAllSprites[i].texture.name == pAtlasName && pAllSprites[i].name == pSpriteName)
				return pAllSprites[i];
		}

		Debug.LogError("Could not find sprite \"" + pSpriteName + "\" from atlas \"" + pAtlasName + "\"!");
		return null;
	}
}
