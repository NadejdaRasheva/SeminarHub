using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Globalization;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext data;

        public SeminarController(SeminarHubDbContext context)
        {
            data = context;
        }
        public async Task<IActionResult> All()
        {
            var seminars = await data.Seminars
                .Select(s => new SeminarInfoViewModel(
                    s.Id,
                    s.Topic,
                    s.Lecturer,
                    s.Category.Name,
                    s.DateAndTime,
                    s.Organizer.UserName))
                .ToListAsync();

            return View(seminars);
        }

        public async Task<IActionResult> Join(int id)
        {
            var s = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (s == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if(!s.SeminarsParticipants.Any(p => p.ParticipantId == userId))
            {
                s.SeminarsParticipants.Add(new SeminarParticipant()
                {
                    SeminarId = s.Id,
                    ParticipantId = userId
                });

                await data.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string userId = GetUserId();

            var model = await data.SeminarsParticipants
                .Where(sp => sp.ParticipantId == userId)
                .AsNoTracking()
                .Select(sp => new SeminarInfoViewModel(
                    sp.SeminarId,
                    sp.Seminar.Topic,
                    sp.Seminar.Lecturer,
                    sp.Seminar.Category.Name,
                    sp.Seminar.DateAndTime,
                    sp.Seminar.Organizer.UserName))
                .ToListAsync();

            return View(model);
        }


        public async Task<IActionResult> Leave(int id)
        {
            var s = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (s == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            var sp = s.SeminarsParticipants
                .FirstOrDefault(sp => sp.ParticipantId == userId);

            if (sp == null)
            {
                return BadRequest();
            }

            s.SeminarsParticipants.Remove(sp);

            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarFormViewModel();
            model.Categories = await GetCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormViewModel model)
        {
            DateTime dateAndTime = DateTime.Now;

            if(!DateTime.TryParseExact(
                model.DateAndTime,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState.AddModelError(nameof(dateAndTime), $"Invalid date! Format is: {DataConstants.DateFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            var seminar = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = dateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserId()
            };

            await data.Seminars.AddAsync(seminar);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var s = await data.Seminars.FindAsync(id);

            if(s == null)
            {
                return BadRequest();
            }

            if(s.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var model = new SeminarFormViewModel()
            {
                Topic = s.Topic,
                Lecturer = s.Lecturer,
                Details = s.Details,
                DateAndTime = s.DateAndTime.ToString(DataConstants.DateFormat),
                Duration = s.Duration,
                CategoryId = s.CategoryId
            };

            model.Categories = await GetCategories();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(SeminarFormViewModel model, int id)
        {
            var s = await data.Seminars.FindAsync(id);

            if (s == null)
            {
                return BadRequest();
            }

            if (s.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            DateTime dateAndTime = DateTime.Now;

            if (!DateTime.TryParseExact(
                model.DateAndTime,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState.AddModelError(nameof(dateAndTime), $"Invalid date! Format is: {DataConstants.DateFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            s.Topic = model.Topic;
            s.Lecturer = model.Lecturer;
            s.Details = model.Details;
            s.DateAndTime = dateAndTime;
            s.Duration = model.Duration;
            s.CategoryId = model.CategoryId;

            await data.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await data.Seminars
                .Where(s => s.Id == id)
                .AsNoTracking()
                .Select(s => new SeminarDetailsViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Details = s.Details,
                    DateAndTime = s.DateAndTime.ToString(DataConstants.DateFormat),
                    Duration = s.Duration,
                    Category = s.Category.Name,
                    Organizer = s.Organizer.UserName  
                })
                .FirstOrDefaultAsync();

            if(model == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await data.Seminars.FindAsync(id);

            if (s == null)
            {
                return BadRequest();
            }

            if (s.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var model = new SeminarDeleteFormViewModel()
            {
                Id = s.Id,
                Topic = s.Topic,
                DateAndTime = s.DateAndTime
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(SeminarDeleteFormViewModel model, int id)
        {
            var s = await data.Seminars.Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (s == null)
            {
                return BadRequest();
            }

            if (s.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var p = await data.SeminarsParticipants.Where(s => s.SeminarId == id).ToListAsync();

            foreach (var pid in p)
            {
                data.SeminarsParticipants.Remove(pid);
            }

            data.Seminars.Remove(s);
            
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }
        //Personal methods
        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            return await data.Categories
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
