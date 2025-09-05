using UnityEngine;

public static class InputManager
{
    public static KeyCode GetKey(string keyName, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(keyName))
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyName));
        }
        else
        {
            return defaultKey;
        }
    }

    public static void SetKey(string keyName, KeyCode newKey)
    {
        PlayerPrefs.SetString(keyName, newKey.ToString());
        PlayerPrefs.Save();
    }
}