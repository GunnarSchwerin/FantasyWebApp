using FantasyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace FantasyWebApp.Controllers
{

    // Replace FantasyDbContext with the name of your existing EF DbContext.
    public class DraftController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            var draft = await db.Drafts
                .OrderByDescending(d => d.Id)
                .FirstOrDefaultAsync();

            if (draft == null)
                return View("Empty");

            var teams = await db.DraftTeams
                .Where(t => t.DraftId == draft.Id)
                .OrderBy(t => t.DraftPosition)
                .ToListAsync();

            var picks = await db.DraftPicks
                .Include(p => p.CurrentTeam)
                .Include(p => p.DraftPlayer)
                .Where(p => p.DraftId == draft.Id)
                .OrderBy(p => p.OverallPick)
            .ToListAsync();

            ViewBag.Draft = draft;
            ViewBag.Teams = teams;
            ViewBag.Picks = picks;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Settings(int? id)
        {
            Draft draft;
            if (id.HasValue)
                draft = await db.Drafts.FindAsync(id.Value);
            else
                draft = await db.Drafts.OrderByDescending(d => d.Id).FirstOrDefaultAsync();

            if (draft == null)
                draft = new Draft();

            var rules = await db.LeagueScoringRules.FirstOrDefaultAsync(r => r.DraftId == draft.Id);

            ViewBag.Rules = rules ?? new LeagueScoringRules();
            return View(draft);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveSettings(Draft draft, LeagueScoringRules rules)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Rules = rules;
                return View("Settings", draft);
            }

            if (draft.Id == 0)
            {
                draft.Status = "Setup";
                db.Drafts.Add(draft);
                await db.SaveChangesAsync();
                rules.DraftId = draft.Id;
                db.LeagueScoringRules.Add(rules);
            }
            else
            {
                var existing = await db.Drafts.FindAsync(draft.Id);
                if (existing == null) return HttpNotFound();
                if (existing.Status != "Setup")
                {
                    ModelState.AddModelError("", "Settings can only be changed before the draft starts.");
                    ViewBag.Rules = rules;
                    return View("Settings", draft);
                }

                existing.Name = draft.Name;
                existing.TeamCount = draft.TeamCount;
                existing.RoundCount = draft.RoundCount;
                existing.IsSnake = true;

                var existingRules = await db.LeagueScoringRules
                    .FirstOrDefaultAsync(r => r.DraftId == existing.Id);

                if (existingRules == null)
                {
                    rules.DraftId = existing.Id;
                    db.LeagueScoringRules.Add(rules);
                }
                else
                {
                    // Copy submitted scoring values onto the tracked rule record.
                    db.Entry(existingRules).CurrentValues.SetValues(rules);
                    existingRules.Id = existingRules.Id;
                    existingRules.DraftId = existing.Id;
                }
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateSnakePicks(int draftId)
        {
            var draft = await db.Drafts.FindAsync(draftId);
            if (draft == null) return HttpNotFound();
            if (draft.Status != "Setup")
                return new HttpStatusCodeResult(400, "Draft has already started.");

            var teams = await db.DraftTeams
                .Where(t => t.DraftId == draftId)
                .OrderBy(t => t.DraftPosition)
                .ToListAsync();

            if (teams.Count != draft.TeamCount)
                return new HttpStatusCodeResult(400, "Create the configured number of teams first.");

            var oldPicks = db.DraftPicks.Where(p => p.DraftId == draftId);
            db.DraftPicks.RemoveRange(oldPicks);
            await db.SaveChangesAsync();

            int overall = 1;
            for (int round = 1; round <= draft.RoundCount; round++)
            {
                var order = round % 2 == 1
                    ? teams
                    : teams.AsEnumerable().Reverse();

                int pickInRound = 1;
                foreach (var team in order)
                {
                    db.DraftPicks.Add(new DraftPick
                    {
                        DraftId = draftId,
                        Round = round,
                        PickInRound = pickInRound++,
                        OverallPick = overall++,
                        OriginalTeamId = team.Id,
                        CurrentTeamId = team.Id
                    });
                }
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StartDraft(int draftId)
        {
            var draft = await db.Drafts.FindAsync(draftId);
            if (draft == null) return HttpNotFound();
            if (draft.Status != "Setup")
                return new HttpStatusCodeResult(400, "Draft is not in setup.");

            if (!await db.DraftPicks.AnyAsync(p => p.DraftId == draftId))
                return new HttpStatusCodeResult(400, "Generate the draft order first.");

            draft.Status = "Live";
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> AvailablePlayers(string search, string position)
        {
            var query = db.DraftPlayers.Where(p => p.IsAvailable);

            if (!String.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.NflTeam.Contains(search));

            if (!String.IsNullOrWhiteSpace(position))
                query = query.Where(p => p.Position == position);

            var players = await query.OrderBy(p => p.Position)
                .ThenBy(p => p.Name).ToListAsync();

            return View(players);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MakePick(int pickId, int playerId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var pick = await db.DraftPicks
                    .FirstOrDefaultAsync(p => p.Id == pickId);
                var player = await db.DraftPlayers
                    .FirstOrDefaultAsync(p => p.Id == playerId);

                if (pick == null || player == null) return HttpNotFound();

                var draft = await db.Drafts.FindAsync(pick.DraftId);
                if (draft == null || draft.Status != "Live")
                    return new HttpStatusCodeResult(400, "Draft is not live.");

                if (pick.IsCompleted || !player.IsAvailable)
                    return new HttpStatusCodeResult(400, "Pick is already complete or player is unavailable.");

                var nextPick = await db.DraftPicks
                    .Where(p => p.DraftId == pick.DraftId && !p.IsCompleted)
                    .OrderBy(p => p.OverallPick)
                    .FirstOrDefaultAsync();

                if (nextPick == null || nextPick.Id != pick.Id)
                    return new HttpStatusCodeResult(400, "This is not the current pick.");

                pick.DraftPlayerId = player.Id;
                pick.IsCompleted = true;
                player.IsAvailable = false;

                if (!String.IsNullOrWhiteSpace(pick.KeeperPlayerName))
                    pick.KeeperPlayerName = null;

                if (!await db.DraftPicks.AnyAsync(p => p.DraftId == pick.DraftId && !p.IsCompleted))
                    draft.Status = "Complete";

                await db.SaveChangesAsync();
                transaction.Commit();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

    }
}