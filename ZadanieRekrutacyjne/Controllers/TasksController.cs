using Microsoft.AspNetCore.Mvc;
using ZadanieRekrutacyjne.Models;

namespace ZadanieRekrutacyjne.Controllers;

public class TasksController : Controller
{
    public IActionResult Index()
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var Query = session.Query<Tasks>().ToList();
            return View(Query);
        }
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task <IActionResult> Create(Tasks NewTask)
    {
        NewTask.Id = Guid.NewGuid();
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                NewTask.CreateTime=DateTime.Now;
                session.Save(NewTask);
                transaction.Commit();
            }
        }
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Edit(Tasks EditTask)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                EditTask.CreateTime = session.Query<Tasks>().Where(x => x.Id == EditTask.Id).Select(x => x.CreateTime).FirstOrDefault();
                session.Merge(EditTask);  // Merge instead of SaveOrUpdate
                transaction.Commit();

                return RedirectToAction("Index");
            }
        }
    }
    public IActionResult Edit(Guid Id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var Query = session.Query<Tasks>().Where(x => x.Id == Id).FirstOrDefault();
            return View(Query);
        }
        
    }
    [HttpPost]
    public IActionResult Delete(Guid Id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var Query = session.Query<Tasks>().Where(x => x.Id == Id).FirstOrDefault();
                session.Delete(Query);
                transaction.Commit();
                return RedirectToAction("Index");
                RedirectToAction("Index");
            }
        }
    }

    public IActionResult Details(Guid Id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var Query = session.Query<Tasks>().Where(x => x.Id == Id).FirstOrDefault();
            return View(Query);
        }
    }
    
}