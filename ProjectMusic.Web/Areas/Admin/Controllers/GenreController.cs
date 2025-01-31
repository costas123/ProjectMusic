﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProjectMusic.Database;
using ProjectMusic.Entities;
using ProjectMusic.Entities.Domain;
using ProjectMusic.Services;
using ProjectMusic.Services.Repositories;

namespace ProjectMusic.Web.Areas.Admin.Controllers
{
    public class GenreController : Controller
    {
        private IUnitOfWork UnitOfWork = new UnitOfWork(new ApplicationDbContext());

        public ActionResult Index(string sortOrder, string searchName, int? pSize, int? page)
        {
            var genres = UnitOfWork.Genres.GetAll();

            //Viewbags
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.CurrentpSize = pSize;
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "NameDesc" : "";

            //Sorting
            switch (sortOrder)
            {
                case "NameDesc": genres = genres.OrderByDescending(x => x.GenreName); break;
                default: genres = genres.OrderBy(x => x.GenreName); break;
            }

            //Filtering First Name
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                genres = genres.Where(x => x.GenreName.ToUpper().Contains(searchName.ToUpper()));
            }

            int pageSize = pSize ?? 5;
            int pageNumber = page ?? 1;

            return View(genres.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Genre genre = UnitOfWork.Genres.Get(id);

            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GenreId,GenreName,GenrePhotoUrl")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork.Genres.Add(genre);
                UnitOfWork.Complete();
                return RedirectToAction("Index");
            }

            return View(genre);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Genre genre = UnitOfWork.Genres.Get(id);

            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GenreId,GenreName,GenrePhotoUrl")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork.Genres.Update(genre);
                UnitOfWork.Complete();
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Genre genre = UnitOfWork.Genres.Get(id);

            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Genre genre = UnitOfWork.Genres.Get(id);
            UnitOfWork.Genres.Remove(genre);
            UnitOfWork.Complete();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
