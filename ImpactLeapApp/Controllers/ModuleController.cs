using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models.OrderModels;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ImpactLeapApp.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class ModuleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public ModuleController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Module
        public async Task<IActionResult> Index()
        {
            return View(await _context.Modules.ToListAsync());
        }

        // GET: Module/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModule = await _context.Modules
                .SingleOrDefaultAsync(m => m.ModuleId == id);
            if (orderModule == null)
            {
                return NotFound();
            }

            List<string> moduleSampleNameList = new List<string>();

            var moduleSampleNames = "";
            if (_context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSampleName != null)
            {
                moduleSampleNames = _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSampleName;
            }

            ViewBag.ModuleSampleNameList = moduleSampleNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return View(orderModule);
        }

        // GET: Module/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Module/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleId,ModuleName,Description,UnitPrice,ModuleSampleName,ModuleSamplePath")] Module orderModule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderModule);
                _context.SaveChanges();

                // Save module sample to database
                var files = HttpContext.Request.Form.Files;
                var tempModuleId = orderModule.ModuleId;

                string uploadPath = Path.Combine(_environment.WebRootPath, "images/moduleSamples/" + tempModuleId);
                string uploadPathLink = HttpContext.Request.Scheme + "://" +
                                        HttpContext.Request.Host + "/images/moduleSamples/" + tempModuleId + "/";

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (IFormFile file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, file.FileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    _context.Modules.SingleOrDefault(m => m.ModuleId == tempModuleId).ModuleSampleName += file.FileName + ",";
                }

                _context.Modules.SingleOrDefault(m => m.ModuleId == tempModuleId).ModuleSamplePath = uploadPathLink;
                _context.Modules.SingleOrDefault(m => m.ModuleId == tempModuleId).ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(orderModule);
        }

        // GET: Module/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModule = await _context.Modules.SingleOrDefaultAsync(m => m.ModuleId == id);
            if (orderModule == null)
            {
                return NotFound();
            }

            List<string> moduleSampleNameList = new List<string>();

            var moduleSampleNames = "";
            if (_context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSampleName != null)
            {
                moduleSampleNames = _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSampleName;
            }

            ViewBag.ModuleSampleNameList = moduleSampleNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return View(orderModule);
        }

        // POST: Module/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModuleId,ModuleName,Description,UnitPrice,ModuleSampleName,ModuleSamplePath")] Module module)
        {
            if (id != module.ModuleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                string uploadPath = Path.Combine(_environment.WebRootPath, "images/moduleSamples/" + id);
                string uploadPathLink = HttpContext.Request.Scheme + "://" +
                                        HttpContext.Request.Host + "/images/moduleSamples/" + id + "/";

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (IFormFile file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, file.FileName), FileMode.Append))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSampleName += file.FileName + ",";
                }

                _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSamplePath = uploadPathLink;
                _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleName = module.ModuleName;
                _context.Modules.SingleOrDefault(m => m.ModuleId == id).Description = module.Description;
                _context.Modules.SingleOrDefault(m => m.ModuleId == id).UnitPrice = module.UnitPrice;
                _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = id });
        }

        // GET: Module/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModule = await _context.Modules
                .SingleOrDefaultAsync(m => m.ModuleId == id);
            if (orderModule == null)
            {
                return NotFound();
            }

            return View(orderModule);
        }

        // POST: Module/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string uploadPath = Path.Combine(_environment.WebRootPath, "images/moduleSamples/" + id);
            if (Directory.Exists(uploadPath))
            {
                DeleteDirectory(uploadPath);
            }

            var orderModule = await _context.Modules.SingleOrDefaultAsync(m => m.ModuleId == id);
            _context.Modules.Remove(orderModule);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OrderModuleExists(int id)
        {
            return _context.Modules.Any(e => e.ModuleId == id);
        }

        private void DeleteDirectory(string path)
        {
            // Delete all files from the Directory
            foreach (string filename in Directory.GetFiles(path))
            {
                System.IO.File.Delete(filename);
            }

            // Check all child Directories and delete files
            foreach (string subfolder in Directory.GetDirectories(path))
            {
                DeleteDirectory(subfolder);
            }

            Directory.Delete(path);
        }

        public void DeleteModuleSample(string moduleName, int id)
        {
            string[] moduleSampleNameList = new string[] { };
            var moduleSampleName = _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSampleName;
            if (moduleSampleName != null)
                moduleSampleNameList = moduleSampleName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var tempModuleList = moduleSampleNameList.ToList();
            var tempModuleName = "";

            foreach (var name in moduleSampleNameList)
            {
                if (String.Equals(name, moduleName))
                {
                    tempModuleList.Remove(name);
                }
            }

            foreach (var temp in tempModuleList)
            {
                tempModuleName += temp + ",";
            }

            _context.Modules.SingleOrDefault(m => m.ModuleId == id).ModuleSampleName = tempModuleName;
            _context.SaveChanges();
        }
    }
}
