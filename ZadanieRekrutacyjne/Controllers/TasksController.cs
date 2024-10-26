using Microsoft.AspNetCore.Mvc;
using ZadanieRekrutacyjne.Models;

namespace ZadanieRekrutacyjne.Controllers;

public class TasksController : Controller
{
    /// <summary>
    /// This return all Tasks to Index View
    /// </summary>
    /// <returns>View(Query)</returns>
    public IActionResult Index() 
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var Query = session.Query<Tasks>().ToList();
            return View(Query);
        }
    }
    /// <summary>
    /// This returns tasks for today 
    /// </summary>
    /// <returns>Today task</returns>
    public IActionResult IndexToday()
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var Query = session.Query<Tasks>().Where(x => x.Deadline.Date == DateTime.Today.Date).ToList();
            return View(Query);
        }
    }
    /// <summary>
    /// Returns tasks for tomorrow 
    /// </summary>
    /// <returns>Tasks for tomorrow</returns>
    public IActionResult IndexTommorow()
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var query = session.Query<Tasks>()
                .Where(x => x.Deadline.Date == tomorrow.Date)
                .ToList();
            return View(query);
        }
    }
    /// <summary>
    /// Returns tasks in this week
    /// </summary>
    /// <returns>Tasks in this week</returns>
    public IActionResult IndexCurrentWeek()
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var query = session.Query<Tasks>()
                .Where(x => x.Deadline.Date >= startOfWeek && x.Deadline.Date < endOfWeek)
                .ToList();
            return View(query);
        }
    }
    /// <summary>
    /// Marks task as done. Sets his tag and progress.
    /// </summary>
    /// <param name="Id">Id of the task</param>
    /// <returns>Redirection to Index</returns>
    public IActionResult MarkAsDone(Guid Id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                Tasks task = session.Query<Tasks>().Where(x => x.Id == Id).FirstOrDefault();
                task.Tag = "Done";
                task.Progress = 100;
                session.Save(task);
                transaction.Commit();
                return RedirectToAction("Index");
            }
        }
    }
    /// <summary>
    /// Returns create view
    /// </summary>
    /// <returns>Create view</returns>
    public IActionResult Create()
    {
        return View();
    }
    /// <summary>
    /// Create new task
    /// </summary>
    /// <param name="NewTask">NewTask to be created</param>
    /// <returns>Redirection to index</returns>
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
    /// <summary>
    /// Edit task
    /// </summary>
    /// <param name="EditTask">Task with edited parameters</param>
    /// <returns>Redirection to index</returns>
    [HttpPost]
    public IActionResult Edit(Tasks EditTask)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                EditTask.CreateTime = session.Query<Tasks>().Where(x => x.Id == EditTask.Id).Select(x => x.CreateTime).FirstOrDefault();
                session.Merge(EditTask); 
                transaction.Commit();

                return RedirectToAction("Index");
            }
        }
    }
    /// <summary>
    /// Create view and pass task to be edited
    /// </summary>
    /// <param name="Id">Id of the task to be edited</param>
    /// <returns>Edit view with passed task in arg</returns>
    public IActionResult Edit(Guid Id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var Query = session.Query<Tasks>().Where(x => x.Id == Id).FirstOrDefault();
            return View(Query);
        }
        
    }
    /// <summary>
    /// Deletes task
    /// </summary>
    /// <param name="Id">Id of the task to be deleted</param>
    /// <returns>Redirect to index</returns>
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
/// <summary>
/// Shows details of selected Task
/// </summary>
/// <param name="Id">Selected task</param>
/// <returns>View with details of the task</returns>
    public IActionResult Details(Guid Id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var Query = session.Query<Tasks>().Where(x => x.Id == Id).FirstOrDefault();
            return View(Query);
        }
    }
    
}