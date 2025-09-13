using System;
using System.Linq;
using System.Threading.Tasks;
using Burg_Storage.Models;
using Burg_Storage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Burg_Storage.Controllers
{
    /// <summary>
    /// MVC controller for managing documents and their versions.
    /// </summary>
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IFileStorageService _fileStorage;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentsController(IDocumentService documentService, IFileStorageService fileStorage, UserManager<ApplicationUser> userManager)
        {
            _documentService = documentService;
            _fileStorage = fileStorage;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentsController(IDocumentService documentService, UserManager<ApplicationUser> userManager)
        {
            _documentService = documentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var docs = await _documentService.ListByOwnerAsync(userId);
            return View(docs);
        }

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string name, AccessLevel accessLevel, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(name) || file == null)
            {
                ModelState.AddModelError(string.Empty, "Name and file are required.");
                return View();
            }

            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            await _documentService.CreateAsync(name, userId, accessLevel, file);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddVersion(int id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var doc = (await _documentService.ListByOwnerAsync(userId)).FirstOrDefault(d => d.Id == id);
            if (doc == null) return NotFound();
            ViewBag.DocumentId = id;
            ViewBag.DocumentName = doc.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVersion(int id, IFormFile file)
        {
            if (file == null)
            {
                ModelState.AddModelError(string.Empty, "File is required.");
                ViewBag.DocumentId = id;
                return View();
            }

            await _documentService.AddVersionAsync(id, file);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _fileStorage.GetAsync(id);
            if (file == null)
                return NotFound();
            var (stream, contentType, downloadFileName) = file.Value;
            return File(stream, contentType, downloadFileName);
        }
    }
}
