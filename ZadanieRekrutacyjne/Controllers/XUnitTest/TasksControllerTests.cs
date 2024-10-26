using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for Task
using Microsoft.AspNetCore.Mvc;
using ZadanieRekrutacyjne.Controllers;
using ZadanieRekrutacyjne.Models;
using Xunit;

namespace ZadanieRekrutacyjne.Tests
{
    public class TasksControllerTests
    {
        /// <summary>
        /// Tests the Index action to ensure it returns a ViewResult with a list of Tasks.
        /// </summary>
        [Fact]
        public void Index_ReturnsViewResult_WithListOfTasks()
        {
            // Arrange
            var controller = new TasksController();

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Tasks>>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            // Optionally, check if the list contains expected items
        }

        /// <summary>
        /// Tests the IndexToday action to ensure it returns tasks with today's deadline.
        /// </summary>
        [Fact]
        public void IndexToday_ReturnsViewResult_WithTodayTasks()
        {
            // Arrange
            var controller = new TasksController();

            // Act
            var result = controller.IndexToday();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Tasks>>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            Assert.All(model, task => Assert.Equal(DateTime.Today.Date, task.Deadline.Date));
        }

        /// <summary>
        /// Tests the IndexTommorow action to ensure it returns tasks with tomorrow's deadline.
        /// </summary>
        [Fact]
        public void IndexTomorrow_ReturnsViewResult_WithTomorrowTasks()
        {
            // Arrange
            var controller = new TasksController();

            // Act
            var result = controller.IndexTommorow(); // Ensure the spelling matches the controller's method

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Tasks>>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            var tomorrow = DateTime.Today.AddDays(1).Date;
            Assert.All(model, task => Assert.Equal(tomorrow, task.Deadline.Date));
        }

        /// <summary>
        /// Tests the IndexCurrentWeek action to ensure it returns tasks within the current week.
        /// </summary>
        [Fact]
        public void IndexCurrentWeek_ReturnsViewResult_WithCurrentWeekTasks()
        {
            // Arrange
            var controller = new TasksController();

            // Act
            var result = controller.IndexCurrentWeek();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Tasks>>(viewResult.ViewData.Model);
            Assert.NotNull(model);

            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;

            Assert.All(model, task =>
            {
                Assert.True(task.Deadline.Date >= startOfWeek && task.Deadline.Date < endOfWeek);
            });
        }

        /// <summary>
        /// Tests the Create action (GET) to ensure it returns the Create view.
        /// </summary>
        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Arrange
            var controller = new TasksController();

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Tests the Create action (POST) to ensure it creates a new task and redirects to Index.
        /// </summary>
        [Fact]
        public async Task Create_Post_ValidTask_RedirectsToIndex()
        {
            // Arrange
            var controller = new TasksController();
            var newTask = new Tasks
            {
                // Initialize necessary properties except Id and CreateTime
                Title = "Test Task",
                Description = "This is a test task",
                Deadline = DateTime.Today.AddDays(2),
                Tag = "Pending",
                Progress = 0
            };

            // Act
            var result = await controller.Create(newTask);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

           
            using (var session = NHibernateHelper.OpenSession())
            {
                var createdTask = session.Query<Tasks>().FirstOrDefault(t => t.Id == newTask.Id);
                Assert.NotNull(createdTask);
                Assert.Equal(newTask.Deadline, createdTask.Deadline);
                Assert.Equal("Pending", createdTask.Tag);
                Assert.Equal(0, createdTask.Progress);
            }
        }

        /// <summary>
        /// Tests the Edit action (GET) to ensure it returns the Edit view with the correct task.
        /// </summary>
        [Fact]
        public void Edit_Get_ExistingId_ReturnsViewResult_WithTask()
        {
            // Arrange
            var controller = new TasksController();
            var existingTask = CreateTestTask();

            // Act
            var result = controller.Edit(existingTask.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Tasks>(viewResult.ViewData.Model);
            Assert.Equal(existingTask.Id, model.Id);
        }

        /// <summary>
        /// Tests the Edit action (POST) to ensure it updates the task and redirects to Index.
        /// </summary>
        [Fact]
        public void Edit_Post_ValidTask_RedirectsToIndex()
        {
            // Arrange
            var controller = new TasksController();
            var existingTask = CreateTestTask();
            existingTask.Title = "Updated Title";

            // Act
            var result = controller.Edit(existingTask);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify the task was updated in the database
            using (var session = NHibernateHelper.OpenSession())
            {
                var updatedTask = session.Query<Tasks>().FirstOrDefault(t => t.Id == existingTask.Id);
                Assert.NotNull(updatedTask);
                Assert.Equal("Updated Title", updatedTask.Title);
            }
        }

        /// <summary>
        /// Tests the Delete action to ensure it deletes the task and redirects to Index.
        /// </summary>
        [Fact]
        public void Delete_Post_ExistingId_RedirectsToIndex()
        {
            // Arrange
            var controller = new TasksController();
            var taskToDelete = CreateTestTask();

            // Act
            var result = controller.Delete(taskToDelete.Id);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify the task was deleted from the database
            using (var session = NHibernateHelper.OpenSession())
            {
                var deletedTask = session.Query<Tasks>().FirstOrDefault(t => t.Id == taskToDelete.Id);
                Assert.Null(deletedTask);
            }
        }

        /// <summary>
        /// Tests the Details action to ensure it returns the Details view with the correct task.
        /// </summary>
        [Fact]
        public void Details_ExistingId_ReturnsViewResult_WithTask()
        {
            // Arrange
            var controller = new TasksController();
            var existingTask = CreateTestTask();

            // Act
            var result = controller.Details(existingTask.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Tasks>(viewResult.ViewData.Model);
            Assert.Equal(existingTask.Id, model.Id);
        }

        /// <summary>
        /// Helper method to create a test task in the database.
        /// </summary>
        /// <returns>The created Tasks object.</returns>
        private Tasks CreateTestTask()
        {
            var testTask = new Tasks
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "This is a test task",
                Deadline = DateTime.Today.AddDays(3),
                Tag = "Pending",
                Progress = 0,
                CreateTime = DateTime.Now
            };

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(testTask);
                    transaction.Commit();
                }
            }

            return testTask;
        }
    }
}
