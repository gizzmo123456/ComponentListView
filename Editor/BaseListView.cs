using System.Collections;
using System.Text.RegularExpressions;
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
	public int GroupCount => items.Count;

	public int GetItemCount( string groupName )
	{
		if ( !items.ContainsKey( groupName ) ) return 0;

		return items[ groupName ].Count;
	}

	protected void AddComponents ( string groupName, T[] components )
	{
		if ( components.Length == 0 ) return; // nothing to add!
											  // add the key if it does not exist
		if ( !items.ContainsKey( groupName ) )
			items.Add( groupName, new List<T>() );

		items[ groupName ].AddRange( components );

	}

	protected void AddComponent( string groupName, T component )
	{
		if ( !items.ContainsKey( groupName ) )
			items.Add( groupName, new List<T>() );

		items[ groupName ].Add( component );
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

	public void RemoveGroup ( string groupName )
	{
		if ( !items.ContainsKey( groupName ) )
			return;

		items.Remove( groupName );

		return;
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

	public void RemoveGroup( Scene sceneAsset )
	{
		RemoveGroup( sceneAsset.name );
	}


}

public class ScriptableObjectListView<T> : BaseListView<T> where T : ScriptableObject
{
	public void RetrieveScriptablesFromTypeName( )
	{
		// Find all assets of Type Name
		string[] assetGUIDs = AssetDatabase.FindAssets( string.Format( "t:{0}", typeof( T ).ToString() ) );

		foreach ( string guid in assetGUIDs )
		{
			string filePath = AssetDatabase.GUIDToAssetPath(guid);
			// remove the file from the path.
			//new Regex( @"\b([/][\w\s]*[.]{1}asset+)\b" );		// << Reg ex to get asset name.
			Regex rx = new Regex( @"([\w\d \t\/]*)([\/])" );    // Regex to get path without file name
			Match match = rx.Match( filePath );

			string path = match.Success ? match.ToString() : filePath;	// fall back on the full file path if theres an error.

			AddComponent( path, AssetDatabase.LoadAssetAtPath<T>(filePath) );
			
		}

	}

}

public class MonoBehaviourListView : BaseListView<System.Type>
{

	public void RetrieveMonoBehaviours<Group>() where Group : MonoBehaviour
	{

		System.Type type = typeof( Group );
		AddComponent( type.ToString(), type );

		foreach ( System.Type t in type.Assembly.GetTypes() )
			if ( t.IsSubclassOf( type ) && !t.IsAbstract )
				AddComponent(type.ToString(), t);
		
	}

}
