using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data;
using UnityEngine;

namespace TowerDefense.Game
{
	/// <summary>
	/// The data store for TD
	/// </summary>
	public sealed class GameDataStore : GameDataStoreBase
	{
		private GameStateLoader _loader = new GameStateLoader();
		
		/// <summary>
		/// Outputs to debug
		/// </summary>
		public override void PreSave()
		{
			Debug.Log("[GAME] Saving Game");
		}

		/// <summary>
		/// Outputs to debug
		/// </summary>
		public override void PostLoad()
		{
			Debug.Log("[GAME] Loaded Game");
		}

		/// <summary>
		/// Marks a level complete
		/// </summary>
		/// <param name="levelId">The levelId to mark as complete</param>
		/// <param name="starsEarned">Stars earned</param>
		public void CompleteLevel(string levelId, int starsEarned)
		{
			GameStateSaver saver = new GameStateSaver();
			saver.CompleteLevel(levelId, starsEarned);
		}

		/// <summary>
		/// Retrieves the star count for a given level
		/// </summary>
		public async Task<int> GetNumberOfStarForLevel(string levelId)
		{
			return await _loader.GetStarsForLevel(levelId);
		}
	}
}