using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;


public class SceneComponentListView<T> where T : MonoBehaviour
{

	// store all components in a dict where the key is the scene name.
	private Dictionary<string, List<T>> sceneCompoents = new Dictionary<string, List<T>>();

	public void GetComponentsFromObject( GameObject obj )
	{
		AddComponents( obj.scene.name, obj.GetComponentsInChildren<T>() );
	}

	public void GetComponentsFromScene( Scene scene )
	{
		GameObject[] gameObjs = scene.GetRootGameObjects();

		foreach ( GameObject go in gameObjs )
			GetComponentsFromObject( go );

	}

	private void AddComponents( string key, T[] components)
	{
		if ( components.Length == 0 ) return; // nothing to add!
		// add the key if it does not exist
		if ( !sceneCompoents.ContainsKey(key) )
			sceneCompoents.Add( key, new List<T>() );

		sceneCompoents[ key ].AddRange( components );

	}

	public T[] GetComponentsFromScene(string sceneName )
	{
		if ( !sceneCompoents.ContainsKey( sceneName ) )
			return new T[0];

		return sceneCompoents[ sceneName ].ToArray();
	}

	public string[] GetSceneNames()
	{
		return new List<string>( sceneCompoents.Keys ).ToArray();
	}

	public bool ContainsScene( string sceneName )
	{
		return sceneCompoents.ContainsKey( sceneName );
	}

	public void ClearSceneComponents ( string sceneName )
	{
		if ( !sceneCompoents.ContainsKey( sceneName ) ) return;

		sceneCompoents.Remove( sceneName );

	}

	public void ClearAllComponents()
	{
		sceneCompoents.Clear();
	}

}
