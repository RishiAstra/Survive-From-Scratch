using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IQuest
{
	/// <summary>
	/// call this when a location is reached
	/// </summary>
	/// <param name="location">the location reached</param>
    void OnLocationReached(string location);
	/// <summary>
	/// call this when an entity is killed
	/// </summary>
	/// <param name="type">the type of thing killed</param>
	/// <param name="killedBy">the thing that killed it, this can be null</param>
    void OnEntityKilled(string type, Abilities killedBy);
	/// <summary>
	/// call this when an entity is damaged
	/// </summary>
	/// <param name="type">the type of entity damaged</param>
	/// <param name="dmgBy">the thing that damaged it</param>
	/// <param name="dmgAmount">the amount of damage dealt</param>
    void OnEntityDamaged(string type, Abilities dmgBy, float dmgAmount);

	/// <summary>
	/// Get the progress of this mission. Useful for showing quest progress bars.
	/// </summary>
	/// <returns>the progress in the range [0, 1]</returns>
	float GetProgress();
	/// <summary>
	/// Get a description of the mission, possibly including the current progress
	/// </summary>
	/// <returns>the description</returns>
	string GetDescription();
	/// <summary>
	/// Has this mision been completed?
	/// </summary>
	/// <returns>true if this mission has been completed</returns>
	bool IsFinished();

	string GetQuestName();
}
