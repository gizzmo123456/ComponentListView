using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;

/// <summary>
/// Base class for list of items of type T. Requires somes from Retrieve item or component functions to be implermented
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseListView<T>
{
	private Dictionary<string, List<T>> items = new Dictionary<string, List<T>>();

	private void AddComponents ( string groupName, T[] components )
	{
		if ( components.Length == 0 ) return; // nothing to add!
											  // add the key if it does not exist
		if ( !items.ContainsKey( groupName ) )
			items.Add( groupName, new List<T>() );

		items[ groupName ].AddRange( components );

	}

	public T[] GetComponentsFromGroup ( string groupName )
	{
		if ( !items.ContainsKey( groupName ) )
			return new T[ 0 ];

		return items[ groupName ].ToArray();
	}

	public string[] GetGroupNames ()
	{
		return new List<string>( items.Keys ).ToArray();
	}

	public bool ContainsGroup ( string sceneName )
	{
		return items.ContainsKey( sceneName );
	}

	public void ClearGroupItems ( string sceneName )
	{
		if ( !items.ContainsKey( sceneName ) ) return;

		items.Remove( sceneName );

	}

	public void ClearAllItems ()
	{
		items.Clear();
	}


}

public class SceneComponentListView<T> : BaseListView<T> where T : MonoBehaviour
{

	// store all components in a dict where the key is the scene name.
	private Dictionary<string, List<T>> items = new Dictionary<string, List<T>>();

	public void RetrieveComponentsFromObject( GameObject obj )
	{
		AddComponents( obj.scene.name, obj.GetComponentsInChildren<T>() );
	}

	public void RetrieveComponentsFromScene( Scene scene )
	{
		GameObject[] gameObjs = scene.GetRootGameObjects();

		foreach ( GameObject go in gameObjs )
			RetrieveComponentsFromObject( go );

	}

	private void AddComponents( string key, T[] components)
	{
		if ( components.Length == 0 ) return; // nothing to add!
		// add the key if it does not exist
		if ( !items.ContainsKey(key) )
			items.Add( key, new List<T>() );

		items[ key ].AddRange( components );

	}

}
