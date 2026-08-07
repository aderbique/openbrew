using System;
using System.Collections.Generic;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Data;

namespace Openbrew.Web.Core.Service
{
	public interface IRecipeService
	{
		/// <summary>
		/// Gets a recipe by Id
		/// </summary>
		Recipe GetRecipeById(int recipeId);

		/// <summary>
		/// Adds a new Recipe
		/// </summary>
		void AddNewRecipe(Recipe newRecipe);

		/// <summary>
		/// Finalizes a recipe for saving
		/// </summary>
		void FinalizeRecipe(Recipe recipe, bool publishRequested);

		/// <summary>
		/// Marks recipe ingredients for deletion
		/// </summary>
		void MarkRecipeIngredientsForDeletion<TIngredientType>(IEnumerable<IRecipeIngredient> recipeIngredients) where TIngredientType : class, IRecipeIngredient;

		/// <summary>
		/// Marks Recipe Steps for deletion
		/// </summary>
		void MarkRecipeStepsForDeletion(List<RecipeStep> stepsForDeletion);

		/// <summary>
		/// Gets a list of user Recipes
		/// </summary>
		IList<RecipeSummary> GetUserRecipes(int userId);

		/// <summary>
		/// Gets a list of recent recipes
		/// </summary>
		IList<RecipeSummary> GetRecentRecipesCached(int numberToReturn);

        /// <summary>
		/// Gets a list of recent recipes
		/// </summary>
		IList<RecipeSummary> GetNewestRecipes(int numberToReturn);

		/// <summary>
		/// Gets all Recipes.  Use with caution.
		/// </summary>
		IList<Recipe> GetAllRecipes();

        /// <summary>
        /// Adds a new RecipeComment
        /// </summary>
        void AddRecipeComment(RecipeComment recipeComment);

        /// <summary>
        /// Adds a new BrewSessionComment
        /// </summary>
        void AddBrewSessionComment(BrewSessionComment brewSessionComment);

        /// <summary>
		/// Deletes a Recipe
		/// </summary>
		void DeleteRecipe(Recipe recipe);

		/// <summary>
		/// Edits a Recipe
		/// </summary>
		void EditRecipe(Recipe recipe);

        /// <summary>
        /// Gets the Comments for a Recipe
        /// </summary>
        IList<RecipeCommentSummary> GetRecipeComments(int recipeId);

        /// <summary>
        /// Gets the Comments for a BrewSession
        /// </summary>
        IList<BrewSessionComment> GetBrewSessionComments(int brewSessionId);

        /// <summary>
		/// Gets a RecipeSummary by Id
		/// </summary>
		RecipeSummary GetRecipeSummaryById(int recipeId);

		/// <summary>
		/// Adds a new Recipe Brew
		/// </summary>
		void AddNewBrewSession(BrewSession brewSession);

		/// <summary>
		/// Gets a BrewSession by Id
		/// </summary>
		BrewSession GetBrewSessionById(int brewSessionId);

		/// <summary>
		/// Adds a new BrewSessionTastingNote
		/// </summary>
		void AddNewTastingNote(TastingNote tastingNote);

		/// <summary>
		/// Gets a list of user recipe brews
		/// </summary>
		IList<BrewSessionSummary> GetUserBrewSessions(int userId);

		/// <summary>
		/// Deletes a BrewSession
		/// </summary>
		void DeleteBrewSession(BrewSession brewSession);

		/// <summary>
		/// Gets a list of Recipe Brew Summaries for a Recipe
		/// </summary>
		IList<BrewSessionSummary> GetRecipeBrewSessions(int recipeId);

		/// <summary>
		/// Deletes a brewSessionTastingNote
		/// </summary>
		void DeleteTastingNote(TastingNote tastingNote);

		/// <summary>
		/// Gets the count of Brew Sessions (BrewSession) for a Recipe
		/// </summary>
		/// <returns></returns>
		int GetRecipeBrewSessionsCount(int recipeId);

		/// <summary>
		/// Gets all recipe brew summaries
		/// </summary>
		IList<BrewSessionSummary> GetAllBrewSessionSummaries();

		/// <summary>
		/// Gets a list of similar recipes
		/// </summary>
		IList<RecipeSummary> GetSimilarRecipes(Recipe recipe, int count);

		/// <summary>
		/// Gets a list of the most popular recipes
		/// </summary>
		IList<RecipeSummary> GetPopularRecipes(int count);

        /// <summary>
        ///  Gets the object ids for the dashboard
        /// </summary>
        DashboardItemHolder GetDashboardItems(int userId, DateTime searchOlderThan, int numberToReturn);

		/// <summary>
		/// Gets the appropriate recipe creation options
		/// </summary>
		RecipeCreationOptions GetRecipeCreationOptions();

		/// <summary>
		/// Gets the most recent brew session
		/// </summary>
		int? GetMostRecentBrewSession(int recipeId);

		/// <summary>
		/// Gets a list of tasting notes for a recipe
		/// </summary>
		IList<TastingNote> GetRecipeTastingNotes(int recipeId);

		/// <summary>
		/// Gets a tasting note
		/// </summary>
		TastingNote GetTastingNote(int tastingNoteId);

		/// <summary>
		/// Checks is the current users is allowed to perform a tasting note submission
		/// </summary>
		bool AllowTastingNoteSubmission(TastingNote tastingNote);

		/// <summary>
		/// Gets a list of recipes that appear to be clones of a popular recipes
		/// </summary>
		IList<RecipeSummary> GetPopularRecipeClones(string cloneNameLoookup);
	}
}
