using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using ctorx.Core.Data;
using ctorx.Core.Messaging;
using ctorx.Core.Web;

namespace Openbrew.Web.Controllers
{
	[RoutePrefix("brew")]
	public class BrewSessionController : BrewgrController
	{
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;
		readonly IRecipeService RecipeService;
		readonly IUserService UserService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewSessionController(IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory, IRecipeService recipeService, IUserService userService)
		{
			this.UnitOfWorkFactory = unitOfWorkFactory;
			this.RecipeService = recipeService;
			this.UserService = userService;
		}

		/// <summary>
		/// Executes the View for BrewSessionDetail
		/// </summary>
		[Route("{brewsessionid}/{recipename}-brew-session")]
		public ActionResult BrewSessionDetail(int brewSessionId)
		{
			var brewSession = this.RecipeService.GetBrewSessionById(brewSessionId);

			if (brewSession == null)
			{
				return this.Issue404();
			}

			// Auto Redirect to EDIT page for owner
			if(this.ActiveUser != null && brewSession.UserId == this.ActiveUser.UserId && string.IsNullOrWhiteSpace(Request["public"]))
			{
				return this.Redirect(Url.EditBrewSessionUrl(brewSessionId));
			}

			// Ensure only Active Tasting Notes
			brewSession.TastingNotes = brewSession.TastingNotes.Where(x => x.IsActive && x.IsPublic).OrderByDescending(x => x.TasteDate).ToList();

			var brewSessionViewModel = Mapper.Map(brewSession, new BrewSessionViewModel());
			brewSessionViewModel.RecipeSummary = Mapper.Map(this.RecipeService.GetRecipeSummaryById(brewSessionViewModel.RecipeId), new RecipeSummaryViewModel());

			// TODO: Encapsulate into Service and Mapper
			var commentWrapperViewModel = new CommentWrapperViewModel();
			commentWrapperViewModel.CommentViewModels = Mapper.Map(this.RecipeService.GetBrewSessionComments(brewSessionId), new List<CommentViewModel>());
			commentWrapperViewModel.GenericId = brewSessionId;
			commentWrapperViewModel.CommentType = CommentType.Session;
			brewSessionViewModel.CommentWrapperViewModel = commentWrapperViewModel;

			return View("~/Views/BrewSession/BrewSessionDetail.cshtml", brewSessionViewModel);
		}

		/// <summary>
		/// Executes the View for BrewSessionDelete
		/// </summary>
		
		[ForceHttps]
		[Authorize]
		[Route("{brewsessionId}/delete")]
		public ActionResult BrewSessionDelete(int brewSessionId)
		{
			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var brewSession = this.RecipeService.GetBrewSessionById(brewSessionId);

					if (!this.VerifyBrewSessionAccess(brewSession))
					{
						return this.Issue404();
					}

					// Delete the Recipe
					this.RecipeService.DeleteBrewSession(brewSession);

					unitOfWork.Commit();

					this.ForwardMessage(new SuccessMessage { Text = BrewgrMessages.BrewSessionDeleted });

					if (Request.UrlReferrer.ToString().ToLower().Contains("/dashboard"))
					{
						return Redirect("/dashboard#brewsessions");
					}
					else if(Request.UrlReferrer.ToString().ToLower().Contains("/brewsessionedit"))
					{
						return this.Redirect(Url.RecipeEditUrl(brewSession.RecipeId));
					}
					else
					{
						return Redirect(Request.UrlReferrer.ToString());
					}
				}
				catch (Exception ex)
				{
					this.LogHandledException(ex);

					this.AppendMessage(new ErrorMessage { Text = GenericMessages.ErrorMessage });

					unitOfWork.Rollback();

					return this.Issue404();
				}
			}
		}



		/// <summary>
		/// Executes the View for BrewSession
		/// </summary>
		[ForceHttps]
		[Authorize]
		public ActionResult NewBrewSession(int recipeId)
		{
			var recipe = this.RecipeService.GetRecipeById(recipeId);
			var recipeSummary = recipe.RecipeSummary;

			if (!recipeSummary.WasCreatedBy(this.ActiveUser.UserId))
			{
				return this.Issue404();
			}

			var brewSessionViewModel = new BrewSessionViewModel
			{
				BrewDate = DateTime.Now,
				RecipeId = recipeId,
				RecipeSummary = Mapper.Map(recipeSummary, new RecipeSummaryViewModel()),
				UnitTypeId = recipeSummary.UnitTypeId

			};

			// Water Infusion Default Values
			if(recipeSummary.GetRecipeType() != RecipeType.Extract)
			{
				brewSessionViewModel.GrainWeight = recipe.Fermentables.Where(x => x.FermentableUsageTypeId == (int)FermentableUsageType.Mash).Sum(x => x.Amount);
				brewSessionViewModel.BoilVolumeEst = recipe.BoilSize;
				brewSessionViewModel.BoilTime = recipe.BoilTime;
				brewSessionViewModel.FermentVolumeEst = recipe.BatchSize;
			}

			return View("~/Views/BrewSession/NewBrewSession.cshtml", brewSessionViewModel);
		}

		/// <summary>
		/// Executes the View for BrewSessionEdit
		/// </summary>
		[ForceHttps]
		[Authorize]
		public ActionResult BrewSessionEdit(int brewSessionId)
		{
			var brewSession = this.RecipeService.GetBrewSessionById(brewSessionId);

			if (!this.VerifyBrewSessionAccess(brewSession))
			{
				return this.Issue404();
			}

			// Ensure only Active Tasting Notes
			brewSession.TastingNotes = brewSession.TastingNotes.Where(x => x.IsActive && x.IsPublic).OrderByDescending(x => x.TasteDate).ToList();

			var brewSessionViewModel = Mapper.Map(brewSession, new BrewSessionViewModel());
			brewSessionViewModel.RecipeSummary = Mapper.Map(this.RecipeService.GetRecipeSummaryById(brewSessionViewModel.RecipeId), new RecipeSummaryViewModel());

			// TODO: Encapsulate into Service and Mapper
			var commentWrapperViewModel = new CommentWrapperViewModel();
			commentWrapperViewModel.CommentViewModels = Mapper.Map(this.RecipeService.GetBrewSessionComments(brewSessionId), new List<CommentViewModel>());
			commentWrapperViewModel.GenericId = brewSessionId;
			commentWrapperViewModel.CommentType = CommentType.Session;
			brewSessionViewModel.CommentWrapperViewModel = commentWrapperViewModel;

			return View("~/Views/BrewSession/BrewSessionEdit.cshtml", brewSessionViewModel);
		}

		[HttpPost]
		[Authorize]
		[ForceHttps]
		[Route("SaveSession")]
		public ActionResult SaveSession(PostedSessionViewModel postedSessionViewModel)
		{
			// NOTE: This action handles saving for both new sessions
			// and edited sessions.  New Sessions post to this action while
			// edits post to this action via Ajax.

			var isNewSession = false;

			// Hydrate JSON ReceipeViewModel
			var postedSession = postedSessionViewModel.HydrateBrewSessionJson();
			isNewSession = postedSession.BrewSessionId == 0;

			// Verify Recipe Access
			var recipeSummary = this.RecipeService.GetRecipeSummaryById(postedSession.RecipeId);
			if (recipeSummary == null || !recipeSummary.WasCreatedBy(this.ActiveUser.UserId))
			{
				return this.Issue404();
			}

			// Validation (client validates also ... this to ensure data consistency)
			if (postedSession.BrewDate == DateTime.MinValue)
			{
				if (isNewSession)
				{
					this.AppendMessage(new ErrorMessage { Text = "Did you leave something blank?  Please check your entries and try again." });
					return View("~/Views/BrewSession/NewBrewSession.cshtml", new BrewSessionViewModel { RecipeId = postedSession.RecipeId, RecipeSummary = Mapper.Map(recipeSummary, new RecipeSummaryViewModel()) });
				}

				// Signals Invalid
				return this.Content("0");
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var session = postedSession.BrewSessionId == 0 ? new BrewSession() : this.RecipeService.GetBrewSessionById(postedSession.BrewSessionId);

					// Issue 404 if session does not exists or not owned by user
					if (!isNewSession && session.UserId != this.ActiveUser.UserId)
					{
						return this.Issue404();
					}

					// Map Session
					Mapper.Map(postedSession, session);

					if(isNewSession)
					{
						this.RecipeService.AddNewBrewSession(session);
					}
					else
					{
						session.DateModified = DateTime.Now;	
					}
					
					unitOfWork.Commit();

					if (isNewSession)
					{
						this.ForwardMessage(new SuccessMessage { Text = BrewgrMessages.SessionSaved });
						return Redirect(Url.EditBrewSessionUrl(session.BrewSessionId));
					}
					else
					{
						// Signals Success
						return Content("1");
					}
				}
				catch (Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();

					if (isNewSession)
					{
						this.AppendMessage(new ErrorMessage { Text = GenericMessages.ErrorMessage });
						return View("~/Views/BrewSession/NewBrewSession.cshtml", new BrewSessionViewModel { RecipeId = postedSession.RecipeId, RecipeSummary = Mapper.Map(recipeSummary, new RecipeSummaryViewModel()) });
					}
					else
					{
						// Signals Failure
						return Content("-1");
					}
				}
			}
		}

		[HttpPost]
		[Authorize]
		[ForceHttps]
		[Route("SaveTastingNote")]
		public ActionResult SaveTastingNote(TastingNote tastingNoteSubmission)
		{
			if(tastingNoteSubmission == null || (tastingNoteSubmission.RecipeId == null && tastingNoteSubmission.BrewSessionId == null))
			{
				return this.Issue404();
			}

			// Simple Validation
			if(tastingNoteSubmission.Rating < 1 || tastingNoteSubmission.Rating > 5)
			{
				return this.Issue500();
			}

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					TastingNote tastingNote = null;

					if(tastingNoteSubmission.TastingNoteId > 0)
					{
						tastingNote = this.RecipeService.GetTastingNote(tastingNoteSubmission.TastingNoteId);
					}

					if (!this.RecipeService.AllowTastingNoteSubmission(tastingNote ?? tastingNoteSubmission))
					{
						// Someone is trying to game the system.  Ahh ahh ahh
						return this.Issue500();
					}

					tastingNote = Mapper.Map(tastingNoteSubmission, tastingNote);
						
					if(tastingNote.TastingNoteId > 0)
					{
						tastingNote.DateModified = DateTime.Now;
					}
					else
					{
						this.RecipeService.AddNewTastingNote(tastingNote);
					}

					unitOfWork.Commit();

					// Set the User who left the notes
					tastingNote.User = this.UserService.GetUserById(this.ActiveUser.UserId);

					return this.PartialView("~/Views/Recipe/_TastingNoteDetail.cshtml", tastingNote);
				}
				catch(Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();

					return Content("-1");
				}
			}
		}

		[HttpPost]
		[Authorize]
		[ForceHttps]
		[Route("DeleteTastingNote/{tastingNoteId}")]
		public ActionResult DeleteTastingNote(int tastingNoteId)
		{
			if(tastingNoteId <= 0)
			{
				throw new ArgumentOutOfRangeException("tastingNoteId");
			}

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{

				try
				{
					var tastingNote = this.RecipeService.GetTastingNote(tastingNoteId);

					if (tastingNote == null)
					{
						return this.Issue404();
					}

					if(tastingNote.UserId != this.ActiveUser.UserId)
					{
						return this.Issue404();
					}

					this.RecipeService.DeleteTastingNote(tastingNote);
					unitOfWork.Commit();
					return Content("1");
				}
				catch(Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();
					return Content("-1");
				}
			}
		}

		/// <summary>
		/// Verifies Recipe Brew Access
		/// </summary>
		bool VerifyBrewSessionAccess(BrewSession brewSession)
		{
			if (brewSession == null || !brewSession.WasBrewedBy(this.ActiveUser.UserId))
			{
				return false;
			}

			return true;
		}
	}
}
